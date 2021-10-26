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
using System;
using Castle.Core.Logging;
using GirafRest.Interfaces;
using GirafRest.IRepositories;
using GirafRest.Models;
using GirafRest.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;


namespace GirafRest.Test
{
    public class AccountControllerTest
    {
#pragma warning disable IDE0051 // Remove unused private members
        private TestContext _testContext= new TestContext();
        private readonly ITestOutputHelper _testLogger;

        private const int ADMIN_DEP_ONE = 0;
        private const int DEPARTMENT_ONE = 1;
        private const int GUARDIAN_DEP_TWO = 1;
        private const int ANOTHER_GUARDIAN_DEP_TWO = 5;
        private const int CITIZEN_DEP_TWO = 2;
        private const int CITIZEN_DEP_THREE = 3;
        private const int ADMIN_NO_DEP = 4;
        private const int DEPARTMENT_DEP_TWO = 6;
#pragma warning restore IDE0051 // Remove unused private members


        public AccountControllerTest(ITestOutputHelper output)
        {
            _testLogger = output;
        }
        
        private AccountController InitializeTest()
        {
            _testContext = new TestContext();


            var mockSignInManager = new MockSignInManager(_testContext.MockUserManager, _testContext);
            var mockGirafService = new MockGirafService(_testContext.MockDbContext.Object, _testContext.MockUserManager);
            var mockUserRepository = Mock.Of<IGirafUserRepository>();
            var mockDepartmentRepository = Mock.Of<IDepartmentRepository>();
            var mockGirafRoleRepository = Mock.Of<IGirafRoleRepository>();
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
                mockUserRepository, 
                mockDepartmentRepository, 
                mockGirafRoleRepository
            );

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

            var body = res.Value as SuccessResponse;

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

