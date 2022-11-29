using System.Linq;
using Xunit;
using GirafRest.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using GirafRest.Controllers;
using Xunit.Abstractions;
using GirafRest.Test.Mocks;
using static GirafRest.Test.UnitTestExtensions;
using GirafRest.Models.DTOs;
using System.IO;
using GirafRest.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting.Internal;
using Castle.Core.Logging;
using Moq;
using GirafRest.Interfaces;
using System.Threading.Tasks;
using System.Security.Claims;


namespace GirafRest.Test
{
    public class PictogramControllerTest
    {


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