using System;
using GirafRest.IRepositories;
using GirafRest.Data;

namespace GirafRest.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GirafDbContext _context;

        public UnitOfWork(GirafDbContext context)
        {
            _context = context;
            AlternateNames = new AlternateNameRepository(_context);
            Departments = new DepartmentRepository(_context);
            GirafRoles = new GirafRoleRepository(_context);
            GirafUsers = new GirafUserRepository(_context);
            Pictograms = new PictogramRepository(_context);
            Settings = new SettingRepository(_context);
            Timers = new TimerRepository(_context);
            Weeks = new WeekRepository(_context);
            WeekBases = new WeekBaseRepository(_context);
            WeekDayColors = new WeekDayColorRepository(_context);
            WeekTemplates = new WeekTemplateRepository(_context);
        }

        public IAlternateNameRepository AlternateNames { get; private set; }

        public IDepartmentRepository Departments { get; private set; }

        public IGirafRoleRepository GirafRoles { get; private set; }

        public IGirafUserRepository GirafUsers { get; private set; }

        public IPictogramRepository Pictograms { get; private set; }

        public ISettingRepository Settings { get; private set; }

        public ITimerRepository Timers { get; private set; }

        public IWeekBaseRepository WeekBases { get; private set; }

        public IWeekDayColorRepository WeekDayColors { get; private set; }

        public IWeekRepository Weeks { get; private set; }

        public IWeekTemplateRepository WeekTemplates { get; private set; }

        public int Complete(){
            
            return _context.SaveChanges();
        }

        public void Dispose(){
            
            _context.Dispose();
        }
    }
}