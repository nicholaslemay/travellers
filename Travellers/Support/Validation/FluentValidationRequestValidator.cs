using FluentValidation;

namespace Travellers.Support.Validation;

public class FluentValidationRequestValidator<TRequest>(IValidator<TRequest> validator)
    : IRequestValidator<TRequest>
{
    public ValidationOutcome Validate(TRequest request)
    {
        var result = validator.Validate(request);

        if (result.IsValid)
        {
            return ValidationOutcome.Success();
        }

        var errors = result.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(group => group.Key, group => group.Select(error => error.ErrorMessage).ToArray());

        return ValidationOutcome.Failure(errors);
    }
}
