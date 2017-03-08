using System.Collections.Generic;
using IdentityServer4.Models;

namespace GirafWebApi.Configurations
{
    public class ApiResources
    {
        public static List<ApiResource> GetApiResources()
        {
            return new List<ApiResource>()
            {
                new ApiResource("MyApi", "Description of MyApi")
            };     
        }
    }
}