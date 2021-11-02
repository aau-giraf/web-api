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
using System.Linq;

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
                : this(new MockUserManager())
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
                GirafService.SetupGet(
                    service => service._userManager
                ).Returns(signInManager.Object.UserManager);
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

        #region Register
        */
        [Fact]
        public void Register_CorrectModelAndConditions_ReturnsCreatedWithDto()
        {
            // Arrange
            var signInManager = new MockSignInManager();
            var accountController = new MockedAccountController(signInManager);
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
                Role = GirafRoles.SuperUser,
                DepartmentId = department.Key,
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

        [Fact]
        public void Register_ExistingUsername_CodeUserAlreadyExists()
        {
            // Arrange
            var accountController = new MockedAccountController();
            var registrationDto = new RegisterDTO()
            {
                Username = "Andreas",
                DisplayName = "Brandhoej",
                Password = "P@ssw0rd",
                Role = GirafRoles.SuperUser,
            };

            // Mock
            accountController.UserRepository.Setup(
                repo => repo.ExistsUsername(registrationDto.Username) 
            ).Returns(true);

            // Act
            var response = accountController.Register(registrationDto);
            var result = response.Result as ObjectResult;
            var body = result.Value as ErrorResponse;

            // Assert
            Assert.Equal(ErrorCode.UserAlreadyExists, body.ErrorCode);
        }

        [Fact]
        public void Register_EmptyUsername_CodeInvalidCredentials()
        {
            // Arrange
            var accountController = new MockedAccountController();
            var registrationDto = new RegisterDTO()
            {
                DisplayName = "Brandhoej",
                Password = "P@ssw0rd",
                Role = GirafRoles.SuperUser,
            };

            // Act
            var response = accountController.Register(registrationDto);
            var result = response.Result as ObjectResult;
            var body = result.Value as ErrorResponse;

            // Assert
            Assert.Equal(ErrorCode.InvalidCredentials, body.ErrorCode);
        }
        
        [Fact]
        public void Register_EmptyPassword_CodeInvalidCredenials()
        {
            // Arrange
            var accountController = new MockedAccountController();
            var registrationDto = new RegisterDTO()
            {
                Username = "Andreas",
                DisplayName = "Brandhoej",
                Role = GirafRoles.SuperUser,
            };

            // Act
            var response = accountController.Register(registrationDto);
            var result = response.Result as ObjectResult;
            var body = result.Value as ErrorResponse;

            // Assert
            Assert.Equal(ErrorCode.InvalidCredentials, body.ErrorCode);
        }

        [Fact]
        //tries to register a new account with no display name 
        public void Register_EmptyDisplayname_CodeInvalidCredentials()
        {
            // Arrange
            var accountController = new MockedAccountController();
            var registrationDto = new RegisterDTO()
            {
                Username = "Andreas",
                Password = "P@ssw0rd",
                Role = GirafRoles.SuperUser,
            };

            // Act
            var response = accountController.Register(registrationDto);
            var result = response.Result as ObjectResult;
            var body = result.Value as ErrorResponse;

            // Assert
            Assert.Equal(ErrorCode.InvalidCredentials, body.ErrorCode);
        }

        [Fact]
        public void Register_GuardianRelation_Success()
        {
            // Arrange
            var signInManager = new MockSignInManager();
            var accountController = new MockedAccountController(signInManager);
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
                Role = GirafRoles.Citizen,
                DepartmentId = department.Key,
            };
            var creationResult = IdentityResult.Success;
            var roleResult = IdentityResult.Success;
            var roleAsString = "Citizen";
            var request = new Mock<HttpRequest>();
            var guardianIds = new List<string>()
            {
                "GuradianId1"
            };
            var guardian = new GirafUser("Emil", "Guardian Of The galaxy", department, GirafRoles.Guardian);
            var guardians = new List<GirafUser>()
            {
                guardian
            };
            IList<GirafUser> capturedNewUsers = new List<GirafUser>();

            // Mock
            accountController.UserRepository.Setup(
                repo => repo.ExistsUsername(registrationDto.Username)
            ).Returns(false);
            accountController.DepartmentRepository.Setup(
                repo => repo.GetDepartmentById(department.Key)
            ).Returns(department);
            signInManager.UserManager.Setup(
                manager => manager.CreateAsync(Capture.In(capturedNewUsers), It.IsAny<string>())
            ).Returns(Task.FromResult(creationResult));
            signInManager.UserManager.Setup(
                manager => manager.AddToRoleAsync(It.IsAny<GirafUser>(), roleAsString)
            ).Returns(Task.FromResult(roleResult));
            signInManager.Setup(
                manager => manager.SignInAsync(It.IsAny<GirafUser>(), true, null)
            ).Returns(Task.CompletedTask);
            accountController.GirafRoleRepository.Setup(
                repo => repo.GetAllGuardians()
            ).Returns(guardianIds);
            accountController.UserRepository.Setup(
                repo => repo.GetUsersInDepartment(department.Key, guardianIds)
            ).Returns(guardians);

            // Arrange
            var response = accountController.Register(registrationDto);
            var result = response.Result as ObjectResult;
            var body = result.Value as SuccessResponse<GirafUserDTO>;
            var newCitizen = capturedNewUsers[0];

            // Assert
            Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
            Assert.Equal(1, capturedNewUsers.Count);
            Assert.Equal(1, newCitizen.Guardians.Count);
            Assert.Equal(guardian, newCitizen.Guardians.First().Guardian);
        }

        [Fact]
        public void Register_NullDTO_MissingProperties()
        {
            // Arrange
            var signInManager = new MockSignInManager();
            var accountController = new MockedAccountController(signInManager);

            // Act
            var response = accountController.Register(null);
            var result = response.Result as ObjectResult;
            var errorResponse = result.Value as ErrorResponse;

            // Assert
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal(ErrorCode.MissingProperties, errorResponse.ErrorCode);
        }

        [Fact]
        public void ChangePassword_ValidInput_Success()
        {
            // Arrange
            var signInManager = new MockSignInManager();
            var accountController = new MockedAccountController(signInManager);
            var userId = "I identitfy as this ID because it is IDeal";
            var user  = new GirafUser()
            {
                UserName = "Johnny Lawrence",
                DisplayName = "Cobra Kai dojo"
            };
            var dto = new ChangePasswordDTO()
            {
                OldPassword = "password",
                NewPassword = "P@ssw0rd"
            };

            // Mock
            accountController.UserRepository.Setup(
                repo => repo.Get(userId)
            ).Returns(user);
            signInManager.UserManager.Setup(
                manager => manager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword)
            ).Returns(Task.FromResult(IdentityResult.Success));

            // Act
            var response = accountController.ChangePasswordByOldPassword(userId, dto);
            var result = response.Result as ObjectResult;

            // Assert
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }


        [Fact]
        public void ChangePassword_NullDTO_MissingProperties()
        {
            // Arrange
            var signInManager = new MockSignInManager();
            var accountController = new MockedAccountController(signInManager);
            var userId = "I identitfy as this ID because it is IDeal";
            var user  = new GirafUser()
            {
                UserName = "Johnny Lawrence",
                DisplayName = "Cobra Kai dojo"
            };
            var dto = new ChangePasswordDTO();

            // Mock
            accountController.UserRepository.Setup(
                repo => repo.Get(userId)
            ).Returns(user);

            // Act
            var response = accountController.ChangePasswordByOldPassword(userId, dto);
            var result = response.Result as ObjectResult;

            // Assert
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }

        [Fact]
        public void ChangePassword_WrongOldPassword_PasswordNotUpdated()
        {
            // Arrange
            var signInManager = new MockSignInManager();
            var accountController = new MockedAccountController(signInManager);
            var userId = "I identitfy as this ID because it is IDeal";
            var user  = new GirafUser()
            {
                UserName = "Johnny Lawrence",
                DisplayName = "Cobra Kai dojo"
            };
            var dto = new ChangePasswordDTO()
            {
                OldPassword = "password",
                NewPassword = "P@ssw0rd"
            };

            // Mock
            accountController.UserRepository.Setup(
                repo => repo.Get(userId)
            ).Returns(user);
            signInManager.UserManager.Setup(
                manager => manager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword)
            ).Returns(Task.FromResult(IdentityResult.Failed()));

            // Act
            var response = accountController.ChangePasswordByOldPassword(userId, dto);
            var result = response.Result as ObjectResult;

            // Assert
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
        }

        [Fact]
        public void DeleteUser_NotFound_UserNotFound()
        {
            // Arrange
            var signInManager = new MockSignInManager();
            var accountController = new MockedAccountController(signInManager);
            var userId = "I identitfy as this ID because it is IDeal";
            var user  = new GirafUser()
            {
                UserName = "Johnny Lawrence",
                DisplayName = "Cobra Kai dojo"
            };

            // Mock
            accountController.UserRepository.Setup(
                repo => repo.GetUserByID(userId)
            ).Returns((GirafUser)default);

            // Act
            var response = accountController.DeleteUser(userId);
            var result = response.Result as ObjectResult;
            var error = result.Value as ErrorResponse;

            // Assert
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
            Assert.Equal(ErrorCode.UserNotFound, error.ErrorCode);
        }

        [Fact]
        public void DeleteUser_ValidInput_Success()
        {
            // Arrange
            var signInManager = new MockSignInManager();
            var accountController = new MockedAccountController(signInManager);
            var userId = "I identitfy as this ID because it is IDeal";
            var user  = new GirafUser()
            {
                UserName = "Johnny Lawrence",
                DisplayName = "Cobra Kai dojo"
            };

            // Mock
            accountController.UserRepository.Setup(
                repo => repo.GetUserByID(userId)
            ).Returns(user);
            accountController.UserRepository.Setup(
                repo => repo.Remove(user)
            );

            // Act
            var response = accountController.DeleteUser(userId);
            var result = response.Result as ObjectResult;

            // Assert
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
    }
}
