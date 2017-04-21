using System.Linq;
using Xunit;
using Moq;
using GirafRest.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using GirafRest.Controllers;
using System;
using Xunit.Abstractions;
using GirafRest.Test.Mocks;
using static GirafRest.Test.UnitTestExtensions;
using Microsoft.Extensions.Logging;

namespace GirafRest.Test
{
    public class PictogramControllerTest
    {
        private readonly PictogramController pictogramController;
        private readonly List<string> logs;
        private readonly MockUserManager userManager;
        
        private readonly ITestOutputHelper testLogger;

        public PictogramControllerTest(ITestOutputHelper output)
        {
            testLogger = output;
            var dbMock = CreateMockDbContext();
            var userStore = new Mock<IUserStore<GirafUser>>();
            userManager = MockUserManager(userStore);
            var lfMock = CreateMockLoggerFactory();
            var lMock = new Mock<ILogger>();
            /*lMock.Setup(l => l.LogError(It.IsAny<string>()))
                .Callback((string s) => output.WriteLine(s));*/

            pictogramController = new PictogramController(new MockGirafService(dbMock.Object, userManager), lfMock.Object);
        }

        [Fact]
        public void GetExistingPublic_NoLogin_ExpectOK()
        {
            userManager.MockLogout();

            Pictogram p = MockPictograms.Where(pict => pict.AccessLevel == AccessLevel.PUBLIC).First();
            var res = pictogramController.ReadPictogram(p.Id);
            IActionResult aRes = res.Result;

            Assert.IsType<OkObjectResult>(aRes);
        }

        [Fact]
        public void GetExistingPrivate_NoLogin_ExpectUnauthorized() {
            userManager.MockLogout();

            var res = pictogramController.ReadPictogram(3);
            IActionResult aRes = res.Result;

            //testLogger.WriteLine(((aRes as OkObjectResult).Value as PictogramDTO).Title);

            Assert.IsType<UnauthorizedResult>(aRes);
        }

        [Fact]
        public void GetExistingProtected_NoLogin_ExpectUnauthorized() {
            try {
                userManager.MockLogout();
                
                var res = pictogramController.ReadPictogram(5);
                IActionResult aRes = res.Result;

                
                Console.WriteLine((aRes as OkObjectResult).Value);

                Assert.IsType<UnauthorizedResult>(aRes);
            }
            catch (Exception e){
                Assert.True(false, $"The method threw an exception: {e.Message}");
            }
        }

        [Fact]
        public void GetExistingPrivate_Login_ExpectOK() {
            try {
                userManager.MockLoginAsUser(MockUsers[0]);

                var res = pictogramController.ReadPictogram(3);
                IActionResult aRes = res.Result;

                Assert.IsType<OkObjectResult>(aRes);
            }
            catch (Exception e) {
                Assert.True(false, $"The method threw an exception: {e.Message}, {e.Source}");
            }
        }

        [Fact]
        public void GetExistingProtectedInDepartment_Login_ExpectOK() {
            try
            {
                userManager.MockLoginAsUser(MockUsers[0]);
                var tRes = pictogramController.ReadPictogram(5);
                var res = tRes.Result;

                Assert.IsType<OkObjectResult>(res);
            }
            catch (Exception e)
            {
                Assert.True(false, $"The method threw an exception: {e.Message}");
            }
        }

        [Fact]
        public void GetExistingProtectedInDepartment_Login_ExpectUnauthorized() {
            try
            {
                userManager.MockLoginAsUser(MockUsers[0]);
                var tRes = pictogramController.ReadPictogram(6);
                var res = tRes.Result;

                Assert.IsType<OkObjectResult>(res);
            }
            catch (Exception e)
            {
                Assert.True(false, $"The method threw an exception: {e.Message}");
            }
        }

        [Fact]
        public void GetExistingPrivateAnotherUser_Login_ExpectUnauthorized() {
            try
            {
                userManager.MockLoginAsUser(MockUsers[0]);
                var tRes = pictogramController.ReadPictogram(4);
                var res = tRes.Result;

                Assert.IsType<OkObjectResult>(res);
            }
            catch (Exception e)
            {
                Assert.True(false, $"The method threw an exception: {e.Message}");
            }
        }

        [Fact]
        public void GetNonexistingPictogram_Login_ExpectNotFound() {
            userManager.MockLoginAsUser(MockUsers[0]);

            var res = pictogramController.ReadPictogram(999);
            var pRes = res.Result;
            Assert.IsAssignableFrom<NotFoundResult>(pRes);
        }

        [Fact]
        public void GetNonexistingPictogram_NoLogin_ExpectNotFound() {

        }
    }
}
