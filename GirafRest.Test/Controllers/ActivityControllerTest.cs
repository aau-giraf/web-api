using System.Linq;
using GirafRest.Controllers;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using GirafRest.Models.Responses;
using GirafRest.Services;
using GirafRest.Test.Mocks;
using Xunit;
using static GirafRest.Test.UnitTestExtensions;

namespace GirafRest.Test
{
    public class ActivityControllerTest
    {
        private const int ADMIN_DEP_ONE = 0;
        private const int GUARDIAN_DEP_TWO = 1;

        private const int _existingId = 1;
        private const int _nonExistingId = 404;
        private TestContext _testContext;

        private ActivityController InitializeTest()
        {
            _testContext = new TestContext();

            //Ensures that the mock user has the mock activity in the user's weekplan
            _testContext.MockDbContext.Object.Users.First().WeekSchedule.First().Weekdays
                .First().Activities.Add(_testContext.MockActivities.Find(a => a.Key == _existingId));

            var ac = new ActivityController(
                new MockGirafService(_testContext.MockDbContext.Object,
                                     _testContext.MockUserManager), _testContext.MockLoggerFactory.Object,
                new GirafAuthenticationService(_testContext.MockDbContext.Object, _testContext.MockRoleManager.Object,
                                               _testContext.MockUserManager));
            _testContext.MockHttpContext = ac.MockHttpContext();
            return ac;
        }

        #region CreateActivity
        [Fact]
        public void PostActivity_ValidDayValidDTO_Succes()
        {
            ActivityController ac = InitializeTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            Week week = mockUser.WeekSchedule.First();

            ActivityDTO newActivity = new ActivityDTO() { Pictogram = new WeekPictogramDTO(_testContext.MockPictograms.First()) };

            var res = ac.PostActivity(newActivity, mockUser.Id, week.Name, week.WeekYear, week.WeekNumber, Days.Monday).Result;

            Assert.True(res.Success);
            Assert.Equal(newActivity.Pictogram.Id, res.Data.Pictogram.Id);
        }

        [Fact]
        public void PostActivity_NoExistingActivitiesOnDayValidDTO_Succes()
        {
            ActivityController ac = InitializeTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            Week week = mockUser.WeekSchedule.First();

            ActivityDTO newActivity = new ActivityDTO() { Pictogram = new WeekPictogramDTO(_testContext.MockPictograms.First()) };

            var res = ac.PostActivity(newActivity, mockUser.Id, week.Name, week.WeekYear, week.WeekNumber, Days.Saturday).Result;

            Assert.True(res.Success);
            Assert.Equal(newActivity.Pictogram.Id, res.Data.Pictogram.Id);
        }

        [Fact]
        public void PostActivity_ValidDayInvalidDTO_MissingProperties()
        {
            ActivityController ac = InitializeTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            Week week = mockUser.WeekSchedule.First();

            ActivityDTO newActivity = null;

            var res = ac.PostActivity(newActivity, mockUser.Id, week.Name, week.WeekYear, week.WeekNumber, Days.Monday).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.MissingProperties, res.ErrorCode);
        }

