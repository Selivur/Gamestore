using System.Net;
using Gamestore.Api.Models.DTO.OrderDTO;
using Gamestore.Api.Services;
using Gamestore.Database.Repositories.Interfaces;
using Moq;
using Moq.Protected;

namespace Gamestore.Tests.Api.Services;

/// <summary>
/// Test class for the <see cref="PaymentService"/> class.
/// </summary>
public class PaymentServiceTests
{
    private const string ApiUrl = "https://localhost:5001/api/payments";
    private readonly Mock<IOrderRepository> _mockUnitOfWork;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly PaymentService _paymentService;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentServiceTests"/> class.
    /// Setup for tests of the <see cref="PaymentService"/> class.
    /// </summary>
    public PaymentServiceTests()
    {
        _mockUnitOfWork = new();
        _mockHttpMessageHandler = new();
        var mockHttpClient = new HttpClient(_mockHttpMessageHandler.Object);
        _paymentService = new PaymentService(_mockUnitOfWork.Object, mockHttpClient);
    }

    /// <summary>
    /// Test for ProcessIboxPayment when HTTP request is successful.
    /// Expects to return an IBoxResponseDTO with the same details as the order.
    /// </summary>
    [Fact]
    public async Task ProcessIboxPayment_WhenHttpRequestIsSuccessful_ReturnsExpectedResult()
    {
        // Arrange
        var testOrder = new Order
        {
            Id = 1,
            Customer = new Customer { Id = 1 },
            OrderDetails = new List<OrderDetails>
        {
            new() { Price = 100 },
            new() { Price = 200 },
        },
        };

        _mockUnitOfWork
            .Setup(uow => uow.GetFirstOpenOrderAsync())
            .ReturnsAsync(testOrder);

        _mockUnitOfWork
            .Setup(uow => uow.CompleteOrder())
            .Returns(Task.CompletedTask)
            .Verifiable();

        var expectedUri = new Uri($"{ApiUrl}/ibox");

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post && req.RequestUri == expectedUri),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        // Act
        var result = await _paymentService.ProcessIboxPayment();

        // Assert
        Assert.Equal(testOrder.Id, result.OrderId);
        Assert.Equal(testOrder.Customer.Id, result.UserId);
        Assert.Equal(testOrder.OrderDetails.Sum(details => details.Price), result.Sum);
        _mockUnitOfWork.Verify();
    }

    /// <summary>
    /// Test for ProcessIboxPayment when HTTP request is unsuccessful.
    /// Expects to return an IBoxResponseDTO with the same details as the order.
    /// </summary>
    [Fact]
    public async Task ProcessIboxPayment_WhenHttpRequestIsUnsuccessful_ReturnsExpectedResult()
    {
        // Arrange
        var testOrder = new Order
        {
            Id = 1,
            Customer = new Customer { Id = 1 },
            OrderDetails = new List<OrderDetails>
        {
            new() { Price = 100 },
            new() { Price = 200 },
        },
        };

        _mockUnitOfWork
            .Setup(uow => uow.GetFirstOpenOrderAsync())
            .ReturnsAsync(testOrder);

        _mockUnitOfWork
            .Setup(uow => uow.CancelledOrder())
            .Returns(Task.CompletedTask)
            .Verifiable();

        var expectedUri = new Uri($"{ApiUrl}/ibox");

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post && req.RequestUri == expectedUri),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadGateway));

        // Act
        var result = await _paymentService.ProcessIboxPayment();

        // Assert
        Assert.Equal(testOrder.Id, result.OrderId);
        Assert.Equal(testOrder.Customer.Id, result.UserId);
        Assert.Equal(testOrder.OrderDetails.Sum(details => details.Price), result.Sum);
        _mockUnitOfWork.Verify();
    }

    /// <summary>
    /// Test for ProcessVisaPayment when HTTP request is successful.
    /// Expects to return an IBoxResponseDTO with the same details as the order.
    /// </summary>
    [Fact]
    public async Task ProcessVisaPayment_WhenHttpRequestIsSuccessful_ReturnsExpectedResult()
    {
        // Arrange
        var testOrder = new Order
        {
            Id = 1,
            Customer = new Customer { Id = 1 },
            OrderDetails = new List<OrderDetails>
        {
            new() { Price = 100 },
            new() { Price = 200 },
        },
        };

        var testVisaTransactionDto = new VisaTransactionDTO
        {
            TransactionAmount = 300,
            Holder = "John Doe",
            CardNumber = "4111111111111111",
            MonthExpire = 10,
            CVV2 = 123,
            YearExpire = 2025,
        };

        _mockUnitOfWork
            .Setup(uow => uow.GetFirstOpenOrderAsync())
            .ReturnsAsync(testOrder);

        _mockUnitOfWork
            .Setup(uow => uow.CompleteOrder())
            .Returns(Task.CompletedTask)
            .Verifiable();

        var expectedUri = new Uri($"{ApiUrl}/visa");

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post && req.RequestUri == expectedUri),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

        // Act
        var result = await _paymentService.ProcessVisaPayment(testVisaTransactionDto);

        // Assert
        Assert.Equal(testOrder.Id, result.OrderId);
        Assert.Equal(testOrder.Customer.Id, result.UserId);
        Assert.Equal(testOrder.OrderDetails.Sum(details => details.Price), result.Sum);
        _mockUnitOfWork.Verify();
    }

    /// <summary>
    /// Test for ProcessVisaPayment when HTTP request is unsuccessful.
    /// Expects to return an IBoxResponseDTO with the same details as the order.
    /// </summary>
    [Fact]
    public async Task ProcessVisaPayment_WhenHttpRequestIsUnsuccessful_ReturnsExpectedResult()
    {
        // Arrange
        var testOrder = new Order
        {
            Id = 1,
            Customer = new Customer { Id = 1 },
            OrderDetails = new List<OrderDetails>
        {
            new() { Price = 100 },
            new() { Price = 200 },
        },
        };

        var testVisaTransactionDto = new VisaTransactionDTO
        {
            TransactionAmount = 300,
            Holder = "John Doe",
            CardNumber = "4111111111111111",
            MonthExpire = 10,
            CVV2 = 123,
            YearExpire = 2025,
        };

        _mockUnitOfWork
            .Setup(uow => uow.GetFirstOpenOrderAsync())
            .ReturnsAsync(testOrder);

        _mockUnitOfWork
            .Setup(uow => uow.CancelledOrder())
            .Returns(Task.CompletedTask)
            .Verifiable();

        var expectedUri = new Uri($"{ApiUrl}/visa");

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post && req.RequestUri == expectedUri),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadGateway));

        // Act
        var result = await _paymentService.ProcessVisaPayment(testVisaTransactionDto);

        // Assert
        Assert.Equal(testOrder.Id, result.OrderId);
        Assert.Equal(testOrder.Customer.Id, result.UserId);
        Assert.Equal(testOrder.OrderDetails.Sum(details => details.Price), result.Sum);
        _mockUnitOfWork.Verify();
    }
}
