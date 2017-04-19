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
using System;

namespace GirafRest.Test
{
    public class PictogramControllerTest
    {
        private readonly PictogramController pictogramController;
        private readonly List<string> logs;
        private readonly FakeUserManager userManager;

        private readonly GirafUser mockUser;

        public PictogramControllerTest()
        {
            var dbMock = UnitTestExtensions.CreateMockDbContext();
            var userStore = new Mock<IUserStore<GirafUser>>();
            userManager = UnitTestExtensions.MockUserManager(userStore);
            var lfMock = UnitTestExtensions.CreateMockLoggerFactory();

            pictogramController = new PictogramController(dbMock.Object, userManager, lfMock.Object);
        }

        [Fact]
        public void GetExistingPublic_NoLogin_ExpectOK()
        {
            userManager.MockLogout();

            Pictogram p = UnitTestExtensions.MockPictograms.Where(pict => pict.AccessLevel == AccessLevel.PUBLIC).First();
            var res = pictogramController.ReadPictogram(p.Id);
            IActionResult aRes = res.Result;

            Assert.IsType<OkObjectResult>(aRes);
        }

        [Fact]
        public void GetExistingPrivate_NoLogin_ExpectUnauthorized() {
            userManager.MockLogout();

            Pictogram p = UnitTestExtensions.MockPictograms.Where(pict => pict.AccessLevel == AccessLevel.PRIVATE).First();
            var res = pictogramController.ReadPictogram(p.Id);
            IActionResult aRes = res.Result;

            Assert.IsType<UnauthorizedResult>(aRes);
        }

        [Fact]
        public void GetExistingProtected_NoLogin_ExpectUnauthorized() {
            try {
                userManager.MockLogout();
                
                var res = pictogramController.ReadPictogram(5);
                IActionResult aRes = res.Result;

                Assert.IsType<UnauthorizedResult>(aRes);
            }
            catch (Exception e){
                Assert.True(false, $"The method threw an exception: {e.Message}");
            }
        }

        [Fact]
        public void GetExistingPrivate_Login_ExpectOK() {
            try {
                userManager.MockLoginAsUser(mockUser);

                var res = pictogramController.ReadPictogram(3);
                IActionResult aRes = res.Result;

                Assert.IsType<OkObjectResult>(aRes);
            }
            catch (Exception e) {
                Assert.True(false, $"The method threw an exception: {e.Message}");
            }
        }

        [Fact]
        public void GetExistingProtectedInDepartment_Login_ExpectOK() {
            try
            {
                userManager.MockLoginAsUser(UnitTestExtensions.MockUsers[0]);
                var tRes = pictogramController.ReadPictogram(5);
                var res = tRes.Result;

                Assert.IsType<OkObjectResult>(res);
            }
            catch (Exception e)
            {
                Assert.True(false);
            }
        }

        [Fact]
        public void GetExistingProtectedInDepartment_Login_ExpectUnauthorized() {
            try
            {

            }
            catch (Exception e)
            {

                throw;
            }
        }

        [Fact]
        public void GetExistingPrivateAnotherUser_Login_ExpectUnauthorized() {

        }

        [Fact]
        public void GetNonexistingPictogram_Login_ExpectNotFound() {
            userManager.MockLoginAsUser(mockUser);

            var res = pictogramController.ReadPictogram(999);
            var pRes = res.Result;
            Assert.IsType<NotFoundObjectResult>(pRes);
        }

        [Fact]
        public void GetNonexistingPictogram_NoLogin_ExpectNotFound() {

        }
    }
}
