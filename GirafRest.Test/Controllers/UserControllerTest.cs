using Xunit;
﻿using Xunit;
using GirafRest;
using GirafRest.Controllers;

namespace GirafRest.Test.Controllers
{
    public class UserControllerTest
    {
        private TestContext _testContext;
        private readonly ITestOutputHelper _testLogger;
        private readonly string PNG_FILEPATH;

        public UserControllerTest(ITestOutputHelper testLogger)
        {
            _testLogger = testLogger;
            PNG_FILEPATH = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "Mocks", "MockImage.png");
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