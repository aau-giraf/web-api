using System;
using System.Linq;
using Xunit;
using Moq;
using GirafRest.Models;
using GirafRest.Controllers;
using GirafRest.Data;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace GirafRest.Test
{
    public class PictogramControllerTest
    {
        private readonly PictogramController pictogramController;
        private readonly Mock<GirafDbContext> dbMock;
        private readonly Mock<UserManager<GirafUser>> umMock;
        private readonly Mock<ILoggerFactory> lfMock;
        private readonly List<string> logs;


        public PictogramControllerTest()
        {
            dbMock = new Mock<GirafDbContext> ();
            dbMock.Setup(c => c.Pictograms.AddRange(testSessions()));
            dbMock.Setup(c => c.SaveChanges());

            umMock = new Mock<UserManager<GirafUser>>();
            umMock.Setup(um => addUsers(um));

            logs = new List<string>();
            var lMock = new Mock<ILogger>();
            lMock.Setup(x => x.LogInformation(It.IsAny<string>())).Callback((string s) => logs.Add(s));
            lMock.Setup(x => x.LogError(It.IsAny<string>())).Callback((string s) => logs.Add(s));

            lfMock = new Mock<ILoggerFactory>();
            lfMock.Setup(lf => lf.CreateLogger(It.IsAny<string>())).Returns(lMock.Object);

            pictogramController = new PictogramController(dbMock.Object, umMock.Object, lfMock.Object);
        }

        [Fact]
        public void AccessExistingPublic_Expect200OK()
        {
            Pictogram p = testSessions().Where(pict => pict.AccessLevel == AccessLevel.PUBLIC).First();
            var res = pictogramController.ReadPictogram((int) p.Id);
            IActionResult aRes = res.Result;

            Assert.IsType<JsonResult>(aRes);
            var jRes = aRes as JsonResult;
            Assert.True(jRes.StatusCode == 400);
        }

        public List<Pictogram> testSessions() {
            var sessions = new List<Pictogram> {
                new Pictogram("Public Picto1", AccessLevel.PUBLIC),
                new Pictogram("Public Picto2", AccessLevel.PUBLIC),
                new Pictogram("No restrictions", AccessLevel.PUBLIC),
                new Pictogram("Restricted", AccessLevel.PRIVATE),
                new Pictogram("Private Pictogram", AccessLevel.PRIVATE)
            };
            
            return sessions;
        }
        public virtual void addUsers(UserManager<GirafUser> userManager) {
            var users = new List<GirafUser> {
                new GirafUser("Alice", 1),
                new GirafUser("Bob", 2),
                new GirafUser("Morten", 1),
                new GirafUser("Brian", 2)
            };

            foreach (var u in users) {
                userManager.CreateAsync(u, "mocking");
            }
        }
    }
}
