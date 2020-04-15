using GirafRest.Controllers;
using GirafRest.Models.DTOs.AccountDTOs;
using GirafRest.Test.Mocks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Xunit.Abstractions;
using static GirafRest.Test.UnitTestExtensions;
using GirafRest.Services;
using GirafRest.Models.DTOs;
using GirafRest.Models.Responses;
using Microsoft.Extensions.Options;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Http; 


namespace GirafRest.Test
{
    public class AccountControllerTest
    {
        //NOTE: We do not test the logout method as it is merely an almost invisble abstraction on top of SignInManager.SignOut.
        private readonly ITestOutputHelper _outputHelper;
        private TestContext _testContext;

        private const int ADMIN_DEP_ONE = 0;
        private const int DEPARTMENT_ONE = 1;
        private const int GUARDIAN_DEP_TWO = 1;
        private const int ANOTHER_GUARDIAN_DEP_TWO = 5;
        private const int CITIZEN_DEP_TWO = 2;
        private const int CITIZEN_DEP_THREE = 3;
        private const int ADMIN_NO_DEP = 4;
        private const int DEPARTMENT_DEP_TWO = 6;


        public AccountControllerTest(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        private void OutputEmail(string r, string s, string m)
        {
            _outputHelper.WriteLine($"Email sent:\nReceiver: {r}\nSubject: {s}\n\n{m}");
        }
        
        private AccountController InitializeTest()
        {
            _testContext = new TestContext();


            var mockSignInManager = new MockSignInManager(_testContext.MockUserManager, _testContext);
            var mockGirafService = new MockGirafService(_testContext.MockDbContext.Object, _testContext.MockUserManager);

            //TODO: Error here. Unsucessful attempt at mocking., TRYING OUT NEW ASGER WAY OF LIFE
            var roleManager = _testContext.MockRoleManager.Object;
            //var roleManager = new RoleManager<GirafRole>(new Mock<IRoleStore<GirafRole>>().Object, null, null, null, null, null);
            
            AccountController ac = new AccountController(
                mockSignInManager,
                _testContext.MockLoggerFactory.Object,
                mockGirafService,
                Options.Create(new JwtConfig()
                {
                    JwtKey = "123456789123456789123456789",
                    JwtIssuer = "example.com",
                    JwtExpireDays = 30
                }),
                roleManager,
                new GirafAuthenticationService(_testContext.MockDbContext.Object, _testContext.MockRoleManager.Object,
                                               _testContext.MockUserManager));

            _testContext.MockHttpContext = ac.MockHttpContext();
            _testContext.MockHttpContext
                .Setup(mhc => mhc.Request.Scheme)
                .Returns("Scheme?");

            var mockUrlHelper = new Mock<IUrlHelper>();
            ac.Url = mockUrlHelper.Object;

            return ac;
        }

        #region Login
        // When logging in, one is only allowed to login as users below them in the hierarchy. The hierarchy in order is: Admin, Department, Guardian, Citizen
        // Check if possible to login with mock credentials. All passwords are initialised (Data folder, DBInitializer.cs) to be "password"

        [Fact]
        public void Login_CredentialsOk_Success()
        {
            var accountController = InitializeTest();

            var res = accountController.Login(new LoginDTO()
            {
                Username = _testContext.MockUsers[ADMIN_DEP_ONE].UserName,
                Password = "password"
            }).Result as ObjectResult;

            var body = res.Value as MyResponse;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);

            // Check that jwt token is not null and atleast contains 40 characters
            Assert.NotNull(body.Data);
            Assert.True(body.Data.Length >= 40);
        }

        // Same user log in twice no problem
        [Fact]
        public void Login_SameUserLoginTwice_Success()
        {
            var accountController = InitializeTest();
            var username = _testContext.MockUsers[ADMIN_DEP_ONE].UserName;

            var resA = accountController.Login(new LoginDTO()
            {
                Username = username,
                Password = "password"
            }).Result as ObjectResult;

            var resB = accountController.Login(new LoginDTO()
            {
                Username = username,
                Password = "password"
            }).Result as ObjectResult;

            var bodyB = resB.Value as MyResponse;

            // Check that both requests are successful
            Assert.Equal(StatusCodes.Status200OK, resA.StatusCode);
            Assert.Equal(StatusCodes.Status200OK, resB.StatusCode);

            // Check that jwt token is not null and atleast contains 40 characters
            Assert.NotNull(bodyB.Data);
            Assert.True(bodyB.Data.Length >= 40);
        }

