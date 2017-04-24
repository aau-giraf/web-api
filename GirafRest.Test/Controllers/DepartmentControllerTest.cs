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
    }
}