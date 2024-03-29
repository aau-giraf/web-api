﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GirafEntities.WeekPlanner;
using GirafRepositories.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Giraf.UnitTest.FakeRepositorysContext
{
   public class FakeWeekdayRepositoryContext
    {
        public FakeWeekdayRepositoryContext(DbContextOptions<GirafDbContext>contextOptions)
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
                Weekday day1 = new Weekday() { Id = 1, Day = Days.Monday };

                context.AddRange(day1);
                context.SaveChanges();
            }
        }
    }
}
