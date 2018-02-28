using System;
using System.Collections.Generic;
using System.IO;
using GirafRest.Controllers;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using GirafRest.Models.Responses;
using GirafRest.Services;
using GirafRest.Test.Mocks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Xunit.Abstractions;
using static GirafRest.Test.UnitTestExtensions.TestContext;

namespace GirafRest.Test
{
    public class UserControllerTest
    {
        private UnitTestExtensions.TestContext _testContext;
        private readonly ITestOutputHelper _testLogger;
        private readonly string _pngFilepath;
        private const string CitizenUsername = "Citizen of dep 2";

        private const int NewApplicationId = 1;
        private const int AdminDepOne = UserAdmin;
        private const int GuardianDepTwo = 1;
        private const int CitizenDepTwo = 2;
        private const int CitizenDepThree = 3;
        private const int PublicPictogram = PictogramPublic1;
        private const int AdminPrivatePictogram = PictogramPrivateUser0;
        private const int GuardianPrivatePictogram = PictogramPrivateUser1;
        private const int AdminProtectedPictogram = PictogramDepartment1;
        private const int GuardianProtectedPictogram = PictogramDepartment2;
        private const int CitizenPrivatePictogram = PictogramPrivateUser0;

        public UserControllerTest(ITestOutputHelper testLogger)
        {
            _testLogger = testLogger;
            _pngFilepath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "Mocks", "MockImage.png");
        }

        private UserController initializeTest()
        {
            _testContext = new UnitTestExtensions.TestContext();

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
            _testContext.MockHttpContext.MockRequestImage(_pngFilepath);

            var response = usercontroller.CreateUserIcon().Result;

            Assert.True(response.Success);
        }

        [Fact]
        public void CreateUserIcon_ExistingIcon_ErrorUserAlreadyHasIconUsePut()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockRequestImage(_pngFilepath);
            usercontroller.CreateUserIcon();
            _testContext.MockHttpContext.MockRequestImage(_pngFilepath);

            var response = usercontroller.CreateUserIcon().Result;

