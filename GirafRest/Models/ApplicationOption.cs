using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.Models
{
    [ComplexType]
    public class ApplicationOption
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string ApplicationName { get; set; }
        [Required]
        public string ApplicationPackage { get; set; }

        public ApplicationOption(string applicationName, string applicationPackage)
        {
            ApplicationName = applicationName;
            ApplicationPackage = applicationPackage;
        }

        public ApplicationOption()
        {

        }
    }
}
