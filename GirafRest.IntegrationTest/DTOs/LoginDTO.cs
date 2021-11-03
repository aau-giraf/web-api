using System;
using System.Collections.Generic;
using System.Text;

namespace GirafRest.IntegrationTest
{
    class LoginDTO
    {
        public string username { get; set; }
        public string password { get; set; }
        public LoginDTO(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
    }
}
