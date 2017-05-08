using GirafRest.Controllers;using GirafRest.Models.DTOs.AccountDTOs;using GirafRest.Test.Mocks;using Microsoft.AspNetCore.Mvc;using Moq;using Xunit;using Xunit.Abstractions;using static GirafRest.Test.UnitTestExtensions;using GirafRest.Services;
using System.Threading.Tasks;
using GirafRest.Models.DTOs;
using GirafRest.Models.DTOs.UserDTOs;

namespace GirafRest.Test{    public class AccountControllerTest    {
        //NOTE: We do not test the logout method as it is merely an almost invisble abstraction on top of SignInManager.SignOut.

        private readonly ITestOutputHelper _outputHelpter;
        private TestContext _testContext;
        private const int USER = 0;        private const int DEPARTMENT_ZERO = 0;
        public AccountControllerTest(ITestOutputHelper outputHelpter)
        {
            _outputHelpter = outputHelpter;
        }

        private void outputEmail(string r, string s, string m)
        {
            _outputHelpter.WriteLine($"Email sent:\nReceiver: {r}\nSubject: {s}\n\n{m}");
        }

        private AccountController initializeTest()
        {
            _testContext = new TestContext();

            var mockEmail = new Mock<IEmailSender>();
            mockEmail.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback(new System.Action<string, string, string>(outputEmail))
                .Returns(Task.FromResult(0));
            AccountController ac = new AccountController(                _testContext.MockUserManager,                new MockSignInManager(_testContext.MockUserManager, _testContext),                mockEmail.Object,                _testContext.MockLoggerFactory.Object);

            _testContext.MockHttpContext = ac.MockHttpContext();            _testContext.MockHttpContext                .Setup(mhc => mhc.Request.Scheme)                .Returns("Scheme?");

            var mockUrlHelper = new Mock<IUrlHelper>();            ac.Url = mockUrlHelper.Object;
            return ac;
        }
        #region Login
        [Fact]
        public void Login_CredentialsOk_OK()
        {
            var accountController = initializeTest();            
            var res = accountController.Login(new LoginDTO()
            {
                Username = _testContext.MockUsers[USER].UserName,
                Password = "password"
            }).Result;            if (res is ObjectResult)                _outputHelpter.WriteLine((res as ObjectResult).Value.ToString());
            Assert.IsType<OkResult>(res);
        }
        [Fact]
        public void Login_UsernameOkPasswordInvalid_Unauthorized()
        {
            var accountController = initializeTest();
            var res = accountController.Login(new LoginDTO()
            {
                Username = _testContext.MockUsers[USER].UserName,
                Password = "INVALID"
            }).Result;            if (res is ObjectResult)                _outputHelpter.WriteLine((res as ObjectResult).Value.ToString());
            Assert.IsType<UnauthorizedResult>(res);
        }
        [Fact]
        public void Login_UsernameInvalidPasswordOk_Unauthorized()
        {
            var accountController = initializeTest();
            var res = accountController.Login(new LoginDTO()
            {
                Username = "INVALID",
                Password = "password"
            }).Result;            if (res is ObjectResult)                _outputHelpter.WriteLine((res as ObjectResult).Value.ToString());
            Assert.IsType<UnauthorizedResult>(res);
        }        [Fact]
        public void Login_NullDTO_BadRequest()
        {
            var accountController = initializeTest();
            var res = accountController.Login(null).Result;            if (res is ObjectResult)                _outputHelpter.WriteLine((res as ObjectResult).Value.ToString());
            Assert.IsType<BadRequestObjectResult>(res);
        }
        #endregion
        #region Register
        [Fact]
        public void Register_InputOk_ExpectOK()
        {
            var accountController = initializeTest();
            var res = accountController.Register( new RegisterDTO()
            {
                Username = "InputOk",
                Password = "InputOk",
                ConfirmPassword = "InputOk",
                DepartmentId = DEPARTMENT_ZERO
            }).Result;
            if (res is ObjectResult)                _outputHelpter.WriteLine((res as ObjectResult).Value.ToString());

            Assert.IsType<OkObjectResult>(res);
        }        
        [Fact]
        public void Register_ExistingUsername_BadRequest()
        {
            var accountController = initializeTest();
            var res = accountController.Register(new RegisterDTO()
            {
                Username = _testContext.MockUsers[USER].UserName,
                Password = "password",
                ConfirmPassword = "password",
                DepartmentId = DEPARTMENT_ZERO
            }).Result;            if (res is ObjectResult)                _outputHelpter.WriteLine((res as ObjectResult).Value.ToString());
            Assert.IsType<BadRequestResult>(res);
        }
        [Fact]
        public void Register_NoConfirmPassword_BadRequest()
        {
            var accountController = initializeTest();
            var res = accountController.Register(new RegisterDTO()
            {
                Username = "NewUser",
                Password = "password",
                DepartmentId = DEPARTMENT_ZERO
            }).Result;            if (res is ObjectResult)                _outputHelpter.WriteLine((res as ObjectResult).Value.ToString());
            Assert.IsType<BadRequestObjectResult>(res);
        }        [Fact]
        public void Register_NoPassword_BadRequest()
        {
            var accountController = initializeTest();
            var res = accountController.Register(new RegisterDTO()
            {
                Username = "NewUser",
                ConfirmPassword = "password",
                DepartmentId = DEPARTMENT_ZERO
            }).Result;            if (res is ObjectResult)                _outputHelpter.WriteLine((res as ObjectResult).Value.ToString());
            Assert.IsType<BadRequestObjectResult>(res);
        }
        [Fact]
        public void Register_NoConfirmUsername_BadRequest()
        {
            var accountController = initializeTest();
            var res = accountController.Register(new RegisterDTO()
            {
                Password = "password",
                ConfirmPassword = "password",
                DepartmentId = DEPARTMENT_ZERO
            }).Result;            if (res is ObjectResult)                _outputHelpter.WriteLine((res as ObjectResult).Value.ToString());
            Assert.IsType<BadRequestObjectResult>(res);
        }

