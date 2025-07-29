using Contacts.Application.Interfaces;
using Contacts.Application.Services;
using Contacts.Application.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Contacts.Application;

public static class DependencyInjection
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IContactService, ContactService>();
        builder.Services.AddScoped<IReportService, ReportService>();
        
        builder.Services.AddValidatorsFromAssembly(typeof(CreatePersonRequestValidator).Assembly);
        builder.Services.AddValidatorsFromAssembly(typeof(AddContactInfoRequestValidator).Assembly);
    }
}