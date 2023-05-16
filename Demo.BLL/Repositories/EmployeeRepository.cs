using Demo.BLL.Interfaces;
using Demo.DAL.Contexts;
using Demo.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Repositories
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        //public readonly MVCAppDbContext dbContext;

        public EmployeeRepository(MVCAppDbContext dbContext_):base(dbContext_)
        {
            //dbContext = dbContext_;
        }

        public IQueryable<Employee> GetEmployeesByAddress(string address)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Employee> SearchEmployeeByName(string name)
            => dbContext.Employees.Where(E => E.Name.ToLower().Contains(name.ToLower()));
        
    }
}
