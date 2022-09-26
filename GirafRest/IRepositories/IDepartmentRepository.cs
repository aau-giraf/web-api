using GirafRest.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.IRepositories
{
    public interface IDepartmentRepository : IRepository<Department>
    {
        Department GetDepartmentById(long departmentId);

        Task<List<DepartmentNameDTO>> GetDepartmentNames();

        Task<Department> GetDepartmentMembers(long departmentId);

        Task RemoveDepartment(Department department);

        Task Update(Department department);

        GirafUser GetUserByDepartment(Department department, GirafUser currentUser);

        string GetCitizenRoleID();

        IQueryable<string> GetUsersWithRoleID(string roleCitizenID);
    }
}