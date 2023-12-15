using System.Diagnostics;

namespace Gamestore.Api.Middleware;

/// <summary>
/// Middleware for logging the performance of incoming requests to a text file.
/// </summary>
public class PerformanceLoggingMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of the <see cref="PerformanceLoggingMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    public PerformanceLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Invokes the middleware to measure the performance and passes the request to the next middleware in the pipeline.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task Invoke(HttpContext context)
    {
        // Start measuring the time
        var watch = Stopwatch.StartNew();

        // Pass the request to the next middleware in the pipeline
        await _next(context);

        // Stop measuring and log the elapsed time
        watch.Stop();
        var elapsedMilliseconds = watch.ElapsedMilliseconds;
        LogPerformance(elapsedMilliseconds);
    }

    /// <summary>
    /// Logs the performance information to a text file.
    /// </summary>
    /// <param name="elapsedMilliseconds">The elapsed time in milliseconds.</param>
    private static void LogPerformance(long elapsedMilliseconds)
    {
        File.AppendAllText("performanceLog.txt", $"{DateTime.Now}: Request took {elapsedMilliseconds}ms\n");
    }
}
