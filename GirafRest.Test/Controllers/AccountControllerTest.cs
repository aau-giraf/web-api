using GirafRest.Controllers;
using GirafRest.Models;
using GirafRest.Models.AccountViewModels;
using GirafRest.Models.DTOs;
using GirafRest.Test.Mocks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using static GirafRest.Test.UnitTestExtensions;

namespace GirafRest.Test
{
    public class AccountControllerTest
    {
        private readonly AccountController accountController;
        private readonly List<string> logs;
        private readonly MockUserManager userManager;
        private readonly ITestOutputHelper _outputHelpter;
        private readonly Mock<MockDbContext> dbMock;

        public AccountControllerTest(ITestOutputHelper outputHelpter)
        {
            dbMock = CreateMockDbContext();

            var userStore = new Mock<IUserStore<GirafUser>>();
            userManager = MockUserManager(userStore);
            var lfMock = CreateMockLoggerFactory();

            // Skal lave alt der skal indsættes som parameter til AccountController
            accountController = new AccountController(MockUserManager(new Mock<IUserStore<GirafUser>>()), null, null, null, lfMock.Object);

            _outputHelpter = outputHelpter;
        }

        [Fact]
        public void Login_CredentialsOk_ExpectOK()
        {
            // Indsæt korrekt username og password
            var res = accountController.Login(new LoginViewModel() { Username =  "", Password = ""});
            Assert.IsType<OkResult>(res.Result);
        }

        [Fact]
        public void Login_CredentialsNotOk_ExpectUnauthorized()
        {
            // Indsæt forkert username og password
            var res = accountController.Login(new LoginViewModel() { Username = "", Password = "" });
            Assert.IsType<UnauthorizedResult>(res.Result);
        }

        [Fact]
        public void Register_InputOk_ExpectOK()
        {
            var res = accountController.Register( new RegisterViewModel()
            { Username = "Kurt", Password = "123", ConfirmPassword = "123", DepartmentId = 0});
            Assert.IsType<OkResult>(res.Result);
        }

        [Fact]
        public void Register_InputExist_ExpectBadRequest()
        {
            var res = accountController.Register(new RegisterViewModel()
            { Username = "Mock User", Password = "123", ConfirmPassword = "123", DepartmentId = 0 });
            Assert.IsType<BadRequestResult>(res.Result);
        }

        [Fact]
        public void Logout_UserLoggedIn_ExpectOK()
        {
            userManager.MockLoginAsUser(MockUsers[0]);
            var res = accountController.Logout();
            Assert.IsType<OkResult>(res.Result);
        }

        [Fact]
        public void Logout_UserNotLoggedIn_ExpectBadRequest()
        {
            userManager.MockLogout();
            var res = accountController.Logout();
            Assert.IsType<BadRequestResult>(res.Result);
        }

        [Fact]
        public void ForgotPassword_UserExist_ExpectOk()
        {
            // Indsæt email
            var res = accountController.ForgotPassword(new ForgotPasswordViewModel() { Username = "Mock User", Email = "" });
            Assert.IsType<OkResult>(res.Result);
        }

        [Fact]
        public void ForgotPassword_UserDoNotExist_ExpectNotFound()
        {
            var res = accountController.ForgotPassword(new ForgotPasswordViewModel() { Username = "No User", Email = "" });
            Assert.IsType<OkResult>(res.Result);
        }

        [Fact]
        public void ResetPassword_() // hvad skal der testes ved den?
        {
            Assert.True(false);
        }

        [Fact]
        public void ResetPasswordConfirmation_ExpectViewReturned()
        {
            Assert.True(false);
        }

        [Fact]
        public void AccessDenied_ExpectUnauthorized()
        {
            Assert.True(false);
        }
    }
}