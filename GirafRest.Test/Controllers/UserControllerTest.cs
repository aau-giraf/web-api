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
using static GirafRest.Models.DTOs.GirafUserDTO;

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
                _testContext.MockRoleManager.Object,
                new GirafAuthenticationService(_testContext.MockDbContext.Object));
            
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

            Assert.IsType<Response>(res);
            Assert.True(res.Success);
            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
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

            Assert.IsType<Response>(res);
            Assert.True(res.Success);
            Assert.Equal(ErrorCode.NoError, res.ErrorCode);

            // Get icon to be sure it is set
            var res2 = usercontroller.GetUserIcon(_testContext.MockUsers[0].Id).Result;

            Assert.IsType<Response<ImageDTO>>(res2);
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

            Assert.IsType<Response>(res);
            Assert.True(res.Success);

            // Get icon to be sure it is deleted
            var res2 = usercontroller.GetUserIcon(_testContext.MockUsers[0].Id).Result;

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

            Assert.IsType<ErrorResponse>(res);
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

            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.UserNotFound, res.ErrorCode);
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

            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.UserNotFound, res.ErrorCode);
        }

        [Fact]
        public void GetUser_CitizenLoginUsernameQuery_Error()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CitizenDepThree]);
            var res = usercontroller.GetUser(CitizenUsername).Result;

            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.UserNotFound, res.ErrorCode);
        }

        [Fact]
        // Because guardians are allowed to call Get Guardians of a given citizen, they will get UserHasNoGuardians error if called on a guardian
        // GetUser_GetCitizensAsCitizen_Error is omitted as it gives a different error depending on from where it is called. It should give NotFound as a Citizen is not authorised to call the method, however, Authorize does not work properly when unit testing and therefore it would return UserHasNoCitizens error.
        public void GetUser_GetGuardiansAsGuardian_Error()
        {
            var usercontroller = initializeTest();
            var user = _testContext.MockUsers[GuardianDepTwo];
            _testContext.MockUserManager.MockLoginAsUser(user);
            var res = usercontroller.GetGuardians(user.UserName).Result;

            Assert.Equal(ErrorCode.UserHasNoGuardians, res.ErrorCode);
            Assert.IsType<ErrorResponse<List<UserNameDTO>>>(res);
            Assert.False(res.Success);
        }

        [Fact]
        public void GetUser_GetCitizensAsGuardian_OK()
        {
            var usercontroller = initializeTest();
            var user = _testContext.MockUsers[GuardianDepTwo];
            _testContext.MockUserManager.MockLoginAsUser(user);
            var res = usercontroller.GetCitizens(user.UserName).Result;

            var citizens = new List<UserNameDTO>();
            var citizenUser = _testContext.MockUsers[CitizenDepTwo];
            citizens.Add(new UserNameDTO { UserId = citizenUser.Id, UserName = citizenUser.UserName });

            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            Assert.IsType<Response<List<UserNameDTO>>>(res);

            Assert.Equal(res.Data.FirstOrDefault().UserName, citizens.FirstOrDefault().UserName);
            Assert.Equal(res.Data.Count(), citizens.Count());
            Assert.True(res.Success);
        }

        [Fact]
        public void GetUser_GetGuardiansAsCitizen_OK()
        {
            var usercontroller = initializeTest();
            var user = _testContext.MockUsers[CitizenDepTwo];
            _testContext.MockUserManager.MockLoginAsUser(user);
            var res = usercontroller.GetGuardians(user.UserName).Result;

            var guardians = new List<UserNameDTO>();
            foreach (var guardian in user.Guardians)
            {
                guardians.Add(new UserNameDTO { UserId = guardian.Guardian.Id, UserName = guardian.Guardian.UserName });
            }

            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            Assert.IsType<Response<List<UserNameDTO>>>(res);

            Assert.Equal(res.Data.FirstOrDefault().UserName, guardians.FirstOrDefault().UserName);
            Assert.Equal(res.Data.Count(), guardians.Count());
            Assert.True(res.Success);
        }

        [Fact]
        public void GetUser_GetCitizensAsGuardianWrongUsername_Error()
        {
            var usercontroller = initializeTest();
            var user = _testContext.MockUsers[GuardianDepTwo];
            _testContext.MockUserManager.MockLoginAsUser(user);
            var res = usercontroller.GetCitizens("").Result;

            Assert.Equal(ErrorCode.MissingProperties, res.ErrorCode);
            Assert.IsType<ErrorResponse<List<UserNameDTO>>>(res);
            Assert.False(res.Success);
        }

        [Fact]
        public void GetUser_GetGuardiansAsCitizenWrongUsername_Error()
        {
            var usercontroller = initializeTest();
            var user = _testContext.MockUsers[CitizenDepTwo];
            _testContext.MockUserManager.MockLoginAsUser(user);
            var res = usercontroller.GetGuardians("").Result;

            Assert.Equal(ErrorCode.MissingProperties, res.ErrorCode);
            Assert.IsType<ErrorResponse<List<UserNameDTO>>>(res);
            Assert.False(res.Success);
        }

        #endregion
        #region UpdateUser

        [Fact] 
        public void UpdateUser_ValidUserValidRequest_Success()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[AdminDepOne]);

            // TODO: Tjek at jeg er korrekt!
            var res = usercontroller.UpdateUser(_testContext.MockUsers[AdminDepOne].UserName, _testContext.MockUsers[AdminDepOne].DisplayName)
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
            var res = usercontroller.UpdateUser(null, null).Result;

            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.MissingProperties, res.ErrorCode);
        }
        
        [Fact]
        public void AddGuardianCitizenRelationship_AddGuardianToCitizen_OK()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[AdminDepOne]);
            var res = usercontroller.AddGuardianCitizenRelationship(_testContext.MockUsers[1].Id, _testContext.MockUsers[2].Id);

            Assert.IsType<Response<GirafUserDTO>>(res.Result);
            Assert.True(res.Result.Success);
        }

        [Fact]
        public void AddGuardianCitizenRelationship_InvalidGuardianUser_Error()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[AdminDepOne]);
            var res = usercontroller.AddGuardianCitizenRelationship("", _testContext.MockUsers[2].Id);

            Assert.IsType<ErrorResponse<GirafUserDTO>>(res.Result);
            Assert.False(res.Result.Success);
            Assert.Equal(res.Result.ErrorCode, ErrorCode.UserNotFound);
        }

