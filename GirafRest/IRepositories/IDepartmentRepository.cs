using GirafRest.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GirafRest.IRepositories
{
    public interface IDepartmentRepository : IRepository<GirafRest.Models.Department>
    {
        Department GetDepartmentById(long departmentId);
    }
}