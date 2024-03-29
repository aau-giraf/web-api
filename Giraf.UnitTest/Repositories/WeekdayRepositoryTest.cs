﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Threading;
using System.Linq;
using Giraf.UnitTest.FakeRepositorysContext;
using GirafEntities.WeekPlanner;
using GirafRepositories.Persistence;
using GirafRepositories.WeekPlanner;
using Microsoft.Extensions.Options;

namespace Giraf.UnitTest.Repositories
{
    public class WeekdayRepositoryTest : FakeWeekdayRepositoryContext
    {
        public  WeekdayRepositoryTest() : base(new DbContextOptionsBuilder<GirafDbContext>().UseInMemoryDatabase("Filename=TestWeekdayRep.db").Options)
        {
            
        }

        [Fact]
        public async void can_update_specific_day()
        {
            using (var context = new GirafDbContext(ContextOptions))
            {
                var Repository = new WeekdayRepository(context);
                var day = context.Weekdays.FirstOrDefault(u => u.Id == 1);
                Assert.Equal(Days.Monday, day.Day);
                day.Day = Days.Friday;
                Repository.UpdateSpecificWeekDay(day);
                var day2 = context.Weekdays.FirstOrDefault(u => u.Id == 1);
                Assert.Equal(Days.Friday, day2.Day);
                Assert.Same(day, day2);
            }
        }



    }
}
