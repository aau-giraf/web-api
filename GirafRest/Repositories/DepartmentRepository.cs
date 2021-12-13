using GirafRest.Data;
using GirafRest.IRepositories;
using GirafRest.Models;
using System.Linq;

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
    }
}