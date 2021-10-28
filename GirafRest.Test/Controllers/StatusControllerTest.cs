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

            var stc = new StatusController(
                 new MockGirafService(
                     _testContext.MockDbContext.Object,
                     _testContext.MockUserManager
                     ),
                 _testContext.MockLoggerFactory.Object,
                 new StatusControllerRepository(new MockGirafService(
                     _testContext.MockDbContext.Object,
                     _testContext.MockUserManager
                     ), _testContext.MockDbContext.Object) as IStatusControllerRepository
             );


            _testContext.MockHttpContext = stc.MockHttpContext();

            return stc;
        }

        [Fact]
        public void CheckUsersDB_Successful_Connection()
        {
            /*
            var stc = InitializeTest();
            var res = stc.DatabaseStatus().Result as ObjectResult;
            var body = res.Value as ErrorResponse;
             
            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
             */
        }

        [Fact]
        public void CheckUsersDB_Failed_Connection()
        {
            var stc = InitializeTest();
            var res = _testContext.MockDbContext.Object.Database.CanConnect();
            var res = _testContext.MockDbContext.Object.Database = new Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade();
            /*
            var res = stc.DatabaseStatus().Result as ObjectResult;
             
            var body = res.Value as ErrorResponse;
            Assert.Equal("Error when connecting to database", body.Message);
            Assert.Equal(ErrorCode.Error, body.ErrorCode);
            Assert.Equal(StatusCodes.Status503ServiceUnavailable, res.StatusCode);
             */

        }
    }
}
