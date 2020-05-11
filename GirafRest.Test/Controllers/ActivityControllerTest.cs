using System.Linq;
using GirafRest.Controllers;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using GirafRest.Models.Responses;
using GirafRest.Services;
using GirafRest.Test.Mocks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using static GirafRest.Test.UnitTestExtensions;
using System.Collections.Generic;

namespace GirafRest.Test
{
    public class ActivityControllerTest
    {
#pragma warning disable IDE0051 // Remove unused private members
        private const int ADMIN_DEP_ONE = 0;
        private const int GUARDIAN_DEP_TWO = 1;
        private const int CITIZEN_NO_DEP = 9;


        private const int PICTO_DEP_TWO = 6;

        private const int _existingId = 1;
        private const int _nonExistingId = 404;
#pragma warning restore IDE0051 // Remove unused private members

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
        public void PostActivity_ValidDayValidDTOWithOnePictogram_Succes()
        {
            ActivityController ac = InitializeTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            Week week = mockUser.WeekSchedule.First();

            ActivityDTO newActivity = new ActivityDTO() { Pictograms = new List<WeekPictogramDTO> { new WeekPictogramDTO(_testContext.MockPictograms.First()) }};

            var res = ac.PostActivity(newActivity, mockUser.Id, week.Name, week.WeekYear, week.WeekNumber, (int)Days.Monday).Result;

            List<long> expectedPictogramIds = newActivity.Pictograms.Select(pictogram => pictogram.Id).ToList();
            List<long> actualPictogramIds = res.Data.Pictograms.Select(pictogram => pictogram.Id).ToList();

            Assert.True(res.Success);
            Assert.Equal(expectedPictogramIds, actualPictogramIds);
        }

        [Fact]
        public void PostActivity_ValidDayValidDTOWithMultiplePictograms_Success()
        {
            ActivityController ac = InitializeTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            Week week = mockUser.WeekSchedule.First();

            ActivityDTO newActivity = new ActivityDTO() { 
                Pictograms = new List<WeekPictogramDTO> { 
                    new WeekPictogramDTO(_testContext.MockPictograms.First()),
                    new WeekPictogramDTO(_testContext.MockPictograms.Last()),
                }
            };

            var res = ac.PostActivity(
                newActivity, mockUser.Id, week.Name, week.WeekYear, week.WeekNumber, (int)Days.Monday
            ).Result as ObjectResult;

            List<long> expectedPictogramIds = newActivity.Pictograms.Select(pictogram => pictogram.Id).ToList();
            List<long> actualPictogramIds = res.Data.Pictograms.Select(pictogram => pictogram.Id).ToList();

            Assert.True(res.Success);
            Assert.Equal(expectedPictogramIds, actualPictogramIds);
        }

        [Fact]
        public void PostActivity_NoExistingActivitiesOnDayValidDTO_Succes()
        {
            ActivityController ac = InitializeTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            Week week = mockUser.WeekSchedule.First();

            ActivityDTO newActivity = new ActivityDTO() { 
                Pictograms = new List<WeekPictogramDTO> { new WeekPictogramDTO(_testContext.MockPictograms.First()) 
            }};

            var res = ac.PostActivity(
                newActivity, mockUser.Id, week.Name, week.WeekYear, week.WeekNumber, (int)Days.Saturday
            ).Result as ObjectResult;

            List<long> expectedPictogramIds = newActivity.Pictograms.Select(pictogram => pictogram.Id).ToList();
            List<long> actualPictogramIds = res.Data.Pictograms.Select(pictogram => pictogram.Id).ToList();

            Assert.True(res.Success);
            Assert.Equal(expectedPictogramIds, actualPictogramIds);
        }

        [Fact]
        public void PostActivity_ValidDayInvalidDTO_MissingProperties()
        {
            ActivityController ac = InitializeTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            Week week = mockUser.WeekSchedule.First();

            ActivityDTO newActivity = null;

            var res = ac.PostActivity(
                newActivity, mockUser.Id, week.Name, week.WeekYear, week.WeekNumber, (int)Days.Monday
            ).Result as ObjectResult;

            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal(ErrorCode.MissingProperties, body.ErrorCode);
        }

