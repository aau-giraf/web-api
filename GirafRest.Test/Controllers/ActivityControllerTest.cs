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

namespace GirafRest.Test
{
    public class ActivityControllerTest
    {
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

        #region PostActivity
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
            int expected = StatusCodes.Status201Created;
            int? actual = (activityController.PostActivity(activity, user.Id, week.Name, week.WeekYear, week.WeekNumber, (int)weekdays[0].Day)
                .Result as ObjectResult).StatusCode;

            // Assert
            activityController.UserRepository.Verify(rep => rep.GetWithWeekSchedules(user.Id), Times.Once);
            activityController.PictogramRepository.Verify(rep => rep.Get(pictogram1.Id), Times.Once);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void PostActivity_ValidDayInvalidDTO_MissingProperties()
        {
            // Arrange
            var activityController = new MockActivityController();

            // Mock
            ActivityDTO activity = null;

            // Act
            ObjectResult actual = activityController.PostActivity(activity, default, default, default, default, default)
                .Result as ObjectResult;

            // Assert
            Assert.Equal(StatusCodes.Status400BadRequest, actual.StatusCode);
            Assert.Equal(ErrorCode.MissingProperties, (actual.Value as ErrorResponse).ErrorCode);
        }

        [Fact]
        public void PostActivity_ValidDayInvalidDTO_InvalidProperties()
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
                Title = "",
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
            ObjectResult actual = activityController.PostActivity(activity, user.Id, week.Name, week.WeekYear, week.WeekNumber, (int)weekdays[0].Day)
                .Result as ObjectResult;

            // Assert
            activityController.UserRepository.Verify(rep => rep.GetWithWeekSchedules(user.Id), Times.Once);
            activityController.PictogramRepository.Verify(rep => rep.Get(pictogram1.Id), Times.Once);
            Assert.Equal(StatusCodes.Status400BadRequest, actual.StatusCode);
            Assert.Equal(ErrorCode.InvalidProperties, (actual.Value as ErrorResponse).ErrorCode);
        }

        [Fact]
        public void PostActivity_InvalidDayValidDTO_InvalidDay()
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

            // Act
            ObjectResult actual = activityController.PostActivity(activity, user.Id, week.Name, week.WeekYear, week.WeekNumber, 69)
                .Result as ObjectResult;

