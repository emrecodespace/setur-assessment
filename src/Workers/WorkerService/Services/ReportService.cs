using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using WorkerService.Apis;
using WorkerService.Apis.Dtos;
using WorkerService.Dtos;
using WorkerService.Interfaces;
using WorkerService.Options;

namespace WorkerService.Services;

public class ReportService(IContactApi contactApi, IContactReportApi contactReportApi,
    IOptions<RabbitMqOptions> rabbitOptions) : IReportService, IAsyncDisposable
{
    private IChannel? _channel;
    private IConnection? _connection;
    private readonly RabbitMqOptions _options = rabbitOptions.Value;
    private bool _disposed;
    
    public async Task Run(CancellationToken ct = default)
    {
        _connection = await GetConnection();
        _channel = await _connection.CreateChannelAsync(cancellationToken: ct);
        await _channel.QueueDeclareAsync(queue: _options.QueueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null, cancellationToken: ct);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += OnConsumerOnReceivedAsync;
        await _channel.BasicConsumeAsync(
            queue: _options.QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: ct);
    }

    private async Task OnConsumerOnReceivedAsync(object model, BasicDeliverEventArgs ea)
    {
        try
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var reportMessageModel = JsonSerializer.Deserialize<ReportRequest>(message);
            
            var reportDetailsResponse = await contactApi.GetReport();
            var reportDetails = reportDetailsResponse
                .Select(p => new ReportDetailsRequest(p.Location, p.PersonCount, p.PhoneCount));
            
            await contactReportApi.SendReportDetails(reportMessageModel!.Id, reportDetails);
            await _channel!.BasicAckAsync(ea.DeliveryTag, false);
        }
        catch (Exception)
        {
            await _channel!.BasicNackAsync(ea.DeliveryTag, false, true);
        }
    }

    private async Task<IConnection> GetConnection()
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                UserName = _options.UserName,
                Password = _options.Password,
                Port = _options.Port,
                AutomaticRecoveryEnabled = true
            };
            return await factory.CreateConnectionAsync();
        }
        catch (BrokerUnreachableException)
        {
            await Task.Delay(5000);
            return await GetConnection();
        }
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;

        if (_channel is IAsyncDisposable asyncChannel)
            await asyncChannel.DisposeAsync();
        else
            _channel?.Dispose();

        if (_connection is IAsyncDisposable asyncConn)
            await asyncConn.DisposeAsync();
        else
            _connection?.Dispose();
    }
}