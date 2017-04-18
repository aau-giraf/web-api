using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GirafRest.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace GirafRest.Test
{
    public static class UnitTestExtensions
    {
        public static Mock<DbSet<T>> CreateMockDbSet<T>(List<T> dataList) 
            where T : class
        {
            IQueryable<T> data = dataList.AsQueryable();
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IAsyncEnumerable<T>>()
                .Setup(m => m.GetEnumerator())
                .Returns(new TestDbAsyncEnumerator<T>(data.GetEnumerator()));

            mockSet.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<T>(data.Provider));
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns (data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            return mockSet;
        }

        public static UserManager<GirafUser> MockUserManager(Mock<IUserStore<GirafUser>> userStore) {
            var umMock = new UserManager<GirafUser> (userStore.Object, null, null, null, null, null, null, null, null);
            //iUS.Setup(x => x.CreateAsync(mockUser, new CancellationToken())).Returns(Task.FromResult(IdentityResult.Success));

            return umMock;
        }

        public static void MockLoginAsUser(this Mock<IUserStore<GirafUser>> userStore, GirafUser user) {
            userStore.Setup(x => x.FindByNameAsync(It.IsAny<string>(), new CancellationToken())).Returns(Task.FromResult(user));
            userStore.Setup(x => x.FindByIdAsync(It.IsAny<string>(), new CancellationToken())).Returns(Task.FromResult(user));
        }

        public static void MockLoggedOut(this Mock<IUserStore<GirafUser>> userStore) {
            userStore.Setup(x => x.FindByNameAsync(It.IsAny<string>(), new CancellationToken())).Returns(Task.FromResult<GirafUser>(null));
            userStore.Setup(x => x.FindByIdAsync(It.IsAny<string>(), new CancellationToken())).Returns(Task.FromResult<GirafUser>(null));
        }

        public static Mock<ILoggerFactory> CreateMockLoggerFactory() {
            var logs = new List<string>();
            var lMock = new Mock<ILogger>();

            var lfMock = new Mock<ILoggerFactory>();
            lfMock.Setup(lf => lf.CreateLogger(It.IsAny<string>())).Returns(lMock.Object);

            return lfMock;
        }
    }
}