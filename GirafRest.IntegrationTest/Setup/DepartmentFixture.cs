using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.IntegrationTest.Setup
{
    public class DepartmentFixture
    {
        public string SuperUserUsername;
        public string Password;
        public string GuardianUsername;
        public string Citizen2Username;
        public string Citizen1Username;
        public string DepartmentUsername;
        public string DepartmentRename;
        public string DepartmentPassword;
        public string Department;
        public string[] NewPictograms = { "{'accessLevel': 3, 'title': 'Cyclopean', 'id': -1, 'lastEdit': '2018-03-19T10:40:26.587Z'}", "{'accessLevel': 1, 'title': '$ sudo rm -rf', 'id': -1, 'lastEdit': '2018-03-19T10:40:26.587Z'}", "{'accessLevel': 1, 'title': '$ telnet nsa.gov', 'id': -1, 'lastEdit': '2018-03-19T10:40:26.587Z'}" };
        public string[] NewPictogramTitles = { "Cyclopean", "rm", "telnet" };

        public DepartmentFixture()
        {
            SuperUserUsername = "Lee";
            Password = "password";
            GuardianUsername = "Graatand";
            Citizen2Username = $"Gunnar{DateTime.Now.Ticks}";
            DepartmentRename = $"DeleteMe{DateTime.Now.Ticks}";
            Citizen1Username = $"Kurt";
            DepartmentUsername = $"Dalgaardsholmstuen{DateTime.Now.Ticks}";
            DepartmentPassword = "0000";
            Department = $"{{'id': 0, 'name': '{DepartmentUsername}', 'members': [], 'resources': []}}";
        }
    }
}
