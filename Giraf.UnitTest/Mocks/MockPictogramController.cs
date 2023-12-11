using GirafAPI.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GirafEntities.User;
using GirafEntities.WeekPlanner;
using GirafRepositories.Interfaces;
using GirafServices.User;
using GirafServices.WeekPlanner;

namespace Giraf.UnitTest.Mocks
{
    public class MockedPictogramController : PictogramController
    {
        public MockedPictogramController() : this(
            new Mock<IUserService>(),
            new Mock<IHostEnvironment>(),
            new Mock<IPictogramRepository>(),
            new Mock<IImageService>(),
            new Mock<IPictogramService>())
        {}

        public MockedPictogramController(
        Mock<IUserService> userService,
        Mock<IHostEnvironment> env,
        Mock<IPictogramRepository> pictogramRepository,
        Mock<IImageService> imageService,
        Mock<IPictogramService> pictogramService
        ) : base(
        userService.Object,
        env.Object,
        pictogramRepository.Object,
        imageService.Object,
        pictogramService.Object)
        {

            UserService = userService;
            ImageService = imageService;
            PictogramRepository = pictogramRepository;
            PictogramService = pictogramService;
            testPictogram = new Pictogram("testPictogram", AccessLevel.PUBLIC);
            testUser = new GirafUser("bob", "Bob", new Department(), GirafRoles.Citizen);
            guardianUser = new GirafUser("guard", "Guard", new Department(), GirafRoles.Guardian);
            IList<string> guardRoles = new List<string>() { "Guardian" };
            IList<string> roles = new List<string>() { "Citizen" };
            IList<Pictogram> pictograms = new List<Pictogram>() { testPictogram };
            

            pictogramRepository.Setup(repo => repo.AddPictogramWith_NO_ImageHash("testPictogram", AccessLevel.PUBLIC));
            pictogramRepository.Setup(repo => repo.fetchPictogramsUserNotPartOfDepartmentContainsQuery("testPictogram", guardianUser)).Returns(pictograms);
            
            
        }

        public Mock<IUserService> UserService { get; }
        public Mock<IImageService> ImageService { get; }

        public Mock<IPictogramService> PictogramService { get; set; }
        public Mock<IPictogramRepository> PictogramRepository { get; }

        public GirafUser testUser { get; }

        public GirafUser guardianUser { get; }

        public Pictogram testPictogram { get; }
    }
}
