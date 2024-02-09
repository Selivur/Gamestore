using Microsoft.AspNetCore.Mvc.Filters;

namespace Gamestore.Api.Filters;

/// <summary>
/// An action filter that logs action execution events.
/// </summary>
public class LoggingActionFilter : IActionFilter
{
    /// <summary>
    /// The logger used for logging events.
    /// </summary>
    private readonly ILogger<LoggingActionFilter> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingActionFilter"/> class.
    /// </summary>
    /// <param name="logger">The logger used for logging events.</param>
    public LoggingActionFilter(ILogger<LoggingActionFilter> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Called before the action executes. Logs the name of the class and the action that is about to execute.
    /// </summary>
    /// <param name="context">The action executing context.</param>
    public void OnActionExecuting(ActionExecutingContext context)
    {
        _logger.LogDebug($"Entering {context.Controller.GetType().Name}.{context.ActionDescriptor.DisplayName}");
    }

    /// <summary>
    /// Called after the action executes. If an error occurred, it logs the error.
    /// If no error occurred, it logs the name of the class and the action that just finished executing.
    /// </summary>
    /// <param name="context">The action executed context.</param>
    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception != null)
        {
            _logger.LogError(context.Exception, $"An error occurred in {context.Controller.GetType().Name}.{context.ActionDescriptor.DisplayName}");
        }
        else
        {
            _logger.LogDebug($"Exiting {context.Controller.GetType().Name}.{context.ActionDescriptor.DisplayName}");
        }
    }
}