using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuatTran.Application.DTOs;
using QuatTran.Application.Interfaces;
using QuatTran.Domain.Interfaces;
using QuatTran.Infrastructure;

namespace QuatTran.Application.Services
{
    public class CategoryService : ICategoryService
    {
        public readonly IRepository<Category> _repository;
        public CategoryService(IRepository<Category> repository)
        {
            _repository = repository;
        }

        public async Task AddCategoryAsync(CategoryDto categoryDto)
        {
            var category = new Category()
            {
                CategoryId = categoryDto.CategoryId,
                Name = categoryDto.Name,
            };
            await _repository.AddAsync(category);
            await _repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoryAsync()
        {
            var categories = await _repository.GetAllAsync();
            return categories.Select(c => new CategoryDto()
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
            });
        }

        public async Task<CategoryDto> GetByIdAsync(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            return new CategoryDto()
            {
                CategoryId = category.CategoryId,
                Name = category.Name
            };
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category != null)
            {
                await _repository.DeleteAsync(category);
                await _repository.SaveChangesAsync();
            }
        }
        public async Task UpdateCategory(CategoryDto categoryDto)
        {
            var category = await _repository.GetByIdAsync(categoryDto.CategoryId);
            if (category != null)
            {
                category.CategoryId = categoryDto.CategoryId;
                category.Name = categoryDto.Name;
                await _repository.UpdateAsync(category);
                await _repository.SaveChangesAsync();
            }
        }
    }
}
