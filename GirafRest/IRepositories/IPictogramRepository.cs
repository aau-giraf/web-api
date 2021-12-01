using GirafRest.Models;
using GirafRest.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GirafRest.Models;
using GirafRest.Models.DTOs;

namespace GirafRest.IRepositories
{
    public interface IPictogramRepository : IRepository<Models.Pictogram>
    {
        public Task<Pictogram> getPictogramMatchingRelation(PictogramRelation pictogramRelation);
        public Task<Pictogram> GetPictogramWithName(string name);
        /// <summary>
        /// Add empty pictogram with specified name and access level.
        /// saves to database.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="access"></param>
        public Task<int> AddPictogramWith_NO_ImageHash(string name, AccessLevel access);
        Task<Pictogram> FetchResourceWithId(ResourceIdDTO resourceIdDTO);
        public Task<Pictogram> FindResource(ResourceIdDTO resourceIdDTO);
        public Task<Pictogram> GetPictogramWithID(long Id);
    }
}