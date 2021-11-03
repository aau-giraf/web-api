using System.Collections.Generic;
using System.Linq;
using GirafRest.Models;
using GirafRest.IRepositories;
using GirafRest.Data;

namespace GirafRest.Repositories
{
    public class AlternateNameRepository : Repository<AlternateName>, IAlternateNameRepository
    {
        public AlternateNameRepository(GirafDbContext context) : base(context) 
        { 
        }
    }
}
