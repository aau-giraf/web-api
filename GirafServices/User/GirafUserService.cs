using GirafEntities.User.DTOs;
using GirafEntities.User;
using Microsoft.AspNetCore.Identity;
using GirafRepositories;

namespace GirafServices.User
{
    public class GirafUserService : IGirafUserService
    {
        /// <summary>
        /// Find belonging members
        /// </summary>
        /// <returns>List of matching users</returns>
        public virtual List<DisplayNameDTO> FindMembers(IEnumerable<GirafUser> users, RoleManager<GirafRole> roleManager, IGirafService girafService)
        {
            return new List<DisplayNameDTO>(
                users.Select(m => new DisplayNameDTO(
                        m.DisplayName,
                        roleManager.findUserRole(girafService._userManager, m).Result,
                        m.Id
                    )
                ));
        }
    }
}
