using GirafEntities.User;
using GirafEntities.User.DTOs;
using Microsoft.AspNetCore.Identity;

namespace GirafRepositories.Interfaces
{
    public interface IDepartmentRepository : IRepository<Department>
    {
        Department GetDepartmentById(long departmentId);
        Department GetDepartmentByUser(GirafUser user);
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