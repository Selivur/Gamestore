using System.Text;
using Gamestore.Api.Controllers;
using Gamestore.Api.Models.DTO.OrderDTO;
using Gamestore.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Gamestore.Tests.Api.Controllers;

/// <summary>
/// Provides unit tests for the <see cref="OrderController"/> class.
/// </summary>
public class OrderControllerTests
{
    private readonly Mock<IOrderService> _orderService;
    private readonly OrderController _controller;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderControllerTests"/> class.
    /// </summary>
    public OrderControllerTests()
    {
        _orderService = new Mock<IOrderService>();
        _controller = new OrderController(_orderService.Object);
    }

    /// <summary>
    /// Test for successful GetOrder call.
    /// </summary>
    [Fact]
    public async Task GetOrder_WithValidId_ReturnsOkResultWithOrder()
    {
        // Arrange
        var testOrder = new OrderResponse { Id = 1 };
        _orderService.Setup(os => os.GetOrderByIdAsync(It.IsAny<int>())).ReturnsAsync(testOrder);

        // Act
        var result = await _controller.GetOrder(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<OrderResponse>(okResult.Value);
        Assert.Equal(testOrder, returnValue);
    }

    /// <summary>
    /// Test for successful GetAllOrders call.
    /// </summary>
    [Fact]
    public async Task GetAllOrders_WhenCalled_ReturnsOkResultWithOrders()
    {
        // Arrange
        var testOrders = new List<OrderResponse> { new() { Id = 1 }, new() { Id = 2 } };
        _orderService.Setup(s => s.GetAllOrdersAsync()).ReturnsAsync(testOrders);

        // Act
        var result = await _controller.GetAllOrders();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<OrderResponse>>(okResult.Value);
        Assert.Equal(testOrders, returnValue);
    }

    /// <summary>
    /// Test for GetAllOrders when no orders are found.
    /// </summary>
    [Fact]
    public async Task GetAllOrders_WhenCalled_ReturnsOkResultWithEmptyList()
    {
        // Arrange
        _orderService.Setup(s => s.GetAllOrdersAsync()).ReturnsAsync(new List<OrderResponse>());

        // Act
        var result = await _controller.GetAllOrders();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<OrderResponse>>(okResult.Value);
        Assert.Empty(returnValue);
    }

    /// <summary>
    /// Test for successful CreateGameOrder call.
    /// </summary>
    [Fact]
    public async Task CreateGameOrder_WhenCalled_ReturnsOkResult()
    {
        // Arrange
        var testOrderBuyResponse = new OrderBuyResponse { Id = "1" };
        _orderService.Setup(s => s.AddOrderWithDetails(It.IsAny<string>())).ReturnsAsync(testOrderBuyResponse);

        // Act
        var result = await _controller.CreateGameOrder("testAlias");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<OrderBuyResponse>(okResult.Value);
        Assert.Equal(testOrderBuyResponse, returnValue);
    }

    /// <summary>
    /// Test for successful GetCartDetails call.
    /// </summary>
    [Fact]
    public async Task GetCartDetails_WhenCalled_ReturnsOkResultWithCartDetails()
    {
        // Arrange
        var testCartDetails = new List<CartDetailsDTO> { new() { Id = 1 }, new() { Id = 2 } };
        _orderService.Setup(s => s.GetCartDetailsAsync(It.IsAny<int>())).ReturnsAsync(testCartDetails);

        // Act
        var result = await _controller.GetCartDetails(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<CartDetailsDTO>>(okResult.Value);
        Assert.Equal(testCartDetails, returnValue);
    }

    /// <summary>
    /// Test for GetCartDetails when no cart details are found.
    /// </summary>
    [Fact]
    public async Task GetCartDetails_WhenCalledButNoCartDetailsFound_ReturnsEmptyList()
    {
        // Arrange
        _orderService.Setup(s => s.GetCartDetailsAsync(It.IsAny<int>())).ReturnsAsync(new List<CartDetailsDTO>());

        // Act
        var result = await _controller.GetCartDetails(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<CartDetailsDTO>>(okResult.Value);
        Assert.Empty(returnValue);
    }

    /// <summary>
    /// Test for GetOpenOrderDetails method when details are available.
    /// </summary>
    [Fact]
    public async Task GetOpenOrderDetails_WhenDetailsAreAvailable_ReturnsOkResultWithDetails()
    {
        // Arrange
        var expectedDetails = new List<CartDetailsDTO>
        {
            new()
            {
                Id = 1,
                Quantity = 1,
                Price = 100,
                GameId = "game1",
            },
            new()
            {
                Id = 2,
                Quantity = 2,
                Price = 200,
                GameId = "game2",
            },
        };

        _orderService
            .Setup(service => service.GetOpenOrderDetailsAsync())
            .ReturnsAsync(expectedDetails);

        // Act
        var result = await _controller.GetOpenOrderDetails();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<CartDetailsDTO>>(okResult.Value);
        Assert.Equal(expectedDetails, returnValue);
    }

    /// <summary>
    /// Test for GetOpenOrderDetails method when there are no details.
    /// </summary>
    [Fact]
    public async Task GetOpenOrderDetails_WhenNoDetails_ReturnsOkResultWithEmptyDetails()
    {
        // Arrange
        var expectedDetails = new List<CartDetailsDTO>();
        _orderService
            .Setup(service => service.GetOpenOrderDetailsAsync())
            .ReturnsAsync(expectedDetails);

        // Act
        var result = await _controller.GetOpenOrderDetails();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<CartDetailsDTO>>(okResult.Value);
        Assert.Empty(returnValue);
    }

    /// <summary>
    /// Test for GetPaymentMethodInfo method when details are available.
    /// </summary>
    [Fact]
    public async Task GetPaymentMethodInfo_WhenDetailsAreAvailable_ReturnsOkResultWithDetails()
    {
        // Arrange
        var order = new Order { Id = 1, OrderDate = DateTime.Now };
        var paymentMethods = new List<PaymentDetails>
        {
            new() { Title = "Method1", Description = "desc1" },
            new() { Title = "Method2", Description = "desc2" },
        };
        var expectedDetails = new CreateOrderDTO { Order = order, PaymentMethods = paymentMethods };

        _orderService
            .Setup(service => service.GetAllPaymentMethodsWithOrder())
            .ReturnsAsync(expectedDetails);

        // Act
        var result = await _controller.GetPaymentMethodInfo();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<CreateOrderDTO>(okResult.Value);
        Assert.Equal(expectedDetails, returnValue);
    }

    /// <summary>
    /// Test for GetPaymentMethodInfo method when there are no details.
    /// </summary>
    [Fact]
    public async Task GetPaymentMethodInfo_WhenNoDetails_ReturnsOkResultWithEmptyDetails()
    {
        // Arrange
        var expectedDetails = new CreateOrderDTO();
        _orderService
            .Setup(service => service.GetAllPaymentMethodsWithOrder())
            .ReturnsAsync(expectedDetails);

        // Act
        var result = await _controller.GetPaymentMethodInfo();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<CreateOrderDTO>(okResult.Value);
        Assert.Equal(expectedDetails, returnValue);
    }

    /// <summary>
    /// Test for RemoveOrderDetails method.
    /// </summary>
    [Fact]
    public async Task RemoveOrderDetails_WhenCalled_CallsServiceWithCorrectGameAliasAndReturnsOk()
    {
        // Arrange
        var gameAlias = "game1";
        _orderService
            .Setup(service => service.RemoveOrderDetailsAsync(gameAlias))
            .Returns(Task.CompletedTask)
            .Verifiable(); // Call is expected, so it's made verifiable

        // Act
        var result = await _controller.RemoveOrderDetails(gameAlias);

        // Assert
        _orderService.Verify(); // Verify that setup method was indeed called
        Assert.IsType<OkResult>(result);
    }

    /// <summary>
    /// Test for GetBankInvoicePdf method with "IBox terminal" payment method.
    /// </summary>
    [Fact]
    public async Task GetBankInvoicePdf_WithIBoxTerminalMethod_ReturnsOkWithIBoxOrderDetails()
    {
        // Arrange
        var paymentRequest = new PaymentRequestDTO { Method = "IBox terminal" };
        var expectedOrderDetails = new IBoxResponseDTO { OrderId = 1, UserId = 2, Sum = 100 };

        _orderService
            .Setup(service => service.GetIBoxTerminalOrderDetailsAsync())
            .ReturnsAsync(expectedOrderDetails);

        // Act
        var result = await _controller.GetBankInvoicePdf(paymentRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<IBoxResponseDTO>(okResult.Value);
        Assert.Equal(expectedOrderDetails, returnValue);
    }

    /// <summary>
    /// Test for GetBankInvoicePdf method with "Visa" payment method.
    /// </summary>
    [Fact]
    public async Task GetBankInvoicePdf_WithVisaMethod_ReturnsOkWithVisaOrderDetails()
    {
        // Arrange
        var paymentRequest = new PaymentRequestDTO { Method = "Visa", Model = new VisaTransactionDTO() };
        var expectedOrderDetails = new IBoxResponseDTO { OrderId = 1, UserId = 2, Sum = 100 };

        _orderService
            .Setup(service => service.GetVisaOrderDetailsAsync(paymentRequest.Model))
            .ReturnsAsync(expectedOrderDetails);

        // Act
        var result = await _controller.GetBankInvoicePdf(paymentRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<IBoxResponseDTO>(okResult.Value);
        Assert.Equal(expectedOrderDetails, returnValue);
    }

    /// <summary>
    /// Test for GetBankInvoicePdf method with "Bank" payment method.
    /// </summary>
    [Fact]
    public async Task GetBankInvoicePdf_WithBankMethod_ReturnsFileWithPdfBytes()
    {
        // Arrange
        var paymentRequest = new PaymentRequestDTO { Method = "Bank" };
        var expectedBytes = Encoding.UTF8.GetBytes("InvoicePDF");

        _orderService
            .Setup(service => service.GetBankPDFAsync(paymentRequest))
            .ReturnsAsync(expectedBytes);

        // Act
        var result = await _controller.GetBankInvoicePdf(paymentRequest);

        // Assert
        var fileResult = Assert.IsAssignableFrom<FileContentResult>(result);
        Assert.Equal(expectedBytes, fileResult.FileContents);
        Assert.Equal("application/pdf", fileResult.ContentType);
        Assert.Equal("invoice.pdf", fileResult.FileDownloadName);
    }

    /// <summary>
    /// Test for GetBankInvoicePdf method with unknown payment method.
    /// </summary>
    [Fact]
    public async Task GetBankInvoicePdf_WithUnknownMethod_ThrowsArgumentException()
    {
        // Arrange
        var paymentRequest = new PaymentRequestDTO { Method = "Unknown" };

        // Act and Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await _controller.GetBankInvoicePdf(paymentRequest));
    }
}
