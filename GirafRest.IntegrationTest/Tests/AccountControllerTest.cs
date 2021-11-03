using Xunit;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

namespace GirafRest.IntegrationTest
{
    public class AccountControllerTest
    : IClassFixture<CustomWebApplicationFactory<Setup.Startup>>
    {
        private readonly CustomWebApplicationFactory<Setup.Startup> _factory;
        private const string BASE_URL = "https://localhost:5000/";

        public AccountControllerTest(CustomWebApplicationFactory<Setup.Startup> factory)
        {
            _factory = factory;
        }
        
        /// <summary>
        ///Testing logging in as Guardian
        ///Endpoint: POST:/v2/Account/login
        /// </summary>
        [Fact]
        public async void TestAccountCanLoginAsGuardian()
        {
            var client = _factory.CreateClient();
            HttpContent content = new StringContent(JsonConvert.SerializeObject(new LoginDTO("Graatand", "password")),
                                    Encoding.UTF8,
                                    "application/json");
            var response = await client.PostAsync($"{BASE_URL}v2/Account/login", content);

            response.EnsureSuccessStatusCode();
            Assert.NotEmpty(await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public void test_account_can_get_guardian_id()
        {

        }


    }
}
