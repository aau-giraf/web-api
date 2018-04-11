using System.Collections.Generic;
using System.IO;
using System.Linq;
using GirafRest.Controllers;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using GirafRest.Models.Responses;
using GirafRest.Services;
using GirafRest.Test.Mocks;
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
            var userController = initializeTest();
            var mockUser = _testContext.MockUsers[0];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            _testContext.MockHttpContext.MockRequestImage(_pngFilepath);
            var res = userController.SetUserIcon().Result;

            Assert.IsType<Response<GirafUserDTO>>(res);
            Assert.True(res.Success);
            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            // Check that we logged in as the user we wanted
            Assert.Equal(res.Data.Username, mockUser.UserName);
        }

        [Fact]
        public void UpdateUserIcon_ExistingIcon_Success()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockRequestImage(_pngFilepath);
            usercontroller.SetUserIcon().Wait();
            _testContext.MockHttpContext.MockRequestImage(_pngFilepath);
            var res = usercontroller.SetUserIcon().Result;

            Assert.IsType<Response<GirafUserDTO>>(res);
            Assert.True(res.Success);
            Assert.Equal(ErrorCode.NoError, res.ErrorCode);

            // Get icon to be sure it is set
            var res2 = usercontroller.GetUserIcon(res.Data.Id).Result;

            Assert.IsType<Response<GirafUserDTO>>(res);
            Assert.True(res2.Success);
            Assert.True(res2.Data.Image != null);
        }

        [Fact]
        public void DeleteUserIcon_ExistingIcon_Success()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockRequestImage(_pngFilepath);
            usercontroller.SetUserIcon().Wait();
            var res = usercontroller.DeleteUserIcon().Result;

            Assert.IsType<Response<GirafUserDTO>>(res);
            Assert.True(res.Success);

            // Get icon to be sure it is deleted
            var res2 = usercontroller.GetUserIcon(res.Data.Id).Result;

            Assert.IsType<ErrorResponse<ImageDTO>>(res2);
            Assert.False(res2.Success);
            Assert.Equal(ErrorCode.UserHasNoIcon, res2.ErrorCode);
        }

        [Fact]
        public void DeleteUserIcon_NoIcon_UserHasNoIcon()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            var res = usercontroller.DeleteUserIcon().Result;

            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.UserHasNoIcon, res.ErrorCode);
        }

        #endregion
        #region GetUser
        public void GetUser_CitizenLogin_Success()
        {
            var usercontroller = initializeTest();
            var mockUser = _testContext.MockUsers[CitizenDepTwo];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var res = usercontroller.GetUser().Result;

            Assert.IsType<Response<GirafUserDTO>>(res);
            Assert.True(res.Success);
            // check that we are logged in as the correct user
            Assert.Equal(res.Data.Username, mockUser.UserName);
            Assert.Equal(res.Data.Department, mockUser.DepartmentKey);
            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
        }

        [Fact] 
        public void GetUser_GuardianLogin_Success()
        {
            var usercontroller = initializeTest();
            var mockUser = _testContext.MockUsers[GuardianDepTwo];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var res = usercontroller.GetUser().Result;

            Assert.IsType<Response<GirafUserDTO>>(res);
            Assert.True(res.Success);
            Assert.Equal(res.Data.Username, mockUser.UserName);
            Assert.Equal(res.Data.Department, mockUser.DepartmentKey);
            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
        }

        [Fact]
        public void GetUser_GuardianLoginUsernameInDepartment_Success()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);

            var res = usercontroller.GetUser().Result;

            Assert.IsType<Response<GirafUserDTO>>(res);
            Assert.True(res.Success);
            Assert.Equal(_testContext.MockUsers[GuardianDepTwo].UserName, res.Data.Username);
            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
        }

        [Fact]
        public void GetUser_GuardianLoginUsernameNotInDepartment_Error()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);
            var res = usercontroller.GetUser("invalid").Result;

// <<<<<<< HEAD
//             var response = usercontroller.GetUser("invalid").Result;

//             Assert.False(response.Success);
//             Assert.Equal(ErrorCode.UserNotFound, response.ErrorCode);
// =======
            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.UserNotFound, res.ErrorCode);
