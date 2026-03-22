using FGC.Payments.Domain.Entities;
using FGC.Payments.Domain.Repositories;
using FGC.Payments.Application.Contracts.Events;
using MassTransit;

namespace FGC.Payments.Application.Services;

public class PaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public PaymentService(IPaymentRepository paymentRepository, IPublishEndpoint publishEndpoint)
    {
        _paymentRepository = paymentRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task ProcessAsync(OrderPlacedEvent order)
    {
        var payment = new Payment(order.OrderId, order.UserId, order.GameId, order.Price);

        // Simulação: aprovar sempre (pode ser expandido com regras reais)
        var approved = order.Price >= 0;

        if (approved)
            payment.Approve();
        else
            payment.Reject();

        await _paymentRepository.AddAsync(payment);

        await _publishEndpoint.Publish(new PaymentProcessedEvent(
            payment.OrderId,
            payment.UserId,
            payment.GameId,
            payment.Status.ToString(),
            payment.ProcessedAt));
    }

    public async Task<Payment?> GetByOrderIdAsync(Guid orderId)
        => await _paymentRepository.GetByOrderIdAsync(orderId);
}
