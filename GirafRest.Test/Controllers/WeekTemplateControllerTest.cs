using System.Linq;
using Xunit;
using GirafRest.Models;
using GirafRest.Controllers;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using GirafRest.Test.Mocks;
using static GirafRest.Test.UnitTestExtensions;
using GirafRest.Models.DTOs;
using Xunit.Abstractions;
using GirafRest.Models.Responses;
using GirafRest.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using static GirafRest.Test.UnitTestExtensions.TestContext;
using Newtonsoft.Json;


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

            var result = wtc.GetWeekTemplates().Result;

            Assert.Equal(ErrorCode.NoWeekTemplateFound, result.ErrorCode);
        }
        
        [Fact]
        public void GetWeekTemplates_SomeTemplates_Success()
        {
            var wtc = InitializeTest();
            
            //There are no weektemplates in department 2.
            var user = _testContext.MockUsers[UserGuardianDepartment1];
            _testContext.MockUserManager.MockLoginAsUser(user);

            var result = wtc.GetWeekTemplates().Result;

            Assert.IsType<Response<IEnumerable<WeekTemplateNameDTO>>>(result);
            Assert.True(result.Success);
            Assert.Contains("Template1", result.Data.Select(x => x.Name));
            Assert.Contains("Template2", result.Data.Select(x => x.Name));
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

            var result = wtc.GetWeekTemplate(int.MaxValue).Result;

            Assert.Equal(ErrorCode.NoWeekTemplateFound, result.ErrorCode);
        }

        [Fact]
        public void GetWeekTemplate_FromOtherDepartment_NoWeekTemplateFound()
        {
            var wtc = InitializeTest();
            
            //There are no weektemplates in department 2.
            var user = _testContext.MockUsers[UserDepartment2];
            _testContext.MockUserManager.MockLoginAsUser(user);

            var result = wtc.GetWeekTemplate(Template1).Result;

            Assert.Equal(ErrorCode.NoWeekTemplateFound, result.ErrorCode);
        }
        
        [Fact]
        public void GetWeekTemplate_ActualTemplate_Success()
        {
            var wtc = InitializeTest();
            
            //There are no weektemplates in department 2.
            var user = _testContext.MockUsers[UserGuardianDepartment1];
            _testContext.MockUserManager.MockLoginAsUser(user);

            var result = wtc.GetWeekTemplate(_testContext.MockWeekTemplates[Template1].Id).Result;

            Assert.True(result.Success);
            Assert.Equal("Template1", result.Data.Name);
            Assert.Contains(Days.Wednesday, result.Data.Days.Select(d => d.Day));
            Assert.DoesNotContain(Days.Friday, result.Data.Days.Select(d => d.Day));
            string json = JsonConvert.SerializeObject(result.Data);
            System.Console.WriteLine(json);
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

            var res = wtc.CreateWeekTemplate(templateDTO).Result;

            Assert.True(res.Success);
            Assert.Equal("Test Week", res.Data.Name);
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

            var res = wtc.CreateWeekTemplate(templateDTO).Result;
            
            Assert.Equal(ErrorCode.InvalidAmountOfWeekdays, res.ErrorCode);
        }

        [Fact]
        public void CreateWeekTemplate_NullDTO_MissingProperties()
        {
            var wtc = InitializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[UserGuardian2Department2]);
            
            var res = wtc.CreateWeekTemplate(null).Result;

            Assert.Equal(ErrorCode.MissingProperties, res.ErrorCode);
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

            var res = wtc.UpdateWeekTemplate(_testContext.MockWeekTemplates[Template1].Id, templateDTO).Result;

            Assert.True(res.Success);
            Assert.Equal("Test Week", res.Data.Name);
        }
        
        [Fact]
        public void UpdateWeekTemplate_AsGuadianOfOtherDepartment_NotAuthorised()
        {
            var wtc = InitializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[UserGuardian2Department2]);
            var template = new WeekTemplate(_testContext.MockDepartments[MockDepartment2]);

            // modify name
            var templateDTO = new WeekTemplateDTO(template){Name = "Test hest"};

            var res = wtc.UpdateWeekTemplate(_testContext.MockWeekTemplates[Template1].Id, templateDTO).Result;
            
            Assert.Equal(ErrorCode.NotAuthorized, res.ErrorCode);;
        }

        [Fact]
        public void UpdateWeekTemplate_NullDTO_MissingProperties()
        {
            var wtc = InitializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[UserGuardian2Department2]);
            
            var res = wtc.UpdateWeekTemplate(_testContext.MockWeekTemplates[Template1].Id, null).Result;

            Assert.Equal(ErrorCode.MissingProperties, res.ErrorCode); ;
        }



        #endregion
    }
}