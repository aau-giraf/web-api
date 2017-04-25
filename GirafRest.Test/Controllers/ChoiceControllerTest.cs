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
        private readonly ChoiceController choiceController;
        private readonly List<string> logs;
        private readonly MockUserManager userManager;
        private readonly ITestOutputHelper _outputHelpter;
        private readonly int publicChoice = 0;
        private readonly int privateChoice = 1;
        private readonly int protectedChoice = 2;
        private readonly int privatePictogram = 3;
        private readonly int protectedPictogram = 5;
        private readonly int nonExisting = 999;
        private readonly Mock<MockDbContext> dbMock;

        public ChoiceControllerTest(ITestOutputHelper outputHelpter)
        {
            dbMock = CreateMockDbContext();

            var userStore = new Mock<IUserStore<GirafUser>>();
            userManager = MockUserManager(userStore);
            var lfMock = CreateMockLoggerFactory();

            choiceController = new ChoiceController(new MockGirafService(dbMock.Object, userManager), lfMock.Object);

            _outputHelpter = outputHelpter;
        }

        [Fact]
        public void GetExistingPublic_NoLogin_ExpectOK()
        {
            userManager.MockLogout();
            var res = choiceController.ReadChoice(MockChoices[publicChoice].Id);
            Assert.IsType<OkObjectResult>(res.Result);
        }

        [Fact]
        public void GetExistingPublic_Login_ExpectOK()
        {
            userManager.MockLoginAsUser(MockUsers[0]);
            var res = choiceController.ReadChoice(MockChoices[publicChoice].Id);
            Assert.IsType<OkObjectResult>(res.Result);
        }

        [Fact]
        public void GetExistingPrivate_NoLogin_ExpectUnauthorized()
        {
            userManager.MockLogout();
            var res = choiceController.ReadChoice(MockChoices[privateChoice].Id);
            Assert.IsType<UnauthorizedResult>(res.Result);
        }

        [Fact]
        public void GetExistingPrivate_Login_ExpectOK()
        {
            userManager.MockLoginAsUser(MockUsers[0]);
            var res = choiceController.ReadChoice(MockChoices[privateChoice].Id);
            Assert.IsType<OkObjectResult>(res.Result);
        }

        [Fact]
        public void GetExistingPrivate_OtherLogin_ExpectUnauthorized()
        {
            userManager.MockLoginAsUser(MockUsers[1]);
            var res = choiceController.ReadChoice(MockChoices[privateChoice].Id);
            Assert.IsType<UnauthorizedResult>(res.Result);
        }

        [Fact]
        public void GetExistingProtected_NoLogin_ExpectUnauthorized()
        {
            userManager.MockLogout();
            var res = choiceController.ReadChoice(MockChoices[protectedChoice].Id);
            Assert.IsType<UnauthorizedResult>(res.Result);
        }

        [Fact]
        public void GetExistingProtected_Login_ExpectOK()
        {
            userManager.MockLoginAsUser(MockUsers[0]);
            var res = choiceController.ReadChoice(MockChoices[protectedChoice].Id);
            Assert.IsType<OkObjectResult>(res.Result);
        }

        [Fact]
        public void GetExistingProtected_OtherLogin_ExpectUnauthorized()
        {
            userManager.MockLoginAsUser(MockUsers[1]);
            var res = choiceController.ReadChoice(MockChoices[protectedChoice].Id);
            Assert.IsType<UnauthorizedResult>(res.Result);
        }

        [Fact]
        public void GetNonExisting_NoLogin_ExpectNotFound()
        {
            userManager.MockLogout();
            var res = choiceController.ReadChoice(nonExisting);
            Assert.IsType<NotFoundResult>(res.Result);
        }

        [Fact]
        public void GetNonExisting_Login_ExpectNotFound()
        {
            userManager.MockLoginAsUser(MockUsers[0]);
            var res = choiceController.ReadChoice(nonExisting);
            Assert.IsType<NotFoundResult>(res.Result);
        }

        [Fact]
        public void CreatePublic_NoLogin_ExpectOk()
        {
            userManager.MockLogout();
            List<PictoFrame> options = MockPictograms.Cast<PictoFrame>().Where(p => p.AccessLevel == AccessLevel.PUBLIC).ToList();
            var res = choiceController.CreateChoice(new ChoiceDTO(new Choice(options) { Id = 100 }));
            Assert.IsType<OkObjectResult>(res.Result);
        }

        [Fact]
        public void CreatePublic_Login_ExpectOk()
        {
            userManager.MockLoginAsUser(MockUsers[0]);
            List<PictoFrame> options = MockPictograms.Cast<PictoFrame>().Where(p => p.AccessLevel == AccessLevel.PUBLIC).ToList();
            var res = choiceController.CreateChoice(new ChoiceDTO(new Choice(options) { Id = 100 }));
            Assert.IsType<OkObjectResult>(res.Result);
        }

        [Fact]
        public void CreatePrivate_NoLogin_ExpectUnauthorized()
        {
            userManager.MockLogout();
            List<PictoFrame> options = new List<PictoFrame> { MockPictograms[privatePictogram] };
            var res = choiceController.CreateChoice(new ChoiceDTO(new Choice(options) { Id = 100 }));
            Assert.IsType<UnauthorizedResult>(res.Result);
        }

        [Fact]
        public void CreatePrivate_Login_ExpectOk()
        {
            userManager.MockLoginAsUser(MockUsers[0]);
            List<PictoFrame> options = new List<PictoFrame> { MockPictograms[privatePictogram] };
            var res = choiceController.CreateChoice(new ChoiceDTO(new Choice(options) { Id = 100 }));
            Assert.IsType<OkObjectResult>(res.Result);
        }

        [Fact]
        public void CreatePrivate_OtherLogin_ExpectUnauthorized()
        {
            userManager.MockLoginAsUser(MockUsers[1]);
            List<PictoFrame> options = new List<PictoFrame> { MockPictograms[privatePictogram] };
            var res = choiceController.CreateChoice(new ChoiceDTO(new Choice(options) { Id = 100 }));
            Assert.IsType<UnauthorizedResult>(res.Result);
        }

        [Fact]
        public void Update_ExistingPublic_NoLogin_ExpectOk()
        {
            Assert.True(false);

            userManager.MockLogout();
            Choice c = new Choice(new List<PictoFrame>()) { Id = MockChoices[publicChoice].Id };
            foreach (var option in MockChoices[publicChoice])
            {
                c.Add(option);
            }
            c.Clear();
            c.AddAll(MockPictograms.Cast<PictoFrame>().Where(p => p.AccessLevel == AccessLevel.PUBLIC).Take(2).ToList());
            var res = choiceController.UpdateChoiceInfo(new ChoiceDTO(c));
            Assert.IsType<OkObjectResult>(res.Result);
            resetChoices();
        }

        [Fact]
        public void Update_ExistingPublic_Login_ExpectOk()
        {
            Assert.True(false);

            userManager.MockLoginAsUser(MockUsers[0]);
            Choice c = new Choice(new List<PictoFrame>()) { Id = MockChoices[publicChoice].Id };
            foreach (var option in MockChoices[publicChoice])
            {
                c.Add(option);
            }
            c.Clear();
            c.AddAll(MockPictograms.Cast<PictoFrame>().Where(p => p.AccessLevel == AccessLevel.PUBLIC).Take(2).ToList());
            var res = choiceController.UpdateChoiceInfo(new ChoiceDTO(c));
            Assert.IsType<OkObjectResult>(res.Result);
            resetChoices();
        }

        [Fact]
        public void Update_ExistingPrivate_NoLogin_ExpectUnauthorized()
        {
            userManager.MockLogout();
            Choice c = new Choice(new List<PictoFrame>()) { Id = MockChoices[privateChoice].Id };
            foreach (var option in MockChoices[privateChoice])
            {
                c.Add(option);
            }
            c.Clear();
            c.AddAll(MockPictograms.Cast<PictoFrame>().Where(p => p.AccessLevel == AccessLevel.PUBLIC).Take(2).ToList());
            var res = choiceController.UpdateChoiceInfo(new ChoiceDTO(c));
            Assert.IsType<UnauthorizedResult>(res.Result);
            resetChoices();
        }

        [Fact]
        public void Update_ExistingPrivate_Login_ExpectOk()
        {
            Assert.True(false);

            userManager.MockLoginAsUser(MockUsers[0]);
            Choice c = new Choice(new List<PictoFrame>()) { Id = MockChoices[privateChoice].Id };
            foreach (var option in MockChoices[privateChoice])
            {
                c.Add(option);
            }
            c.Clear();
            c.AddAll(MockPictograms.Cast<PictoFrame>().Where(p => p.AccessLevel == AccessLevel.PUBLIC).Take(2).ToList());
            foreach (var option in MockChoices[privateChoice])
            {
                _outputHelpter.WriteLine(option.Title);
            }
            var res = choiceController.UpdateChoiceInfo(new ChoiceDTO(c));
            foreach (var option in MockChoices[privateChoice])
            {
                _outputHelpter.WriteLine(option.Title);
            }
            Assert.IsType<OkObjectResult>(res.Result);
            resetChoices();
        }

        [Fact]
        public void Update_ExistingPrivate_OtherLogin_ExpectUnauthorized()
        {
            userManager.MockLoginAsUser(MockUsers[1]);
            Choice c = new Choice(new List<PictoFrame>()) { Id = MockChoices[privateChoice].Id };
            foreach (var option in MockChoices[privateChoice])
            {
                c.Add(option);
            }
            c.Clear();
            c.AddAll(MockPictograms.Cast<PictoFrame>().Where(p => p.AccessLevel == AccessLevel.PUBLIC).Take(2).ToList());
            var res = choiceController.UpdateChoiceInfo(new ChoiceDTO(c));
            Assert.IsType<UnauthorizedResult>(res.Result);
            resetChoices();
        }

        [Fact]
        public void Update_ExistingProtected_NoLogin_ExpectUnauthorized()
        {
            userManager.MockLogout();
            Choice c = new Choice(new List<PictoFrame>()) { Id = MockChoices[protectedChoice].Id };
            foreach (var option in MockChoices[protectedChoice])
            {
                c.Add(option);
            }
            c.Clear();
            c.AddAll(MockPictograms.Cast<PictoFrame>().Where(p => p.AccessLevel == AccessLevel.PUBLIC).Take(2).ToList());
            var res = choiceController.UpdateChoiceInfo(new ChoiceDTO(c));
            Assert.IsType<UnauthorizedResult>(res.Result);
            resetChoices();
        }

        [Fact]
        public void Update_ExistingProtected_Login_ExpectOk()
        {
            Assert.True(false);

            userManager.MockLoginAsUser(MockUsers[0]);
            Choice c = new Choice(new List<PictoFrame>()) { Id = MockChoices[protectedChoice].Id };
            foreach (var option in MockChoices[protectedChoice])
            {
                c.Add(option);
            }
            c.Clear();
            c.AddAll(MockPictograms.Cast<PictoFrame>().Where(p => p.AccessLevel == AccessLevel.PUBLIC).Take(2).ToList());
            var res = choiceController.UpdateChoiceInfo(new ChoiceDTO(c));
            Assert.IsType<OkObjectResult>(res.Result);
            resetChoices();
        }

        [Fact]
        public void Update_ExistingProtected_OtherLogin_ExpectUnauthorized()
        {
            userManager.MockLoginAsUser(MockUsers[1]);
            Choice c = new Choice(new List<PictoFrame>()) { Id = MockChoices[protectedChoice].Id };
            foreach (var option in MockChoices[protectedChoice])
            {
                c.Add(option);
            }
            c.Clear();
            c.AddAll(MockPictograms.Cast<PictoFrame>().Where(p => p.AccessLevel == AccessLevel.PUBLIC).Take(2).ToList());
            var res = choiceController.UpdateChoiceInfo(new ChoiceDTO(c));
            Assert.IsType<UnauthorizedResult>(res.Result);
            resetChoices();
        }

        [Fact]
        public void Update_NonExisting_NoLogin_ExpectNotFound()
        {
            userManager.MockLogout();
            Choice c = new Choice(new List<PictoFrame>()) { Id = 999 };
            c.AddAll(MockPictograms.Cast<PictoFrame>().Where(p => p.AccessLevel == AccessLevel.PUBLIC).Take(2).ToList());
            var res = choiceController.UpdateChoiceInfo(new ChoiceDTO(c));
            Assert.IsType<NotFoundResult>(res.Result);
        }

        [Fact]
        public void Update_NonExisting_Login_ExpectNotFound()
        {
            userManager.MockLoginAsUser(MockUsers[0]);
            Choice c = new Choice(new List<PictoFrame>()) { Id = 999 };
            c.AddAll(MockPictograms.Cast<PictoFrame>().Where(p => p.AccessLevel == AccessLevel.PUBLIC).Take(2).ToList());
            var res = choiceController.UpdateChoiceInfo(new ChoiceDTO(c));
            Assert.IsType<NotFoundResult>(res.Result);
        }

        [Fact]
        public void DeleteExistingPublic_NoLogin_ExpectOk()
        {
            userManager.MockLogout();
            var res = choiceController.DeleteChoice(MockChoices[publicChoice].Id);
            Assert.IsType<OkResult>(res.Result);
        }

        [Fact]
        public void DeleteExistingPublic_Login_ExpectOk()
        {
            userManager.MockLoginAsUser(MockUsers[0]);
            var res = choiceController.DeleteChoice(MockChoices[publicChoice].Id);
            Assert.IsType<OkResult>(res.Result);
        }

        [Fact]
        public void DeleteExistingPrivate_NoLogin_ExpectUnauthorized()
        {
            userManager.MockLogout();
            var res = choiceController.DeleteChoice(MockChoices[privateChoice].Id);
            Assert.IsType<UnauthorizedResult>(res.Result);
        }

        [Fact]
        public void DeleteExistingPrivate_Login_ExpectOk()
        {
            userManager.MockLoginAsUser(MockUsers[0]);
            var res = choiceController.DeleteChoice(MockChoices[privateChoice].Id);
            Assert.IsType<OkResult>(res.Result);
        }

        [Fact]
        public void DeleteExistingPrivate_OtherLogin_ExpectUnauthorized()
        {
            userManager.MockLoginAsUser(MockUsers[1]);
            var res = choiceController.DeleteChoice(MockChoices[privateChoice].Id);
            Assert.IsType<UnauthorizedResult>(res.Result);
        }

        [Fact]
        public void DeleteExistingProtected_NoLogin_ExpectUnauthorized()
        {
            userManager.MockLogout();
            var res = choiceController.DeleteChoice(MockChoices[protectedChoice].Id);
            Assert.IsType<UnauthorizedResult>(res.Result);
        }

        [Fact]
        public void DeleteExistingProtected_Login_ExpectOk()
        {
            userManager.MockLoginAsUser(MockUsers[0]);
            var res = choiceController.DeleteChoice(MockChoices[protectedChoice].Id);
            Assert.IsType<OkResult>(res.Result);
        }

        [Fact]
        public void DeleteExistingProtected_OtherLogin_ExpectUnauthorized()
        {
            userManager.MockLoginAsUser(MockUsers[1]);
            var res = choiceController.DeleteChoice(MockChoices[protectedChoice].Id);
            Assert.IsType<UnauthorizedResult>(res.Result);
        }

        [Fact]
        public void DeleteNonExisting_NoLogin_ExpectNotFound()
        {
            userManager.MockLogout();
            var res = choiceController.DeleteChoice(nonExisting);
            Assert.IsType<NotFoundResult>(res.Result);
        }

        [Fact]
        public void DeleteNonExisting_Login_ExpectNotFound()
        {
            userManager.MockLoginAsUser(MockUsers[0]);
            var res = choiceController.DeleteChoice(nonExisting);
            Assert.IsType<NotFoundResult>(res.Result);
        }

        public void resetChoices()
        {
            dbMock.Setup(c => c.Choices).Returns(CreateMockDbSet(MockChoices).Object);
        }
    }
}