using System.Linq;
using Xunit;
using Moq;
using GirafRest.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using GirafRest.Controllers;

namespace GirafRest.Test
{
    public class PictogramControllerTest
    {
        private readonly PictogramController pictogramController;
        private readonly List<string> logs;
        private readonly Mock<IUserStore<GirafUser>> userStore;

        private readonly GirafUser mockUser;


        public PictogramControllerTest()
        {
            mockUser = new GirafUser("Mock User", 0);
            var mockUser2 = new GirafUser("Owner of other privates", 1);

            var pictograms = testSessions();
            var relations = new List<UserResource> () {
                new UserResource(mockUser, pictograms[3]),
                new UserResource(mockUser2, pictograms[4])
            };

            var mockSet = UnitTestExtensions.CreateMockDbSet<Pictogram>(pictograms);
            var mockRelationSet = UnitTestExtensions.CreateMockDbSet<UserResource>(relations);

            var dbMock = new Mock<FakeDbContext> ();
            dbMock.Setup(c => c.Pictograms).Returns(mockSet.Object);
            dbMock.Setup(c => c.UserResources).Returns (mockRelationSet.Object);

            userStore = new Mock<IUserStore<GirafUser>>();
            var umMock = UnitTestExtensions.MockUserManager(userStore);
            var lfMock = UnitTestExtensions.CreateMockLoggerFactory();

            pictogramController = new PictogramController(dbMock.Object, umMock, lfMock.Object);
        }

        [Fact]
        public void GetExistingPublic_NoLogin_ExpectOK()
        {
            userStore.MockLoggedOut();

            Pictogram p = testSessions().Where(pict => pict.AccessLevel == AccessLevel.PUBLIC).First();
            var res = pictogramController.ReadPictogram(p.Id);
            IActionResult aRes = res.Result;

            Assert.IsType<OkObjectResult>(aRes);
        }

        [Fact]
        public void GetExistingPrivate_NoLogin_ExpectUnauthorized() {
            userStore.MockLoggedOut();

            Pictogram p = testSessions().Where(pict => pict.AccessLevel == AccessLevel.PRIVATE).First();
            var res = pictogramController.ReadPictogram(p.Id);
            IActionResult aRes = res.Result;

            Assert.IsType<UnauthorizedResult>(aRes);
        }

        [Fact]
        public void GetExistingProtected_NoLogin_ExpectUnauthorized() {
            userStore.MockLoggedOut();

            Pictogram p = testSessions().Where(pict => pict.AccessLevel == AccessLevel.PROTECTED).First();
            var res = pictogramController.ReadPictogram(p.Id);
            IActionResult aRes = res.Result;

            Assert.IsType<UnauthorizedResult>(aRes);
        }

        [Fact]
        public void GetExistingPrivate_Login_ExpectOK() {
            userStore.MockLoginAsUser(mockUser);

            var res = pictogramController.ReadPictogram(3);
            IActionResult aRes = res.Result;

            Assert.IsType<OkObjectResult>(aRes);
        }

        [Fact]
        public void GetExistingProtectedInDepartment_Login_ExpectOK() {

        }

        [Fact]
        public void GetExistingProtectedInDepartment_Login_ExpectUnauthorized() {

        }

        [Fact]
        public void GetExistingPrivateAnotherUser_Login_ExpectUnauthorized() {

        }

        [Fact]
        public void GetNonexistingPictogram_Login_ExpectNotFound() {

        }

        [Fact]
        public void GetNonexistingPictogram_NoLogin_ExpectNotFound() {

        }

        public List<Pictogram> testSessions() {
            var sessions = new List<Pictogram> {
                new Pictogram("Public Picto1", AccessLevel.PUBLIC),
                new Pictogram("Public Picto2", AccessLevel.PUBLIC),
                new Pictogram("No restrictions", AccessLevel.PUBLIC),
                new Pictogram("Restricted", AccessLevel.PRIVATE),
                new Pictogram("Private Pictogram", AccessLevel.PRIVATE),
                new Pictogram("Protected Pictogram", AccessLevel.PROTECTED)
            };
            
            return sessions;
        }
    }
}
