using System.Collections.Generic;
using System.Linq;
using GirafRest.Models;
using GirafRest.IRepositories;
using GirafRest.Data;
using System.Threading.Tasks;

namespace GirafRest.Repositories
{
    public class PictogramRepository : Repository<Pictogram>, IPictogramRepository
    {
        public PictogramRepository(GirafDbContext context) : base(context)
        {

        }

        public Task<Pictogram> getPictogramMatchingRelation(PictogramRelation pictogramRelation)
        {

            return Context.Pictograms.FirstOrDefault(p => p.Id == pictogramRelation.PictogramId);
        }




    }
}