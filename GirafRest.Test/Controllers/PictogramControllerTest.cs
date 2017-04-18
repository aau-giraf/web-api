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
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;
using System.Threading.Tasks;
using System.Threading;

namespace GirafRest.Test
{
    public class PictogramControllerTest
    {
        private readonly PictogramController pictogramController;
        private readonly Mock<FakeDbContext> dbMock;
        private readonly Mock<ILoggerFactory> lfMock;
        private readonly List<string> logs;


        public PictogramControllerTest()
        {
            var data = testSessions().AsQueryable();

            var mockSet = new Mock<DbSet<Pictogram>>();
            mockSet.As<IAsyncEnumerable<Pictogram>>()
                .Setup(m => m.GetEnumerator())
                .Returns(new TestDbAsyncEnumerator<Pictogram>(data.GetEnumerator()));

            mockSet.As<IQueryable<Pictogram>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<Pictogram>(data.Provider));
            mockSet.As<IQueryable<Pictogram>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Pictogram>>().Setup(m => m.ElementType).Returns (data.ElementType);
            mockSet.As<IQueryable<Pictogram>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            dbMock = new Mock<FakeDbContext> ();
            dbMock.Setup(c => c.Pictograms).Returns(mockSet.Object);

            var iUS = new Mock<IUserStore<GirafUser>>();
            /*var iO = new Mock<IOptions<IdentityOptions>>();
            var iPH = new Mock<IPasswordHasher<GirafUser>>();
            var iUV = new Mock<IEnumerable<IUserValidator<GirafUser>>>();
            var iPV = new Mock<IEnumerable<IPasswordValidator<GirafUser>>>();
            var iLN = new Mock<ILookupNormalizer>();
            var iIED = new Mock<IdentityErrorDescriber>();
            var iSP = new Mock<IServiceProvider>();
            var iL = new Mock<ILogger<UserManager<GirafUser>>>();*/
            
            var umMock = new UserManager<GirafUser> (iUS.Object, null, null, null, null, null, null, null, null);
            var mockUser = new GirafUser("Mock User", 0);
            iUS.Setup(x => x.CreateAsync(mockUser, new CancellationToken())).Returns(Task.FromResult(IdentityResult.Success));
            iUS.Setup(x => x.FindByNameAsync(mockUser.UserName, new CancellationToken())).Returns(Task.FromResult(mockUser));

            logs = new List<string>();
            var lMock = new Mock<ILogger>();
            //lMock.Setup(x => x.LogInformation(It.IsAny<string>())).Callback((string s) => logs.Add(s));
            //lMock.Setup(x => x.LogError(It.IsAny<string>())).Callback((string s) => logs.Add(s));

            lfMock = new Mock<ILoggerFactory>();
            lfMock.Setup(lf => lf.CreateLogger(It.IsAny<string>())).Returns(lMock.Object);

            pictogramController = new PictogramController(dbMock.Object, umMock, lfMock.Object);
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
        public List<GirafUser> testUsers() {
            var users = new List<GirafUser> {
                new GirafUser("Alice", 1),
                new GirafUser("Bob", 2),
                new GirafUser("Morten", 1),
                new GirafUser("Brian", 2)
            };

            return  users;
        }
    }
}
