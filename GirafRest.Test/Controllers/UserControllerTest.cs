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
using static GirafRest.Models.DTOs.GirafUserDTO;
using Microsoft.AspNetCore.Identity;
using GirafRest.Models.Responses;

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

            var usercontroller = new UserController(
                new MockGirafService(_testContext.MockDbContext.Object,
                _testContext.MockUserManager),
                new Mock<IEmailService>().Object,
                _testContext.MockLoggerFactory.Object,
                _testContext.MockRoleManager.Object);
            _testContext.MockHttpContext = usercontroller.MockHttpContext();
            _testContext.MockHttpContext.MockQuery("username", null);

            return usercontroller;
        }

        #region User icon
        [Fact]
        public void CreateUserIcon_NoIcon_Success()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);

            var response = usercontroller.CreateUserIcon();

            Assert.IsTrue(response.Success);
        }

        [Fact]
        public void CreateUserIcon_ExistingIcon_ErrorUserAlreadyHasIconUsePut()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            usercontroller.CreateUserIcon();
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);

            var response = usercontroller.CreateUserIcon();

            Assert.IsFalse(response.Success);
            Assert.IsEqual<ErrorCode>(response.ErrorCode, ErrorCode.UserAlreadyHasIconUsePut);
        }

        [Fact]
        public void UpdateUserIcon_ExistingIcon_Success()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            usercontroller.CreateUserIcon();
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);

            var response = usercontroller.UpdateUserIcon();

            Assert.IsTrue(response.Success);
        }

        [Fact]
        public void UpdateUserIcon_NoIcon_ErrorUserHasNoIconUsePost()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);

            var response = usercontroller.UpdateUserIcon();

            Assert.IsFalse(response.Success);
            Assert.IsType<ErrorCode>(response.ErrorCode, ErrorCode.UserHasNoIconUsePost);
        }

        [Fact]
        public void DeleteUserIcon_ExistingIcon_Success()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            usercontroller.CreateUserIcon();

            var response = usercontroller.DeleteUserIcon();

            Assert.IsTrue(response.Success);
        }

        [Fact]
        public void DeleteUserIcon_NoIcon_UserHasNoIcon()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);

            var response = usercontroller.DeleteUserIcon();

            Assert.IsFalse(response.Success);
            Assert.IsType<ErrorCode>(response.ErrorCode, ErrorCode.UserHasNoIcon);
        }

        #endregion
        #region GetUser
        public void GetUser_CitizenLogin_Success()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_TWO]);

            var response = usercontroller.GetUser();

            Assert.IsTrue(response.Success); // TODO: Check more than success (what info is sent back, etc)?
        }

        [Fact] 
        public void GetUser_GuardianLogin_Success()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            
            var response = usercontroller.GetUser();

            Assert.IsTrue(response.Success); // TODO: Check more than success (what info is sent back, etc)?
        }

        [Fact]
        public void GetUser_GuardianLoginUsernameInDepartment_Success()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            _testContext.MockHttpContext.MockQuery("username", CITIZEN_USERNAME);

            var response = usercontroller.GetUser();
            
            Assert.IsTrue(response.Success);
        }


        [Fact]
        public void GetUser_GuardianLoginUsernameNotInDepartment_Error()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var response = usercontroller.GetUser("invalid");

            Assert.IsFalse(response.Success);
            Assert.IsType<ErrorCode>(response.ErrorCode, ErrorCode.Error);
        }


        [Fact]
        public void GetUser_AdminLoginUsernameQuery_Success()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockQuery("username", CITIZEN_USERNAME);

            var response = usercontroller.GetUser();

            Assert.IsFalse(response.Success);
            Assert.IsType<ErrorCode>(response.ErrorCode, ErrorCode.Error);
        }


        [Fact]
        public void GetUser_AdminLoginInvalidUsername_Error()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);

            var response = usercontroller.GetUser("invalid");

            Assert.IsFalse(response.Success);
            Assert.IsType<ErrorCode>(response.ErrorCode, ErrorCode.Error);
        }


        [Fact]
        public void GetUser_CitizenLoginUsernameQuery_Error()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_TWO]);

            var response = usercontroller.GetUser(CITIZEN_USERNAME);

            Assert.IsFalse(response.Success);
            Assert.IsType<ErrorCode>(response.ErrorCode, ErrorCode.Error);
        }
        #endregion
        #region UpdateUser
        [Fact]
        public void UpdateUser_ValidUserValidDTO_Success()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);

            var response = usercontroller.UpdateUser(new GirafUserDTO(_testContext.MockUsers[ADMIN_DEP_ONE], GirafRoles.Citizen));

            Assert.IsTrue(response.Success);
        }

        [Fact]
        public void UpdateUser_ValidUserNullDTO_Error()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);

            var response = usercontroller.UpdateUser(null);

            Assert.IsFalse(response.Success);
            Assert.IsType<ErrorCode>(response.ErrorCode, ErrorCode.Error);
        }

        /*We use ModelState.IsValid in this test - ASP.NET fills this for us and thus we cannot unit test it.
        [Fact]
        public void UpdateUser_ValidUserNullDTOContent_BadRequest()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);

            var result = usercontroller.UpdateUser(new GirafUserDTO()).Result;

            Assert.IsType<BadRequestObjectResult>(result);
        }*/

        [Fact]
        public void UpdateUser_ValidUserInvalidDTOContent_Error()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);

            //Create a DTO with an invalid pictogram id
            var response = usercontroller.UpdateUser(new GirafUserDTO(
                _testContext.MockUsers[ADMIN_DEP_ONE], GirafRoles.Citizen)
                {
                    Resources = new List<ResourceDTO> () { new ResourceDTO() }    //I just blindly create an empty object here. Possible source of bug. TODO
                });

            Assert.IsFalse(response.Success);
            Assert.IsType<ErrorCode>(response.ErrorCode, ErrorCode.Error);
        }
        #endregion
        #region AddApplication
        [Fact]
        public void AddApplication_ValidApplication_Success_AppInList() // TODO Split this up into 2 tests 
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var applicationOption = new ApplicationOption("Test application", "test.app");

            var response = usercontroller.AddApplication(_testContext.MockUsers[CITIZEN_DEP_TWO].UserName, applicationOption);
            var user = response.Data as GirafUserDTO;

            Assert.IsTrue(response.Success);
            Assert.Contains(user.Settings.appsUserCanAccess, (a => a.ApplicationName == applicationOption.ApplicationName));
        }


        [Fact]
        public void AddApplication_NoApplicationName_ErrorMissingProperties()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var applicationOption = new ApplicationOption(null, "test.app");

            var response = usercontroller.AddApplication(_testContext.MockUsers[CITIZEN_DEP_TWO].UserName, applicationOption);

            Assert.IsFalse(response.Success);
            Assert.IsEqual<ErrorCode>(reponse.ErrorCode, ErrorCode.MissingProperties), 
            Assert.IsEqual<String>(reponse.Data, "application");
        }


        [Fact]
        public void AddApplication_NoApplicationPackage_ErrorMissingProperties()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var applicationOption = new ApplicationOption("Test application", null);

            var response = usercontroller.AddApplication(_testContext.MockUsers[CITIZEN_DEP_TWO].UserName, applicationOption);

            Assert.IsFalse(response.Success);
            Assert.IsEqual<ErrorCode>(reponse.ErrorCode, ErrorCode.MissingProperties), 
            Assert.IsEqual<String>(reponse.Data, "application");
        }


        [Fact]
        public void AddApplication_NullAsApplicationOption_ErrorMissingProperties()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            ApplicationOption applicationOption = null;

            var response = usercontroller.AddApplication(_testContext.MockUsers[CITIZEN_DEP_TWO].UserName, applicationOption);

            Assert.IsFalse(response.Success);
            Assert.IsEqual<ErrorCode>(reponse.ErrorCode, ErrorCode.MissingProperties), 
            Assert.IsEqual<String>(reponse.Data, "application");
        }


        [Fact]
        public void AddApplication_NullAsUsername_ErrorUserNotFound()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var applicationOption = new ApplicationOption("Test application", "test.app");

            var response = usercontroller.AddApplication(null, applicationOption);

            Assert.IsFalse(response.Success);
            Assert.IsEqual<ErrorCode>(reponse.ErrorCode, ErrorCode.UserNotFound);
        }


        [Fact]
        public void AddApplication_InvalidUsername_ErrorUserNotFound()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var applicationOption = new ApplicationOption("Test application", "test.app");

            var response = usercontroller.AddApplication("invalid", applicationOption);

            Assert.IsFalse(response.Success);
            Assert.IsEqual<ErrorCode>(reponse.ErrorCode, ErrorCode.UserNotFound);
        }


        [Fact]
        public void AddApplication_ApplicationAlreadyInList_ErrorUserAlreadyHasAccess()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var applicationOption = new ApplicationOption("Test application", "test.app");
            usercontroller.AddApplication(_testContext.MockUsers[CITIZEN_DEP_TWO].UserName, applicationOption);

            var response = usercontroller.AddApplication(_testContext.MockUsers[CITIZEN_DEP_TWO].UserName, applicationOption);

            Assert.IsFalse(response.Success);
            Assert.IsEqual<ErrorCode>(reponse.ErrorCode, ErrorCode.UserAlreadyHasAccess);
        }
        #endregion
        #region DeleteApplication
        [Fact]
        public void DeleteApplication_ValidApplicationInList_Success()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var applicationOption = new ApplicationOption("Test Application", "test.app")
            {
                Id = NEW_APPLICATION_ID
            };
            var username = _testContext.MockUsers[CITIZEN_DEP_TWO].UserName;
            usercontroller.AddApplication(username, applicationOption);

            var result = usercontroller.DeleteApplication(username, applicationOption).Result;

            Assert.IsTrue(response.Success);
        }


        [Fact]
        public void DeleteApplication_ValidApplicationNotInList_ErrorMissingProperties()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var applicationOption = new ApplicationOption("Test Application", "test.app")
            {
                Id = NEW_APPLICATION_ID
            };
            var username = _testContext.MockUsers[CITIZEN_DEP_TWO].UserName;

            var response = usercontroller.DeleteApplication(username, applicationOption);

            Assert.IsFalse(response.Success);
            Assert.IsEqual<ErrorCode>(reponse.ErrorCode, ErrorCode.MissingProperties); // Måske skal det her være ApplicationNotFound?
            Assert.IsEqual<String>(reponse.Data, "application");
        }


        [Fact]
        public void DeleteApplication_NoIdOnDTO_NotFound() // NOT FIXED: I don't understand what this is testing
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var applicationOption = new ApplicationOption("Test Application", "test.app")
            {
                Id = NEW_APPLICATION_ID
            };
            var username = _testContext.MockUsers[CITIZEN_DEP_TWO].UserName;
            usercontroller.AddApplication(username, applicationOption);
            applicationOption = new ApplicationOption("Test Application", "test.app")
            {
                Id = -NEW_APPLICATION_ID
            };

            var response = usercontroller.DeleteApplication(username, applicationOption);

            //Assert.IsType<NotFoundObjectResult>(result);
            Assert.Fail();
        }


        [Fact]
        public void DeleteApplication_NullAsApplication_ErrorMissingProperties()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var applicationOption = new ApplicationOption("Test Application", "test.app")
            {
                Id = NEW_APPLICATION_ID
            };
            var username = _testContext.MockUsers[CITIZEN_DEP_TWO].UserName;
            usercontroller.AddApplication(username, applicationOption);

            var response = usercontroller.DeleteApplication(username, null);

            Assert.IsFalse(response.Success);
            Assert.IsEqual<ErrorCode>(reponse.ErrorCode, ErrorCode.MissingProperties);
            Assert.IsEqual<String>(reponse.Data, "application");
        }


        [Fact]
        public void DeleteApplication_InvalidUsername_ErrorUserNotFound()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var applicationOption = new ApplicationOption("Test Application", "test.app")
            {
                Id = NEW_APPLICATION_ID
            };
            var username = _testContext.MockUsers[CITIZEN_DEP_TWO].UserName;
            usercontroller.AddApplication("INVALID USERNAME", applicationOption);

            var response = usercontroller.DeleteApplication(username, null);

            Assert.IsFalse(response.Success);
            Assert.IsEqual<ErrorCode>(reponse.ErrorCode, ErrorCode.UserNotFound);
        }

        [Fact]
        public void DeleteApplication_NullUsername_ErrorUserNotFound()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var applicationOption = new ApplicationOption("Test Application", "test.app")
            {
                Id = NEW_APPLICATION_ID
            };
            var username = _testContext.MockUsers[CITIZEN_DEP_TWO].UserName;
            usercontroller.AddApplication(null, applicationOption);

            var response = usercontroller.DeleteApplication(username, null);

            Assert.IsFalse(response.Success);
            Assert.IsEqual<ErrorCode>(reponse.ErrorCode, ErrorCode.UserNotFound);
        }
        #endregion
        #region UpdateDisplayName
        [Fact]
        public void UpdateDisplayName_ValidStringInput_Success()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_TWO]);
            string newDisplayName = "Display Name";

            var response = usercontroller.UpdateDisplayName(newDisplayName);

            Assert.IsTrue(response.Success);
        }


        [Fact]
        public void UpdateDisplayName_EmptyString_ErrorMissingProperties()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_TWO]);
            string newDisplayName = "";

            var response = usercontroller.UpdateDisplayName(newDisplayName);

            Assert.IsFalse(response.Success);
            Assert.IsEqual<ErrorCode>(reponse.ErrorCode, ErrorCode.MissingProperties);
            Assert.IsEqual<String>(reponse.Data, "displayname");
        }


        [Fact]
        public void UpdateDisplayName_NullInput_ErrorMissingProperties()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_TWO]);
            string newDisplayName = null;

            var response = usercontroller.UpdateDisplayName(newDisplayName);

            Assert.IsFalse(response.Success);
            Assert.IsEqual<ErrorCode>(reponse.ErrorCode, ErrorCode.MissingProperties);
            Assert.IsEqual<String>(reponse.Data, "displayname");
        }
        #endregion
        #region AddUserResource
        [Fact]
        public void AddUserResource_OwnPrivateValidUser_Success()
        {
            var usercontroller = initializeTest();
            string targetUser = _testContext.MockUsers[CITIZEN_DEP_TWO].UserName;
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var response = usercontroller.AddUserResource(targetUser, new ResourceIdDTO() { Id = GUARDIAN_PRIVATE_PICTOGRAM });

            Assert.IsTrue(response.Success);
        }


        [Fact]
        public void AddUserResource_OwnPrivateInvalidUser_ErrorUserNotFound()
        {
            var usercontroller = initializeTest();
            string targetUser = "INVALID";
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var response = usercontroller.AddUserResource(targetUser, new ResourceIdDTO() { Id = GUARDIAN_PRIVATE_PICTOGRAM });

            Assert.IsFalse(response.Success);
            Assert.IsEqual<ErrorCode>(reponse.ErrorCode, ErrorCode.UserNotFound);
        }


        [Fact]
        public void AddUserResource_OwnProtectedValidUser_ErrorResourceMustBePrivate()
        {
            var usercontroller = initializeTest();
            string targetUser = _testContext.MockUsers[CITIZEN_DEP_THREE].UserName;
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var response = usercontroller.AddUserResource(targetUser, new ResourceIdDTO() { Id = GUARDIAN_PROTECTED_PICTOGRAM });

            Assert.IsFalse(response.Success);
            Assert.IsEqual<ErrorCode>(reponse.ErrorCode, ErrorCode.ResourceMustBePrivate);
        }


        [Fact]
        public void AddUserResource_OwnProtectedInvalidUser_NotFound() // Kombinerer 2 fejl
        {
            var usercontroller = initializeTest();
            string targetUser = "INVALID";
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var result = usercontroller.AddUserResource(targetUser, new ResourceIdDTO() { Id = GUARDIAN_PROTECTED_PICTOGRAM });

            //Assert.IsType<BadRequestObjectResult>(result);
            Assert.Fail();
        }


        [Fact]
        public void AddUserResource_AnotherProtectedValidUser_BadRequest() 
        {
            var usercontroller = initializeTest();
            string targetUser = _testContext.MockUsers[CITIZEN_DEP_TWO].UserName;
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_THREE]);

            var result = usercontroller.AddUserResource(targetUser, new ResourceIdDTO() { Id = GUARDIAN_PROTECTED_PICTOGRAM });

            //Assert.IsType<BadRequestObjectResult>(result);
            Assert.Fail();
        }


        [Fact]
        public void AddUserResource_AnotherProtectedInvalidUser_BadRequest()
        {
            var usercontroller = initializeTest();
            string targetUser = "INVALID";
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_THREE]);

            var result = usercontroller.AddUserResource(targetUser, new ResourceIdDTO() { Id = GUARDIAN_PROTECTED_PICTOGRAM });

            //Assert.IsType<BadRequestObjectResult>(result);
            Assert.Fail();
        }


        [Fact]
        public void AddUserResource_PublicValidUser_BadRequest()
        {
            var usercontroller = initializeTest();
            string targetUser = _testContext.MockUsers[GUARDIAN_DEP_TWO].UserName;
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_TWO]);

            var result = usercontroller.AddUserResource(targetUser, new ResourceIdDTO() { Id = PUBLIC_PICTOGRAM });

            //Assert.IsType<BadRequestObjectResult>(result);
            Assert.Fail();
        }


        [Fact]
        public void AddUserResource_PublicInvalidUser_NotFound()
        {
            var usercontroller = initializeTest();
            string targetUser = "INVALID";
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_TWO]);

            var result = usercontroller.AddUserResource(targetUser, new ResourceIdDTO() { Id = PUBLIC_PICTOGRAM });

            //Assert.IsType<BadRequestObjectResult>(result);
            Assert.Fail();
        }


        [Fact]
        public void AddUserResource_AnotherPrivateValidUser_ErrorNotAuthorized()
        {
            var usercontroller = initializeTest();
            string targetUser = _testContext.MockUsers[GUARDIAN_DEP_TWO].UserName;
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_TWO]);

            var response = usercontroller.AddUserResource(targetUser, new ResourceIdDTO() { Id = GUARDIAN_PRIVATE_PICTOGRAM });

            Assert.IsFalse(response.Success);
            Assert.IsEqual<ErrorCode>(reponse.ErrorCode, ErrorCode.NotAuthorized);
       }


        [Fact]
        public void AddUserResource_AnotherPrivateInvalidUser_Unauthorized() // Kombinerer 2 fejl
        {
            var usercontroller = initializeTest();
            string targetUser = "INVALID";
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_TWO]);

            var result = usercontroller.AddUserResource(targetUser, new ResourceIdDTO() { Id = GUARDIAN_PRIVATE_PICTOGRAM });

            //Assert.IsType<UnauthorizedResult>(result);
            Assert.Fail();
        }
        #endregion
        #region DeleteResource
        [Fact]
        public void DeleteResource_OwnPrivateValidUser_Success()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var response = usercontroller.DeleteResource(new ResourceIdDTO() { Id = GUARDIAN_PRIVATE_PICTOGRAM });

            Assert.IsTrue(response.Success);
        }

        [Fact]
        public void DeleteResource_PrivateNoUser_BadRequest()
        {
            var usercontroller = initializeTest();

            var result = usercontroller.DeleteResource(new ResourceIdDTO() { Id = GUARDIAN_PRIVATE_PICTOGRAM });

            Assert.IsFalse(response.Success);
            Assert.IsEqual<ErrorCode>(reponse.ErrorCode, ErrorCode.UserNotFound);
        }


        [Fact]
        public void DeleteResource_OwnProtectedValidUser_BadRequest()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var result = usercontroller.DeleteResource(new ResourceIdDTO() { Id = GUARDIAN_PROTECTED_PICTOGRAM }).Result;

            Assert.IsFalse(response.Success);
            Assert.IsEqual<ErrorCode>(reponse.ErrorCode, ErrorCode.ResourceMustBePrivate);
        }


        [Fact]
        public void DeleteResource_OwnProtectedInvalidUser_BadRequest() // Kombinerer 2 fejl
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var result = usercontroller.DeleteResource(new ResourceIdDTO() { Id = GUARDIAN_PROTECTED_PICTOGRAM }).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            //Assert.IsType<BadRequestObjectResult>(result);
            Assert.Fail();
        }


        /*[Fact]
        public void DeleteResource_AnotherProtectedValidUser_Unauthorized()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var result = usercontroller.DeleteResource(_testContext.MockUsers[GUARDIAN_DEP_TWO].UserName,
                                           new ResourceIdDTO() { Id = ADMIN_PROTECTED_PICTOGRAM }).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<UnauthorizedResult>(result);
        }


        [Fact]
        public void DeleteResource_AnotherProtectedInvalidUser_NotFound()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var result = usercontroller.DeleteResource("Invalid",
                                           new ResourceIdDTO() { Id = ADMIN_PROTECTED_PICTOGRAM }).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<NotFoundObjectResult>(result);
        }
        */

        [Fact]
        public void DeleteResource_PublicValidUser_BadRequset()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var result = usercontroller.DeleteResource(new ResourceIdDTO() { Id = PUBLIC_PICTOGRAM }).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsFalse(response.Success);
            Assert.IsEqual<ErrorCode>(reponse.ErrorCode, ErrorCode.ResourceMustBePrivate);
        }


        [Fact]
        public void DeleteResource_PublicInvalidUser_BadRequest() // Kombinerer 2 fejl
        {
            var usercontroller = initializeTest();

            var result = usercontroller.DeleteResource(new ResourceIdDTO() { Id = PUBLIC_PICTOGRAM }).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            //Assert.IsType<BadRequestObjectResult>(result);
            Assert.Fail();
        }


        /*[Fact]
        public void DeleteResource_AnotherPrivateValidUser_Unauthorized()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var result = usercontroller.DeleteResource(_testContext.MockUsers[GUARDIAN_DEP_TWO].UserName,
                                           new ResourceIdDTO() { Id = ADMIN_PRIVATE_PICTOGRAM }).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<UnauthorizedResult>(result);
        }


        [Fact]
        public void DeleteResource_AnotherPrivateInvalidUser_NotFound()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var result = usercontroller.DeleteResource("Invalid",
                                           new ResourceIdDTO() { Id = ADMIN_PRIVATE_PICTOGRAM }).Result;

            if (result is ObjectResult)
                _testLogger.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<NotFoundObjectResult>(result);
        }*/
        #endregion
        #region ToggleGrayscale
        [Fact]
        public void ToggleGrayscale_True_GrayscaleIsTrue()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_TWO]);

            usercontroller.ToggleGrayscale(true);

            Assert.True(_testContext.MockUsers[CITIZEN_DEP_TWO].Settings.UseGrayscale);
        }

        [Fact]
        public void ToggleGrayscale_False_GrayscaleIsFalse()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_TWO]);

            usercontroller.ToggleGrayscale(false);

            Assert.True(!_testContext.MockUsers[CITIZEN_DEP_TWO].Settings.UseGrayscale);
        }
        #endregion
        #region ToggleAnimations
        [Fact]
        public void ToggleAnimations_True_AnimationsIsTrue()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_TWO]);

            usercontroller.ToggleAnimations(true);

            Assert.True(_testContext.MockUsers[CITIZEN_DEP_TWO].Settings.DisplayLauncherAnimations);
        }

        [Fact]
        public void ToggleAnimations_False_AnimationsIsFalse()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_TWO]);

            usercontroller.ToggleAnimations(false);

            Assert.True(!_testContext.MockUsers[CITIZEN_DEP_TWO].Settings.DisplayLauncherAnimations);
        }
        #endregion
    }
}