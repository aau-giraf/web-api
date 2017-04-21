using System.Collections.Generic;
using GirafRest.Data;
using GirafRest.Models;
using GirafRest.Test.Mocks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

namespace GirafRest.Test.Controllers
{
    public class GirafControllerTest
    {
        private readonly Mock<GirafDbContext> dbContextMock;
        private readonly Mock<IUserStore<GirafUser>> userStore;
        private readonly MockUserManager umMock;
        private readonly Mock<ILoggerFactory> lfMock;
        private readonly List<string> logs;
        private readonly Mock<MockDbContext> dbMock;
    }
}