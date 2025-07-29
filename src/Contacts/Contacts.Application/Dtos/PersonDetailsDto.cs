namespace Contacts.Application.Dtos;

public sealed record PersonDetailsDto(Guid Id, string FirstName, string LastName, string Company,
    IEnumerable<ContactInfoDto> ContactInfos
);
