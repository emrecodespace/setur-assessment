namespace ContactReports.Application.Interfaces;

public interface IReportQueueService
{
    Task Send<T>(T obj);
}