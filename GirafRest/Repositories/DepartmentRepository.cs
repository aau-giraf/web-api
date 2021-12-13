using GirafRest.Data;
using GirafRest.IRepositories;
using GirafRest.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.Repositories
{
    public class DepartmentRepository : Repository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(GirafDbContext context) : base(context)
        {
        }

        public Department GetDepartmentById(long departmentId)
        {
            return Context.Departments.FirstOrDefault(dep => dep.Key == departmentId);
        }

        public Task<List<DepartmentNameDTO>> GetDepartmentNames()
        {
            return Context.Departments.Select(dep => new DepartmentNameDTO(dep.Key, dep.Name)).ToListAsync();
        }
    }
}
