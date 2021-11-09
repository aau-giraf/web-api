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
        public string DepartmentUsername;
        public string GuardianPassword;
        public string Citizen2Password;
        public string Citizen1Password;
        public string DepartmentPassword;

        public AccountFixture()
        {
            GuardianUsername = "Graatand";
            Citizen2Username = $"Grundenberger{DateTime.Now.Ticks}";
            Citizen1Username = $"Gunnar{DateTime.Now.Ticks}";
            DepartmentUsername = "Tobias";
            GuardianPassword = "password";
            Citizen1Password = "password";
            Citizen2Password = "password";
            DepartmentPassword = "password";
        }
    }
}
