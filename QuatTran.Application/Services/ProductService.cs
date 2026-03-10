using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuatTran.Application.DTOs;
using QuatTran.Application.Interfaces;
using QuatTran.Domain.Interfaces;
using QuatTran.Infrastructure;
using QuatTran.Infrastructure.Data;

namespace QuatTran.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _repository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly QLQuatTranDbContext _context;

        public ProductService(
            IRepository<Product> repository,
            IRepository<Category> categoryRepository,
            QLQuatTranDbContext context)
        {
            _repository = repository;
            _categoryRepository = categoryRepository;
            _context = context;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _repository.GetAllIncludingAsync(nameof(Product.ProductImages));
            return products.Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId,
                AdditionalImages = p.ProductImages
                    .Where(x => x.IsMain == false) 
                    .Select(x => new ProductImageDto
                    {
                        ProductImageId = x.ProductImageId,
                        ProductId = x.ProductId,
                        ImageUrl = x.ImageUrl,
                        IsMain = x.IsMain
                    })
                    .ToList()

            });
        }

        public async Task<ProductDto?> GetProductByIdAsync(int productId)
        {
            var p = await _repository.GetByIdIncludingAsync(productId, nameof(Product.ProductImages));
            if (p == null)
                return null;

            return new ProductDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId,
                AdditionalImages = p.ProductImages
                    .Where(x => x.IsMain == false)
                    .Select(x => new ProductImageDto
                    {
                        ProductImageId = x.ProductImageId,
                        ProductId = x.ProductId,
                        ImageUrl = x.ImageUrl,
                        IsMain = x.IsMain
                    })
                    .ToList()

            };
        }

        public async Task<int> AddProductAsync(ProductDto productDto)
        {
            var product = new Product
            {
                ProductName = productDto.ProductName,
                Description = productDto.Description,
                Price = productDto.Price,
                ImageUrl = productDto.ImageUrl,
                CategoryId = productDto.CategoryId
            };
            await _repository.AddAsync(product);
            await _repository.SaveChangesAsync();
            return product.ProductId;
        }

        public async Task UpdateProduct(ProductDto productDto)
        {
            var product = await _repository.GetByIdAsync(productDto.ProductId);
            if (product != null)
            {
                product.ProductName = productDto.ProductName;
                product.Description = productDto.Description;
                product.Price = productDto.Price;
                product.ImageUrl = productDto.ImageUrl;
                product.CategoryId = productDto.CategoryId;
                await _repository.UpdateAsync(product);
                await _repository.SaveChangesAsync();
            }
        }

        public async Task DeleteProductAsync(int productId)
        {
            var product = await _context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
            {
                throw new Exception("Không tìm thấy sản phẩm.");
            }

            if (product.ProductImages != null && product.ProductImages.Any())
            {
                foreach (var image in product.ProductImages)
                {
                    if (!string.IsNullOrEmpty(image.ImageUrl))
                    {
                        DeleteFileFromUploads(image.ImageUrl);
                    }
                }
                _context.ProductImages.RemoveRange(product.ProductImages);
            }

            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                DeleteFileFromUploads(product.ImageUrl);
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        private void DeleteFileFromUploads(string imageUrl)
        {
            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                imageUrl.TrimStart('/'));

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string? keyword, int? categoryId)
        {
            var products = await _repository.GetAllAsync();

            if (!string.IsNullOrEmpty(keyword))
                products = products.Where(p => p.ProductName.Contains(keyword, StringComparison.OrdinalIgnoreCase));

            if (categoryId.HasValue)
                products = products.Where(p => p.CategoryId == categoryId.Value);

            return products.Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId
            });
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return categories.Select(c => new CategoryDto
            {
                CategoryId = (int)c.CategoryId,
                Name = c.Name
            });
        }
    }
}
