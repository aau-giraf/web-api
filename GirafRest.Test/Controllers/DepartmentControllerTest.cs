using GirafRest.Controllers;
using GirafRest.Models.DTOs.AccountDTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using GirafRest.Services;
using GirafRest.Models.DTOs;
using GirafRest.Models.Responses;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GirafRest.Interfaces;
using GirafRest.IRepositories;
using GirafRest.Test.Mocks;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Linq;
using GirafRest.Models.Enums;
using System.Collections.Generic;
using System.Security.Claims;
using GirafRest.Models;
using Microsoft.AspNetCore.Identity;
using static GirafRest.Test.UnitTestExtensions;
using Microsoft.AspNetCore.Http;

namespace GirafRest.Test
{
    public class DepartmentControllerTest
    {
        public class MockedDepartmentController : DepartmentController
        {
            public readonly Mock<IGirafService> _giraf;
            public readonly Mock<IDepartmentRepository> _departmentRepository;
            public readonly Mock<IGirafUserRepository> _userRepository;
            public readonly Mock<IGirafRoleRepository> _roleRepository;
            public readonly Mock<IPictogramRepository> _pictogramRepository;


            public MockedDepartmentController(RoleManager<GirafRole> rolemanager)
                :
            this(
                    new Mock<IGirafService>(),
                    new Mock<ILoggerFactory>(),
                    rolemanager,
                    new Mock<IGirafUserRepository>(),
                    new Mock<IDepartmentRepository>(),
                    new Mock<IGirafRoleRepository>(),
                    new Mock<IPictogramRepository>()
                )
            { }
            public MockedDepartmentController()
                :
            this(
                    new Mock<IGirafService>(),
                    new Mock<ILoggerFactory>(),
                    new MockRoleManager(new List<GirafRoles>()),
                    new Mock<IGirafUserRepository>(),
                    new Mock<IDepartmentRepository>(),
                    new Mock<IGirafRoleRepository>(),
                    new Mock<IPictogramRepository>()
                ) { }
            public MockedDepartmentController(
                    Mock<IGirafService> girafService,
                    Mock<ILoggerFactory> logger,
                    RoleManager<GirafRole> rolemanager,
                    Mock<IGirafUserRepository> userRep,

                    Mock<IDepartmentRepository> departmentRepository,
                    Mock<IGirafRoleRepository> girafRoleRepository,
                    Mock<IPictogramRepository> pictogramRepository
        ) : base(
            girafService.Object,
            logger.Object,
            rolemanager,   
            userRep.Object,
            departmentRepository.Object,
            girafRoleRepository.Object,
            pictogramRepository.Object
        )
              
            {
                girafService.SetupAllProperties();
               
                _giraf = girafService;
                _departmentRepository = departmentRepository;
                _roleRepository = girafRoleRepository;
                _userRepository = userRep;
                _pictogramRepository = pictogramRepository;
                //setting up 
                this.ControllerContext = new ControllerContext();
                this.ControllerContext.HttpContext = new DefaultHttpContext();

            }

        }


