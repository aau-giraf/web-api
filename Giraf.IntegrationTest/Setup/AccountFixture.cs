using System;
using Giraf.IntegrationTest.Extensions;

namespace Giraf.IntegrationTest.Setup
{
    public class AccountFixture : IDisposable
    {
        public string GuardianUsername;
        public string Citizen2Username;
        public string Citizen1Username;
        public string Password;
        public string BrandNewPassword;

        public AccountFixture()
        {
            GuardianUsername = "Guardian-dev";
            Citizen2Username = $"Grundenberger{DateTime.Now.Ticks}";
            Citizen1Username = $"Gunnar{DateTime.Now.Ticks}";
            Password = "password";
            BrandNewPassword = "brand-new-password";
        }

        public void Dispose()
        {
            TestExtension.DeleteAccountAsync(new CustomWebApplicationFactory(), Citizen1Username, Password, GuardianUsername, Password).Wait();
        }
    }
}
