using System.Collections.Generic;
using System.Linq;
using GirafRest.Models;
using GirafRest.IRepositories;
using GirafRest.Data;

namespace GirafRest.Repositories
{
    public class PictogramRepository : Repository<Pictogram>, IPictogramRepository
    {
        public PictogramRepository(GirafDbContext context) : base(context)
        {

        }

        Pictogram GetPictogramByID(long pictogramID) => return Context.Pictograms.FirstOrDefaultAsync(id => id.Id == picId);
}