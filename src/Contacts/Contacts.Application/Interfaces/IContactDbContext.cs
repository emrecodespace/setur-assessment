using Contacts.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Contacts.Application.Interfaces;

public interface IContactDbContext
{
    DbSet<Person> Persons { get; set; }
    DbSet<PersonContactInfo> ContactInfos  { get; set; }
}