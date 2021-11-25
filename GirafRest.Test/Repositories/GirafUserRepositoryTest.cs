using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using GirafRest.Data;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using GirafRest.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GirafRest.Test.Repositories
{
    public class GirafUserRepositoryContext
    {
        private const int GUARDIANRELATIONID = 23;
        protected GirafUserRepositoryContext(DbContextOptions<GirafDbContext> contextOptions)
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
                var user = new GirafUser()
                {
                    UserName = "John",
                    DisplayName = "UsernameDisplay",
                    Id = "1",
                    DepartmentKey = 2
                };
                
                context.Add(user);
                context.SaveChanges();
            }
        }
    }

    public class GirafUserRepositoryTest : GirafUserRepositoryContext
    {
        private const string UserId = "1";
        private const string USERNAME = "John";
        public GirafUserRepositoryTest()
            : base(new DbContextOptionsBuilder<GirafDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N")).Options){}

        [Theory]
        [InlineData(UserId)]
        public void GetWithWeekSchedulesTest(string id)
        {
            //Arrange
            using (var context = new GirafDbContext(ContextOptions))
            {
                var repository = new GirafUserRepository(context);

                //Act
                var username = repository.GetWithWeekSchedules(id).UserName;

                //Assert
                Assert.Equal(USERNAME, username);
            }
        }

        [Theory]
        [InlineData(UserId)]
        public void GetUserWithIdTest(string id)
        {
            //Arrange
            using (var context = new GirafDbContext(ContextOptions))
            {
                var repository = new GirafUserRepository(context);
                
                //Act
                var userName = repository.GetUserWithId(id).UserName;
                
                //Assert
                Assert.Equal(USERNAME, userName);
            }
        }

        [Theory]
        [InlineData(USERNAME)]
        public void UserExists_Sucess_test(string username)
        {
            // Arrange 
            using (var context = new GirafDbContext(ContextOptions))
            {
                var repository = new GirafUserRepository(context);
                
                // Act
                bool response = repository.ExistsUsername(username);
                
                // Assert
                Assert.True(response);
            }
        }

        [Theory]
        [InlineData("NotEvenHere")]
        public void UserExists_UserNotExists_test(string username)
        {
            // Arrange 
            using (var context = new GirafDbContext(ContextOptions))
            {
                var repository = new GirafUserRepository(context);

                // Act
                bool response = repository.ExistsUsername(username);

                // Assert
                Assert.False(response);
            }
        }
        
        [Theory]
        [InlineData(USERNAME)]
        public void GetUserByUsername_Succes_test(string username)
        {
            // Arrange 
            using (var context = new GirafDbContext(ContextOptions))
            {
                var repository = new GirafUserRepository(context);

                // Act
                var response = repository.GetUserByUsername(username);

                // Assert
                Assert.Equal(UserId, response.Id);
                Assert.Equal(USERNAME,response.UserName);
            }
        }
    }
}