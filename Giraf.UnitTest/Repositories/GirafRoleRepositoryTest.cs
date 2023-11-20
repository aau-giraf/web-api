using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using GirafEntities.User;
using GirafRepositories.Persistence;
using GirafRepositories.User;
using GirafRest.Data;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using GirafRest.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Giraf.UnitTest.Repositories
{
    public class FakeGirafRoleRepositoryContext
    {
        protected FakeGirafRoleRepositoryContext(DbContextOptions<GirafDbContext> contextOptions)
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
                
                // add entities here
                var user = new GirafUser()
                {
                    UserName = "John",
                    DisplayName = "UsernameDisplay",
                    Id = "1",
                    DepartmentKey = 2,
                };
                var guardianRole = new GirafRole()
                {
                    Id = "Guardian",
                    Name = "Guardian"
                };
                var citizenRole = new GirafRole()
                {
                    Id = "Citizen",
                    Name = "Citizen"
                };
                
                context.AddRange(user,guardianRole,citizenRole);
                context.SaveChanges();
            }
        }
    }

    public class GirafRoleRepositoryTest : FakeGirafRoleRepositoryContext
    {
        public GirafRoleRepositoryTest()
            : base(new DbContextOptionsBuilder<GirafDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N")).Options)
        {
        }


        [Fact]
        public void GetGuardianRoleId_Test()
        {
            // Arrange
            using (var context = new GirafDbContext(ContextOptions))
            {
                var repository = new GirafRoleRepository(context);

                // Act
                var result = repository.GetGuardianRoleId();

                // Assert
                Assert.Equal(GirafRole.Guardian, result);
            }

        }

        [Fact]
        public void GetCitizenRoleId_Test()
        {
            // Arrange
            using (var context = new GirafDbContext(ContextOptions))
            {
                var repository = new GirafRoleRepository(context);

                // Act
                var result = repository.GetCitizenRoleId();

                // Assert
                Assert.Equal(GirafRole.Citizen, result);
            }
        }
        
        [Fact]
        public void GetAllCitizens_Test()
        {
            // Arrange
            using (var context = new GirafDbContext(ContextOptions))
            {
                var repository = new GirafRoleRepository(context);

                // Act
                var result = repository.GetAllCitizens();

                // Assert
                
            }
        }
        
        
        
    }
}