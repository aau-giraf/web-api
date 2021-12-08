using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GirafRest.Data;
using GirafRest.Models;
using Microsoft.EntityFrameworkCore;

namespace GirafRest.Test.FakeRepositorysContext
{
   public class FakePictogramRepositoryContext
    {
        public FakePictogramRepositoryContext(DbContextOptions<GirafDbContext>contextOptions            )
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
                //PictogramRelations
                //Here i am just setting an activity ID for easy retrieval in tests. 
                PictogramRelation Relation1 = new PictogramRelation() { Pictogram = Pictogram1, PictogramId = 1, ActivityId = 1 };
                PictogramRelation Relation2 = new PictogramRelation() { Pictogram = Pictogram1, PictogramId = 2, ActivityId = 2 };
                PictogramRelation Relation3 = new PictogramRelation() { Pictogram = Pictogram2, PictogramId = 3, ActivityId = 3 };
                PictogramRelation Relation4 = new PictogramRelation() { Pictogram = Pictogram3, PictogramId = 4, ActivityId = 4 };
                PictogramRelation Relation5 = new PictogramRelation() { Pictogram = Pictogram4, PictogramId = 5, ActivityId = 5 };
                PictogramRelation Relation6 = new PictogramRelation() { Pictogram = Pictogram5, PictogramId = 6, ActivityId = 6 };

                var Pictogram6 = new Pictogram()
                {
                    Title = "Unicorn",
                    AccessLevel = AccessLevel.PUBLIC,
                    

                };
                Pictogram6.Id = 345567;

                //add pictograms
                context.AddRange(Pictogram1, Pictogram2, Pictogram3, Pictogram4, Pictogram5, Pictogram6);
                context.AddRange(Relation1, Relation2, Relation3, Relation4, Relation5, Relation6);

                context.SaveChanges();
            }
        }
    }
}
