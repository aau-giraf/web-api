using GirafRest.Controllers;
using GirafRest.Models.DTOs.AccountDTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using GirafRest.Services;
using GirafRest.Models.DTOs;
using GirafRest.Models.Responses;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GirafRest.Interfaces;
using GirafRest.IRepositories;
using GirafRest.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;
using System.Threading;

namespace GirafRest.Test
{
    public class AccountControllerTest
    {
        public class MockUserManager : Mock<UserManager<GirafUser>>
        {
            public MockUserManager()
                : this(new Mock<IUserStore<GirafUser>>())
            { }

            public MockUserManager(Mock<IUserStore<GirafUser>> userStore)
                : this(
                    userStore,
                    new Mock<IOptions<IdentityOptions>>(),
                    new Mock<IPasswordHasher<GirafUser>>(),
                    new Mock<IEnumerable<IUserValidator<GirafUser>>>(),
                    new Mock<IEnumerable<IPasswordValidator<GirafUser>>>(),
                    new Mock<ILookupNormalizer>(),
                    new Mock<IdentityErrorDescriber>(),
                    new Mock<IServiceProvider>(),
                    new Mock<ILogger<UserManager<GirafUser>>>()
                )
            { }

            public MockUserManager(
                    Mock<IUserStore<GirafUser>> userStore,
                    Mock<IOptions<IdentityOptions>> options,
                    Mock<IPasswordHasher<GirafUser>> passwordHasher,
                    Mock<IEnumerable<IUserValidator<GirafUser>>> userValidators,
                    Mock<IEnumerable<IPasswordValidator<GirafUser>>> passwordValidators,
                    Mock<ILookupNormalizer> lookupNormalizers,
                    Mock<IdentityErrorDescriber> identityErrorDescriber,
                    Mock<IServiceProvider> serviceProvider,
                    Mock<ILogger<UserManager<GirafUser>>> logger)
                : base(
                    userStore.Object,
                    options.Object,
                    passwordHasher.Object,
                    null,
                    null,
                    lookupNormalizers.Object,
                    identityErrorDescriber.Object,
                    serviceProvider.Object,
                    logger.Object
                )
            {
                UserStore = userStore;
                Options = options;
                PasswordHasher = passwordHasher;
                UserValidators = userValidators;
                PasswordValidators = passwordValidators;
                LookupNormalizers = lookupNormalizers;
                IdentityErrorDescriber = identityErrorDescriber;
                ServiceProvider = serviceProvider;
                Logger = logger;
            }

            public Mock<IUserStore<GirafUser>> UserStore { get; }
            public Mock<IOptions<IdentityOptions>> Options { get; }
            public Mock<IPasswordHasher<GirafUser>> PasswordHasher { get; }
            public Mock<IEnumerable<IUserValidator<GirafUser>>> UserValidators { get; }
            public Mock<IEnumerable<IPasswordValidator<GirafUser>>> PasswordValidators { get; }
            public Mock<ILookupNormalizer> LookupNormalizers { get; } 
            public Mock<IdentityErrorDescriber> IdentityErrorDescriber { get; }
            public Mock<IServiceProvider> ServiceProvider { get; }
            public Mock<ILogger<UserManager<GirafUser>>> Logger { get; }
        }
        
        public class MockSignInManager : Mock<SignInManager<GirafUser>>
        {
            public MockSignInManager()
                : this(new Mock<UserManager<GirafUser>>())
            { }

            public MockSignInManager(
                Mock<UserManager<GirafUser>> userManager
            )
                : this(
                    userManager,
                    new Mock<IHttpContextAccessor>(),
                    new Mock<IUserClaimsPrincipalFactory<GirafUser>>(),
                    new Mock<IOptions<IdentityOptions>>(),
                    new Mock<ILogger<SignInManager<GirafUser>>>(),
                    new Mock<IAuthenticationSchemeProvider>(),
                    new Mock<IUserConfirmation<GirafUser>>())
            { }

            public MockSignInManager(
                    Mock<UserManager<GirafUser>> userManager,
                    Mock<IHttpContextAccessor> httpContextAccessor,
                    Mock<IUserClaimsPrincipalFactory<GirafUser>> userClaimsPrincipalFactory,
                    Mock<IOptions<IdentityOptions>> options,
                    Mock<ILogger<SignInManager<GirafUser>>> logger,
                    Mock<IAuthenticationSchemeProvider> authenticationSchemeProvider,
                    Mock<IUserConfirmation<GirafUser>> userConfirmation
                )
                : base (
                    userManager.Object,
                    httpContextAccessor.Object,
                    userClaimsPrincipalFactory.Object,
                    options.Object,
                    logger.Object,
                    authenticationSchemeProvider.Object,
                    userConfirmation.Object
                )
            {
                UserManager = userManager;
                IHttpContextAccessor = httpContextAccessor;
                IUserClaimsPrincipalFactory = userClaimsPrincipalFactory;
                IOptions = options;
                ILogger = logger;
                IAuthenticationSchemeProvider = authenticationSchemeProvider;
                IUserConfirmation = userConfirmation;
            }

