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
            IActionResult aRes = res.Result;

            Assert.IsType<OkObjectResult>(aRes);
        }

        [Fact]
        public void ReadWeekSchedules_AccessUsersWeeksNoWeeksExist_NotFound()
        {
            var wc = initializeTest();
            _testContext.MockUsers[CITEZEN_DEP_THREE].WeekSchedule.Clear();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITEZEN_DEP_THREE]);

            var res = wc.ReadWeekSchedules();
            IActionResult aRes = res.Result;

            Assert.IsType<NotFoundResult>(aRes);
        }
        #endregion
        #region ReadWeekSchedule(id)
        [Fact]
        public void ReadWeekSchedules_AccessValidUsersSpecificWeek_Ok()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);

            var res = wc.ReadUsersWeekSchedule(WEEK_ZERO);
            IActionResult aRes = res.Result;
            
            Assert.IsType<OkObjectResult>(aRes);
        }

        [Fact]
        public void ReadWeekSchedules_AccessValidUsersSpecificWeekNoWeekExist_NotFound()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITEZEN_DEP_THREE]);

            var res = wc.ReadUsersWeekSchedule(NONEXISTING);
            IActionResult aRes = res.Result;

            Assert.IsType<NotFoundResult>(aRes);
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
            IActionResult aRes = res.Result;

            Assert.IsType<OkObjectResult>(aRes);

            _testContext.MockUsers[ADMIN_DEP_ONE].WeekSchedule = tempWeek;
        }

        [Fact]
        public void UpdateWeek_InvalidWeekValidDTO_NotFound()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var week = _testContext.MockUsers[GUARDIAN_DEP_TWO].WeekSchedule.First();
            var tempWeek = _testContext.MockUsers[ADMIN_DEP_ONE].WeekSchedule;

            var res = wc.UpdateWeek(NONEXISTING, new WeekDTO(week));
            IActionResult aRes = res.Result;

            Assert.IsType<NotFoundResult>(aRes);

            _testContext.MockUsers[ADMIN_DEP_ONE].WeekSchedule = tempWeek;
        }

        [Fact]
        public void UpdateWeek_ValidWeekInvalidDTO_BadRequest()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var tempWeek = _testContext.MockUsers[ADMIN_DEP_ONE].WeekSchedule;

            var res = wc.UpdateWeek(WEEK_ZERO, new WeekDTO());
            IActionResult aRes = res.Result;

            Assert.IsType<BadRequestObjectResult>(aRes);

            _testContext.MockUsers[ADMIN_DEP_ONE].WeekSchedule = tempWeek;
        }

        [Fact]
        public void UpdateWeek_ValidWeekNullDTO_BadRequest()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);

            var res = wc.UpdateWeek(WEEK_ZERO, null);
            IActionResult aRes = res.Result;

            Assert.IsType<BadRequestObjectResult>(aRes);
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
            IActionResult aRes = res.Result;

            Assert.IsType<OkObjectResult>(aRes);

            _testContext.MockUsers[ADMIN_DEP_ONE].WeekSchedule.Remove(_testContext.MockUsers[ADMIN_DEP_ONE].WeekSchedule.Last());
        }

        [Fact]
        public void CreateWeek_NewWeekInvalidDTO_BadRequest()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);

            var res = wc.CreateWeek(new WeekDTO());
            IActionResult aRes = res.Result;

            Assert.IsType<BadRequestObjectResult>(aRes);

            _testContext.MockUsers[ADMIN_DEP_ONE].WeekSchedule.Remove(_testContext.MockUsers[ADMIN_DEP_ONE].WeekSchedule.Last());
        }

        [Fact]
        public void CreateWeek_NewWeekNullDTO_BadRequest()
        {
            var wc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);

            var res = wc.CreateWeek(null);
            IActionResult aRes = res.Result;

            Assert.IsType<BadRequestObjectResult>(aRes);
        }
        #endregion
    }
}
