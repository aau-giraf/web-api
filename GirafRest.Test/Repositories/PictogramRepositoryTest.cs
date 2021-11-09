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
    public class PictogramRepositoryContext
    {
        protected PictogramRepositoryContext(DbContextOptions<GirafDbContext> contextOptions)
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
                var user = new Pictogram()
                {
                    Id = 1,
                    Title = "pic1"
                };

                context.Add(user);

                context.SaveChanges();
            }
        }
    }

    public class PictogramRepositoryTest : PictogramRepositoryContext
    {
        private const int PicId = 1;
        private const string PicTitle = "pic1";
        public PictogramRepositoryTest()
            : base(new DbContextOptionsBuilder<GirafDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N")).Options)
        { }

        [Theory]
        [InlineData(PicId)]
        public void GetByIDTest(int id)
        {
            //Arrange
            using (var context = new GirafDbContext(ContextOptions))
            {
                var repository = new PictogramRepository(context);

                //Act
                var picTitle = repository.GetByID(id).Title;

                //Assert
                Assert.Equal(PicTitle, picTitle);
            }
        }
    }
}
