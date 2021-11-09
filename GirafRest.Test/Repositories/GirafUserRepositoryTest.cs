using Xunit;
using System;
using System.Linq;
using GirafRest.Data;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using GirafRest.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GirafRest.Test.Repositories
{
    public class GirafUserRepositoryContext
    {
        protected GirafUserRepositoryContext(DbContextOptions<GirafDbContext> contextOptions)
        {
            ContextOptions = contextOptions;

            Seed();
        }

        protected DbContextOptions<GirafDbContext> ContextOptions { get; }

        public void Seed()
        {
            using (var context = new GirafDbContext(ContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                //make new entities here
                var user = new GirafUser()
                {
                    Id = "1",
                    UserName = "user1"
                };

                context.Add(user);

                context.SaveChanges();
            }
        }
    }

    public class GirafUserRepositoryTest : GirafUserRepositoryContext
    {
        private const string Id = "1";
        public GirafUserRepositoryTest()
            : base(new DbContextOptionsBuilder<GirafDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N")).Options)
        { }

        [Theory]
        [InlineData(Id)]
        public void GetWithWeekSchedulesTest(string id)
        {
            //Arrange
            using (var context = new GirafDbContext(ContextOptions))
            {
                var repository = new GirafUserRepository(context);

                //Act
                var username = repository.GetWithWeekSchedules(id).UserName;

                //Assert
                Assert.Equal("user1", username);
            }
        }
    }
}
