namespace WorkerService.Dtos;

public sealed record ReportDetailsRequest(string Location, int PersonCount, int PhoneNumberCount);