            Assert.False(response.Success);
            Assert.Equal(ErrorCode.UserAlreadyHasIconUsePut, response.ErrorCode);
        }

        [Fact]
        public void UpdateUserIcon_ExistingIcon_Success()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockRequestImage(_pngFilepath);
            usercontroller.CreateUserIcon();
            _testContext.MockHttpContext.MockRequestImage(_pngFilepath);

            var response = usercontroller.UpdateUserIcon().Result;

            Assert.True(response.Success);
        }

        [Fact]
        public void UpdateUserIcon_NoIcon_ErrorUserHasNoIconUsePost()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockRequestImage(_pngFilepath);

            var response = usercontroller.UpdateUserIcon().Result;

            Assert.False(response.Success);
            Assert.Equal(ErrorCode.UserHasNoIconUsePost, response.ErrorCode);
        }

        [Fact]
        public void DeleteUserIcon_ExistingIcon_Success()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockRequestImage(_pngFilepath);
            usercontroller.CreateUserIcon();

            var response = usercontroller.DeleteUserIcon().Result;

            Assert.True(response.Success);
        }

        [Fact]
        public void DeleteUserIcon_NoIcon_UserHasNoIcon()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);

            var response = usercontroller.DeleteUserIcon().Result;

            Assert.False(response.Success);
            Assert.Equal(ErrorCode.UserHasNoIcon, response.ErrorCode);
        }

        #endregion
        #region GetUser
        public void GetUser_CitizenLogin_Success()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CitizenDepTwo]);

            var response = usercontroller.GetUser().Result;

            Assert.True(response.Success); // TODO: Check more than success (what info is sent back, etc)?
        }

        [Fact] 
        public void GetUser_GuardianLogin_Success()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);
            
            var response = usercontroller.GetUser().Result;

            Assert.True(response.Success); // TODO: Check more than success (what info is sent back, etc)?
        }

        [Fact]
        public void GetUser_GuardianLoginUsernameInDepartment_Success()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);
            _testContext.MockHttpContext.MockQuery("username", CitizenUsername);

            var response = usercontroller.GetUser().Result;
            
            Assert.True(response.Success);
        }


        [Fact]
        public void GetUser_GuardianLoginUsernameNotInDepartment_Error()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);

            var response = usercontroller.GetUser("invalid").Result;

            Assert.False(response.Success);
            Assert.Equal(ErrorCode.Error, response.ErrorCode);
        }


        [Fact]
        public void GetUser_AdminLoginUsernameQuery_Success()
        {
            var usercontroller = initializeTest();
            
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[AdminDepOne]);

            var response = usercontroller.GetUser().Result;

            Assert.True(response.Success);
        }


        [Fact]
        public void GetUser_AdminLoginInvalidUsername_Error()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[AdminDepOne]);

            var response = usercontroller.GetUser("invalid").Result;

            Assert.False(response.Success);
            Assert.Equal(ErrorCode.Error, response.ErrorCode);
        }


        [Fact]
        public void GetUser_CitizenLoginUsernameQuery_Error()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CitizenDepTwo]);

            var response = usercontroller.GetUser(CitizenUsername).Result;

            Assert.False(response.Success);
            Assert.Equal(ErrorCode.Error, response.ErrorCode);
        }
        #endregion
        #region UpdateUser
        [Fact(Skip = "Crashes")]
        public void UpdateUser_ValidUserValidDTO_Success()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[AdminDepOne]);

            var response = usercontroller.UpdateUser(
                new GirafUserDTO(_testContext.MockUsers[AdminDepOne], GirafUserDTO.GirafRoles.Citizen))
                .Result;

            Assert.True(response.Success);
        }

        [Fact]
        public void UpdateUser_ValidUserNullDTO_Error()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[AdminDepOne]);

            var response = usercontroller.UpdateUser(null).Result;

            Assert.False(response.Success);
            Assert.Equal(ErrorCode.MissingProperties, response.ErrorCode);
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
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[AdminDepOne]);

            //Create a DTO with an invalid pictogram id
            var response = usercontroller.UpdateUser(new GirafUserDTO(
                _testContext.MockUsers[AdminDepOne], GirafUserDTO.GirafRoles.Citizen)
                {
                    Resources = new List<ResourceDTO> () { new ResourceDTO() }    //I just blindly create an empty object here. Possible source of bug. TODO
                })
                .Result;

            Assert.False(response.Success);
            Assert.Equal(ErrorCode.Error, response.ErrorCode);
        }
        #endregion
        #region AddApplication
        [Fact]
        public void AddApplication_ValidApplication_Success_AppInList() // TODO Split this up into 2 tests 
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);
            var applicationOption = new ApplicationOption("Test application", "test.app");

            var response = usercontroller.AddApplication(_testContext.MockUsers[CitizenDepTwo].UserName, applicationOption).Result;
            var user = response.Data as GirafUserDTO;

            Assert.True(response.Success);
            Assert.Contains(user.Settings.appsUserCanAccess, (a => a.ApplicationName == applicationOption.ApplicationName));
        }


        [Fact]
        public void AddApplication_NoApplicationName_ErrorMissingProperties()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);
            var applicationOption = new ApplicationOption(null, "test.app");

            var response = usercontroller.AddApplication(_testContext.MockUsers[CitizenDepTwo].UserName, applicationOption).Result;

            Assert.False(response.Success);
            Assert.Equal(ErrorCode.MissingProperties, response.ErrorCode); 
            //Assert.Equal(response.Data, "application"); // TODO What was this line meant to do?
        }


        [Fact]
        public void AddApplication_NoApplicationPackage_ErrorMissingProperties()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);
            var applicationOption = new ApplicationOption("Test application", null);

            var response = usercontroller
                .AddApplication(_testContext.MockUsers[CitizenDepTwo].UserName, applicationOption)
                .Result;

            Assert.False(response.Success);
            Assert.Equal(ErrorCode.MissingProperties, response.ErrorCode); 
            //Assert.Equal(response.Data, "application"); // TODO What was this line meant to do?
        }


        [Fact]
        public void AddApplication_NullAsApplicationOption_ErrorMissingProperties()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);
            ApplicationOption applicationOption = null;

            var response = usercontroller
                .AddApplication(_testContext.MockUsers[CitizenDepTwo].UserName, applicationOption)
                .Result;

            Assert.False(response.Success);
            Assert.Equal(ErrorCode.MissingProperties, response.ErrorCode); 
            //Assert.Equal(response.Data, "application"); // TODO What was this line meant to do?
        }


        [Fact]
        public void AddApplication_NullAsUsername_ErrorUserNotFound()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);
            var applicationOption = new ApplicationOption("Test application", "test.app");

            var response = usercontroller.AddApplication(null, applicationOption).Result;

            Assert.False(response.Success);
            Assert.Equal(ErrorCode.UserNotFound, response.ErrorCode);
        }


        [Fact]
        public void AddApplication_InvalidUsername_ErrorUserNotFound()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);
            var applicationOption = new ApplicationOption("Test application", "test.app");

            var response = usercontroller.AddApplication("invalid", applicationOption).Result;

            Assert.False(response.Success);
            Assert.Equal(ErrorCode.UserNotFound, response.ErrorCode);
        }


        [Fact]
        public void AddApplication_ApplicationAlreadyInList_ErrorUserAlreadyHasAccess()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);
            var applicationOption = new ApplicationOption("Test application", "test.app");
            usercontroller.AddApplication(_testContext.MockUsers[CitizenDepTwo].UserName, applicationOption);

            var response = usercontroller
                .AddApplication(_testContext.MockUsers[CitizenDepTwo].UserName, applicationOption)
                .Result;

            Assert.False(response.Success);
            Assert.Equal(ErrorCode.UserAlreadyHasAccess, response.ErrorCode);
        }
        #endregion
        #region DeleteApplication
        [Fact]
        public void DeleteApplication_ValidApplicationInList_Success()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);
            var applicationOption = new ApplicationOption("Test Application", "test.app")
            {
                Id = NewApplicationId
            };
            var username = _testContext.MockUsers[CitizenDepTwo].UserName;
            usercontroller.AddApplication(username, applicationOption);

            var response = usercontroller.DeleteApplication(username, applicationOption).Result;

            Assert.True(response.Success);
        }


        [Fact]
        public void DeleteApplication_ValidApplicationNotInList_ErrorMissingProperties()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);
            var applicationOption = new ApplicationOption("Test Application", "test.app")
            {
                Id = NewApplicationId
            };
            var username = _testContext.MockUsers[CitizenDepTwo].UserName;

            var response = usercontroller.DeleteApplication(username, applicationOption).Result;

            Assert.False(response.Success);
            Assert.Equal(ErrorCode.ApplicationNotFound, response.ErrorCode);
            //Assert.Equal(response.Data, "application"); // TODO What was this line supposed to do?
        }


        [Fact]
        public void DeleteApplication_NoIdOnDTO_NotFound()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);
            var applicationOption = new ApplicationOption("Test Application", "test.app")
            {
                Id = NewApplicationId
            };
            var username = _testContext.MockUsers[CitizenDepTwo].UserName;
            usercontroller.AddApplication(username, applicationOption);
            applicationOption = new ApplicationOption("Test Application", "test.app")
            {
                Id = -NewApplicationId
            };

            var response = usercontroller.DeleteApplication(username, applicationOption).Result;

            Assert.False(response.Success);
            Assert.Equal(ErrorCode.ApplicationNotFound, response.ErrorCode);
        }


        [Fact]
        public void DeleteApplication_NullAsApplication_ErrorMissingProperties()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);
            var applicationOption = new ApplicationOption("Test Application", "test.app")
            {
                Id = NewApplicationId
            };
            var username = _testContext.MockUsers[CitizenDepTwo].UserName;
            usercontroller.AddApplication(username, applicationOption);

            var response = usercontroller.DeleteApplication(username, null).Result;

            Assert.False(response.Success);
            Assert.Equal(ErrorCode.MissingProperties, response.ErrorCode);
            //Assert.Equal(response.Data, "application"); // TODO What was this line supposed to do?
        }


        [Fact]
        public void DeleteApplication_InvalidUsername_ErrorUserNotFound()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);
            var applicationOption = new ApplicationOption("Test Application", "test.app")
            {
                Id = NewApplicationId
            };
            
            usercontroller.AddApplication("VALID USERNAME", applicationOption);

            var response = usercontroller.DeleteApplication("INVALID USERNAME", applicationOption).Result;

            Assert.False(response.Success);
            Assert.Equal(ErrorCode.UserNotFound, response.ErrorCode);
        }

        [Fact]
        public void DeleteApplication_NullUsername_ErrorUserNotFound()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);
            var applicationOption = new ApplicationOption("Test Application", "test.app")
            {
                Id = NewApplicationId
            };
            var username = _testContext.MockUsers[CitizenDepTwo].UserName;
            usercontroller.AddApplication(null, applicationOption);

            var response = usercontroller.DeleteApplication(username, null).Result;

            Assert.False(response.Success);
            Assert.Equal(ErrorCode.MissingProperties, response.ErrorCode);
        }
        #endregion
        #region UpdateDisplayName
        [Fact]
        public void UpdateDisplayName_ValidStringInput_Success()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CitizenDepTwo]);
            string newDisplayName = "Display Name";

            var response = usercontroller.UpdateDisplayName(newDisplayName).Result;

            Assert.True(response.Success);
        }


        [Fact]
        public void UpdateDisplayName_EmptyString_ErrorMissingProperties()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CitizenDepTwo]);
            string newDisplayName = "";

            var response = usercontroller.UpdateDisplayName(newDisplayName).Result;

            Assert.False(response.Success);
            Assert.Equal(ErrorCode.MissingProperties, response.ErrorCode);
            //Assert.Equal(response.Data, "displayname"); // TODO What was this line supposed to do?
        }


        [Fact]
        public void UpdateDisplayName_NullInput_ErrorMissingProperties()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CitizenDepTwo]);
            string newDisplayName = null;

            var response = usercontroller.UpdateDisplayName(newDisplayName).Result;

            Assert.False(response.Success);
            Assert.Equal(ErrorCode.MissingProperties, response.ErrorCode);
            //Assert.Equal(response.Data, "displayname"); // TODO What was this line supposed to do?
        }
        #endregion
        #region AddUserResource
        [Fact]
        public void AddUserResource_OwnPrivateValidUser_Success()
        {
            var usercontroller = initializeTest();
            string targetUser = _testContext.MockUsers[CitizenDepTwo].UserName;
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);

            var response = usercontroller
                .AddUserResource(targetUser, new ResourceIdDTO() {Id = GuardianPrivatePictogram})
                .Result;

            Assert.True(response.Success);
        }


        [Fact]
        public void AddUserResource_OwnPrivateInvalidUser_ErrorUserNotFound()
        {
            var usercontroller = initializeTest();
            string targetUser = "INVALID";
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);

            var response = usercontroller
                .AddUserResource(targetUser, new ResourceIdDTO() { Id = GuardianPrivatePictogram })
                .Result;

            Assert.False(response.Success);
            Assert.Equal(ErrorCode.UserNotFound, response.ErrorCode);
        }


        [Fact]
        public void AddUserResource_OwnProtectedValidUser_ErrorResourceMustBePrivate()
        {
            var usercontroller = initializeTest();
            string targetUser = _testContext.MockUsers[CitizenDepThree].UserName;
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);

            var response = usercontroller
                .AddUserResource(targetUser, new ResourceIdDTO() { Id = GuardianProtectedPictogram })
                .Result;

            Assert.False(response.Success);
            Assert.Equal(ErrorCode.ResourceMustBePrivate, response.ErrorCode);
        }


        [Fact]
        public void AddUserResource_OwnProtectedInvalidUser_NotFound() // Kombinerer 2 fejl
        {
            var usercontroller = initializeTest();
            string targetUser = "INVALID";
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);

            var response = usercontroller
                .AddUserResource(targetUser, new ResourceIdDTO() { Id = CitizenPrivatePictogram })
                .Result;

            Assert.False(response.Success);
            Assert.Equal(ErrorCode.UserNotFound, response.ErrorCode);
        }


        [Fact]
        public void AddUserResource_AnotherProtectedValidUser_BadRequest() 
        {
            var usercontroller = initializeTest();
            string targetUser = _testContext.MockUsers[CitizenDepTwo].UserName;
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CitizenDepThree]);

            var response = usercontroller
                .AddUserResource(targetUser, new ResourceIdDTO() { Id = CitizenPrivatePictogram })
                .Result;

            Assert.False(response.Success);
            Assert.Equal(ErrorCode.NotAuthorized, response.ErrorCode);
        }


        [Fact]
        public void AddUserResource_AnotherProtectedInvalidUser_BadRequest()
        {
            var usercontroller = initializeTest();
            string targetUser = "INVALID";
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CitizenDepThree]);

            var response = usercontroller
                .AddUserResource(targetUser, new ResourceIdDTO() { Id = GuardianProtectedPictogram })
                .Result;

            Assert.False(response.Success);
            Assert.Equal(ErrorCode.UserNotFound, response.ErrorCode);
        }


        [Fact]
        public void AddUserResource_PublicValidUser_BadRequest()
        {
            var usercontroller = initializeTest();
            string targetUser = _testContext.MockUsers[GuardianDepTwo].UserName;
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CitizenDepTwo]);

            var response = usercontroller
                .AddUserResource(targetUser, new ResourceIdDTO() { Id = CitizenPrivatePictogram })
                .Result;

            Assert.False(response.Success);
            Assert.Equal(ErrorCode.NotAuthorized, response.ErrorCode);
        }


        [Fact]
        public void AddUserResource_PublicInvalidUser_NotFound()
        {
            var usercontroller = initializeTest();
            string targetUser = "INVALID";
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CitizenDepTwo]);

            var response = usercontroller
                .AddUserResource(targetUser, new ResourceIdDTO() { Id = CitizenPrivatePictogram })
                .Result;

            Assert.False(response.Success);
            Assert.Equal(ErrorCode.UserNotFound, response.ErrorCode);
        }


        [Fact]
        public void AddUserResource_AnotherPrivateValidUser_ErrorNotAuthorized()
        {
            var usercontroller = initializeTest();
            string targetUser = _testContext.MockUsers[GuardianDepTwo].UserName;
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CitizenDepTwo]);

            var response = usercontroller
                .AddUserResource(targetUser, new ResourceIdDTO() { Id = GuardianPrivatePictogram })
                .Result;

            Assert.False(response.Success);
            Assert.Equal(ErrorCode.NotAuthorized, response.ErrorCode);
       }


        [Fact]
        public void AddUserResource_AnotherPrivateInvalidUser_Unauthorized() // Kombinerer 2 fejl
        {
            var usercontroller = initializeTest();
            string targetUser = "INVALID";
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CitizenDepTwo]);

            var response = usercontroller
                .AddUserResource(targetUser, new ResourceIdDTO() { Id = GuardianPrivatePictogram })
                .Result;

            Assert.False(response.Success);
            Assert.Equal(ErrorCode.UserNotFound, response.ErrorCode);
        }
        #endregion
        #region DeleteResource
        [Fact]
        public void DeleteResource_OwnPrivateValidUser_Success()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);

            var response = usercontroller
                .DeleteResource(new ResourceIdDTO() { Id = GuardianPrivatePictogram })
                .Result;

            Assert.True(response.Success);
        }

        [Fact]
        public void DeleteResource_PrivateNoUser_BadRequest()
        {
            var usercontroller = initializeTest();

            var response = usercontroller
                .DeleteResource(new ResourceIdDTO() { Id = GuardianPrivatePictogram })
                .Result;

            Assert.False(response.Success);
            Assert.Equal(ErrorCode.NotAuthorized, response.ErrorCode);
        }


        [Fact]
        public void DeleteResource_OwnProtectedValidUser_BadRequest()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);

            var response = usercontroller
                .DeleteResource(new ResourceIdDTO() { Id = GuardianProtectedPictogram })
                .Result;

            Assert.False(response.Success);
            Assert.Equal(ErrorCode.UserDoesNotOwnResource, response.ErrorCode);
        }


        [Fact]
        public void DeleteResource_OwnProtectedInvalidUser_BadRequest() // Kombinerer 2 fejl
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);

            var response = usercontroller
                .DeleteResource(new ResourceIdDTO() { Id = GuardianProtectedPictogram })
                .Result;
            
            Assert.False(response.Success);
            Assert.Equal(ErrorCode.UserDoesNotOwnResource, response.ErrorCode);
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
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);

            var response = usercontroller
                .DeleteResource(new ResourceIdDTO() { Id = PublicPictogram })
                .Result;

            Assert.False(response.Success);
            Assert.Equal(ErrorCode.UserDoesNotOwnResource, response.ErrorCode);
        }


        [Fact]
        public void DeleteResource_PublicInvalidUser_BadRequest() // Kombinerer 2 fejl
        {
            var usercontroller = initializeTest();

            var response = usercontroller
                .DeleteResource(new ResourceIdDTO() { Id = PublicPictogram })
                .Result;

            Assert.False(response.Success);
            Assert.Equal(ErrorCode.NotAuthorized, response.ErrorCode);
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
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CitizenDepTwo]);

            usercontroller.ToggleGrayscale(true);

            Assert.True(_testContext.MockUsers[CitizenDepTwo].Settings.UseGrayscale);
        }

        [Fact]
        public void ToggleGrayscale_False_GrayscaleIsFalse()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CitizenDepTwo]);

            usercontroller.ToggleGrayscale(false);

            Assert.True(!_testContext.MockUsers[CitizenDepTwo].Settings.UseGrayscale);
        }
        #endregion
        #region ToggleAnimations
        [Fact]
        public void ToggleAnimations_True_AnimationsIsTrue()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CitizenDepTwo]);

            usercontroller.ToggleAnimations(true);

            Assert.True(_testContext.MockUsers[CitizenDepTwo].Settings.DisplayLauncherAnimations);
        }

        [Fact]
        public void ToggleAnimations_False_AnimationsIsFalse()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CitizenDepTwo]);

            usercontroller.ToggleAnimations(false);

            Assert.True(!_testContext.MockUsers[CitizenDepTwo].Settings.DisplayLauncherAnimations);
        }
        #endregion
    }
}