// REMEMBER ME
        [Fact]
        public void UpdateUser_ScreenNameNull_Success(){
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[UserCitizenDepartment1]);
            var res = usercontroller.UpdateUser(_testContext.MockUsers[UserCitizenDepartment1].UserName, null).Result;

            Assert.IsType<Response<GirafUserDTO>>(res);
            Assert.True(res.Success);
            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
        }

        [Fact]
        public void UpdateUser_NotAuthorised()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[UserDepartment2]);
            var user = _testContext.MockUsers[UserCitizenDepartment1];
            var res = usercontroller.UpdateUser(user.Id, "Charles", "Junior").Result;

            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.NotAuthorized, res.ErrorCode);
        }

        [Fact]
        public void UpdateUser_SameDepartmentWithDepLogin_Ok()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[UserDepartment2]);
            var user = _testContext.MockUsers[UserCitizenDepartment2];
            var res = usercontroller.UpdateUser(user.Id, "Charles", "Junior").Result;

            Assert.IsType<Response<GirafUserDTO>>(res);
            Assert.True(res.Success);
            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            // check data
            Assert.Equal("Charles", res.Data.Username);
            Assert.Equal("Junior", res.Data.ScreenName);
        }

        [Fact]
        public void UpdateUser_SameDepartmenSameUsername_Ok()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[UserDepartment2]);
            var user = _testContext.MockUsers[UserCitizenDepartment2];
            var res = usercontroller.UpdateUser(user.Id, user.UserName, "Gunnar").Result;

            Assert.IsType<Response<GirafUserDTO>>(res);
            Assert.True(res.Success);
            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            // check data
            Assert.Equal(user.UserName, res.Data.Username);
            Assert.Equal("Gunnar", res.Data.ScreenName);
        }
