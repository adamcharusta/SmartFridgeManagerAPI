using ValidationException = SmartFridgeManagerAPI.Application.Common.Exceptions.ValidationException;

namespace SmartFridgeManagerAPI.Application.Common.Behaviours;

using ValidationException = ValidationException;

public class UnhandledExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            string requestName = typeof(TRequest).Name;

            Log.Error(ex, "SmartFridgeManagerAPI Request: Unhandled Exception for Request {Name} {@Request}",
                requestName, request);

            throw;
        }
    }
}
