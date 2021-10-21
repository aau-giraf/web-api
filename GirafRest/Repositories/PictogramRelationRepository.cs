﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using GirafRest.Models;
using GirafRest.IRepositories;
using GirafRest.Data;
using System.Threading.Tasks;

namespace GirafRest.Repositories
{
    public class PictogramRelationRepository : Repository<PictogramRelation>, IPictogramRelationRepository
    {
        public PictogramRelationRepository(GirafDbContext context) : base(context)
        {
        }

        public ICollection<PictogramRelation> GetWithPictogram(int activityID)
            => Context.PictogramRelations
                .Include(pictogram => pictogram.Pictogram)
                .Where(pr => pr.ActivityId == activityID)
                .ToList();
    }
}
