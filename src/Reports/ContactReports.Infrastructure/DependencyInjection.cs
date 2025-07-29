using ContactReports.Application.Interfaces;
using ContactReports.Infrastructure.Options;
using ContactReports.Infrastructure.Persistance;
using ContactReports.Infrastructure.Repositories;
using ContactReports.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ContactReports.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<ReportDbContext>((_, options) => options.UseNpgsql(connectionString, opt => opt.EnableRetryOnFailure()));

        builder.Services.AddScoped<IReportDbContext>(provider => provider.GetRequiredService<ReportDbContext>());
        builder.Services.AddScoped<IReportRepository, ReportRepository>();
        builder.Services.AddScoped<IReportQueueService, ReportQueueService>();
        
        builder.Services.Configure<RabbitMqOptions>(
            builder.Configuration.GetSection(RabbitMqOptions.Section));
    }
}