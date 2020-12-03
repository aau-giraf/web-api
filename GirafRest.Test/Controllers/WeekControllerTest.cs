using System.Linq;
using Xunit;
using GirafRest.Models;
using GirafRest.Controllers;
using System.Collections.Generic;
using GirafRest.Test.Mocks;
using static GirafRest.Test.UnitTestExtensions;
using GirafRest.Models.DTOs;
using Xunit.Abstractions;
using GirafRest.Models.Responses;
using GirafRest.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace GirafRest.Test
{
    public class WeekControllerTest
    {
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE0052 // Remove unread private members
        private readonly ITestOutputHelper _testLogger;
#pragma warning restore IDE0052 // Remove unread private members
        private TestContext _testContext;

        private const int ADMIN_DEP_ONE = 0;
        private const int GUARDIAN_DEP_TWO = 1;
        private const int CITIZEN_DEP_TWO = 2;
        private const int DEPARTMENT_USER_DEP_TWO = 6;
        private const int CITIZEN_DEP_THREE = 3; // Have no week
        private const int YEAR_ZERO = 0;
        private const int WEEK_ZERO = 0;
        private const int DAY_ZERO = 0;
        private const int NONEXISTING = 999;
#pragma warning restore IDE0051 // Remove unused private members

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
                                     _testContext.MockUserManager), _testContext.MockLoggerFactory.Object,
                new GirafAuthenticationService(_testContext.MockDbContext.Object, _testContext.MockRoleManager.Object,
                                               _testContext.MockUserManager));
            _testContext.MockHttpContext = wc.MockHttpContext();

            return wc;
        }

        #region ReadWeekSchedules

        [Fact]
        public void ReadWeekScheduleNames_Authenticated_Success()
        {
            var wc = initializeTest();
            var mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var weekschedule = _testContext.MockWeeks[0];

            var res = wc.ReadWeekSchedules(mockUser.Id).Result as ObjectResult;
            var body = res.Value as SuccessResponse<IEnumerable<WeekNameDTO>>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            // check we got the right amount back
            Assert.True(mockUser.WeekSchedule.Count() == body.Data.Count());
            Assert.Equal(weekschedule.Name, body.Data.FirstOrDefault().Name);
        }

        [Theory]
        [InlineData(DEPARTMENT_USER_DEP_TWO, CITIZEN_DEP_THREE, ErrorCode.NotAuthorized)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, ADMIN_DEP_ONE, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, CITIZEN_DEP_THREE, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, GUARDIAN_DEP_TWO, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, DEPARTMENT_USER_DEP_TWO, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, ADMIN_DEP_ONE, ErrorCode.NotAuthorized)]
        public void ReadWeekScheduleNames_CheckAuthentication_Errors(
            int authUser, int userToEdit, ErrorCode expectedError)
        {
            var wc = initializeTest();
            var mockUser = _testContext.MockUsers[authUser];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var weekschedule = _testContext.MockWeeks[0];
            _testContext.MockUsers[userToEdit].WeekSchedule = new List<Week>() {weekschedule};
            var res = wc.ReadWeekSchedules(_testContext.MockUsers[userToEdit].Id).Result as ObjectResult;

            var body = res.Value as ErrorResponse;
            Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
            Assert.Equal(expectedError, body.ErrorCode);
        }

        [Theory]
        [InlineData(ADMIN_DEP_ONE, DEPARTMENT_USER_DEP_TWO)]
        [InlineData(ADMIN_DEP_ONE, GUARDIAN_DEP_TWO)]
        [InlineData(ADMIN_DEP_ONE, CITIZEN_DEP_THREE)]
        [InlineData(ADMIN_DEP_ONE, CITIZEN_DEP_TWO)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, GUARDIAN_DEP_TWO)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, CITIZEN_DEP_TWO)]
        [InlineData(CITIZEN_DEP_TWO, CITIZEN_DEP_TWO)]
        public void ReadWeekScheduleNames_CheckAuthentication_Success(int authUser, int userToEdit)
        {
            var wc = initializeTest();
            var mockUser = _testContext.MockUsers[authUser];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var weekschedule = _testContext.MockWeeks[0];
            _testContext.MockUsers[userToEdit].WeekSchedule = new List<Week>() {weekschedule};
            var res = wc.ReadWeekSchedules(_testContext.MockUsers[userToEdit].Id).Result as ObjectResult;
            
            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
        }

        #endregion
        #region ReadWeekSchedule(id)
        [Fact]
        public void ReadWeekSchedules_AccessValidUsersSpecificWeek_Success()
        {
            var wc = initializeTest();
            var mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = wc.ReadUsersWeekSchedule(mockUser.Id, 2018, 1).Result as ObjectResult;
            var body = res.Value as SuccessResponse<WeekDTO>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);

            var adminDepOneWeekZeroSchedule = _testContext.MockUsers[ADMIN_DEP_ONE].WeekSchedule.FirstOrDefault(w => w.Id == WEEK_ZERO);
            Assert.Equal(adminDepOneWeekZeroSchedule?.Name, body.Data.Name);
            Assert.Equal(adminDepOneWeekZeroSchedule?.WeekYear, body.Data.WeekYear);
            Assert.Equal(adminDepOneWeekZeroSchedule?.WeekNumber, body.Data.WeekNumber);
        }

        [Fact]
        public void ReadWeekSchedules_AccessAnyWeek_Success()
        {
            var wc = initializeTest();
            var mockUser = _testContext.MockUsers[CITIZEN_DEP_THREE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var res = wc.ReadUsersWeekSchedule(mockUser.Id, 999, 999).Result as ObjectResult;
            var body = res.Value as SuccessResponse<WeekDTO>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
        }

        [Theory]
        [InlineData(DEPARTMENT_USER_DEP_TWO, CITIZEN_DEP_THREE, ErrorCode.NotAuthorized)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, ADMIN_DEP_ONE, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, CITIZEN_DEP_THREE, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, GUARDIAN_DEP_TWO, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, DEPARTMENT_USER_DEP_TWO, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, ADMIN_DEP_ONE, ErrorCode.NotAuthorized)]
        public void ReadWeekSchedules_CheckAuthentication_Errors(
            int authUser, int userToEdit, ErrorCode expectedError)
        {
            var wc = initializeTest();
            var mockUser = _testContext.MockUsers[authUser];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var res = wc.ReadUsersWeekSchedule(_testContext.MockUsers[userToEdit].Id, 999, 999).Result as ObjectResult;

            var body = res.Value as ErrorResponse;
            Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
            Assert.Equal(expectedError, body.ErrorCode);
        }

        
        [Theory]
        [InlineData(ADMIN_DEP_ONE, DEPARTMENT_USER_DEP_TWO)]
        [InlineData(ADMIN_DEP_ONE, GUARDIAN_DEP_TWO)]
        [InlineData(ADMIN_DEP_ONE, CITIZEN_DEP_THREE)]
        [InlineData(ADMIN_DEP_ONE, CITIZEN_DEP_TWO)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, GUARDIAN_DEP_TWO)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, CITIZEN_DEP_TWO)]
        [InlineData(CITIZEN_DEP_TWO, CITIZEN_DEP_TWO)]
        public void ReadWeekSchedules_CheckAuthentication_Success(int authUser,
            int userToEdit)
        {
            var wc = initializeTest();
            var mockUser = _testContext.MockUsers[authUser];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var res = wc.ReadUsersWeekSchedule(_testContext.MockUsers[userToEdit].Id, 999, 999).Result as ObjectResult;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
        }

        #endregion
        #region UpdateWeek
        [Fact]
        public void UpdateWeek_ValidWeekValidDTO_Success()
        {
            var wc = initializeTest();
            var mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var week = _testContext.MockUsers[GUARDIAN_DEP_TWO].WeekSchedule.First();
            var tempWeek = _testContext.MockUsers[ADMIN_DEP_ONE].WeekSchedule;
            var res = wc.UpdateWeek(mockUser.Id, 2018, WEEK_ZERO, new WeekDTO(week)).Result as ObjectResult;
            var body = res.Value as SuccessResponse<WeekDTO>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            Assert.Equal(week.Name, body.Data.Name);

            _testContext.MockUsers[ADMIN_DEP_ONE].WeekSchedule = tempWeek;
        }

        [Fact]
        public void UpdateWeek_ValidWeekInvalidDTO_InvalidAmountOfWeekdays()
        {
            var wc = initializeTest();
            var mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var tempWeek = _testContext.MockUsers[ADMIN_DEP_ONE].WeekSchedule;
            var res = wc.UpdateWeek(mockUser.Id, 2018, 10, new WeekDTO()
            {
                Thumbnail = new Models.DTOs.WeekPictogramDTO(_testContext.MockPictograms[0])
            }).Result as ObjectResult;
            var body = res.Value as ErrorResponse;
            
            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal(ErrorCode.InvalidAmountOfWeekdays, body.ErrorCode);
        }

        [Fact]
        public void UpdateWeek_ValidWeekNullDTO_MissingProperties()
        {
            var wc = initializeTest();
            var mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var res = wc.UpdateWeek(mockUser.Id, 2018, 10, null).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal(ErrorCode.MissingProperties, body.ErrorCode);
        }

        [Fact]
        public void UpdateWeek_OtherCitizenAsCitizen_NotAuthorized()
        {
            var wc = initializeTest();
            var mockUser = _testContext.MockUsers[CITIZEN_DEP_TWO];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var week = _testContext.MockUsers[GUARDIAN_DEP_TWO].WeekSchedule.First();

            var tempWeek = _testContext.MockUsers[ADMIN_DEP_ONE].WeekSchedule;
            var res = wc.UpdateWeek(
                _testContext.MockUsers[CITIZEN_DEP_THREE].Id,
                2018,
                WEEK_ZERO,
                new WeekDTO(week)
            ).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
            Assert.Equal(ErrorCode.NotAuthorized, body.ErrorCode);

            _testContext.MockUsers[ADMIN_DEP_ONE].WeekSchedule = tempWeek;
        }

        [Fact]
        public async void UpdateWeek_AddActivityAfterTimerIsAdded()
        {
            var wc = initializeTest();
            var mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            _testContext.MockWeeks[0].Weekdays[0].Activities.Add(_testContext.MockActivities[0]);
            var timer = _testContext.MockTimers[0];
            long timerkey = timer.Key;
            _testContext.MockWeeks[0].Weekdays[0].Activities.ElementAt(0).Timer = timer;
            var week = _testContext.MockWeeks[0];
            await wc.UpdateWeek(mockUser.Id, 2018, 1, new WeekDTO(week));
            Assert.Equal(_testContext.MockWeeks[0].Weekdays[0].Activities.ElementAt(0).Timer.Key, timerkey);
        }

        #endregion
        #region UpdateWeekDay
        [Fact]
        public void UpdateWeekday_ValidWeekdayValidDTO_Success()
        {
            var wc = initializeTest();
            var mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var weekday = mockUser.WeekSchedule.First().Weekdays[0];
            var res = wc.UpdateWeekday(mockUser.Id, 2018, 1, new WeekdayDTO(weekday)).Result as ObjectResult;
            var body = res.Value as SuccessResponse<WeekdayDTO>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            Assert.Equal(weekday.Day.ToString(), body.Data.Day.ToString());
        }
        [Fact]
        public void UpdateWeekday_InvalidWeekdayValdidDTO_MissingProperties()
        {
            var wc = initializeTest();
            var mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);

            var res = wc.UpdateWeekday(mockUser.Id, 2018, 1, null).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal(ErrorCode.MissingProperties, body.ErrorCode);
        }
        [Fact]
        public void UpdateWeekday_InvalidWeek_ValidDTO_NotFound()
        {
            var wc = initializeTest();
            var mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);

            var weekday = mockUser.WeekSchedule.First().Weekdays[0];

            var res = wc.UpdateWeekday(mockUser.Id, 99999, 99, new WeekdayDTO(weekday)).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status404NotFound, res.StatusCode);
            Assert.Equal(ErrorCode.NotFound, body.ErrorCode);
        }
        [Fact]
        public void UpdateWeekday_InvalidUser_NotFound()
        {
            var wc = initializeTest();
            var mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);

            var weekday = mockUser.WeekSchedule.First().Weekdays[0];
            var res = wc.UpdateWeekday("FakeUserId", 2018, 1, new WeekdayDTO(weekday)).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status404NotFound, res.StatusCode);
            Assert.Equal(ErrorCode.UserNotFound, body.ErrorCode);
        }
        #endregion
        #region GetWeekDay

        [Fact]
        public void GetWeekDay_SuccessResponse()
        {
            var wc = initializeTest();
            var mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);

            var week = mockUser.WeekSchedule.FirstOrDefault();

            var res = wc.GetWeekDay(mockUser.Id, week.WeekYear, week.WeekNumber, DAY_ZERO).Result as ObjectResult;
            var body = res.Value as SuccessResponse<WeekdayDTO>;
            
            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            Assert.Equal(week.Weekdays.First().Activities.Count, body.Data.Activities.Count);
        }
        [Fact]
        public void GetWeekDay_WeekDayOutOfBounds()
        {
            var wc = initializeTest();
            var mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var res = wc.GetWeekDay(mockUser.Id, YEAR_ZERO, WEEK_ZERO, NONEXISTING).Result as ObjectResult;
            var body = res.Value as ErrorResponse;
            
            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal(ErrorCode.InvalidDay, body.ErrorCode);
        }

        [Fact]
        public void GetWeekDay_WeekNull_NotFound()
        {
            var wc = initializeTest();
            var mockUser = _testContext.MockUsers[CITIZEN_DEP_THREE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);

            var res = wc.GetWeekDay(mockUser.Id, YEAR_ZERO, WEEK_ZERO, DAY_ZERO).Result as ObjectResult;
            var body = res.Value as ErrorResponse;
            
            Assert.Equal(StatusCodes.Status404NotFound, res.StatusCode);
            Assert.Equal(ErrorCode.NotFound, body.ErrorCode);
        }

        [Fact]
        public void GetWeekDay_UserPermission_NotAuthorized()
        {
            var wc = initializeTest();
            var mockUser = _testContext.MockUsers[4];          

            var res = wc.GetWeekDay(mockUser.Id, YEAR_ZERO, WEEK_ZERO, DAY_ZERO).Result as ObjectResult;
            var body = res.Value as ErrorResponse;
            
            Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
            Assert.Equal(ErrorCode.NotAuthorized, body.ErrorCode);
        }


        #endregion
        #region CreateWeek
        [Fact]
        public void UpdateWeek_OtherCitizenAsGuardian_Success()
        {
            var wc = initializeTest();
            var mockUser = _testContext.MockUsers[GUARDIAN_DEP_TWO];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var week = _testContext.MockUsers[GUARDIAN_DEP_TWO].WeekSchedule.First();

            var tempWeek = _testContext.MockUsers[ADMIN_DEP_ONE].WeekSchedule;
            var res = wc.UpdateWeek(
                _testContext.MockUsers[CITIZEN_DEP_TWO].Id,
                2018,
                WEEK_ZERO,
                new WeekDTO(week)
            ).Result as ObjectResult;
            var body = res.Value as SuccessResponse<WeekDTO>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);

            _testContext.MockUsers[ADMIN_DEP_ONE].WeekSchedule = tempWeek;
        }


        [Fact]
        public void UpdateWeek_NewWeekValidDTO_Success()
        {
            var wc = initializeTest();
            var mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var week = _testContext.MockUsers[ADMIN_DEP_ONE].WeekSchedule.First();
            // modify name
            var newWeek = new WeekDTO(week)
            {
                Name = "new name"
            };
            var res = wc.UpdateWeek(mockUser.Id, 2018, 20, newWeek).Result as ObjectResult;
            var body = res.Value as SuccessResponse<WeekDTO>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            Assert.Equal("new name", body.Data.Name);
        }

        [Fact]
        public void UpdateWeek_NewWeekValidDTO_CheckFrameNr_Success()
        {
            var wc = initializeTest();
            var mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var week = _testContext.MockUsers[ADMIN_DEP_ONE].WeekSchedule.First();
            var orderNumber = 1;
            var state = ActivityState.Active;
            
            var activities = new List<Activity>()
            {
                new Activity(week.Weekdays[0], new List<Pictogram>() {_testContext.MockPictograms[0]}, orderNumber, state, null, false, null )
            };
            
            week.Weekdays[0].Activities = activities;
            var res = wc.UpdateWeek(mockUser.Id, 2018, 20, new WeekDTO(week)).Result as ObjectResult;
            var body = res.Value as SuccessResponse<WeekDTO>;

            //Assert.IsType<Response<WeekDTO>>(res);
            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            Assert.Equal(orderNumber, body.Data.Days.ToList()[0].Activities.ToList()[0].Order);
            Assert.Equal(state, body.Data.Days.ToList()[0].Activities.ToList()[0].State);

            var getResult = wc.ReadUsersWeekSchedule(mockUser.Id, 2018, 20).Result as ObjectResult;
            var getBody = getResult.Value as SuccessResponse<WeekDTO>;
            
            Assert.Equal(StatusCodes.Status200OK, getResult.StatusCode);
            Assert.Equal(orderNumber, getBody.Data.Days.ToList()[0].Activities.ToList()[0].Order);
            Assert.Equal(state, getBody.Data.Days.ToList()[0].Activities.ToList()[0].State);
        }

        [Fact]
        public void UpdateWeek_NewWeekInvalidDTO_InvalidAmountOfWeekdays()
        {
            var wc = initializeTest();
            var mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var res = wc.UpdateWeek(mockUser.Id, 2018, 20, new WeekDTO()
            {
                Thumbnail = new Models.DTOs.WeekPictogramDTO(_testContext.MockPictograms[0])
            }).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal(ErrorCode.InvalidAmountOfWeekdays, body.ErrorCode);
        }

        [Fact]
        public void UpdateWeek_NewWeekNullDTO_MissingProperties()
        {
            var wc = initializeTest();
            var mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var res = wc.UpdateWeek(mockUser.Id, 2018, 10, null).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal(ErrorCode.MissingProperties, body.ErrorCode);
        }

        [Theory]
        [InlineData(DEPARTMENT_USER_DEP_TWO, CITIZEN_DEP_THREE, ErrorCode.NotAuthorized)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, ADMIN_DEP_ONE, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, CITIZEN_DEP_THREE, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, GUARDIAN_DEP_TWO, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, DEPARTMENT_USER_DEP_TWO, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, ADMIN_DEP_ONE, ErrorCode.NotAuthorized)]
        public void UpdateWeek_AuthenticationChecks_Errors(int authUser, int userToEdit, ErrorCode expectedError)
        {
            var wc = initializeTest();
            var mockUser = _testContext.MockUsers[authUser];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var week = _testContext.MockUsers[ADMIN_DEP_ONE].WeekSchedule.First();

            var res = wc.UpdateWeek(
                _testContext.MockUsers[userToEdit].Id,
                2018,
                20,
                new WeekDTO(week)
            ).Result as ObjectResult;

            var body = res.Value as ErrorResponse;
            Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
            Assert.Equal(expectedError, body.ErrorCode);
        }

        [Theory]
        [InlineData(ADMIN_DEP_ONE, DEPARTMENT_USER_DEP_TWO)]
        [InlineData(ADMIN_DEP_ONE, GUARDIAN_DEP_TWO)]
        [InlineData(ADMIN_DEP_ONE, CITIZEN_DEP_THREE)]
        [InlineData(ADMIN_DEP_ONE, CITIZEN_DEP_TWO)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, GUARDIAN_DEP_TWO)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, CITIZEN_DEP_TWO)]
        [InlineData(CITIZEN_DEP_TWO, CITIZEN_DEP_TWO)]
        public void UpdateWeek_AuthenticationChecks_Success(int authUser, int userToEdit)
        {
            var wc = initializeTest();
            var mockUser = _testContext.MockUsers[authUser];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var week = _testContext.MockUsers[ADMIN_DEP_ONE].WeekSchedule.First();

            var res = wc.UpdateWeek(
                _testContext.MockUsers[userToEdit].Id,
                2018,
                20,
                new WeekDTO(week)
            ).Result as ObjectResult;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
        }
        #endregion
    }
}
