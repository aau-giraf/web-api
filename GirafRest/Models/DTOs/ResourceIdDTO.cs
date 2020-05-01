using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.Models.DTOs
{
    /// <summary>
    /// Simple DTO for a Resource ID
    /// </summary>
    public class ResourceIdDTO
    {
        /// <summary>
        /// The Id of the Resource.
        /// </summary>
        public long? Id { get; set; }
    }
}
