using System.Linq.Expressions;
using ContactReports.Domain.Entities;

namespace ContactReports.Application.Interfaces;

public interface IReportRepository
{
    Task<Report> AddAsync(Report report, CancellationToken ct = default);
    Task<Report?> GetAsync(Expression<Func<Report, bool>> predicate, CancellationToken ct = default);
    Task<IEnumerable<ReportDetail>> GetReportDetailsByIdAsync(Guid reportId, CancellationToken ct = default);
    Task<List<Report>> GetAllAsync(CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}