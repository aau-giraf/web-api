using System.Threading;
using System.Threading.Tasks;
using GirafRest.Controllers;
using GirafRest.Interfaces;
using GirafRest.IRepositories;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using GirafRest.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GirafRest.Test
{
    public class AlternateNameControllerTest
    {
        public class MockedAlternateNameController : AlternateNameController {
            public MockedAlternateNameController()
                : this(
                    new Mock<IGirafService>(),
                    new Mock<ILoggerFactory>(),
                    new Mock<IPictogramRepository>(),
                    new Mock<IGirafUserRepository>(),
                    new Mock<IAlternateNameRepository>()
                ) { }

            public MockedAlternateNameController(
                Mock<IGirafService> giraf,
                Mock<ILoggerFactory> loggerFactory,
                Mock<IPictogramRepository> pictogramRepository,
                Mock<IGirafUserRepository> girafUserRepository,
                Mock<IAlternateNameRepository> alternateNameRepository
            ) : base(
                giraf.Object,
                loggerFactory.Object,
                pictogramRepository.Object,
                girafUserRepository.Object,
                alternateNameRepository.Object
            ) {
                Giraf = giraf;
                LoggerFactory = loggerFactory;
                PictogramRepository = pictogramRepository;
                GirafUserRepository = girafUserRepository;
                AlternateNameRepository = alternateNameRepository;

                // The following are primary mocks whcih are generic.
                //   These are added to ease the development of tests.
                var affectedRows = 1;
                Giraf.Setup(
                    service => service._context.SaveChangesAsync(It.IsAny<CancellationToken>())
                ).Returns(Task.FromResult(affectedRows));
            }

            public Mock<IGirafService> Giraf { get; }
            public Mock<ILoggerFactory> LoggerFactory { get; }
            public Mock<IPictogramRepository> PictogramRepository { get; }
            public Mock<IGirafUserRepository> GirafUserRepository { get; }
            public Mock<IAlternateNameRepository> AlternateNameRepository { get; }
        }

        [Fact]
        public void CreateAlternateName_CreateWithUserPictogram_Success() {
            // Arrange
            var controller = new MockedAlternateNameController();
            var newAlternateName = new AlternateNameDTO() {
                Citizen = "Danielsan",
                Name = "Tommysan",
                Pictogram = 420691337,
            };
            var user = new GirafUser() {
                Id = newAlternateName.Citizen
            };
            var pictogram = new Pictogram() {
                Id = newAlternateName.Pictogram
            };

            // Mock
            controller.GirafUserRepository.Setup(
                repo => repo.GetByID(newAlternateName.Citizen)
            ).Returns(user);
            controller.PictogramRepository.Setup(
                repo => repo.GetByID(newAlternateName.Pictogram)
            ).Returns(pictogram);
            controller.AlternateNameRepository.Setup(
                repo => repo.GetForUser(user.Id, pictogram.Id)
            ).Returns((AlternateName)default);
            controller.AlternateNameRepository.Setup(
                repo => repo.Add(It.IsAny<AlternateName>())
            );

            // Act
            var response = controller.CreateAlternateName(newAlternateName);
            var result = response.Result as ObjectResult;
            var body = result.Value as SuccessResponse<AlternateNameDTO>;

            // Assert
            Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
            Assert.Equal(newAlternateName.Citizen, body.Data.Citizen);
            Assert.Equal(newAlternateName.Pictogram, body.Data.Pictogram);
            Assert.Equal(newAlternateName.Name, body.Data.Name);
        }

        [Fact]
        public void CreateAlternateName_CreateWithoutUser_Error() {
            // Arrange
            var controller = new MockedAlternateNameController();
            var newAlternateName = new AlternateNameDTO() {
                Citizen = "",
                Name = "Tommysan",
                Pictogram = 420691337,
            };
            var pictogram = new Pictogram() {
                Id = newAlternateName.Pictogram
            };
             var user = new GirafUser() {
                Id = newAlternateName.Citizen
            };

            // Mock 
            controller.GirafUserRepository.Setup(
                repo => repo.GetByID(newAlternateName.Citizen)
            ).Returns(user);
            controller.PictogramRepository.Setup(
                repo => repo.GetByID(newAlternateName.Pictogram)
            ).Returns(pictogram);
            controller.AlternateNameRepository.Setup(
                repo => repo.GetForUser(user.Id, pictogram.Id)
            ).Returns((AlternateName)default);

            controller.AlternateNameRepository.Setup(
                repo => repo.Add(It.IsAny<AlternateName>())
            );

            // Act 
            var response = controller.CreateAlternateName(newAlternateName);
            var result = response.Result as ObjectResult;
            var body = result.StatusCode;

            // Assert 
            Assert.Equal(StatusCodes.Status400BadRequest, body);

        }

        [Fact]
        public void CreateAlternateName_CreateWithoutPictogram_Error() {
            // Arrange
            var controller = new MockedAlternateNameController();
            var newAlternateName = new AlternateNameDTO() {
                Citizen = "Danielsan",
                Name = "Tommysan",
                Pictogram = -1
            };
            var pictogram = new Pictogram() {
                Id = newAlternateName.Pictogram
            };
             var user = new GirafUser() {
                Id = newAlternateName.Citizen
            };

            // Mock 
            controller.GirafUserRepository.Setup(
                repo => repo.GetByID(newAlternateName.Citizen)
            ).Returns(user);
            
            /*
            controller.PictogramRepository.Setup(
                repo => repo.GetByID(newAlternateName.Pictogram)
            ).Returns(pictogram);
            */
            controller.AlternateNameRepository.Setup(
                repo => repo.GetForUser(user.Id, pictogram.Id)
            ).Returns((AlternateName)default);

            controller.AlternateNameRepository.Setup(
                repo => repo.Add(It.IsAny<AlternateName>())
            );

            // Act 
            var response = controller.CreateAlternateName(newAlternateName);
            var result = response.Result as ObjectResult;
            var body = result.StatusCode;

            // Assert 
            Assert.Equal(StatusCodes.Status404NotFound, body);

        }

        [Fact]
        public void  CreateAlternateName_CreateWithoutName_Error() {
            // Arrange
            var controller = new MockedAlternateNameController();
            var newAlternateName = new AlternateNameDTO() {
                Citizen = "Danielsan",
                Name = null,
                Pictogram = 123453
            };
            var pictogram = new Pictogram() {
                Id = newAlternateName.Pictogram
            };
             var user = new GirafUser() {
                Id = newAlternateName.Citizen
            };

            // Mock 
            controller.GirafUserRepository.Setup(
                repo => repo.GetByID(newAlternateName.Citizen)
            ).Returns(user);
            controller.PictogramRepository.Setup(
                repo => repo.GetByID(newAlternateName.Pictogram)
            ).Returns(pictogram);
            controller.AlternateNameRepository.Setup(
                repo => repo.GetForUser(user.Id, pictogram.Id)
            ).Returns((AlternateName)default);
            controller.AlternateNameRepository.Setup(
                repo => repo.Add(It.IsAny<AlternateName>())
            );

            // Act 
            var response = controller.CreateAlternateName(newAlternateName);
            var result = response.Result as ObjectResult;
            var body = result.StatusCode;

            // Assert 
            Assert.Equal(StatusCodes.Status400BadRequest, body); 
        }

        [Fact]
        public void CreateAlternateName_CreateAlreadyExists_Error(){
            // Arrange
            var controller = new MockedAlternateNameController();
            var newAlternateName = new AlternateNameDTO() {
                Citizen = "Danielsan",
                Name = "Tommysan",
                Pictogram = 123453
            };
            var pictogram = new Pictogram() {
                Id = newAlternateName.Pictogram
            };
             var user = new GirafUser() {
                Id = newAlternateName.Citizen
            };

            // Mock 
            controller.GirafUserRepository.Setup(
                repo => repo.GetByID(newAlternateName.Citizen)
            ).Returns(user);
            controller.PictogramRepository.Setup(
                repo => repo.GetByID(newAlternateName.Pictogram)
            ).Returns(pictogram);
            controller.AlternateNameRepository.Setup(
                repo => repo.UserAlreadyHas(user.Id, pictogram.Id)
            ).Returns(true);

            // Act 
            var response = controller.CreateAlternateName(newAlternateName);
            var result = response.Result as ObjectResult;
            var body = result.StatusCode;

            // Assert 
            Assert.Equal(StatusCodes.Status409Conflict, body);
        }


        /*
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
        
        [Fact]
        public void PostAlternateName_CreateWithoutDTO_Error()
        {
            AlternateNameController ac = InitialiseTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];


            AlternateNameDTO newAN = null;

            var res = ac.CreateAlternateName(newAN).Result as ObjectResult;
    
            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
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
        
        [Fact]
        public void PutAlternateName_WithUnauthorizedUser_Error()
        {
            AlternateNameController ac = InitialiseTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];

            AlternateNameDTO newAN = new AlternateNameDTO()
            {
                Citizen = mockUser.Id,
                Name = "Kage",
                Pictogram = _testContext.MockPictograms.First().Id
            };
            
            var res = ac.EditAlternateName(0,newAN).Result as ObjectResult;
            
            Assert.Equal(StatusCodes.Status403Forbidden,res.StatusCode);
        }
        
        [Fact]
        public void PutAlternateName_WithoutUser_Error()
        {
            AlternateNameController ac = InitialiseTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            
            AlternateNameDTO newAN = new AlternateNameDTO()
            {
                Citizen = "",
                Name = "Kage",
                Pictogram = _testContext.MockPictograms.First().Id
            };
            
            var res = ac.EditAlternateName(0,newAN).Result as ObjectResult;
            
            Assert.Equal(StatusCodes.Status404NotFound,res.StatusCode);
        }
        
        [Fact]
        public void PutAlternateName_WithoutPictogram_Error()
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
            
            var res = ac.EditAlternateName(0,newAN).Result as ObjectResult;
            
            Assert.Equal(StatusCodes.Status404NotFound,res.StatusCode);
        }
        
        [Fact]
        public void PutAlternateName_WithWrongUser_Error()
        {
            AlternateNameController ac = InitialiseTest();
            GirafUser mockUser = _testContext.MockUsers[ADMIN_DEP_ONE];
            GirafUser otherUser = _testContext.MockUsers[1];
            _testContext.MockUserManager.MockLoginAsUser(mockUser);
            
            AlternateNameDTO newAN = new AlternateNameDTO()
            {
                Citizen = otherUser.Id,
                Name = "Kage",
                Pictogram = _testContext.MockPictograms.First().Id
            };
            
            var res = ac.EditAlternateName(0,newAN).Result as ObjectResult;
            
            Assert.Equal(StatusCodes.Status400BadRequest,res.StatusCode);
        }
        
        [Fact]
        public void PutAlternateName_WithWrongPictogram_Error()
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

            var res = ac.EditAlternateName(0,newAN).Result as ObjectResult;

            Assert.Equal(StatusCodes.Status400BadRequest,res.StatusCode);
        }
    */
        [Fact]
        public void PutAlternateName_WithWrongPictogram_Error() {
            // Arrange
            var controller = new MockedAlternateNameController();
            var oldAlternateName = new AlternateName() {
                Id = 80085,
            };
            var newAlternateNameDTO = new AlternateNameDTO() {
                Name = "hywmongous",
                Citizen = "Brandhoej",
                Pictogram = 420,
            };

            // Mock

            // Act
            var response = controller.EditAlternateName(oldAlternateName.Id + 1, newAlternateNameDTO);
            var result = response.Result as ObjectResult;
            var body = result.Value as SuccessResponse<AlternateNameDTO>;

            // Assert
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        }

        [Fact]
        public void PutAlternateName_EditWithoutDTO_Error() {
            // Arrange
            var controller = new MockedAlternateNameController();
            AlternateNameDTO newAlternateNameDTO = default;
            var oldAlternateName = new AlternateName() {
                Id = 80085,
            };

            // Act
            var response = controller.EditAlternateName(oldAlternateName.Id, newAlternateNameDTO);
            var result = response.Result as ObjectResult;
            var body = result.Value as SuccessResponse<AlternateNameDTO>;

            // Assert
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }

        [Fact]
        public void PutAlternateName_EditNotExistingAN_Error() {
            // Arrange
            var controller = new MockedAlternateNameController();
            var newAlternateNameDTO = new AlternateNameDTO() {
                Name = "hywmongous",
                Citizen = "Brandhoej",
                Pictogram = 420,
            };
            var oldAlternateName = new AlternateName() {
                Id = 80085,
                // Different from newAlternateNameDTO
                PictogramId = 123,
            };
            var user = new GirafUser() {
                Id = newAlternateNameDTO.Citizen,
            };

            // Mock
            controller.GirafUserRepository.Setup(
                repo => repo.Get(newAlternateNameDTO.Citizen)
            ).Returns(user);
            controller.AlternateNameRepository.Setup(
                repo => repo.Get(oldAlternateName.Id)
            ).Returns((AlternateName)default);

            // Act
            var response = controller.EditAlternateName(oldAlternateName.Id, newAlternateNameDTO);
            var result = response.Result as ObjectResult;
            var body = result.Value as SuccessResponse<AlternateNameDTO>;

            // Assert
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        }
    }
}