using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using GirafRest.Models;
using GirafRest.IRepositories;
using GirafRest.Data;
using System.Threading.Tasks;

namespace GirafRest.Repositories
{
    public class AlternateNameRepository : Repository<AlternateName>, IAlternateNameRepository
    {
        public AlternateNameRepository(GirafDbContext context) : base(context) 
        {
        }

        public async Task<AlternateName> Get(GirafUser user, long pictogramID)
            => await Context.AlternateNames.FirstOrDefaultAsync(element =>
                element.Citizen == user && element.PictogramId == pictogramID);
    }
}
