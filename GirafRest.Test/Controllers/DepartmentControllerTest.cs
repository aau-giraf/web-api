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
                _giraf = girafService;
                _departmentRepository = departmentRepository;
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
   
            var departmentController = new MockedDepartmentController();
            var principal = new ClaimsPrincipal(new ClaimsIdentity(null, "user"));
            var userManager = new Mock<UserManager<GirafUser>>();
            var departmentRep = departmentController._departmentRepository;
            var deparmentdto = new Mock<DepartmentDTO>();


            
            //mock
            userManager.Setup(repo=>repo.GetUserAsync(principal)).Returns(Task.FromResult<GirafUser>(user));
            userManager.Setup(repo => repo.IsInRoleAsync(user, GirafRole.SuperUser)).Returns(Task.FromResult<bool>(false));

            departmentRep.Setup(repo=>repo.GetDepartmentMembers((long)user.DepartmentKey))
                .Returns(Task.FromResult<Department>(department));
            deparmentdto.Setup(repo => repo.FindMembers(It.IsAny<List<GirafUser>>(),
                It.IsAny<RoleManager<GirafRole>>(), departmentController._giraf.Object)).Returns(displayNameDTOs);
            //acting 
            var response = departmentController.Get();
            var objectResult = response as ObjectResult;
            var actualDTO = objectResult.Value as DepartmentDTO;


            var expected = new DepartmentDTO(department, displayNameDTOs);

            Assert.Equal(actualDTO, expected);


        }




    }
}