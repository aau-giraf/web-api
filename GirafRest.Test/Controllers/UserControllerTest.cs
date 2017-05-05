using Xunit;
using GirafRest;
using GirafRest.Controllers;
using Xunit.Abstractions;
using static GirafRest.Test.UnitTestExtensions;
using GirafRest.Test.Mocks;
using Moq;
using GirafRest.Services;
using Microsoft.AspNetCore.Mvc;
using GirafRest.Models;
using System.Collections.Generic;
using GirafRest.Models.DTOs;

namespace GirafRest.Test.Controllers
{
    public class UserControllerTest
    {
        private TestContext _testContext;
        private readonly ITestOutputHelper _testLogger;
        private readonly string PNG_FILEPATH;
        private const string CITIZEN_USERNAME = "Citizen of dep 2";
        private const int CITIZEN_INDEX = 2;
        private const int GUARDIAN_INDEX = 1;
        private const int ADMIN_INDEX = 0;

        /*public UserControllerTest(ITestOutputHelper testLogger)
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
            _testContext.MockHttpContext.MockQuery("username", null);

            return pc;
        }

        #region User icon
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
        
        #endregion
        public void GetUser_OrdinaryUser_OkUserInformation()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_INDEX]);

            var result = uc.GetUser().Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<OkObjectResult>(result);
        }
        
        [Fact]
        public void GetUser_Guardian_OkListOfUsersInDepartment()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_INDEX]);
            var result = uc.GetUser().Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<GirafUserDTO>>((result as ObjectResult).Value);
        }

        [Fact]
        public void GetUser_GuardianUsernameInDep_Ok()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_INDEX]);
            _testContext.MockHttpContext.MockQuery("username", CITIZEN_USERNAME);

            var result = uc.GetUser().Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<OkObjectResult>(result);
        }


        [Fact]
        public void GetUser_GuardianUsernameNotInDep_NotFound()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_INDEX]);
            _testContext.MockHttpContext.MockQuery("username", "invalid");

            var result = uc.GetUser().Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<NotFoundResult>(result);
        }


        [Fact]
        public void GetUser_AdminUsernameQuery_Ok()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_INDEX]);
            _testContext.MockHttpContext.MockQuery("username", CITIZEN_USERNAME);

            var result = uc.GetUser().Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<OkObjectResult>(result);
        }


        [Fact]
        public void GetUser_AdminInvalidUsername_NotFound()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_INDEX]);
            _testContext.MockHttpContext.MockQuery("username", "invalid");

            var result = uc.GetUser().Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<NotFoundResult>(result);
        }


        [Fact]
        public void GetUser_CitizenUsernameQuery_NotFound()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_INDEX]);
            _testContext.MockHttpContext.MockQuery("username", CITIZEN_USERNAME);

            var result = uc.GetUser().Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void AddApplication_ValidApplicatoin_OkAppInList()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_INDEX]);
            var ao = new ApplicationOption("Test application", "test.app");

            var result = uc.AddApplication(_testContext.MockUsers[CITIZEN_INDEX].Id, ao).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<OkObjectResult>(result);
            var user = (result as ObjectResult).Value as GirafUserDTO;

            Assert.Contains(user.AvailableApplications, (a => a.ApplicationName == ao.ApplicationName));
        }


        [Fact]
        public void AddApplication_NoApplicationName_BadRequest()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_INDEX]);
            var ao = new ApplicationOption(null, "test.app");

            var result = uc.AddApplication(_testContext.MockUsers[CITIZEN_INDEX].Id, ao).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public void AddApplication_NoApplicationPackage_BadRequest()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_INDEX]);
            var ao = new ApplicationOption("Test application", null);

            var result = uc.AddApplication(_testContext.MockUsers[CITIZEN_INDEX].Id, ao).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public void AddApplication_NullAsInput_BadRequest()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_INDEX]);
            ApplicationOption ao = null;

            var result = uc.AddApplication(_testContext.MockUsers[CITIZEN_INDEX].Id, ao).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public void AddApplication_NullAsUserId_NotFound()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_INDEX]);
            var ao = new ApplicationOption("Test application", "test.app");

            var result = uc.AddApplication(null, ao).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<NotFoundObjectResult>(result);
        }


        [Fact]
        public void AddApplication_InvalidUserId_NotFound()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_INDEX]);
            var ao = new ApplicationOption("Test application", "test.app");

            var result = uc.AddApplication("invalid", ao).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<NotFoundObjectResult>(result);
        }


        [Fact]
        public void AddApplication_ApplicationAlreadyInList_BadRequest()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_INDEX]);
            var ao = new ApplicationOption("Test application", "test.app");

            uc.AddApplication(_testContext.MockUsers[CITIZEN_INDEX].Id, ao);
            var result = uc.AddApplication(_testContext.MockUsers[CITIZEN_INDEX].Id, ao).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<BadRequestObjectResult>(result);
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
*/
    }

}