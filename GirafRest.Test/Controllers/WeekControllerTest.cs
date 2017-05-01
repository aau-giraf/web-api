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
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);

            var res = wc.ReadWeekSchedules();
            IActionResult aRes = res.Result;

            Assert.IsType<OkObjectResult>(aRes);
        }        
       
       [Fact]
        public void AccessSpecificUserWeek_Expect200OK()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);

            var res = wc.ReadUsersWeekSchedule(0);
            IActionResult aRes = res.Result;
            
            Assert.IsType<OkObjectResult>(aRes);
        }
        
        [Fact]
        public void UpdateDayInWeek_Expect200OK()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            var day = _testContext.MockUsers[1].WeekSchedule.First().Weekdays.First();
            var tempWeek = _testContext.MockUsers[0].WeekSchedule;
            
            var res = wc.UpdateDay(0, new WeekdayDTO(day));
            IActionResult aRes = res.Result;

            Assert.IsType<OkObjectResult>(aRes);
            _testContext.MockUsers[0].WeekSchedule = tempWeek;
        }


        [Fact]
        public void UpdateWeek_Expect200OK()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            var week = _testContext.MockUsers[1].WeekSchedule.First();
            var tempWeek = _testContext.MockUsers[0].WeekSchedule;

            var res = wc.UpdateWeek(0, new WeekDTO(week));
            IActionResult aRes = res.Result;

            Assert.IsType<OkObjectResult>(aRes);

            _testContext.MockUsers[0].WeekSchedule = tempWeek;
        }

        [Fact]
        public void CreateWeek_Expect200OK()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            var week = _testContext.MockUsers[1].WeekSchedule.First();

            var res = wc.CreateWeek(new WeekDTO(week));
            IActionResult aRes = res.Result;

            Assert.IsType<OkObjectResult>(aRes);

            _testContext.MockUsers[0].WeekSchedule.Remove(_testContext.MockUsers[0].WeekSchedule.Last());
        }

        [Fact]
        public void AccessEmptyUserWeeks_ExpectNotFound()
        {
            var wc = initializeTest();
            _testContext.MockUsers[2].WeekSchedule.Clear();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[2]);

            var res = wc.ReadWeekSchedules();
            IActionResult aRes = res.Result;

            Assert.IsType<NotFoundResult>(aRes);
        }        
       
       [Fact]
        public void AccessSpecificEmptyUserWeek_ExpectNotFound()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[2]);

            var res = wc.ReadUsersWeekSchedule(150);
            IActionResult aRes = res.Result;
            
            Assert.IsType<NotFoundResult>(aRes);
        }
        
        [Fact]
        public void UpdateDayInWeekWithNull_ExpectBadRequest()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            
            var res = wc.UpdateDay(0, null);
            IActionResult aRes = res.Result;

            Assert.IsType<BadRequestObjectResult>(aRes);
        }

        [Fact]
        public void UpdateWeekWithNull_ExpectBadRequest()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);

            var res = wc.UpdateWeek(0, null);
            IActionResult aRes = res.Result;

            Assert.IsType<BadRequestObjectResult>(aRes);
        }

        [Fact]
        public void CreateWeekWithNull_ExpectBadRequest()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);

            var res = wc.CreateWeek(null);
            IActionResult aRes = res.Result;

            Assert.IsType<BadRequestObjectResult>(aRes);
        }

        [Fact]
        public void UpdateDayInNonExistingWeek_ExpectNotFound()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            var day = _testContext.MockUsers[1].WeekSchedule.First().Weekdays.First();
            
            var res = wc.UpdateDay(6540, new WeekdayDTO(day));
            IActionResult aRes = res.Result;

            Assert.IsType<NotFoundResult>(aRes);
        }

        [Fact]
        public void UpdateNonExistingWeek_ExpectNotFound()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            var week = _testContext.MockUsers[1].WeekSchedule.First();

            var res = wc.UpdateWeek(6540, new WeekDTO(week));
            IActionResult aRes = res.Result;

            Assert.IsType<NotFoundResult>(aRes);
        }
    }
}
