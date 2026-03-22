using FGC.Payments.Domain.Entities;

namespace FGC.Payments.Domain.Repositories;

public interface IPaymentRepository
{
    Task AddAsync(Payment payment);
    Task<Payment?> GetByIdAsync(Guid id);
    Task<Payment?> GetByOrderIdAsync(Guid orderId);
    Task UpdateAsync(Payment payment);
}