            // Assert
            activityController.UserRepository.Verify(rep => rep.GetWithWeekSchedules(user.Id), Times.Once);
            Assert.Equal(StatusCodes.Status404NotFound, actual.StatusCode);
            Assert.Equal(ErrorCode.InvalidDay, (actual.Value as ErrorResponse).ErrorCode);
        }

        [Fact]
        public void PostActivity_WeekScheduleEmpty_WeekNotFound()
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
                WeekSchedule = new List<Week>()
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

            // Act
            ObjectResult actual = activityController.PostActivity(activity, user.Id, week.Name, week.WeekYear, week.WeekNumber, 69)
                .Result as ObjectResult;

            // Assert
            activityController.UserRepository.Verify(rep => rep.GetWithWeekSchedules(user.Id), Times.Once);
            Assert.Equal(StatusCodes.Status404NotFound, actual.StatusCode);
            Assert.Equal(ErrorCode.WeekNotFound, (actual.Value as ErrorResponse).ErrorCode);
        }

        [Fact]
        public void PostActivity_NonExistingUserValidDayValidDTO_UserNotFound()
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

            // Act
            ObjectResult actual = activityController.PostActivity(activity, "user", week.Name, week.WeekYear, week.WeekNumber, (int)weekdays[0].Day)
                .Result as ObjectResult;

            // Assert
            activityController.UserRepository.Verify(rep => rep.GetWithWeekSchedules("user"), Times.Once);
            Assert.Equal(StatusCodes.Status404NotFound, actual.StatusCode);
            Assert.Equal(ErrorCode.UserNotFound, (actual.Value as ErrorResponse).ErrorCode);
        }

        [Fact]
        public void PostActivity_Have_AlternateName_Sets_Title()
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
            AlternateName alternateName = new AlternateName
            {
                Citizen = user,
                Pictogram = pictogram1,
                Name = "newPictogramName"
            };
            ActivityDTO activity = new ActivityDTO()
            {
                Pictograms = new List<WeekPictogramDTO>
                {
                    new WeekPictogramDTO(pictogram1)
                }
            };

            activityController.UserRepository.Setup(rep => rep.GetWithWeekSchedules(user.Id)).Returns(user);
            activityController.AlternateNameRepository.Setup(rep => rep.SingleOrDefault(alternateName
                => alternateName.Citizen == user
                && alternateName.PictogramId == activity.Pictograms.First().Id)).Returns(alternateName);
            activityController.PictogramRepository.Setup(rep => rep.Get(pictogram1.Id)).Returns(pictogram1);

            // Act
            ObjectResult actual = activityController.PostActivity(activity, user.Id, week.Name, week.WeekYear, week.WeekNumber, (int)weekdays[0].Day)
                .Result as ObjectResult;

            // Assert
            activityController.UserRepository.Verify(rep => rep.GetWithWeekSchedules(user.Id), Times.Once);
            activityController.PictogramRepository.Verify(rep => rep.Get(pictogram1.Id), Times.Once);
            Assert.Equal(StatusCodes.Status201Created, actual.StatusCode);
            Assert.Equal("newPictogramName", (actual.Value as SuccessResponse<ActivityDTO>).Data.Title);
        }

        [Fact]
        public void PostActivity_No_AlternateName_Sets_Title()
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
            ObjectResult actual = activityController.PostActivity(activity, user.Id, week.Name, week.WeekYear, week.WeekNumber, (int)weekdays[0].Day)
                .Result as ObjectResult;

            // Assert
            activityController.UserRepository.Verify(rep => rep.GetWithWeekSchedules(user.Id), Times.Once);
            activityController.PictogramRepository.Verify(rep => rep.Get(pictogram1.Id), Times.Once);
            Assert.Equal(StatusCodes.Status201Created, actual.StatusCode);
            Assert.Equal("Pictogram1", (actual.Value as SuccessResponse<ActivityDTO>).Data.Title);
        }
        #endregion

        #region DeleteActivity
        [Fact]
        public void DeleteActivity_ExistingActivity_Success()
        {
            // Arrange
            var activityController = new MockActivityController();

            // Mock
            Activity activity = new Activity()
            {
                Key = 1
            };
            List<Weekday> weekdays = new List<Weekday>()
            {
                new Weekday() { Day = Days.Monday },
                new Weekday() { Day = Days.Tuesday },
                new Weekday()
                {
                    Day = Days.Wednesday,
                    Activities = new List<Activity> { activity }
                },
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
            List<PictogramRelation> pictogramRelations = new List<PictogramRelation>()
            {
                new PictogramRelation()
                {
                    ActivityId = activity.Key
                }
            };

            activityController.UserRepository.Setup(rep => rep.GetWithWeekSchedules(user.Id)).Returns(user);
            activityController.ActivityRepository.Setup(rep => rep.Get(activity.Key)).Returns(activity);
            activityController.PictogramRelationRepository.Setup(rep => rep.Find(relation => relation.ActivityId == activity.Key)).Returns(pictogramRelations);

            // Act
            ObjectResult actual = activityController.DeleteActivity(user.Id, activity.Key)
                .Result as ObjectResult;

            // Assert
            activityController.UserRepository.Verify(rep => rep.GetWithWeekSchedules(user.Id), Times.Once);
            activityController.ActivityRepository.Verify(rep => rep.Get(activity.Key), Times.Once);
            activityController.PictogramRelationRepository.Verify(rep => rep.RemoveRange(pictogramRelations), Times.Once);
            activityController.ActivityRepository.Verify(rep => rep.Remove(activity), Times.Once);
            Assert.Equal(StatusCodes.Status200OK, actual.StatusCode);
        }

        [Fact]
        public void DeleteActivity_NonExistingActivity_Failure()
        {
            // Arrange
            var activityController = new MockActivityController();

            // Mock
            GirafUser user = new GirafUser()
            {
                UserName = "user1",
                DisplayName = "user1",
                Id = "u1",
                DepartmentKey = 1,
                WeekSchedule = new List<Week> { }
            };

            activityController.UserRepository.Setup(rep => rep.GetWithWeekSchedules(user.Id)).Returns(user);

            // Act
            ObjectResult actual = activityController.DeleteActivity(user.Id, 1)
                .Result as ObjectResult;

            // Assert
            activityController.UserRepository.Verify(rep => rep.GetWithWeekSchedules(user.Id), Times.Once);
            Assert.Equal(StatusCodes.Status404NotFound, actual.StatusCode);
            Assert.Equal(ErrorCode.ActivityNotFound, (actual.Value as ErrorResponse).ErrorCode);
        }

        [Fact]
        public void DeleteActivity_NonExistingUser_Failure()
        {
            // Arrange
            var activityController = new MockActivityController();

            // Mock

            // Act
            ObjectResult actual = activityController.DeleteActivity("user1", 1)
                .Result as ObjectResult;

            // Assert
            activityController.UserRepository.Verify(rep => rep.GetWithWeekSchedules("user1"), Times.Once);
            Assert.Equal(StatusCodes.Status404NotFound, actual.StatusCode);
            Assert.Equal(ErrorCode.UserNotFound, (actual.Value as ErrorResponse).ErrorCode);
        }
        #endregion

        #region GetActivity
        [Fact]
        public void GetActivity_ValidId_success()
        {
            // Arrange
            var activityController = new MockActivityController();

            // Mock
            Activity activity = new Activity()
            {
                Key = 1
            };
            List<PictogramRelation> pictogramRelations = new List<PictogramRelation>()
            {
                new PictogramRelation()
                {
                    Pictogram = new Pictogram
                    {
                        Title = "pictogram1"
                    },
                    ActivityId = activity.Key
                }
            };

            activityController.ActivityRepository.Setup(rep => rep.Get((long)activity.Key)).Returns(activity);
            activityController.PictogramRelationRepository.Setup(rep => rep.GetWithPictogram((long)activity.Key)).Returns(pictogramRelations);

            // Act
            ObjectResult actual = activityController.GetActivity("user1", (long)activity.Key)
                .Result as ObjectResult;

            // Assert
            activityController.ActivityRepository.Verify(rep => rep.Get((long)activity.Key), Times.Once);
            activityController.PictogramRelationRepository.Verify(rep => rep.GetWithPictogram((long)activity.Key), Times.Once);
            Assert.Equal(StatusCodes.Status200OK, actual.StatusCode);
            Assert.Equal(pictogramRelations.First().Pictogram.Title, (actual.Value as SuccessResponse<ActivityDTO>).Data.Pictograms.First().Title);
        }
        #endregion

        #region UpdateActivity
        [Fact]
        public void UpdateActivity_InvalidActivityDTO_MissingProperties()
        {
            // Arrange
            var activityController = new MockActivityController();

            // Mock
            
            // Act
            ObjectResult actual = activityController.UpdateActivity(null, "user1")
                .Result as ObjectResult;

            // Assert
            Assert.Equal(StatusCodes.Status400BadRequest, actual.StatusCode);
            Assert.Equal(ErrorCode.MissingProperties, (actual.Value as ErrorResponse).ErrorCode);
        }

        [Fact]
        public void UpdateActivity_InvalidActivityValidDTO_ActivityNotFound()
        {
            // Arrange
            var activityController = new MockActivityController();

            // Mock
            Activity oldActivity = new Activity()
            {
                Key = 0,
                Title = "oldActivity"
            };
            ActivityDTO newActivity = new ActivityDTO()
            {
                Id = 0,
                Title = "newActivity",
                Pictograms = new List<WeekPictogramDTO>()
            };
            List<Weekday> weekdays = new List<Weekday>()
            {
                new Weekday() { Day = Days.Monday },
                new Weekday() { Day = Days.Tuesday },
                new Weekday()
                {
                    Day = Days.Wednesday,
                    Activities = new List<Activity> { oldActivity }
                },
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

            activityController.UserRepository.Setup(rep => rep.GetWithWeekSchedules(user.Id)).Returns(user);
            // Act
            ObjectResult actual = activityController.UpdateActivity(newActivity, user.Id)
                .Result as ObjectResult;

            // Assert
            activityController.UserRepository.Verify(rep => rep.GetWithWeekSchedules(user.Id), Times.Once);
            activityController.ActivityRepository.Verify(rep => rep.Get(newActivity.Id), Times.Once);
            Assert.Equal(StatusCodes.Status404NotFound, actual.StatusCode);
            Assert.Equal(ErrorCode.ActivityNotFound, (actual.Value as ErrorResponse).ErrorCode);
        }

        [Fact]
        public void UpdateActivity_InvalidUser_UserNotFound()
        {
            // Arrange
            var activityController = new MockActivityController();

            // Mock
            ActivityDTO newActivity = new ActivityDTO()
            {
                Id = 0,
                Title = "newActivity",
                Pictograms = new List<WeekPictogramDTO>()
            };

            // Act
            ObjectResult actual = activityController.UpdateActivity(newActivity, "user1")
                .Result as ObjectResult;

            // Assert
            activityController.UserRepository.Verify(rep => rep.GetWithWeekSchedules("user1"), Times.Once);
            Assert.Equal(StatusCodes.Status404NotFound, actual.StatusCode);
            Assert.Equal(ErrorCode.UserNotFound, (actual.Value as ErrorResponse).ErrorCode);
        }

        [Fact]
        public void UpdateActivity_ValidActivityValidDTO_ActivitySucess()
        {
            // Arrange
            var activityController = new MockActivityController();

            // Mock
            Activity oldActivity = new Activity()
            {
                Key = 0,
                Title = "oldActivity"
            };
            ActivityDTO newActivity = new ActivityDTO()
            {
                Id = 0,
                Title = "newActivity",
                Pictograms = new List<WeekPictogramDTO>()
            };
            List<PictogramRelation> pictogramRelations = new List<PictogramRelation>()
            {
                new PictogramRelation()
                {
                    ActivityId = oldActivity.Key
                }
            };
            List<Weekday> weekdays = new List<Weekday>()
            {
                new Weekday() { Day = Days.Monday },
                new Weekday() { Day = Days.Tuesday },
                new Weekday()
                {
                    Day = Days.Wednesday,
                    Activities = new List<Activity> { oldActivity }
                },
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

            activityController.UserRepository.Setup(rep => rep.GetWithWeekSchedules(user.Id)).Returns(user);
            activityController.ActivityRepository.Setup(rep => rep.Get(newActivity.Id)).Returns(oldActivity);
            activityController.PictogramRelationRepository.Setup(rep => rep.Find(relation => relation.ActivityId == newActivity.Id)).Returns(pictogramRelations);

            // Act
            ObjectResult actual = activityController.UpdateActivity(newActivity, user.Id)
                .Result as ObjectResult;

            // Assert
            activityController.UserRepository.Verify(rep => rep.GetWithWeekSchedules(user.Id), Times.Once);
            activityController.ActivityRepository.Verify(rep => rep.Get(newActivity.Id), Times.Once);
            activityController.PictogramRelationRepository.Verify(rep => rep.RemoveRange(pictogramRelations), Times.Once);
            Assert.Equal(StatusCodes.Status200OK, actual.StatusCode);
            Assert.Equal(newActivity.Title, (actual.Value as SuccessResponse<ActivityDTO>).Data.Title);
        }
        #endregion
    }
}