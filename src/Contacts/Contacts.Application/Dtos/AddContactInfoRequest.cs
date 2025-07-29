using Contacts.Domain.Enums;

namespace Contacts.Application.Dtos;

public record AddContactInfoRequest(InfoType Type, string Content);