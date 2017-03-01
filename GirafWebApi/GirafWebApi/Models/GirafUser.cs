using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace GirafWebApi.Models
{
    public class GirafUser : IdentityUser
    {
        public long Department_Key { get; set; }
        [ForeignKey("Department_Key")]
        public Department Department { get; set; }

        public GirafImage Icon { get; set; }
    }
}