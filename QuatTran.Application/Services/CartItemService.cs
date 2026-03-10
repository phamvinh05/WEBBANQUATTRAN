using QuatTran.Application.DTOs;
using QuatTran.Application.Interfaces;
using QuatTran.Domain.Interfaces;
using QuatTran.Infrastructure;

namespace QuatTran.Application.Services
{
    public class CartItemService : ICartItemService
    {
        private readonly IRepository<CartItem> _cartRepository;
        private readonly IRepository<Product> _productRepository;

        public CartItemService(IRepository<CartItem> cartRepository, IRepository<Product> productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<CartItemDto>> GetAllCartItemsByUserAsync(int userId)
        {
            var items = await _cartRepository.GetAllIncludingAsync(nameof(CartItem.Product));
            return items
                .Where(x => x.UserId == userId)
                .Select(x => new CartItemDto
                {
                    CartItemId = x.CartItemId,
                    UserId = x.UserId,
                    ProductId = x.ProductId,
                    Quantity = x.Quantity,
                    DateCreated = x.DateCreated,
                    ProductPrice = x.Product?.Price ?? 0,
                    ProductName = x.Product?.ProductName,
                    ProductImageUrl = x.Product?.ImageUrl
                });
        }

        public async Task AddToCartAsync(CartItemDto cartItem)
        {
            var entity = new CartItem
            {
                UserId = cartItem.UserId,
                ProductId = cartItem.ProductId,
                Quantity = cartItem.Quantity,
                DateCreated = DateTime.Now
            };
            await _cartRepository.AddAsync(entity);
            await _cartRepository.SaveChangesAsync();
        }

        public async Task AddToCartAsync(int userId, int productId, int quantity)
        {
            var allItems = await _cartRepository.GetAllAsync();
            var existing = allItems.FirstOrDefault(x => x.UserId == userId && x.ProductId == productId);

            if (existing != null)
            {
                existing.Quantity += quantity;
                await _cartRepository.UpdateAsync(existing);
            }
            else
            {
                var newItem = new CartItem
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity,
                    DateCreated = DateTime.Now
                };
                await _cartRepository.AddAsync(newItem);
            }

            await _cartRepository.SaveChangesAsync();
        }

        public async Task UpdateQuantityAsync(int cartItemId, int quantity)
        {
            var item = await _cartRepository.GetByIdAsync(cartItemId);
            if (item != null)
            {
                item.Quantity = quantity;
                await _cartRepository.UpdateAsync(item);
                await _cartRepository.SaveChangesAsync();
            }
        }

        public async Task RemoveItemAsync(int cartItemId)
        {
            var item = await _cartRepository.GetByIdAsync(cartItemId);
            if (item != null)
            {
                await _cartRepository.DeleteAsync(item);
                await _cartRepository.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(int userId)
        {
            var allItems = await _cartRepository.GetAllAsync();
            var userItems = allItems.Where(x => x.UserId == userId).ToList();

            foreach (var item in userItems)
            {
                await _cartRepository.DeleteAsync(item);
            }

            await _cartRepository.SaveChangesAsync();
        }
    }
}
