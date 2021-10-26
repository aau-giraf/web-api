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
using Microsoft.AspNetCore.Identity;
using Moq;
using GirafRest.IRepositories;
using GirafRest.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

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

        private TestContext _testContext = new TestContext();

        private ActivityController InitializeTest()
        {
            _testContext = new TestContext();

            //Ensures that the mock user has the mock activity in the user's weekplan
            _testContext.MockDbContext.Object.Users.First().WeekSchedule.First().Weekdays
                .First().Activities.Add(_testContext.MockActivities.Find(a => a.Key == _existingId));

            var ac = new MockActivityController();
            _testContext.MockHttpContext = ac.MockHttpContext();
            return ac;
        }

        public class MockActivityController : ActivityController
        {
            public MockActivityController()
                : this(
                    new Mock<ILoggerFactory>(),
                    new Mock<IGirafUserRepository>(),
                    new Mock<IAlternateNameRepository>(),
                    new Mock<IActivityRepository>(),
                    new Mock<IWeekdayRepository>(),
                    new Mock<IPictogramRepository>(),
                    new Mock<IPictogramRelationRepository>(),
                    new Mock<ITimerRepository>()
                )
            { }

            public MockActivityController(
            Mock<ILoggerFactory> loggerFactory,
            Mock<IGirafUserRepository> userRepository,
            Mock<IAlternateNameRepository> alternateNameRepository,
            Mock<IActivityRepository> activityRepository,
            Mock<IWeekdayRepository> weekdayRepository,
            Mock<IPictogramRepository> pictogramRepository,
            Mock<IPictogramRelationRepository> pictogramRelationRepository,
            Mock<ITimerRepository> timerRepository)
                : base(
                    loggerFactory.Object,
                    userRepository.Object,
                    alternateNameRepository.Object,
                    activityRepository.Object,
                    weekdayRepository.Object,
                    pictogramRepository.Object,
                    pictogramRelationRepository.Object,
                    timerRepository.Object
                )
            {
                LoggerFactory = loggerFactory;
                UserRepository = userRepository;
                AlternateNameRepository = alternateNameRepository;
                ActivityRepository = activityRepository;
                WeekdayRepository = weekdayRepository;
                PictogramRepository = pictogramRepository;
                PictogramRelationRepository = pictogramRelationRepository;
                TimerRepository = timerRepository;
            }

            public Mock<ILoggerFactory> LoggerFactory { get; }
            public Mock<IGirafUserRepository> UserRepository { get; }
            public Mock<IAlternateNameRepository> AlternateNameRepository { get; }
            public Mock<IActivityRepository> ActivityRepository { get; }
            public Mock<IWeekdayRepository> WeekdayRepository { get; }
            public Mock<IPictogramRepository> PictogramRepository { get; }
            public Mock<IPictogramRelationRepository> PictogramRelationRepository { get; }
            public Mock<ITimerRepository> TimerRepository { get; }
        }

        #region CreateActivity

        [Fact]
        public void PostActivity_ValidDayValidDTOWithOnePictogram_Succes()
        {
            // Arrange
            var activityController = new MockActivityController();

            // Mock
            List<Weekday> weekdays = new List<Weekday>()
            {
                new Weekday() { Day = Days.Monday },
                new Weekday() { Day = Days.Tuesday },
                new Weekday() { Day = Days.Wednesday },
                new Weekday() { Day = Days.Thursday },
                new Weekday() { Day = Days.Friday },
                new Weekday() { Day = Days.Saturday },
                new Weekday() { Day = Days.Sunday }
            };
            Week week = new Week()
            {
                Id = 0,
                WeekYear = 2000,
                WeekNumber = 1,
                Name = "week",
                Weekdays = weekdays
            };
            GirafUser user = new GirafUser()
            {
                UserName = "user1",
                DisplayName = "user1",
                Id = "u1",
                DepartmentKey = 1,
                WeekSchedule = new List<Week> { week }
            };
            Pictogram pictogram1 = new Pictogram
            {
                Id = 0,
                Title = "Pictogram1",
                AccessLevel = AccessLevel.PUBLIC
            };
            ActivityDTO activity = new ActivityDTO()
            {
                Pictograms = new List<WeekPictogramDTO>
                {
                    new WeekPictogramDTO(pictogram1)
                }
            };

            activityController.UserRepository.Setup(rep => rep.GetWithWeekSchedules(user.Id)).Returns(user);
            activityController.PictogramRepository.Setup(rep => rep.Get(pictogram1.Id)).Returns(pictogram1);

            // Act
            ICollection<WeekPictogramDTO> expected = activity.Pictograms;
            ICollection<WeekPictogramDTO> actual = ((activityController.PostActivity(activity, user.Id, week.Name, week.WeekYear, week.WeekNumber, (int)weekdays[0].Day)
                .Result as ObjectResult)
                .Value as SuccessResponse<ActivityDTO>)
                .Data.Pictograms;

            // Assert
            activityController.UserRepository.Verify(rep => rep.GetWithWeekSchedules(user.Id), Times.Once);
            activityController.PictogramRepository.Verify(rep => rep.Get(pictogram1.Id), Times.Once);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void PostActivity_ValidDayValidDTOWithMultiplePictograms_Success()
        {
            // Arrange
            var activityController = new MockActivityController();

            // Mock
            List<Weekday> weekdays = new List<Weekday>()
            {
                new Weekday() { Day = Days.Monday },
                new Weekday() { Day = Days.Tuesday },
                new Weekday() { Day = Days.Wednesday },
                new Weekday() { Day = Days.Thursday },
                new Weekday() { Day = Days.Friday },
                new Weekday() { Day = Days.Saturday },
                new Weekday() { Day = Days.Sunday }
            };
            Week week = new Week()
            {
                Id = 0,
                WeekYear = 2000,
                WeekNumber = 1,
                Name = "week",
                Weekdays = weekdays
            };
            GirafUser user = new GirafUser()
            {
                UserName = "user1",
                DisplayName = "user1",
                Id = "u1",
                DepartmentKey = 1,
                WeekSchedule = new List<Week> { week }
            };
            Pictogram pictogram1 = new Pictogram
            {
                Id = 0,
                Title = "Pictogram1",
                AccessLevel = AccessLevel.PUBLIC
            };
            Pictogram pictogram2 = new Pictogram
            {
                Id = 1,
                Title = "Pictogram2",
                AccessLevel = AccessLevel.PUBLIC
            };
            ActivityDTO activity = new ActivityDTO()
            {
                Pictograms = new List<WeekPictogramDTO>
                {
                    new WeekPictogramDTO(pictogram1),
                    new WeekPictogramDTO(pictogram2)
                }
            };

            activityController.UserRepository.Setup(rep => rep.GetWithWeekSchedules(user.Id)).Returns(user);
            activityController.PictogramRepository.Setup(rep => rep.Get(pictogram1.Id)).Returns(pictogram1);
            activityController.PictogramRepository.Setup(rep => rep.Get(pictogram2.Id)).Returns(pictogram2);

            // Act
            ICollection<WeekPictogramDTO> expected = activity.Pictograms;
            ICollection<WeekPictogramDTO> actual = ((activityController.PostActivity(activity, user.Id, week.Name, week.WeekYear, week.WeekNumber, (int)weekdays[0].Day)
                .Result as ObjectResult)
                .Value as SuccessResponse<ActivityDTO>)
                .Data.Pictograms;

            // Assert
            activityController.UserRepository.Verify(rep => rep.GetWithWeekSchedules(user.Id), Times.Once);
            activityController.PictogramRepository.Verify(rep => rep.Get(pictogram1.Id), Times.Once);
            activityController.PictogramRepository.Verify(rep => rep.Get(pictogram2.Id), Times.Once);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void PostActivity_NoExistingActivitiesOnDayValidDTO_Succes()
        {
            // Arrange
            var activityController = new MockActivityController();

            // Mock
            List<Weekday> weekdays = new List<Weekday>()
            {
                new Weekday() { Day = Days.Monday },
                new Weekday() { Day = Days.Tuesday },
                new Weekday() { Day = Days.Wednesday },
                new Weekday() { Day = Days.Thursday },
                new Weekday() { Day = Days.Friday },
                new Weekday() { Day = Days.Saturday },
                new Weekday() { Day = Days.Sunday }
            };
            Week week = new Week()
            {
                Id = 0,
                WeekYear = 2000,
                WeekNumber = 1,
                Name = "week",
                Weekdays = weekdays
            };
            GirafUser user = new GirafUser()
            {
                UserName = "user1",
                DisplayName = "user1",
                Id = "u1",
                DepartmentKey = 1,
                WeekSchedule = new List<Week> { week }
            };
            Pictogram pictogram1 = new Pictogram
            {
                Id = 0,
                Title = "Pictogram1",
                AccessLevel = AccessLevel.PUBLIC
            };
            ActivityDTO activity = new ActivityDTO()
            {
                Pictograms = new List<WeekPictogramDTO>
                {
                    new WeekPictogramDTO(pictogram1)
                }
            };

            activityController.UserRepository.Setup(rep => rep.GetWithWeekSchedules(user.Id)).Returns(user);
            activityController.PictogramRepository.Setup(rep => rep.Get(pictogram1.Id)).Returns(pictogram1);

            // Act
            ICollection<WeekPictogramDTO> expected = activity.Pictograms;
            var actual = (activityController.PostActivity(activity, user.Id, week.Name, week.WeekYear, week.WeekNumber, (int)weekdays[0].Day)
                .Result as ObjectResult).StatusCode;

            // Assert
            activityController.UserRepository.Verify(rep => rep.GetWithWeekSchedules(user.Id), Times.Once);
            activityController.PictogramRepository.Verify(rep => rep.Get(pictogram1.Id), Times.Once);
            //Assert.Equal(expected, actual);

            Assert.Equal(StatusCodes.Status201Created, actual);
            //Assert.Equal(newActivity.Pictograms, body.Data.Pictograms);
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
        public void PostActivity_ValidDayInvalidDTO_InvalidProperties()
        {
            ActivityController ac = InitializeTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            Week week = mockUser.WeekSchedule.First();

            ActivityDTO newActivity = new ActivityDTO()
            {
                Pictograms = new List<WeekPictogramDTO>()
               {
                   new WeekPictogramDTO(_testContext.MockPictograms[10])
               }
            };

            var res = ac.PostActivity(
                newActivity, mockUser.Id, week.Name, week.WeekYear, week.WeekNumber, (int)Days.Monday
            ).Result as ObjectResult;

            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal(ErrorCode.InvalidProperties, body.ErrorCode);
        }

        [Fact]
        public void PostActivity_InvalidDayValidDTO_InvalidDay()
        {
            ActivityController ac = InitializeTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            Week week = mockUser.WeekSchedule.First();

            ActivityDTO newActivity = new ActivityDTO()
            {
                Pictograms = new List<WeekPictogramDTO>
                {
                    new WeekPictogramDTO(_testContext.MockPictograms.First())
                }
            };

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

            ActivityDTO newActivity = new ActivityDTO()
            { Pictograms = new List<WeekPictogramDTO> { new WeekPictogramDTO(_testContext.MockPictograms.First()) } };

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

            ActivityDTO newActivity = new ActivityDTO()
            { Pictograms = new List<WeekPictogramDTO> { new WeekPictogramDTO(_testContext.MockPictograms.First()) } };

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

            ActivityDTO newActivity = new ActivityDTO()
            {
                Pictograms = new List<WeekPictogramDTO> { new WeekPictogramDTO(_testContext.MockPictograms.First()) }
            };

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

            ActivityDTO newActivity = new ActivityDTO()
            { Pictograms = new List<WeekPictogramDTO> { new WeekPictogramDTO(_testContext.MockPictograms.First()) } };

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

            ActivityDTO newActivity = new ActivityDTO()
            {
                Pictograms = new List<WeekPictogramDTO>
                {
                    new WeekPictogramDTO(_testContext.MockPictograms.First())
                }
            };

            var res = ac.PostActivity(
                newActivity, differentMockUser.Id, week.Name, week.WeekYear, week.WeekNumber, (int)Days.Sunday
            ).Result as ObjectResult;

            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
            Assert.Equal(ErrorCode.NotAuthorized, body.ErrorCode);
        }

        [Fact]
        public void PostActivity_Have_AlternateName_Sets_Title()
        {
            ActivityController ac = InitializeTest();
            GirafUser mockUser = _testContext.MockUsers.First();
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            Week week = mockUser.WeekSchedule.First();

            ActivityDTO newActivity = new ActivityDTO()
            {
                Pictograms = new List<WeekPictogramDTO>()
                {
                    new WeekPictogramDTO(_testContext.MockPictograms.First())
                }
            };

            var res = ac.PostActivity(
                newActivity, mockUser.Id, week.Name, week.WeekYear, week.WeekNumber, (int)Days.Sunday
            ).Result as ObjectResult;

            var body = res.Value as SuccessResponse<ActivityDTO>;


            Assert.Equal(StatusCodes.Status201Created, res.StatusCode);
            Assert.Equal("Kage", body.Data.Title);

        }

        [Fact]
        public void PostActivity_No_AlternateName_Sets_Title()
        {
            ActivityController ac = InitializeTest();
            GirafUser mockUser = _testContext.MockUsers.First();
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            Week week = mockUser.WeekSchedule.First();
            Pictogram pic = _testContext.MockPictograms[1];

            ActivityDTO newActivity = new ActivityDTO()
            {
                Pictograms = new List<WeekPictogramDTO>()
                {
                    new WeekPictogramDTO(pic)
                }
            };

            var res = ac.PostActivity(
                newActivity, mockUser.Id, week.Name, week.WeekYear, week.WeekNumber, (int)Days.Sunday
            ).Result as ObjectResult;

            var body = res.Value as SuccessResponse<ActivityDTO>;

            Assert.Equal(StatusCodes.Status201Created, res.StatusCode);
            Assert.Equal(pic.Title, body.Data.Title);
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

            var response = activityController.GetActivity(mockUser.Id, _existingId).Result as ObjectResult;
            var body = response.Value as SuccessResponse<ActivityDTO>;

            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);

            var actual = body.Data;
            ActivityDTO expected = new ActivityDTO(_testContext.MockActivities.First());

            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.IsChoiceBoard, actual.IsChoiceBoard);
            Assert.Equal(expected.Order, actual.Order);
            Assert.Equal(expected.State, actual.State);
            Assert.Equal(expected.Timer.ToString(), actual.Timer.ToString());
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
            Assert.Equal(ErrorCode.ActivityNotFound, body.ErrorCode);
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

            ActivityDTO newActivity = new ActivityDTO()
            {
                Pictograms = new List<WeekPictogramDTO>
                {
                    new WeekPictogramDTO(_testContext.MockPictograms.First())
                }
            };

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

            ActivityDTO newActivity = new ActivityDTO()
            {
                Id = _testContext.MockActivities.First().Key,
                Pictograms = new List<WeekPictogramDTO> { new WeekPictogramDTO(_testContext.MockPictograms.First()) }
            };

            var res = ac.UpdateActivity(newActivity, mockUser.Id).Result as ObjectResult;
            var body = res.Value as SuccessResponse<ActivityDTO>;

            List<long> expectedPictogramIds = newActivity.Pictograms.Select(pictogram => pictogram.Id).ToList();
            List<long> actualPictogramIds = body.Data.Pictograms.Select(pictogram => pictogram.Id).ToList();

            Assert.Equal(expectedPictogramIds, actualPictogramIds);
        }

        [Fact]
        public void UpdateActivity_ValidActivityValidDTOWithMulitplePictograms_Success()
        {
            ActivityController ac = InitializeTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);

            ActivityDTO newActivity = new ActivityDTO()
            {
                Id = _testContext.MockActivities.First().Key,
                Pictograms = new List<WeekPictogramDTO>
                {
                    new WeekPictogramDTO(_testContext.MockPictograms.First()),
                    new WeekPictogramDTO(_testContext.MockPictograms.Last())
                }
            };

            var res = ac.UpdateActivity(newActivity, mockUser.Id).Result as ObjectResult;
            var body = res.Value as SuccessResponse<ActivityDTO>;

            List<long> expectedPictogramIds = newActivity.Pictograms.Select(pictogram => pictogram.Id).ToList();
            List<long> actualPictogramIds = body.Data.Pictograms.Select(pictogram => pictogram.Id).ToList();

            Assert.Equal(expectedPictogramIds, actualPictogramIds);
        }

        #endregion
    }
}