using System.Collections.Generic;

namespace GirafRest.Data.Samples
{
    public class SampleGirafUser
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string DepartmentName { get; set; }
        public string Role { get; set; }
        public List<string> Weeks { get; set; }
        public string Password { get; set; }

        public SampleGirafUser(string name, string displayName, string departmentName, string role, List<string> weeks, string password)
        {
            Name = name;
            DisplayName = displayName;
            DepartmentName = departmentName;
            Role = role;
            Weeks = weeks;
            Password = password;
        }
    }
}