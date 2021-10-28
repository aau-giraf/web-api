using System;
using System.Collections.Generic;
using System.Text;
using GirafRest.Data;
using Microsoft.EntityFrameworkCore;

namespace GirafRest.Test.Repositories
{
   public class FakeWeekRepositoryContext
    {
        protected FakeWeekRepositoryContext(DbContextOptions<GirafDbContext>contextOptions            )
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


            }
        }
    }
}
