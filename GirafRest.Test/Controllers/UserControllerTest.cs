using Xunit;
using GirafRest;
using GirafRest.Controllers;
using Xunit.Abstractions;
using static GirafRest.Test.UnitTestExtensions;
using GirafRest.Test.Mocks;
using Moq;
using GirafRest.Services;

namespace GirafRest.Test.Controllers
{
    public class UserControllerTest
    {
        private readonly ITestOutputHelper _testLogger;
        private TestContext _testContext;


        public UserControllerTest(ITestOutputHelper testLogger)
        {
            this._testLogger = testLogger;
        }

        private UserController initializeTest()
        {
            _testContext = new TestContext();

            var pc = new UserController(
                new MockGirafService(_testContext.MockDbContext.Object,
                _testContext.MockUserManager), _testContext.MockLoggerFactory.Object,
                new Mock<IEmailSender>().Object);
            _testContext.MockHttpContext = pc.MockHttpContext();

            return pc;
        }
        
        [Fact]
        public void CreateUserIcon_NoIcon_Ok()
        {

        }
        
        [Fact]
        public void CreateUserIcon_ExistingIcon_BadRequest()
        {

        }
        
        [Fact]
        public void UpdateUserIcon_ExistingIcon_Ok()
        {

        }
        
        [Fact]
        public void UpdateUserIcon_NoIcon_BadRequest()
        {

        }
        
        [Fact]
        public void DeleteUserIcon_NoIcon_BadRequest()
        {

        }
        
        [Fact]
        public void DeleteUserIcon_ExistingIcon_Ok()
        {

        }
        
        [Fact]
        public void GetUser_OrdinaryUser_OkUserInformation()
        {

        }

        [Fact]
        public void GetUser_Guardian_OkListOfUsersInDepartment()
        {

        }


        [Fact]
        public void AddApplication_ValidApplicatoin_OkAppInList()
        {
            Assert.True(false, "Test not implemented yet!");
        }


        [Fact]
        public void AddApplication_NoApplicationName_BadRequest()
        {
            Assert.True(false, "Test not implemented yet!");
        }


        [Fact]
        public void AddApplication_NoApplicationPackage_BadRequest()
        {
            Assert.True(false, "Test not implemented yet!");
        }


        [Fact]
        public void AddApplication_NullAsInput_BadRequest()
        {
            Assert.True(false, "Test not implemented yet!");
        }


        [Fact]
        public void AddApplication_NullAsUserId_NotFound()
        {
            Assert.True(false, "Test not implemented yet!");
        }


        [Fact]
        public void AddApplication_InvalidUserId_NotFound()
        {
            Assert.True(false, "Test not implemented yet!");
        }


        [Fact]
        public void AddApplication_ApplicationAlreadyInList_BadRequest()
        {
            Assert.True(false, "Test not implemented yet!");
        }


        [Fact]
        public void RenoveApplication_ValidApplicationInList_Ok()
        {
            Assert.True(false, "Test not implemented yet!");
        }


        [Fact]
        public void RemoveApplication_ValidApplicationNotInList_NotFound()
        {
            Assert.True(false, "Test not implemented yet!");
        }


        [Fact]
        public void RemoveApplication_NoIdOnDTO_BadRequest()
        {
            Assert.True(false, "Test not implemented yet!");
        }


        [Fact]
        public void RemoveApplication_NullAsApplication_BadRequest()
        {
            Assert.True(false, "Test not implemented yet!");
        }


        [Fact]
        public void RemoveApplication_NullAsUser_BadRequest()
        {
            Assert.True(false, "Test not implemented yet!");
        }


        [Fact]
        public void RemoveApplication_InvalidUserId_NotFound()
        {
            Assert.True(false, "Test not implemented yet!");
        }


        [Fact]
        public void UpdateDisplayName_ValidStringInput_Ok()
        {
            Assert.True(false, "Test not implemented yet!");
        }


        [Fact]
        public void UpdateDisplayName_EmptyString_BadRequest()
        {
            Assert.True(false, "Test not implemented yet!");
        }


        [Fact]
        public void UpdateDisplayName_NullInput_BadRequest()
        {
            Assert.True(false, "Test not implemented yet!");
        }


        [Fact]
        public void AddResource_OwnPrivateValidUser_Ok()
        {
            Assert.True(false, "Test not implemented yet!");
        }


        [Fact]
        public void AddResource_OwnPrivateInvalidUser_NotFound()
        {
            Assert.True(false, "Test not implemented yet!");
        }


        [Fact]
        public void AddResource_OwnProtectedValidUser_BadRequest()
        {
            Assert.True(false, "Test not implemented yet!");
        }


        [Fact]
        public void AddResource_OwnProtectedInvalidUser_NotFound()
        {
            Assert.True(false, "Test not implemented yet!");
        }


        [Fact]
        public void AddResource_AnotherProtectedValidUser_Unauthorized()
        {
            Assert.True(false, "Test not implemented yet!");
        }


        [Fact]
        public void AddResource_AnotherProtectedInvalidUser_NotFound()
        {
            Assert.True(false, "Test not implemented yet!");
        }


        [Fact]
        public void AddResource_PublicValidUser_BadRequest()
        {
            Assert.True(false, "Test not implemented yet!");
        }


        [Fact]
        public void AddResource_PublicInvalidUser_NotFound()
        {
            Assert.True(false, "Test not implemented yet!");
        }


        [Fact]
        public void AddResource_AnotherPrivateValidUser_Unauthorized()
        {
            Assert.True(false, "Test not implemented yet!");
        }


        [Fact]
        public void AddResource_AnotherPrivateInvalidUser_NotFound()
        {
            Assert.True(false, "Test not implemented yet!");
        }

    }

}