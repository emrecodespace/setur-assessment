using ContactReports.Domain.Enums;

namespace ContactReports.Domain.Entities;

public class Report
{
    public Guid Id { get; set; }
    public DateTime RequestedAt { get; set; }
    public ReportStatus Status { get; set; }
    public ICollection<ReportDetail> ReportDetails { get; set; }
}