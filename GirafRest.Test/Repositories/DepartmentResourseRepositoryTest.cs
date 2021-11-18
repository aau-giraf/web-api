using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GirafRest.Data;
using GirafRest.Models;
using GirafRest.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GirafRest.Test.Repositories
{
    public class FakeDepartmentResourceRepositoryContext
    {

        protected FakeDepartmentResourceRepositoryContext(DbContextOptions<GirafDbContext> contextOptions)
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
                var department = new Department()
                {
                    Members = new List<GirafUser>(),
                    Resources = new List<DepartmentResource>()
                };
                var pictogram = new Pictogram()
                {
                    Title = "Bubblegum",
                    AccessLevel = AccessLevel.PUBLIC,
                    
                };
                var user = new GirafUser()
                {
                    UserName = "Giraffe",
                    Id = "1"
                };
                var departmentResource = new DepartmentResource(department, pictogram);
                departmentResource.PictogramKey = 888;
                pictogram.Id = 888; 
                user.Department = new Department();
                user.Department.Key = 0;
                departmentResource.OtherKey = 0;
                


                context.AddRange(department, pictogram, user, departmentResource);



                context.SaveChanges();

            }

        }

    }
    public class DepartmentResourceRepositoryTest : FakeDepartmentResourceRepositoryContext
    {
        public DepartmentResourceRepositoryTest()
            : base(new DbContextOptionsBuilder<GirafDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N")).Options)
        {
        }

        [Fact]
        public async Task CheckProtectedOwnership()
        {
            using (var context = new GirafDbContext(ContextOptions))
            {
                var repository = new DepartmentResourseRepository(context);
                //var departmentResource = await context.DepartmentResources.FirstOrDefaultAsync(u => u.PictogramKey == 888);
                var pictogram = await context.Pictograms.FirstOrDefaultAsync(u => u.Title == "Bubblegum");
                var user = await context.Users.FirstOrDefaultAsync(u => u.Id == "1");

                var resultzeft = await repository.CheckProtectedOwnership(pictogram, user);
                
                Assert.Equal(true, resultzeft);

            }
        }
    }
}