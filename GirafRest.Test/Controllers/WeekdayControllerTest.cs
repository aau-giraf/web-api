using GirafRest.Controllers;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using GirafRest.Models.Responses;
using GirafRest.Test.Mocks;
using System;
using System.Linq;
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
            day.Activities.Add(new WeekdayResource(day, _testContext.MockPictograms[PUBLIC_PICTOGRAM], 0));
            var res = wc.UpdateDay(day.Id, new WeekdayDTO(day)).Result;

            Assert.IsType<Response<WeekDTO>>(res);
            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            Assert.True(res.Success);
            Assert.True(res.Data?.Days?.FirstOrDefault(d => d.Day == day.Day)
                        ?.Activities
                        .Any(el => el.Pictogram.Title == _testContext.MockPictograms[PUBLIC_PICTOGRAM].Title));
        }

        [Fact]
        public void UpdateDay_InNonExistingWeek_NotFound()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[USER_0]);
            var day = _testContext.MockWeeks[USER_0_DAY].Weekdays[(int)DayOfWeek.Monday];
            day.Activities.Add(new WeekdayResource(day, _testContext.MockPictograms[PUBLIC_PICTOGRAM], 0));
            var res = wc.UpdateDay(999, new WeekdayDTO(day)).Result;

            Assert.IsType<ErrorResponse<WeekDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.WeekScheduleNotFound, res.ErrorCode);
        }

        [Fact]
        public void UpdateDay_InExistingWeekNullDTO_NotFound()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[USER_0]);
            var res = wc.UpdateDay(USER_0_DAY, null).Result;

            Assert.IsType<ErrorResponse<WeekDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.FormatError, res.ErrorCode);
        }
        #endregion
    }
}