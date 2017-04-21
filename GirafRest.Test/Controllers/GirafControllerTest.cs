using System.Collections.Generic;
using GirafRest.Data;
using GirafRest.Models;
using GirafRest.Test.Mocks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GirafRest.Test.Controllers
{
    public class GirafControllerTest
    {
        private readonly Mock<GirafDbContext> dbContextMock;
        private readonly Mock<IUserStore<GirafUser>> userStore;
        private readonly MockUserManager umMock;
        private readonly Mock<ILoggerFactory> lfMock;
        private readonly List<string> logs;
        private readonly Mock<MockDbContext> dbMock;

        public GirafControllerTest()
        {

        }

        [Fact]
        public void LoadUserAsync_ExpectSuccess()
        {

        }

        [Fact]
        public void LoadUserAsync_ExpectFail()
        {

        }

        [Fact]
        public void ReadRequestImage_ExpectSuccess()
        {

        }

        [Fact]
        public void ReadRequestImage_ExpectFail()
        {

        }

        [Fact]
        public void CheckResourceOwnership_ExpectTrue()
        {

        }

        [Fact]
        public void CheckResourceOwnership_ExpectFalse()
        {

        }
    }
}