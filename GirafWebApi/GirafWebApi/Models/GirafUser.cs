using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public GirafImage Icon { get; set; }
        public GirafUser (string userName, string password) : base(userName)
        {
            this.Password = password;
        }
        public GirafUser()
        {
                
        }
    }
}