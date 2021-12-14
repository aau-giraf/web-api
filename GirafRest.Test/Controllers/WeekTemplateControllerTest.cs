/*using System.Linq;
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
using Microsoft.AspNetCore.Http;
using static GirafRest.Test.UnitTestExtensions.TestContext;
using Microsoft.AspNetCore.Mvc;

namespace GirafRest.Test
{
    public class WeekTemplateControllerTest
    {
#pragma warning disable IDE0052 // Remove unread private members
        private readonly ITestOutputHelper _testLogger;
#pragma warning restore IDE0052 // Remove unread private members
        TestContext _testContext;

        public WeekTemplateControllerTest(ITestOutputHelper output)
        {
            _testLogger = output;
        }

        private WeekTemplateController InitializeTest()
        {
            _testContext = new TestContext();

            var wtc = new WeekTemplateController(
                new MockGirafService(
                    _testContext.MockDbContext.Object,
                    _testContext.MockUserManager
                    ),
                _testContext.MockLoggerFactory.Object,
                new GirafAuthenticationService(_testContext.MockDbContext.Object,
                    _testContext.MockRoleManager.Object,
                    _testContext.MockUserManager
                    )
            );
            

            _testContext.MockHttpContext = wtc.MockHttpContext();

            return wtc;
        }
        
        
        #region GetWeekTemplates

        
        [Fact]
        public void GetWeekTemplates_NoTemplates_NoWeekTemplateFound()
        {
            var wtc = InitializeTest();
            
            //There are no weektemplates in department 2.
            var user = _testContext.MockUsers[UserGuardianDepartment2];
            _testContext.MockUserManager.MockLoginAsUser(user);

            var res = wtc.GetWeekTemplates().Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status404NotFound, res.StatusCode);
            Assert.Equal(ErrorCode.NoWeekTemplateFound, body.ErrorCode);
        }
        
        [Fact]
        public void GetWeekTemplates_SomeTemplates_Success()
        {
            var wtc = InitializeTest();
            
            //There are no weektemplates in department 2.
            var user = _testContext.MockUsers[UserGuardianDepartment1];
            _testContext.MockUserManager.MockLoginAsUser(user);

            var res = wtc.GetWeekTemplates().Result as ObjectResult;
            var body = res.Value as SuccessResponse<IEnumerable<WeekTemplateNameDTO>>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            Assert.Contains("Template1", body.Data.Select(x => x.Name));
            Assert.Contains("Template2", body.Data.Select(x => x.Name));
        }

        #endregion


        #region GetWeekTemplate
        
        [Fact]
        public void GetWeekTemplate_InvalidID_NoWeekTemplateFound()
        {
            var wtc = InitializeTest();
            
            //There are no weektemplates in department 2.
            var user = _testContext.MockUsers[UserCitizenDepartment1];
            _testContext.MockUserManager.MockLoginAsUser(user);

            var res = wtc.GetWeekTemplate(int.MaxValue).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status404NotFound, res.StatusCode);
            Assert.Equal(ErrorCode.NoWeekTemplateFound, body.ErrorCode);
        }

        [Fact]
        public void GetWeekTemplate_FromOtherDepartment_NoWeekTemplateFound()
        {
            var wtc = InitializeTest();
            
            //There are no weektemplates in department 2.
            var user = _testContext.MockUsers[UserDepartment2];
            _testContext.MockUserManager.MockLoginAsUser(user);

            var res = wtc.GetWeekTemplate(Template1).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status404NotFound, res.StatusCode);
            Assert.Equal(ErrorCode.NoWeekTemplateFound, body.ErrorCode);
        }
        
        [Fact]
        public void GetWeekTemplate_ActualTemplate_Success()
        {
            var wtc = InitializeTest();
            
            //There are no weektemplates in department 2.
            var user = _testContext.MockUsers[UserGuardianDepartment1];
            _testContext.MockUserManager.MockLoginAsUser(user);

            var res = wtc.GetWeekTemplate(_testContext.MockWeekTemplates[Template1].Id).Result as ObjectResult;
            var body = res.Value as SuccessResponse<WeekTemplateDTO>;
            
            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            Assert.Equal("Template1", body.Data.Name);
            Assert.Contains(Days.Wednesday, body.Data.Days.Select(d => d.Day));
            Assert.DoesNotContain(Days.Friday, body.Data.Days.Select(d => d.Day));
        }

        #endregion


        #region CreateWeekTemplate
        
        [Fact]
        public void CreateWeekTemplate_NewTemplateValidDTO_Success()
        {
            var wtc = InitializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[UserGuardian2Department2]);
            var template = new WeekTemplate(_testContext.MockDepartments[MockDepartment2]);
            
            // modify name
            var templateDTO = new WeekTemplateDTO(template)
            {
                Name = "Test Week",
                Days = _testContext.MockWeekTemplates[Template1].Weekdays.Select(d => new WeekdayDTO(d)).ToList()
            };

            var res = wtc.CreateWeekTemplate(templateDTO).Result as ObjectResult;
            var body = res.Value as SuccessResponse<WeekTemplateDTO>;

            Assert.Equal(StatusCodes.Status201Created, res.StatusCode);
            Assert.Equal("Test Week", body.Data.Name);
        }
        
        [Fact]
        public void CreateWeekTemplate_NoDays_InvalidAmountOfWeekdays()
        {
            var wtc = InitializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[UserGuardian2Department2]);
            var template = new WeekTemplate(_testContext.MockDepartments[MockDepartment2]);
            
            // modify name
            var templateDTO = new WeekTemplateDTO(template)
            {
                Name = "Test Week"
            };

            var res = wtc.CreateWeekTemplate(templateDTO).Result as ObjectResult;
            var body = res.Value as ErrorResponse;
            
            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal(ErrorCode.InvalidAmountOfWeekdays, body.ErrorCode);
        }

        [Fact]
        public void CreateWeekTemplate_NullDTO_MissingProperties()
        {
            var wtc = InitializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[UserGuardian2Department2]);
            
            var res = wtc.CreateWeekTemplate(null).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal(ErrorCode.MissingProperties, body.ErrorCode);
        }



        #endregion
        
        
        #region UpdateWeekTemplate
        
        [Fact]
        public void UpdateWeekTemplate_NewTemplateValidDTO_Success()
        {
            var wtc = InitializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[UserAdmin]);
            var template = new WeekTemplate(_testContext.MockDepartments[MockDepartment2]);
            
            // modify name
            var templateDTO = new WeekTemplateDTO(template)
            {
                Name = "Test Week",
                Days = _testContext.MockWeekTemplates[Template2].Weekdays.Select(d => new WeekdayDTO(d)).ToList()
            };

            var res = wtc.UpdateWeekTemplate(_testContext.MockWeekTemplates[Template1].Id, templateDTO)
                .Result as ObjectResult;
            var body = res.Value as SuccessResponse<WeekTemplateDTO>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            Assert.Equal("Test Week", body.Data.Name);
        }
        
        [Fact]
        public void UpdateWeekTemplate_AsGuadianOfOtherDepartment_NotAuthorised()
        {
            var wtc = InitializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[UserGuardian2Department2]);
            var template = new WeekTemplate(_testContext.MockDepartments[MockDepartment2]);

            // modify name
            var templateDTO = new WeekTemplateDTO(template){Name = "Test hest"};

            var res = wtc.UpdateWeekTemplate(_testContext.MockWeekTemplates[Template1].Id, templateDTO).Result as ObjectResult;
            var body = res.Value as ErrorResponse;
            
            Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
            Assert.Equal(ErrorCode.NotAuthorized, body.ErrorCode);
        }

        [Fact]
        public void UpdateWeekTemplate_NullDTO_MissingProperties()
        {
            var wtc = InitializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[UserGuardian2Department2]);
            
            var res = wtc.UpdateWeekTemplate(_testContext.MockWeekTemplates[Template1].Id, null).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal(ErrorCode.MissingProperties, body.ErrorCode);
        }
        #endregion
    }
}*/