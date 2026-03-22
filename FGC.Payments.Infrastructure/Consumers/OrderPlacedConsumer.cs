using MassTransit;
using FGC.Payments.Application.Contracts.Events;
using FGC.Payments.Application.Services;

namespace FGC.Payments.Infrastructure.Consumers;

public class OrderPlacedConsumer : IConsumer<OrderPlacedEvent>
{
    private readonly PaymentService _paymentService;

    public OrderPlacedConsumer(PaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    public async Task Consume(ConsumeContext<OrderPlacedEvent> context)
    {
        await _paymentService.ProcessAsync(context.Message);
    }
}
