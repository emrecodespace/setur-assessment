using Contacts.Domain.Entities;

namespace Contacts.Domain.Interfaces;

public interface IReportRepository
{
    public IQueryable<PersonContactInfo> ContactInfos { get; }
}