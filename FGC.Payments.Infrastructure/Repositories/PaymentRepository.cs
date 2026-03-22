using Microsoft.EntityFrameworkCore;
using FGC.Payments.Domain.Entities;
using FGC.Payments.Domain.Repositories;
using FGC.Payments.Infrastructure.Persistence.Contexts;

namespace FGC.Payments.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly ApplicationDbContext _context;

    public PaymentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Payment payment)
    {
        await _context.Payments.AddAsync(payment);
        await _context.SaveChangesAsync();
    }

    public async Task<Payment?> GetByIdAsync(Guid id)
        => await _context.Payments.FindAsync(id);

    public async Task<Payment?> GetByOrderIdAsync(Guid orderId)
        => await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);

    public async Task UpdateAsync(Payment payment)
    {
        _context.Payments.Update(payment);
        await _context.SaveChangesAsync();
    }
}
