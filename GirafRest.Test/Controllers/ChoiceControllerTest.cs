using GirafRest.Controllers;
using GirafRest.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static GirafRest.Test.UnitTestExtensions;

namespace GirafRest.Test
{
    public class ChoiceControllerTest
    {
        private readonly ChoiceController choiceController;
        private readonly List<string> logs;
        private readonly FakeUserManager userManager;

        public ChoiceControllerTest()
        {
            var dbMock = UnitTestExtensions.CreateMockDbContext();

            var userStore = new Mock<IUserStore<GirafUser>>();
            userManager = UnitTestExtensions.MockUserManager(userStore);
            var lfMock = UnitTestExtensions.CreateMockLoggerFactory();

            choiceController = new ChoiceController(dbMock.Object, userManager, lfMock.Object);
        }

        [Fact]
        public void GetExistingPublic_NoLogin_ExpectOK()
        {
            userManager.MockLogout();

            Choice c = UnitTestExtensions.MockChoices.Where(ch => ch.Options.All(cr => (cr.Resource as PictoFrame).AccessLevel == AccessLevel.PUBLIC)).First();
            var res = choiceController.ReadChoice(c.Id);
            IActionResult aRes = res.Result;

            Assert.IsType<OkObjectResult>(aRes);
        }

        [Fact]
        public void GetExistingPublic_Login_ExpectOK()
        {
            userManager.MockLoginAsUser(MockUsers[0]);

            Choice c = UnitTestExtensions.MockChoices.Where(ch => ch.Options.All(cr => (cr.Resource as PictoFrame).AccessLevel == AccessLevel.PUBLIC)).First();
            var res = choiceController.ReadChoice(c.Id);
            IActionResult aRes = res.Result;

            Assert.IsType<OkObjectResult>(aRes);
        }

        [Fact]
        public void GetExistingPrivate_NoLogin_ExpectUnauthorized()
        {
            userManager.MockLogout();

            Choice c = UnitTestExtensions.MockChoices.Where(ch => ch.Options.All(cr => (cr.Resource as PictoFrame).AccessLevel == AccessLevel.PRIVATE)).First();
            var res = choiceController.ReadChoice(c.Id);
            IActionResult aRes = res.Result;

            Assert.IsType<UnauthorizedResult>(aRes);
        }

        [Fact]
        public void GetExistingPrivate_Login_ExpectOK() {
            userManager.MockLoginAsUser(MockUsers[0]);

            Choice c = UnitTestExtensions.MockChoices.Where(ch => ch.Options.All(cr => (cr.Resource as PictoFrame).AccessLevel == AccessLevel.PRIVATE)).First();
            var res = choiceController.ReadChoice(c.Id);
            IActionResult aRes = res.Result;

            Assert.IsType<OkObjectResult>(aRes);
        }

        [Fact]
        public void GetExistingPrivate_OtherLogin_ExpectUnauthorized()
        {
            userManager.MockLoginAsUser(MockUsers[1]);

            Choice c = UnitTestExtensions.MockChoices.Where(ch => ch.Options.All(cr => (cr.Resource as PictoFrame).AccessLevel == AccessLevel.PRIVATE)).First();
            var res = choiceController.ReadChoice(c.Id);
            IActionResult aRes = res.Result;

            Assert.IsType<UnauthorizedResult>(aRes);
        }

        [Fact]
        public void GetExistingProtected_NoLogin_ExpectUnauthorized()
        {
            userManager.MockLogout();

            Choice c = UnitTestExtensions.MockChoices.Where(ch => ch.Options.All(cr => (cr.Resource as PictoFrame).AccessLevel == AccessLevel.PROTECTED)).First();
            var res = choiceController.ReadChoice(c.Id);
            IActionResult aRes = res.Result;

            Assert.IsType<UnauthorizedResult>(aRes);
        }

