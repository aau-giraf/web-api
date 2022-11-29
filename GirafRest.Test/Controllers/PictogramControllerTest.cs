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
                this.ControllerContext = new ControllerContext();
                this.ControllerContext.HttpContext = new DefaultHttpContext();

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
            //arranging

            //here we setup the different stuff we need
            var pictogramController = new MockedPictogramController();
            var userController = new MockedUserController();
            var repository = userController.GirafUserRepository;
            var pictogramRepository = pictogramController.PictogramRepository;
            var giraf = pictogramController.GirafService;
            var HttpContext = pictogramController.ControllerContext.HttpContext;
            var logger = new Mock<ILogger>();

            //here we create the data we need to return from our functions in the function
            //these are also our expected pictograms
            var expected_pictogram1 = new Pictogram() { Id = 5 };
            var expected_pictogram2 = new Pictogram() { Id = 6 };
            var expected_pictogram3 = new Pictogram() { Id = 7 };
            var expected_pictograms = new List<Pictogram>();
            expected_pictograms.Add(expected_pictogram1);
            expected_pictograms.Add(expected_pictogram1);
            expected_pictograms.Add(expected_pictogram1);

            //here we create the data we need to return from our functions in the function
            var pictogram1 = new Pictogram() { Id = 1 };
            var pictogram2 = new Pictogram() { Id = 2 };
            var pictogram3 = new Pictogram() { Id = 3 };
            var pictograms = new List<Pictogram>();
            pictograms.Add(pictogram1);
            pictograms.Add(pictogram2);
            pictograms.Add(pictogram3);
            var user = new GirafUser()
            {
                UserName = "user1",
                DisplayName = "user1",
                Id = "u1",
                DepartmentKey = 1,
                Department = new Department(),
            };
            //mocking
            //here we moq functions we depend on in the function we are trying to test with the moq framework
            // Dont EVER moq the controller functions them self you are supposed to test them, not give them a predfined output!



            //  the syntax can be hard to understand
            //  but basically says "when this function gets called gets called, return this value.

            giraf.Setup(x => x.LoadUserWithDepartment(HttpContext.User)).Returns(Task.FromResult(user));

            //mocking fetchPictogramsNoUserLoggedIn

            pictogramRepository.Setup(x => x.fetchPictogramsNoUserLoggedInStartsWithQuery(It.IsAny<string>())).Returns(pictograms);
            pictogramRepository.Setup(x => x.fetchPictogramsNoUserLoggedInContainsQuery(It.IsAny<string>())).Returns(pictograms);
            //mocking fetchingPictogramsUserNotInDepartment
            pictogramRepository.Setup(x => x.fetchPictogramsUserNotPartOfDepartmentStartsWithQuery(It.IsAny<string>(), user)).Returns(pictograms);
            pictogramRepository.Setup( x=> x.fetchPictogramsUserNotPartOfDepartmentContainsQuery(It.IsAny<string>(), user)).Returns(pictograms);
            //mocking fetchingPictogramsFromDepartment


            pictogramRepository.Setup(x => x.fetchPictogramsFromDepartmentStartsWithQuery(It.IsAny<string>(), user)).Returns(expected_pictograms);
            pictogramRepository.Setup(x=> x.fetchPictogramsFromDepartmentsContainsQuery(It.IsAny<string>(), user)).Returns(expected_pictograms);


            giraf.Setup(x => x._logger).Returns(logger.Object);
            

            //now we actually test it :D
            //  here we get a list of pictograms_result
            var pictograms_result = await pictogramController.ReadAllPictograms("");

            foreach (var (expected, actual) in expected_pictograms.Zip(pictograms_result, (x, y) => (x, y))) {
              
                Assert.Equal(expected.Id, actual.Id);
            }



        }


        [Fact]
        public async Task ReadAllPictograms_Fail()
        {
            var pictogramController = new MockedPictogramController();
        }

    }
    }
