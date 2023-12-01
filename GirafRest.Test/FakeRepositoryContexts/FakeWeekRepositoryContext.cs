using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GirafRest.Data;
using GirafRest.Models;
using Microsoft.EntityFrameworkCore;

namespace GirafRest.Test.FakeRepositorysContext
{
   public class FakeWeekRepositoryContext
    {
        public FakeWeekRepositoryContext(DbContextOptions<GirafDbContext>contextOptions)
        {
            ContextOptions = contextOptions;

            seed();

        }
        protected DbContextOptions<GirafDbContext> ContextOptions { get; }

        private void seed()
        {
            using(var context = new GirafDbContext(ContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                //Pictograms
                Pictogram Pictogram1 = new Pictogram("Apple", AccessLevel.PUBLIC, "AppleHash");
                Pictogram Pictogram2 = new Pictogram("Banana", AccessLevel.PUBLIC, "BananaHash");
                Pictogram Pictogram3 = new Pictogram("Orange", AccessLevel.PUBLIC, "OrangeHash");
                Pictogram Pictogram4 = new Pictogram("Kiwi", AccessLevel.PUBLIC, "KiwiHash");
                Pictogram Pictogram5 = new Pictogram("Peach", AccessLevel.PUBLIC, "PeachHash");
                //add pictograms
                context.AddRange(Pictogram1, Pictogram2, Pictogram3, Pictogram4, Pictogram5);
           
                //girafuser1 used for getAllWeeksOfUser
                var girafuser1 = new GirafUser();
                girafuser1.Id = "1";
                girafuser1.DisplayName = "";
                var ID1_user1week1 = new Week();
                ID1_user1week1.Name = "FirstWeek";
                var ID2_user1week2 = new Week();
                ID2_user1week2.Name = "SecondWeek";
                var ID3_user1week3 = new Week();
                ID3_user1week3.Name = "ThirdWeek";
       
                girafuser1.WeekSchedule.Add(ID1_user1week1);
                girafuser1.WeekSchedule.Add(ID2_user1week2);
                girafuser1.WeekSchedule.Add(ID3_user1week3);
                //girafuser2 used for  LoadUserWithWeekSchedules
                var girafuser2 = new GirafUser();
                var ID4_user2week1 = new Week();
                var ID5_user2week2 = new Week();
                var weekday1 = new Weekday();
                var weekday2 = new Weekday();
                var week1 = new Week();
                var week2 = new Week();
                girafuser2.Id = "TWO";
                girafuser2.DisplayName = "";
                ID4_user2week1.Thumbnail = Pictogram1;
                ID5_user2week2.Thumbnail = Pictogram2;
                weekday1.Day = Days.Monday;
                weekday2.Day = Days.Tuesday;
                var activity1 = new Activity(weekday1, new List<Pictogram> { Pictogram3 }, 1, ActivityState.Active, new Timer(), true, "Test1");
                var activity2 = new Activity(weekday1, new List<Pictogram> { Pictogram1 }, 1, ActivityState.Active, new Timer(), true, "Test2");
                var activity3 = new Activity(weekday2, new List<Pictogram> { Pictogram5 }, 1, ActivityState.Active, new Timer() { StartTime = 2, FullLength = 10, Paused = false }, true, "Test3");
                var activity4 = new Activity(weekday2, new List<Pictogram> { Pictogram4 }, 1, ActivityState.Active, new Timer(), true, "Test4");
                activity1.Pictograms.ToList()[0].PictogramId=  3;
                activity2.Pictograms.ToList()[0].PictogramId = 1;
                activity3.Pictograms.ToList()[0].PictogramId = 5;
                activity4.Pictograms.ToList()[0].PictogramId = 4;
                weekday1.Activities.Add(activity1);
                weekday1.Activities.Add(activity2);
                weekday2.Activities.Add(activity3);
                weekday2.Activities.Add(activity4);
                ID4_user2week1.Weekdays.Add(weekday1);
                ID5_user2week2.Weekdays.Add(weekday2);
                girafuser2.WeekSchedule.Add(ID4_user2week1);
                girafuser2.WeekSchedule.Add(ID5_user2week2);
                //GirafUser for Deletespecificweek
                var girafuser3 = new GirafUser() { Id = "Three", DisplayName = ""};
                var ID6_user3week1 = new Week() { WeekYear = 2016 , WeekNumber = 40};
                girafuser3.WeekSchedule.Add(ID6_user3week1);
                //items for tesing SetWeekDTO and AddPictograms
                var activity5 = new Activity(weekday2, new List<Pictogram> { Pictogram4 }, 1, ActivityState.Active, new Timer(), true, "Test5");
                var activity6 = new Activity(weekday2, new List<Pictogram> { Pictogram4 }, 1, ActivityState.Active, new Timer(), true, "Test6");
                var activity7 = new Activity(weekday2, new List<Pictogram> { Pictogram4 }, 1, ActivityState.Active, new Timer(), true, "Test7");
                var activity8 = new Activity(weekday2, new List<Pictogram> { Pictogram4 }, 1, ActivityState.Active, new Timer(), true, "Test8");

               // Weekday Day = new Weekday(Days.Wednesday,new List<List<Pictogram>> { new List<Pictogram> { }, new List<Pictogram> { } });
                
                //add giraf users
                context.AddRange(girafuser1,girafuser2,girafuser3);

                context.AddRange(activity1,activity2,activity3,activity4);

                context.SaveChanges();
            }
        }
    }
}
