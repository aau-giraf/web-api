using GirafRest.Data;
using GirafRest.IRepositories;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.Repositories
{
    /// <inheritdoc cref="IPictogramRepository"/>

    /// <summary>
    /// Repository for the pictogram model.
    /// </summary>
    public class PictogramRepository : Repository<Pictogram>, IPictogramRepository
    {
        /// <summary>
        /// Domain specific repository implementation facade for the DBContext.
        /// </summary>
        /// <param name="context">The context to operate on</param>
        public PictogramRepository(GirafDbContext context) : base(context)
        {
        }

        /// <inheritdoc />
        public Pictogram GetByID(long pictogramID) => Get(pictogramID);

        public Task<Pictogram> getPictogramMatchingRelation(PictogramRelation pictogramRelation)
        {
            return Context.Pictograms.FirstOrDefaultAsync(p => p.Id == pictogramRelation.PictogramId);
        }

        public Task<Pictogram> GetPictogramsById(long pictogramID)
        {
            return Context
                .Pictograms
                .Where(pictogram => pictogram.Id == pictogramID)
                .FirstOrDefaultAsync();
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
        public async Task CreatePictorgram(Pictogram pict)
        {
            await Context.Pictograms.AddAsync(pict);
            await Context.SaveChangesAsync();
        }

        public async Task RemovePictogram(Pictogram pict)
        {
            Context.Pictograms.Remove(pict);

            await Context.SaveChangesAsync();
        }



        public async Task UpdatePictogram(Pictogram pict)
        {

            Context.Pictograms.Update(pict);
            await Context.SaveChangesAsync();
        }


        public async Task SaveState()
        {

            await Context.SaveChangesAsync();

        }
        public async Task RemoveRelations(Pictogram pict)
        {
            // Before we can remove a pictogram we must delete all its relations
            var userRessourceRelations = Context.UserResources.Where(ur => ur.PictogramKey == pict.Id);
            Context.UserResources.RemoveRange(userRessourceRelations);

            var depRessourceRelations = Context.DepartmentResources.Where(ur => ur.PictogramKey == pict.Id);
            Context.DepartmentResources.RemoveRange(depRessourceRelations);

            var pictogramRelations = Context.PictogramRelations.Where(relation => relation.PictogramId == pict.Id);
            Context.PictogramRelations.RemoveRange(pictogramRelations);

            await Context.SaveChangesAsync();
        }

        public IEnumerable<Pictogram> fetchPictogramsFromDepartmentStartsWithQuery(string query, GirafUser user)
        {
            return Context.Pictograms.Where(pictogram => (!string.IsNullOrEmpty(query)
                && pictogram.Title.ToLower().Replace(" ", string.Empty).StartsWith(query)
                || string.IsNullOrEmpty(query))
                && (pictogram.AccessLevel == AccessLevel.PUBLIC
                || pictogram.Users.Any(ur => ur.OtherKey == user.Id)
                || pictogram.Departments.Any(dr => dr.OtherKey == user.DepartmentKey)));
        }

        public IEnumerable<Pictogram> fetchPictogramsFromDepartmentsContainsQuery(string query, GirafUser user)
        {
            return Context.Pictograms.Where(pictogram => (!string.IsNullOrEmpty(query)
                && pictogram.Title.ToLower().Replace(" ", string.Empty).Contains(query)
                || string.IsNullOrEmpty(query))
                && (pictogram.AccessLevel == AccessLevel.PUBLIC
                || pictogram.Users.Any(ur => ur.OtherKey == user.Id)
                || pictogram.Departments.Any(dr => dr.OtherKey == user.DepartmentKey)));
        }

        public IEnumerable<Pictogram> fetchPictogramsUserNotPartOfDepartmentStartsWithQuery(string query, GirafUser user)
        {
            return Context.Pictograms.Where(pictogram => (!string.IsNullOrEmpty(query)
                && pictogram.Title.ToLower().Replace(" ", string.Empty).StartsWith(query)
                || string.IsNullOrEmpty(query))
                && (pictogram.AccessLevel == AccessLevel.PUBLIC
                || pictogram.Users.Any(ur => ur.OtherKey == user.Id)));
        }

        public IEnumerable<Pictogram> fetchPictogramsUserNotPartOfDepartmentContainsQuery(string query, GirafUser user)
        {
            return Context.Pictograms.Where(pictogram => (!string.IsNullOrEmpty(query)
                && pictogram.Title.ToLower().Replace(" ", string.Empty).Contains(query)
                || string.IsNullOrEmpty(query))
                && (pictogram.AccessLevel == AccessLevel.PUBLIC
                || pictogram.Users.Any(ur => ur.OtherKey == user.Id)));
        }

        public IEnumerable<Pictogram> fetchPictogramsNoUserLoggedInStartsWithQuery(string query)
        {
            return Context.Pictograms.Where(pictogram => (!string.IsNullOrEmpty(query)
                && pictogram.Title.ToLower().Replace(" ", string.Empty).StartsWith(query)
                || string.IsNullOrEmpty(query))
                && (pictogram.AccessLevel == AccessLevel.PUBLIC));
        }

        public IEnumerable<Pictogram> fetchPictogramsNoUserLoggedInContainsQuery(string query)
        {
            return Context.Pictograms.Where(pictogram => (!string.IsNullOrEmpty(query)
                && pictogram.Title.ToLower().Replace(" ", string.Empty).Contains(query)
                || string.IsNullOrEmpty(query))
                && (pictogram.AccessLevel == AccessLevel.PUBLIC));
        }
    }
}