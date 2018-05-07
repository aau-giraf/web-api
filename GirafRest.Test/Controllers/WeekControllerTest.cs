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

namespace GirafRest.Test
{
    public class WeekControllerTest
    {
        private readonly ITestOutputHelper _testLogger;
        private TestContext _testContext;

        private const int ADMIN_DEP_ONE = 0;
        private const int GUARDIAN_DEP_TWO = 1;
        private const int CITIZEN_DEP_TWO= 2;
        private const int DEPARTMENT_USER_DEP_TWO = 6;
        private const int CITIZEN_DEP_THREE = 3; // Have no week
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
                                     _testContext.MockUserManager), _testContext.MockLoggerFactory.Object, 
                new GirafAuthenticationService(_testContext.MockDbContext.Object, _testContext.MockRoleManager.Object,
                                               _testContext.MockUserManager));
            _testContext.MockHttpContext = wc.MockHttpContext();

            return wc;
        }

        #region ReadWeekSchedules

        [Fact]
        public void ReadWeekScheduleNames_Authenticated_Success(){
            var wc = initializeTest();
            var mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var weekschedule = _testContext.MockWeeks[0];

            var res = wc.ReadWeekSchedules(mockUser.Id).Result;

            Assert.IsType<Response<IEnumerable<WeekNameDTO>>>(res);
            Assert.True(res.Success);
            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            // check we got the right amount back
            Assert.True(mockUser.WeekSchedule.Count() == res.Data.Count());
            Assert.Equal(weekschedule.Name, res.Data.FirstOrDefault().Name);
        }

        [Theory]
        [InlineData(ADMIN_DEP_ONE, DEPARTMENT_USER_DEP_TWO, ErrorCode.NoError)]
        [InlineData(ADMIN_DEP_ONE, GUARDIAN_DEP_TWO, ErrorCode.NoError)]
        [InlineData(ADMIN_DEP_ONE, CITIZEN_DEP_THREE, ErrorCode.NoError)]
        [InlineData(ADMIN_DEP_ONE, CITIZEN_DEP_TWO, ErrorCode.NoError)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, GUARDIAN_DEP_TWO, ErrorCode.NoError)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, CITIZEN_DEP_TWO, ErrorCode.NoError)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, CITIZEN_DEP_THREE, ErrorCode.NotAuthorized)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, ADMIN_DEP_ONE, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, CITIZEN_DEP_TWO, ErrorCode.NoError)]
        [InlineData(CITIZEN_DEP_TWO, CITIZEN_DEP_THREE, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, GUARDIAN_DEP_TWO, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, DEPARTMENT_USER_DEP_TWO, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, ADMIN_DEP_ONE, ErrorCode.NotAuthorized)]
        public void ReadWeekScheduleNames_CheckAuthentication(int authUser,
                                                  int userToEdit, ErrorCode expectedError)
        {
            var wc = initializeTest();
            var mockUser = _testContext.MockUsers[authUser];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var weekschedule = _testContext.MockWeeks[0];
            _testContext.MockUsers[userToEdit].WeekSchedule = new List<Week>(){weekschedule};
            var res = wc.ReadWeekSchedules(_testContext.MockUsers[userToEdit].Id).Result;
            Assert.Equal(expectedError, res.ErrorCode);
        }

        #endregion
        #region ReadWeekSchedule(id)
        [Fact]
        public void ReadWeekSchedules_AccessValidUsersSpecificWeek_Success()
        {
            var wc = initializeTest();
            var mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = wc.ReadUsersWeekSchedule(mockUser.Id, 2018, 1).Result;

            Assert.True(res.Success);
            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            var adminDepOneWeekZeroSchedule = _testContext.MockUsers[ADMIN_DEP_ONE].WeekSchedule.FirstOrDefault(w => w.Id == WEEK_ZERO);
            Assert.Equal(adminDepOneWeekZeroSchedule?.Name, res.Data.Name);
            Assert.Equal(adminDepOneWeekZeroSchedule?.WeekYear, res.Data.WeekYear);
            Assert.Equal(adminDepOneWeekZeroSchedule?.WeekNumber, res.Data.WeekNumber);
        }

        [Fact]
        public void ReadWeekSchedules_AccessAnyWeek_Success()
        {
            var wc = initializeTest();
            var mockUser = _testContext.MockUsers[CITIZEN_DEP_THREE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var res = wc.ReadUsersWeekSchedule(mockUser.Id, 999, 999).Result;

            Assert.True(res.Success);
            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
        }

        [Theory]
        [InlineData(ADMIN_DEP_ONE, DEPARTMENT_USER_DEP_TWO, ErrorCode.NoError)]
        [InlineData(ADMIN_DEP_ONE, GUARDIAN_DEP_TWO, ErrorCode.NoError)]
        [InlineData(ADMIN_DEP_ONE, CITIZEN_DEP_THREE, ErrorCode.NoError)]
        [InlineData(ADMIN_DEP_ONE, CITIZEN_DEP_TWO, ErrorCode.NoError)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, GUARDIAN_DEP_TWO, ErrorCode.NoError)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, CITIZEN_DEP_TWO, ErrorCode.NoError)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, CITIZEN_DEP_THREE, ErrorCode.NotAuthorized)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, ADMIN_DEP_ONE, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, CITIZEN_DEP_TWO, ErrorCode.NoError)]
        [InlineData(CITIZEN_DEP_TWO, CITIZEN_DEP_THREE, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, GUARDIAN_DEP_TWO, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, DEPARTMENT_USER_DEP_TWO, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, ADMIN_DEP_ONE, ErrorCode.NotAuthorized)]
        public void ReadWeekSchedules_CheckAuthentication(int authUser, 
                                                          int userToEdit, ErrorCode expectedError)
        {
            var wc = initializeTest();
            var mockUser = _testContext.MockUsers[authUser];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var res = wc.ReadUsersWeekSchedule(_testContext.MockUsers[userToEdit].Id, 999, 999).Result;

            Assert.Equal(expectedError, res.ErrorCode);
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
            var res = wc.UpdateWeek(mockUser.Id, 2018, WEEK_ZERO, new WeekDTO(week)).Result;

            Assert.IsType<Response<WeekDTO>>(res);
            Assert.True(res.Success);
            Assert.Equal(week.Name, res.Data.Name);

            _testContext.MockUsers[ADMIN_DEP_ONE].WeekSchedule = tempWeek;
        }

        [Fact]
        public void UpdateWeek_ValidWeekInvalidDTO_Error()
        {
            var wc = initializeTest();
            var mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var tempWeek = _testContext.MockUsers[ADMIN_DEP_ONE].WeekSchedule;
            var res = wc.UpdateWeek(mockUser.Id, 2018, 10, new WeekDTO() { Thumbnail = new Models.DTOs.WeekPictogramDTO(_testContext.MockPictograms[0]) }).Result;
            Assert.False(res.Success);
            Assert.IsType<ErrorResponse<WeekDTO>>(res);
            Assert.Equal(ErrorCode.InvalidAmountOfWeekdays, res.ErrorCode);
        }

        [Fact]
        public void UpdateWeek_ValidWeekNullDTO_Error()
        {
            var wc = initializeTest();
            var mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var res = wc.UpdateWeek(mockUser.Id, 2018, 10, null).Result;

            Assert.False(res.Success);
            Assert.IsType<ErrorResponse<WeekDTO>>(res);
            Assert.Equal(ErrorCode.MissingProperties, res.ErrorCode);
        }

        [Fact]
        public void UpdateWeek_OtherCitizenAsCitizen_Error()
        {
            var wc = initializeTest();
            var mockUser = _testContext.MockUsers[CITIZEN_DEP_TWO];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var week = _testContext.MockUsers[GUARDIAN_DEP_TWO].WeekSchedule.First();

            var tempWeek = _testContext.MockUsers[ADMIN_DEP_ONE].WeekSchedule;
            var res = wc.UpdateWeek(_testContext.MockUsers[CITIZEN_DEP_THREE].Id, 2018, WEEK_ZERO, new WeekDTO(week)).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.NotAuthorized, res.ErrorCode);

            _testContext.MockUsers[ADMIN_DEP_ONE].WeekSchedule = tempWeek;
        }

        [Fact]
        public void UpdateWeek_OtherCitizenAsGuardian_Success()
        {
            var wc = initializeTest();
            var mockUser = _testContext.MockUsers[GUARDIAN_DEP_TWO];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var week = _testContext.MockUsers[GUARDIAN_DEP_TWO].WeekSchedule.First();

            var tempWeek = _testContext.MockUsers[ADMIN_DEP_ONE].WeekSchedule;
            var res = wc.UpdateWeek(_testContext.MockUsers[CITIZEN_DEP_TWO].Id, 2018, WEEK_ZERO, new WeekDTO(week)).Result;

            Assert.True(res.Success);
            Assert.Equal(ErrorCode.NoError, res.ErrorCode);

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
            var res = wc.UpdateWeek(mockUser.Id, 2018, 20, newWeek).Result;

            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            Assert.True(res.Success);
            Assert.Equal("new name", res.Data.Name);
        }

        [Fact]
        public void UpdateWeek_NewWeekValidDTO_CheckFrameNr_Success(){
            var wc = initializeTest();
            var mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var week = _testContext.MockUsers[ADMIN_DEP_ONE].WeekSchedule.First();
            var orderNumber = 1;
            var state = ActivityState.Active;
            
            var activities = new List<Activity>()
            {
                new Activity(week.Weekdays[0], _testContext.MockPictograms[0], orderNumber, state)
            };
            
            week.Weekdays[0].Activities = activities;
            var res = wc.UpdateWeek(mockUser.Id, 2018, 20, new WeekDTO(week)).Result;

            //Assert.IsType<Response<WeekDTO>>(res);
            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            Assert.True(res.Success);
            Assert.Equal(orderNumber, res.Data.Days.ToList()[0].Activities.ToList()[0].Order);
            Assert.Equal(state, res.Data.Days.ToList()[0].Activities.ToList()[0].State);

            var getResult = wc.ReadUsersWeekSchedule(mockUser.Id, 2018, 20).Result;
            
            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            Assert.True(getResult.Success);
            Assert.Equal(orderNumber, getResult.Data.Days.ToList()[0].Activities.ToList()[0].Order);
            Assert.Equal(state, getResult.Data.Days.ToList()[0].Activities.ToList()[0].State);
        }

        [Fact]
        public void UpdateWeek_NewWeekInvalidDTO_Error()
        {
            var wc = initializeTest();
            var mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var res = wc.UpdateWeek(mockUser.Id, 2018, 20, new WeekDTO() { Thumbnail = new Models.DTOs.WeekPictogramDTO(_testContext.MockPictograms[0]) }).Result;
            Assert.IsType<ErrorResponse<WeekDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.InvalidAmountOfWeekdays, res.ErrorCode);
        }

        [Fact]
        public void UpdateWeek_NewWeekNullDTO_Error()
        {
            var wc = initializeTest();
            var mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var res = wc.UpdateWeek(mockUser.Id, 2018, 10, null).Result;

            Assert.IsType<ErrorResponse<WeekDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.MissingProperties, res.ErrorCode);
        }

        [Theory]
        [InlineData(ADMIN_DEP_ONE, DEPARTMENT_USER_DEP_TWO, ErrorCode.NoError)]
        [InlineData(ADMIN_DEP_ONE, GUARDIAN_DEP_TWO, ErrorCode.NoError)]
        [InlineData(ADMIN_DEP_ONE, CITIZEN_DEP_THREE, ErrorCode.NoError)]
        [InlineData(ADMIN_DEP_ONE, CITIZEN_DEP_TWO, ErrorCode.NoError)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, GUARDIAN_DEP_TWO, ErrorCode.NoError)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, CITIZEN_DEP_TWO, ErrorCode.NoError)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, CITIZEN_DEP_THREE, ErrorCode.NotAuthorized)]
        [InlineData(DEPARTMENT_USER_DEP_TWO, ADMIN_DEP_ONE, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, CITIZEN_DEP_TWO, ErrorCode.NoError)]
        [InlineData(CITIZEN_DEP_TWO, CITIZEN_DEP_THREE, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, GUARDIAN_DEP_TWO, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, DEPARTMENT_USER_DEP_TWO, ErrorCode.NotAuthorized)]
        [InlineData(CITIZEN_DEP_TWO, ADMIN_DEP_ONE, ErrorCode.NotAuthorized)]
        public void UpdateWeek_AuthenticationChecks(int authUser,
                                          int userToEdit, ErrorCode expectedError)
        {
            var wc = initializeTest();
            var mockUser = _testContext.MockUsers[authUser];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var week = _testContext.MockUsers[ADMIN_DEP_ONE].WeekSchedule.First();

            var res = wc.UpdateWeek(_testContext.MockUsers[userToEdit].Id, 2018, 20, new WeekDTO(week)).Result;

            Assert.Equal(expectedError, res.ErrorCode);
        }

        #endregion
    }
}
