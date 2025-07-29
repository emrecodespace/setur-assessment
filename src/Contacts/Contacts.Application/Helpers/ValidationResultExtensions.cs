using FluentValidation.Results;

namespace Contacts.Application.Helpers;

public static class ValidationResultExtensions
{
    public static string ToErrorString(this ValidationResult validation)
        => string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
}