// >>>>>>> release-v1.002.01
        }

        [Fact]
        public void GetUser_AdminLoginUsernameQuery_Success()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[AdminDepOne]);
            var res = usercontroller.GetUser().Result;

            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            Assert.IsType<Response<GirafUserDTO>>(res);
            Assert.True(res.Success);
            Assert.Equal(_testContext.MockUsers[AdminDepOne].UserName, res.Data.Username);
            Assert.Equal(_testContext.MockUsers[AdminDepOne].DepartmentKey, res.Data.Department);
        }

        [Fact]
        public void GetUser_AdminLoginInvalidUsername_Error()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[AdminDepOne]);
            var res = usercontroller.GetUser("invalid").Result;

// <<<<<<< HEAD
//             var response = usercontroller.GetUser("invalid").Result;

//             Assert.False(response.Success);
//             Assert.Equal(ErrorCode.UserNotFound, response.ErrorCode);
// =======
            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.UserNotFound, res.ErrorCode);
// >>>>>>> release-v1.002.01
        }

        [Fact]
        public void GetUser_CitizenLoginUsernameQuery_Error()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CitizenDepTwo]);
            var res = usercontroller.GetUser(CitizenUsername).Result;

// <<<<<<< HEAD
//             var response = usercontroller.GetUser(CitizenUsername).Result;

//             Assert.False(response.Success);
//             Assert.Equal(ErrorCode.UserNotFound, response.ErrorCode);
// =======
            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.UserNotFound, res.ErrorCode);
