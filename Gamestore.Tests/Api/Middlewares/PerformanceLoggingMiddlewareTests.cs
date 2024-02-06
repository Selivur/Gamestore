using Gamestore.Api.Middleware;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Gamestore.Tests.Api.Middlewares;

/// <summary>
/// Contains unit tests for the <see cref="PerformanceLoggingMiddleware"/> class.
/// </summary>
public class PerformanceLoggingMiddlewareTests
{
    private readonly Mock<RequestDelegate> _nextMock;
    private readonly PerformanceLoggingMiddleware _middleware;

    /// <summary>
    /// Initializes a new instance of the <see cref="PerformanceLoggingMiddlewareTests"/> class.
    /// </summary>
    public PerformanceLoggingMiddlewareTests()
    {
        _nextMock = new Mock<RequestDelegate>();
        _middleware = new PerformanceLoggingMiddleware(_nextMock.Object);
    }

    /// <summary>
    /// Tests the Invoke method to ensure it logs the performance and calls the next middleware.
    /// </summary>
    [Fact]
    public async Task Invoke_LogsPerformanceAndCallsNext()
    {
        // Arrange
        var contextMock = new Mock<HttpContext>();

        // Act
        await _middleware.Invoke(contextMock.Object);

        // Assert
        _nextMock.Verify(x => x(contextMock.Object), Times.Once);

        using var reader = new StreamReader("performanceLog.txt");
        var logEntry = await reader.ReadToEndAsync();
        Assert.Contains("Request took", logEntry);
    }
}
