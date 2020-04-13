using System.Collections.Generic;
using System.Linq;
using GirafRest.Extensions;
using GirafRest.Services;
using Microsoft.AspNetCore.Identity;

namespace GirafRest.Models.DTOs
{
    public class DepartmentDTO
    {
        /// <summary>
        /// The id of the department.
        /// </summary>
        public long Id { get; internal set; }
        /// <summary>
        /// The name of the department.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// A list of the usernames of all members of the department.
        /// </summary>
        public ICollection<UserNameDTO> Members { get; set; }
        /// <summary>
        /// A list of ids of all resources owned by the department.
        /// </summary>
        public ICollection<long> Resources { get; set; }

        /// <summary>
        /// Creates a new department data transfer object from a given department.
        /// </summary>
        /// <param name="department">The department to transfer.</param>
        /// <param name="users">Used for finding the members.</param>
        public DepartmentDTO(Department department, IEnumerable<UserNameDTO> users)
        {
            Id = department.Key;
            Members = users.ToList();
            Name = department.Name;
            
            Resources = new List<long> (department.Resources.Select(dr => dr.PictogramKey));
        }

        public DepartmentDTO ()
        {
            Members = new List<UserNameDTO>();
            Resources = new List<long>();
        }

        public static List<UserNameDTO> FindMembers(IEnumerable<GirafUser> users, RoleManager<GirafRole> roleManager, IGirafService girafService)
        {
            return new List<UserNameDTO>(
                users.Select(m => new UserNameDTO(
                        m.UserName,
                        roleManager.findUserRole(girafService._userManager, m).Result,
                        m.Id
                    )
                ));
        }
    }
}