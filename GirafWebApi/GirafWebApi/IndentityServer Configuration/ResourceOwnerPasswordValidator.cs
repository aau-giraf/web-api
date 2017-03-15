using System.Threading.Tasks;
using IdentityServer4.Validation;
using System.Linq;
using GirafWebApi.Setup;
using static IdentityModel.OidcConstants;

namespace GirafWebApi.IdentityServer.Configurations
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        GirafDbContext _dbContext;
        public ResourceOwnerPasswordValidator (GirafDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
           /* Here we validate our users */
           var users = _dbContext.Users.Where(usr => usr.UserName == context.UserName 
                && usr.Password == context.Password);
           
           if (users.Count() == 0)
           {    
               return Task.FromResult(new GrantValidationResult(TokenErrors.InvalidRequest, 
                    "Username or Password is incorrect"));
           }

           context.Result = new GrantValidationResult(users.First().Id.ToString(), "password");
           return Task.FromResult(0);
        }
    }
}