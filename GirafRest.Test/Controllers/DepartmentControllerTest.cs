using Moq;
using Xunit;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GirafRest.Setup;
using GirafRest.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using GirafRest.Data;
using Microsoft.Extensions.Logging;
using GirafRest.Models.DTOs;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using GirafRest.Controllers;
using Microsoft.AspNetCore.Http;
using GirafRest.Test.Mocks;

namespace GirafRest.Test.Controllers
{
    public class DepartmentControllerTest
    {
        private readonly DepartmentController departmentController;

        private readonly Mock<GirafDbContext> dbContextMock;
        private readonly Mock<IUserStore<GirafUser>> userStore;
        private readonly MockUserManager umMock;
        private readonly Mock<ILoggerFactory> lfMock;
        private readonly List<string> logs;
        private readonly Mock<MockDbContext> dbMock;
        
        public DepartmentControllerTest()
        {
            userStore = new Mock<IUserStore<GirafUser>>();
            umMock = UnitTestExtensions.MockUserManager(userStore);
            var lfMock = UnitTestExtensions.CreateMockLoggerFactory();
            
            dbMock = UnitTestExtensions.CreateMockDbContext();
            departmentController = new DepartmentController(new MockGirafService(dbMock.Object, umMock), lfMock.Object);
            var hContext = new DefaultHttpContext();
            departmentController.ControllerContext = new ControllerContext();
            departmentController.ControllerContext.HttpContext = hContext;
        }
        
        [Fact]
        public void Department_Get_all_Departments_ExpectOK()
        {
            UnitTestExtensions.AddSampleDepartmentList(dbMock);
            var res = departmentController.Get();
            IActionResult aRes = res.Result;
            Assert.IsType<OkObjectResult>(aRes);
        }

        [Fact]
        public void Department_Get_Department_byID_ExpectOK()
        {
            UnitTestExtensions.AddSampleDepartmentList(dbMock);
            var res = departmentController.Get(1);
            IActionResult aRes = res.Result;
            Assert.IsType<OkObjectResult>(aRes);
        }

        [Fact]
        public void Department_Get_all_Departments_ExpectNotFound()
        {
            UnitTestExtensions.AddEmptyDepartmentList(dbMock);
            var res = departmentController.Get();
            IActionResult aRes = res.Result;
            Assert.IsType<NotFoundResult>(aRes);
        }

        [Fact]
        public void Department_Get_Department_byID_ExpectNotFound()
        {
            UnitTestExtensions.AddEmptyDepartmentList(dbMock);
            var res = departmentController.Get(1);
            IActionResult aRes = res.Result;
            Assert.IsType<NotFoundResult>(aRes);
        }
        
        [Fact]
        public void Department_Post_Department_ExpectOK()
        {
            var depDTO = new DepartmentDTO (new Department(){
                Key = 1,
                Name = "dep1"
            });
            
            umMock.MockLoginAsUser(UnitTestExtensions.MockUsers[0]);
            UnitTestExtensions.AddSampleDepartmentList(dbMock);

            var res = departmentController.Post(depDTO);
            IActionResult aRes = res.Result;
            Assert.IsType<OkObjectResult>(aRes);
        }
    }
}