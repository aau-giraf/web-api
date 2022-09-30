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
using Ubiety.Dns.Core;

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
     

            public MockedDepartmentController()
                :
            this(
                    new Mock<IGirafService>(),
                    new Mock<ILoggerFactory>(),
                    new Mock<IGirafUserRepository>(),
                    new Mock<IDepartmentRepository>(),
                    new Mock<IGirafRoleRepository>(),
                    new Mock<IPictogramRepository>()
                ) { }
            public MockedDepartmentController(
                    Mock<IGirafService> girafService,
                    Mock<ILoggerFactory> logger,
                    Mock<IGirafUserRepository> userRep,
                
                    Mock<IDepartmentRepository> departmentRepository,
                    Mock<IGirafRoleRepository> girafRoleRepository,
                    Mock<IPictogramRepository> pictogramRepository
        ) : base(
            girafService.Object,
            logger.Object,

            new MockRoleManager(new List<GirafRoles>()),
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
    }
}