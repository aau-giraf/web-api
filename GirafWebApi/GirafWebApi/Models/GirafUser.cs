using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace GirafWebApi.Models
{
    [Table("User")]
    public class GirafUser : IdentityUser
    {
        public static string[] GirafRoles = new string[] { "User", "Guardian", "Admin" };
        public string Password { get; set; }
        
        IdentityUserRole<string> _role = new IdentityUserRole<string>();
        public ICollection<PictoFrame> Resources { get; set; }
    
        public GirafUser (string userName, string password, IdentityRole role) : base(userName)
        {
            this.UserName = userName;
            this.Password = password;
            _role.RoleId = role.Id;
            _role.UserId = this.Id;
            
            this.Roles.Add(_role);
            this.Resources = new List<PictoFrame>();
        }
        public GirafUser()
        {
            this.Resources = new List<PictoFrame>();
        }
    }
}