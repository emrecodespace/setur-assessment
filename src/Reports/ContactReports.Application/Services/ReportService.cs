using ContactReports.Application.Common;
using ContactReports.Application.Dtos;
using ContactReports.Application.Interfaces;
using ContactReports.Domain.Entities;
using ContactReports.Domain.Enums;

namespace ContactReports.Application.Services;

public class ReportService(IReportRepository reportRepository, IReportQueueService reportQueueService) : IReportService
{
    public async Task<Result<IEnumerable<ReportDto>>> GetReports(CancellationToken ct)
    {
        var reports = await reportRepository.GetAllAsync(ct);

        return Result<IEnumerable<ReportDto>>.Success(reports.Select(r =>
            new ReportDto(r.Id, r.Status, r.RequestedAt)));
    }

    public async Task<Result<IEnumerable<ReportDetailsDto>>> GetReportDetails(Guid id, CancellationToken ct)
    {
        var details = await reportRepository.GetReportDetailsByIdAsync(id, ct);

        return Result<IEnumerable<ReportDetailsDto>>.Success(details.Select(r =>
            new ReportDetailsDto(r.Location, r.PersonCount, r.PhoneNumberCount)));
    }

    public async Task<Result> CreateReport(CancellationToken ct)
    {
        var report = new Report
        {
            Status = ReportStatus.Preparing,
            RequestedAt = DateTime.UtcNow,
        };
        
        await reportRepository.AddAsync(report, ct);
        await reportRepository.SaveChangesAsync(ct);
        await reportQueueService.Send(new ReportQueueRequest(report.Id));
        
        return Result.Success();
    }
    
    public async Task<Result> AddReportDetails(Guid id, IEnumerable<ReportDetailsRequest> details, CancellationToken ct)
    { 
        var report = await reportRepository.GetAsync(p => p.Id == id && p.Status == ReportStatus.Preparing, ct);
        
        if (report is null)
        {
            return Result.Failure("Report not found");
        }
        
        report.Status = ReportStatus.Completed;
        report.ReportDetails = details.Select(p => new ReportDetail
        {
            Location = p.Location,
            PersonCount = p.PersonCount,
            PhoneNumberCount = p.PhoneNumberCount,
        }).ToList();
        
        await reportRepository.SaveChangesAsync(ct);
        
        return Result.Success();
    }
}