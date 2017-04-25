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

namespace GirafRest.Test.Controllers
{
    public class DepartmentControllerTest
    {
        private readonly DepartmentController departmentController;
        
        private readonly MockUserManager umMock;
        private readonly Mock<MockDbContext> dbMock;
        
        public DepartmentControllerTest()
        {
            var userStore = new Mock<IUserStore<GirafUser>>();
            umMock = MockUserManager(userStore);
            var lfMock = CreateMockLoggerFactory();
            
            dbMock = CreateMockDbContext();
            departmentController = new DepartmentController(new MockGirafService(dbMock.Object, umMock), lfMock.Object);
            departmentController.MockHttpContext();
        }
        
        [Fact]
        public void Department_Get_all_Departments_ExpectOK()
        {
            AddSampleDepartmentList(dbMock);
            var res = departmentController.Get();
            IActionResult aRes = res.Result;
            Assert.IsType<OkObjectResult>(aRes);
        }

        [Fact]
        public void Department_Get_Department_byID_ExpectOK()
        {
            AddSampleDepartmentList(dbMock);
            var res = departmentController.Get(1);
            IActionResult aRes = res.Result;
            Assert.IsType<OkObjectResult>(aRes);
        }

        [Fact]
        public void Department_Get_all_Departments_ExpectNotFound()
        {
            AddEmptyDepartmentList(dbMock);
            var res = departmentController.Get();
            IActionResult aRes = res.Result;
            Assert.IsType<NotFoundResult>(aRes);
        }

        [Fact]
        public void Department_Get_Department_byID_ExpectNotFound()
        {
            AddEmptyDepartmentList(dbMock);
            var res = departmentController.Get(1);
            IActionResult aRes = res.Result;
            Assert.IsType<NotFoundResult>(aRes);
        }
        
        [Fact]
        public void Department_Post_Department_ExpectOK()
        {
            var depDTO = new DepartmentDTO (new Department(){
                Name = "dep1"
            });
            
            umMock.MockLoginAsUser(MockUsers[0]);
            AddSampleDepartmentList(dbMock);

            var res = departmentController.Post(depDTO);
            IActionResult aRes = res.Result;
            Assert.IsType<OkObjectResult>(aRes);
        }

        [Fact]
        public void Department_AddUser_ExpectOK()
        {
            MockUsers.Add(new GirafUser());
            MockUsers[MockUsers.Count - 1].UserName = "AddUserTest";

            var res = departmentController.AddUser(1, MockUsers[MockUsers.Count - 1]);
            IActionResult aRes = res.Result;
            Assert.IsType<OkObjectResult>(aRes);
        }

        [Fact]
        public void Department_AddUser_ExpectNotFound()
        {
            MockUsers.Add(new GirafUser());
            MockUsers[MockUsers.Count - 1].UserName = "AddUserTest";

            var res = departmentController.AddUser(10, MockUsers[MockUsers.Count - 1]);
            IActionResult aRes = res.Result;
            Assert.IsType<NotFoundObjectResult>(aRes);
        }

        [Fact]
        public void Department_RemoveUser_ExpectOK()
        {
            MockUsers.Add(new GirafUser());
            MockUsers[MockUsers.Count - 1].UserName = "AddUserTest";

            var res = departmentController.RemoveUser(1, MockUsers[MockUsers.Count - 1]);
            IActionResult aRes = res.Result;
            Assert.IsType<OkObjectResult>(aRes);
        }

        /* If the user is null */
        [Fact]
        public void Department_RemoveUser_UserNullExpectBadRequest()
        {
            var res = departmentController.RemoveUser(1, null);
            IActionResult aRes = res.Result;
            Assert.IsType<BadRequestObjectResult>(aRes);
        }

        /* If the department is not found */
        [Fact]
        public void Department_RemoveUser_ExpectNotFound()
        {
            var res = departmentController.RemoveUser(10, MockUsers[0]);
            IActionResult aRes = res.Result;
            Assert.IsType<NotFoundObjectResult>(aRes);
        }

        /* If the user is valid but not in the specified department */
        [Fact]
        public void Department_RemoveUser_ExpectBadRequest()
        {
            MockUsers.Add(new GirafUser());
            MockUsers[MockUsers.Count - 1].UserName = "AddUserTest";

            var res = departmentController.RemoveUser(1, MockUsers[MockUsers.Count - 1]);
            IActionResult aRes = res.Result;
            Assert.IsType<BadRequestObjectResult>(aRes);
        }

        [Fact]
        public void Department_AddResource_ExpectOK()
        {

        }

        [Fact]
        public void Department_AddResource_ExpectDepartmentNotFound()
        {

        }

        [Fact]
        public void Deparment_AddResource_ExpectInvalidResourceBadRequest()
        {

        }

        [Fact]
        public void Department_AddResource_ExpectResourceNotFound()
        {

        }

        [Fact]
        public void Department_AddResource_ExpectUnauthorized()
        {

        }

        [Fact]
        public void Department_AddResource_ExpectAlreadyOwnedBadRequest()
        {

        }
    }
}