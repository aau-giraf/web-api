using GirafRest.Models;

namespace GirafRest.IRepositories
{
    public interface IDepartmentRepository : IRepository<GirafRest.Models.Department>
    {
        Department GetDepartmentById(long departmentId);
    }
}