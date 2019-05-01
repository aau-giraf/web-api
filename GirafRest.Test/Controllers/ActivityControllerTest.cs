using System.Collections.Generic;
using System.IO;
using System.Linq;
using GirafRest.Controllers;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using GirafRest.Models.Responses;
using GirafRest.Services;
using GirafRest.Test.Mocks;
using Moq;
using Xunit;
using Xunit.Abstractions;
using static GirafRest.Test.UnitTestExtensions;

namespace GirafRest.Test.Controllers
{
    class ActivityControllerTest
    {
        private TestContext _testContext;

        private ActivityController initializeTest()
        {
            _testContext = new TestContext();

            var wc = new ActivityController(
                new MockGirafService(_testContext.MockDbContext.Object,
                                     _testContext.MockUserManager), _testContext.MockLoggerFactory.Object,
                new GirafAuthenticationService(_testContext.MockDbContext.Object, _testContext.MockRoleManager.Object,
                                               _testContext.MockUserManager));
            _testContext.MockHttpContext = wc.MockHttpContext();

            return wc;
        }
    }
}
