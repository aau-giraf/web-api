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
        private readonly ITestOutputHelper _outputHelpter;
        private TestContext _testContext;

        public AccountControllerTest(ITestOutputHelper outputHelpter)
        {
            _outputHelpter = outputHelpter;
        }

        private AccountController initializeTest()
        {
            _testContext = new TestContext();

            AccountController ac = new AccountController(_testContext.MockUserManager, null, null, null, _testContext.MockLoggerFactory.Object);
            _testContext.MockHttpContext = ac.MockHttpContext();
            return ac;
        }

        [Fact]
        public void Login_CredentialsOk_ExpectOK()
        {
            var accountController = initializeTest();

            // Indsæt korrekt username og password
            var res = accountController.Login(new LoginViewModel() { Username =  "", Password = ""});
            Assert.IsType<OkResult>(res.Result);
        }

        [Fact]
        public void Login_CredentialsNotOk_ExpectUnauthorized()
        {
            var accountController = initializeTest();

            // Indsæt forkert username og password
            var res = accountController.Login(new LoginViewModel() { Username = "", Password = "" });
            Assert.IsType<UnauthorizedResult>(res.Result);
        }

        [Fact]
        public void Register_InputOk_ExpectOK()
        {
            var accountController = initializeTest();

            var res = accountController.Register( new RegisterViewModel()
            { Username = "Kurt", Password = "123", ConfirmPassword = "123", DepartmentId = 0});
            Assert.IsType<OkResult>(res.Result);
        }

        [Fact]
        public void Register_InputExist_ExpectBadRequest()
        {
            var accountController = initializeTest();

            var res = accountController.Register(new RegisterViewModel()
            { Username = "Mock User", Password = "123", ConfirmPassword = "123", DepartmentId = 0 });
            Assert.IsType<BadRequestResult>(res.Result);
        }

        [Fact]
        public void Logout_UserLoggedIn_ExpectOK()
        {
            var accountController = initializeTest();

            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            var res = accountController.Logout();
            Assert.IsType<OkResult>(res.Result);
        }

        [Fact]
        public void Logout_UserNotLoggedIn_ExpectBadRequest()
        {
            var accountController = initializeTest();

            _testContext.MockUserManager.MockLogout();
            var res = accountController.Logout();
            Assert.IsType<BadRequestResult>(res.Result);
        }

        [Fact]
        public void ForgotPassword_UserExist_ExpectOk()
        {
            var accountController = initializeTest();

            // Indsæt email
            var res = accountController.ForgotPassword(new ForgotPasswordViewModel() { Username = "Mock User", Email = "" });
            Assert.IsType<OkResult>(res.Result);
        }

        [Fact]
        public void ForgotPassword_UserDoNotExist_ExpectNotFound()
        {
            var accountController = initializeTest();

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