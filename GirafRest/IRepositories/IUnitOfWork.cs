using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace GirafRest.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        IAlternateNameRepository AlternateNames { get; }
        IDepartmentRepository Departments { get; }
        IGirafRoleRepository GirafRoles { get; }
        IGirafUserRepository GirafUsers { get; }
        IPictogramRepository Pictograms { get; }
        ISettingRepository Settings { get; }
        ITimerRepository Timers { get; }
        IWeekBaseRepository WeekBases { get; }
        IWeekDayColorRepository WeekDayColors { get; }
        IWeekRepository Weeks { get; }
        IWeekTemplateRepository WeekTemplates { get; }
       //other repositories
       
       int Complete();
    }
}