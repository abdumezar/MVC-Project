using Demo.BLL.Interfaces;
using Demo.DAL.Contexts;
using Demo.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        // Composition
        private protected readonly MVCAppDbContext dbContext;

        // Dependency Injection
        public GenericRepository(MVCAppDbContext _dbContext)
        {
            dbContext = _dbContext;
        }

        public async Task AddAsync(T item)
        {
            await dbContext.Set<T>().AddAsync(item);
        }

        public void Delete(T item)
        {
            dbContext.Set<T>().Remove(item);
        }  

        public async Task<T> GetAsync(int id)
            => await dbContext.Set<T>().FindAsync(id);

        // Specification Design Pattern
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            if(typeof(T) == typeof(Employee))
                return (IEnumerable<T>) await dbContext.Employees.Include(E => E.Department).ToListAsync();
            else
                return await dbContext.Set<T>().ToListAsync();
        }

        public void Update(T item)
        {
            dbContext.Set<T>().Update(item);
        }
    }
}
