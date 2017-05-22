using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using System.Collections.Generic;
using GirafRest.Controllers;
using GirafRest.Test.Mocks;
using static GirafRest.Test.UnitTestExtensions;
using Xunit.Abstractions;

namespace GirafRest.Test.Controllers
{
    public class DepartmentControllerTest
    {
        private TestContext _testContext;
        private readonly ITestOutputHelper _testLogger;
        private const int DEPARTMENT_ONE = 1;
        private const int DEPARTMENT_TWO = 2;
        private const int DEPARTMENT_TEN = 10;
        private const int RESOURCE_ONE = 1;
        private const int RESOURCE_THREE = 3;
        private const int RESOURCE_FIVE = 5;
        private const int NONEXISTING = 999;
        private const int ADMIN_DEP_ONE = 0;

        public DepartmentControllerTest(ITestOutputHelper testLogger)
        {
            _testLogger = testLogger;
        }

        private DepartmentController initializeTest()
        {
            _testContext = new TestContext();

            var dc = new DepartmentController(
                new MockGirafService(
                    _testContext.MockDbContext.Object,
                    _testContext.MockUserManager),
                    _testContext.MockLoggerFactory.Object);

            _testContext.MockHttpContext = dc.MockHttpContext();
            _testContext.MockHttpContext.MockClearQueries();

            return dc;
        }

        #region Get
        [Fact]
        public void Get_GetAllExistingDepartments_OK()
        {
            var dc = initializeTest();

            var res = dc.Get().Result;
            Assert.IsType<OkObjectResult>(res);
        }

        [Fact]
        public void Get_GetAllNoExistingDepartments_NotFound()
        {
            var dc = initializeTest();
            AddEmptyDepartmentList();

            var res = dc.Get().Result;

            if (res is ObjectResult)
                _testLogger.WriteLine((res as ObjectResult).Value.ToString());
            Assert.IsType<NotFoundResult>(res);
        }

        [Fact]
        public void Get_GetExistingDepartmentByID_OK()
        {
            var dc = initializeTest();

            var res = dc.Get(DEPARTMENT_ONE).Result;
            Assert.IsType<OkObjectResult>(res);
        }

        [Fact]
        public void Get_GetNonExistingDepartmentByID_NotFound()
        {
            var dc = initializeTest();
            AddEmptyDepartmentList();

            var res = dc.Get(DEPARTMENT_ONE).Result;
            Assert.IsType<NotFoundObjectResult>(res);
        }
        #endregion

        #region Post
        [Fact]
        public void Post_NewDepartmentValidDTO_OK()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var depDTO = new DepartmentDTO (new Department(){
                Name = "dep1"
            });

            var res = dc.Post(depDTO).Result;
            Assert.IsType<OkObjectResult>(res);
        }

        [Fact]
        public void Post_NewDepartmentInvalidDTO_BadRequest()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var depDTO = new DepartmentDTO(new Department()
            {
            });

