using System.Collections.Generic;
using System.Threading.Tasks;
using QuatTran.Application.DTOs;

namespace QuatTran.Application.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto> GetProductByIdAsync(int productId);
        Task<int> AddProductAsync(ProductDto productDto);
        Task UpdateProduct(ProductDto productDto);
        Task DeleteProductAsync(int productId);
        Task<IEnumerable<ProductDto>> SearchProductsAsync(string? keyword, int? categoryId);
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
    }
}
