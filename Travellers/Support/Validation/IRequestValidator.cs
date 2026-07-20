namespace Travellers.Support.Validation;

public interface IRequestValidator<in TRequest>
{
    ValidationOutcome Validate(TRequest request);
}