//end remember
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
        #region Settings
        [Fact]
        public void UpdateUserSettings_10_appGridSizeColumns()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CitizenDepTwo]);
            
            var dto = new LauncherOptionsDTO();
            dto.appGridSizeColumns = 10;
            usercontroller.UpdateUserSettings(dto).Wait();

            Assert.Equal(10, _testContext.MockUsers[CitizenDepTwo].Settings.appGridSizeColumns);
        }
        [Fact]
        public void UpdateUserSettings_10_appGridSizeRows()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CitizenDepTwo]);
            
            var dto = new LauncherOptionsDTO();
            dto.appGridSizeRows = 10;
            usercontroller.UpdateUserSettings(dto).Wait();

            Assert.Equal(10, _testContext.MockUsers[CitizenDepTwo].Settings.appGridSizeRows);
        }
        [Fact]
        public void UpdateUserSettings_True_DisplayLauncherAnimations()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CitizenDepTwo]);
            
            var dto = new LauncherOptionsDTO();
            dto.DisplayLauncherAnimations = true;
            usercontroller.UpdateUserSettings(dto).Wait();

            Assert.True(_testContext.MockUsers[CitizenDepTwo].Settings.DisplayLauncherAnimations);
        }
        [Fact]
        public void UpdateUserSettings_landscape_OrientationOk()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CitizenDepTwo]);
            
            var dto = new LauncherOptionsDTO();
            dto.orientation = orientation_enum.landscape;
            usercontroller.UpdateUserSettings(dto).Wait();

            Assert.Equal(orientation_enum.landscape, _testContext.MockUsers[CitizenDepTwo].Settings.orientation);
        }        
        [Fact]
        public void UpdateUserSettings_movedToRight_checkResourceAppearenceOk()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CitizenDepTwo]);
            
            var dto = new LauncherOptionsDTO();
            dto.checkResourceAppearence = resourceAppearence_enum.movedToRight;
            usercontroller.UpdateUserSettings(dto).Wait();

            Assert.Equal(resourceAppearence_enum.movedToRight, _testContext.MockUsers[CitizenDepTwo].Settings.checkResourceAppearence);
        }        
        [Fact]
        public void UpdateUserSettings_analogClock_defaultTimerOk()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CitizenDepTwo]);
            
            var dto = new LauncherOptionsDTO();
            dto.defaultTimer = defaultTimer_enum.analogClock;
            usercontroller.UpdateUserSettings(dto).Wait();

            Assert.Equal(defaultTimer_enum.analogClock, _testContext.MockUsers[CitizenDepTwo].Settings.defaultTimer);
        }        
        [Fact]
        public void UpdateUserSettings_25_timerSecondsOk()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CitizenDepTwo]);
            
            var dto = new LauncherOptionsDTO();
            dto.timerSeconds = 25;
            usercontroller.UpdateUserSettings(dto).Wait();

            Assert.Equal(25, _testContext.MockUsers[CitizenDepTwo].Settings.timerSeconds);
        }
        [Fact]
        public void UpdateUserSettings_10_activitiesCountOk()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CitizenDepTwo]);
            
            var dto = new LauncherOptionsDTO();
            dto.activitiesCount = 30;
            usercontroller.UpdateUserSettings(dto).Wait();

            Assert.Equal(30, _testContext.MockUsers[CitizenDepTwo].Settings.activitiesCount);
        }
        [Fact]
        public void UpdateUserSettings_girafGreen_themeOk()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CitizenDepTwo]);
            
            var dto = new LauncherOptionsDTO();
            dto.theme = theme_enum.girafGreen;
            usercontroller.UpdateUserSettings(dto).Wait();

            Assert.Equal(theme_enum.girafGreen, _testContext.MockUsers[CitizenDepTwo].Settings.theme);
        }

        [Fact]
        public void UpdateSameUserSettings_Ok(){
            var usercontroller = initializeTest();
            var user = _testContext.MockUsers[CitizenDepTwo];
            _testContext.MockUserManager.MockLoginAsUser(user);

            var dto = new LauncherOptionsDTO() { 
                theme = theme_enum.girafGreen,
                timerSeconds = 120,
                defaultTimer = defaultTimer_enum.analogClock,
                activitiesCount = 5
            };
            usercontroller.UpdateUserSettings(user.Id, dto).Wait();

            Assert.Equal(theme_enum.girafGreen, _testContext.MockUsers[CitizenDepTwo].Settings.theme);
            Assert.Equal(120, _testContext.MockUsers[CitizenDepTwo].Settings.timerSeconds);
            Assert.Equal(defaultTimer_enum.analogClock, _testContext.MockUsers[CitizenDepTwo].Settings.defaultTimer);
            Assert.Equal(5, _testContext.MockUsers[CitizenDepTwo].Settings.activitiesCount);
        }

        [Fact]
        public void UpdateOtherUserSettings_Error()
        {
            var usercontroller = initializeTest();
            var user = _testContext.MockUsers[UserCitizenDepartment1];
            _testContext.MockUserManager.MockLoginAsUser(user);

            var idOfUserToUpdate = _testContext.MockUsers[CitizenDepTwo].Id;

            var dto = new LauncherOptionsDTO()
            {
                theme = theme_enum.girafGreen,
                timerSeconds = 120,
                defaultTimer = defaultTimer_enum.analogClock,
                activitiesCount = 5
            };
            var res = usercontroller.UpdateUserSettings(idOfUserToUpdate, dto).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.NotAuthorized, res.ErrorCode);
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