        [Fact]
        public void Register_NoDepartment_OkDepKeyIsMinus1()
        {
            var accountController = initializeTest();
            var res = accountController.Register(new RegisterDTO()
            {
                Username = "NewUser",
                Password = "password",
                ConfirmPassword = "password"
            }).Result;            if (res is ObjectResult)                _outputHelpter.WriteLine((res as ObjectResult).Value.ToString());
            Assert.IsType<OkObjectResult>(res);
            var user = (res as ObjectResult).Value as GirafUserDTO;
            Assert.Equal(-1, user.DepartmentKey);
        }

        [Fact]
        public void Register_PasswordMismatch_BadRequest()
        {
            var accountController = initializeTest();
            var res = accountController.Register(new RegisterDTO()
            {
                Username = "NewUser",
                Password = "password",
                ConfirmPassword = "drowssap",
                DepartmentId = DEPARTMENT_ZERO
            }).Result;            if (res is ObjectResult)                _outputHelpter.WriteLine((res as ObjectResult).Value.ToString());
            Assert.IsType<BadRequestObjectResult>(res);
        }        [Fact]
        public void Register_BlankDTO_BadRequest()
        {
            var accountController = initializeTest();
            var res = accountController.Register(new RegisterDTO()
            {
                Username = "",
                Password = "",
                ConfirmPassword = ""
            }).Result;            if (res is ObjectResult)                _outputHelpter.WriteLine((res as ObjectResult).Value.ToString());
            Assert.IsType<BadRequestObjectResult>(res);
        }        [Fact]
        public void Register_NullDTO_BadRequest()
        {
            var accountController = initializeTest();
            var res = accountController.Register(null).Result;            if (res is ObjectResult)                _outputHelpter.WriteLine((res as ObjectResult).Value.ToString());
            Assert.IsType<BadRequestObjectResult>(res);
        }
        #endregion
        #region SetPassword
        [Fact]
        public void SetPassword_ValidInput_Ok()
        {
            var ac = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[USER]);

            SetPasswordDTO spDTO = new SetPasswordDTO()
            {
                NewPassword = "newPassword",
                ConfirmPassword = "newPassword"
            };
            var result = ac.SetPassword(spDTO).Result;

            if (result is ObjectResult)
                _outputHelpter.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<OkObjectResult>(result);
        }


        [Fact]
        public void SetPassword_MissingNewPasswod_BadRequest()
        {
            var ac = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[USER]);

            SetPasswordDTO spDTO = new SetPasswordDTO()
            {
                ConfirmPassword = "newPassword"
            };
            var result = ac.SetPassword(spDTO).Result;

            if (result is ObjectResult)
                _outputHelpter.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public void SetPassword_MissingConfirmPassword_BadRequest()
        {
            var ac = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[USER]);

            SetPasswordDTO spDTO = new SetPasswordDTO()
            {
                NewPassword = "newPassword"
            };
            var result = ac.SetPassword(spDTO).Result;

            if (result is ObjectResult)
                _outputHelpter.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public void SetPasswod_NullDTO_BadRequest()
        {
            var ac = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[USER]);

            SetPasswordDTO spDTO = null;
            var result = ac.SetPassword(spDTO).Result;

            if (result is ObjectResult)
                _outputHelpter.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public void SetPassword_PasswordMismatch_BadRequest()
        {
            var ac = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[USER]);

            SetPasswordDTO spDTO = new SetPasswordDTO()
            {
                NewPassword = "newPassword",
                ConfirmPassword = "NEWPassword"
            };
            var result = ac.SetPassword(spDTO).Result;

