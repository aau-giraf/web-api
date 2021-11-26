using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.IntegrationTest.Setup
{
    public class AccountFixture
    {
        public string GuardianUsername;
        public string Citizen2Username;
        public string Citizen1Username;
        public string Password;

        public AccountFixture()
        {
            GuardianUsername = "Guardian-dev";
            Citizen2Username = $"Grundenberger{DateTime.Now.Ticks}";
            Citizen1Username = $"Gunnar{DateTime.Now.Ticks}";
            Password = "password";
        }
    }
}
