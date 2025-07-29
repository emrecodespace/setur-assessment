using Contacts.Domain.Enums;

namespace Contacts.Application.Dtos;

public sealed record ContactInfoDto(Guid Id, InfoType Type, string Content);