        [Fact]
        public void PostActivity_InvalidDayValidDTO_InvalidDay()
        {
            ActivityController ac = InitializeTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            Week week = mockUser.WeekSchedule.First();

            ActivityDTO newActivity = new ActivityDTO() { Pictogram = new WeekPictogramDTO(_testContext.MockPictograms.First()) };

            var res = ac.PostActivity(newActivity, mockUser.Id, week.Name, week.WeekYear, week.WeekNumber, Days.Sunday + 1).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.InvalidDay, res.ErrorCode);
        }

        [Fact]
        public void PostActivity_InvalidWeekYearValidDTO_WeekNotFound()
        {
            ActivityController ac = InitializeTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            Week week = mockUser.WeekSchedule.First();

            ActivityDTO newActivity = new ActivityDTO() { Pictogram = new WeekPictogramDTO(_testContext.MockPictograms.First()) };

            var res = ac.PostActivity(newActivity, mockUser.Id, week.Name, 9000, week.WeekNumber, Days.Sunday).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.WeekNotFound, res.ErrorCode);
        }

        [Fact]
        public void PostActivity_InvalidWeekNumberValidDTO_WeekNotFound()
        {
            ActivityController ac = InitializeTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            Week week = mockUser.WeekSchedule.First();

            ActivityDTO newActivity = new ActivityDTO() { Pictogram = new WeekPictogramDTO(_testContext.MockPictograms.First()) };

            var res = ac.PostActivity(newActivity, mockUser.Id, week.Name, week.WeekYear, 54, Days.Sunday).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.WeekNotFound, res.ErrorCode);
        }

        [Fact]
        public void PostActivity_InvalidWeekNameValidDTO_WeekNotFound()
        {
            ActivityController ac = InitializeTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            Week week = mockUser.WeekSchedule.First();

            ActivityDTO newActivity = new ActivityDTO() { Pictogram = new WeekPictogramDTO(_testContext.MockPictograms.First()) };

            var res = ac.PostActivity(newActivity, mockUser.Id, "WrongName", week.WeekYear, week.WeekYear, Days.Sunday).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.WeekNotFound, res.ErrorCode);
        }

        [Fact]
        public void PostActivity_NonExistingUserValidDayValidDTO_UserNotFound()
        {
            ActivityController ac = InitializeTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            Week week = mockUser.WeekSchedule.First();

            ActivityDTO newActivity = new ActivityDTO() { Pictogram = new WeekPictogramDTO(_testContext.MockPictograms.First()) };

            var res = ac.PostActivity(newActivity, "NonExistingUserId", week.Name, week.WeekYear, week.WeekNumber, Days.Sunday).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.UserNotFound, res.ErrorCode);
        }

        [Fact]
        public void PostActivity_UserNotAuthorizedValidDayValidDTO_NotAuthorized()
        {
            ActivityController ac = InitializeTest();
            GirafUser mockUser = _testContext.MockUsers[GUARDIAN_DEP_TWO];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            GirafUser differentMockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            Week week = differentMockUser.WeekSchedule.First();

            ActivityDTO newActivity = new ActivityDTO() { Pictogram = new WeekPictogramDTO(_testContext.MockPictograms.First()) };

            var res = ac.PostActivity(newActivity, differentMockUser.Id, week.Name, week.WeekYear, week.WeekNumber, Days.Sunday).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.NotAuthorized, res.ErrorCode);
        }
        #endregion

        #region DeleteActivity
        [Fact]
        public void DeleteActivity_ExistingActivity_Success()
        {
            ActivityController activityController = InitializeTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);

            //There exist an acitivity with the id
            Assert.True(_testContext.MockActivities.Any(a => a.Key == _existingId));

            Response res = activityController.DeleteActivity(mockUser.Id, _existingId).Result;
            Assert.True(res.Success);
        }

        [Fact]
        public void DeleteActivity_NonExistingActivity_Failure()
        {
            ActivityController activityController = InitializeTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);

            Response res = activityController.DeleteActivity(mockUser.Id, _nonExistingId).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.ActivityNotFound, res.ErrorCode);
        }

        [Fact]
        public void DeleteActivity_NonExistingUser_Failure()
        {
            ActivityController activityController = InitializeTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];

            Response res = activityController.DeleteActivity("NonExistingUserId", _existingId).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.UserNotFound, res.ErrorCode);
        }

        [Fact]
        public void DeleteActivity_UnauthorizedUserSendsIdOfActivitysOwner_Failure()
        {
            ActivityController activityController = InitializeTest();
            GirafUser loggedInUser = _testContext.MockUsers[GUARDIAN_DEP_TWO];
            _testContext.MockUserManager.MockLoginAsUser(loggedInUser);
            GirafUser activityOwnerUser = _testContext.MockUsers[ADMIN_DEP_ONE];

            // logged in as GUARDIAN_DEP_TWO and trying to delete one of ADMIN_DEP_ONE's activities
            Response res = activityController.DeleteActivity(activityOwnerUser.Id, _existingId).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.NotAuthorized, res.ErrorCode);
        }

        [Fact]
        public void DeleteActivity_UnauthorizedUserSendsIdOfLoggedInUser_Failure()
        {
            ActivityController activityController = InitializeTest();
            GirafUser loggedInUser = _testContext.MockUsers[GUARDIAN_DEP_TWO];
            _testContext.MockUserManager.MockLoginAsUser(loggedInUser);

            // logged in as GUARDIAN_DEP_TWO and trying to delete one of ADMIN_DEP_ONES activities
            Response res = activityController.DeleteActivity(loggedInUser.Id, _existingId).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.ActivityNotFound, res.ErrorCode);
        }
        #endregion

        #region UpdateActivity
        [Fact]
        public void UpdateActivity_InvalidActivityDTO_MissingProperties()
        {
            ActivityController ac = InitializeTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);

            ActivityDTO newActivity = null;

            var res = ac.UpdateActivity(newActivity).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.MissingProperties, res.ErrorCode);
        }

        [Fact]
        public void UpdateActivity_InvalidActivityValidDTO_ActivityNotFound()
        {
            ActivityController ac = InitializeTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);

            ActivityDTO newActivity = new ActivityDTO() { Id = 420 };

            var res = ac.UpdateActivity(newActivity).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.ActivityNotFound, res.ErrorCode);
        }

        [Fact]
        public void UpdateActivity_ValidActivityValidDTO_ActivitySucess()
        {
            ActivityController ac = InitializeTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);

            ActivityDTO newActivity = new ActivityDTO() { Pictogram = new WeekPictogramDTO(_testContext.MockPictograms.First()) };

            var res = ac.UpdateActivity(newActivity).Result;

            Assert.True(res.Success);
            Assert.Equal(newActivity.Pictogram.Id, res.Data.Pictogram.Id);
        }
        #endregion
    }
}
