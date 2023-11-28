using GirafRepositories.Interfaces;
using GirafRepositories.Persistence;

namespace GirafRepositories.WeekPlanner
{
    public class ImageRepository : Repository<byte[]>, IImageRepository
    {
        public ImageRepository(GirafDbContext context) : base(context)
        {
            
        }

    }
    
}