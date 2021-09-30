using System;
using System.Collections.Generic;
using GirafRest.Controllers;
using System.Text;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using GirafRest.Extensions;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using GirafRest.Models.Responses;
using GirafRest.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.InMemory;
using GirafRest.Data;
using GirafRest.Test.Mocks;


namespace GirafRest.Test.Controllers
{
    public class UserControllerTest2021
    {

        public readonly DbContextOptions<GirafDbContext> options;
        public readonly Mock<ILoggerFactory> loggerFactory;
        public readonly Mock<MockRoleManager> roleManager;
        private readonly MockGirafService girafService;
        private readonly GirafAuthenticationService authenticationService;
        private readonly UnitTestExtensions.TestContext testContext;

        public UserControllerTest2021()
        {
            options = new DbContextOptionsBuilder<GirafDbContext>()
                .UseInMemoryDatabase(databaseName: "GirafDatabase").Options;
            var mocklogger = new Mock<ILogger>();
            loggerFactory = new Mock<ILoggerFactory>();
            loggerFactory.Setup(lf => lf.CreateLogger(It.IsAny<string>())).Returns(mocklogger.Object);

            var rolestore = new Mock<IRoleStore<GirafRole>>();
            var mockRoles = new List<GirafRole>()
                        {
                            new GirafRole(GirafRole.SuperUser) {
                                Id = GirafRole.SuperUser
                            },
                            new GirafRole(GirafRole.Guardian) {
                                Id = GirafRole.Guardian
                            },
                            new GirafRole(GirafRole.Citizen)
                            {
                                Id = GirafRole.Citizen
                            },
                            new GirafRole(GirafRole.Department)
                            {
                                Id = GirafRole.Department
                            }
                        };

            roleManager = new Mock<MockRoleManager>(rolestore.Object);
            roleManager.Setup(m => m.Roles).Returns(mockRoles.AsQueryable());

            var userStore = new Mock<IUserStore<GirafUser>>();
            MockUserManager mockUserManager = new MockUserManager(userStore.Object, testContext); 
            girafService = new MockGirafService(new GirafDbContext(options), mockUserManager);

            authenticationService = new GirafAuthenticationService(new GirafDbContext(options), roleManager.Object, mockUserManager);
        }
        
        [Fact]
        public async Task GetSortedCitizens()
        {
            //Arrange
            string expectedItem = "ANNA";
            int expectedStatus = 200;
            using (var context = new GirafDbContext(options))
            {
                context.Users.Add(new GirafUser("kenneth", "KENNETH", new Department(), GirafRoles.Citizen));
                context.Users.Add(new GirafUser("christoffer", "CHRIS", new Department(), GirafRoles.Citizen));
                context.Users.Add(new GirafUser("anna", "ANNA", new Department(), GirafRoles.Citizen));
                context.SaveChanges();
            }
            //Act
            using(var context = new GirafDbContext(options))
            {
              
                UserController userController = 
                    new UserController(girafService, loggerFactory.Object, roleManager.Object, authenticationService);

                var response = await userController.GetCitizens("12");
                OkObjectResult okObjectResult = response as OkObjectResult;
                List<DisplayNameDTO> actualItems = (List<DisplayNameDTO>)okObjectResult.Value;
                var actualStatus = okObjectResult.StatusCode;
                //Assert
                Console.WriteLine(actualItems);
                Assert.Equal(expectedStatus, actualStatus);
                Assert.Equal(expectedItem, actualItems[0].DisplayName);            
            }

        }

    }
}
