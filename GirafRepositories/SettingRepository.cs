using GirafRepositories.Interfaces;
using GirafRepositories.Persistence;
using GirafRest.Models;

namespace GirafRepositories
{
    public class SettingRepository : Repository<Setting>, ISettingRepository
    {
        public SettingRepository(GirafDbContext context) : base(context)
        {
        }
    }
}