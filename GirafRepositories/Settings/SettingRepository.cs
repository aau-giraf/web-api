using GirafEntities.Settings;
using GirafRepositories.Interfaces;
using GirafRepositories.Persistence;

namespace GirafRepositories
{
    public class SettingRepository : Repository<Setting>, ISettingRepository
    {
        public SettingRepository(GirafDbContext context) : base(context)
        {
        }
    }
}