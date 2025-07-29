namespace WorkerService.Apis.Dtos;

public sealed record ReportDetailsDto
{
    public string Location { get; set; }
    public int PersonCount { get; set; }
    public int PhoneCount { get; set; }
}