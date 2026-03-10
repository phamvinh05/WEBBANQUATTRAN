using QuatTran.Application.DTOs;

public interface IPaymentService
{
    Task<IEnumerable<PaymentDto>> GetAllPaymentsAsync();
    Task<PaymentDto> GetByIdAsync(int paymentId);
    Task AddPaymentAsync(PaymentDto paymentDto);
    Task UpdatePaymentAsync(PaymentDto paymentDto);
    Task DeletePaymentAsync(int paymentId);
}
