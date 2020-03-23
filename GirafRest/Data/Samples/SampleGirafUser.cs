using System.Collections.Generic;

namespace GirafRest.Setup
{
    public class SampleGirafUser
    {
        public string Name { get; set; }
        public long DepKey { get; set; }
        public string Role { get; set; }
        public List<string> Weeks { get; set; }
        public string Password { get; set; }

        public SampleGirafUser(string name, long depKey, string role, List<string> weeks, string password)
        {
            Name = name;
            DepKey = depKey;
            Role = role;
            Weeks = weeks;
            Password = password;
        }
    }
}