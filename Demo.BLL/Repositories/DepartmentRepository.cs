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
    public class DepartmentRepository : GenericRepository<Department> ,IDepartmentRepository
    {
        //public readonly MVCAppDbContext dbContext;

        public DepartmentRepository(MVCAppDbContext dbContext_) : base(dbContext_)
        {
            //dbContext = dbContext_;
        }

        public IQueryable<Department> SearchForDepartmentByName(string name)
            => dbContext.Departments.Where(E => E.Name.ToLower().Contains(name.ToLower()));
    }
}
