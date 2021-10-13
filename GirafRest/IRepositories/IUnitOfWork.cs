using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace GirafRest.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {


        IUserRepository Users { get; }

        IWeekRepository Weeks { get; }
       
       //other repositories
       
       int Complete();
    }
}