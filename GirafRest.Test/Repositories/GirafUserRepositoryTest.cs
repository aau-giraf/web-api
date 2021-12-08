using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GirafRest.Data;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using GirafRest.Models.Enums;
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
                var user2 = new GirafUser();
                user2.Id = "2";
                user2.UserName = "John Appleseed";
                
                
                var user3 = new GirafUser();
                user3.Id = "3";
                user3.UserName = "Anna";

                var user4 = new GirafUser()
                {
                    Id = "4",
                    UserName = "Jacob"
                };
                
                GirafUser userAmazing = new GirafUser()
                {
                    Id = "40",
                    UserName =  "Aladdin",
                    DisplayName = "display",
                    DepartmentKey = 1
                };

                var guardianRelation = new GuardianRelation(user2, user3)
                {
                    Id = 23,
                    Guardian = user2,
                    Citizen = user3
                    
                };
                
                
                context.AddRange(user, user2, user3, user4, userAmazing,guardianRelation);
                context.SaveChanges();
                
            }

        }

    }

    public class GirafUserRepositoryTest : GirafUserRepositoryContext
    {
        private const string UserId = "1";
        private const int GUARDIANRELATIONID = 23;
        private const string USERNAME = "John";
        private const string USERNAME_1 = "Anna";
        private const string SweetAnnasId = "3";
        private const string USERNAME_2 = "Jacob";
        private const string ALADDIN = "Aladdin";
        private const string ALADDIN_ID = "40";
        public GirafUserRepositoryTest()
            : base(new DbContextOptionsBuilder<GirafDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N")).Options){}
    

        [Theory]
        [InlineData(UserId)]
        public async void GetWithWeekSchedulesTest(string id)
        {
            //Arrange
            using (var context = new GirafDbContext(ContextOptions))
            {
                var repository = new GirafUserRepository(context);
                
                //Act
                var user = await repository.GetUserWithId(id);
                var userName = user.UserName;
                
                //Assert
                Assert.Equal(USERNAME,userName);
            }
            
        }

        [Theory]
        [InlineData(USERNAME_1)]
        public async Task GetUserByUsername(string username)
        {
            using (var context = new GirafDbContext(ContextOptions))
            {
                var repository = new GirafUserRepository(context);
                var user = await repository.GetUserByUsername(username);
                var userName = user.UserName;

                //Assert
                Assert.Equal("Anna", userName);
            }
        }

        [Theory]
        [InlineData(UserId)]
        public async Task GetUserWithIdTest(string id)
        {
            //Arrange
            using (var context = new GirafDbContext(ContextOptions))
            {
                var repository = new GirafUserRepository(context);

                //Act
                var user = await repository.GetUserWithId(id);
                var userName = user.UserName;

                //Assert
                Assert.Equal(USERNAME, userName);
            }
        }
        

        [Theory]
        [InlineData(UserId)]
        public void CheckIfUserExistsTest(string id)
        {
            using (var context = new GirafDbContext(ContextOptions))
            {
                var repository = new GirafUserRepository(context);
                
                var userName = repository.CheckIfUserExists(id).UserName;
                
                //Assert
                Assert.Equal(USERNAME, userName);
            }
        }

        [Theory]
        [InlineData("4")]
        public void GetCitizensWithId(string id)
        {
            using (var context = new GirafDbContext(ContextOptions))
            {
                var repository = new GirafUserRepository(context);
                
                var userName = repository.GetCitizensWithId(id).UserName;
                
                Assert.Equal(USERNAME_2, userName);
            }
        }

        [Fact]
        public void GetFirstCitizen()
        {
            using (var context = new GirafDbContext(ContextOptions))
            {
                //Arrange
                var repository = new GirafUserRepository(context);
                var guardianRelation = context.GuardianRelations.FirstOrDefault(u => u.Id == GUARDIANRELATIONID);
                
                
                //Act
                var guardianRelationCitizenIdId = guardianRelation.CitizenId;
                var citizenId = repository.GetFirstCitizen(guardianRelation).Id;
                
                //Assert
                Assert.Equal(guardianRelationCitizenIdId,citizenId);
                
            }
            
        }

        [Theory]
        [InlineData(UserId)]
        public void GetGuardianWithId(string id)
        {
            using (var context = new GirafDbContext(ContextOptions))
            {
                var repository = new GirafUserRepository(context);

                var userName = repository.GetGuardianWithId(id).UserName;
                
                Assert.Equal(USERNAME, userName);
            }
        }

        [Fact]
        public void GetGuardianFromRelation()
        {
            using (var context = new GirafDbContext(ContextOptions))
            {
                //Arrange
                var repository = new GirafUserRepository(context);
                var guardianRelation = context.GuardianRelations.FirstOrDefault(u => u.Id == GUARDIANRELATIONID);
                
                
                //Act
                var guardianRelationGuardianId = guardianRelation.GuardianId;
                var citizenId = repository.GetGuardianFromRelation(guardianRelation).Id;

                
                Assert.Equal(guardianRelationGuardianId,citizenId);
                
            }
        }

        [Theory]
        [InlineData(UserId)]
        public void GetCitizenRelationship(string citizenId)
        {
            using (var context = new GirafDbContext(ContextOptions))
            {
                var repository = new GirafUserRepository(context);

                var userName = repository.GetCitizenRelationship(UserId).UserName;
                
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

                var userName = repository.GetUserSettingsByWeekDayColor(UserId).UserName;
                
                Assert.Equal(USERNAME, userName);
            }
        }

        [Fact]
        public void CheckIfUsernameHasSameId()
        {
            using (var context = new GirafDbContext(ContextOptions))
            {
                
                var repository = new GirafUserRepository(context);
                var user = context.Users.FirstOrDefault(u => u.Id == UserId);
                var girafUserDTO = new GirafUserDTO(user, GirafRoles.Citizen);
               
                bool output = repository.CheckIfUsernameHasSameId(girafUserDTO,user);
                
                // check whether user with that username already exist that does not have the same id
                Assert.False(output, "The id doesn't match");
            }
        }

        [Fact]
        public async Task LoadUserWithResources()
        {
            using (var context = new GirafDbContext(ContextOptions))
            {
                //Arrange
                var repository = new GirafUserRepository(context);
                var user = await context.Users.FirstOrDefaultAsync(x => x.Id == "3");
                
                var result = await repository.LoadUserWithResources(user);
                
                Assert.Equal("Anna", result.UserName);
                
            }
        }

        [Fact]
        public async Task LoadUserWithDepartment()
        {
            using (var context = new GirafDbContext(ContextOptions))
            {
                //Arrange
                var repository = new GirafUserRepository(context);
                var user = await context.Users.FirstOrDefaultAsync(x => x.Id == ALADDIN_ID);
                //Act
                var result = await repository.LoadUserWithDepartment(user);
                
                Assert.Equal(ALADDIN, result.UserName);
                
            }
        }

        [Theory]
        [InlineData("40")]
        public async Task LoadUserWithWeekSchedules(string id)
        {
            using (var context = new GirafDbContext(ContextOptions))
            {
                //Arrange
                var repository = new GirafUserRepository(context);
                
                //Act
                var result = await repository.LoadUserWithWeekSchedules(id);
                
                Assert.Equal(id, result.Id);

            }
        }
        [Fact]
        public async Task LoadBasicUserDataAsync()
        {
            using (var context = new GirafDbContext(ContextOptions))
            {
                //Arrange
                var repository = new GirafUserRepository(context);
                var user = await context.Users.FirstOrDefaultAsync(x => x.Id == "40");
                //Act
                var result = await repository.LoadBasicUserDataAsync(user);
                
                Assert.Equal(ALADDIN, result.UserName);
                
            }
        }
    }
}
