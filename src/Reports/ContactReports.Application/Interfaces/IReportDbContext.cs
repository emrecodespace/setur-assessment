using ContactReports.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContactReports.Application.Interfaces;

public interface IReportDbContext
{
    DbSet<Report> Reports { get; set; }
    DbSet<ReportDetail> ReportDetails  { get; set; }
}