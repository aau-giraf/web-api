using GirafRest.Controllers;
using GirafRest.Models;
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
    public class ChoiceControllerTest
    {
        private TestContext _testContext;
        private readonly ITestOutputHelper _outputHelpter;
        private const int PUBLIC_CHOICE = 0;
        private const int PRIVATE_CHOICE = 1;
        private const int PROTECTED_CHOICE = 2;
        private const int PRIVATE_PICTOGRAM = 3;
        private const int PROTECTED_PICTOGRAM = 5;
        private const int NONEXISTING = 999;

        public ChoiceControllerTest(ITestOutputHelper outputHelpter)
        {

        }

        private ChoiceController initializeTest()
        {
            _testContext = new TestContext();

            var cc = new ChoiceController(
                new MockGirafService(_testContext.MockDbContext.Object,
                _testContext.MockUserManager), _testContext.MockLoggerFactory.Object);
            _testContext.MockHttpContext = cc.MockHttpContext();

            return cc;
        }

        [Fact]
        public void GetExistingPublic_NoLogin_ExpectOK()
        {
            var cc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            var res = cc.ReadChoice(_testContext.MockChoices[PUBLIC_CHOICE].Id);
            Assert.IsType<OkObjectResult>(res.Result);
        }

        [Fact]
        public void GetExistingPublic_Login_ExpectOK()
        {
            var choiceController = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);

            var res = choiceController.ReadChoice(_testContext.MockChoices[PUBLIC_CHOICE].Id);
            Assert.IsType<OkObjectResult>(res.Result);
        }

        [Fact]
        public void GetExistingPrivate_NoLogin_ExpectUnauthorized()
        {
            var cc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            var res = cc.ReadChoice(_testContext.MockChoices[PRIVATE_CHOICE].Id);
            Assert.IsType<UnauthorizedResult>(res.Result);
        }

        [Fact]
        public void GetExistingPrivate_Login_ExpectOK()
        {
            var choiceController = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);

            var res = choiceController.ReadChoice(_testContext.MockChoices[PRIVATE_CHOICE].Id);
            Assert.IsType<OkObjectResult>(res.Result);
        }

        [Fact]
        public void GetExistingPrivate_OtherLogin_ExpectUnauthorized()
        {
            var choiceController = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[1]);
            var res = choiceController.ReadChoice(_testContext.MockChoices[PRIVATE_CHOICE].Id);
            Assert.IsType<UnauthorizedResult>(res.Result);
        }

        [Fact]
        public void GetExistingProtected_NoLogin_ExpectUnauthorized()
        {
            var cc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            var res = cc.ReadChoice(_testContext.MockChoices[PROTECTED_CHOICE].Id);
            Assert.IsType<UnauthorizedResult>(res.Result);
        }

        [Fact]
        public void GetExistingProtected_Login_ExpectOK()
        {
            var choiceController = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);

            var res = choiceController.ReadChoice(_testContext.MockChoices[PROTECTED_CHOICE].Id);
            Assert.IsType<OkObjectResult>(res.Result);
        }

        [Fact]
        public void GetExistingProtected_OtherLogin_ExpectUnauthorized()
        {
            var choiceController = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[1]);
            var res = choiceController.ReadChoice(_testContext.MockChoices[PROTECTED_CHOICE].Id);
            Assert.IsType<UnauthorizedResult>(res.Result);
        }

        [Fact]
        public void GetNonExisting_NoLogin_ExpectNotFound()
        {
            var cc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            var res = cc.ReadChoice(NONEXISTING);
            Assert.IsType<NotFoundResult>(res.Result);
        }

        [Fact]
        public void GetNonExisting_Login_ExpectNotFound()
        {
            var choiceController = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            var res = choiceController.ReadChoice(NONEXISTING);
            Assert.IsType<NotFoundResult>(res.Result);
        }

        [Fact]
        public void CreatePublic_NoLogin_ExpectOk()
        {
            var cc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            List<PictoFrame> options = _testContext.MockPictograms.Cast<PictoFrame>().Where(p => p.AccessLevel == AccessLevel.PUBLIC).ToList();
            var res = cc.CreateChoice(new ChoiceDTO(new Choice(options) { Id = 100 }));
            Assert.IsType<OkObjectResult>(res.Result);
        }

        [Fact]
        public void CreatePublic_Login_ExpectOk()
        {
            var choiceController = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            List<PictoFrame> options = _testContext.MockPictograms
                .Cast<PictoFrame>()
                .Where(p => p.AccessLevel == AccessLevel.PUBLIC)
                .ToList();
            var res = choiceController.CreateChoice(new ChoiceDTO(new Choice(options) { Id = 100 }));
            Assert.IsType<OkObjectResult>(res.Result);
        }

        [Fact]
        public void CreatePrivate_NoLogin_ExpectUnauthorized()
        {
            var cc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            List<PictoFrame> options = new List<PictoFrame> { _testContext.MockPictograms[PRIVATE_PICTOGRAM] };
            var res = cc.CreateChoice(new ChoiceDTO(new Choice(options) { Id = 100 }));
            Assert.IsType<UnauthorizedResult>(res.Result);
        }

        [Fact]
        public void CreatePrivate_Login_ExpectOk()
        {
            var choiceController = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            List<PictoFrame> options = new List<PictoFrame> { _testContext.MockPictograms[PRIVATE_PICTOGRAM] };
            var res = choiceController.CreateChoice(new ChoiceDTO(new Choice(options) { Id = 100 }));
            Assert.IsType<OkObjectResult>(res.Result);
        }

        [Fact]
        public void CreatePrivate_OtherLogin_ExpectUnauthorized()
        {
            var choiceController = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[1]);
            List<PictoFrame> options = new List<PictoFrame> { _testContext.MockPictograms[PRIVATE_PICTOGRAM] };
            var res = choiceController.CreateChoice(new ChoiceDTO(new Choice(options) { Id = 100 }));
            Assert.IsType<UnauthorizedResult>(res.Result);
        }

        [Fact]
        public void Update_ExistingPublic_NoLogin_ExpectOk()
        {
            var cc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            Choice c = new Choice(new List<PictoFrame>()) { Id = _testContext.MockChoices[PUBLIC_CHOICE].Id };
            foreach (var option in _testContext.MockChoices[PUBLIC_CHOICE])
            {
                c.Add(option);
            }
            c.Clear();
            c.AddAll(_testContext.MockPictograms.Cast<PictoFrame>().Where(p => p.AccessLevel == AccessLevel.PUBLIC).Take(2).ToList());
            var res = cc.UpdateChoiceInfo(new ChoiceDTO(c));
            Assert.IsType<OkObjectResult>(res.Result);
        }

        [Fact]
        public void Update_ExistingPublic_Login_ExpectOk()
        {
            var choiceController = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            Choice c = new Choice(new List<PictoFrame>()) { Id = _testContext.MockChoices[PUBLIC_CHOICE].Id };
            foreach (var option in _testContext.MockChoices[PUBLIC_CHOICE])
            {
                c.Add(option);
            }
            c.Clear();
            c.AddAll(_testContext.MockPictograms.Cast<PictoFrame>().Where(p => p.AccessLevel == AccessLevel.PUBLIC).Take(2).ToList());
            var res = choiceController.UpdateChoiceInfo(new ChoiceDTO(c));
            Assert.IsType<OkObjectResult>(res.Result);
        }

        [Fact]
        public void Update_ExistingPrivate_NoLogin_ExpectUnauthorized()
        {
            var cc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            Choice c = new Choice(new List<PictoFrame>()) { Id = _testContext.MockChoices[PRIVATE_CHOICE].Id };
            foreach (var option in _testContext.MockChoices[PRIVATE_CHOICE])
            {
                c.Add(option);
            }
            c.Clear();
            c.AddAll(_testContext.MockPictograms.Cast<PictoFrame>().Where(p => p.AccessLevel == AccessLevel.PUBLIC).Take(2).ToList());
            var res = cc.UpdateChoiceInfo(new ChoiceDTO(c));
            Assert.IsType<UnauthorizedResult>(res.Result);
        }

        [Fact]
        public void Update_ExistingPrivate_Login_ExpectOk()
        {
            var choiceController = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            Choice c = new Choice(new List<PictoFrame>()) { Id = _testContext.MockChoices[PRIVATE_CHOICE].Id };
            foreach (var option in _testContext.MockChoices[PRIVATE_CHOICE])
            {
                c.Add(option);
            }
            c.Clear();
            c.AddAll(_testContext.MockPictograms.Cast<PictoFrame>().Where(p => p.AccessLevel == AccessLevel.PUBLIC).Take(2).ToList());
            foreach (var option in _testContext.MockChoices[PRIVATE_CHOICE])
            {
                _outputHelpter.WriteLine(option.Title);
            }
            var res = choiceController.UpdateChoiceInfo(new ChoiceDTO(c));
            foreach (var option in _testContext.MockChoices[PRIVATE_CHOICE])
            {
                _outputHelpter.WriteLine(option.Title);
            }
            Assert.IsType<OkObjectResult>(res.Result);
        }

        [Fact]
        public void Update_ExistingPrivate_OtherLogin_ExpectUnauthorized()
        {
            var choiceController = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[1]);
            Choice c = new Choice(new List<PictoFrame>()) { Id = _testContext.MockChoices[PRIVATE_CHOICE].Id };
            foreach (var option in _testContext.MockChoices[PRIVATE_CHOICE])
            {
                c.Add(option);
            }
            c.Clear();
            c.AddAll(_testContext.MockPictograms.Cast<PictoFrame>().Where(p => p.AccessLevel == AccessLevel.PUBLIC).Take(2).ToList());
            var res = choiceController.UpdateChoiceInfo(new ChoiceDTO(c));
            Assert.IsType<UnauthorizedResult>(res.Result);
        }

        [Fact]
        public void Update_ExistingProtected_NoLogin_ExpectUnauthorized()
        {
            var cc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            Choice c = new Choice(new List<PictoFrame>()) { Id = _testContext.MockChoices[PROTECTED_CHOICE].Id };
            foreach (var option in _testContext.MockChoices[PROTECTED_CHOICE])
            {
                c.Add(option);
            }
            c.Clear();
            c.AddAll(_testContext.MockPictograms.Cast<PictoFrame>().Where(p => p.AccessLevel == AccessLevel.PUBLIC).Take(2).ToList());
            var res = cc.UpdateChoiceInfo(new ChoiceDTO(c));
            Assert.IsType<UnauthorizedResult>(res.Result);
        }

        [Fact]
        public void Update_ExistingProtected_Login_ExpectOk()
        {
            var choiceController = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            Choice c = new Choice(new List<PictoFrame>()) { Id = _testContext.MockChoices[PROTECTED_CHOICE].Id };
            foreach (var option in _testContext.MockChoices[PROTECTED_CHOICE])
            {
                c.Add(option);
            }
            c.Clear();
            c.AddAll(_testContext.MockPictograms.Cast<PictoFrame>().Where(p => p.AccessLevel == AccessLevel.PUBLIC).Take(2).ToList());
            var res = choiceController.UpdateChoiceInfo(new ChoiceDTO(c));
            Assert.IsType<OkObjectResult>(res.Result);
        }

        [Fact]
        public void Update_ExistingProtected_OtherLogin_ExpectUnauthorized()
        {
            var choiceController = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[1]);
            Choice c = new Choice(new List<PictoFrame>()) { Id = _testContext.MockChoices[PROTECTED_CHOICE].Id };
            foreach (var option in _testContext.MockChoices[PROTECTED_CHOICE])
            {
                c.Add(option);
            }
            c.Clear();
            c.AddAll(_testContext.MockPictograms.Cast<PictoFrame>().Where(p => p.AccessLevel == AccessLevel.PUBLIC).Take(2).ToList());
            var res = choiceController.UpdateChoiceInfo(new ChoiceDTO(c));
            Assert.IsType<UnauthorizedResult>(res.Result);
        }

        [Fact]
        public void Update_NonExisting_NoLogin_ExpectNotFound()
        {
            var cc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            Choice c = new Choice(new List<PictoFrame>()) { Id = 999 };
            c.AddAll(_testContext.MockPictograms.Cast<PictoFrame>().Where(p => p.AccessLevel == AccessLevel.PUBLIC).Take(2).ToList());
            var res = cc.UpdateChoiceInfo(new ChoiceDTO(c));
            Assert.IsType<NotFoundResult>(res.Result);
        }

        [Fact]
        public void Update_NonExisting_Login_ExpectNotFound()
        {
            var choiceController = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            Choice c = new Choice(new List<PictoFrame>()) { Id = 999 };
            c.AddAll(_testContext.MockPictograms
                .Cast<PictoFrame>()
                .Where(p => p.AccessLevel == AccessLevel.PUBLIC)
                .Take(2)
                .ToList());
            var res = choiceController.UpdateChoiceInfo(new ChoiceDTO(c));
            Assert.IsType<NotFoundResult>(res.Result);
        }

        [Fact]
        public void DeleteExistingPublic_NoLogin_ExpectOk()
        {
            var cc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            var res = cc.DeleteChoice(_testContext.MockChoices[PUBLIC_CHOICE].Id);
            Assert.IsType<OkResult>(res.Result);
        }

        [Fact]
        public void DeleteExistingPublic_Login_ExpectOk()
        {
            var choiceController = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            var res = choiceController.DeleteChoice(_testContext.MockChoices[PUBLIC_CHOICE].Id);
            Assert.IsType<OkResult>(res.Result);
        }

        [Fact]
        public void DeleteExistingPrivate_NoLogin_ExpectUnauthorized()
        {
            var cc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            var res = cc.DeleteChoice(_testContext.MockChoices[PRIVATE_CHOICE].Id);
            Assert.IsType<UnauthorizedResult>(res.Result);
        }

        [Fact]
        public void DeleteExistingPrivate_Login_ExpectOk()
        {
            var choiceController = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            var res = choiceController.DeleteChoice(_testContext.MockChoices[PRIVATE_CHOICE].Id);
            Assert.IsType<OkResult>(res.Result);
        }

        [Fact]
        public void DeleteExistingPrivate_OtherLogin_ExpectUnauthorized()
        {
            var choiceController = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[1]);
            var res = choiceController.DeleteChoice(_testContext.MockChoices[PRIVATE_CHOICE].Id);
            Assert.IsType<UnauthorizedResult>(res.Result);
        }

        [Fact]
        public void DeleteExistingProtected_NoLogin_ExpectUnauthorized()
        {
            var cc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            var res = cc.DeleteChoice(_testContext.MockChoices[PROTECTED_CHOICE].Id);
            Assert.IsType<UnauthorizedResult>(res.Result);
        }

        [Fact]
        public void DeleteExistingProtected_Login_ExpectOk()
        {
            var choiceController = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            var res = choiceController.DeleteChoice(_testContext.MockChoices[PROTECTED_CHOICE].Id);
            Assert.IsType<OkResult>(res.Result);
        }

        [Fact]
        public void DeleteExistingProtected_OtherLogin_ExpectUnauthorized()
        {
            var choiceController = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[1]);
            var res = choiceController.DeleteChoice(_testContext.MockChoices[PROTECTED_CHOICE].Id);
            Assert.IsType<UnauthorizedResult>(res.Result);
        }

        [Fact]
        public void DeleteNonExisting_NoLogin_ExpectNotFound()
        {
            var cc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            var res = cc.DeleteChoice(NONEXISTING);
            Assert.IsType<NotFoundResult>(res.Result);
        }

        [Fact]
        public void DeleteNonExisting_Login_ExpectNotFound()
        {
            var choiceController = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            var res = choiceController.DeleteChoice(NONEXISTING);
            Assert.IsType<NotFoundResult>(res.Result);
        }
    }
}