﻿using GirafRest.Models;
using GirafRest.Services;
using Xunit;
using static GirafRest.Test.UnitTestExtensions;
using System.Linq;
using System.IO;
using System;

namespace GirafRest.Test.Services
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
        public void ReadRequestImage_ValidImage_InputArrayEqualsOutputArray()
        {
            var gs = initializeTest();
            var testArray = new byte[] { 0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26 };
            Stream byteStream = new MemoryStream(testArray);

            var res = gs.ReadRequestImage(byteStream).Result;

            Assert.Equal(testArray, res);
        }

        [Fact]
        public void ReadRequestImage_NullImage_BadRequest()
        {
            var gs = initializeTest();

            Assert.ThrowsAsync(typeof(ArgumentNullException), () => gs.ReadRequestImage(null));
        }
        #endregion
        #region CheckPrivateOwnership
        [Fact]
        public void CheckPrivateOwnership_OwnResource_True()
        {
            var gs = initializeTest();
            var user = _testContext.MockUsers[ADMIN_DEP_ONE];

            var res = gs.CheckPrivateOwnership((Pictogram)_testContext.MockUsers[ADMIN_DEP_ONE].Resources.First().Pictogram, user);

            Assert.True(res.Result);
        }

        [Fact]
        public void CheckPrivateOwnership_DoNotOwnResource_False()
        {
            var gs = initializeTest();
            var user = _testContext.MockUsers[ADMIN_DEP_ONE];

            var res = gs.CheckPrivateOwnership((Pictogram)_testContext.MockUsers[GUARDIAN_DEP_TWO].Resources.First().Pictogram, user);

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

            var res = gs.CheckProtectedOwnership((Pictogram)userDepartment.Resources.First().Pictogram, user);

            Assert.True(res.Result);
        }

        [Fact]
        public void CheckProtectedOwnership_DoNotOwnResource_False()
        {
            var gs = initializeTest();
            var user = _testContext.MockUsers[ADMIN_DEP_ONE];
            var notUserDepartment = _testContext.MockDepartments.Where(d => d.Key != user.DepartmentKey).First();

            var res = gs.CheckProtectedOwnership((Pictogram)notUserDepartment.Resources.First().Pictogram, user);

            Assert.False(res.Result);
        }
        #endregion
    }
}