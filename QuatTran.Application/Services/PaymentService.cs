using QuatTran.Application.DTOs;
using QuatTran.Domain.Interfaces;
using QuatTran.Infrastructure;

public class PaymentService : IPaymentService
{
    private readonly IRepository<Payment> _repository;

    public PaymentService(IRepository<Payment> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<PaymentDto>> GetAllPaymentsAsync()
    {
        var payments = await _repository.GetAllAsync();
        return payments.Select(p => new PaymentDto
        {
            PaymentId = p.PaymentId,
            OrderId = p.OrderId,
            PaymentDate = p.PaymentDate,
            Amount = p.Amount,
            PaymentMethod = p.PaymentMethod,
            PaymentStatus = p.PaymentStatus
        });
    }

    public async Task<PaymentDto> GetByIdAsync(int paymentId)
    {
        var p = await _repository.GetByIdAsync(paymentId);
        if (p == null) return null;

        return new PaymentDto
        {
            PaymentId = p.PaymentId,
            OrderId = p.OrderId,
            PaymentDate = p.PaymentDate,
            Amount = p.Amount,
            PaymentMethod = p.PaymentMethod,
            PaymentStatus = p.PaymentStatus
        };
    }

    public async Task AddPaymentAsync(PaymentDto dto)
    {
        var payment = new Payment
        {
            OrderId = dto.OrderId,
            PaymentDate = dto.PaymentDate ?? DateTime.UtcNow,
            Amount = dto.Amount,
            PaymentMethod = dto.PaymentMethod,
            PaymentStatus = dto.PaymentStatus
        };

        await _repository.AddAsync(payment);
        await _repository.SaveChangesAsync();
    }

    public async Task UpdatePaymentAsync(PaymentDto dto)
    {
        var p = await _repository.GetByIdAsync(dto.PaymentId);
        if (p != null)
        {
            p.PaymentDate = dto.PaymentDate;
            p.Amount = dto.Amount;
            p.PaymentMethod = dto.PaymentMethod;
            p.PaymentStatus = dto.PaymentStatus;

            await _repository.UpdateAsync(p);
            await _repository.SaveChangesAsync();
        }
    }

    public async Task DeletePaymentAsync(int paymentId)
    {
        var p = await _repository.GetByIdAsync(paymentId);
        if (p != null)
        {
            await _repository.DeleteAsync(p);
            await _repository.SaveChangesAsync();
        }
    }
}
