using FluentAssertions;
using MassTransit;
using Moq;
using FGC.Payments.Application.Contracts.Events;
using FGC.Payments.Application.Services;
using FGC.Payments.Domain.Repositories;
using FGC.Payments.Infrastructure.Consumers;

namespace FGC.Payments.Tests.Consumers;

public class OrderPlacedConsumerTests
{
    private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;
    private readonly OrderPlacedConsumer _consumer;

    public OrderPlacedConsumerTests()
    {
        _paymentRepositoryMock = new Mock<IPaymentRepository>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        var paymentService = new PaymentService(_paymentRepositoryMock.Object, _publishEndpointMock.Object);
        _consumer = new OrderPlacedConsumer(paymentService);
    }

    [Fact]
    public async Task Dado_OrderPlacedEvent_Quando_Consume_Entao_ProcessaPagamento()
    {
        // Dado
        var ev = new OrderPlacedEvent(
            OrderId: Guid.NewGuid(),
            UserId: Guid.NewGuid(),
            GameId: Guid.NewGuid(),
            GameTitle: "Cyber Quest",
            Price: 49.90m,
            PlacedAt: DateTime.UtcNow);

        var contextMock = new Mock<ConsumeContext<OrderPlacedEvent>>();
        contextMock.Setup(c => c.Message).Returns(ev);

        // Quando
        await _consumer.Consume(contextMock.Object);

        // Então
        _paymentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.Payment>()), Times.Once);
        _publishEndpointMock.Verify(
            p => p.Publish(It.IsAny<PaymentProcessedEvent>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
