using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GirafWebApi.Contexts;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace GirafWebApi.Models
{
    [Table("User")]
    public class GirafUser : IdentityUser
    {
        public static string[] GirafRoles = new string[] { "User", "Guardian", "Admin" };
        public string Password { get; set; }
        /*[Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }*/
        public long Department_Key { get; set; }
        [ForeignKey("Department_Key")]
        //public Department Department { get; set; }
        IdentityUserRole<string> _role = new IdentityUserRole<string>();
    
        public GirafImage Icon { get; set; }
        public GirafUser (string userName, string password, IdentityRole role) : base(userName)
        {
            this.Password = password;
            _role.RoleId = role.Id;
            _role.UserId = this.Id;
            
            Roles.Add(_role);
        }
        public GirafUser()
        {
                
        }
    }
}