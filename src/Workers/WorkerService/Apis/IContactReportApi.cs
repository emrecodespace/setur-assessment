using Refit;
using WorkerService.Dtos;

namespace WorkerService.Apis;

public interface IContactReportApi
{
    [Post("/reports/{id}/details")]
    Task SendReportDetails(Guid id, [Body] IEnumerable<ReportDetailsRequest> details,
        CancellationToken ct = default);
}