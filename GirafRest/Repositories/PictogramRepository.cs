using System.Collections.Generic;
using System.Linq;
using GirafRest.Models;
using GirafRest.IRepositories;
using GirafRest.Data;
using System.Threading;

namespace GirafRest.Repositories
{
    public class PictogramRepository : Repository<Pictogram>, IPictogramRepository
    {
        public PictogramRepository(GirafDbContext context) : base(context)
        {
        }
        
    }
    
}