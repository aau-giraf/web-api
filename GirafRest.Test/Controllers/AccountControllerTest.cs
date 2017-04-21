using System.Collections.Generic;
using GirafRest.Data;
using GirafRest.Models;
using GirafRest.Test.Mocks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators.Internal;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GirafRest.Test.Controllers
{
    public class AccountControllerTest
    {
        private readonly Mock<GirafDbContext> dbContextMock;
        private readonly Mock<IUserStore<GirafUser>> userStore;
        private readonly MockUserManager umMock;
        private readonly Mock<ILoggerFactory> lfMock;
        private readonly List<string> logs;
        private readonly Mock<MockDbContext> dbMock;

        public AccountControllerTest()
        {

        }

        [Fact]
        public void Login_ExpectOK()
        {

        }

        [Fact]
        public void Login_ExpectUnauthorized()
        {

        }

        [Fact]
        public void Register_ExpectOK()
        {

        }

        [Fact]
        public void Register_ExpectBadRequest()
        {

        }

        [Fact]
        public void Logout_ExpectOK()
        {

        }

        /* Lav en test hvor du logger ud
         * også selvom du ikke er logget ind
         */

        [Fact]
        public void ForgotPassword_ExpectNotFound()
        {

        }

        [Fact]
        public void ForgotPassword_ExpectOK()
        {

        }

        [Fact]
        public void ResetPassword_ExpectSomething() // hvad expecter vi?
        {

        }

        [Fact]
        public void ResetPasswordConfirmation_ExpectViewReturned()
        {

        }

        [Fact]
        public void AccessDenied_ExpectUnauthorized()
        {

        }

        [Fact]
        public void AddErrors_ExpectErrorsAdded()
        {

        }
    }
}