            var res = dc.Post(depDTO).Result;
            Assert.IsType<BadRequestObjectResult>(res);
        }
        #endregion

        #region AddUser
        [Fact]
        public void AddUser_ExistingDepartment_OK()
        {
            var dc = initializeTest();
            var user = new GirafUserDTO()
            {
                Username = "TestUser"
            };

            var res = dc.AddUser(DEPARTMENT_ONE, user).Result;
            Assert.IsType<OkObjectResult>(res);
        }

        [Fact]
        public void AddUser_NonExistingDepartment_NotFound()
        {
            var dc = initializeTest();
            var user = new GirafUserDTO()
            {
                Username = "AddUserTest"
            };

            var res = dc.AddUser(DEPARTMENT_TEN, user).Result;
            Assert.IsType<NotFoundObjectResult>(res);
        }

        [Fact]
        public void AddUser_ExistingDepartmentInvalidUser_BadRequest()
        {
            var dc = initializeTest();
            var user = new GirafUserDTO()
            {
            };

            var res = dc.AddUser(DEPARTMENT_ONE, user).Result;
            Assert.IsType<BadRequestObjectResult>(res);
        }
        #endregion

        #region RemoveUser
        [Fact]
        public void RemoveUser_RemoveExistingUser_OK()
        {
            var dc = initializeTest();


            var res = dc.RemoveUser(DEPARTMENT_ONE, _testContext.MockUsers[ADMIN_DEP_ONE]).Result;
            Assert.IsType<OkObjectResult>(res);
        }
        
        [Fact]
        public void RemoveUser_RemoveNullUser_BadRequest()
        {
            var dc = initializeTest();
            var res = dc.RemoveUser(DEPARTMENT_ONE, null).Result;
            Assert.IsType<BadRequestObjectResult>(res);
        }
        
        [Fact]
        public void RemoveUser_RemoveUserNonExistingDepartment_NotFound()
        {
            var dc = initializeTest();
            var res = dc.RemoveUser(DEPARTMENT_TEN, _testContext.MockUsers[ADMIN_DEP_ONE]).Result;
            Assert.IsType<NotFoundObjectResult>(res);
        }
        
        [Fact]
        public void RemoveUser_RemoveUserWrongDepartment_BadRequest()
        {
            var dc = initializeTest();

            var res = dc.RemoveUser(DEPARTMENT_TWO, _testContext.MockUsers[ADMIN_DEP_ONE]).Result;
            Assert.IsType<BadRequestObjectResult>(res);
        }
        #endregion

        #region AddResource
        [Fact]
        public void AddResource_ValidDepartmentValidDTO_Ok()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);

            var res = dc.AddResource(DEPARTMENT_ONE, new ResourceIdDTO() { Id = RESOURCE_THREE }).Result;
            Assert.IsType<OkObjectResult>(res);
        }

        [Fact]
        public void AddResource_ValidDepartmentInvalidDTO_BadRequest()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);

            var res = dc.AddResource(DEPARTMENT_ONE, new ResourceIdDTO()).Result;
            Assert.IsType<BadRequestObjectResult>(res);
        }

        [Fact]
        public void AddResource_InvalidDepartmentValidDTO_NotFound()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);

            var res = dc.AddResource(DEPARTMENT_TEN, new ResourceIdDTO() { Id = RESOURCE_THREE }).Result;
            Assert.IsType<NotFoundObjectResult>(res);
        }

        [Fact]
        public void AddResource_ValidDepartmentNullDTO_BadRequest()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);


            var res = dc.AddResource(DEPARTMENT_ONE, null).Result;
            Assert.IsType<BadRequestObjectResult>(res);
        }

        [Fact]
        public void AddResource_ValidDepartmentNonExistingResourceInDTO_NotFound()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);

            var res = dc.AddResource(DEPARTMENT_ONE, new ResourceIdDTO() { Id = NONEXISTING }).Result;
            Assert.IsType<NotFoundObjectResult>(res);
        }

        [Fact]
        public void AddResource_ValidDepartmentValidDTONoLogin_Unauthorized()
        {
            var dc = initializeTest();

            var res = dc.AddResource(DEPARTMENT_ONE, new ResourceIdDTO() { Id = RESOURCE_ONE }).Result;
            Assert.IsType<UnauthorizedResult>(res);
        }
        #endregion

        #region RemoveResource
        [Fact]
        public void RemoveResource_RemoveExistingResource_OK()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);

            var res = dc.RemoveResource(new ResourceIdDTO() { Id = RESOURCE_FIVE }).Result;
            Assert.IsType<OkObjectResult>(res);
        }

        [Fact]
        public void RemoveResource_RemoveNullResource_BadRequest()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);

            var res = dc.RemoveResource(null).Result;
            Assert.IsType<BadRequestObjectResult>(res);
        }

        [Fact]
        public void RemoveResource_RemoveResourceWrongDepartment_BadRequest()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[3]);

            var res = dc.RemoveResource(new ResourceIdDTO() { Id = RESOURCE_FIVE }).Result;
            Assert.IsType<BadRequestObjectResult>(res);
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