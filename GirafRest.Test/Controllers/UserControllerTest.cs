using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GirafRest.Controllers;
using GirafRest.Interfaces;
using GirafRest.IRepositories;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using GirafRest.Models.Responses;
using GirafRest.Repositories;
using GirafRest.Services;
using GirafRest.Test.Mocks;
using GirafRest.Test.RepositoryMocks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GirafRest.Test
{
    public class UserControllerTest
    {
        private UnitTestExtensions.TestContext _testContext;
        public class MockedUserController : UserController
        {
            public MockedUserController() : this(
                new Mock<IGirafService>(),
                new Mock<ILoggerFactory>(),
                new Mock<RoleManager<GirafRole>>(),
                new Mock<IGirafUserRepository>(),
                new Mock<IImageRepository>(),
                new Mock<IUserResourseRepository>(),
                new Mock<IPictogramRepository>()){}

            public MockedUserController(
                Mock<IGirafService> giraf,
                Mock<ILoggerFactory> loggerFactory,
                Mock<RoleManager<GirafRole>> roleManager,
                Mock<IGirafUserRepository> girafUserRepository,
                Mock<IImageRepository> imageRepository,
                Mock<IUserResourseRepository> userResourseRepository,
                Mock<IPictogramRepository> pictogramRepository) : base(
                giraf.Object,
                loggerFactory.Object,
                roleManager.Object,
                girafUserRepository.Object,
                imageRepository.Object,
                userResourseRepository.Object,
                pictogramRepository.Object)
            {
                Giraf = giraf;
                LoggerFactory = loggerFactory;
                RoleManager = roleManager;
                GirafUserRepository = girafUserRepository;
                ImageRepository = imageRepository;
                UserResourseRepository = userResourseRepository;
                PictogramRepository = pictogramRepository;
                
                Giraf.Setup(
                    service => service._context.SaveChangesAsync(It.IsAny<CancellationToken>())
                ).Returns(Task.FromResult(1));
                
                // Giraf.SetupGet(
                //     service => service._userManager
                // ).Returns(signInManager.Object.UserManager);

            }

            public Mock<IGirafService> Giraf { get; }
            public Mock<ILoggerFactory> LoggerFactory { get; }
            public Mock<RoleManager<GirafRole>> RoleManager { get; }
            public Mock<IGirafUserRepository> GirafUserRepository { get; }
            public Mock<IImageRepository>  ImageRepository { get; }
            public Mock<IUserResourseRepository> UserResourseRepository { get; }
            public Mock<IPictogramRepository> PictogramRepository { get; }
            
            
            

        }
        private UserController initializeTest()
        {
            _testContext = new UnitTestExtensions.TestContext();
            
            var mockGirafService = new MockGirafService(_testContext.MockDbContext.Object, _testContext.MockUserManager);
            var mockUserRepository = Mock.Of<IGirafUserRepository>();
            var mockImageRepository = Mock.Of<IImageRepository>();
            var mockUserResourceRepository = Mock.Of<IUserResourseRepository>();
            var mockPictogramRepository = Mock.Of<IPictogramRepository>();
            
            var usercontroller = new UserController(
                mockGirafService,
                _testContext.MockLoggerFactory.Object,
                _testContext.MockRoleManager.Object,
                mockUserRepository, mockImageRepository,mockUserResourceRepository, mockPictogramRepository);

            _testContext.MockHttpContext = usercontroller.MockHttpContext();
            _testContext.MockHttpContext.MockQuery("username", null);

            

            return usercontroller;
        }

        [Fact]
        public void GetUser_Success()
        {
            //Arrange
            var controller = new MockedUserController();
            var userRepository = controller.GirafUserRepository;
            userRepository.Setup(repo => repo.Add(It.IsAny<GirafUser>()));
            var mockUser = new GirafUser()
            {
                Id = "3",
                UserName =  "PowerPuffGirls",
                DisplayName = "PPG",
                DepartmentKey = 1
                
            };
            var newUserDto = new GirafUserDTO(mockUser, GirafRoles.Citizen);
            var userManager = new MockedUserManager();
            var signInManager = new MockedSignInManager(userManager);
            var request = new Mock<HttpRequest>();
            var httpContext = new Mock<HttpContext>();
            signInManager.UserManager.Setup(x => x.GetUserAsync(httpContext.Object.User)).ReturnsAsync(mockUser);

            userRepository.Setup(x => x.GetUserWithId(mockUser.Id)).Returns(mockUser);
            //_testContext.MockUserManager.Setup(x => x.GetUserAsync(mockUser).Result());
            //_testContext.MockUserManager.MockLoginAsUser(mockUser);
            
            
            
            //Act
            //var response = controller.GetUser();
            //var objectResult = response.Result as ObjectResult;
            //var successResponse = objectResult.Value as SuccessResponse;
            var res = controller.GetUser().Result as ObjectResult;
            var body = res.Value as SuccessResponse<GirafUserDTO>;
            Task.FromResult(body);

            
            
            //Assert
            Assert.NotNull(body.Data);
            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            Assert.Equal(mockUser.UserName, body.Data.Username);
            Assert.Equal(mockUser.DepartmentKey, body.Data.Department);

        }

        [Fact]
        public void GetUserLoginSuccess()
        {
            var usercontroller = initializeTest();
            var mockUser = new GirafUser()
            {
                Id = "3",
                UserName =  "PowerPuffGirls",
                DisplayName = "PPG",
                DepartmentKey = 1
                
            };
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var res = usercontroller.GetUser(mockUser.Id).Result as ObjectResult;
            var body = res.Value as SuccessResponse<GirafUserDTO>;

            //Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            // check that we are logged in as the correct user
            Assert.Equal(mockUser.UserName, body.Data.Username);
            //Assert.Equal(mockUser.DepartmentKey, body.Data.Department);
        }
        
    }
}
