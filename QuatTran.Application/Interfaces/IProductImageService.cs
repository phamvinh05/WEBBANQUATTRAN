using System.Collections.Generic;
using System.Threading.Tasks;
using QuatTran.Application.DTOs;
using QuatTran.Domain.Interfaces;
using QuatTran.Infrastructure;

namespace QuatTran.Application.Interfaces
{
    public interface IProductImageService
    {
        Task<IEnumerable<ProductImageDto>> GetAllProductImagesAsync();
        Task<ProductImageDto> GetProductImageByIdAsync(int id);
        Task<IEnumerable<ProductImageDto>> GetProductImagesByProductIdAsync(int productId);
        Task AddProductImageAsync(ProductImageDto productImageDto);
        Task UpdateProductImageAsync(ProductImageDto productImageDto);
        Task DeleteProductImageAsync(int id);
    }
}
