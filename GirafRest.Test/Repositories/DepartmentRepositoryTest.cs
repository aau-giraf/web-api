using Xunit;
using System;
using GirafRest.Data;
using GirafRest.Models;
using GirafRest.Repositories;
using Microsoft.EntityFrameworkCore;



namespace GirafRest.Test.Repositories
{
    public class FakeDepartmentRepositoryContext
    {
        protected FakeDepartmentRepositoryContext(DbContextOptions<GirafDbContext> contextOptions)
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

                var dep = new Department()
                {
                    Key = 1,
                    Name = "dep1",
                };
                
                context.Add(dep);
                context.SaveChanges();

            }
        }
    }
    
    public class DepartmentRepositoryTest : FakeDepartmentRepositoryContext
    {
        private const long DEP_1 = 1;
        private const string DEP1_NAME = "dep1";
        public DepartmentRepositoryTest()
            : base(new DbContextOptionsBuilder<GirafDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N")).Options){}

        [Theory]
        [InlineData(DEP_1)]
        public void GetDepartmentById_Test(long depKey)
        {   // Arrange
            using (var context = new GirafDbContext(ContextOptions))
            {
                var repository = new DepartmentRepository(context);
                
                // Act
                var response = repository.GetDepartmentById(depKey);
                
                // Assert 
                Assert.Equal(depKey,response.Key);
                Assert.Equal(DEP1_NAME,response.Name);
            }
        }
    }
}