            if (result is ObjectResult)
                _outputHelpter.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<BadRequestObjectResult>(result);
        }


        #endregion
        #region ChangePassword
        [Fact]
        public void ChangePassword_ValidInput_Ok()
        {
            var ac = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[USER]);

            ChangePasswordDTO cpDTO = new ChangePasswordDTO()
            {
                OldPassword = "password",
                NewPassword = "PASSWORD",
                ConfirmPassword = "PASSWORD"
            };
            var result = ac.ChangePassword(cpDTO).Result;

            if (result is ObjectResult)
                _outputHelpter.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<OkObjectResult>(result);
        }


        [Fact]
        public void ChangePassword_NoNewPassword_BadRequest()
        {
            var ac = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[USER]);

            ChangePasswordDTO cpDTO = new ChangePasswordDTO()
            {
                OldPassword = "password",
                ConfirmPassword = "PASSWORD"
            };
            var result = ac.ChangePassword(cpDTO).Result;

            if (result is ObjectResult)
                _outputHelpter.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public void ChangePassword_NoConfirmPassword_BadRequest()
        {
            var ac = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[USER]);

            ChangePasswordDTO cpDTO = new ChangePasswordDTO()
            {
                OldPassword = "password",
                NewPassword = "PASSWORD"
            };
            var result = ac.ChangePassword(cpDTO).Result;

            if (result is ObjectResult)
                _outputHelpter.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public void ChangePassword_NoOldPasswod_BadRequest()
        {
            var ac = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[USER]);

            ChangePasswordDTO cpDTO = new ChangePasswordDTO()
            {
                NewPassword = "PASSWORD",
                ConfirmPassword = "PASSWORD"
            };
            var result = ac.ChangePassword(cpDTO).Result;

            if (result is ObjectResult)
                _outputHelpter.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public void ChangePassword_NullDTO_BadRequest()
        {
            var ac = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[USER]);

            ChangePasswordDTO cpDTO = null;
            var result = ac.ChangePassword(cpDTO).Result;

            if (result is ObjectResult)
                _outputHelpter.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public void ChangePassword_PasswordMismatch_BadRequest()
        {
            var ac = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[USER]);

            ChangePasswordDTO cpDTO = new ChangePasswordDTO()
            {
                OldPassword = "password",
                NewPassword = "PASSWORD",
                ConfirmPassword = "DROWSSAP"
            };
            var result = ac.ChangePassword(cpDTO).Result;

            if (result is ObjectResult)
                _outputHelpter.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public void ChangePassword_WrongOldPassword_BadRequest()
        {
            var ac = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[USER]);

            ChangePasswordDTO cpDTO = new ChangePasswordDTO()
            {
                OldPassword = "drowssap",
                NewPassword = "PASSWORD",
                ConfirmPassword = "PASSWORD"
            };
            var result = ac.ChangePassword(cpDTO).Result;

            if (result is ObjectResult)
                _outputHelpter.WriteLine((result as ObjectResult).Value.ToString());

            Assert.IsType<BadRequestObjectResult>(result);
        }
        #endregion
        #region ForgotPassword
        [Fact]
        public void ForgotPassword_UserExist_Ok()
        {
            var accountController = initializeTest();

            var res = accountController.ForgotPassword(new ForgotPasswordDTO()
            {
                Username = _testContext.MockUsers[USER].UserName,
                Email = "unittest@giraf.cs.aau.dk"
            }).Result;            if (res is ObjectResult)                _outputHelpter.WriteLine((res as ObjectResult).Value.ToString());
            Assert.IsType<OkObjectResult>(res);
        }
        [Fact]
        public void ForgotPassword_UserDoNotExist_Ok()
        {
            //It might seem contradictory that this should return Ok, but we wish to keep it secret if the username exists or not.
            var accountController = initializeTest();
            var res = accountController.ForgotPassword(new ForgotPasswordDTO()
            {
                Username = "UserDoNotExist",
                Email = "UserDoNotExist@UserDoNotExist.com"
            }).Result;            if (res is ObjectResult)                _outputHelpter.WriteLine((res as ObjectResult).Value.ToString());
            Assert.IsType<OkObjectResult>(res);
        }        [Fact]
        public void ForgotPassword_NoUsername_BadRequest()
        {
            var accountController = initializeTest();
            var res = accountController.ForgotPassword(new ForgotPasswordDTO()
            {
                Email = "unittest@giraf.cs.aau.dk"
            }).Result;            if (res is ObjectResult)                _outputHelpter.WriteLine((res as ObjectResult).Value.ToString());
            Assert.IsType<BadRequestObjectResult>(res);
        }        [Fact]
        public void ForgotPassword_NoEmail_BadRequest()
        {
            var accountController = initializeTest();
            var res = accountController.ForgotPassword(new ForgotPasswordDTO()
            {
                Username = _testContext.MockUsers[USER].UserName
            }).Result;            if (res is ObjectResult)                _outputHelpter.WriteLine((res as ObjectResult).Value.ToString());
            Assert.IsType<BadRequestObjectResult>(res);
        }        [Fact]
        public void ForgotPassword_NullDTO_BadRequest()
        {
            var accountController = initializeTest();
            var res = accountController.ForgotPassword(null).Result;            if (res is ObjectResult)                _outputHelpter.WriteLine((res as ObjectResult).Value.ToString());
            Assert.IsType<BadRequestObjectResult>(res);
        }
        #endregion
        [Fact(Skip = "Not implemented yet!")]
        public void ResetPassword_() // hvad skal der testes ved den?
        {
        }        
        [Fact(Skip = "Not implemented yet!")]
        public void ResetPasswordConfirmation_ExpectViewReturned()
        {
        }
    }}