        [Fact]
        // If no user is found with given user name, return ErrorResponse with relevant ErrorCode (invalid credentials ensures we do not give the bad guys any information)
        public void Login_UsernameInvalidPasswordOk_InvalidCredentials()
        {
            var accountController = InitializeTest();

            var res = accountController.Login(new LoginDTO()
            {
                Username = "INVALID",
                Password = "password"
            }).Result as ObjectResult;
            var body = res.Value as RESTError;

            Assert.Equal(StatusCodes.Status401Unauthorized, res.StatusCode);
            Assert.Equal(ErrorCode.InvalidCredentials, body.ErrorCode);                        
        }

        [Fact]
        // Trying to login with no credentials:
        public void Login_NullDTO_MissingProperties()
        {
            var accountController = InitializeTest();

            var res = accountController.Login(null).Result as ObjectResult;

            var body = res.Value as RESTError;

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal(ErrorCode.MissingProperties, body.ErrorCode);
        }

        [Fact]
        // Trying to login with no password:
        public void Login_NullDTO_MissingPassword()
        {
            var accountController = InitializeTest();
            var username = _testContext.MockUsers[ADMIN_DEP_ONE].UserName;


            var res = accountController.Login(new LoginDTO(){
                Username = username
            }).Result as ObjectResult;

            var body = res.Value as RESTError;

            Assert.Equal(StatusCodes.Status401Unauthorized, res.StatusCode);
            Assert.Equal(ErrorCode.MissingProperties, body.ErrorCode);
        }

        [Fact]
        // Trying to login with no username:
        public void Login_NullDTO_MissingUsername()
        {
            var accountController = InitializeTest();
            var username = _testContext.MockUsers[ADMIN_DEP_ONE].UserName;


            var res = accountController.Login(new LoginDTO(){
                Password = "password"
            }).Result as ObjectResult;

            var body = res.Value as RESTError;

            Assert.Equal(StatusCodes.Status401Unauthorized, res.StatusCode);
            Assert.Equal(ErrorCode.MissingProperties, body.ErrorCode);
        }

        #endregion

        #region Register
        [Fact]
        public void Register_InputOk_Success()
        {
            var accountController = InitializeTest();

            var userName = "GenericName";
            var displayName = "GenericDisplayName";
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = accountController.Register(new RegisterDTO()
            {
                Username = userName,
                Password = "GenericPassword",
                DepartmentId = DEPARTMENT_ONE,
                Role = GirafRoles.Citizen,
                DisplayName = displayName
            }).Result;

            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            Assert.True(res.Success);
            Assert.NotNull(res.Data);
            // check data
            Assert.Equal(res.Data.Username, userName);
            Assert.Equal(res.Data.Department, DEPARTMENT_ONE);
        }

        [Fact]
        public void Register_NoDisplayNameSetAsUsername_Success()
        {
            var accountController = InitializeTest();

            var userName = "GenericName";
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = accountController.Register(new RegisterDTO()
            {
                Username = userName,
                Password = "GenericPassword",
                DepartmentId = DEPARTMENT_ONE,
                Role = GirafRoles.Citizen,
                DisplayName = null
            }).Result;

            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            Assert.True(res.Success);
            Assert.NotNull(res.Data);
            Assert.Equal(res.Data.ScreenName, userName);
        }
     
        [Fact]
        public void Register_ExistingUsername_UserAlreadyExists()
        {
            var accountController = InitializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = accountController.Register(new RegisterDTO()
            {
                Username = _testContext.MockUsers[ADMIN_DEP_ONE].UserName,
                Password = "password",
                DepartmentId = DEPARTMENT_ONE,
                Role = GirafRoles.Citizen
            }).Result;

            Assert.Equal(ErrorCode.UserAlreadyExists, res.ErrorCode);
            Assert.False(res.Success);
        }

