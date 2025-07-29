using ContactReports.Application.Interfaces;
using ContactReports.Infrastructure.Options;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace ContactReports.Infrastructure.Services;

public class ReportQueueService (IOptions<RabbitMqOptions> rabbitMqOptions): IReportQueueService
{
    private readonly RabbitMqOptions _options = rabbitMqOptions.Value;

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
                AutomaticRecoveryEnabled = true,
            };
            return await factory.CreateConnectionAsync();
        }
        catch (BrokerUnreachableException)
        {
            await Task.Delay(5000);
            return await GetConnection();
        }
    }

    public async Task Send<T>(T obj)
    {
        await using var connection = await GetConnection();
        await using var channel = await connection.CreateChannelAsync();
        await channel.QueueDeclareAsync(queue: _options.QueueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        var message = System.Text.Json.JsonSerializer.Serialize(obj);
        var body = System.Text.Encoding.UTF8.GetBytes(message);

        var properties = new BasicProperties
        {
            Persistent = false
        };

        await channel.BasicPublishAsync(exchange: string.Empty,
            routingKey: _options.QueueName,
            mandatory: false,
            basicProperties: properties,
            body: body);
    }
}