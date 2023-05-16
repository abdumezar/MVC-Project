using Demo.BLL.Interfaces;
using Demo.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MVCAppDbContext dbContext;

        public IEmployeeRepository employeeRepository { get; set; }
        public IDepartmentRepository departmentRepository { get; set; }

        // Ask CLR to create object of dbContext
        public UnitOfWork(MVCAppDbContext dbContext_)
        {
            employeeRepository = new EmployeeRepository(dbContext_);
            departmentRepository = new DepartmentRepository(dbContext_);
            dbContext = dbContext_;
        }

        public async Task<int> Complete()
            => await dbContext.SaveChangesAsync();

        public void Dispose()
        {
            dbContext.Dispose();
        }
    }
}
