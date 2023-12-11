using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GirafRepositories.Persistence
{
    public class DatabaseRepository : IDatabaseRepository
    {
        public DatabaseRepository(GirafDbContext context) { 
            _context = context;
        }

        private readonly GirafDbContext _context;
        public void EnsureCreated()
        {
            _context.Database.EnsureCreated();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AnyAsync<T>() where T : class
        {
            return await _context.Set<T>().AnyAsync();
        }

        public async Task AddAsync<T>(T entity) where T : class
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public async Task ExecuteSqlRawAsync(string sql)
        {
            await _context.Database.ExecuteSqlRawAsync(sql);
        }

        public async Task<List<T>> ToListAsync<T>() where T : class
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> FirstAsync<T> (Expression<Func<T, bool>> predicate) where T : class
        {
            return await _context.Set<T>().FirstAsync(predicate);
        }

    }
}
