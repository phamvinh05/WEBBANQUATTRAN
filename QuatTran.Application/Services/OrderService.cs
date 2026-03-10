using QuatTran.Application.DTOs;
using QuatTran.Application.Interfaces;
using QuatTran.Domain.Interfaces;
using QuatTran.Infrastructure;

namespace QuatTran.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _orderRepository;

        public OrderService(IRepository<Order> orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(int userId)
        {
            var orders = await _orderRepository.GetAllAsync(
                o => o.UserId == userId,
                "OrderItems",
                "OrderItems.Product"
            );

            return orders.Select(MapOrderToDto).ToList();
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllIncludingAsync(
                nameof(Order.OrderItems),
                "OrderItems.Product"
            );

            return orders.Select(MapOrderToDto).ToList();
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdIncludingAsync(
                orderId,
                "OrderItems",
                "OrderItems.Product"
            );

            return order == null ? null : MapOrderToDto(order);
        }

        public async Task<int> AddOrderAsync(OrderDto orderDto)
        {
            var order = new Order
            {
                UserId = orderDto.UserId,
                OrderDate = DateTime.Now,
                TotalAmount = orderDto.TotalAmount,
                FullName = orderDto.FullName,
                Address = orderDto.Address,
                Phone = orderDto.Phone,
                ShipperId = orderDto.ShipperId,
                ShippingStatus = orderDto.ShippingStatus ?? "Chờ xử lý",
                OrderItems = orderDto.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();
            return order.OrderId;
        }

        public async Task UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new Exception("Không tìm thấy đơn hàng.");

            order.ShippingStatus = status;
            await _orderRepository.UpdateAsync(order);
            await _orderRepository.SaveChangesAsync();
        }

        public async Task DeleteOrderAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order != null)
            {
                await _orderRepository.DeleteAsync(order);
                await _orderRepository.SaveChangesAsync();
            }
        }

        private static OrderDto MapOrderToDto(Order o)
        {
            return new OrderDto
            {
                OrderId = o.OrderId,
                UserId = o.UserId,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                FullName = o.FullName,
                Address = o.Address,
                Phone = o.Phone,
                ShipperId = o.ShipperId,
                ShippingStatus = o.ShippingStatus,
                Items = o.OrderItems.Select(i => new OrderItemDto
                {
                    OrderItemId = i.OrderItemId,
                    OrderId = i.OrderId,
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    ProductName = i.Product?.ProductName,
                    ProductImageUrl = i.Product?.ImageUrl
                }).ToList()
            };
        }
        public async Task UpdateOrderShipperAsync(int orderId, int shipperId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) throw new Exception("Không tìm thấy đơn hàng.");

            order.ShipperId = shipperId;

            await _orderRepository.UpdateAsync(order);
            await _orderRepository.SaveChangesAsync();
        }

    }
}
