using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GirafRest.Data;
using GirafRest.Models;
using GirafRest.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GirafRest.Test.Repositories
{

    public class FakeUserResourceRepositoryContext
    {
        protected FakeUserResourceRepositoryContext(DbContextOptions<GirafDbContext> contextOptions)
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
                    AccessLevel = AccessLevel.PUBLIC

                };
                pictogram.Id = 23;
                var user = new GirafUser()
                {
                    UserName = "John",
                    DisplayName = "UsernameDisplay",
                    Id = "1",
                    DepartmentKey = 2

                };
                var userResource = new UserResource(user, pictogram)
                {
                    
                };
                var user2 = new GirafUser();
                user2.Id = "2";
                user2.UserName = "John Appleseed";
                
                var newPictogram = new Pictogram()
                {
                    Title = "ABC",
                    AccessLevel = AccessLevel.PUBLIC

                };
                pictogram.Id = 123;



                context.AddRange(user, pictogram, userResource, user2, newPictogram);



                context.SaveChanges();

            }

        }
    }

    public class UserResourceRepositoryTest : FakeUserResourceRepositoryContext
    {
        
        public UserResourceRepositoryTest()
            : base(new DbContextOptionsBuilder<GirafDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N")).Options)
        {
        }

        [Fact]
        public async Task AddAsync()
        {
            using (var context = new GirafDbContext(ContextOptions))
            {
                var repository = new UserResourseRepository(context);
                
                var user = context.Users.FirstOrDefault(u => u.Id == "1");
                var pictogram = context.Pictograms.FirstOrDefault((u => u.Title == "Unicorn"));
                
                var userResource = new UserResource(user, pictogram);
                
                var result = await repository.AddAsync(userResource);
                
                
                var findUserResource = context.UserResources.FirstOrDefault(u => u.Pictogram.Title == "Unicorn");

                
                Assert.NotNull(findUserResource);

            }
        }

        [Fact]
        public async Task FetchRelationshipFromDb()
        {
            using (var context = new GirafDbContext(ContextOptions))
            {
                var repository = new UserResourseRepository(context);
                var user = context.Users.FirstOrDefault(u => u.Id == "1");
                var user2 = context.Users.FirstOrDefault(u => u.Id == "2");
                var pictogram = context.Pictograms.FirstOrDefault((u => u.Title == "Unicorn"));
                var userResource = await repository.FetchRelationshipFromDb(pictogram, user);
                
                Assert.Equal("Unicorn", userResource.Pictogram.Title);
                Assert.Equal("John",userResource.Other.UserName);
                
                var userResource1 = await repository.FetchRelationshipFromDb(pictogram, user2);
                
                Assert.Null(userResource1);

            }
        }
        

        [Fact]
        public void Remove()
        {
            using (var context = new GirafDbContext(ContextOptions))
            {
                var repository = new UserResourseRepository(context);
                var user = context.Users.FirstOrDefault(u => u.Id == "2");
                var pictogram = context.Pictograms.FirstOrDefault((u => u.Title == "ABC"));
                var userResource = new UserResource(user, pictogram);
                repository.Add(userResource);
                context.SaveChanges();
                Assert.NotNull(context.UserResources.FirstOrDefault(u => u.Pictogram.Title == "ABC"));
                repository.Remove(userResource);
                context.SaveChanges();
                var findUserResource = context.UserResources.FirstOrDefault(u => u.Pictogram.Title == "ABC");
                Assert.Null(findUserResource);
            }
        }
        
    }
}
