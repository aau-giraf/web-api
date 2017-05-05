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

namespace GirafRest.Test
{
    public class WeekControllerTest
    {
        private readonly ITestOutputHelper _testLogger;
        private TestContext _testContext;

        private const int USER = 0;
        private const int OTHER_USER = 1;
        private const int NO_WEEK_USER = 2;
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

        [Fact]
        public void AccessUserWeeks_Expect200OK()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[USER]);

            var res = wc.ReadWeekSchedules();
            IActionResult aRes = res.Result;

            Assert.IsType<OkObjectResult>(aRes);
        }        
       
       [Fact]
        public void AccessSpecificUserWeek_Expect200OK()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[USER]);

            var res = wc.ReadUsersWeekSchedule(WEEK_ZERO);
            IActionResult aRes = res.Result;
            
            Assert.IsType<OkObjectResult>(aRes);
        }
        
        [Fact]
        public void UpdateDayInWeek_Expect200OK()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[USER]);
            var day = _testContext.MockUsers[OTHER_USER].WeekSchedule.First().Weekdays.First();
            var tempWeek = _testContext.MockUsers[USER].WeekSchedule;
            
            var res = wc.UpdateDay(DAY_ZERO, new WeekdayDTO(day));
            IActionResult aRes = res.Result;

            Assert.IsType<OkObjectResult>(aRes);
            _testContext.MockUsers[USER].WeekSchedule = tempWeek;
        }


        [Fact]
        public void UpdateWeek_Expect200OK()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[USER]);
            var week = _testContext.MockUsers[OTHER_USER].WeekSchedule.First();
            var tempWeek = _testContext.MockUsers[USER].WeekSchedule;

            var res = wc.UpdateWeek(WEEK_ZERO, new WeekDTO(week));
            IActionResult aRes = res.Result;

            Assert.IsType<OkObjectResult>(aRes);

            _testContext.MockUsers[USER].WeekSchedule = tempWeek;
        }

        [Fact]
        public void CreateWeek_Expect200OK()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[USER]);
            var week = _testContext.MockUsers[OTHER_USER].WeekSchedule.First();

            var res = wc.CreateWeek(new WeekDTO(week));
            IActionResult aRes = res.Result;

            Assert.IsType<OkObjectResult>(aRes);

            _testContext.MockUsers[USER].WeekSchedule.Remove(_testContext.MockUsers[USER].WeekSchedule.Last());
        }

        [Fact]
        public void AccessEmptyUserWeeks_ExpectNotFound()
        {
            var wc = initializeTest();
            _testContext.MockUsers[NO_WEEK_USER].WeekSchedule.Clear();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[NO_WEEK_USER]);

            var res = wc.ReadWeekSchedules();
            IActionResult aRes = res.Result;

            Assert.IsType<NotFoundResult>(aRes);
        }        
       
       [Fact]
        public void AccessSpecificEmptyUserWeek_ExpectNotFound()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[NO_WEEK_USER]);

            var res = wc.ReadUsersWeekSchedule(NONEXISTING);
            IActionResult aRes = res.Result;
            
            Assert.IsType<NotFoundResult>(aRes);
        }
        
        [Fact]
        public void UpdateDayInWeekWithNull_ExpectBadRequest()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[USER]);
            
            var res = wc.UpdateDay(DAY_ZERO, null);
            IActionResult aRes = res.Result;

            Assert.IsType<BadRequestObjectResult>(aRes);
        }

        [Fact]
        public void UpdateWeekWithNull_ExpectBadRequest()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[USER]);

            var res = wc.UpdateWeek(WEEK_ZERO, null);
            IActionResult aRes = res.Result;

            Assert.IsType<BadRequestObjectResult>(aRes);
        }

        [Fact]
        public void CreateWeekWithNull_ExpectBadRequest()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[USER]);

            var res = wc.CreateWeek(null);
            IActionResult aRes = res.Result;

            Assert.IsType<BadRequestObjectResult>(aRes);
        }

        [Fact]
        public void UpdateDayInNonExistingWeek_ExpectNotFound()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[USER]);
            var day = _testContext.MockUsers[OTHER_USER].WeekSchedule.First().Weekdays.First();
            
            var res = wc.UpdateDay(NONEXISTING, new WeekdayDTO(day));
            IActionResult aRes = res.Result;

            Assert.IsType<NotFoundResult>(aRes);
        }

        [Fact]
        public void UpdateNonExistingWeek_ExpectNotFound()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[USER]);
            var week = _testContext.MockUsers[OTHER_USER].WeekSchedule.First();

            var res = wc.UpdateWeek(NONEXISTING, new WeekDTO(week));
            IActionResult aRes = res.Result;

            Assert.IsType<NotFoundResult>(aRes);
        }
    }
}
