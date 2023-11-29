using System.Threading;
using System.Threading.Tasks;
using Giraf.UnitTest.Controllers;
using GirafEntities.User;
using GirafRepositories.Interfaces;
using GirafAPI.Controllers;
using GirafEntities.Authentication;
using GirafServices.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Giraf.UnitTest.RepositoryMocks
{
    public class MockedAccountController : AccountController
        {
            public MockedAccountController()
                : this(new MockSignInManager())
            { }

            public MockedAccountController(IOptions<JwtConfig> configuration) 
                : this(
                    new MockSignInManager(), 
                    configuration
                )
            { }

            public MockedAccountController(MockSignInManager signInManager) 
                : this(
                    signInManager, 
                    new AccountControllerTest.OptionsJwtConfig(default)
                )
            { }

            public MockedAccountController(
                MockSignInManager signInManager,
                IOptions<JwtConfig> configuration)
                : this(
                    signInManager, 
                    new Mock<IUserService>(),
                    configuration,
                    new Mock<IGirafUserRepository>(),
                    new Mock<IDepartmentRepository>(),
                    new Mock<ISettingRepository>()
                )
            { }
            
            public MockedAccountController(
                Mock<SignInManager<GirafUser>> signInManager,
                Mock<IUserService> userService,
                IOptions<JwtConfig> configuration,
                Mock<IGirafUserRepository> userRepository,
                Mock<IDepartmentRepository> departmentRepository, 
                Mock<ISettingRepository> settingRepository
                ) 
                : base(
                    signInManager.Object, 
                    userService.Object, 
                    configuration,
                    userRepository.Object,
                    departmentRepository.Object,
                    settingRepository.Object
                )
            {
                SignInManager = signInManager;
                GirafService = userService;
                UserRepository = userRepository;
                DepartmentRepository = departmentRepository;
                // The following are primary mocks whcih are generic.
                //   These are added to ease the development of tests.
                var affectedRows = 1;
                userRepository.Setup(
                    service => service.SaveChangesAsync()
                ).Returns(Task.FromResult(affectedRows));
                GirafService.SetupGet(
                    service => service._userManager
                ).Returns(signInManager.Object.UserManager);
            }
            
            public Mock<SignInManager<GirafUser>> SignInManager { get; }
            public Mock<IUserService> GirafService { get; }
            public Mock<IGirafUserRepository> UserRepository { get; }
            public Mock<IDepartmentRepository> DepartmentRepository { get; }
            public Mock<IGirafRoleRepository> GirafRoleRepository { get; }
        }
}