            public Mock<UserManager<GirafUser>> UserManager { get; }
            public Mock<IHttpContextAccessor> IHttpContextAccessor { get; }
            public Mock<IUserClaimsPrincipalFactory<GirafUser>> IUserClaimsPrincipalFactory { get; }
            public Mock<IOptions<IdentityOptions>> IOptions { get; }
            public Mock<ILogger<SignInManager<GirafUser>>> ILogger { get; }
            public Mock<IAuthenticationSchemeProvider> IAuthenticationSchemeProvider { get; }
            public Mock<IUserConfirmation<GirafUser>> IUserConfirmation { get; }
        }

        public class MockedAccountController : AccountController
        {
            public MockedAccountController()
                : this(new MockSignInManager())
            { }

            public MockedAccountController(MockSignInManager signInManager) 
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
            
            public MockedAccountController(
                Mock<SignInManager<GirafUser>> signInManager,
                Mock<ILoggerFactory> loggerFactory,
                Mock<IGirafService> giraf,
                Mock<IOptions<JwtConfig>> configuration,
                Mock<IGirafUserRepository> userRepository,
                Mock<IDepartmentRepository> departmentRepository, 
                Mock<IGirafRoleRepository> girafRoleRepository
                ) 
                : base(
                    signInManager.Object, 
                    loggerFactory.Object,
                    giraf.Object, 
                    configuration.Object,
                    userRepository.Object,
                    departmentRepository.Object,
                    girafRoleRepository.Object
                )
            {
                SignInManager = signInManager;
                LoggerFactory = loggerFactory;
                GirafService = giraf;
                Configuration = configuration;
                UserRepository = userRepository;
                DepartmentRepository = departmentRepository;
                GirafRoleRepository = girafRoleRepository;

                // The following are primary mocks whcih are generic.
                //   These are added to ease the development of tests.
                var affectedRows = 1;
                GirafService.Setup(
                    service => service._context.SaveChangesAsync(It.IsAny<CancellationToken>())
                ).Returns(Task.FromResult(affectedRows));
            }
            
            public Mock<SignInManager<GirafUser>> SignInManager { get; }
            public Mock<ILoggerFactory> LoggerFactory { get; }
            public Mock<IGirafService> GirafService { get; }
            public Mock<IOptions<JwtConfig>> Configuration { get; }
            public Mock<IGirafUserRepository> UserRepository { get; }
            public Mock<IDepartmentRepository> DepartmentRepository { get; }
            public Mock<IGirafRoleRepository> GirafRoleRepository { get; }
        }

        /*
        #region Login
        // When logging in, one is only allowed to login as users below them in the hierarchy. The hierarchy in order is: Admin, Department, Guardian, Citizen
        // Check if possible to login with mock credentials. All passwords are initialised (Data folder, DBInitializer.cs) to be "password"

        [Fact]
        public void Login_CredentialsOk_Success()
        {
            // Arrange
            var signInManager = new FakeSignInManager();
            var accountController = new MockAccountController(signInManager);
            var userRepository = accountController.UserRepository;
            var dto = new LoginDTO()
            {
                Username = "Thomas",
                Password = "password"
            };
            var mockuser = new GirafUser()
            {
                UserName = "Thomas",
                DisplayName = "Thomas",
                Id = "Thomas22",
                DepartmentKey = 1
            };
            
            // Mock
            userRepository.Setup(x => x.ExistsUsername(dto.Username)).Returns(false);
            userRepository.Setup(x => x.GetUserByUsername(dto.Username)).Returns(mockuser);
            // Act
            var response = accountController.Login(dto);
            var objectResult = response.Result as ObjectResult;
            var succesResponse = objectResult.Value as SuccessResponse;
            
            // Assert
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);

            // Check that jwt token is not null and atleast contains 40 characters
            Assert.NotNull(succesResponse.Data);
            Assert.True(succesResponse.Data.Length >= 40);
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
        */

