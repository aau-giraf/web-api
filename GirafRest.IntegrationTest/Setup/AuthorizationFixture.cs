using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.IntegrationTest.Setup
{
    public class AuthorizationFixture 
    {
        public string ExpiredToken;
        public string Username;
        public string Password;
        public string WeekplanName;
        public int WeekYear;
        public int WeekNumber;
        public int WeekDayNumber;
        public int ActivityId;
        public int DepartmentId;
        public int WrongDepartmentId;
        public string CitizenId;

        public AuthorizationFixture()
        {
            ExpiredToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI1YmM0YzAzMC1mOGQxLTRhYTAtOTBlOC05MTNh" +
                                    "MDYwOTA4YWIiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltc" +
                                    "y9uYW1laWRlbnRpZmllciI6Ijg0MTJkOTk1LWIzODEtNGY4My1iZDI1LWU5ODY2NzBiNTdkOSIsImV4cCI6MT" +
                                    "UyNTMwMzQyNSwiaXNzIjoibm90bWUiLCJhdWQiOiJub3RtZSJ9.8KXRRqF3B5s8tUki7u5j0TqK-189QIpApd" +
                                    "OC6aSxOms";
            Username = "guardian-dev";
            Password = "password";
            WeekplanName = "Normal Uge";
            WeekYear = 0;
            WeekNumber = 0;
            WeekDayNumber = 1;
            ActivityId = 1;  // Might need to be existing activity id. choose 2
            DepartmentId = 1;  // Might need another number
            WrongDepartmentId = 9872536;
            CitizenId = "Jane Doe";
        }
    }
}
