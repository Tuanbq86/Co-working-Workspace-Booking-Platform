using FluentValidation;
using MediatR;
using WorkHive.BuildingBlocks.CQRS;

namespace WorkHive.BuildingBlocks.Behaviors;
/*This class for validate the request before command*/
public class ValidationBehavior<TRequest, TResponse>
    (IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        //Create context for all request
        //Using all validation in constructor to handle request
        var context = new ValidationContext<TRequest>(request);

        //Scan in context and await result for task handle
        var validationResults =
            await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        //Select and filter all failures in task
        var failures =
            validationResults
            .Where(r => r.Errors.Any())
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures.Any())
            throw new ValidationException(failures);

        //Continue handle request
        return await next();
    }
}