        [Fact]
        public void GetExistingProtected_Login_ExpectOK()
        {
            userManager.MockLoginAsUser(MockUsers[1]);

            Choice c = UnitTestExtensions.MockChoices.Where(ch => ch.Options.All(cr => (cr.Resource as PictoFrame).AccessLevel == AccessLevel.PROTECTED)).First();
            var res = choiceController.ReadChoice(c.Id);
            IActionResult aRes = res.Result;

            Assert.IsType<OkObjectResult>(aRes);
        }

        [Fact]
        public void GetExistingProtected_OtherLogin_ExpectUnauthorized()
        {
            userManager.MockLoginAsUser(MockUsers[0]);

            Choice c = UnitTestExtensions.MockChoices.Where(ch => ch.Options.All(cr => (cr.Resource as PictoFrame).AccessLevel == AccessLevel.PROTECTED)).First();
            var res = choiceController.ReadChoice(c.Id);
            IActionResult aRes = res.Result;

            Assert.IsType<UnauthorizedResult>(aRes);
        }

        [Fact]
        public void GetNonExistingChoice_NoLogin_ExpectNotFound()
        {
            userManager.MockLogout();

            var res = choiceController.ReadChoice(999);
            IActionResult aRes = res.Result;

            Assert.IsType<NotFoundResult>(aRes);
        }

        [Fact]
        public void GetNonExistingChoice_Login_ExpectNotFound()
        {
            userManager.MockLoginAsUser(MockUsers[0]);

            var res = choiceController.ReadChoice(999);
            IActionResult aRes = res.Result;

            Assert.IsType<NotFoundResult>(aRes);
        }

        [Fact]
        public void DeleteExistingPublic_NoLogin_ExpectOk()
        {
            userManager.MockLogout();

            Choice c = UnitTestExtensions.MockChoices.Where(ch => ch.Options.All(cr => (cr.Resource as PictoFrame).AccessLevel == AccessLevel.PUBLIC)).First();
            var res = choiceController.DeleteChoice(c.Id);
            IActionResult aRes = res.Result;

            Assert.IsType<OkResult>(aRes);
        }

        [Fact]
        public void DeleteExistingPublic_Login_ExpectOk()
        {
            userManager.MockLoginAsUser(MockUsers[0]);

            Choice c = UnitTestExtensions.MockChoices.Where(ch => ch.Options.All(cr => (cr.Resource as PictoFrame).AccessLevel == AccessLevel.PUBLIC)).First();
            var res = choiceController.DeleteChoice(c.Id);
            IActionResult aRes = res.Result;

            Assert.IsType<OkResult>(aRes);
        }

        [Fact]
        public void DeleteExistingPrivate_NoLogin_ExpectUnauthorized()
        {
            userManager.MockLogout();

            Choice c = UnitTestExtensions.MockChoices.Where(ch => ch.Options.All(cr => (cr.Resource as PictoFrame).AccessLevel == AccessLevel.PRIVATE)).First();
            var res = choiceController.DeleteChoice(c.Id);
            IActionResult aRes = res.Result;

            Assert.IsType<UnauthorizedResult>(aRes);
        }

        [Fact]
        public void DeleteExistingPrivate_Login_ExpectOk()
        {
            userManager.MockLoginAsUser(MockUsers[0]);

            Choice c = UnitTestExtensions.MockChoices.Where(ch => ch.Options.All(cr => (cr.Resource as PictoFrame).AccessLevel == AccessLevel.PRIVATE)).First();
            var res = choiceController.DeleteChoice(c.Id);
            IActionResult aRes = res.Result;

            Assert.IsType<OkResult>(aRes);
        }

        [Fact]
        public void DeleteExistingPrivate_OtherLogin_ExpectUnauthorized()
        {
            
        }

        [Fact]
        public void DeleteExistingProtected_NoLogin_ExpectUnauthorized()
        {

        }

        [Fact]
        public void DeleteExistingProtected_Login_ExpectOk()
        {

        }

        [Fact]
        public void DeleteExistingProtected_OtherLogin_ExpectUnauthorized()
        {

        }
    }
}