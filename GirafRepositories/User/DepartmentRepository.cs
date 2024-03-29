using GirafEntities.User;
using GirafEntities.User.DTOs;
using GirafRepositories.Interfaces;
using GirafRepositories.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GirafRepositories.User
{
    public class DepartmentRepository : Repository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(GirafDbContext context) : base(context)
        {
        }

        public Department GetDepartmentByUser(GirafUser user)
        {
            return Context.Departments.FirstOrDefault(d => d.Key == user.DepartmentKey);
        }
        
        public Department GetDepartmentById(long departmentId)
        {
            return Context.Departments.FirstOrDefault(dep => dep.Key == departmentId);
        }

        public Task<List<DepartmentNameDTO>> GetDepartmentNames()
        {
            return Context.Departments.Select(dep => new DepartmentNameDTO(dep.Key, dep.Name)).ToListAsync();
        }

        public async Task<Department> GetDepartmentMembers(long departmentId)
        {
            var department = Context.Departments.Where(dep => dep.Key == departmentId);
            //.Include is used to get information on members aswell when getting the Department
            return await department.Include(dep => dep.Members).Include(dep => dep.Resources).FirstOrDefaultAsync();
        }

        public async Task RemoveDepartment(Department department)
        {
            Context.Remove(department);
            Context.SaveChanges();
        }

        public async Task Update(Department department)
        {
            Context.SaveChanges();
        }

        public GirafUser GetUserByDepartment(Department department, GirafUser currentUser)
        {
            return Context.Users.Include(a => a.Department).FirstOrDefault(d => d.UserName == currentUser.UserName);
        }
        public async Task AddDepartment(Department department)
        {
            Context.Departments.Add(department);
            Context.SaveChanges();

        }

        public async Task AddDepartmentResource(DepartmentResource dr)
        {
            await Context.DepartmentResources.AddAsync(dr);
        }

        public async  Task<GirafRoles> GetUserRole(RoleManager<GirafRole> roleManager,
            UserManager<GirafUser> userManager,
            GirafUser user)
        {
            return await roleManager.findUserRole(userManager, user);
        }
        public string GetCitizenRoleID()
        {
            return Context.Roles.Where(r => r.Name == GirafRole.Citizen).Select(c => c.Id).FirstOrDefault();
        }

        public IQueryable<string> GetUsersWithRoleID(string roleCitizenId)
        {
            return Context.UserRoles.Where(u => u.RoleId == roleCitizenId).Select(r => r.UserId).Distinct();
        }
    }
}