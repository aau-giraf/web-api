using System.Collections.Generic;
using System.Linq;
using GirafRest.Models;
using GirafRest.IRepositories;
using GirafRest.Data;

namespace GirafRest.Repositories
{
    public class ImageRepository : Repository<byte[]>, IImageRepository
    {
        public ImageRepository(GirafDbContext context) : base(context)
        {

        }
    }
}