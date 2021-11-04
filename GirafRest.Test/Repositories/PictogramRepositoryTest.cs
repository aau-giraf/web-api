using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GirafRest.Data;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using GirafRest.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GirafRest.Test.Repositories
{
    public class FakePictogramRepositoryContext
    {
        protected FakePictogramRepositoryContext(DbContextOptions<GirafDbContext> contextOptions)
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
                var pictogram = new Pictogram()
                {
                    Title = "Unicorn",
                    AccessLevel = AccessLevel.PUBLIC,
                    

                };
                pictogram.Id = 345567;



                context.AddRange(pictogram);



                context.SaveChanges();

            }

        }

    }

    public class PictogramRepositoryTest : FakePictogramRepositoryContext
    {
        public PictogramRepositoryTest()
            : base(new DbContextOptionsBuilder<GirafDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N")).Options)
        {
        }

        [Fact]
        public async Task FindResource()
        {
            using (var context = new GirafDbContext(ContextOptions))
            {
                var repository = new PictogramRepository(context);
                
                var pictogramDb = context.Pictograms.FirstOrDefault(u => u.Title == "Unicorn");
                var expectedPictogramTitle = pictogramDb.Title;
                
                var resourceIdDTO = new ResourceIdDTO()
                {
                    Id = 345567
                };
                
                var resource = await repository.FindResource(resourceIdDTO);
                //var title = resource.Title;
                
                //Assert.NotNull(title);
                Assert.Equal("Unicorn", resource.Title);



            }
        }

    }
}