        [Fact]
        public void DepartmentController_Should_Get_All_Deparments()
        {
            //arranging
            var _departmentdto = new DepartmentDTO();
            List<DepartmentNameDTO> DepartmentDtos_expected = new List<DepartmentNameDTO>();
            DepartmentDtos_expected.Add(new DepartmentNameDTO(2, "Dalhaven"));
            DepartmentDtos_expected.Add(new DepartmentNameDTO(1, "BjælkeHytten"));
            DepartmentDtos_expected.Add(new DepartmentNameDTO(3, "Satelitten"));

            var departmentController = new MockedDepartmentController();

            //mocking
            var departmentRep = departmentController._departmentRepository;
            departmentRep.Setup(repo => repo.GetDepartmentNames()).
                Returns(Task.FromResult<List<DepartmentNameDTO>>(DepartmentDtos_expected));

            // Acting 
            var response = departmentController.Get();
            var objectResult = response as ObjectResult;
            var list = objectResult.Value as SuccessResponse<List<DepartmentNameDTO>>;

            //Assert
            Assert.True(DepartmentDtos_expected.SequenceEqual(list.Data));


        }
        [Fact]
        public void DepartmentController_Should_Get_By_ID()
        {
            // arranging
            var user = new GirafUser("thomas","thomas",new Department(),GirafRoles.Guardian);
            
            user.DepartmentKey = 1;

            List<DisplayNameDTO> displayNameDTOs = new List<DisplayNameDTO>();
            displayNameDTOs.Add(new DisplayNameDTO("thomas", GirafRoles.Guardian, "1"));
            displayNameDTOs.Add(new DisplayNameDTO("Christian", GirafRoles.Citizen, "2"));
            displayNameDTOs.Add(new DisplayNameDTO("Manfred", GirafRoles.Trustee, "3"));

            List<GirafUser> girafUsers  = new List<GirafUser>();
            girafUsers.Add(new GirafUser("thomas", "thomas", new Department(), GirafRoles.Guardian));
            girafUsers.Add(new GirafUser("Christian", "Christian", new Department(), GirafRoles.Citizen));
            girafUsers.Add(new GirafUser("Manfred", "Manfred", new Department(), GirafRoles.Trustee));

            var department = new Department();
            department.Key = 1;
            department.Name = "DenckerHaven";
            department.Members = girafUsers;
            var departmentController = new MockedDepartmentController();

            var principal = new ClaimsPrincipal(new ClaimsIdentity(null, "user"));
            var userManager = new MockUserManagerDepartment();
            var departmentRep = departmentController._departmentRepository;
            var deparmentdto = new Mock<DepartmentDTO>();
            userManager.MockLoginAsUser(user); 
            
            departmentController._giraf.Object._userManager = userManager;
            departmentController._departmentdto = deparmentdto.Object;

            //mock
            departmentRep.Setup(repo=>repo.GetDepartmentMembers((long)user.DepartmentKey))
                .Returns(Task.FromResult<Department>(department));
            deparmentdto.Setup(repo => repo.FindMembers(It.IsAny<List<GirafUser>>(),
                It.IsAny<RoleManager<GirafRole>>(), departmentController._giraf.Object)).Returns(displayNameDTOs);

            //acting 
            var response = departmentController.Get(1);
            var objectResult = response.Result as ObjectResult;
            var actualDTO = objectResult.Value as SuccessResponse<DepartmentDTO>;
            
            var expected = new DepartmentDTO(department, displayNameDTOs);

            //Assert
            Assert.Equal(expected.Name ,actualDTO.Data.Name);
            Assert.Equal(expected.Id, actualDTO.Data.Id);
        }

