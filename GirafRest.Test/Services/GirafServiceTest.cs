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
    public class GirafServiceTest
    {
        private TestContext _testContext;

        private const int ADMIN_DEP_ONE = 0;
        private const int GUARDIAN_DEP_TWO = 1;

        public GirafServiceTest()
        {

        }
        private GirafService initializeTest()
        {
            _testContext = new TestContext();

            var gs = new GirafService(
                    _testContext.MockDbContext.Object,
                    _testContext.MockUserManager);

            return gs;
        }

        #region ReadRequestImage
        [Fact]
        public void ReadRequestImage_ValidImage_Ok()
        {
            var gs = initializeTest();
            var testArray = new byte[] { 0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26 };
            Stream byteStream = new MemoryStream(testArray);

            var res = gs.ReadRequestImage(byteStream).Result;

            Assert.Equal(testArray, res);
        }

        [Fact(Skip = "Not implemented yet!")]
        public void ReadRequestImage_NullImage_BadRequest()
        {
            var gs = initializeTest();
            var testArray = new byte[] { 0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26 };
            Stream byteStream = new MemoryStream(testArray);

            var res = gs.ReadRequestImage(byteStream).Result;

            Assert.Equal(testArray, res);
        }
        #endregion
        #region CheckPrivateOwnership
        [Fact]
        public void CheckPrivateOwnership_OwnResource_True()
        {
            var gs = initializeTest();
            var user = _testContext.MockUsers[ADMIN_DEP_ONE];

            var res = gs.CheckPrivateOwnership((Pictogram)_testContext.MockUsers[ADMIN_DEP_ONE].Resources.First().Resource, user);

            Assert.True(res.Result);
        }

        [Fact]
        public void CheckPrivateOwnership_DoNotOwnResource_False()
        {
            var gs = initializeTest();
            var user = _testContext.MockUsers[ADMIN_DEP_ONE];

            var res = gs.CheckPrivateOwnership((Pictogram)_testContext.MockUsers[GUARDIAN_DEP_TWO].Resources.First().Resource, user);

            Assert.False(res.Result);
        }
        #endregion
        #region CheckProtectedOwnership
        [Fact]
        public void CheckProtectedOwnership_OwnsResource_True()
        {
            var gs = initializeTest();
            var user = _testContext.MockUsers[ADMIN_DEP_ONE];
            var userDepartment = _testContext.MockDepartments.Where(d => d.Key == user.DepartmentKey).First();

            var res = gs.CheckProtectedOwnership((Pictogram)userDepartment.Resources.First().Resource, user);

            Assert.True(res.Result);
        }

        [Fact]
        public void CheckProtectedOwnership_DoNotOwnResource_False()
        {
            var gs = initializeTest();
            var user = _testContext.MockUsers[ADMIN_DEP_ONE];
            var notUserDepartment = _testContext.MockDepartments.Where(d => d.Key != user.DepartmentKey).First();

            var res = gs.CheckProtectedOwnership((Pictogram)notUserDepartment.Resources.First().Resource, user);

            Assert.False(res.Result);
        }
        #endregion
    }
}