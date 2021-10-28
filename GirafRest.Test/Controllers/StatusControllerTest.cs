using System.Linq;
using Xunit;
using GirafRest.Models;
using GirafRest.Controllers;
using System.Collections.Generic;
using GirafRest.Test.Mocks;
using static GirafRest.Test.UnitTestExtensions;
using GirafRest.Models.DTOs;
using Xunit.Abstractions;
using GirafRest.Models.Responses;
using GirafRest.Services;
using Microsoft.AspNetCore.Http;
using static GirafRest.Test.UnitTestExtensions.TestContext;
using Microsoft.AspNetCore.Mvc;
using GirafRest.Repositories;
using GirafRest.IRepositories;
using GirafRest.Interfaces;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading;

namespace GirafRest.Test
{
    public class StatusControllerTest
    {

        private readonly ITestOutputHelper _testLogger;
        TestContext _testContext;
        public StatusControllerTest(ITestOutputHelper output)
        {
            _testLogger = output;
        }
        private StatusController InitializeTest()
        {
            _testContext = new TestContext();
            var mockGirafService = new Mock<GirafService>(_testContext.MockDbContext.Object, _testContext.MockUserManager);
            var mockRepository = new Mock<StatusControllerRepository>(mockGirafService.Object, _testContext.MockDbContext.Object);
            mockRepository.Setup(c => c.CheckDbConnectionAsync()).Returns(Task.FromResult(true));


            var stc = new Mock<StatusController>(
                 mockGirafService.Object,
                 _testContext.MockLoggerFactory.Object,
                 mockRepository as IStatusControllerRepository
             );

            

            _testContext.MockHttpContext = stc.Object.MockHttpContext();
            
            return stc.Object;
        }

        [Fact]
        public async void CheckUsersDB_Successful_Connection()
        {
            
            var _girafService = new Mock<IGirafService>(_dbContext);
            var _logger = new LoggerFactory();
            var _statusControllerRepository = new Mock<StatusControllerRepository>(_girafService, _dbContext);
            var statusController = new MockStatusController(_girafService.Object, _logger, _statusControllerRepository.Object);
            var res = await statusController.DatabaseStatus() as ObjectResult;
            //var body = res.Value as ErrorResponse;
             
            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
        }

        [Fact]
        public void CheckUsersDB_Failed_Connection()
        {
            var stc = InitializeTest();
            var res = stc.DatabaseStatus().Result as ObjectResult;
            var body = res.Value as ErrorResponse;
            Assert.Equal("Error when connecting to database", body.Message);
            Assert.Equal(ErrorCode.Error, body.ErrorCode);
            Assert.Equal(StatusCodes.Status503ServiceUnavailable, res.StatusCode);

        }
    }

    internal class MockStatusController : StatusController
    {
        private Mock<IStatusControllerRepository> _statusControllerRepository;
        private Mock<IGirafService> _girafService;
        private Mock<ILoggerFactory> _loggerFactory;

        public Mock<ILoggerFactory> LoggerFactory { get { return _loggerFactory; } set { _loggerFactory = value; } }
        public Mock<IGirafService> GirafService { get {return _girafService; } set { _girafService = value; } }
        public Mock<IStatusControllerRepository> StatusControllerRepository{ get { return _statusControllerRepository; } set { _statusControllerRepository = value;} }
        public MockStatusController(IGirafService girafService, ILoggerFactory logger, IStatusControllerRepository statusControllerRepository):base(girafService, logger, statusControllerRepository)
        {

        }
        public MockStatusController(Mock<IGirafService> girafService, Mock<ILoggerFactory> logger, Mock<IStatusControllerRepository> statusControllerRepository) : base(girafService.Object, logger.Object, statusControllerRepository.Object)
        {
            _girafService = girafService;
            _loggerFactory = logger;
            _statusControllerRepository = statusControllerRepository;
        }

    }
    
}
