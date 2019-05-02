using System.Collections.Generic;
using System.IO;
using System.Linq;
using GirafRest.Controllers;
using GirafRest.Data;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using GirafRest.Models.Responses;
using GirafRest.Services;
using GirafRest.Test.Mocks;
using Moq;
using Xunit;
using Xunit.Abstractions;
using static GirafRest.Test.UnitTestExtensions;

namespace GirafRest.Test
{
    public class ActivityControllerTest
    {
        private const int _existingId = 1;
        private const int _nonExistingId = 404;
        private TestContext _testContext;

        private ActivityController InitializeTest()
        {
            _testContext = new TestContext();

            //Ensures that the mock user has the mock activity in the user's weekplan
            _testContext.MockDbContext.Object.Users.First().WeekSchedule.First().Weekdays
                .First().Activities.Add(_testContext.MockActivities.Find(a => a.Key == _existingId));
           
            var wc = new ActivityController(
                new MockGirafService(_testContext.MockDbContext.Object,
                                     _testContext.MockUserManager), _testContext.MockLoggerFactory.Object,
                new GirafAuthenticationService(_testContext.MockDbContext.Object, _testContext.MockRoleManager.Object,
                                               _testContext.MockUserManager));
            _testContext.MockHttpContext = wc.MockHttpContext();
            return wc;
        }

        [Fact]
        public void DeleteActivity_ExistingActivity_Success()
        {
            ActivityController activityController = InitializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            
            //There exist an acitivity with the id
            Assert.True(_testContext.MockActivities.Any(a => a.Key == _existingId));

            Response res = activityController.DeleteActivity(_testContext.MockUsers[0].Id, _existingId).Result;
            Assert.True(res.Success);
        }

        [Fact]
        public void DeleteActivity_NonExistingActivity_Failure()
        {
            ActivityController activityController = InitializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);

            Response res = activityController.DeleteActivity(_testContext.MockUsers[0].Id, _nonExistingId).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.ActivityNotFound, res.ErrorCode);
        }
        [Fact]
        public void DeleteActivity_Unauthorized_Failure()
        {
            ActivityController activityController = InitializeTest();
            Activity mockActivity = new Activity(new Weekday(), new Pictogram(), 1, ActivityState.Normal)
            {
                Key = _existingId
            };
            _testContext.MockUsers[0].WeekSchedule.First().Weekdays.First().Activities.Add(mockActivity);

            Response res = activityController.DeleteActivity("invalidId", _existingId).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.NotAuthorized, res.ErrorCode);
        }
    }
}
