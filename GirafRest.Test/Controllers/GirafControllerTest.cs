using System.Collections.Generic;
using GirafRest.Test.Mocks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using GirafRest.Data;
using GirafRest.Models;
using GirafRest.Controllers;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using static GirafRest.Test.UnitTestExtensions;
using System.Linq;
using System.IO;

namespace GirafRest.Test.Controllers
{
    public class GirafControllerTest
    {
        private readonly MockUserManager userManager;
        private readonly Mock<MockDbContext> dbMock;
        private readonly GirafService giraf;
        private TestContext _testContext;

        public GirafControllerTest()
        {
            /*var dbMock = CreateMockDbContext();

            var userStore = new Mock<IUserStore<GirafUser>>();
            userManager = MockUserManager(userStore);

            var lfMock = CreateMockLoggerFactory();

            giraf = new GirafController(dbMock.Object, userManager);*/
        }
        private GirafService initializeTest()
        {
            _testContext = new TestContext();

            var gs = new GirafService(
                    _testContext.MockDbContext.Object,
                    _testContext.MockUserManager);

            return gs;
        }

        [Fact]
        public void ReadRequestImage_ExpectSuccess()
        {
            var gs = initializeTest();
            var testArray = new byte[] { 0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26 };
            Stream byteStream = new MemoryStream(testArray);

            var res = gs.ReadRequestImage(byteStream).Result;

            Assert.Equal(testArray, res);
        }
        
        [Fact]
        public void CheckPrivateOwnership_ExpectTrue()
        {
            var gs = initializeTest();
            var user = _testContext.MockUsers[0];

            var res = gs.CheckPrivateOwnership((Pictogram)_testContext.MockUsers[0].Resources.First().Resource, user);

            Assert.True(res.Result);
        }

        [Fact]
        public void CheckPrivateOwnership_ExpectFalse()
        {
            var gs = initializeTest();
            var user = _testContext.MockUsers[0];

            var res = gs.CheckPrivateOwnership((Pictogram)_testContext.MockUsers[1].Resources.First().Resource, user);

            Assert.False(res.Result);
        }

        [Fact]
        public void CheckProtectedOwnership_ExpectTrue()
        {
            var gs = initializeTest();
            var user = _testContext.MockUsers[0];
            var userDepartment = _testContext.MockDepartments.Where(d => d.Key == user.DepartmentKey).First();

            var res = gs.CheckProtectedOwnership((Pictogram)userDepartment.Resources.First().Resource, user);

            Assert.True(res.Result);
        }

        [Fact]
        public void CheckProtectedOwnership_ExpectFalse()
        {
            var gs = initializeTest();
            var user = _testContext.MockUsers[0];
            var notUserDepartment = _testContext.MockDepartments.Where(d => d.Key != user.DepartmentKey).First();

            var res = gs.CheckProtectedOwnership((Pictogram)notUserDepartment.Resources.First().Resource, user);

            Assert.False(res.Result);
        }
    }
}