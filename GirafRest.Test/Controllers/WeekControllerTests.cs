using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using GirafRest.Test.Mocks;
using GirafRest.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GirafRest.Models.Responses;
using Microsoft.AspNetCore.Http;
using GirafRest.Models.DTOs;
using System.Linq;
namespace GirafRest.Test.Controllers
{
    public class WeekControllerTests
    {

        [Fact]
        public async void test_ReadFullWeeksSchedules_Succes()
        {
            //Arrange
            var WeekController = new MockedWeekController();

            var user = new GirafUser() { UserName = "Platos" };
            user.WeekSchedule.Add(new Week() { });
            user.WeekSchedule.Add(new Week() { });
            user.WeekSchedule.Add(new Week() { });
            user.WeekSchedule.Add(new Week() { });
            var weekRepository = WeekController.WeekRepository;
            //Mock
            weekRepository.Setup(x => x.getAllWeeksOfUser(user.UserName)).Returns(Task.FromResult(user));
            //Act
            var response = await WeekController.ReadFullWeekSchedules("Platos");
            var objectResult = response as ObjectResult;
            var successResponse = objectResult.Value as SuccessResponse<IEnumerable<WeekDTO>>;
            int count = 0;
            foreach (var item in successResponse.Data)
            {
                count++;
            }
            //Assert
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);
            Assert.NotNull(successResponse.Data);
            Assert.Equal(4, count);
        }
        [Fact]
        public async void test_ReadFullWeeksSchedules_NoUserFound()
        {
            //Arrange
            var WeekController = new MockedWeekController();
            //Mock
            //Act
            var response = await WeekController.ReadFullWeekSchedules("Aristoteles");
            var objectResult = response as ObjectResult;
            var errorResponse = objectResult.Value as ErrorResponse;
            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
            Assert.Equal(ErrorCode.UserNotFound, errorResponse.ErrorKey);
        }
        [Fact]
        public async void test_ReadFullWeeksSchedules_UserWithNoWeeks()
        {
            //Arrange
            var WeekController = new MockedWeekController();

            var user = new GirafUser() { UserName = "Platos" };
            var weekRepository = WeekController.WeekRepository;
            //Mock
            weekRepository.Setup(x => x.getAllWeeksOfUser(user.UserName)).Returns(Task.FromResult(user));
            //Act
            var response = await WeekController.ReadFullWeekSchedules("Platos");
            var objectResult = response as ObjectResult;
            var successResponse = objectResult.Value as SuccessResponse<IEnumerable<WeekDTO>>;
            int count = 0;
            foreach (var item in successResponse.Data)
            {
                count++;
            }
            //Assert
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);
            Assert.NotNull(successResponse.Data);
            Assert.Equal(0, count);
        }
        // Only change here compared to  test_ReadFullWeeksSchedules_Succes() is the cast to WeekNameDTO instead of WeekDTO, and method call ReadWeekSchedules instead of ReadFullWeekSchedules
        [Fact]
        public async void test_ReadWeekSchedules_Succes()
        {
            //Arrange
            var WeekController = new MockedWeekController();

            var user = new GirafUser() { UserName = "Platos" };
            user.WeekSchedule.Add(new Week() { });
            user.WeekSchedule.Add(new Week() { });
            user.WeekSchedule.Add(new Week() { });
            user.WeekSchedule.Add(new Week() { });
            var weekRepository = WeekController.WeekRepository;
            //Mock
            weekRepository.Setup(x => x.getAllWeeksOfUser(user.UserName)).Returns(Task.FromResult(user));
            //Act
            var response = await WeekController.ReadWeekSchedules("Platos");
            var objectResult = response as ObjectResult;
            var successResponse = objectResult.Value as SuccessResponse<IEnumerable<WeekNameDTO>>;
            int count = 0;
            foreach (var item in successResponse.Data)
            {
                count++;
            }
            //Assert
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);
            Assert.NotNull(successResponse.Data);
            Assert.Equal(4, count);
        }
        [Fact]
        public async void test_ReadWeeksSchedules_NoUserFound()
        {
            //Arrange
            var WeekController = new MockedWeekController();
            //Mock
            //Act
            var response = await WeekController.ReadWeekSchedules("Aristoteles");
            var objectResult = response as ObjectResult;
            var errorResponse = objectResult.Value as ErrorResponse;
            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
            Assert.Equal(ErrorCode.UserNotFound, errorResponse.ErrorKey);
        }
        [Fact]
        public async void test_ReadWeeksSchedules_UserWithNoWeeks()
        {
            //Arrange
            var WeekController = new MockedWeekController();

            var user = new GirafUser() { UserName = "Platos" };
            var weekRepository = WeekController.WeekRepository;
            //Mock
            weekRepository.Setup(x => x.getAllWeeksOfUser(user.UserName)).Returns(Task.FromResult(user));
            //Act
            var response = await WeekController.ReadWeekSchedules("Platos");
            var objectResult = response as ObjectResult;
            var successResponse = objectResult.Value as SuccessResponse<IEnumerable<WeekNameDTO>>;
            int count = 0;
            foreach (var item in successResponse.Data)
            {
                count++;
            }
            //Assert
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);
            Assert.NotNull(successResponse.Data);
            Assert.Equal(0, count);
        }
        [Fact]
        public async void test_ReadUsersWeekSchedule_UserNotFound()
        {
            //Arange
            var WeekController = new MockedWeekController();
            //Mock
            //Act
            var response = await WeekController.ReadUsersWeekSchedule("Aristoteles", 2020, 35);
            var objectResult = response as ObjectResult;
            var errorResponse = objectResult.Value as ErrorResponse;
            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
            Assert.Equal(ErrorCode.UserNotFound, errorResponse.ErrorKey);
        }

        // The test mocks 2 calls of getPictoGramMatchingRelation because I want it to fail the second time in the foreach loops. 
        [Fact]
        public async void test_ReadUsersWeekSchedule_PitogramNotFound()
        {
            //Arange
            var WeekController = new MockedWeekController();
            var weekRepository = WeekController.WeekRepository;
            var pictogramRepository = WeekController.PictogramRepository;
            GirafUser user = new GirafUser() { UserName = "Platos" };
            Week week = new Week() { WeekYear = 2020, WeekNumber = 35 };
            Pictogram pictogram1 = new Pictogram("Apple", AccessLevel.PUBLIC, "AppleHash");
            Pictogram pictogram2 = new Pictogram("Banana", AccessLevel.PUBLIC, "BananaHash");
            Weekday weekday1 = new Weekday(Days.Monday, new List<List<Pictogram>> { new List<Pictogram> { pictogram1 } }, new List<ActivityState> { ActivityState.Normal });
            // Ignore that weekday2 gets a pictogram, it is simply to fullfill the call of the activity constructor inside the Weekday Constructor. 
            Weekday weekday2 = new Weekday(Days.Monday, new List<List<Pictogram>> { new List<Pictogram> { pictogram1 } }, new List<ActivityState> { ActivityState.Normal });
            week.Weekdays.Add(weekday1);
            week.Weekdays.Add(weekday2);
            user.WeekSchedule.Add(week);
            PictogramRelation relation1 = user.WeekSchedule.ToList()[0].Weekdays.ToList()[0].Activities.ToList()[0].Pictograms.ToList()[0];
            PictogramRelation relation2 = user.WeekSchedule.ToList()[0].Weekdays.ToList()[1].Activities.ToList()[0].Pictograms.ToList()[0];
            //Mock
            weekRepository.Setup(x => x.LoadUserWithWeekSchedules(user.UserName)).Returns(Task.FromResult(user));
            pictogramRepository.Setup(x => x.getPictogramMatchingRelation(relation1)).Returns(Task.FromResult(pictogram1));
            pictogramRepository.Setup(x => x.getPictogramMatchingRelation(relation2)).Returns(Task.FromResult(null as Pictogram));
            //Mock
            weekRepository.Setup(x => x.LoadUserWithWeekSchedules(user.UserName)).Returns(Task.FromResult(user));
            //Act
            var response = await WeekController.ReadUsersWeekSchedule("Platos", 2020, 35);
            var objectResult = response as ObjectResult;
            var errorResponse = objectResult.Value as ErrorResponse;
            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
            Assert.Equal(ErrorCode.PictogramNotFound, errorResponse.ErrorKey);
        }
        // The constructor of weekday calls a constructer for activities which in turn calls a constructer for pictogramrelations. 
        [Fact]
        public async void test_ReadUsersWeekSchedule_ExistSuccess()
        {
            //Arange
            var WeekController = new MockedWeekController();
            var weekRepository = WeekController.WeekRepository;
            var pictogramRepository = WeekController.PictogramRepository;
            GirafUser user = new GirafUser() { UserName = "Platos" };
            Week week = new Week() { WeekYear = 2020, WeekNumber = 35 };
            Pictogram pictogram1 = new Pictogram("Apple", AccessLevel.PUBLIC, "AppleHash");
            Pictogram pictogram2 = new Pictogram("Banana", AccessLevel.PUBLIC, "BananaHash");
            Weekday weekday1 = new Weekday(Days.Monday, new List<List<Pictogram>> { new List<Pictogram> { pictogram1 } }, new List<ActivityState> { ActivityState.Normal });
            Weekday weekday2 = new Weekday(Days.Monday, new List<List<Pictogram>> { new List<Pictogram> { pictogram2 } }, new List<ActivityState> { ActivityState.Normal });
            week.Weekdays.Add(weekday1);
            week.Weekdays.Add(weekday2);
            user.WeekSchedule.Add(week);
            PictogramRelation relation1 = user.WeekSchedule.ToList()[0].Weekdays.ToList()[0].Activities.ToList()[0].Pictograms.ToList()[0];
            PictogramRelation relation2 = user.WeekSchedule.ToList()[0].Weekdays.ToList()[1].Activities.ToList()[0].Pictograms.ToList()[0];
            //Mock
            weekRepository.Setup(x => x.LoadUserWithWeekSchedules(user.UserName)).Returns(Task.FromResult(user));
            pictogramRepository.Setup(x => x.getPictogramMatchingRelation(relation1)).Returns(Task.FromResult(pictogram1));
            pictogramRepository.Setup(x => x.getPictogramMatchingRelation(relation2)).Returns(Task.FromResult(pictogram2));
            //Act
            var response = await WeekController.ReadUsersWeekSchedule("Platos", 2020, 35);
            var objectResult = response as ObjectResult;
            var successResponse = objectResult.Value as SuccessResponse<WeekDTO>;
            //Assert
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);
            Assert.NotNull(successResponse.Data);
            Assert.Equal(35, successResponse.Data.WeekNumber);
            Assert.Equal(2020, successResponse.Data.WeekYear);
            Assert.Equal(2, successResponse.Data.Days.Count);
        }
        [Fact]
        public async void test_ReadUsersWeekSchedule_DefaultDoes_NOT_ExistInDB()
        {
            //Arrange
            var WeekController = new MockedWeekController();
            var weekRepository = WeekController.WeekRepository;
            var pictogramRepository = WeekController.PictogramRepository;

            GirafUser user = new GirafUser() { UserName = "Platos" };
            Pictogram Default = new Pictogram("default", AccessLevel.PUBLIC);
            //Mock
            weekRepository.Setup(x => x.LoadUserWithWeekSchedules(user.UserName)).Returns(Task.FromResult(user));
            pictogramRepository.SetupSequence(x => x.GetPictogramWithName("default")).Returns(Task.FromResult(null as Pictogram)).Returns(Task.FromResult(Default));
            //Act
            var response = await WeekController.ReadUsersWeekSchedule("Platos", 2050, 20);
            var objectResult = response as ObjectResult;
            var successResponse = objectResult.Value as SuccessResponse<WeekDTO>;
            //Assert
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);
            Assert.NotNull(successResponse.Data);
            Assert.Equal("2050 - 20", successResponse.Data.Name);

        }
        [Fact]
        public async void test_ReadUsersWeekSchedule_DefaultExistInDB()
        {
            //Arrange
            var WeekController = new MockedWeekController();
            var weekRepository = WeekController.WeekRepository;
            var pictogramRepository = WeekController.PictogramRepository;

            GirafUser user = new GirafUser() { UserName = "Platos" };
            Pictogram Default = new Pictogram("default", AccessLevel.PUBLIC);
            //Mock
            weekRepository.Setup(x => x.LoadUserWithWeekSchedules(user.UserName)).Returns(Task.FromResult(user));
            pictogramRepository.SetupSequence(x => x.GetPictogramWithName("default")).Returns(Task.FromResult(Default));
            //Act
            var response = await WeekController.ReadUsersWeekSchedule("Platos", 2050, 20);
            var objectResult = response as ObjectResult;
            var successResponse = objectResult.Value as SuccessResponse<WeekDTO>;
            //Assert
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);
            Assert.NotNull(successResponse.Data);
            Assert.Equal("2050 - 20", successResponse.Data.Name);
        }
        [Fact]
        public async void test_GetWeekDay_BadRequestOverSix()
        {
            //Arrange
            var WeekController = new MockedWeekController();

            //Mock
            //Act
            var response = await WeekController.GetWeekDay("Platos", 2020, 40, 7);
            var objectResult = response as ObjectResult;
            var errorResponse = objectResult.Value as ErrorResponse;
            //Assert
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
            Assert.Equal(ErrorCode.InvalidDay, errorResponse.ErrorKey);
            Assert.Equal("Day must be between 0 and 6", errorResponse.Message);
        }
        [Fact]
        public async void test_GetWeekDay_BadRequestUnderZero()
        {
            //Arrange
            var WeekController = new MockedWeekController();
            //Mock
            //Act
            var response = await WeekController.GetWeekDay("Platos", 2020, 40, -1);
            var objectResult = response as ObjectResult;
            var errorResponse = objectResult.Value as ErrorResponse;
            //Assert
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
            Assert.Equal(ErrorCode.InvalidDay, errorResponse.ErrorKey);
            Assert.Equal("Day must be between 0 and 6", errorResponse.Message);
        }

        [Fact]
        public async void test_GetWeekDay_UserNotFound()
        {

            //Arrange
            var WeekController = new MockedWeekController();
            var weekRepository = WeekController.WeekRepository;
            //Mock
            weekRepository.Setup(x => x.LoadUserWithWeekSchedules("Aristoteles")).Returns(Task.FromResult(null as GirafUser));
            //Act
            var response = await WeekController.GetWeekDay("Aristoteles", 2020, 40, 4);
            var objectResult = response as ObjectResult;
            var errorResponse = objectResult.Value as ErrorResponse;
            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
            Assert.Equal(ErrorCode.UserNotFound, errorResponse.ErrorKey);
            Assert.Equal("User not found", errorResponse.Message);
        }
        [Fact]
        public async void test_GetWeekDay_WeekNotFound()
        {
            //Arrange
            var WeekController = new MockedWeekController();
            var weekRepository = WeekController.WeekRepository;

            GirafUser user = new GirafUser() { UserName = "Platos" };
            //Mock
            weekRepository.Setup(x => x.LoadUserWithWeekSchedules(user.UserName)).Returns(Task.FromResult(user));
            //Act
            var response = await WeekController.GetWeekDay("Platos", 2020, 40, 4);
            var objectResult = response as ObjectResult;
            var errorResponse = objectResult.Value as ErrorResponse;
            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
            Assert.Equal(ErrorCode.NotFound, errorResponse.ErrorKey);
            Assert.Equal("Week not found", errorResponse.Message);
        }
        [Fact]
        public async void test_GetWeekDay_WeekDayNotFound()
        {

            //Arrange
            var WeekController = new MockedWeekController();
            var weekRepository = WeekController.WeekRepository;

            GirafUser user = new GirafUser() { UserName = "Platos" };
            Week week = new Week() { WeekNumber = 40, WeekYear = 2020 };
            user.WeekSchedule.Add(week);
            //Mock
            weekRepository.Setup(x => x.LoadUserWithWeekSchedules(user.UserName)).Returns(Task.FromResult(user));
            //Act
            var response = await WeekController.GetWeekDay("Platos", 2020, 40, 4);
            var objectResult = response as ObjectResult;
            var errorResponse = objectResult.Value as ErrorResponse;
            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
            Assert.Equal(ErrorCode.NotFound, errorResponse.ErrorKey);
            Assert.Equal("Weekday not found", errorResponse.Message);
        }
        [Fact]
        public async void test_GetWeekDay_PictogramNotFound()
        {

            //Arrange
            var WeekController = new MockedWeekController();
            var weekRepository = WeekController.WeekRepository;
            var pictogramRepository = WeekController.PictogramRepository;

            GirafUser user = new GirafUser() { UserName = "Platos" };
            Week week = new Week() { WeekNumber = 40, WeekYear = 2020 };
            Pictogram pictogram = new Pictogram("Apple", AccessLevel.PUBLIC, "AppleHash");
            Weekday weekday = new Weekday(Days.Friday, new List<List<Pictogram>> { new List<Pictogram> { pictogram } }, new List<ActivityState> { ActivityState.Normal });

            user.WeekSchedule.Add(week);
            week.Weekdays.Add(weekday);
            user.WeekSchedule.Add(week);

            PictogramRelation relation = user.WeekSchedule.ToList()[0].Weekdays.ToList()[0].Activities.ToList()[0].Pictograms.ToList()[0];
            //Mock
            weekRepository.Setup(x => x.LoadUserWithWeekSchedules(user.UserName)).Returns(Task.FromResult(user));
            pictogramRepository.Setup(x => x.getPictogramMatchingRelation(relation)).Returns(Task.FromResult(null as Pictogram));
            //Act
            var response = await WeekController.GetWeekDay("Platos", 2020, 40, 4);
            var objectResult = response as ObjectResult;
            var errorResponse = objectResult.Value as ErrorResponse;
            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
            Assert.Equal(ErrorCode.PictogramNotFound, errorResponse.ErrorKey);
            Assert.Equal("Pictogram not found", errorResponse.Message);
        }
        [Fact]
        public async void test_GetWeekDay_Success()
        {
            //Arange
            var WeekController = new MockedWeekController();
            var weekRepository = WeekController.WeekRepository;
            var pictogramRepository = WeekController.PictogramRepository;

            GirafUser user = new GirafUser() { UserName = "Platos" };
            Week week = new Week() { WeekYear = 2020, WeekNumber = 40 };
            Pictogram pictogram = new Pictogram("Apple", AccessLevel.PUBLIC, "AppleHash");
            Weekday weekday = new Weekday(Days.Friday, new List<List<Pictogram>> { new List<Pictogram> { pictogram } }, new List<ActivityState> { ActivityState.Normal });

            week.Weekdays.Add(weekday);
            user.WeekSchedule.Add(week);
            PictogramRelation relation = user.WeekSchedule.ToList()[0].Weekdays.ToList()[0].Activities.ToList()[0].Pictograms.ToList()[0];
            //Mock
            weekRepository.Setup(x => x.LoadUserWithWeekSchedules(user.UserName)).Returns(Task.FromResult(user));
            pictogramRepository.Setup(x => x.getPictogramMatchingRelation(relation)).Returns(Task.FromResult(pictogram));
            //Act
            var response = await WeekController.GetWeekDay("Platos", 2020, 40, 4);
            var objectResult = response as ObjectResult;
            var successResponse = objectResult.Value as SuccessResponse<WeekdayDTO>;
            //Assert
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);
            Assert.NotNull(successResponse.Data);
            Assert.Equal(Days.Friday, successResponse.Data.Day);
        }
        [Fact]
        public async void test_UpdateWeek_BadRequestMissingWeek()
        {
            //Arrange
            var weekController = new MockedWeekController();
            //Mock
            //Act
            var response = await weekController.UpdateWeek("Platos", 2020, 21, null);
            var objectResult = response as ObjectResult;
            var errorResponse = objectResult.Value as ErrorResponse;
            //Assert
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
            Assert.Equal(ErrorCode.MissingProperties, errorResponse.ErrorCode);
            Assert.Equal("Missing newWeek", errorResponse.Message);
        }
        [Fact]
        public async void test_UpdateWeek_UserNotFound()
        {
            //Arrange
            var weekController = new MockedWeekController();

            WeekDTO newWeek = new WeekDTO();
            //Mock
            //Act
            var response = await weekController.UpdateWeek("Platos", 2020, 21, newWeek);
            var objectResult = response as ObjectResult;
            var errorResponse = objectResult.Value as ErrorResponse;
            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
            Assert.Equal(ErrorCode.UserNotFound, errorResponse.ErrorCode);
            Assert.Equal("User not found", errorResponse.Message);
        }

        //Update week calls SetWeekFromDTO which returns multiple kinds of errors. SetWeekFromDTO should have it's own unittest.
        //As long as this test shows that updateweek can reach that return after SetWeekFromDTO has failed then this test will have forfilled its purpose
        [Fact]
        public async void test_UpdateWeek_BadRequestSetWeekFromDTO()
        {
            //Arrange
            var weekController = new MockedWeekController();
            var weekRepository = weekController.WeekRepository;

            WeekDTO newWeek = new WeekDTO();
            GirafUser user = new GirafUser() { UserName = "Platos" };
            Week week = new Week() { WeekYear = 2021, WeekNumber = 21 };

            user.WeekSchedule.Add(week);
            //Mock
            weekRepository.Setup(x => x.LoadUserWithWeekSchedules("Platos")).Returns(Task.FromResult(user));
            weekRepository.Setup(x => x.SetWeekFromDTO(newWeek, week)).Returns(Task.FromResult(new ErrorResponse(ErrorCode.Error, "Mocked error")));
            //Act
            var response = await weekController.UpdateWeek("Platos", 2021, 21, newWeek);
            var objectResult = response as ObjectResult;
            var errorResponse = objectResult.Value as ErrorResponse;
            //Assert
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
            Assert.Equal(ErrorCode.Error, errorResponse.ErrorCode);
            Assert.Equal("Mocked error", errorResponse.Message);

        }
        [Fact]
        public async void test_UpdateWeek_SuccessWithEmptyWeek()
        {
            //Arrange
            var weekController = new MockedWeekController();
            var weekRepository = weekController.WeekRepository;

            GirafUser user = new GirafUser() { UserName = "Platos" };
            Week week = new Week() { WeekYear = 2021, WeekNumber = 21};
            WeekDTO newWeek = new WeekDTO();
          
            //Mock
            weekRepository.Setup(x => x.LoadUserWithWeekSchedules("Platos")).Returns(Task.FromResult(user));
            weekRepository.Setup(x => x.SetWeekFromDTO(newWeek, week)).Returns(Task.FromResult(null as ErrorResponse));
            //Act
            var response = await weekController.UpdateWeek("Platos", 2021, 21, newWeek);
            var objectResult = response as ObjectResult;
            var successResponse = objectResult.Value as SuccessResponse<WeekDTO>;
            //Assert
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);
            Assert.NotNull(successResponse.Data);
            Assert.Equal(21, successResponse.Data.WeekNumber);
            Assert.Equal(2021, successResponse.Data.WeekYear);
        }
      

        [Fact]
        public async void test_UpdateWeek_Success()
        {
            //Arrange
            var weekController = new MockedWeekController();
            var weekRepository = weekController.WeekRepository;

            WeekDTO newWeek = new WeekDTO();
            GirafUser user = new GirafUser() { UserName = "Platos" };
            Week week = new Week() { WeekYear = 2021, WeekNumber = 21, Name = "this week has a name for some reason" };

            user.WeekSchedule.Add(week);
            //Mock
            weekRepository.Setup(x => x.LoadUserWithWeekSchedules("Platos")).Returns(Task.FromResult(user));
            weekRepository.Setup(x => x.SetWeekFromDTO(newWeek, week)).Returns(Task.FromResult(null as ErrorResponse));
            
            //Act
            var response = await weekController.UpdateWeek("Platos", 2021, 21, newWeek);
            var objectResult = response as ObjectResult;
            var successResponse = objectResult.Value as SuccessResponse<WeekDTO>;
            //Assert
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);
            Assert.NotNull(successResponse.Data);
            Assert.Equal("this week has a name for some reason", successResponse.Data.Name);

        }
        [Fact]
        public async void test_UpdateWeekDay_BadRequestMissingWeekday()
        {
            //Arrange
            var weekController = new MockedWeekController();
            //Mock
            //Act
            var response = await weekController.UpdateWeekday("Platos", 2021, 21, null);
            var objectResult = response as ObjectResult;
            var errorResponse = objectResult.Value as ErrorResponse;
            //Assert
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);
            Assert.Equal(ErrorCode.MissingProperties, errorResponse.ErrorCode);
            Assert.Equal("Missing weekday", errorResponse.Message);
        }
        [Fact]
        public async void test_UpdateWeekDay_UserNotFound()
        {
            //Arrange
            var weekController = new MockedWeekController();

            WeekdayDTO newWeekDay = new WeekdayDTO() { Day = Days.Wednesday };
            //Mock
            //Act
            var response = await weekController.UpdateWeekday("Platos", 2021, 21, newWeekDay);
            var objectResult = response as ObjectResult;
            var errorResponse = objectResult.Value as ErrorResponse;
            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
            Assert.Equal(ErrorCode.UserNotFound, errorResponse.ErrorCode);
            Assert.Equal("User not found", errorResponse.Message);

        }
        [Fact]
        public async void test_UpdateWeekDay_WeekNotfound()
        {
            //Arrange
            var weekController = new MockedWeekController();
            var weekRepository = weekController.WeekRepository;

            WeekdayDTO newWeekDay = new WeekdayDTO() { Day = Days.Wednesday };
            GirafUser user = new GirafUser() { UserName = "Platos" };
            //Mock
            weekRepository.Setup(x => x.LoadUserWithWeekSchedules(user.UserName)).Returns(Task.FromResult(user));
            //Act
            var response = await weekController.UpdateWeekday("Platos", 2021, 21, newWeekDay);
            var objectResult = response as ObjectResult;
            var errorResponse = objectResult.Value as ErrorResponse;
            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
            Assert.Equal(ErrorCode.WeekNotFound, errorResponse.ErrorCode);
            Assert.Equal("Week not found", errorResponse.Message);
        }

        [Fact]
        public async void test_UpdateWeekDay_ResourseNotFoundPictogram()
        {
            //Arrange
            var weekController = new MockedWeekController();
            var weekRepository = weekController.WeekRepository;

            WeekdayDTO newWeekDay = new WeekdayDTO() { Day = Days.Wednesday };
            GirafUser user = new GirafUser() { UserName = "Platos" };
            Week week = new Week() { WeekYear = 2021, WeekNumber = 21 };
            Weekday day = new Weekday() { Day = Days.Wednesday };

            week.Weekdays.Add(day);
            user.WeekSchedule.Add(week);
            //Mock
            weekRepository.Setup(x => x.LoadUserWithWeekSchedules(user.UserName)).Returns(Task.FromResult(user));
            weekRepository.Setup(x => x.AddPictogramsToWeekday(day, newWeekDay)).Returns(Task.FromResult(false));
            //Act
            var response = await weekController.UpdateWeekday("Platos", 2021, 21, newWeekDay);
            var objectResult = response as ObjectResult;
            var errorResponse = objectResult.Value as ErrorResponse;
            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
            Assert.Equal(ErrorCode.ResourceNotFound, errorResponse.ErrorCode);
            Assert.Equal("Missing pictogram", errorResponse.Message);
        }
        [Fact]
        public async void test_UpdateWeekDay_Success()
        {
            //Arrange
            var weekController = new MockedWeekController();
            var weekRepository = weekController.WeekRepository;

            WeekdayDTO newWeekDay = new WeekdayDTO() { Day = Days.Wednesday };
            GirafUser user = new GirafUser() { UserName = "Platos" };
            Week week = new Week() { WeekYear = 2021, WeekNumber = 21 };
            Weekday day = new Weekday() { Day = Days.Wednesday };

            week.Weekdays.Add(day);
            user.WeekSchedule.Add(week);
            //Mock
            weekRepository.Setup(x => x.LoadUserWithWeekSchedules(user.UserName)).Returns(Task.FromResult(user));
            weekRepository.Setup(x => x.AddPictogramsToWeekday(day, newWeekDay)).Returns(Task.FromResult(true));
            //Act
            var response = await weekController.UpdateWeekday("Platos", 2021, 21, newWeekDay);
            var objectResult = response as ObjectResult;
            var successResponse = objectResult.Value as SuccessResponse<WeekdayDTO>;
            //Assert
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);
            Assert.NotNull(successResponse.Data);
            Assert.Equal(Days.Wednesday, successResponse.Data.Day);
        }
        [Fact]
        public async void test_DeleteWeek_NoUserFound()
        {
            //Arrange
            var weekController = new MockedWeekController();
            //Mock
            //Act
            var response = await weekController.DeleteWeek("Platos", 2021, 24);
            var objectResult = response as ObjectResult;
            var errorResponse = objectResult.Value as ErrorResponse;
            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
            Assert.Equal(ErrorCode.UserNotFound, errorResponse.ErrorCode);
            Assert.Equal("User not found", errorResponse.Message);

        }
        [Fact]
        public async void test_DeleteWeek_NoWeekScheduleFoundELSE()
        {
            //Arrange
            var weekController = new MockedWeekController();
            var weekRepository = weekController.WeekRepository;

            GirafUser user = new GirafUser() { UserName = "Platos" };
            //Mock
            weekRepository.Setup(x => x.getAllWeeksOfUser("Platos")).Returns(Task.FromResult(user));
            //Act
            var response = await weekController.DeleteWeek("Platos", 2021, 24);
            var objectResult = response as ObjectResult;
            var errorResponse = objectResult.Value as ErrorResponse;
            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
            Assert.Equal(ErrorCode.NoWeekScheduleFound, errorResponse.ErrorCode);
            Assert.Equal("No week schedule found", errorResponse.Message);
        }
        [Fact]
        public async void test_DeleteWeek_NoWeekScheduleFound()
        {
            //Arrange
            var weekController = new MockedWeekController();
            var weekRepository = weekController.WeekRepository;

            GirafUser user = new GirafUser() { UserName = "Platos" };
            Week week = new Week() { WeekNumber = 24, WeekYear = 2021 };
            //Mock
            weekRepository.Setup(x => x.getAllWeeksOfUser("Platos")).Returns(Task.FromResult(user));
            //Act
            var response = await weekController.DeleteWeek("Platos", 2021, 24);
            var objectResult = response as ObjectResult;
            var errorResponse = objectResult.Value as ErrorResponse;
            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
            Assert.Equal(ErrorCode.NoWeekScheduleFound, errorResponse.ErrorCode);
            Assert.Equal("No week schedule found", errorResponse.Message);
        }
        [Fact]
        public async void test_DeleteWeek_Success()
        {
            //Arrange
            var weekController = new MockedWeekController();
            var weekRepository = weekController.WeekRepository;

            GirafUser user = new GirafUser() { UserName = "Platos" };
            Week week = new Week() { WeekNumber = 24, WeekYear = 2021 };

            user.WeekSchedule.Add(week);
            //Mock
            weekRepository.Setup(x => x.getAllWeeksOfUser("Platos")).Returns(Task.FromResult(user));
            //Act
            var response = await weekController.DeleteWeek("Platos", 2021, 24);
            var objectResult = response as ObjectResult;
            var successResponse = objectResult.Value as SuccessResponse;
            //Assert
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);
            Assert.NotNull(successResponse.Data);
            Assert.Equal("Deleted info for entire week", successResponse.Data);
        }
    }
}
