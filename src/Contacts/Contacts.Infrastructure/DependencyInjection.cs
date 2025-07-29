using Bogus;
using Contacts.Application.Interfaces;
using Contacts.Domain.Entities;
using Contacts.Domain.Enums;
using Contacts.Domain.Interfaces;
using Contacts.Infrastructure.Persistance;
using Contacts.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Person = Contacts.Domain.Entities.Person;

namespace Contacts.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<ContactDbContext>((_, options)
            => options.UseNpgsql(connectionString, opt => opt.EnableRetryOnFailure())
                
                .UseAsyncSeeding( async (context, _, cancellationToken) =>
                {
                    const string locale = "tr";
                    var persons = context.Set<Person>();
                    var hasPersonData = await persons.AnyAsync(cancellationToken: cancellationToken);
                    if (hasPersonData) return;
                    var contactInfo = new Faker<PersonContactInfo>(locale)
                        .RuleFor(p=> p.Id, f=> f.Random.Guid())
                        .RuleFor(p => p.InfoType, f => f.PickRandom<InfoType>())
                        .RuleFor(p => p.Content, (f, p) => p.InfoType switch
                        {
                            InfoType.Email => f.Internet.Email(),
                            InfoType.Phone => f.Phone.PhoneNumber(),
                            InfoType.Location => f.Address.City(),
                            _ => f.Lorem.Word()
                        });

                    var userdata = new Faker<Person>(locale)
                        .RuleFor(p=> p.Id, f=> f.Random.Guid())
                        .RuleFor(p => p.FirstName, f => f.Name.FirstName())
                        .RuleFor(p => p.LastName, f => f.Name.LastName())
                        .RuleFor(p => p.Company, f => f.Company.CompanyName())
                        .RuleFor(p => p.ContactInfos, f => contactInfo
                            .Generate(3).ToList())
                        .Generate(100);

                    await persons.AddRangeAsync(userdata, cancellationToken);
                    await context.SaveChangesAsync(cancellationToken);
                }));
        
        
        builder.Services.AddScoped<IContactDbContext>(provider => provider.GetRequiredService<ContactDbContext>());
        builder.Services.AddScoped<IContactRepository, ContactRepository>();
        builder.Services.AddScoped<IReportRepository, ReportRepository>();
    }
}