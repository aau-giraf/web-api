using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GirafEntities.WeekPlanner;
using GirafRepositories.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Giraf.UnitTest.FakeRepositorysContext
{
   public class FakeTimerRepositoryContext
    {
        public FakeTimerRepositoryContext(DbContextOptions<GirafDbContext>contextOptions)
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

                Activity activity = new Activity() { Title = "Title" };
                Timer timer = new Timer() { FullLength = 100,Key = 1 };
                activity.Timer = timer;

                context.AddRange(activity);

                context.SaveChanges();
            }
        }
    }
}
