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
using System.Security.Claims;
using Xunit;
using GirafRest.Extensions;

namespace GirafRest.Test
{
    public class UserControllerTest
    {
        [Fact]
        public async Task GetUserSuccessTest()
        {
            var controller = new MockedUserController();
            var principal = new ClaimsPrincipal();
            GirafUser user = new GirafUser("bob", "Bob", new Department(), GirafRoles.Citizen);
            controller.GirafService
                      .Setup(service => service._userManager.GetUserAsync(principal))
                      .ReturnsAsync(user);
            controller.RoleManager
                      .Setup(manager => manager.findUserRole(controller.GirafService.Object._userManager, user))
                      .ReturnsAsync(GirafRoles.Citizen);

            var response = await controller.GetUser(user.UserName);
            var result = response as ObjectResult;
            var body = result.Value as SuccessResponse<GirafRoles>;

            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async Task GetUserRoleOK_Success()
        {
            var controller = new MockedUserController();
            var userRepository = controller.GirafUserRepository;
            GirafUser user = new GirafUser()
            {
                Id = "40",
                UserName = "Aladdin",
                DisplayName = "display",
                DepartmentKey = 1
            };

            userRepository.Setup(x => x.GetUserByUsername(user.UserName)).Returns(Task.FromResult(user));
            var response = await controller.GetUserRole(user.UserName);
            var result = response as ObjectResult;
            var body = result.Value as SuccessResponse<GirafRoles>;

            //Assert.NotNull(body);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
    }
}
