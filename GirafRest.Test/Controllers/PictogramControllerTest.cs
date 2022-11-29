using System.Linq;
using Xunit;
using GirafRest.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using GirafRest.Controllers;

using GirafRest.Models.DTOs;
using GirafRest.Models.Responses;
using Microsoft.AspNetCore.Http;

using Moq;
using GirafRest.Interfaces;
using System.Threading.Tasks;

using GirafRest.IRepositories;

using GirafRest.Models.Enums;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GirafRest.Test
{
    public class PictogramControllerTest
    {

        public class MockedPictogramController : PictogramController
        {
            public MockedPictogramController() : this(
                new Mock<IGirafService>(),
                new Mock<IHostEnvironment>(),
                new Mock<ILoggerFactory>(),
                new Mock<IGirafUserRepository>(),
                new Mock<IPictogramRepository>())
            { }

            public MockedPictogramController(
            Mock<IGirafService> giraf,
            Mock<IHostEnvironment> env,
            Mock<ILoggerFactory> lFactory,
            Mock<IGirafUserRepository> girafUserRepository,
            Mock<IPictogramRepository> pictogramRepository
            ) : base(
            giraf.Object,
            env.Object,
            lFactory.Object,
            girafUserRepository.Object,
            pictogramRepository.Object)
            {

                GirafService = giraf;
                LoggerFactory = lFactory;
                GirafUserRepository = girafUserRepository;
                PictogramRepository = pictogramRepository;

                testPictogram = new Pictogram("testPictogram", AccessLevel.PUBLIC);
                testUser = new GirafUser("bob", "Bob", new Department(), GirafRoles.Citizen);
                guardianUser = new GirafUser("guard", "Guard", new Department(), GirafRoles.Guardian);
                IList<string> guardRoles = new List<string>() { "Guardian" };
                IList<string> roles = new List<string>() { "Citizen" };
                IList<Pictogram> pictograms = new List<Pictogram>() { testPictogram };


                pictogramRepository.Setup(repo => repo.AddPictogramWith_NO_ImageHash("testPictogram", AccessLevel.PUBLIC));
                pictogramRepository.Setup(repo => repo.fetchPictogramsUserNotPartOfDepartmentContainsQuery("testPictogram", guardianUser)).Returns(pictograms);


            }

            public Mock<IGirafService> GirafService { get; }
            public Mock<ILoggerFactory> LoggerFactory { get; }
            public Mock<IGirafUserRepository> GirafUserRepository { get; }
            public Mock<IImageRepository> ImageRepository { get; }
            public Mock<IUserResourseRepository> UserResourseRepository { get; }
            public Mock<IPictogramRepository> PictogramRepository { get; }

            public GirafUser testUser { get; }

            public GirafUser guardianUser { get; }

            public Pictogram testPictogram { get; }
        }
        [Fact]
        
        public async Task ReadPictogram_Success()
        {

        }
        [Fact]
        public async Task ReadPictogram_Fail()
        {

        }

        [Fact]
        public async Task CreatePictogram_Success()
        {

        }
        [Fact]
        public async Task CreatePictogram_Fail()
        {

        }
        [Fact]
        public async Task UpdatePictogramInfo_Success()
        {

        }
        [Fact]
        public async Task UpdatePictogramInfo_Fail()
        {

        }
        [Fact]
        public async Task DeletePictogram_Success()
        {

        }
        [Fact]
        public async Task DeletePictogram_Fail()
        {

        }
        [Fact]
        public async Task SetPictogramImage_Success()
        {

        }
        [Fact]
        public async Task SetPictogramImage_Fail()
        {

        }
        [Fact]
        public async Task ReadPictogramImage_Success()
        {

        }
        [Fact]
        public async Task ReadPictogramImage_Fail()
        {

        }
        [Fact]
        public async Task ReadRawpictogram_Success()
        {

        }
        [Fact]
        public async Task ReadRawPictogram_Fail()
        {

        }
        [Fact]
        public async Task CheckOwnership_Success()
        {

        }
        [Fact]
        public async Task CheckOwnership_Fail()
        {

        }
        [Fact]
        public async Task ReadAllPictograms_Success()
        {
            var pictogramController = new MockedPictogramController();
            var userController = new MockedUserController();
            var repository = userController.GirafUserRepository;
            var pictogramRepository = pictogramController.PictogramRepository;
            var giraf = pictogramController.GirafService;
            var pictogramController1 = new Mock<PictogramController>();


            var pictogram1 = new Pictogram() { Id = 1 };
            var pictogram2 = new Pictogram() { Id = 2 };
            var pictogram3 = new Pictogram() { Id = 3 };
            var pictograms = new List<Pictogram>();
            pictograms.Add(pictogram1);
            pictograms.Add(pictogram2);
            pictograms.Add(pictogram3);


            pictogramController1.Setup(x =>
            x.ReadAllPictograms(It.IsAny<string>())).Returns(Task.FromResult<IEnumerable<Pictogram>>(pictograms));


            //Mock user
            var user = pictogramController.guardianUser;
            var response = await pictogramController.ReadAllPictograms("");
            var result = response as ObjectResult;
            var body = result.Value as SuccessResponse<PictogramDTO>;



            Assert.Equal(StatusCodes.Status200OK, result.StatusCode.Value);
        }
        [Fact]
        public async Task ReadAllPictograms_Fail()
        {
            var pictogramController = new MockedPictogramController();
        }

    }
    }
