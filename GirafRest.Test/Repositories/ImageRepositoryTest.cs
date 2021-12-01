using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using GirafRest.Data;
using GirafRest.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GirafRest.Test.Repositories
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
                    byte[] image = Encoding.UTF8.GetBytes("whatever");

                    //Act
                    var result = await repository.ReadRequestImage(test_Stream);

                    //Assert
                    Assert.Equal(image, result);
                }
            }
        }

    }
}