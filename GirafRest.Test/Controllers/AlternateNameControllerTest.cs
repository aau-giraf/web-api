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
        #region Create

        [Fact]
        public void Create_CorrectRequest_Status201() {
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
            var response = controller.Create(newAlternateName);
            var result = response.Result as ObjectResult;
            var body = result.Value as SuccessResponse<AlternateNameDTO>;

            // Assert
            Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
            Assert.Equal(newAlternateName.Citizen, body.Data.Citizen);
            Assert.Equal(newAlternateName.Pictogram, body.Data.Pictogram);
            Assert.Equal(newAlternateName.Name, body.Data.Name);
        }

        [Fact]
        public void Create_CreateWithoutUser_Status400() {
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
            var response = controller.Create(newAlternateName);
            var result = response.Result as ObjectResult;
            var body = result.StatusCode;

            // Assert 
            Assert.Equal(StatusCodes.Status400BadRequest, body);

        }

        [Fact]
        public void Create_CreateWithoutPictogram_Status404() {
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
            
            controller.AlternateNameRepository.Setup(
                repo => repo.GetForUser(user.Id, pictogram.Id)
            ).Returns((AlternateName)default);

            controller.AlternateNameRepository.Setup(
                repo => repo.Add(It.IsAny<AlternateName>())
            );

            // Act 
            var response = controller.Create(newAlternateName);
            var result = response.Result as ObjectResult;
            var body = result.StatusCode;

            // Assert 
            Assert.Equal(StatusCodes.Status404NotFound, body);

        }

        [Fact]
        public void  Create_CreateWithoutName_Status400() {
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
            var response = controller.Create(newAlternateName);
            var result = response.Result as ObjectResult;
            var body = result.StatusCode;

            // Assert 
            Assert.Equal(StatusCodes.Status400BadRequest, body); 
        }

        [Fact]
        public void Create_CreateAlreadyExists_Status409(){
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
            var response = controller.Create(newAlternateName);
            var result = response.Result as ObjectResult;
            var body = result.StatusCode;

            // Assert 
            Assert.Equal(StatusCodes.Status409Conflict, body);
        }

        [Fact]
        public void Create_CreateWithoutDTO_Status400() {
            // Arrange
            var controller = new MockedAlternateNameController();
            AlternateNameDTO newAlternateName = null;
        

            // Mock 

            // Act 
            var response = controller.Create(newAlternateName);
            var result = response.Result as ObjectResult;
            var body = result.StatusCode;

            // Assert 
            Assert.Equal(StatusCodes.Status400BadRequest, body);
        }

        #endregion

        #region Get
        
        [Fact]
        public void Get_WithUserPictogram_Status200() {
            // Arrange
            var controller = new MockedAlternateNameController();
            var pictogram = new Pictogram() {
                Id = 42069
            };
             var user = new GirafUser() {
                Id = "Danielsan"
            };
            var newAlternateName = new AlternateName(user, pictogram, "Tommysan");

            // Mock 
             controller.GirafUserRepository.Setup(
                repo => repo.GetByID(newAlternateName.CitizenId)
            ).Returns(user);
            controller.PictogramRepository.Setup(
                repo => repo.GetByID(newAlternateName.PictogramId)
            ).Returns(pictogram);
            controller.AlternateNameRepository.Setup(
                repo => repo.GetForUser(user.Id, pictogram.Id)
            ).Returns(newAlternateName);

            // Act 
            var result = controller.Get(
                    newAlternateName.CitizenId, newAlternateName.PictogramId) 
                    as ObjectResult;
            var statusCode = result.StatusCode;
            var body = result.Value as SuccessResponse<AlternateNameDTO>;

            // Assert 
            Assert.Equal(StatusCodes.Status200OK, statusCode);
            Assert.Equal(newAlternateName.Name, body.Data.Name);
            Assert.Equal(user.Id, body.Data.Citizen);
            Assert.Equal(pictogram.Id, body.Data.Pictogram);
        }

        [Fact]
        public void Get_WithNoUser_Status404()
        {
            // Arrange
            var controller = new MockedAlternateNameController();
            var pictogram = new Pictogram() {
                Id = 42069
            };
             var user = new GirafUser() {
                Id = "Danielsan"
            };
            var newAlternateName = new AlternateName(user, pictogram, "Tommysan");

            // Mock 
            controller.PictogramRepository.Setup(
                repo => repo.GetByID(newAlternateName.PictogramId)
            ).Returns(pictogram);
            controller.AlternateNameRepository.Setup(
                repo => repo.GetForUser(user.Id, pictogram.Id)
            ).Returns(newAlternateName);

            // Act 
            var result = controller.Get(
                    "", newAlternateName.PictogramId) 
                    as ObjectResult;
            var statusCode = result.StatusCode;

            // Assert 
            Assert.Equal(StatusCodes.Status404NotFound, statusCode);

        }



        [Fact]
        public void Get_MissingPictogram_Status404() {
            // Arrange
            var controller = new MockedAlternateNameController();
            var alternateName = new AlternateNameDTO() {
                Name = "hywmongous",
                Citizen = "Brandhoej",
                Pictogram = 420,
            };
            var user = new GirafUser() {
                Id = alternateName.Citizen,
            };

            // Mock
            controller.GirafUserRepository.Setup(
                repo => repo.GetByID(user.Id)
            ).Returns(user);
            controller.PictogramRepository.Setup(
                repo => repo.GetByID(alternateName.Pictogram)
            ).Returns((Pictogram)default);

            // Act
            var result = controller.Get(user.Id, alternateName.Pictogram) as ObjectResult;

            // Assert
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        }

        [Fact]
        public void Get_MissingAlternateName_Status404() {
            // Arrange
            var controller = new MockedAlternateNameController();
            var alternateName = new AlternateNameDTO() {
                Name = "hywmongous",
                Citizen = "Brandhoej",
                Pictogram = 420,
            };
            var user = new GirafUser() {
                Id = alternateName.Citizen,
            };
            var pictogram = new Pictogram() {
                Id = alternateName.Pictogram,
            };

            // Mock
            controller.GirafUserRepository.Setup(
                repo => repo.GetByID(user.Id)
            ).Returns(user);
            controller.PictogramRepository.Setup(
                repo => repo.GetByID(pictogram.Id)
            ).Returns(pictogram);

            // Act
            var result = controller.Get(user.Id, pictogram.Id) as ObjectResult;

            // Assert
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        }

        #endregion

        #region Edit
        [Fact]
        public void Edit_CorrectRequest_Status200() {
            // Arrange
            var controller = new MockedAlternateNameController();
            var newAlternateNameDTO = new AlternateNameDTO() {
                Name = "hywmongous",
                Citizen = "Brandhoej",
                Pictogram = 420,
            };
            var user = new GirafUser() {
                Id = newAlternateNameDTO.Citizen,
            };
            var pictogram = new Pictogram() {
                Id = newAlternateNameDTO.Pictogram,
            };
            var oldAlternateName = new AlternateName() {
                Id = 80085,
                // The following: Equals from pictogram and user (respectively)
                PictogramId = pictogram.Id,
                CitizenId = user.Id,
            };

            // Mock
            controller.GirafUserRepository.Setup(
                repo => repo.Get(newAlternateNameDTO.Citizen)
            ).Returns(user);
            controller.PictogramRepository.Setup(
                repo => repo.GetByID(newAlternateNameDTO.Pictogram)
            ).Returns(pictogram);
            controller.AlternateNameRepository.Setup(
                repo => repo.Get(oldAlternateName.Id)
            ).Returns(oldAlternateName);
            controller.AlternateNameRepository.Setup(
                repo => repo.UserAlreadyHas(newAlternateNameDTO.Citizen, oldAlternateName.Id)
            ).Returns(false);
            controller.AlternateNameRepository.Setup(
                repo => repo.Add(It.IsAny<AlternateName>())
            );

            // Act
            var response = controller.Edit(oldAlternateName.Id, newAlternateNameDTO);
            var result = response.Result as ObjectResult;
            var body = result.Value as SuccessResponse<AlternateNameDTO>;

            // Assert
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public void Edit_MissingNameProperty_Status400() {
            // Arrange
            var controller = new MockedAlternateNameController();
            var newAlternateNameDTO = new AlternateNameDTO() {
                Citizen = "Brandhoej",
                Pictogram = 420,
            };
            var oldAlternateName = new AlternateName() {
                Id = 80085,
                // Different from newAlternateNameDTO
                PictogramId = 123,
            };

            // Act
            var response = controller.Edit(oldAlternateName.Id, newAlternateNameDTO);
            var result = response.Result as ObjectResult;

            // Assert
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }

        [Fact]
        public void Edit_UserNotFound_Status404() {
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

            // Mock
            controller.GirafUserRepository.Setup(
                repo => repo.Get(newAlternateNameDTO.Citizen)
            ).Returns((GirafUser)default);

            // Act
            var response = controller.Edit(oldAlternateName.Id, newAlternateNameDTO);
            var result = response.Result as ObjectResult;

            // Assert
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        }

        [Fact]
        public void Edit_PictogramNotFound_Status404() {
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
            ).Returns(oldAlternateName);
            controller.PictogramRepository.Setup(
                repo => repo.Get(newAlternateNameDTO.Pictogram)
            ).Returns((Pictogram)default);

            // Act
            var response = controller.Edit(oldAlternateName.Id, newAlternateNameDTO);
            var result = response.Result as ObjectResult;

            // Assert
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        }

        [Fact]
        public void Edit_AlternateNameMismatch_Status404() {
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

            // Act
            var response = controller.Edit(oldAlternateName.Id + 1, newAlternateNameDTO);
            var result = response.Result as ObjectResult;

            // Assert
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        }

        [Fact]
        public void Edit_WithoutDTO_Status400() {
            // Arrange
            var controller = new MockedAlternateNameController();
            AlternateNameDTO newAlternateNameDTO = default;
            var oldAlternateName = new AlternateName() {
                Id = 80085,
            };

            // Act
            var response = controller.Edit(oldAlternateName.Id, newAlternateNameDTO);
            var result = response.Result as ObjectResult;
            var body = result.Value as SuccessResponse<AlternateNameDTO>;

            // Assert
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }

        [Fact]
        public void Edit_AlternateNameNotFound_Status404() {
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
            var response = controller.Edit(oldAlternateName.Id, newAlternateNameDTO);
            var result = response.Result as ObjectResult;

            // Assert
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        }

        #endregion
    }
}