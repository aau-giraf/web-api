/*using Moq;
using Xunit;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using System.Collections.Generic;
using GirafRest.Controllers;
using GirafRest.Models.Responses;
using GirafRest.Test.Mocks;
using static GirafRest.Test.UnitTestExtensions;
using System.Linq;
using GirafRest.Services;
using static GirafRest.Test.UnitTestExtensions.TestContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace GirafRest.Test
{
    public class DepartmentControllerTest
    {
#pragma warning disable IDE0051 // Remove unused private members
        private TestContext _testContext;

        private const int DEPARTMENT_ONE = 1;
        private const int DEPARTMENT_TWO = 2;
        private const int DEPARTMENT_TEN = 10;
        private const int RESOURCE_ONE = 1;
        private const int RESOURCE_THREE = 3;
        private const int RESOURCE_FIVE = 5;
        private const int NONEXISTING = 999;
        private const int ADMIN_DEP_ONE = 0;
        private const int DEPARTMENT_TWO_USER = 6;
        private const int DEPARTMENT_TWO_OBJECT = 1;
        private const int CITIZEN_NO_DEPARTMENT = 9;
        private const int CITIZEN_DEP_THREE = 3;
#pragma warning restore IDE0051 // Remove unused private members

        public DepartmentControllerTest() {}

        private DepartmentController initializeTest()
        {
            _testContext = new TestContext();

            var dc = new DepartmentController(
                new MockGirafService(
                    _testContext.MockDbContext.Object,
                    _testContext.MockUserManager),
                _testContext.MockLoggerFactory.Object,
                _testContext.MockRoleManager.Object,
                new GirafAuthenticationService(_testContext.MockDbContext.Object, _testContext.MockRoleManager.Object,
                    _testContext.MockUserManager));

            _testContext.MockHttpContext = dc.MockHttpContext();
            _testContext.MockHttpContext.MockClearQueries();

            return dc;
        }

        #region Get

        [Fact]
        public void Get_GetExistingDepartmentByID_Success()
        {
            var dc = initializeTest();

            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[UserGuardianDepartment1]);

            var res = dc.Get(DEPARTMENT_ONE).Result as ObjectResult;
            var body = res.Value as SuccessResponse<DepartmentDTO>;


            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            //Check data
            Assert.Equal(_testContext.MockDepartments[0].Name, body.Data.Name);
            Assert.Equal(_testContext.MockDepartments[0].Members.Count, body.Data.Members.Count);
        }

        [Fact]
        public void Get_GetNonExistingDepartmentByID_NotFound()
        {
            var dc = initializeTest();
            AddEmptyDepartmentList();


            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[UserGuardianDepartment1]);

            var res = dc.Get(DEPARTMENT_ONE).Result as ObjectResult;

            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status404NotFound, res.StatusCode);
            Assert.Equal(ErrorCode.NotFound, body.ErrorCode);
        }

        [Fact]
        public void Get_AllDepartmentNames_Success()
        {
            var dc = initializeTest();
            var res = dc.Get().Result as ObjectResult;
            var body = res.Value as SuccessResponse<List<DepartmentNameDTO>>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            // check data
            Assert.Equal(_testContext.MockDepartments.Count(), body.Data.Count());

            for (int i = 0; i < body.Data.Count; i++)
            {
                Assert.Equal(_testContext.MockDepartments[i].Name, body.Data[i].Name);
                Assert.Equal(_testContext.MockDepartments[i].Key, body.Data[i].ID);
            }
        }

        [Fact]
        public void Get_AllDepartmentNames_NotFound()
        {
            var dc = initializeTest();
            AddEmptyDepartmentList();
            var res = dc.Get().Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status404NotFound, res.StatusCode);
            Assert.Equal(ErrorCode.NotFound, body.ErrorCode);
        }

        #endregion

        #region Post

        [Fact]
        public void Post_NewDepartmentValidDTO_Success()
        {
            var dc = initializeTest();
            var name = "dep1";
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);

            var depDTO = new DepartmentDTO(new Department()
            {
                Name = name
            }, new List<DisplayNameDTO>());

            var res = dc.Post(depDTO).Result as ObjectResult;
            var body = res.Value as SuccessResponse<DepartmentDTO>;

            Assert.Equal(StatusCodes.Status201Created, res.StatusCode);
            Assert.Equal(name, body.Data.Name);
        }

        [Fact]
        public void Post_NewDepartmentInvalidDTO_MissingProperties()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var depDTO = new DepartmentDTO(new Department()
            {
            }, new List<DisplayNameDTO>());

            var res = dc.Post(depDTO).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal(ErrorCode.MissingProperties, body.ErrorCode);
        }

        #endregion

        #region AddResource

        [Fact]
        [System.Obsolete]
        public void AddResource_ValidDepartmentValidResource_Success()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = dc.AddResource(DEPARTMENT_ONE, RESOURCE_THREE).Result as ObjectResult;
            var body = res.Value as SuccessResponse<DepartmentDTO>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            Assert.True(body.Data.Resources.Any(r => r == RESOURCE_THREE));
        }

        [Fact]
        [System.Obsolete]
        public void AddResource_InvalidDepartmentValidResource_DepartmentNotFound()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = dc.AddResource(DEPARTMENT_TEN, RESOURCE_THREE).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status404NotFound, res.StatusCode);
            Assert.Equal(ErrorCode.DepartmentNotFound, body.ErrorCode);
        }

        [Fact]
        [System.Obsolete]
        public void AddResource_ValidDepartmentNonExistingResourceInDTO_ResourceNotFound()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = dc.AddResource(DEPARTMENT_ONE, NONEXISTING).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status404NotFound, res.StatusCode);
            Assert.Equal(ErrorCode.ResourceNotFound, body.ErrorCode);
        }

        [Fact]
        [System.Obsolete]
        public void AddResource_ValidDepartmentValidDTONoLogin_Unauthorized()
        {
            var dc = initializeTest();
            var res = dc.AddResource(DEPARTMENT_ONE, RESOURCE_ONE).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
            Assert.Equal(ErrorCode.NotAuthorized, body.ErrorCode);
        }

        #endregion

        #region RemoveResource

        [Fact]
        [System.Obsolete]
        public void RemoveResource_RemoveExistingResource_Success()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = dc.RemoveResource(RESOURCE_FIVE).Result as ObjectResult;
            var body = res.Value as SuccessResponse<DepartmentDTO>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            // Check that ressource no longer exist
            Assert.True(!(body.Data.Resources.Any(r => r == RESOURCE_FIVE)));
        }

        [Fact]
        [System.Obsolete]
        public void RemoveResource_RemoveResourceWrongDepartment_NotAuthorized()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[3]);
            var res = dc.RemoveResource(RESOURCE_FIVE).Result as ObjectResult;
            var body = res.Value as ErrorResponse;
            
            Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
            Assert.Equal(ErrorCode.NotAuthorized, body.ErrorCode);
        }

        [Fact]
        public void AddDepartment_ValidDTO_Success()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            var depName = "Børnehave Toften";
            var res = dc.Post(new DepartmentDTO() {Name = depName}).Result as ObjectResult;
            var body = res.Value as SuccessResponse<DepartmentDTO>;

            Assert.Equal(StatusCodes.Status201Created, res.StatusCode);
            // Check that there now exist a børnehave named toften
            Assert.Equal(depName, body.Data.Name);
        }

        [Fact]
        public void GetAllCitizensFromDepartment_Success()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[DEPARTMENT_TWO_USER]);
            var departmentTwoId = _testContext.MockDepartments[DEPARTMENT_TWO_OBJECT].Key;
            var res = dc.GetCitizenNamesAsync(departmentTwoId).Result as ObjectResult;
            var body = res.Value as SuccessResponse<List<DisplayNameDTO>>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            // Check that we found all citizens in department
            var countCitizens = 1;
            Assert.Equal(countCitizens, body.Data.Count);
        }

        #endregion

        #region Helpers

        private void AddEmptyDepartmentList()
        {
            _testContext.MockDbContext.Reset();
            var emptyDep = CreateMockDbSet(new List<Department>());
            _testContext.MockDbContext.Setup(c => c.Departments).Returns(emptyDep.Object);
        }

        #endregion
    }
}*/