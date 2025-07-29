using Contacts.Application.Dtos;
using Contacts.Domain.Enums;
using FluentValidation;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Contacts.Application.Validators;

public class AddContactInfoRequestValidator : AbstractValidator<AddContactInfoRequest>
{
    private static readonly Regex PhoneRegex = new(@"^\+?\d{10,15}$", RegexOptions.Compiled);
    
    public AddContactInfoRequestValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid contact type selected.");

        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Contact content cannot be empty.")
            .MaximumLength(100)
            .WithMessage("Content must be at most 100 characters.")
            .Must((model, content) =>
                model.Type switch
                {
                    InfoType.Phone => PhoneRegex.IsMatch(content ?? ""),
                    InfoType.Email => new EmailAddressAttribute().IsValid(content),
                    _ => true
                })
            .WithMessage(model =>
                model.Type switch
                {
                    InfoType.Phone => "Invalid phone number format.",
                    InfoType.Email => "Invalid email format.",
                    _ => "Invalid content format."
                });
    }
}