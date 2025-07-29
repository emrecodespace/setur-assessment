using Contacts.Domain.Entities;
using Contacts.Domain.Interfaces;
using Contacts.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Contacts.Infrastructure.Repositories;

public class ReportRepository(ContactDbContext personDbContext) : IReportRepository
{
    private readonly ContactDbContext _personDbContext =
        personDbContext ?? throw new ArgumentNullException(nameof(personDbContext));
    
    public IQueryable<PersonContactInfo> ContactInfos => 
        _personDbContext.ContactInfos.AsNoTracking();
}