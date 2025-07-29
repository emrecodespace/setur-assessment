using Contacts.Application.Common;
using Contacts.Application.Dtos;
using Contacts.Application.Helpers;
using Contacts.Application.Interfaces;
using Contacts.Domain.Entities;
using Contacts.Domain.Interfaces;
using FluentValidation;

namespace Contacts.Application.Services;

public class ContactService(IContactRepository contactRepository, IValidator<CreatePersonRequest> createPersonValidator, IValidator<AddContactInfoRequest> addContactInfoRequestValidator) : IContactService
{
    public async Task<Result<Guid>> CreatePersonAsync(CreatePersonRequest req, CancellationToken ct = default)
    {
        var validation = await createPersonValidator.ValidateAsync(req, ct);
        if (!validation.IsValid)
            return Result<Guid>.Failure(validation.ToErrorString(), 400);
        
        var person = new Person
        {
            Id = Guid.NewGuid(),
            FirstName = req.FirstName,
            LastName = req.LastName,
            Company = req.Company
        };

        await contactRepository.AddAsync(person, ct);
        await contactRepository.SaveChangesAsync(ct);
        return Result<Guid>.Success(person.Id);
    }

    public async Task<Result<bool>> DeletePersonAsync(Guid personId, CancellationToken ct = default)
    {
        var deleted = await contactRepository.DeleteAsync(personId, ct);
        if (!deleted)
            return Result<bool>.Failure("Not Found.", 404);

        await contactRepository.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }

    public async Task<Result<Guid>> AddContactInfoAsync(Guid personId, AddContactInfoRequest req,
        CancellationToken ct = default)
    {
        var validation = await addContactInfoRequestValidator.ValidateAsync(req, ct);
        if (!validation.IsValid)
            return Result<Guid>.Failure(validation.ToErrorString(), 400);

        var personExists = await contactRepository.ExistsAsync(personId, ct);
        if (!personExists)
            return Result<Guid>.Failure("Not Found.", 404);

        var contact = new PersonContactInfo
        {
            Id = Guid.NewGuid(),
            PersonId = personId,
            InfoType = req.Type,
            Content = req.Content
        };

        await contactRepository.AddContactInfoAsync(contact, ct);
        await contactRepository.SaveChangesAsync(ct);
        return Result<Guid>.Success(contact.Id);
    }

    public async Task<Result<bool>> RemoveContactInfoAsync(Guid personId, Guid contactInfoId,
        CancellationToken ct = default)
    {
        var removed = await contactRepository.RemoveContactInfoAsync(personId, contactInfoId, ct);
        if (!removed)
            return Result<bool>.Failure("Not Found", 404);

        await contactRepository.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }

    public async Task<Result<IEnumerable<PersonsDto>>> GetAllPersonsAsync(CancellationToken ct = default)
    {
        var persons = await contactRepository.GetAllAsync(ct);

        var mapped = persons.Select(p => new PersonsDto(
            p.Id,
            p.FirstName,
            p.LastName,
            p.Company
        ));
      
        return Result<IEnumerable<PersonsDto>>.Success(mapped);
    }

    public async Task<Result<PersonDetailsDto?>> GetPersonWithContactInfosAsync(Guid personId, CancellationToken ct = default)
    {
        var person = await contactRepository.GetPersonWithContactInfosAsync(personId, ct);
        if (person is null)
            return Result<PersonDetailsDto?>.Failure("Not Found.", 404);

        var mapped = new PersonDetailsDto(
            person.Id,
            person.FirstName,
            person.LastName,
            person.Company,
            person.ContactInfos.Select(ci => new ContactInfoDto(
                ci.Id,
                ci.InfoType,
                ci.Content
            )).ToList()
        );

        return Result<PersonDetailsDto?>.Success(mapped);
    }
}