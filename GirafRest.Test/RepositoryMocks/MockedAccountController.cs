using System.Threading;
using System.Threading.Tasks;
using GirafRest.Controllers;
using GirafRest.Interfaces;
using GirafRest.IRepositories;
using GirafRest.Models;
using GirafRest.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace GirafRest.Test.RepositoryMocks
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
                    new Mock<ILoggerFactory>(),
                    new Mock<IGirafService>(),
                    configuration,
                    new Mock<IGirafUserRepository>(),
                    new Mock<IDepartmentRepository>(),
                    new Mock<IGirafRoleRepository>(),
                    new Mock<ISettingRepository>()
                )
            { }
            
            public MockedAccountController(
                Mock<SignInManager<GirafUser>> signInManager,
                Mock<ILoggerFactory> loggerFactory,
                Mock<IGirafService> giraf,
                IOptions<JwtConfig> configuration,
                Mock<IGirafUserRepository> userRepository,
                Mock<IDepartmentRepository> departmentRepository, 
                Mock<IGirafRoleRepository> girafRoleRepository,
                Mock<ISettingRepository> settingRepository
                ) 
                : base(
                    signInManager.Object, 
                    loggerFactory.Object,
                    giraf.Object, 
                    configuration,
                    userRepository.Object,
                    departmentRepository.Object,
                    girafRoleRepository.Object,
                    settingRepository.Object
                )
            {
                SignInManager = signInManager;
                LoggerFactory = loggerFactory;
                GirafService = giraf;
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
            public Mock<IGirafUserRepository> UserRepository { get; }
            public Mock<IDepartmentRepository> DepartmentRepository { get; }
            public Mock<IGirafRoleRepository> GirafRoleRepository { get; }
        }
}