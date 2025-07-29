namespace Contacts.Application.Dtos;

public sealed record ReportDto
{
    public string Location { get; set; }
    public int PersonCount { get; set; }
    public int PhoneCount { get; set; }
}