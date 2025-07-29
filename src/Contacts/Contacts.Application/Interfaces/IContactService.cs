using Contacts.Application.Common;
using Contacts.Application.Dtos;

namespace Contacts.Application.Interfaces;

public interface IContactService
{
    Task<Result<Guid>> CreatePersonAsync(CreatePersonRequest req, CancellationToken ct = default);
    Task<Result<bool>> DeletePersonAsync(Guid personId, CancellationToken ct = default);
    Task<Result<Guid>> AddContactInfoAsync(Guid personId, AddContactInfoRequest req, CancellationToken ct = default);
    Task<Result<bool>> RemoveContactInfoAsync(Guid personId, Guid contactInfoId, CancellationToken ct = default);
    Task<Result<IEnumerable<PersonsDto>>> GetAllPersonsAsync(CancellationToken ct = default);
    Task<Result<PersonDetailsDto?>> GetPersonWithContactInfosAsync(Guid personId, CancellationToken ct = default);
}