using Contacts.Application.Common;
using Contacts.Application.Dtos;

namespace Contacts.Application.Interfaces;

public interface IReportService
{
    Task<Result<List<ReportDto>>> GetReport(CancellationToken ct = default);
}