            var bodyB = resB.Value as SuccessResponse;

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
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status401Unauthorized, res.StatusCode);
            Assert.Equal(ErrorCode.InvalidCredentials, body.ErrorCode);                        
        }

        [Fact]
        // Trying to login with no credentials:
        public void Login_NullDTO_MissingProperties()
        {
            var accountController = InitializeTest();

            var res = accountController.Login(null).Result as ObjectResult;

            var body = res.Value as ErrorResponse;

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

            var body = res.Value as ErrorResponse;

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

            var body = res.Value as ErrorResponse;

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
            }).Result as ObjectResult;

            var body = res.Value as SuccessResponse<GirafUserDTO>;
            
            Assert.Equal(StatusCodes.Status201Created, res.StatusCode);

            // check data
            Assert.Equal(userName, body.Data.Username);
            Assert.Equal(DEPARTMENT_ONE, body.Data.Department);
            Assert.Equal(displayName, body.Data.DisplayName);
        }
        public class FakeUserManager : UserManager<GirafUser>
        {
            public FakeUserManager()
                : base(new Mock<IUserStore<GirafUser>>().Object,
                    new Mock<IOptions<IdentityOptions>>().Object,
                    new Mock<IPasswordHasher<GirafUser>>().Object,
                    new IUserValidator<GirafUser>[0],
                    new IPasswordValidator<GirafUser>[0],
                    new Mock<ILookupNormalizer>().Object,
                    new Mock<IdentityErrorDescriber>().Object,
                    new Mock<IServiceProvider>().Object,
                    new Mock<ILogger<UserManager<GirafUser>>>().Object)
            { }
        }
        
        public class FakeSignInManager : SignInManager<GirafUser>
        {
            public FakeSignInManager()
                : base(new FakeUserManager(),
                    new Mock<IHttpContextAccessor>().Object,
                    new Mock<IUserClaimsPrincipalFactory<GirafUser>>().Object,
                    new Mock<IOptions<IdentityOptions>>().Object,
                    new Mock<ILogger<SignInManager<GirafUser>>>().Object,
                    new Mock<IAuthenticationSchemeProvider>().Object,
                    new Mock<IUserConfirmation<GirafUser>>().Object)
            { }
        }

        public class MockAccountController : AccountController
        {
            public MockAccountController(SignInManager<GirafUser> signInManager) 
                : this(
                    signInManager, 
                    new Mock<ILoggerFactory>(),
                    new Mock<IGirafService>(),
                    new Mock<IOptions<JwtConfig>>(),
                    new Mock<IGirafUserRepository>(),
                    new Mock<IDepartmentRepository>(),
                    new Mock<IGirafRoleRepository>()
                )
            { }
            
            public MockAccountController(
                SignInManager<GirafUser> signInManager,
                Mock<ILoggerFactory> loggerFactory,
                Mock<IGirafService> giraf,
                Mock<IOptions<JwtConfig>> configuration,
                Mock<IGirafUserRepository> userRepository,
                Mock<IDepartmentRepository> departmentRepository, 
                Mock<IGirafRoleRepository> girafRoleRepository) 
                : base(
                    signInManager, 
                    loggerFactory.Object,
                    giraf.Object, 
                    configuration.Object,
                    userRepository.Object,
                    departmentRepository.Object,
                    girafRoleRepository.Object
                )
            {
                LoggerFactory = loggerFactory;
                GirafService = giraf;
                Configuration = configuration;
                UserRepository = userRepository;
                DepartmentRepository = departmentRepository;
                GirafRoleRepository = girafRoleRepository;
            }
            
            
            public Mock<ILoggerFactory> LoggerFactory { get; }
            public Mock<IGirafService> GirafService { get; }
            public Mock<IOptions<JwtConfig>> Configuration { get; }
            public Mock<IGirafUserRepository> UserRepository { get; }
            public Mock<IDepartmentRepository> DepartmentRepository { get; }
            public Mock<IGirafRoleRepository> GirafRoleRepository { get; }
            
        }
        [Fact]
        public void Register_ExistingUsername_UserAlreadyExists()
        {
            // Arrange
            var signInManager = new FakeSignInManager();
            var accountController = new MockAccountController(signInManager);
            var userRepository = accountController.UserRepository;
            var dto = new RegisterDTO()
            {
                Username = "Thomas",
                DisplayName = "Seje Thomas",
                Password = "password",
                DepartmentId = 1,
                Role = GirafRoles.Citizen,
            };
            
            // Mock
            userRepository.Setup(x => x.ExistsUsername(dto.Username)).Returns(true);

            // Act
            var response = accountController.Register(dto);
            var objectResult = response.Result as ObjectResult;
            var errorResponse = objectResult.Value as ErrorResponse;
            
            // Assert
            Assert.Equal(ErrorCode.UserAlreadyExists, errorResponse.ErrorCode);
        }

        [Fact]
        public void Register_NoUsername_InvalidCredentials()
        {
            // Arrange 
            var signInManager = new FakeSignInManager();
            var accountController = new MockAccountController(signInManager);
            var dto = new RegisterDTO()
            {
                DisplayName = "Seje Thomas",
                Password = "password",
                DepartmentId = 1,
                Role = GirafRoles.Citizen,
            };
            // Mock 
            
            // Act 
            var response = accountController.Register(dto);
            var objectResult = response.Result as ObjectResult;
            var errorResponse = objectResult.Value as ErrorResponse;
            // Assert
            Assert.Equal(ErrorCode.InvalidCredentials, errorResponse.ErrorCode);
        }
        
        [Fact]
        //tries to register a new account with no display name 
        public void Register_NoDisplayName_InvalidCredentials()
        {
            // Arrange
            var signInManager = new FakeSignInManager();
            var accountController = new MockAccountController(signInManager);
            var dto = new RegisterDTO()
            {
                Username = "Thomas",
                Password = "password",
                DepartmentId = 1,
                Role = GirafRoles.Citizen,
            };
            // Mock
            // Act
            var response = accountController.Register(dto);
            var objectResult = response.Result as ObjectResult;
            var errorResponse = objectResult.Value as ErrorResponse;
            // Assert
            Assert.Equal(ErrorCode.InvalidCredentials, errorResponse.ErrorCode);
        }
        
        
        [Fact]
        // Tries to register a new account with an empty displayName
        public void Register_user_empty_displayName()
        {
            // Arrange
            var signInManager = new FakeSignInManager();
            var accountController = new MockAccountController(signInManager);
            var dto = new RegisterDTO()
            {
                Username = "Thomas",
                DisplayName = "",
                Password = "password",
                DepartmentId = 1,
                Role = GirafRoles.Citizen,
            };
            // Mock
            // Act
            var response = accountController.Register(dto);
            var objectResult = response.Result as ObjectResult;
            var errorResponse = objectResult.Value as ErrorResponse;
            // Assert
            Assert.Equal(ErrorCode.InvalidCredentials, errorResponse.ErrorCode);
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
                DisplayName = "DisplayName",
                Role = GirafRoles.Citizen
            }).Result as ObjectResult;

            var body = res.Value as SuccessResponse<GirafUserDTO>;

            Assert.Equal(StatusCodes.Status201Created, res.StatusCode);
            Assert.Null(body.Data.Department);
        }
        
        [Fact]
        public void Register_BlankDTO_InvalidCredentials()
        {
            // Arrange
            var signInManager = new FakeSignInManager();
            var accountController = new MockAccountController(signInManager);
            var dto = new RegisterDTO()
            {
                Username = "",
                Password = "",
            };
            // Mock
            // Act
            var response = accountController.Register(dto);
            var objectResult = response.Result as ObjectResult;
            var errorResponse = objectResult.Value as ErrorResponse;
            
            // Assert
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
            Assert.Equal(ErrorCode.InvalidCredentials, errorResponse.ErrorCode);
        }

        [Fact]
        public void Register_GuardianRelation_Success(){
            // Arrange
            var signInManager = new FakeSignInManager();
            var accountController = new MockAccountController(signInManager);
            var userRepository = accountController.UserRepository;
            var dto = new RegisterDTO()
            {
                Username = "Thomas",
                DisplayName = "",
                Password = "password",
                DepartmentId = 1,
                Role = GirafRoles.Citizen,
            };
            var mockUser = new GirafUser();
            
            // Mock
            userRepository.Setup(x => x.GetUserByUsername(dto.Username)).Returns(mockUser);
            
            // Act
            var response = accountController.Register(dto);
            var objectResult = response.Result as ObjectResult;
            var errorResponse = objectResult.Value as ErrorResponse;
            
            // Assert
            Assert.Equal(StatusCodes.Status201Created, objectResult.StatusCode);
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
            // Arrange
            var signInManager = new FakeSignInManager();
            var accountController = new MockAccountController(signInManager);
            
            // Act
            var response = accountController.Register(null);
            var objectResult = response.Result as ObjectResult;
            var errorResponse = objectResult.Value as ErrorResponse;
            
            // Assert
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
            Assert.Equal(ErrorCode.MissingProperties, errorResponse.ErrorCode);
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

            // var res = ac.ChangePasswordByOldPassword(mockUser.Id, cpDTO).Result as ObjectResult;

            // Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
        }

        [Fact]
        public void ChangePassword_NullDTO_MissingProperties()
        {
            // Arrange
            var signInManager = new FakeSignInManager();
            var accountController = new MockAccountController(signInManager);
            var userRepository = accountController.UserRepository;
            var cpDTO = new ChangePasswordDTO();
            var userid = "Username";
            var mockuser = new GirafUser()
            {
                UserName = "Username",
                DisplayName = "UsernameDisplay",
                Id = "username22",
                DepartmentKey = 2

            };
            
            // Mock
            userRepository.Setup(x => x.Get(userid)).Returns(mockuser);
                
            // Act
            var response = accountController.ChangePasswordByOldPassword(userid, cpDTO);
            var objectResult = response.Result as ObjectResult;
            var errorResponse = objectResult.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
            Assert.Equal(ErrorCode.MissingProperties, errorResponse.ErrorCode);
        }

        [Fact]
        public void ChangePassword_WrongOldPassword_PasswordNotUpdated()
        {
            // Arrange
            var signInManager = new FakeSignInManager();
            var accountController = new MockAccountController(signInManager);
            var userRepository = accountController.UserRepository;
            var girafService = accountController.GirafService;
            var userid = "Username";
            var mockuser = new GirafUser()
            {
                UserName = "Username",
                DisplayName = "UsernameDisplay",
                Id = "username22",
                DepartmentKey = 2

            };
            ChangePasswordDTO cpDTO = new ChangePasswordDTO()
            {
                OldPassword = "drowssap",
                NewPassword = "PASSWORD",
            };

            // Mock
            userRepository.Setup(x => x.Get(userid)).Returns(mockuser);
           
            // Act
            var response = accountController.ChangePasswordByOldPassword(userid, cpDTO);
            var objectResult = response.Result as ObjectResult;
            var errorResponse = objectResult.Value as ErrorResponse;
            
            // Assert
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.Equal(ErrorCode.PasswordNotUpdated, errorResponse.ErrorCode);
        }

        #endregion

        #region DeleteUser
        [Fact]
        public void DeleteUser_NotFound_UserNotFound()
        {
            // Arrange
            var signInManager = new FakeSignInManager();
            var accountController = new MockAccountController(signInManager);
            var userid = "Username";
            
            // Act
            var response = accountController.DeleteUser(userid);
            var objectResult = response.Result as ObjectResult;
            var errorresponse = objectResult.Value as ErrorResponse;
            
            // Assert
            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
            Assert.Equal(ErrorCode.UserNotFound, errorresponse.ErrorCode);
        }

        [Fact]
        public void DeleteUser_ValidInput_Success()
        {
            // Arrange
            var signInManager = new FakeSignInManager();
            var accountController = new MockAccountController(signInManager);
            var userRepository = accountController.UserRepository;
            var userid = "Username";
            var mockUser = new GirafUser()
            {
                UserName = "Username",
                DisplayName = "UsernameDisplay",
                Id = "username22",
                DepartmentKey = 2

            };
            
            // Mock
            userRepository.Setup(x => x.Get(userid)).Returns(mockUser);
            
            
            // Act
            var response = accountController.DeleteUser(mockUser.Id);
            var objectResult = response.Result as ObjectResult;
            var succesResponse = objectResult.Value as SuccessResponse;

            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);
        }   
        #endregion

    }
}