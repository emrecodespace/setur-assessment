namespace WorkerService.Options;
public class ApiClientsOptions
{
    public const string Section = "ApiClients";
    public ContactsApiOptions ContactsApi { get; set; } = new();
    public ContactReportsApiOptions ContactReportsApi { get; set; } = new();
}
public class ContactsApiOptions
{
    public string BaseUrl { get; set; } = string.Empty;
}
public class ContactReportsApiOptions
{
    public string BaseUrl { get; set; } = string.Empty;
}
