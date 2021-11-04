using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.IntegrationTest.Setup
{
    public class AccountMock : IDisposable
    {
        public string Citizen2Username; 
        public AccountMock()
        {
            Citizen2Username = $"Grundenberger{DateTime.Now.Ticks}";
        }

        public void Dispose()
        {
        }
    }
}
