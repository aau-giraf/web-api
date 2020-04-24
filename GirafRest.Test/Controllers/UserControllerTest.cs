using System.Collections.Generic;
using System.IO;
using System.Linq;
using GirafRest.Controllers;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using GirafRest.Models.Responses;
using GirafRest.Services;
using GirafRest.Test.Mocks;
using Microsoft.AspNetCore.Http;
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
#pragma warning disable IDE0052 // Remove unread private members
        private readonly ITestOutputHelper _testLogger;
#pragma warning restore IDE0052 // Remove unread private members

        private readonly string _pngFilepath;

#pragma warning disable IDE0051 // Remove unused private members
                private const int NewApplicationId = 1;
        private const int ADMIN_DEP_ONE = 0;
        private const int GUARDIAN_DEP_TWO = 1;
        private const int CITIZEN_DEP_TWO = 2;
        private const int DEPARTMENT_USER_DEP_TWO = 6;
        private const int CITIZEN_DEP_THREE = 3; // Have no week
        private const int PublicPictogram = PictogramPublic1;
        private const int AdminPrivatePictogram = PictogramPrivateUser0;
        private const int GuardianPrivatePictogram = PictogramPrivateUser1;
        private const int AdminProtectedPictogram = PictogramDepartment1;
        private const int GuardianProtectedPictogram = PictogramDepartment2;
        private const int CitizenPrivatePictogram = PictogramPrivateUser0;
