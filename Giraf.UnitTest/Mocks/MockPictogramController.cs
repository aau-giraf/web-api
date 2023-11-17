using GirafRest.Controllers;
using GirafRest.Interfaces;
using GirafRest.IRepositories;
using GirafRest.Models;
using GirafRest.Models.Enums;
using GirafRest.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giraf.UnitTest.Mocks
{
    public class MockedPictogramController : PictogramController
    {
        public MockedPictogramController() : this(
            new Mock<IGirafService>(),
            new Mock<IHostEnvironment>(),
            new Mock<ILoggerFactory>(),
            new Mock<IGirafUserRepository>(),
            new Mock<IPictogramRepository>())
        {}

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
}
