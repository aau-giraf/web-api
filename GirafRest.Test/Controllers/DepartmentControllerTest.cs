using Moq;
using Xunit;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using System.Collections.Generic;
using GirafRest.Controllers;
using GirafRest.Models.Responses;
using GirafRest.Test.Mocks;
using static GirafRest.Test.UnitTestExtensions;
using Xunit.Abstractions;
using System.Linq;
using GirafRest.Services;
using static GirafRest.Test.UnitTestExtensions.TestContext;

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

            var res = dc.Get(DEPARTMENT_ONE).Result;

            Assert.True(res.Success);
            //Check data
            Assert.Equal(_testContext.MockDepartments[0].Name, res.Data.Name);
            Assert.Equal(_testContext.MockDepartments[0].Members.Count, res.Data.Members.Count);
        }

        [Fact]
        public void Get_GetNonExistingDepartmentByID_NotFound()
        {
            var dc = initializeTest();
            AddEmptyDepartmentList();


            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[UserGuardianDepartment1]);

            var res = dc.Get(DEPARTMENT_ONE).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.NotFound, res.ErrorCode);
        }

        [Fact]
        public void Get_AllDepartmentNames_Success()
        {
            var dc = initializeTest();
            var res = dc.Get().Result;

            Assert.True(res.Success);
            Assert.Equal(res.ErrorCode, ErrorCode.NoError);
            // check data
            Assert.Equal(_testContext.MockDepartments.Count(), res.Data.Count());
            for (int i = 0; i < res.Data.Count; i++)
            {
                Assert.Equal(_testContext.MockDepartments[i].Name, res.Data[i].Name);
                Assert.Equal(_testContext.MockDepartments[i].Key, res.Data[i].ID);
            }
        }

        [Fact]
        public void Get_AllDepartmentNames_NotFound()
        {
            var dc = initializeTest();
            AddEmptyDepartmentList();
            var res = dc.Get().Result;

            Assert.False(res.Success);
            Assert.Equal(res.ErrorCode, ErrorCode.NotFound);
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

            var res = dc.Post(depDTO).Result;

            Assert.True(res.Success);
            //Check data
            Assert.Equal(name, res.Data.Name);
        }

        [Fact]
        public void Post_NewDepartmentInvalidDTO_MissingProperties()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var depDTO = new DepartmentDTO(new Department()
            {
            }, new List<DisplayNameDTO>());

            var res = dc.Post(depDTO).Result;

            Assert.Equal(ErrorCode.MissingProperties, res.ErrorCode);
        }

        #endregion

        #region AddUser

        [Fact]
        public void AddUser_ExistingDepartment_Success()
        {
            var dc = initializeTest();
            var mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var user = _testContext.MockUsers[CITIZEN_NO_DEPARTMENT];

            var res = dc.AddUser(DEPARTMENT_TWO, user.Id).Result;

            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
        }

        [Fact]
        public void AddUser_ExistingDepartment_UserAlreadyHasDepartment()
        {
            var dc = initializeTest();
            var mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            var user = _testContext.MockUsers[CITIZEN_DEP_THREE];

            var res = dc.AddUser(DEPARTMENT_TWO, user.Id).Result;

            Assert.Equal(ErrorCode.UserAlreadyHasDepartment, res.ErrorCode);
        }


        [Fact]
        public void AddUser_NonExistingDepartment_DepartmentNotFound()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var user = _testContext.MockUsers[DEPARTMENT_TWO_USER];

            var res = dc.AddUser(DEPARTMENT_TEN, user.Id).Result;
            Assert.Equal(res.ErrorCode, ErrorCode.DepartmentNotFound);
        }

        [Fact]
        public void AddUser_ExistingDepartmentInvalidUser_MissingProperties()
        {
            var dc = initializeTest();
            var user = new GirafUserDTO() { };
            var res = dc.AddUser(DEPARTMENT_ONE, user.Id).Result;

            Assert.Equal(res.ErrorCode, ErrorCode.MissingProperties);
        }

        #endregion

        #region AddResource

        [Fact]
        [System.Obsolete]
        public void AddResource_ValidDepartmentValidResource_Success()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = dc.AddResource(DEPARTMENT_ONE, RESOURCE_THREE).Result;

            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            //Check Data
            Assert.True(res.Data.Resources.Any(r => r == RESOURCE_THREE));
        }

        [Fact]
        [System.Obsolete]
        public void AddResource_InvalidDepartmentValidResource_DepartmentNotFound()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = dc.AddResource(DEPARTMENT_TEN, RESOURCE_THREE).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.DepartmentNotFound, res.ErrorCode);
        }

        [Fact]
        [System.Obsolete]
        public void AddResource_ValidDepartmentNonExistingResourceInDTO_ResourceNotFound()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = dc.AddResource(DEPARTMENT_ONE, NONEXISTING).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.ResourceNotFound, res.ErrorCode);
        }

        [Fact]
        [System.Obsolete]
        public void AddResource_ValidDepartmentValidDTONoLogin_Unauthorized()
        {
            var dc = initializeTest();
            var res = dc.AddResource(DEPARTMENT_ONE, RESOURCE_ONE).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.NotAuthorized, res.ErrorCode);
        }

        #endregion

        #region RemoveResource

        [Fact]
        [System.Obsolete]
        public void RemoveResource_RemoveExistingResource_Success()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = dc.RemoveResource(RESOURCE_FIVE).Result;

            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            // Check that ressource no longer exist
            Assert.True(!(res.Data.Resources.Any(r => r == RESOURCE_FIVE)));
        }

        [Fact]
        [System.Obsolete]
        public void RemoveResource_RemoveResourceWrongDepartment_NotAuthorized()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[3]);
            var res = dc.RemoveResource(RESOURCE_FIVE).Result;

            Assert.Equal(ErrorCode.NotAuthorized, res.ErrorCode);
        }

        [Fact]
        public void AddDepartment_ValidDTO_Success()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            var depName = "Børnehave Toften";
            var res = dc.Post(new DepartmentDTO() {Name = depName}).Result;

            Assert.True(res.Success);
            // Check that there now exist a børnehave named toften
            Assert.True(res.Data.Name == depName);
        }

        [Fact]
        public void GetAllCitizensFromDepartment_Success()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[DEPARTMENT_TWO_USER]);
            var departmentTwoId = _testContext.MockDepartments[DEPARTMENT_TWO_OBJECT].Key;
            var res = dc.GetCitizenNamesAsync(departmentTwoId).Result;

            Assert.True(res.Success);
            // Check that we found all citizens in department
            var countCitizens = 1;
            Assert.True(countCitizens == res.Data.Count);
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
}