using Gamestore.Api.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;

namespace Gamestore.Tests.Database.ServicesTests;

/// <summary>
/// Unit tests for the LoggingActionFilter.
/// </summary>
public class LoggingActionFilterTests
{
    private readonly Mock<ILogger<LoggingActionFilter>> _mockLogger;
    private readonly LoggingActionFilter _filter;
    private ActionExecutingContext _actionExecutingContext;
    private ActionExecutedContext _actionExecutedContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingActionFilterTests"/> class.
    /// </summary>
    public LoggingActionFilterTests()
    {
        _mockLogger = new Mock<ILogger<LoggingActionFilter>>();
        _filter = new LoggingActionFilter(_mockLogger.Object);
    }

    /// <summary>
    /// Verifies the logger was called when the OnActionExecuting event occurs.
    /// </summary>
    [Fact]
    public void OnActionExecuting_CallsLogger()
    {
        // Arrange
        SetupExecutingContext();

        // Act
        _filter.OnActionExecuting(_actionExecutingContext);

        // Assert
        _mockLogger.Verify(x => x.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((value, type) => value.ToString().Contains($"Entering {nameof(TestController)}")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
    }

    /// <summary>
    /// Verifies the logger was called when the OnActionExecuted event occurs without an exception.
    /// </summary>
    [Fact]
    public void OnActionExecuted_CallsLogger_When_Method_Execute_Without_Exception()
    {
        // Arrange
        SetupExecutedContext();

        // Act
        _filter.OnActionExecuted(_actionExecutedContext);

        // Assert
        _mockLogger.Verify(x => x.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((value, type) => value.ToString().Contains($"Exiting {nameof(TestController)}")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
    }

    /// <summary>
    /// Verifies the logger was called when an exception is thrown in the OnActionExecuted event.
    /// </summary>
    [Fact]
    public void OnActionExecuted_CallsLogger_When_Exception_Throw()
    {
        // Arrange
        SetupExecutedContext(new Exception());

        // Act
        _filter.OnActionExecuted(_actionExecutedContext);

        // Assert
        _mockLogger.Verify(x => x.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((value, type) => value.ToString().Contains($"An error occurred in {nameof(TestController)}")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
    }

    private void SetupExecutedContext(Exception? ex = null)
    {
        var context = new DefaultHttpContext();
        var routeData = new RouteData();
        var descriptor = new ControllerActionDescriptor();

        var controllerContext = new ControllerContext(new ActionContext(context, routeData, descriptor));

        _actionExecutedContext = new ActionExecutedContext(controllerContext, new List<IFilterMetadata>(), new TestController())
        {
            Exception = ex,
        };
    }

    private void SetupExecutingContext()
    {
        var context = new DefaultHttpContext();
        var routeData = new RouteData();
        var descriptor = new ControllerActionDescriptor();

        var controllerContext = new ControllerContext(new ActionContext(context, routeData, descriptor));

        _actionExecutingContext = new ActionExecutingContext(controllerContext, new List<IFilterMetadata>(), new Dictionary<string, object?>(), new TestController());
    }

    /// <summary>
    /// Mock controller used for testing.
    /// </summary>
    public class TestController : Controller
    {
        /// <summary>
        /// A test action method.
        /// </summary>
        /// <returns>An IActionResult.</returns>
        public IActionResult TestAction() => Ok();
    }
}