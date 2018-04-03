using GirafRest.Controllers;
using GirafRest.Models.DTOs.AccountDTOs;
using GirafRest.Test.Mocks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Xunit.Abstractions;
using static GirafRest.Test.UnitTestExtensions;
using GirafRest.Services;
using System.Threading.Tasks;
using GirafRest.Models.DTOs;
using GirafRest.Models.DTOs.UserDTOs;
using GirafRest.Models.Responses;
using Microsoft.Extensions.Options;

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

            var mockEmail = new Mock<IEmailService>();
            mockEmail.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback(new System.Action<string, string, string>(OutputEmail))
                .Returns(Task.FromResult(0));

            var mockSignInManager = new MockSignInManager(_testContext.MockUserManager, _testContext);
            var mockGirafService = new MockGirafService(_testContext.MockDbContext.Object, _testContext.MockUserManager);

            //TODO: Error here. Unsucessful attempt at mocking., TRYING OUT NEW ASGER WAY OF LIFE
            var roleManager = _testContext.MockRoleManager.Object;
            //var roleManager = new RoleManager<GirafRole>(new Mock<IRoleStore<GirafRole>>().Object, null, null, null, null, null);
            
            AccountController ac = new AccountController(
                mockSignInManager,
                mockEmail.Object,
                _testContext.MockLoggerFactory.Object,
                mockGirafService,
                Options.Create(new JwtConfig()
                {
                    JwtKey = "123456789123456789123456789",
                    JwtIssuer = "example.com",
                    JwtExpireDays = 30
                }),
                roleManager);

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
        public void Login_CredentialsOk_OK()
        {
            var accountController = InitializeTest();

            var res = accountController.Login(new LoginDTO()
            {
                Username = _testContext.MockUsers[ADMIN_DEP_ONE].UserName,
                Password = "password"
            }).Result;

            // Assert if type is reponse (verfies that it is the exact type and not a derived type (ErrorResponse)). No functionality enforces that we should not have type=ErrorResponse, ErrorCode=NoError OR type=Response, ErrorCode=some actual error
            Assert.IsType<Response<string>>(res);
            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            // Check that jwt token is not null and atleast contains 40 characters
            Assert.NotNull(res.Data);
            Assert.True(res.Data.Length >= 40);
        }

        // Same user log in twice no problem
        [Fact]
        public void Login_SameUserLoginTwice_OK()
        {
            var accountController = InitializeTest();
            var username = _testContext.MockUsers[ADMIN_DEP_ONE].UserName;

            var resA = accountController.Login(new LoginDTO()
            {
                Username = username,
                Password = "password"
            }).Result;

            var resB = accountController.Login(new LoginDTO()
            {
                Username = username,
                Password = "password"
            }).Result;

            // accountController.Login returns: new Response<GirafUserDTO>(new GirafUserDTO(loginUser, userRoles)) if login succeded
            Assert.IsType<Response<string>>(resB);
            Assert.Equal(ErrorCode.NoError, resB.ErrorCode);
            Assert.True(resB.Success);
            // Check that jwt token is not null and atleast contains 40 characters
            Assert.NotNull(resB.Data);
            Assert.True(resB.Data.Length >= 40);
        }

        [Fact]
        // If no user is found with given user name, return ErrorResponse with relevant ErrorCode (invalid credentials ensures we do not give the bad guys any information)
        public void Login_UsernameInvalidPasswordOk_Unauthorized()
        {
            var accountController = InitializeTest();

            var res = accountController.Login(new LoginDTO()
            {
                Username = "INVALID",
                Password = "password"
            }).Result;

            Assert.IsType<ErrorResponse<string>>(res);
            Assert.Equal(ErrorCode.InvalidCredentials, res.ErrorCode);
            Assert.False(res.Success);
        }

        [Fact]
        // Trying to login with no credentials:
        public void Login_NullDTO_BadRequest()
        {
            var accountController = InitializeTest();

            var res = accountController.Login(null).Result;

            Assert.IsType<ErrorResponse<string>>(res);
            Assert.Equal(ErrorCode.MissingProperties, res.ErrorCode);
            Assert.False(res.Success);
        }

        [Fact]
        // A guardian should be able to login as citizen
        public void Login_LoginAsGuardianDTOWithCitizenName_Ok()
        {
            var ac = InitializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            
            var res = ac.Login(new LoginDTO() { Username = _testContext.MockUsers[CITIZEN_DEP_TWO].UserName }).Result;

            Assert.IsType<Response<string>>(res);
            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            Assert.True(res.Success);
            // Check that jwt token is not null and atleast contains 40 characters
            Assert.NotNull(res.Data);
            Assert.True(res.Data.Length >= 40);
        }

        [Fact]
        // Guardian cannot login as admin, if departments do not match
        public void Login_LoginAsGuardianDTOWithAdminInNoDep_NotFound()
        {
            var ac = InitializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var res = ac.Login(new LoginDTO() { Username = _testContext.MockUsers[ADMIN_NO_DEP].UserName }).Result;

            Assert.IsType<ErrorResponse<string>>(res);
            Assert.Equal(ErrorCode.InvalidCredentials, res.ErrorCode);
            Assert.False(res.Success);
        }

        [Fact]
        // Guardian cannot login as another guardian (not even if in same department)
        public void Login_LoginAsGuardianDTOWithGuardianInSameDep_Unauthorized()
        {
            var ac = InitializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var res = ac.Login(new LoginDTO() { Username = _testContext.MockUsers[ANOTHER_GUARDIAN_DEP_TWO].UserName }).Result;

            Assert.IsType<ErrorResponse<string>>(res);
            Assert.Equal(ErrorCode.NotAuthorized, res.ErrorCode);
            Assert.False(res.Success);
        }

        [Fact]
        // Guardian cannot login as user that is not in their own department
        public void Login_LoginAsGuardianDTOWithUserInAnotherDep_NotFound()
        {
            var ac = InitializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var res = ac.Login(new LoginDTO()
            {
                Username = _testContext.MockUsers[CITIZEN_DEP_THREE].UserName
            }).Result;

            Assert.IsType<ErrorResponse<string>>(res);
            Assert.Equal(ErrorCode.InvalidCredentials, res.ErrorCode);
            Assert.False(res.Success);
        }

        [Fact]
        // Citizen cannot login as Guardian
        public void Login_LoginAsCitizenDTOWithGuardianInSameDep_Unauthorized()
        {
            var ac = InitializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_TWO]);

            var res = ac.Login(new LoginDTO() { Username = _testContext.MockUsers[GUARDIAN_DEP_TWO].UserName}).Result;

            Assert.IsType<ErrorResponse<string>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.InvalidCredentials, res.ErrorCode);
        }

        [Fact]
        public void Login_LoginAsDepartmentDTOWithGuardianInSameDep_Ok()
        {
            var ac = InitializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[DEPARTMENT_DEP_TWO]);

            var res = ac.Login(new LoginDTO() { Username = _testContext.MockUsers[GUARDIAN_DEP_TWO].UserName }).Result;

            Assert.IsType<Response<string>>(res);
            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            Assert.True(res.Success);
            // Check that jwt token is not null and atleast contains 40 characters
            Assert.NotNull(res.Data);
            Assert.True(res.Data.Length >= 40);
        }

        [Fact]
        public void Login_LoginAsGuardianDTOWithDepartmentInSameDep_Unauthorized()
        {
            var ac = InitializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var res = ac.Login(new LoginDTO() { Username = _testContext.MockUsers[DEPARTMENT_DEP_TWO].UserName }).Result;

            Assert.IsType<ErrorResponse<string>>(res);
            Assert.Equal(ErrorCode.NotAuthorized, res.ErrorCode);
            Assert.False(res.Success);
        }
        #endregion

        #region Register
        [Fact]
        public void Register_InputOk_ExpectOK()
        {
            var accountController = InitializeTest();

            var userName = "GenericName";

            var res = accountController.Register( new RegisterDTO()
            {
                Username = userName,
                Password = "GenericPassword",
                DepartmentId = DEPARTMENT_ONE
            }).Result;

            Assert.IsType<Response<GirafUserDTO>>(res);
            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            Assert.True(res.Success);
            Assert.NotNull(res.Data);
            // check data
            Assert.Equal(res.Data.Username, userName);
            Assert.Equal(res.Data.Department, DEPARTMENT_ONE);
        }
     
        [Fact]
        public void Register_ExistingUsername_BadRequest()
        {
            var accountController = InitializeTest();

            var res = accountController.Register(new RegisterDTO()
            {
                Username = _testContext.MockUsers[ADMIN_DEP_ONE].UserName,
                Password = "password",
                DepartmentId = DEPARTMENT_ONE
            }).Result;

            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
            Assert.Equal(ErrorCode.UserAlreadyExists, res.ErrorCode);
            Assert.False(res.Success);
        }

        [Fact]
        public void Register_NoUsername_BadRequest()
        {
            var accountController = InitializeTest();
            
            var res = accountController.Register(new RegisterDTO()
            {
                Password = "password",
                DepartmentId = DEPARTMENT_ONE
            }).Result;

            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
            Assert.Equal(ErrorCode.InvalidCredentials, res.ErrorCode);
            Assert.False(res.Success);
        }

        [Fact]
        // Account may exist without department
        // If user is without department, then Department=null, otherwise department = user.DepartmentKey
        public void Register_NoDepartment_OkDepKeyIsMinus1()
        {
            var accountController = InitializeTest();
            
            var res = accountController.Register(new RegisterDTO()
            {
                Username = "NewUser",
                Password = "password"
            }).Result;

            Assert.IsType<Response<GirafUserDTO>>(res);
            Assert.True(res.Success);
            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            // check data
            Assert.Equal(null, res.Data.Department);
        }
        
        [Fact]
        public void Register_BlankDTO_BadRequest()
        {
            var accountController = InitializeTest();

            var res = accountController.Register(new RegisterDTO()
            {
                Username = "",
                Password = ""
            }).Result;

            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.InvalidCredentials, res.ErrorCode);
        }

        [Fact]
        public void Register_NullDTO_BadRequest()
        {
            var accountController = InitializeTest();

            var res = accountController.Register(null).Result;

            Assert.IsType<ErrorResponse<GirafUserDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.MissingProperties, res.ErrorCode);
        }
        #endregion

        #region ForgotPassword
        [Fact]
        public void ForgotPassword_UserExist_Ok()
        {
            var accountController = InitializeTest();

            var res = accountController.ForgotPassword(new ForgotPasswordDTO()
            {
                Username = _testContext.MockUsers[ADMIN_DEP_ONE].UserName,
                Email = "unittest@giraf.cs.aau.dk"
            }).Result;

            Assert.IsType<Response>(res);
            Assert.True(res.Success);
            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
        }

        [Fact]
        // Returns Ok as we do not want anyone to know if a given user does not exist
        public void ForgotPassword_UserDoesNotExist_Ok()
        {
            var accountController = InitializeTest();

            var res = accountController.ForgotPassword(new ForgotPasswordDTO()
            {
                Username = "UserDoNotExist",
                Email = "UserDoNotExist@UserDoNotExist.com"
            }).Result;

            Assert.IsType<Response>(res);
            Assert.True(res.Success);
            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
        }

        [Fact]
        public void ForgotPassword_NoEmail_BadRequest()
        {
            var accountController = InitializeTest();

            var res = accountController.ForgotPassword(new ForgotPasswordDTO()
            {
                Username = _testContext.MockUsers[ADMIN_DEP_ONE].UserName
            }).Result;

            Assert.IsType<ErrorResponse>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.MissingProperties, res.ErrorCode);
        }

        [Fact]
        public void ForgotPassword_NullDTO_BadRequest()
        {
            var accountController = InitializeTest();

            var res = accountController.ForgotPassword(null).Result;

            Assert.IsType<ErrorResponse>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.MissingProperties, res.ErrorCode);
        }
        #endregion

        #region SetPassword
        [Fact]
        public void SetPassword_ValidInput_Ok()
        {
            var ac = InitializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            SetPasswordDTO spDTO = new SetPasswordDTO()
            {
                NewPassword = "newPassword",
            };
            
            var res = ac.SetPassword(spDTO).Result;

            Assert.IsType<Response>(res);
            Assert.True(res.Success);
            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
        }

        [Fact]
        public void SetPasswod_NullDTO_BadRequest()
        {
            var ac = InitializeTest();

            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);

            SetPasswordDTO spDTO = null;

            var res = ac.SetPassword(spDTO).Result;

            Assert.IsType<ErrorResponse>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.MissingProperties, res.ErrorCode);
        }
        #endregion

        #region ChangePassword
        [Fact]
        public void ChangePassword_ValidInput_Ok()
        {
            var ac = InitializeTest();

            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);

            ChangePasswordDTO cpDTO = new ChangePasswordDTO()
            {
                OldPassword = "password",
                NewPassword = "PASSWORD"
            };

            var res = ac.ChangePassword(cpDTO).Result;

            Assert.IsType<Response>(res);
            Assert.True(res.Success);
            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
        }

        [Fact]
        public void ChangePassword_NullDTO_BadRequest()
        {
            var ac = InitializeTest();

            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);

            ChangePasswordDTO cpDTO = null;

            var res = ac.ChangePassword(cpDTO).Result;

            Assert.IsType<ErrorResponse>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.MissingProperties, res.ErrorCode);
        }

        [Fact]
        public void ChangePassword_WrongOldPassword_BadRequest()
        {
            var ac = InitializeTest();

            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);

            ChangePasswordDTO cpDTO = new ChangePasswordDTO()
            {
                OldPassword = "drowssap",
                NewPassword = "PASSWORD",
            };

            var res = ac.ChangePassword(cpDTO).Result;

            Assert.IsType<ErrorResponse>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.PasswordNotUpdated, res.ErrorCode);
        }
        
        #endregion
    }
}