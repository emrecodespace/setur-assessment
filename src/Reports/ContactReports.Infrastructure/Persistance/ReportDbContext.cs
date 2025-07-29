using ContactReports.Application.Interfaces;
using ContactReports.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContactReports.Infrastructure.Persistance;

public class ReportDbContext(DbContextOptions<ReportDbContext> options) : DbContext(options), IReportDbContext
{
    public DbSet<Report> Reports { get; set; }
    public DbSet<ReportDetail> ReportDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReportDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}