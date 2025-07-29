using Contacts.Domain.Entities;

namespace Contacts.Domain.Interfaces;

public interface IContactRepository
{
    Task<Guid> AddAsync(Person person, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid personId, CancellationToken ct = default);
    Task<Guid> AddContactInfoAsync(PersonContactInfo contact, CancellationToken ct = default);
    Task<bool> RemoveContactInfoAsync(Guid personId, Guid contactInfoId, CancellationToken ct = default);
    Task<IEnumerable<Person>> GetAllAsync(CancellationToken ct = default);
    Task<Person?> GetPersonWithContactInfosAsync(Guid personId, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken ct);
}