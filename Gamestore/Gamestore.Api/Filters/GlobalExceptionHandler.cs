using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Gamestore.Api.Filters;

/// <summary>
/// A global exception handler for handling unhandled exceptions in the application.
/// Implements the <see cref="IExceptionFilter"/> interface.
/// </summary>
public class GlobalExceptionHandler : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GlobalExceptionHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger used for logging exception details.</param>
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handles exceptions by logging the error and returning a 500 Internal Server Error response.
    /// </summary>
    /// <param name="context">The exception context containing information about the exception.</param>
    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception.Message);

        context.Result = new JsonResult(context.Exception.Message)
        {
            StatusCode = 500,
        };
    }
}
