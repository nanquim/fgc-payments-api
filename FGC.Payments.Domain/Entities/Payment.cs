using FGC.Payments.Domain.Enums;

namespace FGC.Payments.Domain.Entities;

public class Payment
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid GameId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateTime ProcessedAt { get; private set; }

    protected Payment() { }

    public Payment(Guid orderId, Guid userId, Guid gameId, decimal amount)
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        UserId = userId;
        GameId = gameId;
        Amount = amount;
        Status = PaymentStatus.Pending;
        ProcessedAt = DateTime.UtcNow;
    }

    public void Approve()
    {
        Status = PaymentStatus.Approved;
        ProcessedAt = DateTime.UtcNow;
    }

    public void Reject()
    {
        Status = PaymentStatus.Rejected;
        ProcessedAt = DateTime.UtcNow;
    }
}
