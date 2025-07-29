namespace WorkerService.Interfaces;

public interface IReportService
{
    Task Run(CancellationToken ct = default);
}