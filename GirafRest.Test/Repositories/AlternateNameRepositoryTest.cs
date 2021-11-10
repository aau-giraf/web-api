using Xunit;
using System;
using System.Linq;
using GirafRest.Data;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using GirafRest.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace GirafRest.Test.Repositories
{
    public class AlternameNameRepositoryContext
    {
        protected AlternameNameRepositoryContext(DbContextOptions<GirafDbContext> contextOptions)
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
                var alternateName = new AlternateName()
                {
                    CitizenId = "1",
                    PictogramId = 1,
                    Name = "alterName"
                };

                context.Add(alternateName);

                context.SaveChanges();
            }
        }
    }

    public class AlternameNameRepositoryTest : AlternameNameRepositoryContext
    {
        private const string UserId = "1";
        private const string AlternateName = "alterName";
        private const long PicId = 1;
        public AlternameNameRepositoryTest()
            : base(new DbContextOptionsBuilder<GirafDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N")).Options)
        { }

        [Theory]
        [InlineData(UserId, PicId)]
        public void GetForUserTest(string userId, long picId)
        {
            //Arrange
            using (var context = new GirafDbContext(ContextOptions))
            {
                var repository = new AlternateNameRepository(context);

                //Act
                var alternateName = repository.GetForUser(userId, picId).Name;

                //Assert
                Assert.Equal(AlternateName, alternateName);
            }
        }

        [Fact]
        public void GetForUser_PictogramHasAlternateNameForUser_ReturnsAlternateName()
        {
            // Arrange
            var context = new Mock<GirafDbContext>();
            var dbSet = new Mock<DbSet<AlternateName>>();
            var expected = new AlternateName()
            {
                CitizenId = "1",
                PictogramId = 1,
                Name = "alterName"
            };
            var repository = new AlternateNameRepository(context.Object);

            // Mock
            context.Setup(x => x.Set<AlternateName>()).Returns(dbSet.Object);
            dbSet.Setup(x => x.Find(expected.PictogramId)).Returns(expected);

            // Act
            var actual = repository.GetForUser(expected.CitizenId, expected.PictogramId);

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
