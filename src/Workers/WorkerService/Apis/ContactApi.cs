using Refit;
using WorkerService.Apis.Dtos;

namespace WorkerService.Apis;

public interface IContactApi
{
    [Get("/reports")]
    Task<IEnumerable<ReportDetailsDto>> GetReport(CancellationToken ct = default);
}