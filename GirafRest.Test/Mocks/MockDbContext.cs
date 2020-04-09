using GirafRest.Data;
using Microsoft.EntityFrameworkCore;

namespace GirafRest.Test.Mocks
{
    public class MockDbContext : GirafDbContext
    {
        public MockDbContext() : base () {}
    }
}