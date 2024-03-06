using MediatR;
using RecipeSocialMediaAPI.Domain.Utilities;

namespace RecipeSocialMediaAPI.Presentation.Middleware;

public class LoggingPipelineBehaviour<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingPipelineBehaviour<TRequest, TResponse>> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;

    public LoggingPipelineBehaviour(ILogger<LoggingPipelineBehaviour<TRequest, TResponse>> logger, IDateTimeProvider dateTimeProvider)
    {
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting request {RequestName} ({DateTimeUTC})", typeof(TRequest).Name, _dateTimeProvider.Now);

        try
        {
            return await next();
        }
        finally
        {
            _logger.LogInformation("Completed request {RequestName} ({DateTimeUTC})", typeof(TRequest).Name, _dateTimeProvider.Now);
        }
    }
}