        [Fact]
        public void Register_NoUsername_InvalidCredentials()
        {
            var accountController = InitializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = accountController.Register(new RegisterDTO()
            {
                Password = "password",
                DepartmentId = DEPARTMENT_ONE
            }).Result;

            Assert.Equal(ErrorCode.InvalidCredentials, res.ErrorCode);
            Assert.False(res.Success);
        }

        [Fact]
        // Account may exist without department
        // If user is without department, then Department=null, otherwise department = user.DepartmentKey
        public void Register_NoDepartment_Success()
        {
            var accountController = InitializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = accountController.Register(new RegisterDTO()
            {
                Username = "NewUser",
                Password = "password",
                Role = GirafRoles.Citizen
            }).Result;

            Assert.True(res.Success);
            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            // check data
            Assert.Equal(null, res.Data.Department);
        }
        
        [Fact]
        public void Register_BlankDTO_InvalidCredentials()
        {
            var accountController = InitializeTest();

            var res = accountController.Register(new RegisterDTO()
            {
                Username = "",
                Password = ""
            }).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.InvalidCredentials, res.ErrorCode);
        }

        [Fact]
        public void Register_GuardianRelation_Success(){
            var accountController = InitializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = accountController.Register(new RegisterDTO() { Username = "JohnDoe", 
                Password= "iSecretlyLoveMileyCyrus", DepartmentId = 2, Role = GirafRoles.Citizen}).Result;

            Assert.True(res.Success);
            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            // fetch expected guardian from test data
            var guardian = _testContext.MockUsers.FirstOrDefault(u => u.UserName == "Guardian in dep 2");
            var newUser = _testContext.MockUsers.FirstOrDefault(u => u.UserName == "JohnDoe");
            // check data
            Assert.Equal(2, newUser.Guardians.Count());
            Assert.Equal(guardian, newUser.Guardians.First().Guardian);
        }

        [Fact]
        public void Register_NullDTO_MissingProperties()
        {
            var accountController = InitializeTest();

            var res = accountController.Register(null).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.MissingProperties, res.ErrorCode);
        }
        #endregion


        #region ChangePassword
        [Fact]
        public void ChangePassword_ValidInput_Success()
        {
            var ac = InitializeTest();
            var mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);

            ChangePasswordDTO cpDTO = new ChangePasswordDTO()
            {
                OldPassword = "password",
                NewPassword = "PASSWORD"
            };

            var res = ac.ChangePasswordByOldPassword(mockUser.Id, cpDTO).Result;

            Assert.True(res.Success);
            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
        }

        [Fact]
        public void ChangePassword_NullDTO_MissingProperties()
        {
            var ac = InitializeTest();

            var mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
                _testContext.MockUserManager.MockLoginAsUser(mockUser);

            ChangePasswordDTO cpDTO = null;

            var res = ac.ChangePasswordByOldPassword(mockUser.Id, cpDTO).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.MissingProperties, res.ErrorCode);
        }

        [Fact]
        public void ChangePassword_WrongOldPassword_PasswordNotUpdated()
        {
            var ac = InitializeTest();

            var mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);

            ChangePasswordDTO cpDTO = new ChangePasswordDTO()
            {
                OldPassword = "drowssap",
                NewPassword = "PASSWORD",
            };

            var res = ac.ChangePasswordByOldPassword(mockUser.Id, cpDTO).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.PasswordNotUpdated, res.ErrorCode);
        }

        #endregion

        #region DeleteUser
        [Fact]
        public void DeleteUser_NotFound_UserNotFound()
        {
            var ac = InitializeTest();

            var result = ac.DeleteUser("7394").Result;

            var mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);

            Assert.False(result.Success);
            Assert.Equal(ErrorCode.UserNotFound, result.ErrorCode);
        }

        [Fact]
        public void DeleteUser_ValidInput_Success()
        {
            var ac = InitializeTest();
            var mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);

            var result = ac.DeleteUser(_testContext.MockUsers[CITIZEN_DEP_TWO].Id).Result;

            Assert.True(result.Success);
            Assert.Equal(ErrorCode.NoError, result.ErrorCode);
        }
        #endregion

    }
}