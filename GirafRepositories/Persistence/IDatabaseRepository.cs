using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GirafRepositories.Persistence
{
    public interface IDatabaseRepository
    {
        void EnsureCreated();
        Task SaveChangesAsync();
        Task<bool> AnyAsync<T>() where T : class;
        Task AddAsync<T>(T entity) where T : class;
        Task ExecuteSqlRawAsync(string sql);
        Task<List<T>> ToListAsync<T>() where T : class;
        Task<T> FirstAsync<T>(Expression<Func<T, bool>> predicate) where T : class;


    }
}
