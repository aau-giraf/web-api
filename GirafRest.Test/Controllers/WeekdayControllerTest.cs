using GirafRest.Controllers;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using GirafRest.Models.Responses;
using GirafRest.Test.Mocks;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;
using static GirafRest.Test.UnitTestExtensions;

namespace GirafRest.Test.Controllers
{
    public class WeekdayControllerTest
    {
        private readonly ITestOutputHelper _testLogger;
        private TestContext _testContext;

        private const int USER_0_DAY = 0;
        private const int USER_0 = 0;
        private const int PUBLIC_PICTOGRAM = 0;

        //public List<GirafUser> users; // UNUSED
        public WeekdayControllerTest(ITestOutputHelper output)
        {
            _testLogger = output;
        }
        private DayController initializeTest()
        {
            _testContext = new TestContext();

            var wc = new DayController(
                new MockGirafService(_testContext.MockDbContext.Object,
                _testContext.MockUserManager), _testContext.MockLoggerFactory.Object);
            _testContext.MockHttpContext = wc.MockHttpContext();

            return wc;
        }

        #region UpdateDay
        [Fact]
        public void UpdateDay_InExistingWeek_Ok()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[USER_0]);
            var day = _testContext.MockWeeks[USER_0_DAY].Weekdays[(int)DayOfWeek.Monday];
            day.Elements.Add(new WeekdayResource(day, _testContext.MockPictograms[PUBLIC_PICTOGRAM]));

            var res = wc.UpdateDay(day.Id, new WeekdayDTO(day)).Result;

            Assert.True(res.Success);
        }

        [Fact]
        public void UpdateDay_InNonExistingWeek_NotFound()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[USER_0]);
            var day = _testContext.MockWeeks[USER_0_DAY].Weekdays[(int)DayOfWeek.Monday];
            day.Elements.Add(new WeekdayResource(day, _testContext.MockPictograms[PUBLIC_PICTOGRAM]));

            var res = wc.UpdateDay(999, new WeekdayDTO(day));
            var aRes = res.Result;

            Assert.False(aRes.Success);
            Assert.Equal(ErrorCode.WeekScheduleNotFound, aRes.ErrorCode);
        }

        [Fact]
        public void UpdateDay_InExistingWeekNullDTO_NotFound()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[USER_0]);

            var res = wc.UpdateDay(USER_0_DAY, null);
            var aRes = res.Result;

            Assert.False(aRes.Success);
            Assert.Equal(ErrorCode.FormatError, aRes.ErrorCode);
        }
        #endregion
    }
}