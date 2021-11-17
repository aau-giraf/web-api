using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GirafRest.Controllers;
using GirafRest.Interfaces;
using GirafRest.IRepositories;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using GirafRest.Models.Enums;
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
       
        [Fact]
        public async Task GetUserRoleOK_Success()
        {
            var girafService = new MockGirafService(new MockedUserManager());
            var controller = new MockedUserController(girafService);
            var userRepository = controller.GirafUserRepository;
            GirafUser user = new GirafUser()
            {
                Id = "40",
                UserName =  "Aladdin",
                DisplayName = "display",
                DepartmentKey = 1
            };
            
            userRepository.Setup(x => x.GetUserByUsername(user.UserName)).Returns(Task.FromResult(user));
            var response =  await controller.GetUserRole(user.UserName);
            var result = response as ObjectResult;
            var body = result.Value as SuccessResponse<GirafRoles>;
            
            //Assert.NotNull(body);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
    }
    
    public class MockedUserController : UserController
        {
            public static Mock<RoleManager<GirafRole>> _roleManager = GetMockRoleManager();
            
            public MockedUserController(IGirafService giraf) : this (
                giraf,
                new Mock<ILoggerFactory>(),
                _roleManager,
                new Mock<IGirafUserRepository>(),
                new Mock<IImageRepository>(),
                new Mock<IUserResourseRepository>(),
                new Mock<IPictogramRepository>()) {}
            
            public MockedUserController(
                IGirafService giraf,
                Mock<ILoggerFactory> loggerFactory,
                Mock<RoleManager<GirafRole>> roleManager,
                Mock<IGirafUserRepository> girafUserRepository,
                Mock<IImageRepository> imageRepository,
                Mock<IUserResourseRepository> userResourseRepository,
                Mock<IPictogramRepository> pictogramRepository) : base(
                giraf,
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
                
            }
            
            public static Mock<RoleManager<GirafRole>> GetMockRoleManager()
            {
                var roleStore = new Mock<IRoleStore<GirafRole>>();
                return new Mock<RoleManager<GirafRole>>(
                    roleStore.Object,null,null,null,null);

            }

            public IGirafService Giraf { get; }
            public Mock<ILoggerFactory> LoggerFactory { get; }
            public Mock<RoleManager<GirafRole>> RoleManager { get; }
            public Mock<IGirafUserRepository> GirafUserRepository { get; }
            public Mock<IImageRepository> ImageRepository { get; }
            public Mock<IUserResourseRepository> UserResourseRepository { get; }
            public Mock<IPictogramRepository> PictogramRepository { get; }

        }
}
