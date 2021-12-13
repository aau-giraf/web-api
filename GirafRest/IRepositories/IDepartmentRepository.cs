using GirafRest.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GirafRest.IRepositories
{
    public interface IDepartmentRepository : IRepository<Department>
    {
        Department GetDepartmentById(long departmentId);

        Task<List<DepartmentNameDTO>> GetDepartmentNames();
    }
}
