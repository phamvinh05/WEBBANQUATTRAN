using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuatTran.Domain.Interfaces;
using QuatTran.Infrastructure.Data;

namespace QuatTran.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly QLQuatTranDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(QLQuatTranDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
        public async Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? filter = null,
        params string[] includes)
        {
            IQueryable<T> query = _dbSet.AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }

        public Task<IEnumerable<T>> GetByConditionAsync(Func<T, bool> predicate)
        {
            return Task.FromResult(_dbSet.AsEnumerable().Where(predicate));
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<T>> GetAllIncludingAsync(params string[] includes)
        {
            IQueryable<T> query = _dbSet.AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.ToListAsync();
        }

        public async Task<T?> GetByIdIncludingAsync(int id, params string[] includes)
        {
            IQueryable<T> query = _dbSet.AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            var entityType = typeof(T);
            var key = _context.Model.FindEntityType(entityType)?.FindPrimaryKey()?.Properties.FirstOrDefault();

            if (key == null)
                throw new Exception($"Không tìm thấy khóa chính của entity {entityType.Name}");

            var parameter = Expression.Parameter(entityType, "e");
            var property = Expression.Property(parameter, key.Name);
            var constant = Expression.Constant(id);
            var equal = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<T, bool>>(equal, parameter);

            return await query.FirstOrDefaultAsync(lambda);
        }


    }

}
