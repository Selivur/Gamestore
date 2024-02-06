using System.Net;
using Gamestore.Api.Middleware;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Gamestore.Tests.Api.Middlewares;

/// <summary>
/// Contains unit tests for the <see cref="IpAddressLoggingMiddleware"/> class.
/// </summary>
public class IpAddressLoggingMiddlewareTests
{
    private readonly Mock<RequestDelegate> _nextMock;
    private readonly IpAddressLoggingMiddleware _middleware;

    /// <summary>
    /// Initializes a new instance of the <see cref="IpAddressLoggingMiddlewareTests"/> class.
    /// </summary>
    public IpAddressLoggingMiddlewareTests()
    {
        _nextMock = new Mock<RequestDelegate>();
        _middleware = new IpAddressLoggingMiddleware(_nextMock.Object);
    }

    /// <summary>
    /// Tests the Invoke method to ensure it logs the IP address and calls the next middleware.
    /// </summary>
    [Fact]
    public async Task Invoke_LogsIpAddressAndCallsNext()
    {
        // Arrange
        var contextMock = new Mock<HttpContext>();
        contextMock.SetupGet(x => x.Connection.RemoteIpAddress).Returns(IPAddress.Parse("127.0.0.1"));

        // Act
        await _middleware.Invoke(contextMock.Object);

        // Assert
        _nextMock.Verify(x => x(contextMock.Object), Times.Once);

        using var reader = new StreamReader("ipAddressLog.txt");
        var logEntry = await reader.ReadToEndAsync();
        Assert.Contains("127.0.0.1", logEntry);
    }
}
