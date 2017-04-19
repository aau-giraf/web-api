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

namespace GirafRest.Test.Controllers
{
    public class DepartmentControllerTest
    {
        private readonly DepartmentController departmentController;
        private readonly Mock<GirafDbContext> dbMock;
        private readonly Mock<IUserStore<GirafUser>> userStore;
        private readonly Mock<ILoggerFactory> lfMock;
        private readonly List<string> logs;
        public testDepartments testDeps;
        
        public DepartmentControllerTest()
        {
            userStore = new Mock<IUserStore<GirafUser>>();
            var umMock = UnitTestExtensions.MockUserManager(userStore);
            var lfMock = UnitTestExtensions.CreateMockLoggerFactory();
            
            setupDepartments (umMock);

            dbMock = new Mock<GirafDbContext> ();
            var depMockDbset = UnitTestExtensions.CreateMockDbSet<Department>(testDeps.departments);
            dbMock.Setup(x => x.Departments).Returns(depMockDbset.Object);

            departmentController = new DepartmentController(dbMock.Object, umMock, lfMock.Object);
            var hContext = new DefaultHttpContext();
            departmentController.ControllerContext = new ControllerContext();
            departmentController.ControllerContext.HttpContext = hContext;
        }

        public struct testDepartments {
            public List<Pictogram> pictograms;
            public List<GirafUser> users;
            public List<Department> departments;

            public testDepartments (List<Pictogram> p, List<GirafUser> u, List<Department> d) {
                pictograms = p;
                users = u;
                departments = d;
            }
        }

        public virtual void setupDepartments (UserManager<GirafUser> userManager)
        {
            /* setup pictograms */
            List<Pictogram> pictograms = new List<Pictogram> {
                new Pictogram("Public Picto1", AccessLevel.PUBLIC),
                new Pictogram("Public Picto2", AccessLevel.PUBLIC),
                new Pictogram("No restrictions", AccessLevel.PUBLIC),
                new Pictogram("Restricted", AccessLevel.PRIVATE),
                new Pictogram("Private Pictogram", AccessLevel.PRIVATE)
            };

            /* setup users */
            var users = new List<GirafUser> {
                new GirafUser("Alice", 1),
                new GirafUser("Bob", 2),
                new GirafUser("Morten", 1),
                new GirafUser("Brian", 2)
            };
            
            foreach (var u in users) {
                userManager.CreateAsync(u, "mocking");
            }

            //testDeps = new testDepartments(pictograms, users, departments);
        }
        
        [Fact]
        public void DepartmentGetSuccessTest()
        {
            var res = departmentController.Get();
            IActionResult aRes = res.Result;
            Assert.IsType<OkObjectResult>(aRes);
        }
    }
}