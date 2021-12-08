using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GirafRest.Models;
using GirafRest.IRepositories;
using GirafRest.Data;
using Microsoft.EntityFrameworkCore;

namespace GirafRest.Repositories
{
    public class UserResourseRepository : Repository<UserResource>, IUserResourseRepository
    {
        public UserResourseRepository(GirafDbContext context) : base(context)
        {

        }

        public Task<int> AddAsync(UserResource userResource)
        {
            Context.UserResources.Add(userResource);
            return Context.SaveChangesAsync();
        }

        public Task<UserResource> FetchRelationshipFromDb(Pictogram resource, GirafUser user)
        {
            return Context.UserResources.Where(ur => ur.PictogramKey == resource.Id && ur.OtherKey == user.Id)
                .FirstOrDefaultAsync();
        }

        public override void Remove(UserResource relationship)
        {
            Context.UserResources.Remove(relationship);
        }

        public async Task<bool> CheckPrivateOwnership(Pictogram pictogram, GirafUser user)
        {
            return await Context.UserResources
                .Where(ur => ur.PictogramKey == pictogram.Id && ur.OtherKey == user.Id)
                .AnyAsync();
        }

        public bool CheckIfUserOwnsResource(Pictogram pictogram, GirafUser user)
        {
            return Context.UserResources
                .Any(ur => ur.Pictogram == pictogram && ur.Other == user);
        } 
    }
}