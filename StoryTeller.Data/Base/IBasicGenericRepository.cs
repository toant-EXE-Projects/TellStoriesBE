using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data.Base
{
    public interface IBasicGenericRepository<T>
    {
        Task<List<T>> GetAllAsync();

        Task<T?> GetByIdAsync(Guid id);

        Task<T?> GetByIdAsync(string id);

        Task<T?> GetByIdAsync(int id);

        Task CreateAsync(T entity);

        void Update(T entity);

        void Remove(T entity);
    }
}
