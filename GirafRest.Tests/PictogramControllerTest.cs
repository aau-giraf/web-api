using System;
using Xunit;
using Moq;
using System.Collections.Generic;
using GirafRest.Models;
using GirafRest.Controllers;
using GirafRest.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace GirafRest.Tests
{
    public class PictogramControllerTest
    {
        private readonly PictogramController Controller;

        public PictogramControllerTest()
        {
            var mockContext = new Mock<GirafDbContext> ();
            mockContext.Setup(c => addTestSessions(c));
            
            var mockUserManager = new Mock<UserManager<GirafUser>>();
            mockUserManager.Setup(um => addUsers(um));

            var mockHostingEnvironment = new Mock<IHostingEnvironment>();

            var mockLogger = new Mock<ILoggerFactory>();

            Controller = new PictogramController(mockContext.Object, mockUserManager.Object, mockHostingEnvironment.Object, mockLogger.Object);
        }

        [Fact]
        public void Get_NoLogin_ExpectPublicPictograms()
        {
            var result = Controller.ReadPictograms();
            result.
        }

        private void addTestSessions(GirafDbContext context) {
            var sessions = new List<PictoFrame> {
                new Pictogram("Public Picto1", AccessLevel.PUBLIC),
                new Pictogram("Public Picto2", AccessLevel.PUBLIC),
                new Pictogram("No restrictions", AccessLevel.PUBLIC),
                new Pictogram("Restricted", AccessLevel.PRIVATE),
                new Pictogram("Private Pictogram", AccessLevel.PRIVATE)
            };
            foreach(var p in sessions) {
                context.Add(p);
            }
            context.SaveChanges();
        }
        private void addUsers(UserManager<GirafUser> userManager) {
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
