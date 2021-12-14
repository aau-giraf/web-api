using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GirafRest.Data;
using GirafRest.Models;
using GirafRest.Models.Enums;
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
                    Resources = new List<DepartmentResource>(),
                    Key = 1
                };
                var pictogram = new Pictogram()
                {
                    Title = "Bubblegum",
                    AccessLevel = AccessLevel.PUBLIC,
                    
                };
                var dep = new Department()
                {
                    Key = 1
                };
                var user = new GirafUser("Giraffe", "giraffe", department, GirafRoles.Citizen);
                var departmentResource = new DepartmentResource(department, pictogram);
                departmentResource.PictogramKey = 888;
                departmentResource.PictogramKey = 1;
                pictogram.Id = 888;
                
               
                
                


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
        
    }
}