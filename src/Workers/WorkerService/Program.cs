using WorkerService.Apis;
using WorkerService.Interfaces;
using WorkerService.Options;
using WorkerService.Services;
using WorkerService.Workers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<ReportWorker>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.AddApisSetup();

builder.Services.Configure<ApiClientsOptions>(
    builder.Configuration.GetSection(ApiClientsOptions.Section));
builder.Services.Configure<RabbitMqOptions>(
    builder.Configuration.GetSection(RabbitMqOptions.Section));

var host = builder.Build();
host.Run();