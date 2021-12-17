using GirafRest.Data;
using GirafRest.IRepositories;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.Repositories
{
    /// <inheritdoc cref="GirafRest.IRepositories.IPictogramRepository"/>
    public class PictogramRepository : Repository<Pictogram>, IPictogramRepository
    {
        /// <summary>
        /// Domain specific repository implementation facade for the DBContext.
        /// </summary>
        /// <param name="context">The context to operate on</param>
        public PictogramRepository(GirafDbContext context)
            : base(context)
        { }

        /// <inheritdoc />
        public Pictogram GetByID(long pictogramID)
            => Get(pictogramID);

        public Task<Pictogram> getPictogramMatchingRelation(PictogramRelation pictogramRelation)
        {
            return Context.Pictograms.FirstOrDefaultAsync(p => p.Id == pictogramRelation.PictogramId);
        }

        public Task<Pictogram> GetPictogramWithName(string name)
        {
            return Context.Pictograms.FirstOrDefaultAsync(r => r.Title == name);
        }
        public async Task<int> AddPictogramWith_NO_ImageHash(string name, AccessLevel access)
        {
            Context.Pictograms.Add(new Pictogram(name, access));
            return await Context.SaveChangesAsync();
        }
        public Task<Pictogram> FetchResourceWithId(ResourceIdDTO resourceIdDTO)
        {
            return Context.Pictograms.Where(f => f.Id == resourceIdDTO.Id).FirstOrDefaultAsync();
        }
        public Task<Pictogram> FindResource(ResourceIdDTO resourceIdDTO)
        {
            return Context.Pictograms.Where(pf => pf.Id == resourceIdDTO.Id).FirstOrDefaultAsync();
        }
        public Task<Pictogram> GetPictogramWithID(long Id)
        {
            return Context.Pictograms.FirstOrDefaultAsync(p => p.Id == Id);
        }
    }
}