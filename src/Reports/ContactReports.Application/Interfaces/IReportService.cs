using ContactReports.Application.Common;
using ContactReports.Application.Dtos;

namespace ContactReports.Application.Interfaces;

public interface IReportService
{
    Task<Result<IEnumerable<ReportDto>>> GetReports(CancellationToken ct);
    Task<Result<IEnumerable<ReportDetailsDto>>> GetReportDetails(Guid id, CancellationToken ct);
    Task<Result> CreateReport(CancellationToken ct);
    Task<Result> AddReportDetails(Guid id, IEnumerable<ReportDetailsRequest> details, CancellationToken ct);

}