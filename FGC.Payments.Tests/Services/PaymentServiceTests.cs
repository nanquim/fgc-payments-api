using FluentAssertions;
using MassTransit;
using Moq;
using FGC.Payments.Application.Contracts.Events;
using FGC.Payments.Application.Services;
using FGC.Payments.Domain.Entities;
using FGC.Payments.Domain.Enums;
using FGC.Payments.Domain.Repositories;

namespace FGC.Payments.Tests.Services;

public class PaymentServiceTests
{
    private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;
    private readonly PaymentService _paymentService;

    public PaymentServiceTests()
    {
        _paymentRepositoryMock = new Mock<IPaymentRepository>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _paymentService = new PaymentService(_paymentRepositoryMock.Object, _publishEndpointMock.Object);
    }

    [Fact]
    public async Task Dado_PedidoValido_Quando_ProcessAsync_Entao_SalvaPaymentNoRepositorio()
    {
        // Dado
        var order = new OrderPlacedEvent(
            OrderId: Guid.NewGuid(),
            UserId: Guid.NewGuid(),
            GameId: Guid.NewGuid(),
            GameTitle: "Cyber Quest",
            Price: 49.90m,
            PlacedAt: DateTime.UtcNow);

        // Quando
        await _paymentService.ProcessAsync(order);

        // Então
        _paymentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Payment>()), Times.Once);
    }

    [Fact]
    public async Task Dado_PrecoPositivo_Quando_ProcessAsync_Entao_PublicaPaymentProcessedEventApproved()
    {
        // Dado
        var order = new OrderPlacedEvent(
            OrderId: Guid.NewGuid(),
            UserId: Guid.NewGuid(),
            GameId: Guid.NewGuid(),
            GameTitle: "Cyber Quest",
            Price: 49.90m,
            PlacedAt: DateTime.UtcNow);

        // Quando
        await _paymentService.ProcessAsync(order);

        // Então
        _publishEndpointMock.Verify(
            p => p.Publish(It.Is<PaymentProcessedEvent>(e =>
                e.OrderId == order.OrderId &&
                e.UserId == order.UserId &&
                e.GameId == order.GameId &&
                e.Status == PaymentStatus.Approved.ToString()),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_PedidoValido_Quando_ProcessAsync_Entao_PublicaPaymentProcessedEvent()
    {
        // Dado
        var order = new OrderPlacedEvent(
            OrderId: Guid.NewGuid(),
            UserId: Guid.NewGuid(),
            GameId: Guid.NewGuid(),
            GameTitle: "Jogo X",
            Price: 10m,
            PlacedAt: DateTime.UtcNow);

        // Quando
        await _paymentService.ProcessAsync(order);

        // Então
        _publishEndpointMock.Verify(
            p => p.Publish(It.IsAny<PaymentProcessedEvent>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_OrderId_Quando_GetByOrderIdAsync_Entao_RetornaPayment()
    {
        // Dado
        var orderId = Guid.NewGuid();
        var payment = new Payment(orderId, Guid.NewGuid(), Guid.NewGuid(), 59m);
        _paymentRepositoryMock.Setup(r => r.GetByOrderIdAsync(orderId)).ReturnsAsync(payment);

        // Quando
        var result = await _paymentService.GetByOrderIdAsync(orderId);

        // Então
        result.Should().NotBeNull();
        result!.OrderId.Should().Be(orderId);
    }

    [Fact]
    public async Task Dado_OrderIdInexistente_Quando_GetByOrderIdAsync_Entao_RetornaNull()
    {
        // Dado
        var orderId = Guid.NewGuid();
        _paymentRepositoryMock.Setup(r => r.GetByOrderIdAsync(orderId)).ReturnsAsync((Payment?)null);

        // Quando
        var result = await _paymentService.GetByOrderIdAsync(orderId);

        // Então
        result.Should().BeNull();
    }
}
