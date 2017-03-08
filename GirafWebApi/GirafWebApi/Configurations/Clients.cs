using System.Collections.Generic;
using IdentityServer4.Models;

namespace GirafWebApi.Configurations
{
    public class Clients
    {
        public static List<Client> GetClients()
        {
            return new List<Client>()
            {
                new Client()
                {
                    ClientId = "resClient",
                    ClientSecrets = new List<Secret>()
                    {
                        new Secret("topsecret".Sha256())
                    },
                    AllowedScopes = new List<string>()
                    {
                        "MyApi"
                    },
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword
                }
            };
        }
    }
}