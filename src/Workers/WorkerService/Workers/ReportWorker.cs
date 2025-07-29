using WorkerService.Interfaces;

namespace WorkerService.Workers;

public class ReportWorker(IServiceProvider serviceProvider) : BackgroundService
{
    private IServiceScope _scope = null!;
    private IReportService _reportService = null!;

    public override Task StartAsync(CancellationToken ct)
    {
        Console.WriteLine($"{nameof(ReportWorker)} has started.");
        _scope = serviceProvider.CreateScope();
        _reportService = _scope.ServiceProvider.GetRequiredService<IReportService>();

        return base.StartAsync(ct);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        => await _reportService.Run(stoppingToken);

    public override Task StopAsync(CancellationToken ct)
    {
        Console.WriteLine($"{nameof(ReportWorker)} was stopped.");
        return base.StopAsync(ct);
    }
}