using QuatTran.Application.DTOs;

namespace QuatTran.Application.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(int userId);
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto?> GetOrderByIdAsync(int orderId);
        Task<int> AddOrderAsync(OrderDto orderDto);
        Task UpdateOrderStatusAsync(int orderId, string status);
        Task DeleteOrderAsync(int orderId);
        Task UpdateOrderShipperAsync(int orderId, int shipperId);

    }

}
