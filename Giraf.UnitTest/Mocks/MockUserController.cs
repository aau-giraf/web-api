using System.Collections.Generic;
using GirafEntities.User;
using GirafRepositories.Interfaces;
using GirafAPI.Controllers;
using GirafServices.Authentication;
using GirafServices.User;
using GirafServices.WeekPlanner;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

public class MockedUserController : UserController
{
    public MockedUserController() : this(
        new Mock<IUserService>(),
        GetMockRoleManager(),
        new Mock<IGirafUserRepository>(),
        new Mock<IUserResourseRepository>(),
        new Mock<IPictogramRepository>(),
        new Mock<IAuthenticationService>(),
        new Mock<IImageService>())
    { }

    public MockedUserController(
        Mock<IUserService> userService,
        Mock<RoleManager<GirafRole>> roleManager,
        Mock<IGirafUserRepository> girafUserRepository,
        Mock<IUserResourseRepository> userResourseRepository,
        Mock<IPictogramRepository> pictogramRepository,
        Mock<IAuthenticationService> authenticationService,
        Mock<IImageService> imageService) : base(
        userService.Object,
        roleManager.Object,
        girafUserRepository.Object,
        userResourseRepository.Object,
        pictogramRepository.Object,
        authenticationService.Object,
        imageService.Object)
    {
        var userStoreMock = new Mock<IUserStore<GirafUser>>();
        var userManagerMock = new Mock<UserManager<GirafUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);
        
        GirafService = userService;
        RoleManager = roleManager;
        GirafUserRepository = girafUserRepository;
        ImageService = imageService;
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

    public Mock<IUserService> GirafService { get; }
    public Mock<ILoggerFactory> LoggerFactory { get; }
    public Mock<RoleManager<GirafRole>> RoleManager { get; }
    public Mock<IGirafUserRepository> GirafUserRepository { get; }
    public Mock<IImageService> ImageService { get; }
    public Mock<IUserResourseRepository> UserResourseRepository { get; }
    public Mock<IPictogramRepository> PictogramRepository { get; }
    
    public GirafUser testUser { get; }
    
    public GirafUser guardianUser { get; }
    
}