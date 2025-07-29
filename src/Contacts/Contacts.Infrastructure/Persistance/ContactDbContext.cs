using Contacts.Application.Interfaces;
using Contacts.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Contacts.Infrastructure.Persistance;

public class ContactDbContext(DbContextOptions<ContactDbContext> options) : DbContext(options), IContactDbContext
{
    public DbSet<Person> Persons { get; set; }
    public DbSet<PersonContactInfo> ContactInfos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ContactDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}