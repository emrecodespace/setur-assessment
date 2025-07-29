namespace ContactReports.Infrastructure.Options;

public class RabbitMqOptions
{
    public const string Section = "RabbitMq";
    public string HostName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string QueueName { get; set; } = string.Empty;
    public int Port { get; set; }
}