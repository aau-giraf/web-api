using GirafEntities.User;

namespace GirafServices.User;

public interface IDepartmentService
{
    Task<Department> GetDepartmentByUser(GirafUser user);
}