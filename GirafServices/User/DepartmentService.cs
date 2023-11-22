using GirafEntities.User;
using GirafRepositories.Interfaces;

namespace GirafServices.User;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;

    public DepartmentService(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    public async Task<Department> GetDepartmentByUser(GirafUser user)
    {
        return _departmentRepository.GetDepartmentByUser(user);
    }
}