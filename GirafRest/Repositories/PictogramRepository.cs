using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GirafRest.Models;
using GirafRest.IRepositories;
using GirafRest.Data;
using GirafRest.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace GirafRest.Repositories
{
    /// <summary>
    /// Repository for the pictogram model.
    /// </summary>
    public class PictogramRepository : Repository<Pictogram>, IPictogramRepository
    {
        public PictogramRepository(GirafDbContext context) : base(context)
        {
        }
        public Task<Pictogram> FindResource(ResourceIdDTO resourceIdDTO)
        {
            return Context.Pictograms.Where(pf => pf.Id == resourceIdDTO.Id).FirstOrDefaultAsync();
        }
        
        public Task<Pictogram> FetchResourceWithId(ResourceIdDTO resourceIdDTO)
        {
            return Context.Pictograms.Where(f => f.Id == resourceIdDTO.Id).FirstOrDefaultAsync();
        }
        
    }
}