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
    public class PictogramRelationRepositoryContext
    {
        protected PictogramRelationRepositoryContext(DbContextOptions<GirafDbContext> contextOptions)
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
                var pictogramRelation = new PictogramRelation()
                {
                    Pictogram = new Pictogram()
                    {
                        Id = 1,
                        Title = "pictogram1"
                    },
                    ActivityId = 1
                };

                context.Add(pictogramRelation);

                context.SaveChanges();
            }
        }
    }

    public class PictogramRelationRepositoryTest : PictogramRelationRepositoryContext
    {
        private const long Id = 1;
        public PictogramRelationRepositoryTest()
            : base(new DbContextOptionsBuilder<GirafDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N")).Options)
        { }

        [Theory]
        [InlineData(Id)]
        public void GetWithPictogramTest(long id)
        {
            //Arrange
            using (var context = new GirafDbContext(ContextOptions))
            {
                var repository = new PictogramRelationRepository(context);

                //Act
                var pictogramTitle = repository.GetWithPictogram(id).First().Pictogram.Title;

                //Assert
                Assert.Equal("pictogram1", pictogramTitle);
            }
        }
    }
}
