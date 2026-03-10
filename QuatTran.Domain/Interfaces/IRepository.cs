using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace QuatTran.Domain.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? filter = null,
        params string[] includes);

        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity); 
        Task SaveChangesAsync();
        Task<IEnumerable<T>> GetAllIncludingAsync(params string[] includes);
        Task<T?> GetByIdIncludingAsync(int id, params string[] includes);
        Task<IEnumerable<T>> GetByConditionAsync(Func<T, bool> predicate);


    }
}
