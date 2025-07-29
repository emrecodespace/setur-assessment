using Contacts.Application.Common;
using Contacts.Application.Dtos;
using Contacts.Application.Interfaces;
using Contacts.Domain.Enums;
using Contacts.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Contacts.Application.Services;

public class ReportService(IReportRepository reportRepository) : IReportService
{
    public async Task<Result<List<ReportDto>>> GetReport(CancellationToken ct = default)
    {
        var result = await reportRepository.ContactInfos
            .Where(c => c.InfoType == InfoType.Location)
            .GroupBy(c => c.Content)
            .Select(locationGroup => new ReportDto
            {
                Location = locationGroup.Key,
                PersonCount = locationGroup.Select(x => x.PersonId).Distinct().Count(),
                PhoneCount = reportRepository.ContactInfos.Count(ci => ci.InfoType == InfoType.Phone &&
                                                                       locationGroup.Select(x => x.PersonId)
                                                                           .Contains(ci.PersonId))
            })
            .ToListAsync(ct);

        return Result<List<ReportDto>>.Success(result);
    }
}
