using System.Collections.Generic;
using System.Threading.Tasks;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using GirafRest.Models.Enums;
using GirafRest.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GirafRest.Test
{
    public class UserControllerTest
    {
        
        
        #region Get/Update

        [Fact]
        public async Task GetUserWithId_200OK()
        {
            //Arrange
            var controller = new MockedUserController();
            var repository = controller.GirafUserRepository;
            
            //Mock
            repository.Setup(x => x.GetUserWithId(controller.testUser.Id))
                .ReturnsAsync(controller.testUser);

            //Act
            var response = await controller.GetUser(controller.testUser.Id);
            var result = response as ObjectResult;
            var body = result.Value as SuccessResponse<GirafUserDTO>;
            
            //Assert
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            // check that we are logged in as the correct user
            Assert.Equal(controller.testUser.UserName, body.Data.Username);
            Assert.Equal(controller.testUser.DepartmentKey, body.Data.Department);
        }

        [Fact]
        public async Task GetUserRole_200OK()
        {
            //Arrange
            var controller = new MockedUserController();
            
            //Mock
            controller.GirafUserRepository.Setup(g => g.GetUserByUsername(controller.testUser.UserName))
                .ReturnsAsync(controller.testUser);
            //Act
            var response = await controller.GetUserRole(controller.testUser.UserName);
            var result = response as ObjectResult;
            //Assert
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async Task GetUserRole_400BadRequest()
        {
            var controller = new MockedUserController();

            var response = await controller.GetUserRole(null) as ObjectResult;
            
            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
            
            
        }

        [Fact]
        public async Task GetUserRole_404UserNotFound()
        {
            var controller = new MockedUserController();
            
            controller.GirafUserRepository.Setup(g => g.GetUserByUsername("34657946"))
                .ReturnsAsync(controller.testUser);

            var response = await controller.GetUserRole(controller.testUser.UserName);
            var result = response as ObjectResult;

            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        }

        [Fact]
        public async Task GetSettings_200OK()
        {
            var controller = new MockedUserController();

            controller.GirafUserRepository.Setup(x => x.GetUserSettingsByWeekDayColor(controller.testUser.Id))
                .Returns(controller.testUser);

            var response = await controller.GetSettings(controller.testUser.Id) as ObjectResult;
            
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            
        }

        [Fact]
        public async Task GetSettings_400BadRequest()
        {
            var controller = new MockedUserController();

            var response = await controller.GetSettings(null) as ObjectResult;
            
            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetSettings_404NotFound()
        {
            var controller = new MockedUserController();

            controller.GirafUserRepository.Setup(x => x.GetUserSettingsByWeekDayColor("48593285"))
                .Returns(controller.testUser);

            var response = await controller.GetSettings(controller.testUser.Id) as ObjectResult;
            
            Assert.Equal(StatusCodes.Status404NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateUser_200OK()
        {
            var controller = new MockedUserController();
            
            controller.GirafUserRepository.Setup(x => x.GetUserWithId(controller.testUser.Id)).ReturnsAsync(controller.testUser);
            controller.GirafUserRepository.Setup(x => x.Update(controller.testUser));
            controller.GirafUserRepository.Setup(x => x.SaveChangesAsync());

            var userDTO = new GirafUserDTO(controller.testUser, GirafRoles.Citizen);

            var response = await controller.UpdateUser(controller.testUser.Id,userDTO) as ObjectResult;
            
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            
        }
        
        [Fact]
        public async Task UpdateUser_400BadRequest()
        {
            var controller = new MockedUserController();
            
            var response = await controller.UpdateUser(controller.testUser.Id,new GirafUserDTO()) as ObjectResult;
            var body = response.Value as ErrorResponse;
            
            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
            Assert.Equal(ErrorCode.MissingProperties, body.ErrorCode);
            
        }

        [Fact]
        public async Task UpdateUser_409Conflict()
        {
            var controller = new MockedUserController();
            var girafUserDto = new GirafUserDTO(controller.testUser, GirafRoles.Citizen)
            {
                Username = "bob",
                DisplayName = "Bob",
            };

            controller.GirafUserRepository.Setup(x => x.GetUserWithId(controller.testUser.Id)).ReturnsAsync(controller.testUser);
            controller.GirafUserRepository.Setup(x => x.CheckIfUsernameHasSameId(girafUserDto, controller.testUser))
                .Returns(true);
            var response = await controller.UpdateUser(controller.testUser.Id, girafUserDto) as ObjectResult;
            
            Assert.Equal(StatusCodes.Status409Conflict, response.StatusCode);
        }
        
        #endregion
        
        #region UserIcon

        [Fact]
        public async Task GetUserIcon_404IconNotFound()
        {
            var controller = new MockedUserController();
            
            controller.GirafUserRepository.Setup(y => y.GetUserWithId(controller.testUser.Id)).ReturnsAsync(controller.testUser);
            
            var response = await controller.GetUserIcon(controller.testUser.Id);
            var result = response as ObjectResult;
            var body = result.Value as ErrorResponse;
            
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
            Assert.Equal(ErrorCode.UserHasNoIcon, body.ErrorCode);
        }

        [Fact]
        public async Task GetUserIcon_SUCCESS()
        {
            var controller = new MockedUserController();
            controller.testUser.UserIcon = new byte[]{0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20};
            controller.GirafUserRepository.Setup(y => y.GetUserWithId(controller.testUser.Id)).ReturnsAsync(controller.testUser);
            var response = await controller.GetUserIcon(controller.testUser.Id);
            var result = response as ObjectResult;
            var body = result.Value as SuccessResponse<ImageDTO>;
            
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.True(body.Data.Image != null);
        }
        
        [Fact]
        public async Task GetUserIcon_404UserNotFound()
        {
            var controller = new MockedUserController();
            var response = await controller.GetUserIcon(null);
            var result = response as ObjectResult;
            var body = result.Value as ErrorResponse;
            
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
            Assert.Equal(ErrorCode.UserNotFound, body.ErrorCode);
        }
        
        [Fact]
        public async Task DeleteUserIcon_200OK()
        {
            //Arrange
            var controller = new MockedUserController();
           
            controller.testUser.UserIcon = new byte[]{0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20};
            
            //Mock
            controller.GirafUserRepository.Setup(y => y.GetUserWithId(controller.testUser.Id)).ReturnsAsync(controller.testUser);
            controller.GirafUserRepository.Setup(z => z.SaveChangesAsync());
            
            //Act
            var response = await controller.DeleteUserIcon(controller.testUser.Id);
            var result = response as ObjectResult;

            //Assert
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            
            // Get icon to be sure it is deleted
            var result2 =  controller.GetUserIcon(controller.testUser.Id).Result as ObjectResult;
            var body = result2.Value as ErrorResponse;


            Assert.Equal(StatusCodes.Status404NotFound, result2.StatusCode);
            Assert.Equal(ErrorCode.UserHasNoIcon, body.ErrorCode);
            
        }

        [Fact]
        public async Task DeleteUserIcon_404NotFound()
        {
            //Arrange
            var controller = new MockedUserController();
            
            
            //Mock
            controller.GirafUserRepository.Setup(x => x.GetUserWithId(controller.testUser.Id)).ReturnsAsync(controller.testUser);
            controller.GirafUserRepository.Setup(x => x.SaveChangesAsync());
            
            //Act
            var response = await controller.DeleteUserIcon(controller.testUser.Id);
            var result = response as ObjectResult;

            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        }
        #endregion

        #region GetGuardians

        [Fact]
        public async Task GetGuardian_200OK()
        {
            var controller = new MockedUserController();
            GuardianRelation gr = new GuardianRelation() { GuardianId = "1"};
            controller.testUser.Id = "1";
            controller.testUser.Guardians = new List<GuardianRelation>();
            controller.testUser.Guardians.Add(gr);
            controller.GirafUserRepository.Setup(x => x.GetGuardianWithId(controller.testUser.Id))
                .Returns(controller.testUser);
            controller.GirafUserRepository.Setup(x => x.GetGuardianFromRelation(gr)).Returns(controller.testUser);

            var response = await controller.GetGuardians(controller.testUser.Id) as ObjectResult;
            
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
        }

        [Fact]
        public async Task GetGuardians_400BadRequest()
        {
            var controller = new MockedUserController();
            
            controller.GirafUserRepository.Setup(x => x.GetGuardianWithId(null))
                .Returns(controller.testUser);
            
            var response = await controller.GetGuardians(controller.testUser.Id) as ObjectResult;
            var body = response.Value as ErrorResponse;
            
            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
            Assert.Equal(ErrorCode.InvalidProperties, body.ErrorCode);

        }

        [Fact]
        public async Task GetGuardians_404NotFound()
        {
            var controller = new MockedUserController();
            
            GuardianRelation gr = new GuardianRelation() { GuardianId = "1"};
            controller.testUser.Id = "1";

            controller.GirafUserRepository.Setup(x => x.GetGuardianWithId(controller.testUser.Id))
                .Returns(controller.testUser);
            
            controller.GirafUserRepository.Setup(x => x.GetGuardianFromRelation(gr)).Returns(controller.testUser);
            
            var response = await controller.GetGuardians(controller.testUser.Id) as ObjectResult;
            
            Assert.Equal(StatusCodes.Status404NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetGuardians_403Forbidden()
        {
            var controller = new MockedUserController();
            GuardianRelation gr = new GuardianRelation() { GuardianId = "1"};
            controller.guardianUser.Id = "1";
            
            
            controller.GirafUserRepository.Setup(x => x.GetGuardianWithId(controller.guardianUser.Id))
                .Returns(controller.guardianUser);
            
            var response = await controller.GetGuardians(controller.guardianUser.Id) as ObjectResult;
            var body = response.Value as ErrorResponse;
            
            Assert.Equal(StatusCodes.Status403Forbidden, response.StatusCode);
            Assert.Equal(ErrorCode.Forbidden, body.ErrorCode);
        }
        
        

        #endregion

        #region GetCitizens

        [Fact]
        public async Task GetCitizens_400BadRequest()
        {
            var controller = new MockedUserController();
            var response = await controller.GetCitizens(null) as ObjectResult;
            
            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetCitizens_200OK()
        {
            var controller = new MockedUserController();
            var gUser = controller.guardianUser;
            var cUser = controller.testUser;
            var gr = new GuardianRelation(gUser, cUser);
            gUser.Citizens.Add(gr);


            controller.GirafUserRepository.Setup(x => x.GetCitizensWithId(gUser.Id))
                .Returns(gUser);
            controller.GirafUserRepository.Setup(x => x.GetFirstCitizen(gr)).Returns(cUser);

            var response = await controller.GetCitizens(gUser.Id) as ObjectResult;
            
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
        }

        #endregion

        #region AddGuardianCitizenRelationship

        [Fact]
        public async Task AddGuardianCitizenRelationship_200OK()
        {
            var controller = new MockedUserController();
            var gUser = controller.guardianUser;
            var cUser = controller.testUser;
            gUser.AddCitizen(cUser);
            

            controller.GirafUserRepository.Setup(x => x.GetCitizenRelationship(cUser.Id)).Returns(cUser);
            controller.GirafUserRepository.Setup(x => x.GetUserWithId(gUser.Id)).ReturnsAsync(gUser);

            var response = await controller.AddGuardianCitizenRelationship(gUser.Id, cUser.Id) as ObjectResult;
            
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
        }

        #endregion

        #region UpdateUserSettings

        [Fact]
        public async Task UpdateUserSettings_200OK()   
        {
            var controller = new MockedUserController();
            var user = controller.testUser;

            controller.GirafUserRepository.Setup(x => x.GetUserSettingsByWeekDayColor(user.Id)).Returns(user);
            controller.GirafUserRepository.Setup(x => x.SaveChangesAsync());

            var response = await controller.UpdateUserSettings(user.Id, new SettingDTO()) as ObjectResult;
            
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
        }

        [Fact]
        public async Task UpdateUserSettings_404NotFound()
        {
            var controller = new MockedUserController();                                                         
            var user = controller.testUser;                                                                      
                                                                                                     
            controller.GirafUserRepository.Setup(x => x.GetUserSettingsByWeekDayColor(null)).Returns(user);

            var response = await controller.UpdateUserSettings(user.Id, new SettingDTO()) as ObjectResult;       
                                                                                                     
            Assert.Equal(StatusCodes.Status404NotFound, response.StatusCode);                                          
        }

        [Fact]
        public async Task UpdateUserSettings_400BadRequest()
        {
            var controller = new MockedUserController();                                                         
            var user = controller.guardianUser;                                                                      
                                                                                                     
            controller.GirafUserRepository.Setup(x => x.GetUserSettingsByWeekDayColor(user.Id)).Returns(user);

            var response = await controller.UpdateUserSettings(user.Id, new SettingDTO()) as ObjectResult;       
                                                                                                     
            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode); 
        }

        
        #endregion

        
    }
}
