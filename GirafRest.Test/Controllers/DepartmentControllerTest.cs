using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using System.Collections.Generic;
using GirafRest.Controllers;
using GirafRest.Models.Responses;
using GirafRest.Test.Mocks;
// using Microsoft.Extensions.ProjectModel.Resolution;
using Org.BouncyCastle.Asn1.Misc;
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
            Assert.IsType<Response<List<DepartmentDTO>>>(res);
        }

        [Fact]
        public void Get_GetAllNoExistingDepartments_NotFound()
        {
            var dc = initializeTest();
            AddEmptyDepartmentList();

            var res = dc.Get().Result;
            
            Assert.IsType<ErrorResponse<List<DepartmentDTO>>>(res);
            Assert.Equal(res.ErrorCode, ErrorCode.NotFound);
        }

        [Fact]
        public void Get_GetExistingDepartmentByID_OK()
        {
            var dc = initializeTest();

            var res = dc.Get(DEPARTMENT_ONE).Result;
            Assert.IsType<Response<DepartmentDTO>>(res);
        }

        [Fact]
        public void Get_GetNonExistingDepartmentByID_NotFound()
        {
            var dc = initializeTest();
            AddEmptyDepartmentList();

            var res = dc.Get(DEPARTMENT_ONE).Result;
            Assert.IsType<ErrorResponse<DepartmentDTO>>(res);
            Assert.Equal(res.ErrorCode, ErrorCode.NotFound);
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
            Assert.IsType<Response<DepartmentDTO>>(res);
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
            Assert.IsType<ErrorResponse<DepartmentDTO>>(res);
            Assert.Equal(res.ErrorCode, ErrorCode.MissingProperties);
        }
        #endregion

        #region AddUser
        [Fact(Skip = "dc.AddUser uses await _giraf._context.SaveChangesAsync(); which does not currently work")]
        public void AddUser_ExistingDepartment_OK()
        {
            var dc = initializeTest();
            var user = new GirafUserDTO()
            {
                Username = "TestUser"
            };

            var res = dc.AddUser(DEPARTMENT_ONE, user).Result;
            Assert.IsType<Response<DepartmentDTO>>(res);
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
            Assert.IsType<ErrorResponse<DepartmentDTO>>(res);
            Assert.Equal(res.ErrorCode, ErrorCode.DepartmentNotFound);
        }

        [Fact]
        public void AddUser_ExistingDepartmentInvalidUser_BadRequest()
        {
            var dc = initializeTest();
            var user = new GirafUserDTO()
            {
            };

            var res = dc.AddUser(DEPARTMENT_ONE, user).Result;
            Assert.IsType<ErrorResponse<DepartmentDTO>>(res);
            Assert.Equal(res.ErrorCode, ErrorCode.MissingProperties);
        }
        #endregion

        #region RemoveUser
        [Fact]
        public void RemoveUser_RemoveExistingUser_OK()
        {
            var dc = initializeTest();


            var res = dc.RemoveUser(DEPARTMENT_ONE, _testContext.MockUsers[ADMIN_DEP_ONE]).Result;
            Assert.IsType<Response<DepartmentDTO>>(res);
        }
        
        [Fact]
        public void RemoveUser_RemoveNullUser_BadRequest()
        {
            var dc = initializeTest();
            var res = dc.RemoveUser(DEPARTMENT_ONE, null).Result;
            Assert.IsType<ErrorResponse<DepartmentDTO>>(res);
            Assert.Equal(res.ErrorCode, ErrorCode.MissingProperties);
        }
        
        [Fact]
        public void RemoveUser_RemoveUserNonExistingDepartment_NotFound()
        {
            var dc = initializeTest();
            var res = dc.RemoveUser(DEPARTMENT_TEN, _testContext.MockUsers[ADMIN_DEP_ONE]).Result;
            Assert.IsType<ErrorResponse<DepartmentDTO>>(res);
            Assert.Equal(res.ErrorCode, ErrorCode.DepartmentNotFound);
        }
        
        [Fact]
        public void RemoveUser_RemoveUserWrongDepartment_BadRequest()
        {
            var dc = initializeTest();

            var res = dc.RemoveUser(DEPARTMENT_TWO, _testContext.MockUsers[ADMIN_DEP_ONE]).Result;
            Assert.IsType<ErrorResponse<DepartmentDTO>>(res);
            Assert.Equal(res.ErrorCode, ErrorCode.UserNotFound);
        }
        #endregion

        #region AddResource
        [Fact]
        public void AddResource_ValidDepartmentValidDTO_Ok()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);

            var res = dc.AddResource(DEPARTMENT_ONE, new ResourceIdDTO() { Id = RESOURCE_THREE }).Result;
            Assert.IsType<Response<DepartmentDTO>>(res);
        }

        [Fact]
        public void AddResource_ValidDepartmentInvalidDTO_BadRequest()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);

            var res = dc.AddResource(DEPARTMENT_ONE, new ResourceIdDTO()).Result;
            Assert.IsType<ErrorResponse<DepartmentDTO>>(res);
            Assert.Equal(res.ErrorCode, ErrorCode.MissingProperties);
        }

        [Fact]
        public void AddResource_InvalidDepartmentValidDTO_NotFound()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);

            var res = dc.AddResource(DEPARTMENT_TEN, new ResourceIdDTO() { Id = RESOURCE_THREE }).Result;
            Assert.IsType<ErrorResponse<DepartmentDTO>>(res);
            Assert.Equal(res.ErrorCode, ErrorCode.DepartmentNotFound);
        }

        [Fact]
        public void AddResource_ValidDepartmentNullDTO_BadRequest()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);


            var res = dc.AddResource(DEPARTMENT_ONE, null).Result;
            Assert.IsType<ErrorResponse<DepartmentDTO>>(res);
            Assert.Equal(res.ErrorCode, ErrorCode.MissingProperties);
        }

        [Fact]
        public void AddResource_ValidDepartmentNonExistingResourceInDTO_NotFound()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);

            var res = dc.AddResource(DEPARTMENT_ONE, new ResourceIdDTO() { Id = NONEXISTING }).Result;
            Assert.IsType<ErrorResponse<DepartmentDTO>>(res);
            Assert.Equal(res.ErrorCode, ErrorCode.ResourceNotFound);
        }

        [Fact]
        public void AddResource_ValidDepartmentValidDTONoLogin_Unauthorized()
        {
            var dc = initializeTest();

            var res = dc.AddResource(DEPARTMENT_ONE, new ResourceIdDTO() { Id = RESOURCE_ONE }).Result;
            Assert.IsType<ErrorResponse<DepartmentDTO>>(res);
            Assert.Equal(res.ErrorCode, ErrorCode.NotAuthorized);
        }
        #endregion

        #region RemoveResource
        [Fact]
        public void RemoveResource_RemoveExistingResource_OK()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);

            var res = dc.RemoveResource(new ResourceIdDTO() { Id = RESOURCE_FIVE }).Result;
            Assert.IsType<Response<DepartmentDTO>>(res);
        }

        [Fact]
        public void RemoveResource_RemoveNullResource_BadRequest()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);

            var res = dc.RemoveResource(null).Result;
            Assert.IsType<ErrorResponse<DepartmentDTO>>(res);
            Assert.Equal(res.ErrorCode, ErrorCode.MissingProperties);
        }

        [Fact]
        public void RemoveResource_RemoveResourceWrongDepartment_BadRequest()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[3]);

            var res = dc.RemoveResource(new ResourceIdDTO() { Id = RESOURCE_FIVE }).Result;
            Assert.IsType<ErrorResponse<DepartmentDTO>>(res);
            Assert.Equal(res.ErrorCode, ErrorCode.NotAuthorized);
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