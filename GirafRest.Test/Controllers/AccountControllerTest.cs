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
using GirafRest.Models.Enums;
using GirafRest.Test.RepositoryMocks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace GirafRest.Test
{
    public class AccountControllerTest
    {
        public class OptionsJwtConfig : IOptions<JwtConfig>
        {
            public JwtConfig Value { get; }

            public OptionsJwtConfig(JwtConfig config)
            {
                Value = config;
            }
        }
        
        #region Login

        // When logging in, one is only allowed to login as users below them in the hierarchy. The hierarchy in order is: Admin, Department, Guardian, Citizen
        // Check if possible to login with mock credentials. All passwords are initialised (Data folder, DBInitializer.cs) to be "password"

        [Fact]
       public void Login_CredentialsOk_Success()
        {
            // Arrange
            var userManager = new MockUserManager();
            var signInManager = new MockSignInManager(userManager);
            var jwtOptions = new OptionsJwtConfig(new JwtConfig() {
                JwtKey = "SuperSuperSuperSUperSuperSUperSuperSecretKey",
                JwtIssuer = "TestIssuer",
                JwtExpireDays = 1,
            });
            var accountController = new MockedAccountController(signInManager,jwtOptions);
            var userRepository = accountController.UserRepository;
            var dto = new LoginDTO()
            {
                Username = "Thomas",
                Password = "password"
            };
            var mockUser = new GirafUser()
            {
                UserName = "Thomas",
                DisplayName = "Thomas",
                Id = "Thomas22",
                DepartmentKey = 1
            };
            var signInResult = SignInResult.Success;
            Task.FromResult(signInResult);
            var userRoles = new List<string>();

            // Mock
            userRepository.Setup(x => x.ExistsUsername(dto.Username)).Returns(true);
            userRepository.Setup(x => x.GetUserByUsername(dto.Username)).ReturnsAsync(mockUser);
            signInManager.Setup(x => x.PasswordSignInAsync(dto.Username, dto.Password, true, false))
                .Returns(Task.FromResult(signInResult));
            userRepository.Setup(x => x.GetUserByUsername(dto.Username)).ReturnsAsync(mockUser);
            userManager.Setup(x => x.GetRolesAsync(mockUser)).Returns(Task.FromResult<IList<string>>(userRoles));
            
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
            // Arrange
            var userManager = new MockUserManager();
            var signInManager = new MockSignInManager(userManager);
            var jwtOptions = new OptionsJwtConfig(new JwtConfig() {
                JwtKey = "SuperSuperSuperSUperSuperSUperSuperSecretKey",
                JwtIssuer = "TestIssuer",
                JwtExpireDays = 1,
            });
            var accountController = new MockedAccountController(signInManager, jwtOptions);
            var userRepository = accountController.UserRepository;
            
            var dto1 = new LoginDTO()
            {
                Username = "Dawg",
                Password = "Kenzo"
            };
            var dto2 = new LoginDTO()
            {
                Username = "Dawg",
                Password = "Kenzo"
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
                UserName = "Thomas",
                DisplayName = "Thomas",
                Id = "Thomas22",
                DepartmentKey = 1

            };
            var signInResult = SignInResult.Success;
            Task.FromResult(signInResult);
            var userRoles = new List<string>();


            // Mock
            userRepository.Setup(x => x.ExistsUsername(dto1.Username)).Returns(true);
            userRepository.Setup(x => x.ExistsUsername(dto2.Username)).Returns(true);
            signInManager.Setup(x => x.PasswordSignInAsync(dto1.Username, dto1.Password, true, false))
                .Returns(Task.FromResult(signInResult));
            signInManager.Setup(x => x.PasswordSignInAsync(dto2.Username, dto2.Password, true, false))
                .Returns(Task.FromResult(signInResult));
            userRepository.Setup(x => x.GetUserByUsername(dto1.Username)).ReturnsAsync(mockUser1);
            userRepository.Setup(x => x.GetUserByUsername(dto2.Username)).ReturnsAsync(mockUser2);
            userManager.Setup(x => x.GetRolesAsync(mockUser1)).Returns(Task.FromResult<IList<string>>(userRoles));
            userManager.Setup(x => x.GetRolesAsync(mockUser2)).Returns(Task.FromResult<IList<string>>(userRoles));

            // Act
            var response1 = accountController.Login(dto1);
            var result1 = response1.Result as ObjectResult;
            var body1 = result1.Value as SuccessResponse;

            var response2 = accountController.Login(dto1);
            var result2 = response2.Result as ObjectResult;
            var body2 = result2.Value as SuccessResponse;

            // Assert
            // Check that both requests are successful
            Assert.Equal(StatusCodes.Status200OK, result1.StatusCode);
            Assert.Equal(StatusCodes.Status200OK, result2.StatusCode);
            
            // Check that jwt token is not null and atleast contains 40 characters
            Assert.NotNull(body2.Data);
            Assert.True(body2.Data.Length >= 40);
        }

        [Fact]
        // If no user is found with given user name, return ErrorResponse with relevant ErrorCode (invalid credentials ensures we do not give the bad guys any information)
        public void Login_UsernameInvalidPasswordOk_InvalidCredentials()
        {
            // Arrange
            var userManager = new MockUserManager();
            var signInManager = new MockSignInManager(userManager);
            var accountController = new MockedAccountController(signInManager);
            var userRepository = accountController.UserRepository;
            var dto = new LoginDTO()
            {
                Username = "Dawg",
                Password = "Kenzo"
            };

            // Mock
            userRepository.Setup(x => x.ExistsUsername(dto.Username)).Returns(false);

            // Act
            var response = accountController.Login(dto);
            var result = response.Result as ObjectResult;
            var body = result.Value as ErrorResponse;

            // Assert
            Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
            Assert.Equal(ErrorCode.InvalidCredentials, body.ErrorCode);
        }

        [Fact]
        // Trying to login with no credentials:
        public void Login_NullDTO_MissingProperties()
        {
            // Arrange
            var userManager = new MockUserManager();
            var signInManager = new MockSignInManager(userManager);
            var accountController = new MockedAccountController(signInManager);

            // Act
            var response = accountController.Login(null);
            var result = response.Result as ObjectResult;
            var body = result.Value as ErrorResponse;

            // Assert
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal(ErrorCode.MissingProperties, body.ErrorCode);
        }

        [Fact]
        // Trying to login with no password:
        public void Login_NullDTO_MissingPassword()
        {
            // Arrange
            var userManager = new MockUserManager();
            var signInManager = new MockSignInManager(userManager);
            var accountController = new MockedAccountController(signInManager);
            var dto = new LoginDTO()
            {
                Username = "Dawg"
            };

            // Act
            var response = accountController.Login(dto);
            var result = response.Result as ObjectResult;
            var body = result.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
            Assert.Equal(ErrorCode.MissingProperties, body.ErrorCode);
        }

        [Fact]
        // Trying to login with no username:
        public void Login_NullDTO_MissingUsername()
        {
            // Arrange
            var userManager = new MockUserManager();
            var signInManager = new MockSignInManager(userManager);
            var accountController = new MockedAccountController(signInManager);
            var dto = new LoginDTO()
            {
                Password = "Password"
            };

            // Act
            var response = accountController.Login(dto);
            var result = response.Result as ObjectResult;
            var body = result.Value as ErrorResponse;

            // Assert
            Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
            Assert.Equal(ErrorCode.MissingProperties, body.ErrorCode);
        }

        #endregion

        #region Register
        
        [Fact]
        public void Register_CorrectModelAndConditions_Returns201_Success()
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
        #endregion register
        
        
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
                repo => repo.GetUserWithId(userId)
            ).ReturnsAsync((GirafUser)default);

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
                repo => repo.GetUserWithId(userId)
            ).ReturnsAsync(user);
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