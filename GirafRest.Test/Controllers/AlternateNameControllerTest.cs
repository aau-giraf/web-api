using System.Linq;
using GirafRest.Controllers;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using GirafRest.Models.Responses;
using GirafRest.Services;
using GirafRest.Test.Mocks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace GirafRest.Test
{
    public class AlternateNameControllerTest
    {
        private UnitTestExtensions.TestContext _testContext;
        private const int ADMIN_DEP_ONE = 0;

        private AlternateNameController InitialiseTest()
        {
            _testContext = new UnitTestExtensions.TestContext();
            var ac = new AlternateNameController(
                new MockGirafService(_testContext.MockDbContext.Object,
                    _testContext.MockUserManager), _testContext.MockLoggerFactory.Object,
                new GirafAuthenticationService(_testContext.MockDbContext.Object, _testContext.MockRoleManager.Object,
                    _testContext.MockUserManager));
            _testContext.MockHttpContext = ac.MockHttpContext();
            return ac;
        }

        #region PostAlternateName

        [Fact]
        public void PostAlternateName_CreateWithUserPictogram_Success()
        {
            AlternateNameController ac = InitialiseTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);

            AlternateNameDTO newAN = new AlternateNameDTO()
            {
                Citizen = mockUser.Id,
                Name = "Kage",
                Pictogram = _testContext.MockPictograms[1].Id
            };

            var res = ac.CreateAlternateName(newAN).Result as ObjectResult;
            var body = res.Value as SuccessResponse<AlternateNameDTO>;
            
            Assert.Equal(StatusCodes.Status201Created, res.StatusCode);
            Assert.Equal(newAN.Citizen,body.Data.Citizen);
            Assert.Equal(newAN.Pictogram,body.Data.Pictogram);
            Assert.Equal(newAN.Name,body.Data.Name);
        }
        
        [Fact]
        public void PostAlternateName_CreateWithoutUser_Error()
        {
            AlternateNameController ac = InitialiseTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);

            AlternateNameDTO newAN = new AlternateNameDTO()
            {
                Citizen = "",
                Name = "Kage",
                Pictogram = _testContext.MockPictograms[1].Id
            };

            var res = ac.CreateAlternateName(newAN).Result as ObjectResult;

            Assert.Equal(StatusCodes.Status404NotFound, res.StatusCode);
        }

        [Fact]
        public void PostAlternateName_CreateWithoutPictogram_Error()
        {
            AlternateNameController ac = InitialiseTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);

            AlternateNameDTO newAN = new AlternateNameDTO()
            {
                Citizen = mockUser.Id,
                Name = "Kage",
                Pictogram = -1
            };

            var res = ac.CreateAlternateName(newAN).Result as ObjectResult;

            Assert.Equal(StatusCodes.Status404NotFound, res.StatusCode);
        }
        
        [Fact]
        public void PostAlternateName_CreateWithoutName_Error()
        {
            AlternateNameController ac = InitialiseTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);

            AlternateNameDTO newAN = new AlternateNameDTO()
            {
                Citizen = mockUser.Id,
                Name = null,
                Pictogram = _testContext.MockPictograms[1].Id
            };

            var res = ac.CreateAlternateName(newAN).Result as ObjectResult;

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
        }
        
        [Fact]
        public void PostAlternateName_CreateAlreadyExists_Error()
        {
            AlternateNameController ac = InitialiseTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);

            AlternateNameDTO newAN = new AlternateNameDTO()
            {
                Citizen = mockUser.Id,
                Name = "Kage",
                Pictogram = _testContext.MockPictograms.First().Id
            };

            var res = ac.CreateAlternateName(newAN).Result as ObjectResult;
    
            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
        }
        
        [Fact]
        public void PostAlternateName_CreateWhileUnauthorized_Error()
        {
            AlternateNameController ac = InitialiseTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            

            AlternateNameDTO newAN = new AlternateNameDTO()
            {
                Citizen = mockUser.Id,
                Name = "Kage",
                Pictogram = _testContext.MockPictograms.First().Id
            };

            var res = ac.CreateAlternateName(newAN).Result as ObjectResult;
    
            Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
        }
        #endregion

        #region GetAlternateName

        [Fact]
        public void GetAlternateName_WithUserPictogram_Success()
        {
            AlternateNameController ac = InitialiseTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            Pictogram pic = _testContext.MockPictograms.First();

            var res = ac.GetName(mockUser.Id, pic.Id).Result as ObjectResult;
            var body = res.Value as SuccessResponse<AlternateNameDTO>;
            
            Assert.Equal(StatusCodes.Status200OK,res.StatusCode);
            Assert.Equal("Kage",body.Data.Name);
            Assert.Equal(mockUser.Id,body.Data.Citizen);
            Assert.Equal(pic.Id,body.Data.Pictogram);
        }

        [Fact]
        public void GetAlternateName_WithNoUser_Error()
        {
            AlternateNameController ac = InitialiseTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            Pictogram pic = _testContext.MockPictograms.First();

            var res = ac.GetName("", pic.Id).Result as ObjectResult;

            Assert.Equal(StatusCodes.Status404NotFound,res.StatusCode);

        }
        
        [Fact]
        public void GetAlternateName_WithUnauthorizesUser_Error()
        {
            AlternateNameController ac = InitialiseTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            Pictogram pic = _testContext.MockPictograms.First();

            var res = ac.GetName(mockUser.Id, pic.Id).Result as ObjectResult;

            Assert.Equal(StatusCodes.Status403Forbidden,res.StatusCode);

        }
        [Fact]
        public void GetAlternateName_WithNoPictogram_Error()
        {
            AlternateNameController ac = InitialiseTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            Pictogram pic = _testContext.MockPictograms.First();

            var res = ac.GetName(mockUser.Id, -1).Result as ObjectResult;

            Assert.Equal(StatusCodes.Status404NotFound,res.StatusCode);

        }
        
        [Fact]
        public void GetAlternateName_WithNoANExist_Error()
        {
            AlternateNameController ac = InitialiseTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            Pictogram pic = _testContext.MockPictograms[1];

            var res = ac.GetName(mockUser.Id, pic.Id).Result as ObjectResult;

            Assert.Equal(StatusCodes.Status404NotFound,res.StatusCode);

        }

        #endregion

        #region PutAlternateName

        [Fact]
        public void PutAlternateName_WithNewName_Success()
        {
            AlternateNameController ac = InitialiseTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            
            AlternateNameDTO newAN = new AlternateNameDTO()
            {
                Citizen = mockUser.Id,
                Name = "Leg",
                Pictogram = _testContext.MockPictograms.First().Id
            };

            var res = ac.EditAlternateName(0,newAN).Result as ObjectResult;
            var body = res.Value as SuccessResponse<AlternateNameDTO>;
            
            
            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            Assert.Equal("Leg",body.Data.Name);
        }
        
        [Fact]
        public void PutAlternateName_WithoutName_Error()
        {
            AlternateNameController ac = InitialiseTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            
            AlternateNameDTO newAN = new AlternateNameDTO()
            {
                Citizen = mockUser.Id,
                Name = null,
                Pictogram = _testContext.MockPictograms.First().Id
            };
            
            var res = ac.EditAlternateName(0,newAN).Result as ObjectResult;
            
            Assert.Equal(StatusCodes.Status400BadRequest,res.StatusCode);
        }
        
        

        

        #endregion
    }
}