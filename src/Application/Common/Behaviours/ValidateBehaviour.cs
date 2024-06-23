using FluentValidation.Results;
using ValidationException = SmartFridgeManagerAPI.Application.Common.Exceptions.ValidationException;

namespace SmartFridgeManagerAPI.Application.Common.Behaviours;

public class ValidateBehaviour<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (validators.Any())
        {
            ValidationContext<TRequest> context = new(request);

            ValidationResult[] validationResults = await Task.WhenAll(
                validators.Select(v =>
                    v.ValidateAsync(context, cancellationToken)));

            List<ValidationFailure> failures = validationResults
                .Where(r => r.Errors.Any())
                .SelectMany(r => r.Errors)
                .ToList();

            if (failures.Any())
            {
                throw new ValidationException(failures);
            }
        }

        return await next();
    }
}