#pragma warning restore IDE0051 // Remove unused private members

        public List<SettingDTO> UserSettings { get; set; }

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
                _testContext.MockLoggerFactory.Object,
                _testContext.MockRoleManager.Object,
                new GirafAuthenticationService(_testContext.MockDbContext.Object,_testContext.MockRoleManager.Object,
                                               _testContext.MockUserManager));
            
            _testContext.MockHttpContext = usercontroller.MockHttpContext();
            _testContext.MockHttpContext.MockQuery("username", null);

            this.UserSettings = new List<SettingDTO>() {
                new SettingDTO(){WeekDayColors = new List<WeekDayColorDTO> {
                new WeekDayColorDTO() { Day = Days.Monday, HexColor = "#067700"},
                new WeekDayColorDTO() { Day = Days.Tuesday, HexColor = "#8c1086"},
                new WeekDayColorDTO() { Day = Days.Wednesday, HexColor = "#ff7f00"},
                new WeekDayColorDTO() { Day = Days.Thursday, HexColor = "#0017ff"},
                new WeekDayColorDTO() { Day = Days.Friday, HexColor = "#ffdd00"},
                new WeekDayColorDTO() { Day = Days.Saturday, HexColor = "#ff0102"},
                new WeekDayColorDTO() { Day = Days.Sunday, HexColor = "#ffffff"}},
                Theme = Theme.girafGreen,
                TimerSeconds = 120,
                DefaultTimer = DefaultTimer.analogClock,
                ActivitiesCount = 5,
                NrOfDaysToDisplay = 5,
                GreyScale = true,
                LockTimerControl = true}
            };

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
            var res = userController.SetUserIcon(mockUser.Id).Result as ObjectResult;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
        }

        [Theory]
        [InlineData(DEPARTMENT_USER_DEP_TWO, CITIZEN_DEP_THREE, ErrorCode.NotAuthorized)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, ADMIN_DEP_ONE, ErrorCode.NotAuthorized)]
        [InlineData(GUARDIAN_DEP_TWO, ADMIN_DEP_ONE, ErrorCode.NotAuthorized)]
        [InlineData(GUARDIAN_DEP_TWO, DEPARTMENT_USER_DEP_TWO, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, CITIZEN_DEP_THREE, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, GUARDIAN_DEP_TWO, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, DEPARTMENT_USER_DEP_TWO, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, ADMIN_DEP_ONE, ErrorCode.NotAuthorized)]
        [InlineData(GUARDIAN_DEP_TWO, CITIZEN_DEP_THREE, ErrorCode.NotAuthorized)]
        public void CreateUserIncon_AuthenticationChecks_Errors(int authUser, int userToEdit, ErrorCode expectedError)
        {
            var userController = initializeTest();
            var mockUser = _testContext.MockUsers[authUser];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            _testContext.MockHttpContext.MockRequestImage(_pngFilepath);
            var res = userController.SetUserIcon(_testContext.MockUsers[userToEdit].Id).Result as ObjectResult;
            var body = res.Value as ErrorResponse;
            
            Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
            Assert.Equal(expectedError, body.ErrorCode);
        }


        [Theory]
        [InlineData(ADMIN_DEP_ONE, DEPARTMENT_USER_DEP_TWO)]
        [InlineData(ADMIN_DEP_ONE, GUARDIAN_DEP_TWO)]
        [InlineData(ADMIN_DEP_ONE, CITIZEN_DEP_THREE)]
        [InlineData(ADMIN_DEP_ONE, CITIZEN_DEP_TWO)]
        [InlineData(GUARDIAN_DEP_TWO, CITIZEN_DEP_TWO)]
        [InlineData(CITIZEN_DEP_TWO, CITIZEN_DEP_TWO)]
        public void CreateUserIncon_AuthenticationChecks_NoErrors(int authUser, int userToEdit)
        {
            var userController = initializeTest();
            var mockUser = _testContext.MockUsers[authUser];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            _testContext.MockHttpContext.MockRequestImage(_pngFilepath);
            var res = userController.SetUserIcon(_testContext.MockUsers[userToEdit].Id).Result as ObjectResult;
            var body = res.Value as SuccessResponse;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            Assert.NotNull(body.Data);
        }


        [Fact]
        public void UpdateUserIcon_ExistingIcon_Success()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockRequestImage(_pngFilepath);
            usercontroller.SetUserIcon(_testContext.MockUsers[0].Id).Wait();
            _testContext.MockHttpContext.MockRequestImage(_pngFilepath);
            var res = usercontroller.SetUserIcon(_testContext.MockUsers[0].Id).Result as ObjectResult;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);

            // Get icon to be sure it is set
            var res2 = usercontroller.GetUserIcon(_testContext.MockUsers[0].Id).Result as ObjectResult;
            var body = res2.Value as SuccessResponse<ImageDTO>;

            Assert.Equal(StatusCodes.Status200OK, res2.StatusCode);
            Assert.True(body.Data.Image != null);
        }

        [Theory]
        [InlineData(DEPARTMENT_USER_DEP_TWO, CITIZEN_DEP_THREE, ErrorCode.NotAuthorized)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, ADMIN_DEP_ONE, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, CITIZEN_DEP_THREE, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, GUARDIAN_DEP_TWO, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, DEPARTMENT_USER_DEP_TWO, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, ADMIN_DEP_ONE, ErrorCode.NotAuthorized)]
        [InlineData(GUARDIAN_DEP_TWO, ADMIN_DEP_ONE, ErrorCode.NotAuthorized)]
        [InlineData(GUARDIAN_DEP_TWO, DEPARTMENT_USER_DEP_TWO, ErrorCode.NotAuthorized)]
        [InlineData(GUARDIAN_DEP_TWO, CITIZEN_DEP_THREE, ErrorCode.NotAuthorized)]
        public void UpdateUserIcon_AuthenticationChecks_Errors(int authUser, int userToEdit, ErrorCode expectedError)
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[authUser]);
            _testContext.MockHttpContext.MockRequestImage(_pngFilepath);
            usercontroller.SetUserIcon(_testContext.MockUsers[authUser].Id).Wait();
            _testContext.MockHttpContext.MockRequestImage(_pngFilepath);
            var res = usercontroller.SetUserIcon(_testContext.MockUsers[userToEdit].Id).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
            Assert.Equal(expectedError, body.ErrorCode);
        }

        [Theory]
        [InlineData(ADMIN_DEP_ONE, DEPARTMENT_USER_DEP_TWO)]
        [InlineData(ADMIN_DEP_ONE, GUARDIAN_DEP_TWO)]
        [InlineData(ADMIN_DEP_ONE, CITIZEN_DEP_THREE)]
        [InlineData(ADMIN_DEP_ONE, CITIZEN_DEP_TWO)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, GUARDIAN_DEP_TWO)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, CITIZEN_DEP_TWO)]
        [InlineData(CITIZEN_DEP_TWO, CITIZEN_DEP_TWO)]
        [InlineData(GUARDIAN_DEP_TWO, CITIZEN_DEP_TWO)]
        public void UpdateUserIcon_AuthenticationChecks_NoError(int authUser, int userToEdit)
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[authUser]);
            _testContext.MockHttpContext.MockRequestImage(_pngFilepath);
            usercontroller.SetUserIcon(_testContext.MockUsers[authUser].Id).Wait();
            _testContext.MockHttpContext.MockRequestImage(_pngFilepath);
            var res = usercontroller.SetUserIcon(_testContext.MockUsers[userToEdit].Id).Result as ObjectResult;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
        }

        [Fact]
        public void DeleteUserIcon_ExistingIcon_Success()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockRequestImage(_pngFilepath);
            usercontroller.SetUserIcon(_testContext.MockUsers[0].Id).Wait();
            var res = usercontroller.DeleteUserIcon(_testContext.MockUsers[0].Id).Result as ObjectResult;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);

            // Get icon to be sure it is deleted
            var res2 = usercontroller.GetUserIcon(_testContext.MockUsers[0].Id).Result as ObjectResult;
            var body = res2.Value as ErrorResponse;


            Assert.Equal(StatusCodes.Status404NotFound, res2.StatusCode);
            Assert.Equal(ErrorCode.UserHasNoIcon, body.ErrorCode);
        }

        [Theory]
        [InlineData(ADMIN_DEP_ONE, DEPARTMENT_USER_DEP_TWO, ErrorCode.NoError)]
        [InlineData(ADMIN_DEP_ONE, GUARDIAN_DEP_TWO, ErrorCode.NoError)]
        [InlineData(ADMIN_DEP_ONE, CITIZEN_DEP_THREE, ErrorCode.NoError)]
        [InlineData(ADMIN_DEP_ONE, CITIZEN_DEP_TWO, ErrorCode.NoError)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, GUARDIAN_DEP_TWO, ErrorCode.NoError)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, CITIZEN_DEP_TWO, ErrorCode.NoError)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, CITIZEN_DEP_THREE, ErrorCode.NotAuthorized)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, ADMIN_DEP_ONE, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, CITIZEN_DEP_TWO, ErrorCode.NoError)]
        [InlineData(CITIZEN_DEP_TWO, CITIZEN_DEP_THREE, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, GUARDIAN_DEP_TWO, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, DEPARTMENT_USER_DEP_TWO, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, ADMIN_DEP_ONE, ErrorCode.NotAuthorized)]
        [InlineData(GUARDIAN_DEP_TWO, CITIZEN_DEP_TWO, ErrorCode.NoError)]
        [InlineData(GUARDIAN_DEP_TWO, ADMIN_DEP_ONE, ErrorCode.NotAuthorized)]
        [InlineData(GUARDIAN_DEP_TWO, DEPARTMENT_USER_DEP_TWO, ErrorCode.NotAuthorized)]
        [InlineData(GUARDIAN_DEP_TWO, CITIZEN_DEP_THREE, ErrorCode.NotAuthorized)]
        public void DeleteUserIcon_AuthenticationChecks(int authUser,
                                                        int userToEdit, ErrorCode expectedError)
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[userToEdit]);
            _testContext.MockHttpContext.MockRequestImage(_pngFilepath);
            usercontroller.SetUserIcon(_testContext.MockUsers[userToEdit].Id).Wait();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[authUser]);
            var res = usercontroller.DeleteUserIcon(_testContext.MockUsers[userToEdit].Id).Result as ObjectResult;

            if (expectedError != ErrorCode.NoError) {
                var body = res.Value as ErrorResponse;
                Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
                Assert.Equal(expectedError, body.ErrorCode);
            } else {
                Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            }
        }


        #endregion
        #region GetUser
        public void GetUser_CitizenLogin_Success()
        {
            var usercontroller = initializeTest();
            var mockUser = _testContext.MockUsers[CITIZEN_DEP_TWO];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var res = usercontroller.GetUser(mockUser.Id).Result as ObjectResult;
            var body = res.Value as SuccessResponse<GirafUserDTO> ;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            // check that we are logged in as the correct user
            Assert.Equal(mockUser.UserName, body.Data.Username);
            Assert.Equal(mockUser.DepartmentKey, body.Data.Department);
        }

        [Fact] 
        public void GetUser_GuardianLogin_Success()
        {
            var usercontroller = initializeTest();
            var mockUser = _testContext.MockUsers[GUARDIAN_DEP_TWO];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var res = usercontroller.GetUser(mockUser.Id).Result as ObjectResult;
            var body = res.Value as SuccessResponse<GirafUserDTO>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            Assert.Equal(mockUser.UserName, body.Data.Username);
            Assert.Equal(mockUser.DepartmentKey, body.Data.Department);
        }

        [Fact]
        public void GetUser_GuardianLoginUsernameInDepartment_Success()
        {
            var usercontroller = initializeTest();
            var mockUser = _testContext.MockUsers[GUARDIAN_DEP_TWO];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);

            var res = usercontroller.GetUser(mockUser.Id).Result as ObjectResult;
            var body = res.Value as SuccessResponse<GirafUserDTO>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            Assert.Equal(_testContext.MockUsers[GUARDIAN_DEP_TWO].UserName, body.Data.Username);
        }

        [Fact]
        public void GetUser_GuardianLoginUsernameNotInDepartment_UserNotFound()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var res = usercontroller.GetUser("invalid").Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status404NotFound, res.StatusCode);
            Assert.Equal(ErrorCode.UserNotFound, body.ErrorCode);
        }

        [Fact]
        public void GetUser_AdminLoginUsernameQuery_Success()
        {
            var usercontroller = initializeTest();
            var mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var res = usercontroller.GetUser(mockUser.Id).Result as ObjectResult;
            var body = res.Value as SuccessResponse<GirafUserDTO>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            Assert.Equal(_testContext.MockUsers[ADMIN_DEP_ONE].UserName, body.Data.Username);
            Assert.Equal(_testContext.MockUsers[ADMIN_DEP_ONE].DepartmentKey, body.Data.Department);
        }

        [Fact]
        public void GetUser_AdminLoginInvalidUsername_UserNotFound()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = usercontroller.GetUser("invalidId").Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status404NotFound, res.StatusCode);
            Assert.Equal(ErrorCode.UserNotFound, body.ErrorCode);
        }

        [Fact]
        public void GetUser_LoginAsCitizenTryGetOtherCitizen_NotAuthorized()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_THREE]);
            var res = usercontroller.GetUser(_testContext.MockUsers[CITIZEN_DEP_TWO].Id).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
            Assert.Equal(ErrorCode.NotAuthorized, body.ErrorCode);
        }
        #endregion

        #region GetGuardians

        [Fact]
        // Because guardians are allowed to call Get Guardians of a given citizen, they will get UserHasNoGuardians error if called on a guardian
        // GetUser_GetCitizensAsCitizen_Error is omitted as it gives a different error depending on from where it is called. It should give NotFound as a Citizen is not authorised to call the method, however, Authorize does not work properly when unit testing and therefore it would return UserHasNoCitizens error.
        public void GetUser_GetGuardiansAsGuardian_Forbidden()
        {
            var usercontroller = initializeTest();
            var user = _testContext.MockUsers[GUARDIAN_DEP_TWO];
            _testContext.MockUserManager.MockLoginAsUser(user);
            var res = usercontroller.GetGuardians(user.Id).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
            Assert.Equal(ErrorCode.Forbidden, body.ErrorCode);
        }

        [Fact]
        public void GetUser_GetGuardiansAsCitizen_Success()
        {
            var usercontroller = initializeTest();
            var user = _testContext.MockUsers[CITIZEN_DEP_TWO];
            _testContext.MockUserManager.MockLoginAsUser(user);
            var res = usercontroller.GetGuardians(user.Id).Result as ObjectResult;
            var body = res.Value as SuccessResponse<List<UserNameDTO>>;

            var guardians = new List<UserNameDTO>();
            foreach (var guardian in user.Guardians)
            {
                guardians.Add(new UserNameDTO { UserId = guardian.Guardian.Id, UserName = guardian.Guardian.UserName });
            }
            
            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);

            Assert.Equal(body.Data.FirstOrDefault().UserName, guardians.FirstOrDefault().UserName);
            Assert.Equal(body.Data.Count(), guardians.Count());
        }

        [Fact]
        public void GetUser_GetGuardiansAsCitizenWrongUsername_Invalid()
        {
            var usercontroller = initializeTest();
            var user = _testContext.MockUsers[CITIZEN_DEP_TWO];
            _testContext.MockUserManager.MockLoginAsUser(user);
            var res = usercontroller.GetGuardians("").Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal(ErrorCode.InvalidProperties, body.ErrorCode);
        }

        [Theory]
        [InlineData(ADMIN_DEP_ONE, DEPARTMENT_USER_DEP_TWO, ErrorCode.Forbidden)]
        [InlineData(ADMIN_DEP_ONE, GUARDIAN_DEP_TWO, ErrorCode.Forbidden)]
        [InlineData(ADMIN_DEP_ONE, CITIZEN_DEP_THREE, ErrorCode.NoError)]
        [InlineData(ADMIN_DEP_ONE, CITIZEN_DEP_TWO, ErrorCode.NoError)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, GUARDIAN_DEP_TWO, ErrorCode.Forbidden)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, CITIZEN_DEP_TWO, ErrorCode.NoError)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, CITIZEN_DEP_THREE, ErrorCode.NotAuthorized)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, ADMIN_DEP_ONE, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, CITIZEN_DEP_TWO, ErrorCode.NoError)]
        [InlineData(CITIZEN_DEP_TWO, CITIZEN_DEP_THREE, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, GUARDIAN_DEP_TWO, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, DEPARTMENT_USER_DEP_TWO, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, ADMIN_DEP_ONE, ErrorCode.NotAuthorized)]
        [InlineData(GUARDIAN_DEP_TWO, CITIZEN_DEP_TWO, ErrorCode.NoError)]
        [InlineData(GUARDIAN_DEP_TWO, ADMIN_DEP_ONE, ErrorCode.NotAuthorized)]
        [InlineData(GUARDIAN_DEP_TWO, DEPARTMENT_USER_DEP_TWO, ErrorCode.NotAuthorized)]
        [InlineData(GUARDIAN_DEP_TWO, CITIZEN_DEP_THREE, ErrorCode.NotAuthorized)]
        public void GetGuardians_AuthenticationChecks(int authUser, int userToEdit, ErrorCode expectedError)
        {
            var usercontroller = initializeTest();
            var user = _testContext.MockUsers[authUser];
            _testContext.MockUserManager.MockLoginAsUser(user);

            // add some test guardians to the user we are trying to edit as we just want to check authentication
            _testContext.MockUsers[userToEdit].Guardians = _testContext.MockUsers[CITIZEN_DEP_TWO].Guardians;
            var res = usercontroller.GetGuardians(_testContext.MockUsers[userToEdit].Id).Result as ObjectResult;

            if (expectedError != ErrorCode.NoError) {
                var body = res.Value as ErrorResponse;
                Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
                Assert.Equal(expectedError, body.ErrorCode);
            } else {
                Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            }
        }

        #endregion

        #region GetCitizens


        [Fact]
        public void GetUser_GetCitizensAsGuardian_Success()
        {
            var usercontroller = initializeTest();
            var user = _testContext.MockUsers[GUARDIAN_DEP_TWO];
            _testContext.MockUserManager.MockLoginAsUser(user);
            var res = usercontroller.GetCitizens(user.Id).Result as ObjectResult;
            var body = res.Value as SuccessResponse<List<UserNameDTO>>;

            var citizens = new List<UserNameDTO>();
            var citizenUser = _testContext.MockUsers[CITIZEN_DEP_TWO];
            citizens.Add(new UserNameDTO { UserId = citizenUser.Id, UserName = citizenUser.UserName });

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);

            Assert.Equal(body.Data.FirstOrDefault().UserName, citizens.FirstOrDefault().UserName);
            Assert.Equal(body.Data.Count(), citizens.Count());
        }

        [Fact]
        public void GetUser_GetCitizensAsGuardianWrongUsername_MissingProperties()
        {
            var usercontroller = initializeTest();
            var user = _testContext.MockUsers[GUARDIAN_DEP_TWO];
            _testContext.MockUserManager.MockLoginAsUser(user);
            var res = usercontroller.GetCitizens("").Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal(ErrorCode.MissingProperties, body.ErrorCode);
        }

        [Theory]
        [InlineData(ADMIN_DEP_ONE, DEPARTMENT_USER_DEP_TWO, ErrorCode.Forbidden)]
        [InlineData(ADMIN_DEP_ONE, GUARDIAN_DEP_TWO, ErrorCode.NoError)]
        [InlineData(ADMIN_DEP_ONE, CITIZEN_DEP_THREE, ErrorCode.Forbidden)]
        [InlineData(ADMIN_DEP_ONE, CITIZEN_DEP_TWO, ErrorCode.Forbidden)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, GUARDIAN_DEP_TWO, ErrorCode.NoError)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, CITIZEN_DEP_TWO, ErrorCode.Forbidden)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, CITIZEN_DEP_THREE, ErrorCode.NotAuthorized)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, ADMIN_DEP_ONE, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, CITIZEN_DEP_TWO, ErrorCode.Forbidden)]
        [InlineData(CITIZEN_DEP_TWO, CITIZEN_DEP_THREE, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, GUARDIAN_DEP_TWO, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, DEPARTMENT_USER_DEP_TWO, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, ADMIN_DEP_ONE, ErrorCode.NotAuthorized)]
        [InlineData(GUARDIAN_DEP_TWO, ADMIN_DEP_ONE, ErrorCode.NotAuthorized)]
        [InlineData(GUARDIAN_DEP_TWO, DEPARTMENT_USER_DEP_TWO, ErrorCode.NotAuthorized)]
        public void GetCitizens_AuthenticationChecks(int authUser, int userToEdit, ErrorCode expectedError)
        {
            var usercontroller = initializeTest();
            var user = _testContext.MockUsers[authUser];
            _testContext.MockUserManager.MockLoginAsUser(user);
            var res = usercontroller.GetCitizens(_testContext.MockUsers[userToEdit].Id).Result as ObjectResult;

            if (expectedError != ErrorCode.NoError) {
                var body = res.Value as ErrorResponse;
                Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
                Assert.Equal(expectedError, body.ErrorCode);
            } else {
                Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            }

        }

        #endregion


        #region UpdateUser

        [Fact] 
        public void UpdateUser_ValidUserValidRequest_Success()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var newUserName = "John";
            var newScreenName = "Sir John";

            var res = usercontroller.UpdateUser(_testContext.MockUsers[ADMIN_DEP_ONE].Id, new GirafUserDTO(){
                ScreenName = newScreenName,
                Username = newUserName
            }).Result as ObjectResult;
            var body = res.Value as SuccessResponse<GirafUserDTO>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            // check that the updated data is correct
            Assert.Equal(newUserName, body.Data.Username);
            Assert.Equal(newScreenName, body.Data.ScreenName);
        }

        [Fact]
        public void UpdateUser_ValidUserNullDTO_MissingProperties()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = usercontroller.UpdateUser(null, null).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal(ErrorCode.MissingProperties, body.ErrorCode);
        }


        [Fact]
        public void UpdateUser_ScreenNameNull_MissingProperties(){
            var usercontroller = initializeTest();
            var mockUser = _testContext.MockUsers[UserCitizenDepartment1];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var res = usercontroller.UpdateUser(mockUser.Id, new GirafUserDTO(){
                ScreenName = null,
                Username = "Henning"
            }).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal(ErrorCode.MissingProperties, body.ErrorCode);
        }

        [Fact]
        public void UpdateUser_NotAuthorized()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[UserDepartment2]);
            var user = _testContext.MockUsers[UserCitizenDepartment1];
            var res = usercontroller.UpdateUser(user.Id, new GirafUserDTO(){
                Username = "Charles Junior",
                ScreenName = "Charles Junior"
            }).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
            Assert.Equal(ErrorCode.NotAuthorized, body.ErrorCode);
        }

        [Fact]
        public void UpdateUser_SameDepartmentWithDepLogin_Success()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[UserDepartment2]);
            var user = _testContext.MockUsers[UserCitizenDepartment2];
            var res = usercontroller.UpdateUser(user.Id, new GirafUserDTO(){
                Username = "Charles",
                ScreenName = "Junior"
            }).Result as ObjectResult;
            var body = res.Value as SuccessResponse<GirafUserDTO>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            // check data
            Assert.Equal("Charles", body.Data.Username);
            Assert.Equal("Junior", body.Data.ScreenName);
        }

        [Fact]
        public void UpdateUser_SameDepartmenSameUsername_Success()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[UserDepartment2]);
            var user = _testContext.MockUsers[UserCitizenDepartment2];
            var res = usercontroller.UpdateUser(user.Id, new GirafUserDTO(){
                Username = user.UserName,
                ScreenName = "Gunnar"
            }).Result as ObjectResult;
            var body = res.Value as SuccessResponse<GirafUserDTO>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            // check data
            Assert.Equal(user.UserName, body.Data.Username);
            Assert.Equal("Gunnar", body.Data.ScreenName);
        }

        [Theory]
        [InlineData(DEPARTMENT_USER_DEP_TWO, CITIZEN_DEP_THREE, ErrorCode.NotAuthorized)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, ADMIN_DEP_ONE, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, CITIZEN_DEP_THREE, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, GUARDIAN_DEP_TWO, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, DEPARTMENT_USER_DEP_TWO, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, ADMIN_DEP_ONE, ErrorCode.NotAuthorized)]
        [InlineData(GUARDIAN_DEP_TWO, ADMIN_DEP_ONE, ErrorCode.NotAuthorized)]
        [InlineData(GUARDIAN_DEP_TWO, DEPARTMENT_USER_DEP_TWO, ErrorCode.NotAuthorized)]
        public void UpdateUser_AuthenticationChecksErrors(int authUser, int userToEdit, ErrorCode expectedError)
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[authUser]);
            var userName = "Henning";
            var screenName = "Heavy Henning";
            var res = usercontroller.UpdateUser(_testContext.MockUsers[userToEdit].Id, new GirafUserDTO(){
                ScreenName = screenName,
                Username = userName
            }).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
            Assert.Equal(expectedError, body.ErrorCode);
        }

        [Theory]
        [InlineData(ADMIN_DEP_ONE, DEPARTMENT_USER_DEP_TWO)]
        [InlineData(ADMIN_DEP_ONE, GUARDIAN_DEP_TWO)]
        [InlineData(ADMIN_DEP_ONE, CITIZEN_DEP_THREE)]
        [InlineData(ADMIN_DEP_ONE, CITIZEN_DEP_TWO)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, GUARDIAN_DEP_TWO)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, CITIZEN_DEP_TWO)]
        [InlineData(GUARDIAN_DEP_TWO, CITIZEN_DEP_TWO)]
        [InlineData(CITIZEN_DEP_TWO, CITIZEN_DEP_TWO)]
        public void UpdateUser_AuthenticationChecksNoErrors(int authUser, int userToEdit)
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[authUser]);
            var userName = "Henning";
            var screenName = "Heavy Henning";
            var res = usercontroller.UpdateUser(_testContext.MockUsers[userToEdit].Id, new GirafUserDTO(){
                ScreenName = screenName,
                Username = userName
            }).Result as ObjectResult;
            var body = res.Value as SuccessResponse<GirafUserDTO>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            Assert.NotNull(body.Data);
        }

        #endregion

        #region GuardianRelation 

        [Fact]
        public void AddGuardianCitizenRelationship_AddGuardianToCitizen_Success()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = usercontroller.AddGuardianCitizenRelationship(
                _testContext.MockUsers[1].Id, _testContext.MockUsers[2].Id
            ).Result as ObjectResult;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
        }

        [Fact]
        public void AddGuardianCitizenRelationship_InvalidGuardianUser_UserNotFound()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = usercontroller.AddGuardianCitizenRelationship(
                "", _testContext.MockUsers[2].Id
            ).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status404NotFound, res.StatusCode);
            Assert.Equal(ErrorCode.UserNotFound, body.ErrorCode);
        }

        #endregion


        #region AddUserResource
        [Fact]
        [System.Obsolete]
        public void AddUserResource_OwnPrivateValidUser_Success()
        {
            var usercontroller = initializeTest();
            string targetUserId = _testContext.MockUsers[CITIZEN_DEP_TWO].Id;
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var res = usercontroller
                .AddUserResource(targetUserId, new ResourceIdDTO() {Id = GuardianPrivatePictogram})
                .Result as ObjectResult;
            var body = res.Value as GirafUserDTO;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            // check ressource is added correctly
            Assert.True(_testContext.MockUsers[CITIZEN_DEP_TWO].Resources
                        .FirstOrDefault(r => r.PictogramKey == GuardianPrivatePictogram) != null);
        }


        [Fact]
        [System.Obsolete]
        public void AddUserResource_OwnPrivateInvalidUser_UserNotFound()
        {
            var usercontroller = initializeTest();
            string targetUserId = "INVALID";
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var res = usercontroller
                .AddUserResource(targetUserId, new ResourceIdDTO() { Id = GuardianPrivatePictogram })
                .Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status404NotFound, res.StatusCode);
            Assert.Equal(ErrorCode.UserNotFound, body.ErrorCode);
        }


        [Fact]
        [System.Obsolete]
        public void AddUserResource_OwnProtectedValidUser_ResourceMustBePrivate()
        {
            var usercontroller = initializeTest();
            string targetUserId = _testContext.MockUsers[CITIZEN_DEP_TWO].Id;
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var res = usercontroller
                .AddUserResource(targetUserId, new ResourceIdDTO() { Id = GuardianProtectedPictogram })
                .Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal(ErrorCode.ResourceMustBePrivate, body.ErrorCode);
        }


        [Fact]
        [System.Obsolete]
        public void AddUserResource_OwnProtectedInvalidUser_UserNotFound()
        {
            var usercontroller = initializeTest();
            string targetUserId = "INVALID";
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var res = usercontroller
                .AddUserResource(targetUserId, new ResourceIdDTO() { Id = PublicPictogram })
                .Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status404NotFound, res.StatusCode);
            Assert.Equal(ErrorCode.UserNotFound, body.ErrorCode);
        }

        [Fact]
        [System.Obsolete]
        public void AddUserResource_AnotherProtectedValidUser_NotAuthorized() 
        {
            var usercontroller = initializeTest();
            string targetUserId = _testContext.MockUsers[CITIZEN_DEP_TWO].Id;
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_THREE]);
            var res = usercontroller
                .AddUserResource(targetUserId, new ResourceIdDTO() { Id = CitizenPrivatePictogram })
                .Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
            Assert.Equal(ErrorCode.NotAuthorized, body.ErrorCode);
        }

        [Fact]
        [System.Obsolete]
        public void AddUserResource_AnotherProtectedInvalidUser_UserNotFound()
        {
            var usercontroller = initializeTest();
            string targetUserId = "INVALID";
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_THREE]);
            var res = usercontroller
                .AddUserResource(targetUserId, new ResourceIdDTO() { Id = PublicPictogram })
                .Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status404NotFound, res.StatusCode);
            Assert.Equal(ErrorCode.UserNotFound, body.ErrorCode);
        }

        [Fact]
        [System.Obsolete]
        public void AddUserResource_PublicValidUser_NotAuthorized()
        {
            var usercontroller = initializeTest();
            string targetUserId = _testContext.MockUsers[GUARDIAN_DEP_TWO].Id;
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_TWO]);
            var res = usercontroller
                .AddUserResource(targetUserId, new ResourceIdDTO() { Id = CitizenPrivatePictogram })
                .Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
            Assert.Equal(ErrorCode.NotAuthorized, body.ErrorCode);
        }

        [Fact]
        [System.Obsolete]
        public void AddUserResource_PublicInvalidUser_UserNotFound()
        {
            var usercontroller = initializeTest();
            string targetUserId = "INVALID";
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_TWO]);
            var res = usercontroller
                .AddUserResource(targetUserId, new ResourceIdDTO() { Id = PublicPictogram })
                .Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status404NotFound, res.StatusCode);
            Assert.Equal(ErrorCode.UserNotFound, body.ErrorCode);
        }

        [Fact]
        [System.Obsolete]
        public void AddUserResource_AnotherPrivateValidUser_NotAuthorized()
        {
            var usercontroller = initializeTest();
            string targetUserId = _testContext.MockUsers[GUARDIAN_DEP_TWO].Id;
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_TWO]);
            var res = usercontroller
                .AddUserResource(targetUserId, new ResourceIdDTO() { Id = GuardianPrivatePictogram })
                .Result as ObjectResult;
            var body = res.Value as ErrorResponse;
            
            Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
            Assert.Equal(ErrorCode.NotAuthorized, body.ErrorCode);
       }

        [Fact]
        [System.Obsolete]
        public void AddUserResource_AnotherPrivateInvalidUser_UserNotFound()
        {
            var usercontroller = initializeTest();
            string targetUserId = "INVALID";
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_TWO]);

            var res = usercontroller
                .AddUserResource(targetUserId, new ResourceIdDTO() { Id = PublicPictogram })
                .Result as ObjectResult;
            var body = res.Value as ErrorResponse;
            
            Assert.Equal(StatusCodes.Status404NotFound, res.StatusCode);
            Assert.Equal(ErrorCode.UserNotFound, body.ErrorCode);
        }
        #endregion
        #region DeleteResource
        [Fact]
        [System.Obsolete]
        public void DeleteResource_OwnPrivateValidUser_Success()
        {
            var usercontroller = initializeTest();
            var mockuser = _testContext.MockUsers[GUARDIAN_DEP_TWO];
            _testContext.MockUserManager.MockLoginAsUser(mockuser);
            var res = usercontroller
                .DeleteResource(mockuser.Id,new ResourceIdDTO() { Id = GuardianPrivatePictogram })
                .Result as ObjectResult;
            var body = res.Value as SuccessResponse<GirafUserDTO>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            // check that ressource no longer exist
            Assert.True(_testContext.MockUsers[GUARDIAN_DEP_TWO].Resources.FirstOrDefault(r => r.PictogramKey == GuardianPrivatePictogram) == null);
        }

        [Fact]
        [System.Obsolete]
        public void DeleteResource_PrivateNoUser_Error()
        {
            var usercontroller = initializeTest();
            var res = usercontroller
                .DeleteResource("INVALID",new ResourceIdDTO() { Id = GuardianPrivatePictogram })
                .Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status404NotFound, res.StatusCode);
            Assert.Equal(ErrorCode.UserNotFound, body.ErrorCode);
        }

        [Fact]
        [System.Obsolete]
        public void DeleteResource_OwnProtectedValidUser_UserDoesNotOwnResource()
        {
            var usercontroller = initializeTest();
            var mockUser = _testContext.MockUsers[GUARDIAN_DEP_TWO];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var res = usercontroller
                .DeleteResource(mockUser.Id, new ResourceIdDTO() { Id = GuardianProtectedPictogram })
                .Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
            Assert.Equal(ErrorCode.UserDoesNotOwnResource, body.ErrorCode);
        }

        [Fact]
        [System.Obsolete]
        public void DeleteResource_OwnProtectedInvalidUser_UserDoesNotOwnResource()
        {
            var usercontroller = initializeTest();
            var mockUser = _testContext.MockUsers[GUARDIAN_DEP_TWO];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var res = usercontroller
                .DeleteResource(mockUser.Id, new ResourceIdDTO() { Id = GuardianProtectedPictogram })
                .Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
            Assert.Equal(ErrorCode.UserDoesNotOwnResource, body.ErrorCode);
        }

        [Fact]
        [System.Obsolete]
        public void DeleteResource_PublicValidUser_UserDoesNotOwnResource()
        {
            var usercontroller = initializeTest();
            var mockUser = _testContext.MockUsers[GUARDIAN_DEP_TWO];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);

            var res = usercontroller
                .DeleteResource(mockUser.Id, new ResourceIdDTO() { Id = PublicPictogram })
                .Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
            Assert.Equal(ErrorCode.UserDoesNotOwnResource, body.ErrorCode);
        }

        [Fact]
        [System.Obsolete]
        public void DeleteResource_PublicInvalidUser_UserNotFound()
        {
            var usercontroller = initializeTest();

            var res = usercontroller
                .DeleteResource("Invalid", new ResourceIdDTO() { Id = PublicPictogram })
                .Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status404NotFound, res.StatusCode);
            Assert.Equal(ErrorCode.UserNotFound, body.ErrorCode);
        }

        #endregion
        #region Settings

        [Fact]
        public void UpdateUserSettings_landscapeOrientation_Success()
        {
            var usercontroller = initializeTest();
            var mockUser = _testContext.MockUsers[CITIZEN_DEP_TWO];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            
            var dto = UserSettings[0];
            dto.Orientation = Orientation.landscape;
            usercontroller.UpdateUserSettings(mockUser.Id, dto).Wait();
            Assert.Equal(Orientation.landscape, _testContext.MockUsers[CITIZEN_DEP_TWO].Settings.Orientation);
        }  

        [Fact]
        public void UpdateUserSettings_checkedResourceAppearenceMovedToRight_Success()
        {
            var usercontroller = initializeTest();
            var mockUser = _testContext.MockUsers[CITIZEN_DEP_TWO];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            
            var dto = UserSettings[0];
            dto.CompleteMark = CompleteMark.MovedRight;
            usercontroller.UpdateUserSettings(mockUser.Id, dto).Wait();

            Assert.Equal(CompleteMark.MovedRight, _testContext.MockUsers[CITIZEN_DEP_TWO].Settings.CompleteMark);
        }

        /// <summary>
        ///  Check that we cannot update user settings for any other user if our role is that of a citizen
        /// </summary>
        [Fact]
        public void UpdateUserSettings_AsAnotherUnrelatedUser_Error()
        {
            var usercontroller = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_TWO]);

            var dto = UserSettings[0];
            dto.CompleteMark = CompleteMark.MovedRight;
            var res = usercontroller.UpdateUserSettings(_testContext.MockUsers[CITIZEN_DEP_THREE].Id, dto).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
            Assert.Equal(ErrorCode.NotAuthorized, body.ErrorCode);
        }

        [Fact]
        public void UpdateUserSettings_landscape_Success()
        {
            var usercontroller = initializeTest();
            var mockUser = _testContext.MockUsers[CITIZEN_DEP_TWO];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            
            var dto = UserSettings[0];
            dto.Orientation = Orientation.landscape;
            usercontroller.UpdateUserSettings(mockUser.Id, dto).Wait();

            Assert.Equal(Orientation.landscape, _testContext.MockUsers[CITIZEN_DEP_TWO].Settings.Orientation);
        }        

        [Fact]
        public void UpdateUserSettings_analogClock_defaultTimer_Success()
        {
            var usercontroller = initializeTest();
            var mockUser = _testContext.MockUsers[CITIZEN_DEP_TWO];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            
            var dto = UserSettings[0];
            dto.DefaultTimer = DefaultTimer.analogClock;
            usercontroller.UpdateUserSettings(mockUser.Id, dto).Wait();

            Assert.Equal(DefaultTimer.analogClock, _testContext.MockUsers[CITIZEN_DEP_TWO].Settings.DefaultTimer);
        }    

        [Fact]
        public void UpdateUserSettings_timerSeconds_Success()
        {
            var usercontroller = initializeTest();
            var mockUser = _testContext.MockUsers[CITIZEN_DEP_TWO];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            
            var dto = UserSettings[0];
            dto.TimerSeconds = 25;
            usercontroller.UpdateUserSettings(mockUser.Id, dto).Wait();

            Assert.Equal(25, _testContext.MockUsers[CITIZEN_DEP_TWO].Settings.TimerSeconds);
        }

        [Fact]
        public void UpdateUserSettings_activitiesCount_Success()
        {
            var usercontroller = initializeTest();
            var mockUser = _testContext.MockUsers[CITIZEN_DEP_TWO];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            
            var dto = UserSettings[0];
            dto.ActivitiesCount = 30;
            usercontroller.UpdateUserSettings(mockUser.Id, dto).Wait();

            Assert.Equal(30, _testContext.MockUsers[CITIZEN_DEP_TWO].Settings.ActivitiesCount);
        }

        [Fact]
        public void UpdateUserSettings_girafGreen_Success()
        {
            var usercontroller = initializeTest();
            var mockUser = _testContext.MockUsers[CITIZEN_DEP_TWO];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            
            var dto = UserSettings[0];
            dto.Theme = Theme.girafGreen;
            usercontroller.UpdateUserSettings(mockUser.Id, dto).Wait();

            Assert.Equal(Theme.girafGreen, _testContext.MockUsers[CITIZEN_DEP_TWO].Settings.Theme);
        }

        [Fact]
        public void UpdateUserSettings_UpdateSameUserSettings_Success(){
            var usercontroller = initializeTest();
            var user = _testContext.MockUsers[CITIZEN_DEP_TWO];
            _testContext.MockUserManager.MockLoginAsUser(user);
            var dto = UserSettings[0];

            var res = usercontroller.UpdateUserSettings(user.Id, dto).Result as ObjectResult;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            Assert.Equal(Theme.girafGreen, _testContext.MockUsers[CITIZEN_DEP_TWO].Settings.Theme);
            Assert.Equal(120, _testContext.MockUsers[CITIZEN_DEP_TWO].Settings.TimerSeconds);
            Assert.Equal(DefaultTimer.analogClock, _testContext.MockUsers[CITIZEN_DEP_TWO].Settings.DefaultTimer);
            Assert.Equal(5, _testContext.MockUsers[CITIZEN_DEP_TWO].Settings.ActivitiesCount);
            Assert.Equal(5, _testContext.MockUsers[CITIZEN_DEP_TWO].Settings.NrOfDaysToDisplay);
            Assert.True(_testContext.MockUsers[CITIZEN_DEP_TWO].Settings.GreyScale);
            Assert.True(_testContext.MockUsers[CITIZEN_DEP_TWO].Settings.LockTimerControl);
        }

        [Theory]
        [InlineData(ADMIN_DEP_ONE, CITIZEN_DEP_THREE, ErrorCode.NoError)]
        [InlineData(ADMIN_DEP_ONE, CITIZEN_DEP_TWO, ErrorCode.NoError)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, CITIZEN_DEP_TWO, ErrorCode.NoError)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, CITIZEN_DEP_THREE, ErrorCode.NotAuthorized)]
        [InlineData(GUARDIAN_DEP_TWO, CITIZEN_DEP_TWO, ErrorCode.NoError)]
        [InlineData(GUARDIAN_DEP_TWO, CITIZEN_DEP_THREE, ErrorCode.NotAuthorized)]
        public void UpdateUserSettings__AuthenticationChecks(int authUser, int userToEdit, ErrorCode expectedError)
        {
            var usercontroller = initializeTest();
            var user = _testContext.MockUsers[authUser];
            _testContext.MockUserManager.MockLoginAsUser(user);
            var dto = UserSettings[0];

            var res = usercontroller.UpdateUserSettings(_testContext.MockUsers[userToEdit].Id, dto).Result as ObjectResult;

            if (expectedError != ErrorCode.NoError) {
                var body = res.Value as ErrorResponse;
                Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
                Assert.Equal(expectedError, body.ErrorCode);
            } else {
                Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            }
        }

        [Theory]
        [InlineData(ADMIN_DEP_ONE, CITIZEN_DEP_THREE, ErrorCode.NoError)]
        [InlineData(ADMIN_DEP_ONE, CITIZEN_DEP_TWO, ErrorCode.NoError)]
        [InlineData(ADMIN_DEP_ONE, DEPARTMENT_USER_DEP_TWO, ErrorCode.RoleMustBeCitizien)]
        [InlineData(ADMIN_DEP_ONE, GUARDIAN_DEP_TWO, ErrorCode.RoleMustBeCitizien)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, CITIZEN_DEP_TWO, ErrorCode.NoError)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, GUARDIAN_DEP_TWO, ErrorCode.RoleMustBeCitizien)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, ADMIN_DEP_ONE, ErrorCode.RoleMustBeCitizien)]
        [InlineData(GUARDIAN_DEP_TWO, ADMIN_DEP_ONE, ErrorCode.RoleMustBeCitizien)]
        [InlineData(GUARDIAN_DEP_TWO, DEPARTMENT_USER_DEP_TWO, ErrorCode.RoleMustBeCitizien)]
        [InlineData(GUARDIAN_DEP_TWO, CITIZEN_DEP_TWO, ErrorCode.NoError)]
        public void UpdateUserSettings__RoleChecks(int authUser, int userToEdit, ErrorCode expectedError)
        {
            var usercontroller = initializeTest();
            var user = _testContext.MockUsers[authUser];
            _testContext.MockUserManager.MockLoginAsUser(user);
            var dto = UserSettings[0];

            var res = usercontroller.UpdateUserSettings(_testContext.MockUsers[userToEdit].Id, dto).Result as ObjectResult;

            if (expectedError != ErrorCode.NoError) {
                var body = res.Value as ErrorResponse;
                Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
                Assert.Equal(expectedError, body.ErrorCode);
            } else {
                Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            }
        }
        
        #endregion

    }
}
