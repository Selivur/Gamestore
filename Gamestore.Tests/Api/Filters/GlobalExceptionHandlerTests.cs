using Gamestore.Api.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;

namespace Gamestore.Tests.Api.Filters;

/// <summary>
/// Unit tests for the OnException method of the GlobalExceptionHandler class.
/// Tests whether the method correctly logs error messages and returns a 500 Internal Server Error response.
/// </summary>
public class GlobalExceptionHandlerTests
{
    private readonly Mock<ILogger<GlobalExceptionHandler>> _loggerMock;
    private readonly GlobalExceptionHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GlobalExceptionHandlerTests"/> class.
    /// </summary>
    public GlobalExceptionHandlerTests()
    {
        _loggerMock = new Mock<ILogger<GlobalExceptionHandler>>();
        _handler = new GlobalExceptionHandler(_loggerMock.Object);
    }

    /// <summary>
    /// Tests the OnException method to ensure it logs the exception message and sets the appropriate response.
    /// </summary>
    [Fact]
    public void OnException_LogsErrorAndSetsResponse()
    {
        // Arrange
        var exceptionMessage = "Test exception";
        var httpContextMock = new Mock<HttpContext>();
        var actionContext = new ActionContext(
            httpContextMock.Object,
            new RouteData(),
            new ActionDescriptor(),
            new ModelStateDictionary());
        var exceptionContext = new ExceptionContext(
            actionContext,
            new List<IFilterMetadata>())
        {
            Exception = new Exception(exceptionMessage),
        };

        // Act
        _handler.OnException(exceptionContext);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception?>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);

        var result = Assert.IsType<JsonResult>(exceptionContext.Result);
        Assert.Equal(500, result.StatusCode);
        Assert.Equal(exceptionMessage, result.Value);
    }
}
