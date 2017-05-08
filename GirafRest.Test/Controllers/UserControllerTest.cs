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
using System.IO;
using System;

namespace GirafRest.Test.Controllers
{
    public class UserControllerTest
    {
        private TestContext _testContext;
        private readonly ITestOutputHelper _testLogger;
        private readonly string PNG_FILEPATH;
        private const string CITIZEN_USERNAME = "Citizen of dep 2";

        private const int ADMIN_DEP_ONE = 0;
        private const int GUARDIAN_DEP_TWO = 1;
        private const int CITIZEN_DEP_TWO = 2;
        private const int CITIZEN_DEP_THREE = 3;
        private const int GUARDIAN_PRIVATE_PICTOGRAM = 4;
        private const int GUARDIAN_PROTECTED_PICTOGRAM = 6;
        private const int PUBLIC_PICTOGRAM = 0;

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
                _testContext.MockUserManager),
                new Mock<IEmailSender>().Object,
                _testContext.MockLoggerFactory.Object);
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

            var response = uc.CreateUserIcon().Result;

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

            var response = uc.CreateUserIcon().Result;

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
            uc.CreateUserIcon();
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);

            var response = uc.UpdateUserIcon().Result;

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

            var response = uc.UpdateUserIcon().Result;

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

            var response = uc.DeleteUserIcon().Result;

            if(response is ObjectResult)
                _testLogger.WriteLine((response as ObjectResult).Value.ToString());

            Assert.IsType<OkObjectResult>(response);
        }
        
        [Fact]
        public void DeleteUserIcon_NoIcon_BadRequest()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);

            var response = uc.DeleteUserIcon().Result;

            if(response is ObjectResult)
                _testLogger.WriteLine((response as ObjectResult).Value.ToString());
            Assert.IsType<BadRequestObjectResult>(response);
        }
        
        #endregion
        public void GetUser_OrdinaryUser_OkUserInformation()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_TWO]);

            var result = uc.GetUser().Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<OkObjectResult>(result);
        }
        
        [Fact]
        public void GetUser_Guardian_OkListOfUsersInDepartment()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var result = uc.GetUser().Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            /*
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<GirafUserDTO>>((result as ObjectResult).Value);
            */
        }

        [Fact]
        public void GetUser_GuardianUsernameInDep_Ok()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
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
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
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
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
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
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
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
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_TWO]);
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
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var ao = new ApplicationOption("Test application", "test.app");

            var result = uc.AddApplication(_testContext.MockUsers[CITIZEN_DEP_TWO].UserName, ao).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<OkObjectResult>(result);
            var user = (result as ObjectResult).Value as GirafUserDTO;

            Assert.Contains(user.LauncherOptions.AvailableApplications, (a => a.ApplicationName == ao.ApplicationName));
        }


        [Fact]
        public void AddApplication_NoApplicationName_BadRequest()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var ao = new ApplicationOption(null, "test.app");

            var result = uc.AddApplication(_testContext.MockUsers[CITIZEN_DEP_TWO].UserName, ao).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public void AddApplication_NoApplicationPackage_BadRequest()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var ao = new ApplicationOption("Test application", null);

            var result = uc.AddApplication(_testContext.MockUsers[CITIZEN_DEP_TWO].UserName, ao).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public void AddApplication_NullAsInput_BadRequest()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            ApplicationOption ao = null;

            var result = uc.AddApplication(_testContext.MockUsers[CITIZEN_DEP_TWO].UserName, ao).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public void AddApplication_NullAsUsername_NotFound()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var ao = new ApplicationOption("Test application", "test.app");

            var result = uc.AddApplication(null, ao).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<NotFoundObjectResult>(result);
        }


        [Fact]
        public void AddApplication_InvalidUsername_NotFound()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
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
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var ao = new ApplicationOption("Test application", "test.app");

            uc.AddApplication(_testContext.MockUsers[CITIZEN_DEP_TWO].UserName, ao);
            var result = uc.AddApplication(_testContext.MockUsers[CITIZEN_DEP_TWO].UserName, ao).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public void RenoveApplication_ValidApplicationInList_Ok()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var ao = new ApplicationOption("Test Application", "test.app")
            {
                Id = 1
            };
            var username = _testContext.MockUsers[CITIZEN_DEP_TWO].UserName;
            uc.AddApplication(username, ao);

            var result = uc.DeleteApplication(username, ao).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<OkObjectResult>(result);
        }


        [Fact]
        public void RemoveApplication_ValidApplicationNotInList_NotFound()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var ao = new ApplicationOption("Test Application", "test.app")
            {
                Id = 1
            };
            var username = _testContext.MockUsers[CITIZEN_DEP_TWO].UserName;
            
            var result = uc.DeleteApplication(username, ao).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<NotFoundObjectResult>(result);
        }


        [Fact]
        public void RemoveApplication_NoIdOnDTO_NotFound()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var ao = new ApplicationOption("Test Application", "test.app")
            {
                Id = 1
            };
            var username = _testContext.MockUsers[CITIZEN_DEP_TWO].UserName;
            uc.AddApplication(username, ao);
            ao = new ApplicationOption("Test Application", "test.app")
            {
                Id = -1
            };

            var result = uc.DeleteApplication(username, ao).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<NotFoundObjectResult>(result);
        }


        [Fact]
        public void RemoveApplication_NullAsApplication_BadRequest()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var ao = new ApplicationOption("Test Application", "test.app")
            {
                Id = 1
            };
            var username = _testContext.MockUsers[CITIZEN_DEP_TWO].UserName;
            uc.AddApplication(username, ao);

            var result = uc.DeleteApplication(username, null).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public void RemoveApplication_InvalidUsername_NotFound()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var ao = new ApplicationOption("Test Application", "test.app")
            {
                Id = 1
            };
            var username = _testContext.MockUsers[CITIZEN_DEP_TWO].UserName;
            uc.AddApplication("INVALID USERNAME", ao);

            var result = uc.DeleteApplication(username, null).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void RemoveApplication_NullUsername_NotFound()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var ao = new ApplicationOption("Test Application", "test.app")
            {
                Id = 1
            };
            var username = _testContext.MockUsers[CITIZEN_DEP_TWO].UserName;
            uc.AddApplication(null, ao);

            var result = uc.DeleteApplication(username, null).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void UpdateDisplayName_ValidStringInput_Ok()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_TWO]);
            string newDisplayName = "Display Name";

            var result = uc.UpdateDisplayName(newDisplayName).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<OkObjectResult>(result);
        }


        [Fact]
        public void UpdateDisplayName_EmptyString_BadRequest()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_TWO]);
            string newDisplayName = "";

            var result = uc.UpdateDisplayName(newDisplayName).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public void UpdateDisplayName_NullInput_BadRequest()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_TWO]);
            string newDisplayName = null;

            var result = uc.UpdateDisplayName(newDisplayName).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public void AddResource_OwnPrivateValidUser_Ok()
        {
            var uc = initializeTest();
            string targetUser = _testContext.MockUsers[CITIZEN_DEP_TWO].UserName;
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var result = uc.AddUserResource(targetUser, new ResourceIdDTO() { ResourceId = GUARDIAN_PRIVATE_PICTOGRAM }).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<OkObjectResult>(result);
        }


        [Fact]
        public void AddResource_OwnPrivateInvalidUser_NotFound()
        {
            var uc = initializeTest();
            string targetUser = "INVALID";
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var result = uc.AddUserResource(targetUser, new ResourceIdDTO() { ResourceId = GUARDIAN_PRIVATE_PICTOGRAM }).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<NotFoundObjectResult>(result);
        }


        [Fact]
        public void AddResource_OwnProtectedValidUser_BadRequest()
        {
            var uc = initializeTest();
            string targetUser = _testContext.MockUsers[CITIZEN_DEP_THREE].UserName;
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var result = uc.AddUserResource(targetUser, new ResourceIdDTO() { ResourceId = GUARDIAN_PROTECTED_PICTOGRAM }).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public void AddResource_OwnProtectedInvalidUser_NotFound()
        {
            var uc = initializeTest();
            string targetUser = "INVALID";
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var result = uc.AddUserResource(targetUser, new ResourceIdDTO() { ResourceId = GUARDIAN_PROTECTED_PICTOGRAM }).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public void AddResource_AnotherProtectedValidUser_BadRequest()
        {
            var uc = initializeTest();
            string targetUser = _testContext.MockUsers[CITIZEN_DEP_TWO].UserName;
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_THREE]);

            var result = uc.AddUserResource(targetUser, new ResourceIdDTO() { ResourceId = GUARDIAN_PROTECTED_PICTOGRAM }).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public void AddResource_AnotherProtectedInvalidUser_BadRequest()
        {
            var uc = initializeTest();
            string targetUser = "INVALID";
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_THREE]);

            var result = uc.AddUserResource(targetUser, new ResourceIdDTO() { ResourceId = GUARDIAN_PROTECTED_PICTOGRAM }).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public void AddResource_PublicValidUser_BadRequest()
        {
            var uc = initializeTest();
            string targetUser = _testContext.MockUsers[GUARDIAN_DEP_TWO].UserName;
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_TWO]);

            var result = uc.AddUserResource(targetUser, new ResourceIdDTO() { ResourceId = PUBLIC_PICTOGRAM }).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public void AddResource_PublicInvalidUser_NotFound()
        {
            var uc = initializeTest();
            string targetUser = "INVALID";
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_TWO]);

            var result = uc.AddUserResource(targetUser, new ResourceIdDTO() { ResourceId = PUBLIC_PICTOGRAM }).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public void AddResource_AnotherPrivateValidUser_Unauthorized()
        {
            var uc = initializeTest();
            string targetUser = _testContext.MockUsers[GUARDIAN_DEP_TWO].UserName;
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_TWO]);

            var result = uc.AddUserResource(targetUser, new ResourceIdDTO() { ResourceId = GUARDIAN_PRIVATE_PICTOGRAM }).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<UnauthorizedResult>(result);
        }


        [Fact]
        public void AddResource_AnotherPrivateInvalidUser_Unauthorized()
        {
            var uc = initializeTest();
            string targetUser = "INVALID";
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_TWO]);

            var result = uc.AddUserResource(targetUser, new ResourceIdDTO() { ResourceId = GUARDIAN_PRIVATE_PICTOGRAM }).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<UnauthorizedResult>(result);
        }


        [Fact]
        public void ToggleGrayscale_True_GrayscaleIsTrue()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_TWO]);

            var result = uc.ToggleGrayscale(true).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<OkObjectResult>(result);
            Assert.True(_testContext.MockUsers[CITIZEN_DEP_TWO].LauncherOptions.UseGrayscale);
        }

        [Fact]
        public void ToggleGrayscale_False_GrayscaleIsFalse()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_TWO]);

            var result = uc.ToggleGrayscale(false).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<OkObjectResult>(result);
            Assert.True(!_testContext.MockUsers[CITIZEN_DEP_TWO].LauncherOptions.UseGrayscale);
        }

        [Fact]
        public void ToggleAnimations_True_AnimationsIsTrue()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_TWO]);

            var result = uc.ToggleAnimations(true).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<OkObjectResult>(result);
            Assert.True(_testContext.MockUsers[CITIZEN_DEP_TWO].LauncherOptions.DisplayLauncherAnimations);
        }

        [Fact]
        public void ToggleAnimations_False_AnimationsIsFalse()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_TWO]);

            var result = uc.ToggleAnimations(false).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<OkObjectResult>(result);
            Assert.True(!_testContext.MockUsers[CITIZEN_DEP_TWO].LauncherOptions.DisplayLauncherAnimations);
        }
    }

}