// >>>>>>> release-v1.002.01
        }
        #endregion
        #region UpdateUser

        [Fact] 
        public void UpdateUser_ValidUserValidDTO_Success()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[AdminDepOne]);

            var res = usercontroller.UpdateUser(
                new GirafUserDTO(_testContext.MockUsers[AdminDepOne], GirafRoles.Citizen))
                .Result;

            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            Assert.IsType<Response<GirafUserDTO>>(res);
            Assert.True(res.Success);
            // check that the updated data is correct
            Assert.Equal(_testContext.MockUsers[AdminDepOne].UserName, res.Data.Username);
            Assert.Equal(_testContext.MockUsers[AdminDepOne].DepartmentKey, res.Data.Department);
        }

        [Fact]
        public void UpdateUser_ValidUserNullDTO_Error()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[AdminDepOne]);
            var res = usercontroller.UpdateUser(null).Result;

            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.MissingProperties, res.ErrorCode);
        }
        #endregion
        #region AddApplication
        [Fact]
        public void AddApplication_ValidApplication_Success_AppInList()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);
            var applicationOption = new ApplicationOption("Test application", "test.app");

            var res = usercontroller.AddApplication(_testContext.MockUsers[CitizenDepTwo].UserName, applicationOption).Result;
            var user = res.Data as GirafUserDTO;

            Assert.IsType<Response<GirafUserDTO>>(res);
            Assert.True(res.Success);

            var res2 = usercontroller.GetSettings(res.Data.Username).Result;
            // check that the ApplicationName exists on the user
            Assert.Contains(res2.Data.appsUserCanAccess, (a => a.ApplicationName == applicationOption.ApplicationName));
        }

        [Fact]
        public void AddApplication_NoApplicationName_ErrorMissingProperties()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);
            var applicationOption = new ApplicationOption(null, "test.app");

            var res = usercontroller.AddApplication(_testContext.MockUsers[CitizenDepTwo].UserName, applicationOption).Result;

            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.MissingProperties, res.ErrorCode); 
        }

        [Fact]
        public void AddApplication_NoApplicationPackage_ErrorMissingProperties()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);
            var applicationOption = new ApplicationOption("Test application", null);
            var res = usercontroller
                .AddApplication(_testContext.MockUsers[CitizenDepTwo].UserName, applicationOption)
                .Result;

            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.MissingProperties, res.ErrorCode); 
        }

        [Fact]
        public void AddApplication_NullAsApplicationOption_ErrorMissingProperties()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);
            ApplicationOption applicationOption = null;
            var res = usercontroller
                .AddApplication(_testContext.MockUsers[CitizenDepTwo].UserName, applicationOption)
                .Result;

            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.MissingProperties, res.ErrorCode); 
        }

        [Fact]
        public void AddApplication_NullAsUsername_ErrorUserNotFound()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);
            var applicationOption = new ApplicationOption("Test application", "test.app");
            var res = usercontroller.AddApplication(null, applicationOption).Result;

            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.UserNotFound, res.ErrorCode);
        }

        [Fact]
        public void AddApplication_InvalidUsername_ErrorUserNotFound()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);
            var applicationOption = new ApplicationOption("Test application", "test.app");
            var res = usercontroller.AddApplication("invalid", applicationOption).Result;

            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.UserNotFound, res.ErrorCode);
        }

        [Fact]
        public void AddApplication_ApplicationAlreadyInList_ErrorUserAlreadyHasAccess()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);
            var applicationOption = new ApplicationOption("Test application", "test.app");
            usercontroller.AddApplication(_testContext.MockUsers[CitizenDepTwo].UserName, applicationOption).Wait();

            var res = usercontroller
                .AddApplication(_testContext.MockUsers[CitizenDepTwo].UserName, applicationOption)
                .Result;
            
            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.UserAlreadyHasAccess, res.ErrorCode);
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
            usercontroller.AddApplication(username, applicationOption).Wait();

            var res = usercontroller.DeleteApplication(username, applicationOption).Result;

            Assert.IsType<Response<GirafUserDTO>>(res);
            Assert.True(res.Success);

            var res2 = usercontroller.GetSettings(res.Data.Username).Result;
            // check that the image is actually deleted
            Assert.True(res2.Data.appsUserCanAccess
                        .FirstOrDefault(a => a.ApplicationName == "Test Application") == null);
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
            var res = usercontroller.DeleteApplication(username, applicationOption).Result;

            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.ApplicationNotFound, res.ErrorCode);
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
            usercontroller.AddApplication(username, applicationOption).Wait();
            applicationOption = new ApplicationOption("Test Application", "test.app")
            {
                Id = -NewApplicationId
            };

            var res = usercontroller.DeleteApplication(username, applicationOption).Result;

            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.ApplicationNotFound, res.ErrorCode);
            // check that the image is not deleted
            Assert.True(_testContext.MockUsers[CitizenDepTwo].Settings.appsUserCanAccess
                        .FirstOrDefault(a => a.ApplicationName == "Test Application") != null);
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
            usercontroller.AddApplication(username, applicationOption).Wait();
            var res = usercontroller.DeleteApplication(username, null).Result;

            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.MissingProperties, res.ErrorCode);
            // check that the image is not deleted
            Assert.True(_testContext.MockUsers[CitizenDepTwo].Settings.appsUserCanAccess
                        .FirstOrDefault(a => a.ApplicationName == "Test Application") != null);
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
            
            usercontroller.AddApplication("VALID USERNAME", applicationOption).Wait();
            var res = usercontroller.DeleteApplication("INVALID USERNAME", applicationOption).Result;

            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.UserNotFound, res.ErrorCode);
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
            usercontroller.AddApplication(null, applicationOption).Wait();
            var res = usercontroller.DeleteApplication(username, null).Result;

            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.MissingProperties, res.ErrorCode);
        }
        #endregion
        #region UpdateDisplayName
        [Fact]
        public void UpdateDisplayName_ValidStringInput_Success()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CitizenDepTwo]);
            const string newDisplayName = "Display Name";
            var res = usercontroller.UpdateDisplayName(newDisplayName).Result;

            Assert.IsType<Response<GirafUserDTO>>(res);
            Assert.True(res.Success);
            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            Assert.Equal(newDisplayName, res.Data.ScreenName);
        }

        [Fact]
        public void UpdateDisplayName_EmptyString_ErrorMissingProperties()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CitizenDepTwo]);
            const string newDisplayName = "";
            var res = usercontroller.UpdateDisplayName(newDisplayName).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.MissingProperties, res.ErrorCode);
            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
        }


        [Fact]
        public void UpdateDisplayName_NullInput_ErrorMissingProperties()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CitizenDepTwo]);
            string newDisplayName = null;
            var res = usercontroller.UpdateDisplayName(newDisplayName).Result;

            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.MissingProperties, res.ErrorCode);
        }
        #endregion
        #region AddUserResource
        [Fact]
        public void AddUserResource_OwnPrivateValidUser_Success()
        {
            var usercontroller = initializeTest();
            string targetUser = _testContext.MockUsers[CitizenDepTwo].UserName;
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);
            var res = usercontroller
                .AddUserResource(targetUser, new ResourceIdDTO() {Id = GuardianPrivatePictogram})
                .Result;

            Assert.IsType<Response<GirafUserDTO>>(res);
            Assert.True(res.Success);
            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            // check ressource is added correctly
            Assert.True(_testContext.MockUsers[CitizenDepTwo].Resources
                        .FirstOrDefault(r => r.ResourceKey == GuardianPrivatePictogram) != null);
        }

        [Fact]
        public void AddUserResource_OwnPrivateInvalidUser_ErrorUserNotFound()
        {
            var usercontroller = initializeTest();
            string targetUser = "INVALID";
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);
            var res = usercontroller
                .AddUserResource(targetUser, new ResourceIdDTO() { Id = GuardianPrivatePictogram })
                .Result;

            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.UserNotFound, res.ErrorCode);
        }


        [Fact]
        public void AddUserResource_OwnProtectedValidUser_ErrorResourceMustBePrivate()
        {
            var usercontroller = initializeTest();
            string targetUser = _testContext.MockUsers[CitizenDepThree].UserName;
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);
            var res = usercontroller
                .AddUserResource(targetUser, new ResourceIdDTO() { Id = GuardianProtectedPictogram })
                .Result;

            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.ResourceMustBePrivate, res.ErrorCode);
        }


        [Fact]
        public void AddUserResource_OwnProtectedInvalidUser_NotFound()
        {
            var usercontroller = initializeTest();
            string targetUser = "INVALID";
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);
            var res = usercontroller
                .AddUserResource(targetUser, new ResourceIdDTO() { Id = PublicPictogram })
                .Result;

            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.UserNotFound, res.ErrorCode);
        }

        [Fact]
        public void AddUserResource_AnotherProtectedValidUser_BadRequest() 
        {
            var usercontroller = initializeTest();
            string targetUser = _testContext.MockUsers[CitizenDepTwo].UserName;
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CitizenDepThree]);
            var res = usercontroller
                .AddUserResource(targetUser, new ResourceIdDTO() { Id = CitizenPrivatePictogram })
                .Result;

            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.NotAuthorized, res.ErrorCode);
        }

        [Fact]
        public void AddUserResource_AnotherProtectedInvalidUser_BadRequest()
        {
            var usercontroller = initializeTest();
            string targetUser = "INVALID";
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CitizenDepThree]);
            var res = usercontroller
                .AddUserResource(targetUser, new ResourceIdDTO() { Id = PublicPictogram })
                .Result;

            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.UserNotFound, res.ErrorCode);
        }

        [Fact]
        public void AddUserResource_PublicValidUser_BadRequest()
        {
            var usercontroller = initializeTest();
            string targetUser = _testContext.MockUsers[GuardianDepTwo].UserName;
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CitizenDepTwo]);
            var res = usercontroller
                .AddUserResource(targetUser, new ResourceIdDTO() { Id = CitizenPrivatePictogram })
                .Result;

            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.NotAuthorized, res.ErrorCode);
        }

        [Fact]
        public void AddUserResource_PublicInvalidUser_NotFound()
        {
            var usercontroller = initializeTest();
            string targetUser = "INVALID";
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CitizenDepTwo]);
            var res = usercontroller
                .AddUserResource(targetUser, new ResourceIdDTO() { Id = PublicPictogram })
                .Result;

            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.UserNotFound, res.ErrorCode);
        }

        [Fact]
        public void AddUserResource_AnotherPrivateValidUser_ErrorNotAuthorized()
        {
            var usercontroller = initializeTest();
            string targetUser = _testContext.MockUsers[GuardianDepTwo].UserName;
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CitizenDepTwo]);
            var res = usercontroller
                .AddUserResource(targetUser, new ResourceIdDTO() { Id = GuardianPrivatePictogram })
                .Result;
            
            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.NotAuthorized, res.ErrorCode);
       }

        [Fact]
        public void AddUserResource_AnotherPrivateInvalidUser_Unauthorized()
        {
            var usercontroller = initializeTest();
            string targetUser = "INVALID";
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CitizenDepTwo]);

            var res = usercontroller
                .AddUserResource(targetUser, new ResourceIdDTO() { Id = PublicPictogram })
                .Result;

            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.UserNotFound, res.ErrorCode);
        }
        #endregion
        #region DeleteResource
        [Fact]
        public void DeleteResource_OwnPrivateValidUser_Success()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);
            var res = usercontroller
                .DeleteResource(new ResourceIdDTO() { Id = GuardianPrivatePictogram })
                .Result;

            Assert.IsType<Response<GirafUserDTO>>(res);
            Assert.True(res.Success);
            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            // check that ressource no longer exist
            Assert.True(_testContext.MockUsers[GuardianDepTwo].Resources.FirstOrDefault(r => r.ResourceKey == GuardianPrivatePictogram) == null);
        }

        [Fact]
        public void DeleteResource_PrivateNoUser_BadRequest()
        {
            var usercontroller = initializeTest();
            var res = usercontroller
                .DeleteResource(new ResourceIdDTO() { Id = GuardianPrivatePictogram })
                .Result;

            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.NotAuthorized, res.ErrorCode);
        }

        [Fact]
        public void DeleteResource_OwnProtectedValidUser_BadRequest()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);
            var res = usercontroller
                .DeleteResource(new ResourceIdDTO() { Id = GuardianProtectedPictogram })
                .Result;

            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.UserDoesNotOwnResource, res.ErrorCode);
        }

        [Fact]
        public void DeleteResource_OwnProtectedInvalidUser_BadRequest()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);
            var res = usercontroller
                .DeleteResource(new ResourceIdDTO() { Id = GuardianProtectedPictogram })
                .Result;

            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.UserDoesNotOwnResource, res.ErrorCode);
        }

        [Fact]
        public void DeleteResource_PublicValidUser_BadRequset()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GuardianDepTwo]);

            var res = usercontroller
                .DeleteResource(new ResourceIdDTO() { Id = PublicPictogram })
                .Result;

            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.UserDoesNotOwnResource, res.ErrorCode);
        }

        [Fact]
        public void DeleteResource_PublicInvalidUser_BadRequest()
        {
            var usercontroller = initializeTest();

            var res = usercontroller
                .DeleteResource(new ResourceIdDTO() { Id = PublicPictogram })
                .Result;

            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.NotAuthorized, res.ErrorCode);
        }

        #endregion
        #region ToggleGrayscale
        [Fact]
        public void ToggleGrayscale_True_GrayscaleIsTrue()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CitizenDepTwo]);

            usercontroller.ToggleGrayscale(true).Wait();

            // Check that we currectly toggled GreyScale
            Assert.True(_testContext.MockUsers[CitizenDepTwo].Settings.UseGrayscale);
        }

        [Fact]
        public void ToggleGrayscale_False_GrayscaleIsFalse()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CitizenDepTwo]);

            usercontroller.ToggleGrayscale(false).Wait();

            // Check that we currectly disabled GreyScale
            Assert.True(!_testContext.MockUsers[CitizenDepTwo].Settings.UseGrayscale);
        }
        #endregion
        #region ToggleAnimations
        [Fact]
        public void ToggleAnimations_True_AnimationsIsTrue()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CitizenDepTwo]);

            usercontroller.ToggleAnimations(true).Wait();

            Assert.True(_testContext.MockUsers[CitizenDepTwo].Settings.DisplayLauncherAnimations);
        }

        [Fact]
        public void ToggleAnimations_False_AnimationsIsFalse()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CitizenDepTwo]);

            usercontroller.ToggleAnimations(false).Wait();

            Assert.True(!_testContext.MockUsers[CitizenDepTwo].Settings.DisplayLauncherAnimations);
        }
        #endregion
        
        #region RemoveDepartment
        [Fact]
        public void RemoveDepartment_RemoveExistingUser_OK()
        {
            var userController = initializeTest();
            var user = _testContext.MockUsers.Where(u => u.UserName == CitizenUsername).FirstOrDefault();
            _testContext.MockUserManager.MockLoginAsUser(user);            
            Assert.True(_testContext.MockDepartments.Where(d => d.Key == user.DepartmentKey).First().Members.Any(u => u.Id == user.Id));
            
            var res = userController.RemoveDepartment(CitizenUsername).Result;

            Assert.IsType<Response<DepartmentDTO>>(res);
            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            // TODO: Check that department no longer has this user
            
        }
        
        [Fact]
        public void RemoveUser_RemoveNullUser_BadRequest()
        {
            var userController = initializeTest();
            var res = userController.RemoveDepartment(null).Result;
            Assert.IsType<ErrorResponse<DepartmentDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.MissingProperties, res.ErrorCode);
        }
        #endregion

    }
}