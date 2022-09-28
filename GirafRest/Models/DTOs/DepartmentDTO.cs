using GirafRest.Extensions;
using GirafRest.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;

namespace GirafRest.Models.DTOs
{
    /// <summary>
    /// DTO for <see cref="Department"/>
    /// </summary>
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
        /// A list of the displaynames of all members of the department.
        /// </summary>
        public ICollection<DisplayNameDTO> Members { get; set; }

        /// <summary>
        /// A list of ids of all resources owned by the department.
        /// </summary>
        public ICollection<long> Resources { get; set; }

        /// <summary>
        /// Creates a new department data transfer object from a given department.
        /// </summary>
        /// <param name="department">The department to transfer.</param>
        /// <param name="users">Used for finding the members.</param>
        public DepartmentDTO(Department department, IEnumerable<DisplayNameDTO> users)
        {
            Id = department.Key;
            Members = users.ToList();
            Name = department.Name;

            Resources = new List<long>(department.Resources.Select(dr => dr.PictogramKey));
        }

        /// <summary>
        /// Empty constructor for JSON Generation
        /// </summary>
        public DepartmentDTO()
        {
            Members = new List<DisplayNameDTO>();
            Resources = new List<long>();
        }

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