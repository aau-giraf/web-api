using GirafRest.Models;
using GirafRest.Models.Enums;
using Microsoft.AspNetCore.Identity;
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

        Task AddDepartmentResource(DepartmentResource dr);
        Task RemoveDepartment(Department department);

        Task<GirafRoles> GetUserRole(RoleManager<GirafRole> roleManager, UserManager<GirafUser> userManager, GirafUser user);
        Task Update(Department department);

        Task AddDepartment(Department department);


        GirafUser GetUserByDepartment(Department department, GirafUser currentUser);

        string GetCitizenRoleID();

        IQueryable<string> GetUsersWithRoleID(string roleCitizenID);
    }
}