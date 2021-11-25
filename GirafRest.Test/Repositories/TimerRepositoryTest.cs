using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using GirafRest.Data;
using Xunit;
using GirafRest.Repositories;
using GirafRest.Models;
using System.Threading;
using System.Linq;
using Microsoft.Extensions.Options;
using GirafRest.Test.FakeRepositorysContext;

namespace GirafRest.Test.Repositories
{
    public class TimerRepositoryTest : FakeTimerRepositoryContext
    {
        public TimerRepositoryTest() : base(new DbContextOptionsBuilder<GirafDbContext>().UseInMemoryDatabase("Filename=TestTimerRep.db").Options)
        {

        }

        [Fact]
        public async void can_get_activitys_timer()
        {
            using (var context = new GirafDbContext(ContextOptions))
            {
                var Repository = new TimerRepository(context);
                Activity activity = context.Activities.FirstOrDefault(u => u.Title == "Title");
                Models.Timer timer = await Repository.getActivitysTimerkey(activity);
                Assert.Equal(100, timer.FullLength);
                Assert.Equal(1, timer.Key);

            }
        }
        [Fact]
        public async void can_get_activitys_timer_through_key()
        {
            using (var context = new GirafDbContext(ContextOptions))
            {
                var Repository = new TimerRepository(context);
                Activity activity = context.Activities.FirstOrDefault(u => u.Title == "Title");
                Models.Timer timer = await Repository.getTimerWithKey(1);
                Assert.Equal(100, timer.FullLength);
                Assert.Equal(1, timer.Key);

            }
        }


    }
}
