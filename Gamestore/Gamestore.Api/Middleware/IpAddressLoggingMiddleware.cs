namespace Gamestore.Api.Middleware;

/// <summary>
/// Middleware for logging the IP address of incoming requests to a text file.
/// </summary>
public class IpAddressLoggingMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of the <see cref="IpAddressLoggingMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    public IpAddressLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Invokes the middleware to log the IP address and passes the request to the next middleware in the pipeline.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task Invoke(HttpContext context)
    {
        var ipAddress = context.Connection.RemoteIpAddress.ToString();

        LogIpAddress(ipAddress);

        await _next(context);
    }

    /// <summary>
    /// Logs the specified IP address to a text file.
    /// </summary>
    /// <param name="ipAddress">The IP address to log.</param>
    private static void LogIpAddress(string ipAddress)
    {
        using var fileStream = new FileStream("ipAddressLog.txt", FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
        using var writer = new StreamWriter(fileStream);
        writer.WriteLine($"{DateTime.UtcNow}: IP Address: {ipAddress}");
    }
}
