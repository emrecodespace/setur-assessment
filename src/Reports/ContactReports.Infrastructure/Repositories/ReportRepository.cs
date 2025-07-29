using System.Linq.Expressions;
using ContactReports.Application.Interfaces;
using ContactReports.Domain.Entities;
using ContactReports.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace ContactReports.Infrastructure.Repositories;

public class ReportRepository(ReportDbContext reportDbContext) : IReportRepository
{
    private readonly ReportDbContext _reportDbContext =
        reportDbContext ?? throw new ArgumentNullException(nameof(reportDbContext));
    
    public async Task<Report> AddAsync(Report report, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(report);

        await _reportDbContext.Reports.AddAsync(report, ct);
        await _reportDbContext.SaveChangesAsync(ct);
        return report;
    }
    
    public async Task<Report?> GetAsync(Expression<Func<Report, bool>> predicate, CancellationToken ct = default)
        => await _reportDbContext.Reports.FirstOrDefaultAsync(predicate, ct);

    public async Task<IEnumerable<ReportDetail>> GetReportDetailsByIdAsync(Guid reportId,
        CancellationToken ct = default)
        => await _reportDbContext.ReportDetails
            .AsNoTracking()
            .Where(r => r.ReportId == reportId)
            .ToListAsync(ct);

    public async Task<List<Report>> GetAllAsync(CancellationToken ct = default)
    {
        return await _reportDbContext.Reports
            .AsNoTracking()
            .OrderByDescending(r => r.RequestedAt)
            .ToListAsync(ct);
    }
    
    public async Task SaveChangesAsync(CancellationToken ct = default) 
        => await _reportDbContext.SaveChangesAsync(ct);
}