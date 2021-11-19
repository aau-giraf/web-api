using System.Collections.Generic;
using System.Linq;
using GirafRest.Models;
using GirafRest.IRepositories;
using GirafRest.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GirafRest.Repositories
{
    public class DepartmentRepository : Repository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(GirafDbContext context) : base(context)
        {
        }

        public Department GetDepartmentById(long departmentId) 
            => Context.Departments.FirstOrDefault(dep => dep.Key == departmentId);
    }
}