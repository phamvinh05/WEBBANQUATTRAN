using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuatTran.Application.DTOs;
using QuatTran.Application.Interfaces;
using QuatTran.Domain.Interfaces;
using QuatTran.Infrastructure;

namespace QuatTran.Application.Services
{
    public class ProductImageService : IProductImageService
    {
        private readonly IRepository<ProductImage> _repository;

        public ProductImageService(IRepository<ProductImage> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ProductImageDto>> GetAllProductImagesAsync()
        {
            var productImages = await _repository.GetAllAsync();
            return productImages.Select(pi => new ProductImageDto
            {
                ProductImageId = pi.ProductImageId,
                ProductId = pi.ProductId,
                ImageUrl = pi.ImageUrl,
                IsMain = pi.IsMain
            });
        }

        public async Task<ProductImageDto> GetProductImageByIdAsync(int productImageId)
        {
            var productImage = await _repository.GetByIdAsync(productImageId);
            if (productImage == null)
                return null;

            return new ProductImageDto
            {
                ProductImageId = productImage.ProductImageId,
                ProductId = productImage.ProductId,
                ImageUrl = productImage.ImageUrl,
                IsMain = productImage.IsMain
            };
        }

        public async Task<IEnumerable<ProductImageDto>> GetProductImagesByProductIdAsync(int productId)
        {
            var productImages = await _repository.GetAllAsync();
            var filtered = productImages.Where(pi => pi.ProductId == productId);
            return filtered.Select(pi => new ProductImageDto
            {
                ProductImageId = pi.ProductImageId,
                ProductId = pi.ProductId,
                ImageUrl = pi.ImageUrl,
                IsMain = pi.IsMain
            });
        }

        public async Task AddProductImageAsync(ProductImageDto productImageDto)
        {
            var productImage = new ProductImage
            {
                ProductId = productImageDto.ProductId,
                ImageUrl = productImageDto.ImageUrl,
                IsMain = productImageDto.IsMain
            };
            await _repository.AddAsync(productImage);
            await _repository.SaveChangesAsync();
        }

        public async Task UpdateProductImageAsync(ProductImageDto productImageDto)
        {
            var productImage = await _repository.GetByIdAsync(productImageDto.ProductImageId);
            if (productImage != null)
            {
                productImage.ProductId = productImageDto.ProductId;
                productImage.ImageUrl = productImageDto.ImageUrl;
                productImage.IsMain = productImageDto.IsMain;
                await _repository.UpdateAsync(productImage);
                await _repository.SaveChangesAsync();
            }
        }

        public async Task DeleteProductImageAsync(int id)
        {
            var productImage = await _repository.GetByIdAsync(id);
            if (productImage != null)
            {
                await _repository.DeleteAsync(productImage);
                await _repository.SaveChangesAsync();
            }
        }
    }
}
