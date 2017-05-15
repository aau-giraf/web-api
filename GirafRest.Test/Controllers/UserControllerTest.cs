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

        private const int NEW_APPLICATION_ID = 1;
        private const int ADMIN_DEP_ONE = 0;
        private const int GUARDIAN_DEP_TWO = 1;
        private const int CITIZEN_DEP_TWO = 2;
        private const int CITIZEN_DEP_THREE = 3;
        private const int PUBLIC_PICTOGRAM = 0;
        private const int ADMIN_PRIVATE_PICTOGRAM = 3;
        private const int GUARDIAN_PRIVATE_PICTOGRAM = 4;
        private const int ADMIN_PROTECTED_PICTOGRAM = 5;
        private const int GUARDIAN_PROTECTED_PICTOGRAM = 6;

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
                new Mock<IEmailService>().Object,
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

            if (response is ObjectResult)
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

            if (response is ObjectResult)
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

            if (response is ObjectResult)
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

            if (response is ObjectResult)
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

            if (response is ObjectResult)
                _testLogger.WriteLine((response as ObjectResult).Value.ToString());

            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public void DeleteUserIcon_NoIcon_BadRequest()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);

            var response = uc.DeleteUserIcon().Result;

            if (response is ObjectResult)
                _testLogger.WriteLine((response as ObjectResult).Value.ToString());
            Assert.IsType<BadRequestObjectResult>(response);
        }

        #endregion
        #region GetUser
        public void GetUser_CitizenLogin_OkUserInfo()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_TWO]);

            var result = uc.GetUser().Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact] 
        public void GetUser_GuardianLogin_OkListOfUsersInDepartmentAndGuardiansInfo()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var result = uc.GetUser().Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());
            
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<GirafUserDTO>>((result as ObjectResult).Value);
        }

        [Fact]
        public void GetUser_GuardianLoginUsernameInDep_Ok()
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
        public void GetUser_GuardianLoginUsernameNotInDep_NotFound()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var result = uc.GetUser("invalid").Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<NotFoundResult>(result);
        }


        [Fact]
        public void GetUser_AdminLoginUsernameQuery_Ok()
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
        public void GetUser_AdminLoginInvalidUsername_NotFound()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);

            var result = uc.GetUser("invalid").Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<NotFoundResult>(result);
        }


        [Fact]
        public void GetUser_CitizenLoginUsernameQuery_NotFound()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_TWO]);

            var result = uc.GetUser(CITIZEN_USERNAME).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<NotFoundResult>(result);
        }
        #endregion
        #region UpdateUser
        [Fact]
        public void UpdateUser_ValidUserValidDTO_Ok()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);

            var result = uc.UpdateUser(new GirafUserDTO(_testContext.MockUsers[ADMIN_DEP_ONE])).Result;

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void UpdateUser_ValidUserNullDTO_BadRequest()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);

            var result = uc.UpdateUser(null).Result;

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void UpdateUser_ValidUserNullDTOContent_BadRequest()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);

            var result = uc.UpdateUser(new GirafUserDTO()).Result;

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void UpdateUser_ValidUserInvalidDTOContent_NotFound()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);

            var result = uc.UpdateUser(new GirafUserDTO(
                _testContext.MockUsers[ADMIN_DEP_ONE]){ WeekScheduleIds = new List<long>() { 999 }}).Result;

            Assert.IsType<NotFoundObjectResult>(result);
        }
        #endregion
        #region AddApplication
        [Fact]
        public void AddApplication_ValidApplication_OkAppInList()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var ao = new ApplicationOption("Test application", "test.app");

            var result = uc.AddApplication(_testContext.MockUsers[CITIZEN_DEP_TWO].UserName, ao).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<OkObjectResult>(result);
            var user = (result as ObjectResult).Value as GirafUserDTO;

            Assert.Contains(user.LauncherOptions.appsUserCanAccess, (a => a.ApplicationName == ao.ApplicationName));
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
        #endregion
        #region DeleteApplication
        [Fact]
        public void DeleteApplication_ValidApplicationInList_Ok()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var ao = new ApplicationOption("Test Application", "test.app")
            {
                Id = NEW_APPLICATION_ID
            };
            var username = _testContext.MockUsers[CITIZEN_DEP_TWO].UserName;
            uc.AddApplication(username, ao);

            var result = uc.DeleteApplication(username, ao).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<OkObjectResult>(result);
        }


        [Fact]
        public void DeleteApplication_ValidApplicationNotInList_NotFound()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var ao = new ApplicationOption("Test Application", "test.app")
            {
                Id = NEW_APPLICATION_ID
            };
            var username = _testContext.MockUsers[CITIZEN_DEP_TWO].UserName;

            var result = uc.DeleteApplication(username, ao).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<NotFoundObjectResult>(result);
        }


        [Fact]
        public void DeleteApplication_NoIdOnDTO_NotFound()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var ao = new ApplicationOption("Test Application", "test.app")
            {
                Id = NEW_APPLICATION_ID
            };
            var username = _testContext.MockUsers[CITIZEN_DEP_TWO].UserName;
            uc.AddApplication(username, ao);
            ao = new ApplicationOption("Test Application", "test.app")
            {
                Id = -NEW_APPLICATION_ID
            };

            var result = uc.DeleteApplication(username, ao).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<NotFoundObjectResult>(result);
        }


        [Fact]
        public void DeleteApplication_NullAsApplication_BadRequest()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var ao = new ApplicationOption("Test Application", "test.app")
            {
                Id = NEW_APPLICATION_ID
            };
            var username = _testContext.MockUsers[CITIZEN_DEP_TWO].UserName;
            uc.AddApplication(username, ao);

            var result = uc.DeleteApplication(username, null).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public void DeleteApplication_InvalidUsername_NotFound()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var ao = new ApplicationOption("Test Application", "test.app")
            {
                Id = NEW_APPLICATION_ID
            };
            var username = _testContext.MockUsers[CITIZEN_DEP_TWO].UserName;
            uc.AddApplication("INVALID USERNAME", ao);

            var result = uc.DeleteApplication(username, null).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void DeleteApplication_NullUsername_NotFound()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var ao = new ApplicationOption("Test Application", "test.app")
            {
                Id = NEW_APPLICATION_ID
            };
            var username = _testContext.MockUsers[CITIZEN_DEP_TWO].UserName;
            uc.AddApplication(null, ao);

            var result = uc.DeleteApplication(username, null).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<BadRequestObjectResult>(result);
        }
        #endregion
        #region UpdateDisplayName
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
        #endregion
        #region AddUserResource
        [Fact]
        public void AddUserResource_OwnPrivateValidUser_Ok()
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
        public void AddUserResource_OwnPrivateInvalidUser_NotFound()
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
        public void AddUserResource_OwnProtectedValidUser_BadRequest()
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
        public void AddUserResource_OwnProtectedInvalidUser_NotFound()
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
        public void AddUserResource_AnotherProtectedValidUser_BadRequest()
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
        public void AddUserResource_AnotherProtectedInvalidUser_BadRequest()
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
        public void AddUserResource_PublicValidUser_BadRequest()
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
        public void AddUserResource_PublicInvalidUser_NotFound()
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
        public void AddUserResource_AnotherPrivateValidUser_Unauthorized()
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
        public void AddUserResource_AnotherPrivateInvalidUser_Unauthorized()
        {
            var uc = initializeTest();
            string targetUser = "INVALID";
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_TWO]);

            var result = uc.AddUserResource(targetUser, new ResourceIdDTO() { ResourceId = GUARDIAN_PRIVATE_PICTOGRAM }).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<UnauthorizedResult>(result);
        }
        #endregion
        #region DeleteResource
        [Fact]
        public void DeleteResource_OwnPrivateValidUser_Ok()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var result = uc.DeleteResource(_testContext.MockUsers[GUARDIAN_DEP_TWO].UserName,
                                           new ResourceIdDTO() { ResourceId = GUARDIAN_PRIVATE_PICTOGRAM }).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void DeleteResource_OwnPrivateInvalidUser_NotFound()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var result = uc.DeleteResource("InvalidUser",
                                           new ResourceIdDTO() { ResourceId = GUARDIAN_PRIVATE_PICTOGRAM }).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<NotFoundObjectResult>(result);
        }


        [Fact]
        public void DeleteResource_OwnProtectedValidUser_Unauthorized()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var result = uc.DeleteResource(_testContext.MockUsers[GUARDIAN_DEP_TWO].UserName,
                                           new ResourceIdDTO() { ResourceId = GUARDIAN_PROTECTED_PICTOGRAM }).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<UnauthorizedResult>(result);
        }


        [Fact]
        public void DeleteResource_OwnProtectedInvalidUser_NotFound()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var result = uc.DeleteResource("Invalid",
                                           new ResourceIdDTO() { ResourceId = GUARDIAN_PROTECTED_PICTOGRAM }).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<NotFoundObjectResult>(result);
        }


        [Fact]
        public void DeleteResource_AnotherProtectedValidUser_Unauthorized()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var result = uc.DeleteResource(_testContext.MockUsers[GUARDIAN_DEP_TWO].UserName,
                                           new ResourceIdDTO() { ResourceId = ADMIN_PROTECTED_PICTOGRAM }).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<UnauthorizedResult>(result);
        }


        [Fact]
        public void DeleteResource_AnotherProtectedInvalidUser_NotFound()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var result = uc.DeleteResource("Invalid",
                                           new ResourceIdDTO() { ResourceId = ADMIN_PROTECTED_PICTOGRAM }).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<NotFoundObjectResult>(result);
        }


        [Fact]
        public void DeleteResource_PublicValidUser_Unauthorized()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var result = uc.DeleteResource(_testContext.MockUsers[GUARDIAN_DEP_TWO].UserName,
                                           new ResourceIdDTO() { ResourceId = PUBLIC_PICTOGRAM }).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<UnauthorizedResult>(result);
        }


        [Fact]
        public void DeleteResource_PublicInvalidUser_NotFound()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var result = uc.DeleteResource("Invalid",
                                           new ResourceIdDTO() { ResourceId = PUBLIC_PICTOGRAM }).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<NotFoundObjectResult>(result);
        }


        [Fact]
        public void DeleteResource_AnotherPrivateValidUser_Unauthorized()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var result = uc.DeleteResource(_testContext.MockUsers[GUARDIAN_DEP_TWO].UserName,
                                           new ResourceIdDTO() { ResourceId = ADMIN_PRIVATE_PICTOGRAM }).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<UnauthorizedResult>(result);
        }


        [Fact]
        public void DeleteResource_AnotherPrivateInvalidUser_NotFound()
        {
            var uc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var result = uc.DeleteResource("Invalid",
                                           new ResourceIdDTO() { ResourceId = ADMIN_PRIVATE_PICTOGRAM }).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<NotFoundObjectResult>(result);
        }
        #endregion
        #region ToggleGrayscale
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
        #endregion
        #region ToggleAnimations
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
        #endregion
    }

}