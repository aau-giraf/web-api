using GirafEntities.User.DTOs;
using GirafEntities.User;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GirafServices.User
{
    public interface IGirafUserService
    {
        List<DisplayNameDTO> FindMembers(IEnumerable<GirafUser> users, RoleManager<GirafRole> roleManager, IGirafService girafService);
    }
}
