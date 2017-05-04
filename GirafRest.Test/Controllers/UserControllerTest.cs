using Xunit;
using GirafRest;
using GirafRest.Controllers;

namespace GirafRest.Test.Controllers
{
    public class UserControllerTest
    {
        private readonly UserController manageController;
        private TestContext _testContext;
        private readonly ITestOutputHelper _testLogger;
        private readonly string PNG_FILEPATH;

        public UserControllerTest(ITestOutputHelper testLogger)
        {
            _testLogger = testLogger;
            PNG_FILEPATH = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "Mocks", "MockImage.png");
        }

        private UserController initializeTest() {
            _testContext = new TestContext();

            var uc = new UserController(
                new MockGirafService(_testContext.MockDbContext.Object,
                _testContext.MockUserManager), _testContext.MockLoggerFactory.Object,
                new Mock<IEmailSender>());
            _testContext.MockHttpContext = pc.MockHttpContext();
        }
        
        [Fact]
        public void CreateUserIcon_NoIcon_Ok()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);

            var response = uc.CreateUserIcon();

            if(response is ObjectResult)
                _testLogger.WriteLine((response as ObjectResult).Value.ToString());

            Assert.IsType<OkObjectResult>(response);
        }
        
        [Fact]
        public void CreateUserIcon_ExistingIcon_BadRequest()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            uc.CreateUserIcon();
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);

            var response = uc.CreateUserIcon();

            if(response is ObjectResult)
                _testLogger.WriteLine((response as ObjectResult).Value.ToString());

            Assert.IsType<BadRequestObjectResult>(response);
        }
        
        [Fact]
        public void UpdateUserIcon_ExistingIcon_Ok()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            _uc.CreateUserIcon();
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);

            var response = uc.UpdateUserIcon();

            if(response is ObjectResult)
                _testLogger.WriteLine((response as ObjectResult).Value.ToString());

            Assert.IsType<OkObjectResult>(response);
        }
        
        [Fact]
        public void UpdateUserIcon_NoIcon_BadRequest()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);

            var response = uc.UpdateUserIcon();

            if(response is ObjectResult)
                _testLogger.WriteLine((response as ObjectResult).Value.ToString());

            Assert.IsType<BadRequestObjectResult>(response);
        }
        
        [Fact]
        public void DeleteUserIcon_ExistingIcon_Ok()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            uc.CreateUserIcon();

            var response = uc.DeleteUserIcon();

            if(response is ObjectResult)
                _testLogger.WriteLine((response as ObjectResult).Value.ToString());

            Assert.IsType<OkObjectResult>(response);
        }
        
        [Fact]
        public void DeleteUserIcon_NoIcon_BadRequest()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);

            var response = uc.DeleteUserIcon();

            if(response is ObjectResult)
                _testLogger.WriteLine((response as ObjectResult).Value.ToString());

            Assert.IsType<BadRequestObjectResult>(response);
        }
    }

}