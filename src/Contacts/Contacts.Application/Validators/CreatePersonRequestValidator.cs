using Contacts.Application.Dtos;
using FluentValidation;

namespace Contacts.Application.Validators;

public class CreatePersonRequestValidator : AbstractValidator<CreatePersonRequest>
{
    public CreatePersonRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name cannot be empty.")
            .MaximumLength(100).WithMessage("First name must be at most 100 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name cannot be empty.")
            .MaximumLength(100).WithMessage("Last name must be at most 100 characters.");

        RuleFor(x => x.Company)
            .MaximumLength(100).WithMessage("Company name must be at most 150 characters.");
    }
}