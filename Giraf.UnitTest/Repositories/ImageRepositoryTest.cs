using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using GirafRepositories.Persistence;
using GirafRepositories.WeekPlanner;
using GirafServices.WeekPlanner;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Giraf.UnitTest.Repositories
{
    public class FakeImageRepositoryContext
    {

        protected FakeImageRepositoryContext(DbContextOptions<GirafDbContext> contextOptions)
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



                context.AddRange();



                context.SaveChanges();

            }

        }

    }

    public class ImageRepositoryTest : FakeImageRepositoryContext
    {
        public ImageRepositoryTest()
            : base(new DbContextOptionsBuilder<GirafDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N")).Options)
        {
        }

        [Fact]
        public async Task ReadRequestImage()
        {
            using (var context = new GirafDbContext(ContextOptions))
            {
                using (var test_Stream = new MemoryStream(Encoding.UTF8.GetBytes("whatever")))
                {
                    //Arrange
                    var repository = new ImageRepository(context);
                    var imageService = new ImageService();
                    byte[] image = Encoding.UTF8.GetBytes("whatever");

                    //Act
                    var result = await imageService.ReadRequestImage(test_Stream);

                    //Assert
                    Assert.Equal(image, result);
                }
            }
        }

    }
}