        [Fact]
        public void PostActivity_InvalidDayValidDTO_InvalidDay()
        {
            ActivityController ac = InitializeTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            Week week = mockUser.WeekSchedule.First();

            ActivityDTO newActivity = new ActivityDTO() { 
                Pictograms = new List<WeekPictogramDTO> { new WeekPictogramDTO(_testContext.MockPictograms.First()) 
            }};

            var res = ac.PostActivity(
                newActivity, mockUser.Id, week.Name, week.WeekYear, week.WeekNumber, (int)Days.Sunday + 1
            ).Result as ObjectResult;

            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status404NotFound, res.StatusCode);
            Assert.Equal(ErrorCode.InvalidDay, body.ErrorCode);
        }

        [Fact]
        public void PostActivity_InvalidWeekYearValidDTO_WeekNotFound()
        {
            ActivityController ac = InitializeTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            Week week = mockUser.WeekSchedule.First();

            ActivityDTO newActivity = new ActivityDTO() { Pictograms = new List<WeekPictogramDTO> { new WeekPictogramDTO(_testContext.MockPictograms.First()) }};

            var res = ac.PostActivity(
                newActivity, mockUser.Id, week.Name, 9000, week.WeekNumber, (int)Days.Sunday
            ).Result as ObjectResult;

            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status404NotFound, res.StatusCode);
            Assert.Equal(ErrorCode.WeekNotFound, body.ErrorCode);
        }

        [Fact]
        public void PostActivity_InvalidWeekNumberValidDTO_WeekNotFound()
        {
            ActivityController ac = InitializeTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            Week week = mockUser.WeekSchedule.First();

            ActivityDTO newActivity = new ActivityDTO() { Pictograms = new List<WeekPictogramDTO> { new WeekPictogramDTO(_testContext.MockPictograms.First()) }};

            var res = ac.PostActivity(
                newActivity, mockUser.Id, week.Name, week.WeekYear, 54, (int)Days.Sunday
            ).Result as ObjectResult;

            var body = res.Value as ErrorResponse;
            
            Assert.Equal(StatusCodes.Status404NotFound, res.StatusCode);
            Assert.Equal(ErrorCode.WeekNotFound, body.ErrorCode);
        }

        [Fact]
        public void PostActivity_InvalidWeekNameValidDTO_WeekNotFound()
        {
            ActivityController ac = InitializeTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            Week week = mockUser.WeekSchedule.First();

            ActivityDTO newActivity = new ActivityDTO() { 
                Pictograms = new List<WeekPictogramDTO> { new WeekPictogramDTO(_testContext.MockPictograms.First()) }};

            var res = ac.PostActivity(
                newActivity, mockUser.Id, "WrongName", week.WeekYear, week.WeekYear, (int)Days.Sunday
            ).Result as ObjectResult;

            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status404NotFound, res.StatusCode);
            Assert.Equal(ErrorCode.WeekNotFound, body.ErrorCode);
        }

        [Fact]
        public void PostActivity_NonExistingUserValidDayValidDTO_UserNotFound()
        {
            ActivityController ac = InitializeTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            Week week = mockUser.WeekSchedule.First();

            ActivityDTO newActivity = new ActivityDTO() { Pictograms = new List<WeekPictogramDTO> { new WeekPictogramDTO(_testContext.MockPictograms.First()) }};

            var res = ac.PostActivity(
                newActivity, "NonExistingUserId", week.Name, week.WeekYear, week.WeekNumber, (int)Days.Sunday
            ).Result as ObjectResult;
            
            var body = res.Value as ErrorResponse;
            
            Assert.Equal(StatusCodes.Status404NotFound, res.StatusCode);
            Assert.Equal(ErrorCode.UserNotFound, body.ErrorCode);
            
        }

        [Fact]
        public void PostActivity_UserNotAuthorizedValidDayValidDTO_NotAuthorized()
        {
            ActivityController ac = InitializeTest();
            GirafUser mockUser = _testContext.MockUsers[GUARDIAN_DEP_TWO];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            GirafUser differentMockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            Week week = differentMockUser.WeekSchedule.First();

            ActivityDTO newActivity = new ActivityDTO() { 
                Pictograms = new List<WeekPictogramDTO> { new WeekPictogramDTO(_testContext.MockPictograms.First()) 
            }};

            var res = ac.PostActivity(
                newActivity, differentMockUser.Id, week.Name, week.WeekYear, week.WeekNumber, (int)Days.Sunday
            ).Result as ObjectResult;

            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
            Assert.Equal(ErrorCode.NotAuthorized, body.ErrorCode);
        }
        #endregion

        #region GetActivity

        [Fact]
        public void GetActivity_ValidId_success()
        {
            ActivityController activityController = InitializeTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            
            Assert.True(_testContext.MockActivities.Any(a => a.Key == _existingId));

            var response = activityController.GetActivity(mockUser.Id, _existingId);
            Assert.NotNull(response);
            Assert.NotNull(response.Result);
            Assert.True(response.Result.Success);

            var actual = response.Result.Data;
            ActivityDTO expected = new ActivityDTO(_testContext.MockActivities.First());
            
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.IsChoiceBoard, actual.IsChoiceBoard);
            Assert.Equal(expected.Order, actual.Order);
            Assert.Equal(expected.State, actual.State);
            Assert.Equal(expected.Timer, actual.Timer);
            Assert.Equal(expected.Pictograms.Count, actual.Pictograms.Count);

            var actualPictogram = actual.Pictograms.First();
            var expectedPictogram = _testContext.MockPictograms[0];
            
            Assert.Equal(expectedPictogram.Id, actualPictogram.Id);
        }

        #endregion

        #region DeleteActivity
        [Fact]
        public void DeleteActivity_ExistingActivity_Success()
        {
            ActivityController activityController = InitializeTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);

            // There exist an acitivity with the id
            Assert.True(_testContext.MockActivities.Any(a => a.Key == _existingId));

            var res = activityController.DeleteActivity(
                mockUser.Id, _existingId
            ).Result as ObjectResult;
            
            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
        }

        [Fact]
        public void DeleteActivity_NonExistingActivity_Failure()
        {
            ActivityController activityController = InitializeTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);

            var res = activityController.DeleteActivity(
                mockUser.Id, _nonExistingId
            ).Result as ObjectResult;

            var body = res.Value as ErrorResponse;
            
            Assert.Equal(StatusCodes.Status404NotFound, res.StatusCode);
            Assert.Equal(ErrorCode.ActivityNotFound , body.ErrorCode);
        }

        [Fact]
        public void DeleteActivity_NonExistingUser_Failure()
        {
            ActivityController activityController = InitializeTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];

            var res = activityController.DeleteActivity(
                "NonExistingUserId", _existingId
            ).Result as ObjectResult;

            var body = res.Value as ErrorResponse;
            
            Assert.Equal(StatusCodes.Status404NotFound, res.StatusCode);
            Assert.Equal(ErrorCode.UserNotFound, body.ErrorCode);
        }

        [Fact]
        public void DeleteActivity_UnauthorizedUserSendsIdOfActivitysOwner_Failure()
        {
            ActivityController activityController = InitializeTest();
            GirafUser loggedInUser = _testContext.MockUsers[GUARDIAN_DEP_TWO];
            _testContext.MockUserManager.MockLoginAsUser(loggedInUser);
            GirafUser activityOwnerUser = _testContext.MockUsers[ADMIN_DEP_ONE];

            // logged in as GUARDIAN_DEP_TWO and trying to delete one of ADMIN_DEP_ONE's activities
            var res = activityController.DeleteActivity(
                activityOwnerUser.Id, _existingId
            ).Result as ObjectResult;
            
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
            Assert.Equal(ErrorCode.NotAuthorized, body.ErrorCode);
        }

        [Fact]
        public void DeleteActivity_UnauthorizedUserSendsIdOfLoggedInUser_Failure()
        {
            ActivityController activityController = InitializeTest();
            GirafUser loggedInUser = _testContext.MockUsers[GUARDIAN_DEP_TWO];
            _testContext.MockUserManager.MockLoginAsUser(loggedInUser);

            // logged in as GUARDIAN_DEP_TWO and trying to delete one of ADMIN_DEP_ONES activities
            var res = activityController.DeleteActivity(loggedInUser.Id, _existingId).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status404NotFound, res.StatusCode);
            Assert.Equal(ErrorCode.ActivityNotFound, body.ErrorCode);
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

            var res = ac.UpdateActivity(newActivity, mockUser.Id).Result as ObjectResult;

            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal(ErrorCode.MissingProperties, body.ErrorCode);
        }

        [Fact]
        public void UpdateActivity_InvalidActivityValidDTO_ActivityNotFound()
        {
            ActivityController ac = InitializeTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);

            ActivityDTO newActivity = new ActivityDTO() { Id = 420 };

            var res = ac.UpdateActivity(newActivity, mockUser.Id).Result as ObjectResult;

            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status404NotFound, res.StatusCode);
            Assert.Equal(ErrorCode.ActivityNotFound, body.ErrorCode);
        }

        [Fact]
        public void UpdateActivity_ValidActivityInvalidUser_ActivityNotFound()
        {
            ActivityController ac = InitializeTest();
            GirafUser mockUser = _testContext.MockUsers[GUARDIAN_DEP_TWO];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            GirafUser differentMockUser = _testContext.MockUsers[ADMIN_DEP_ONE];

            ActivityDTO newActivity = new ActivityDTO() { 
                Pictograms = new List<WeekPictogramDTO> { new WeekPictogramDTO(_testContext.MockPictograms.First()) 
            }};

            var res = ac.UpdateActivity(newActivity, differentMockUser.Id).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
            Assert.Equal(ErrorCode.NotAuthorized, body.ErrorCode);
        }

        [Fact]
        public void UpdateActivity_ValidActivityValidDTO_ActivitySucess()
        {
            ActivityController ac = InitializeTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE]; 
            _testContext.MockUserManager.MockLoginAsUser(mockUser);

            ActivityDTO newActivity = new ActivityDTO() { 
                Id = _testContext.MockActivities.First().Key, 
                Pictograms = new List<WeekPictogramDTO> { new WeekPictogramDTO(_testContext.MockPictograms.First()) }
            };

            var res = ac.UpdateActivity(newActivity, mockUser.Id).Result as ObjectResult;
            var body = res.Value as SuccessResponse<ActivityDTO>;

            List<long> expectedPictogramIds = newActivity.Pictograms.Select(pictogram => pictogram.Id).ToList();
            List<long> actualPictogramIds = res.Data.Pictograms.Select(pictogram => pictogram.Id).ToList();

            Assert.True(res.Success);
            Assert.Equal(expectedPictogramIds, actualPictogramIds);
        }

        [Fact]
        public void UpdateActivity_ValidActivityValidDTOWithMulitplePictograms_Success()
        {
            ActivityController ac = InitializeTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE]; 
            _testContext.MockUserManager.MockLoginAsUser(mockUser);

            ActivityDTO newActivity = new ActivityDTO() { 
                Id = _testContext.MockActivities.First().Key, 
                Pictograms = new List<WeekPictogramDTO> { 
                    new WeekPictogramDTO(_testContext.MockPictograms.First()),
                    new WeekPictogramDTO(_testContext.MockPictograms.Last())
                }
            };

            var res = ac.UpdateActivity(newActivity, mockUser.Id).Result;

            List<long> expectedPictogramIds = newActivity.Pictograms.Select(pictogram => pictogram.Id).ToList();
            List<long> actualPictogramIds = res.Data.Pictograms.Select(pictogram => pictogram.Id).ToList();

            Assert.True(res.Success);
            Assert.Equal(expectedPictogramIds, actualPictogramIds);
        }
        #endregion
    }
}
