using Contacts.Domain.Entities;
using Contacts.Domain.Interfaces;
using Contacts.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Contacts.Infrastructure.Repositories;

public class ContactRepository(ContactDbContext personDbContext) : IContactRepository
{
    private readonly ContactDbContext _personDbContext =
        personDbContext ?? throw new ArgumentNullException(nameof(personDbContext));
    
    public async Task<Guid> AddAsync(Person person, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(person);

        await _personDbContext.Persons.AddAsync(person, ct);

        return person.Id;
    }
    
    public async Task<IEnumerable<Person>> GetAllAsync(CancellationToken ct = default)
        => await _personDbContext.Persons
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        => await _personDbContext.Persons.AnyAsync(x => x.Id == id, ct);

    public async Task<bool> DeleteAsync(Guid personId, CancellationToken ct = default)
    {
        var person = await _personDbContext.Persons
            .FirstOrDefaultAsync(p => p.Id == personId, ct);

        if (person is null) return false;

        _personDbContext.Persons.Remove(person);
        return true;
    }

    public async Task<Guid> AddContactInfoAsync(PersonContactInfo contact, CancellationToken ct = default)
    {
        await _personDbContext.ContactInfos.AddAsync(contact, ct);
        return contact.Id;
    }

    public async Task<bool> RemoveContactInfoAsync(Guid personId, Guid contactInfoId, CancellationToken ct = default)
    {
        var contact = await _personDbContext.ContactInfos
            .FirstOrDefaultAsync(c => c.Id == contactInfoId && c.PersonId == personId, ct);

        if (contact is null)
            return false;

        _personDbContext.ContactInfos.Remove(contact);
        return true;
    }

    public Task<Person?> GetPersonWithContactInfosAsync(Guid personId, CancellationToken ct = default)
    {
        return _personDbContext.Persons
            .Include(p => p.ContactInfos)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == personId, ct);
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct) => 
        await _personDbContext.SaveChangesAsync(ct);
    
}