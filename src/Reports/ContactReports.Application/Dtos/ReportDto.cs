using ContactReports.Domain.Enums;

namespace ContactReports.Application.Dtos;

public record ReportDto(Guid Id, ReportStatus Status, DateTime RequestedAt);

public record ReportDetailsDto(string Location, int PersonCount, int PhoneNumberCount);

public record ReportDetailsRequest(string Location, int PersonCount, int PhoneNumberCount);