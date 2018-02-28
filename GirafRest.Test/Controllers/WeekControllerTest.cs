using System;
using System.Linq;
using Xunit;
using Moq;
using GirafRest.Models;
using GirafRest.Controllers;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using GirafRest.Test.Mocks;
using static GirafRest.Test.UnitTestExtensions;
using GirafRest.Models.DTOs;
using System.IO;
using Xunit.Abstractions;
using GirafRest.Models.Responses;

namespace GirafRest.Test
{
    public class WeekControllerTest
    {
        private readonly ITestOutputHelper _testLogger;
        private TestContext _testContext;

        private const int ADMIN_DEP_ONE = 0;
        private const int GUARDIAN_DEP_TWO = 1;
        private const int CITEZEN_DEP_THREE = 3; // Have no week
        private const int WEEK_ZERO = 0;
        private const int DAY_ZERO = 0;
        private const int NONEXISTING = 999;

        public List<GirafUser> users;
        public WeekControllerTest(ITestOutputHelper output)
        {
            _testLogger = output;
        }
        private WeekController initializeTest()
        {
            _testContext = new TestContext();

            var wc = new WeekController(
                new MockGirafService(_testContext.MockDbContext.Object,
                _testContext.MockUserManager), _testContext.MockLoggerFactory.Object);
            _testContext.MockHttpContext = wc.MockHttpContext();

            return wc;
        }

        #region ReadWeekSchedules
        [Fact]
        public void ReadWeekSchedules_AccessUsersWeeks_Ok()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);

            var res = wc.ReadWeekSchedules();
            var aRes = res.Result;

            Assert.True(aRes.Success);
        }

        [Fact]
        public void ReadWeekSchedules_AccessUsersWeeksNoWeeksExist_NotFound()
        {
            var wc = initializeTest();
            _testContext.MockUsers[CITEZEN_DEP_THREE].WeekSchedule.Clear();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITEZEN_DEP_THREE]);

            var res = wc.ReadWeekSchedules().Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.NoWeekScheduleFound, res.ErrorCode);
        }
        #endregion
        #region ReadWeekSchedule(id)
        [Fact]
        public void ReadWeekSchedules_AccessValidUsersSpecificWeek_Ok()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);

            var res = wc.ReadUsersWeekSchedule(WEEK_ZERO);
            var aRes = res.Result;
            
            Assert.True(aRes.Success);
        }

        [Fact]
        public void ReadWeekSchedules_AccessValidUsersSpecificWeekNoWeekExist_NotFound()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITEZEN_DEP_THREE]);

            var res = wc.ReadUsersWeekSchedule(NONEXISTING).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.WeekScheduleNotFound, res.ErrorCode);
        }
        #endregion
        #region UpdateWeek
        [Fact]
        public void UpdateWeek_ValidWeekValidDTO_Ok()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var week = _testContext.MockUsers[GUARDIAN_DEP_TWO].WeekSchedule.First();
            var tempWeek = _testContext.MockUsers[ADMIN_DEP_ONE].WeekSchedule;

            var res = wc.UpdateWeek(WEEK_ZERO, new WeekDTO(week));
            var aRes = res.Result;

            Assert.True(aRes.Success);

            _testContext.MockUsers[ADMIN_DEP_ONE].WeekSchedule = tempWeek;
        }

        [Fact]
        public void UpdateWeek_InvalidWeekValidDTO_NotFound()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var week = _testContext.MockUsers[GUARDIAN_DEP_TWO].WeekSchedule.First();
            var tempWeek = _testContext.MockUsers[ADMIN_DEP_ONE].WeekSchedule;

            var res = wc.UpdateWeek(NONEXISTING, new WeekDTO(week)).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.WeekScheduleNotFound, res.ErrorCode);

            _testContext.MockUsers[ADMIN_DEP_ONE].WeekSchedule = tempWeek;
        }

        [Fact]
        public void UpdateWeek_ValidWeekInvalidDTO_BadRequest()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var tempWeek = _testContext.MockUsers[ADMIN_DEP_ONE].WeekSchedule;

            var res = wc.UpdateWeek(WEEK_ZERO, new WeekDTO()).Result;

            Assert.False(res.Success);
            Assert.IsType<ErrorResponse<WeekDTO>>(res);
            Assert.Equal(ErrorCode.MissingProperties, res.ErrorCode);

            _testContext.MockUsers[ADMIN_DEP_ONE].WeekSchedule = tempWeek;
        }

        [Fact]
        public void UpdateWeek_ValidWeekNullDTO_BadRequest()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);

            var res = wc.UpdateWeek(WEEK_ZERO, null);
            var aRes = res.Result;

            Assert.False(aRes.Success);
        }
        #endregion
        #region CreateWeek
        [Fact]
        public void CreateWeek_NewWeekValidDTO_Ok()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var week = _testContext.MockUsers[GUARDIAN_DEP_TWO].WeekSchedule.First();

            var res = wc.CreateWeek(new WeekDTO(week));
            var aRes = res.Result;

            Assert.True(aRes.Success);

            _testContext.MockUsers[ADMIN_DEP_ONE].WeekSchedule.Remove(_testContext.MockUsers[ADMIN_DEP_ONE].WeekSchedule.Last());
        }

        [Fact]
        public void CreateWeek_NewWeekInvalidDTO_BadRequest()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);

            var res = wc.CreateWeek(new WeekDTO()).Result;

            Assert.False(res.Success);

            _testContext.MockUsers[ADMIN_DEP_ONE].WeekSchedule.Remove(_testContext.MockUsers[ADMIN_DEP_ONE].WeekSchedule.Last());
        }

        [Fact]
        public void CreateWeek_NewWeekNullDTO_BadRequest()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);

            var res = wc.CreateWeek(null);
            var aRes = res.Result;

            Assert.False(aRes.Success);
        }
        #endregion
    }
}
