using System;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using GirafRest.Models;
using GirafRest.Data;
using Microsoft.Extensions.Logging;
using GirafRest.Models.DTOs;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using GirafRest.Controllers;
using Microsoft.AspNetCore.Http;
using GirafRest.Test.Mocks;
using Google.Protobuf.WellKnownTypes;
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
        private const int RESOURCE_FIVE = 1;
        private const int NONEXISTING = 999;
        private const int USER = 0;
        private const int OTHER_USER = 0;

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
        
        [Fact]
        public void Department_Get_all_Departments_ExpectOK()
        {
            var dc = initializeTest();

            var res = dc.Get().Result;
            Assert.IsType<OkObjectResult>(res);
        }

        [Fact]
        public void Department_Get_Department_byID_ExpectOK()
        {
            var dc = initializeTest();

            var res = dc.Get(DEPARTMENT_ONE).Result;
            Assert.IsType<OkObjectResult>(res);
        }

        [Fact]
        public void Department_Get_all_Departments_ExpectNotFound()
        {
            var dc = initializeTest();
            AddEmptyDepartmentList();

            var res = dc.Get().Result;

            if(res is ObjectResult)
                _testLogger.WriteLine((res as ObjectResult).Value.ToString());
            Assert.IsType<NotFoundResult>(res);
        }

        [Fact]
        public void Department_Get_Department_byID_ExpectNotFound()
        {
            var dc = initializeTest();
            AddEmptyDepartmentList();

            var res = dc.Get(DEPARTMENT_ONE).Result;
            Assert.IsType<NotFoundObjectResult>(res);
        }
        
        [Fact]
        public void Department_Post_Department_ExpectOK()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[USER]);
            var depDTO = new DepartmentDTO (new Department(){
                Name = "dep1"
            });

            var res = dc.Post(depDTO).Result;
            Assert.IsType<OkObjectResult>(res);
        }

        //[Fact]
        public void Department_AddUser_ExpectOK()
        {
            var dc = initializeTest();
            var user = new GirafUser()
            {
                UserName = "TestUser"
            };

            var res = dc.AddUser(DEPARTMENT_ONE, user).Result;
            Assert.IsType<OkObjectResult>(res);
        }

        //[Fact]
        public void Department_AddUser_ExpectNotFound()
        {
            var dc = initializeTest();
            var user = new GirafUser()
            {
                UserName = "AddUserTest"
            };

            var res = dc.AddUser(DEPARTMENT_TEN, user).Result;
            Assert.IsType<NotFoundObjectResult>(res);
        }

        //[Fact]
        public void Department_RemoveUser_ExpectOK()
        {
            var dc = initializeTest();


            var res = dc.RemoveUser(DEPARTMENT_TWO, _testContext.MockUsers[OTHER_USER]).Result;
            Assert.IsType<OkObjectResult>(res);
        }

        /* If the user is null */
        [Fact]
        public void Department_RemoveUser_UserNullExpectBadRequest()
        {
            var dc = initializeTest();
            var res = dc.RemoveUser(DEPARTMENT_ONE, null).Result;
            Assert.IsType<BadRequestObjectResult>(res);
        }

        /* If the department is not found */
        [Fact]
        public void Department_RemoveUser_ExpectNotFound()
        {
            var dc = initializeTest();
            var res = dc.RemoveUser(DEPARTMENT_TEN, _testContext.MockUsers[USER]).Result;
            Assert.IsType<NotFoundObjectResult>(res);
        }

        /* If the user is valid but not in the specified department */
        //[Fact]
        public void Department_RemoveUser_ExpectBadRequest()
        {
            var dc = initializeTest();
            var user = new GirafUser()
            {
                UserName = "AddUserTest"
            };

            var res = dc.RemoveUser(DEPARTMENT_ONE, user).Result;
            Assert.IsType<BadRequestObjectResult>(res);
        }

        [Fact]
        public void Department_AddResource_ExpectOK()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[USER]);


        }

        [Fact]
        public void AddResource_InvalidDepartmentValidDTO_NotFound()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[USER]);

            var res = dc.AddResource(DEPARTMENT_TEN, new ResourceIdDTO() { ResourceId = RESOURCE_FIVE }).Result;
            Assert.IsType<NotFoundObjectResult>(res);
        }

        [Fact]
        public void AddResource_NullAsResourceDTO_BadRequest()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[USER]);


            var res = dc.AddResource(DEPARTMENT_ONE, null).Result;
            Assert.IsType<BadRequestObjectResult>(res);
        }

        [Fact]
        public void Department_AddResource_ExpectResourceNotFound()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[USER]);

            var res = dc.AddResource(DEPARTMENT_TWO, new ResourceIdDTO() { ResourceId = NONEXISTING }).Result;
            Assert.IsType<NotFoundObjectResult>(res);
        }

        [Fact]
        public void Department_AddResource_ExpectUnauthorized()
        {
            var dc = initializeTest();

            var res = dc.AddResource(DEPARTMENT_ONE, new ResourceIdDTO() { ResourceId = RESOURCE_ONE }).Result;
            Assert.IsType<UnauthorizedResult>(res);
        }

        [Fact]
        public void Department_AddResource_ExpectAlreadyOwnedBadRequest()
        {
            var dc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[USER]);

        }

        private void AddEmptyDepartmentList()
        {
            _testContext.MockDbContext.Reset();
            var emptyDep = CreateMockDbSet(new List<Department>());
            _testContext.MockDbContext.Setup(c => c.Departments).Returns(emptyDep.Object);
        }
    }
}