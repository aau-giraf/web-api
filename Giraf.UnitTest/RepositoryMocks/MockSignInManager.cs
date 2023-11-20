using GirafEntities.User;
using GirafAPI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Giraf.UnitTest.RepositoryMocks
{
    public class MockSignInManager : Mock<SignInManager<GirafUser>>
        {
            public MockSignInManager()
                : this(new MockUserManager())
            { }

            public MockSignInManager(
                Mock<UserManager<GirafUser>> userManager
            )
                : this(
                    userManager,
                    new Mock<IHttpContextAccessor>(),
                    new Mock<IUserClaimsPrincipalFactory<GirafUser>>(),
                    new Mock<IOptions<IdentityOptions>>(),
                    new Mock<ILogger<SignInManager<GirafUser>>>(),
                    new Mock<IAuthenticationSchemeProvider>(),
                    new Mock<IUserConfirmation<GirafUser>>())
            { }

            public MockSignInManager(
                    Mock<UserManager<GirafUser>> userManager,
                    Mock<IHttpContextAccessor> httpContextAccessor,
                    Mock<IUserClaimsPrincipalFactory<GirafUser>> userClaimsPrincipalFactory,
                    Mock<IOptions<IdentityOptions>> options,
                    Mock<ILogger<SignInManager<GirafUser>>> logger,
                    Mock<IAuthenticationSchemeProvider> authenticationSchemeProvider,
                    Mock<IUserConfirmation<GirafUser>> userConfirmation
                )
                : base (
                    userManager.Object,
                    httpContextAccessor.Object,
                    userClaimsPrincipalFactory.Object,
                    options.Object,
                    logger.Object,
                    authenticationSchemeProvider.Object,
                    userConfirmation.Object
                )
            {
                UserManager = userManager;
                IHttpContextAccessor = httpContextAccessor;
                IUserClaimsPrincipalFactory = userClaimsPrincipalFactory;
                IOptions = options;
                ILogger = logger;
                IAuthenticationSchemeProvider = authenticationSchemeProvider;
                IUserConfirmation = userConfirmation;
            }

            public Mock<UserManager<GirafUser>> UserManager { get; }
            public Mock<IHttpContextAccessor> IHttpContextAccessor { get; }
            public Mock<IUserClaimsPrincipalFactory<GirafUser>> IUserClaimsPrincipalFactory { get; }
            public Mock<IOptions<IdentityOptions>> IOptions { get; }
            public Mock<ILogger<SignInManager<GirafUser>>> ILogger { get; }
            public Mock<IAuthenticationSchemeProvider> IAuthenticationSchemeProvider { get; }
            public Mock<IUserConfirmation<GirafUser>> IUserConfirmation { get; }
        }
}