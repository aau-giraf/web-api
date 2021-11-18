using GirafRest.Controllers;
using GirafRest.Interfaces;
using GirafRest.IRepositories;
using GirafRest.Models;
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
        GirafService = giraf;
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
            roleStore.Object, null, null, null, null);
    }

    public Mock<IGirafService> GirafService { get; }
    public Mock<ILoggerFactory> LoggerFactory { get; }
    public Mock<RoleManager<GirafRole>> RoleManager { get; }
    public Mock<IGirafUserRepository> GirafUserRepository { get; }
    public Mock<IImageRepository> ImageRepository { get; }
    public Mock<IUserResourseRepository> UserResourseRepository { get; }
    public Mock<IPictogramRepository> PictogramRepository { get; }
}