using QuatTran.Application.DTOs;

namespace QuatTran.Application.Interfaces
{
    public interface ICartItemService
    {
        Task<IEnumerable<CartItemDto>> GetAllCartItemsByUserAsync(int userId);
        Task AddToCartAsync(CartItemDto cartItem);
        Task UpdateQuantityAsync(int cartItemId, int quantity);
        Task RemoveItemAsync(int cartItemId);
        Task ClearCartAsync(int userId);
        Task AddToCartAsync(int userId, int productId, int quantity);

    }
}
