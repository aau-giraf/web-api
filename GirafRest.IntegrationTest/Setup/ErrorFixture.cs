﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.IntegrationTest.Setup
{
    public class ErrorFixture : IDisposable
    {
        public string GuardianUsername;
        public string Citizen2Username;
        public string Citizen1Username;
        public string Department;
        public ErrorFixture()
        {
            GuardianUsername = "Graatand";
            Citizen2Username = $"Grundenberger{DateTime.Now.Ticks}";
            Citizen1Username = $"Gunnar{DateTime.Now.Ticks}";
            Department = "Tobias";
        }

        public void Dispose()
        {
        }
    }
}