        #region Register
        [Fact]
        public void Register_InputOk_Success()
        {
            // Arrange
            var userManager = new MockUserManager();
            var signInManager = new MockSignInManager(userManager);
            var accountController = new MockedAccountController(
                signInManager
            );
            var department = new Department()
            {
                Key = 1,
                Name = "SomewhereOverTheRainbow",
                // For some reason the empty constructor of "Department"
                //   initializes all collection/enumerables with a List
                //   but not "WeekTemplates", for this reason to ensure
                //   no weird braking changes, i manually initialize it.
                WeekTemplates = new List<WeekTemplate>()
            };
            var registrationDto = new RegisterDTO() 
            {
                Username = "Andreas",
                DisplayName = "Brandhoej",
                Password = "P@ssw0rd",
                DepartmentId = department.Key,
                Role = GirafRoles.SuperUser,
            };
            var creationResult = IdentityResult.Success;
            var roleResult = IdentityResult.Success;
            var roleAsString = "SuperUser";
            var request = new Mock<HttpRequest>();

            // Mock
            accountController.UserRepository.Setup(
                repo => repo.ExistsUsername(registrationDto.Username)
            ).Returns(false);
            accountController.DepartmentRepository.Setup(
                repo => repo.GetDepartmentById(department.Key)
            ).Returns(department);
            signInManager.UserManager.Setup(
                manager => manager.CreateAsync(It.IsAny<GirafUser>(), It.IsAny<string>())
            ).Returns(Task.FromResult(creationResult));
            signInManager.UserManager.Setup(
                manager => manager.AddToRoleAsync(It.IsAny<GirafUser>(), roleAsString)
            ).Returns(Task.FromResult(roleResult));
            signInManager.Setup(
                manager => manager.SignInAsync(It.IsAny<GirafUser>(), true, null)
            ).Returns(Task.CompletedTask);

            // Arrange
            var response = accountController.Register(registrationDto);
            var result = response.Result as ObjectResult;
            var body = result.Value as SuccessResponse<GirafUserDTO>;

            // Assert
            Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
            Assert.Equal(registrationDto.Username, body.Data.Username);
            Assert.Equal(registrationDto.DepartmentId, body.Data.Department);
            Assert.Equal(registrationDto.DisplayName, body.Data.DisplayName);
        }
        /*
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
            // Arrange
            var signInManager = new FakeSignInManager();
            var accountController = new MockAccountController(signInManager);
            var userManager = new Mock<UserManager<GirafUser>>();
            var departmentRepository = accountController.DepartmentRepository;
            var userRepository = accountController.UserRepository;
            var dto = new RegisterDTO()
            {
                Username = "Thomas",
                DisplayName = "Thomas",
                Password = "password",
                DepartmentId = 1,
                Role = GirafRoles.Citizen,
            };
            var mockUser1 = new GirafUser()
            {
                UserName = "Thomas",
                DisplayName = "Thomas",
                Id = "Thomas22",
                DepartmentKey = 1

            };
            var dep = new Department()
            {
                Key = 1,
                Name = "Mock Department1",
                Members = new List<GirafUser>()
                {
                    mockUser1,
                }
            };
            var identityResult = IdentityResult.Success;
            
            // Mock
            userRepository.Setup(x => x.ExistsUsername(dto.Username)).Returns(false);
            departmentRepository.Setup(x => x.GetDepartmentById((long) dto.DepartmentId)).Returns(dep);
            userManager.Setup(x => x.CreateAsync(It.IsAny<GirafUser>(), It.IsAny<string>()))
                .Returns(Task.FromResult(identityResult));
             
            
            // Act
            var response = accountController.Register(dto);
            var objectResult = response.Result as ObjectResult;
            var succesResponse = objectResult.Value as SuccessResponse<GirafUserDTO>;
            
            // Assert
            Assert.Equal(StatusCodes.Status201Created, objectResult.StatusCode);
            Assert.Null(succesResponse.Data.Department);
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
            var dto1 = new RegisterDTO()
            {
                Username = "Thomas",
                DisplayName = "Thomas",
                Password = "password",
                DepartmentId = 1,
                Role = GirafRoles.Citizen,
            };
            var dto2 = new RegisterDTO()
            {
                Username = "Daniel",
                DisplayName = "Daniel",
                Password = "password",
                DepartmentId = 1,
                Role = GirafRoles.Citizen,
            };
            var mockUser1 = new GirafUser()
            {
                UserName = "Thomas",
                DisplayName = "Thomas",
                Id = "Thomas22",
                DepartmentKey = 1

            };
            var mockUser2 = new GirafUser()
            {
                UserName = "Daniel",
                DisplayName = "Daniel",
                Id = "Daniel23",
                DepartmentKey = 2

            };
            
            // Mock
            userRepository.Setup(x => x.GetUserByUsername(dto1.Username)).Returns(mockUser1);
            
            // Act
            var response = accountController.Register(dto1);
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
            userRepository.Setup(x => x.GetUserByID(mockUser.Id)).Returns(mockUser);
            
            
            // Act
            var response = accountController.DeleteUser(mockUser.Id);
            var objectResult = response.Result as ObjectResult;
            var succesResponse = objectResult.Value as SuccessResponse;

            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);
        }   
        */
        #endregion

    }
}