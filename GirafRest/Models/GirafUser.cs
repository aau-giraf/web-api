using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace GirafRest.Models
{
    [Table("User")]
    public class GirafUser : IdentityUser
    {
        public static string[] GirafRoles = new string[] { "User", "Guardian", "Admin" };
        //public string Password { get; set; }
        
        public ICollection<PictoFrame> Resources { get; set; }
    
        public GirafUser (string userName/*, string password*/) : base(userName)
        {
            this.UserName = userName;
            //this.Password = password;
            
            this.Resources = new List<PictoFrame>();
        }
        public GirafUser()
        {
            this.Resources = new List<PictoFrame>();
        }
    }
}