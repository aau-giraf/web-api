using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Threading;
using System.Linq;
using Giraf.UnitTest.FakeRepositorysContext;
using GirafEntities.User;
using GirafEntities.WeekPlanner;
using GirafRepositories.Persistence;
using GirafRepositories.WeekPlanner;

namespace Giraf.UnitTest.Repositories
{

    public class WeekRepositoryTest : FakeWeekRepositoryContext
    {
        public WeekRepositoryTest() : base(new DbContextOptionsBuilder<GirafDbContext>().UseInMemoryDatabase("Filename=TestWeekRep.db").Options)
        {
                
        }

        /// <summary>
        /// Tests whether <see cref="WeekRepository.getAllWeeksOfUser(string)">getAllWeeksOfUser</see>
        /// gets all relevant data to a specific user. 
        /// <para>If you want to test getAllWeeksOfUser for more data then go to <see cref="FakeWeekRepositoryContext"></see> and add it. Afterwards test it with Assert.Collection </para>
        /// 
        /// </summary>
        [Fact]
        public async void can_get_all_weeks_of_user()
        {
            using (var context = new GirafDbContext(ContextOptions))
            {
                var Repository = new WeekRepository(context);
                GirafUser testweeks = await Repository.getAllWeeksOfUser("1");

                Assert.Equal(3, testweeks.WeekSchedule.Count);
                Assert.Collection(testweeks.WeekSchedule,
                       item => Assert.Equal(1, item.Id),
                       item => Assert.Equal(2, item.Id),
                       item => Assert.Equal(3, item.Id)
                       );
                Assert.Collection(testweeks.WeekSchedule,
                       item => Assert.Equal("FirstWeek", item.Name),
                       item => Assert.Equal("SecondWeek", item.Name),
                       item => Assert.Equal("ThirdWeek", item.Name)
                         );
            } 
        }

        [Fact]
        public async void can_Load_User_With_Week_Schedules()
        {
            using (var context = new GirafDbContext(ContextOptions))
            {
                var Repository = new WeekRepository(context);
                GirafUser testUser = await Repository.LoadUserWithWeekSchedules("two");
                List<Week> weeks = testUser.WeekSchedule.ToList();
                Assert.NotNull(testUser);
                Assert.Equal(2, testUser.WeekSchedule.Count);
                Assert.Equal("Apple", weeks[0].Thumbnail.Title);
                Assert.Equal("Banana", weeks[1].Thumbnail.Title);
                Assert.Equal(2, testUser.WeekSchedule.Sum(w => w.Weekdays.Count));
                Assert.Equal(4, testUser.WeekSchedule.Sum(w=> w.Weekdays.Sum(d=>d.Activities.Count)));
                Assert.Equal(3, weeks[0].Weekdays.ToList()[0].Activities.ToList()[0].Pictograms.ToList()[0].PictogramId);
                Assert.Equal(3, weeks[1].Weekdays.ToList()[0].Activities.ToList()[0].TimerKey);


            }
        }
        /// <summary>
        /// Test deletion of specific week 
        /// <para>First makes sure the week exists before deleting then test afterwards if the week is deleted</para>
        /// </summary>
        [Fact]
        public async void can_delete_specific_week()
        {
            using(var context = new GirafDbContext(ContextOptions))
            {
                var Repository = new WeekRepository(context);
                GirafUser testUser = await Repository.getAllWeeksOfUser("Three");
                Assert.True(testUser.WeekSchedule.Any(w => w.WeekYear == 2016 && w.WeekNumber == 40));

                var week = testUser.WeekSchedule.FirstOrDefault(w => w.WeekYear == 2016 && w.WeekNumber == 40);
                Assert.NotNull(week);

                Repository.DeleteSpecificWeek(testUser, week);
                GirafUser testUserWithDeletedWeek = await Repository.getAllWeeksOfUser("1");
                Assert.False(testUserWithDeletedWeek.WeekSchedule.Any(w => w.WeekYear == 2016 && w.WeekNumber == 40));

            }
        }
      
        //No idea how to test that the actual database gets changed. It is always the same object i grab. Goahead and change weektest after weektest2 is defined. THe change will affect weektest2 aswell
        [Fact]
        public async void can_update_specific_week()
        {
            using(var context = new GirafDbContext(ContextOptions))
            {
                var Repository = new WeekRepository(context);
                long one = 1;
                var weektest = Repository.Get(one);
                weektest.WeekYear = 30000;
                int awaitResult = await Repository.UpdateSpecificWeek(weektest);
                var weektest2 = Repository.Get(one);

                Assert.Equal(30000, weektest2.WeekYear);

            }
        }


        


    }
}
    