        [Fact]
        public void DepartmentController_should_get_citizens_by_id()
        {
            //Arranging
            var user = new GirafUser("thomas","thomas",new Department(),GirafRoles.Guardian);
            user.DepartmentKey = 1;

            var department = new Department();
            department.Key = 1;
            department.Name = "DenckerHaven";

            List<GirafUser> girafUsers  = new List<GirafUser>();
            girafUsers.Add(new GirafUser("Christian", "Christian", department, GirafRoles.Citizen));
            girafUsers[0].Id = "2";

            List<DisplayNameDTO> displayNameDTOs = new List<DisplayNameDTO>();
            displayNameDTOs.Add(new DisplayNameDTO("Christian", GirafRoles.Citizen, "2"));

            var userListids = new List<string>();
            userListids.Add("2");
            var userids = userListids.AsQueryable();

            var departmentController = new MockedDepartmentController();

            var userManager = new MockUserManagerDepartment();
            var departmentRep = departmentController._departmentRepository;
            var departmentRole = departmentController._roleRepository;
            var departmentUser = departmentController._userRepository;
            userManager.MockLoginAsUser(user);

            departmentController._giraf.Object._userManager = userManager;

            //mock
            departmentRep.Setup(repo => repo.GetDepartmentById(department.Key)).Returns(department);
            departmentRep.Setup(repo => repo.GetUserByDepartment(department, user)).Returns(user);
            departmentRep.Setup(repo => repo.GetCitizenRoleID()).Returns(GirafRole.Citizen);
            departmentRep.Setup(repo => repo.GetUsersWithRoleID(GirafRole.Citizen)).Returns(userids);
            departmentRole.Setup(repo => repo.GetAllCitizens()).Returns(userids);
            departmentUser.Setup(repo => repo.GetUsersInDepartment(department.Key, userids)).Returns(girafUsers);

            //act 
            var response = departmentController.GetCitizenNamesAsync(department.Key);
            var objectResult = response.Result as ObjectResult;
            var list = objectResult.Value as SuccessResponse <List<DisplayNameDTO>>;

            //assert
            Assert.True(displayNameDTOs.SequenceEqual(list.Data));

        }
        [Fact]
        public void Should_Create_New_Department()
        {
            //Arranging
            var departmentController = new MockedDepartmentController(new MockRoleManager(new List<GirafRoles> { GirafRoles.SuperUser}));
            var girafService = departmentController._giraf;
            var HttpContext = departmentController.ControllerContext.HttpContext;
            var depRep = departmentController._departmentRepository;
            var picRep = departmentController._pictogramRepository;
            var depDto = new Mock<DepartmentDTO>();
            var userRep = departmentController._userRepository;
            departmentController._departmentdto = depDto.Object;
            var testContext = new TestContext();
            var userManager = new MockUserManagerDepartment();
            
            girafService.Object._userManager = userManager;

            var dep = new Department();
       
            dep.Key = 1;
            dep.Name = "DenckerHaven";
            var girafUsers = new List<GirafUser>() { new GirafUser("luscus", "luscus", dep, GirafRoles.Citizen) };
            girafUsers[0].Id = "2";
            dep.Members = girafUsers;

            var displayNameDTOS = new List<DisplayNameDTO> { new DisplayNameDTO("luscus", GirafRoles.Citizen, "2") };
            var depDTO = new DepartmentDTO(dep, displayNameDTOS);
            var departmentUser = new GirafUser (depDTO.Name,depDTO.Name,dep,GirafRoles.Department) { IsDepartment = true };
            var authenticatedUser = new GirafUser("dencker","dencker",new Department(),GirafRoles.SuperUser);
            //mock
            girafService.Setup(repo => repo.LoadBasicUserDataAsync(HttpContext.User)).Returns(Task.FromResult<GirafUser>(authenticatedUser));
            depRep.Setup(repo => repo.Update(dep)).Returns(Task.CompletedTask);
            depRep.Setup(repo => repo.AddDepartment(dep)).Returns(Task.CompletedTask);
            depRep.Setup(repo => repo.GetUserRole(It.IsAny<RoleManager<GirafRole>>(),
                It.IsAny<UserManager<GirafUser>>(),
                It.IsAny<GirafUser>()
                )).Returns(Task.FromResult<GirafRoles>(GirafRoles.SuperUser));
            userRep.Setup(repo => repo.GetUserWithId("2")).Returns(Task.FromResult<GirafUser>(girafUsers[0]));
            picRep.Setup(repo => repo.GetPictogramWithID(It.IsAny<long>())).Returns(Task.FromResult<Pictogram>(new Pictogram()));
            depRep.Setup(repo => repo.AddDepartmentResource(It.IsAny<DepartmentResource>())).Returns(Task.CompletedTask);
            depDto.Setup(repo => repo.FindMembers(It.IsAny<List<GirafUser>>(),
                It.IsAny<RoleManager<GirafRole>>(), departmentController._giraf.Object)).Returns(displayNameDTOS);
            //act
            var response = departmentController.Post(depDTO);
            var result = response.Result as CreatedAtRouteResult;
            var val = result.Value as SuccessResponse<DepartmentDTO>;

            //assert
            Assert.Equal(val.Data.Name, depDTO.Name);
        }

        [Fact]
        public void Department_Name_Should_Change()
        {
            //Arranging
            var departmentController = new MockedDepartmentController();
            var departmentRep = departmentController._departmentRepository;

            DepartmentNameDTO departmentNameDTO = new DepartmentNameDTO();
            departmentNameDTO.Name = "Mansestuen";

            var department = new Department();
            department.Key = 1;
            department.Name = "DenckerHaven";

            //Mock
            departmentRep.Setup(repo => repo.GetDepartmentById(department.Key)).Returns(department);
            departmentRep.Setup(repo => repo.Update(department)).Returns(Task.CompletedTask);

            //act
            var response = departmentController.ChangeDepartmentName(department.Key, departmentNameDTO);
            var objectResult = response.Result as ObjectResult;
            var newName = objectResult.Value as SuccessResponse <string>;

            //assert
            Assert.True(newName.Data.Equals("Name of department changed"));
        }

        [Fact]
        public void Department_should_get_deleted()
        {
            //Arranging
            var departmentController = new MockedDepartmentController();
            var departmentRep = departmentController._departmentRepository;

            var department = new Department();
            department.Key = 1;
            department.Name = "DenckerHaven";

            //Mock
            departmentRep.Setup(repo => repo.GetDepartmentById(department.Key)).Returns(department);
            departmentRep.Setup(repo => repo.RemoveDepartment(department)).Returns(Task.CompletedTask);

            //act
            var response = departmentController.DeleteDepartment(department.Key);
            var objectResult = response.Result as ObjectResult;
            var res = objectResult.Value as SuccessResponse<string>;

            //assert
            Assert.True(res.Data.Equals("Department deleted"));

        }
    }
}