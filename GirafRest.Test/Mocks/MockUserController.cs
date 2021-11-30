using System.Collections.Generic;
using GirafRest.Controllers;
using GirafRest.Interfaces;
using GirafRest.IRepositories;
using GirafRest.Models;
using GirafRest.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

public class MockedUserController : UserController
{
    public MockedUserController() : this(
        new Mock<IGirafService>(),
        new Mock<ILoggerFactory>(),
        GetMockRoleManager(),
        new Mock<IGirafUserRepository>(),
        new Mock<IImageRepository>(),
        new Mock<IUserResourseRepository>(),
        new Mock<IPictogramRepository>())
    { }

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
        var userStoreMock = new Mock<IUserStore<GirafUser>>();
        var userManagerMock = new Mock<UserManager<GirafUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);
        
        GirafService = giraf;
        LoggerFactory = loggerFactory;
        RoleManager = roleManager;
        GirafUserRepository = girafUserRepository;
        ImageRepository = imageRepository;
        UserResourseRepository = userResourseRepository;
        PictogramRepository = pictogramRepository;
        
        testUser = new GirafUser("bob", "Bob", new Department(), GirafRoles.Citizen);
        guardianUser = new GirafUser("guard", "Guard", new Department(), GirafRoles.Guardian);
        IList<string> guardRoles = new List<string>(){"Guardian"};
        IList<string> roles = new List<string>(){"Citizen"};
        userManagerMock.Setup(manager => manager.GetRolesAsync(testUser)).ReturnsAsync(roles);
        userManagerMock.Setup(manager => manager.GetRolesAsync(guardianUser)).ReturnsAsync(guardRoles);
        GirafService.Setup(service => service._userManager).Returns(userManagerMock.Object);
        
        
    }

    public static Mock<RoleManager<GirafRole>> GetMockRoleManager()
    {
        var roleStore = new Mock<IRoleStore<GirafRole>>();
        return new Mock<RoleManager<GirafRole>>(
            roleStore.Object, null, null, null, null);
    }

    public Mock<IGirafService> GirafService { get; }
    public Mock<ILoggerFactory> LoggerFactory { get; }
    public Mock<RoleManager<GirafRole>> RoleManager { get; }
    public Mock<IGirafUserRepository> GirafUserRepository { get; }
    public Mock<IImageRepository> ImageRepository { get; }
    public Mock<IUserResourseRepository> UserResourseRepository { get; }
    public Mock<IPictogramRepository> PictogramRepository { get; }
    
    public GirafUser testUser { get; }
    
    public GirafUser guardianUser { get; }
    
}