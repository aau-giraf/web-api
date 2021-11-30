using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GirafRest.Data;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using GirafRest.Repositories;
using GirafRest.Test.FakeRepositorysContext;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GirafRest.Test.Repositories
{
    

    public class PictogramRepositoryTest : FakePictogramRepositoryContext
    {
        private const string PICTOGRAM_TITLE = "Unicorn";
        public PictogramRepositoryTest() : base(
            new DbContextOptionsBuilder<GirafDbContext>()
            .UseInMemoryDatabase("Filename=TestPictogramRep.db").Options)
        {

        }
        [Fact]
        public async Task FetchResourceWithId()
        {
            using (var context = new GirafDbContext(ContextOptions))
            {
                var repository = new PictogramRepository(context);
                
                var resourceIdDTO = new ResourceIdDTO()
                {
                    Id = 345567
                };
                
                var resource = await repository.FetchResourceWithId(resourceIdDTO);
                
                Assert.Equal(PICTOGRAM_TITLE, resource.Title);
            }
        }
        [Fact]
        // Tests whether you can get pictograms with from PictogramRelations and that different relations can share same pictograms.
        public async void can_get_pictogram_with_matcing_relation()
        {
            using (var context = new GirafDbContext(ContextOptions))
            {
                var Repository = new PictogramRepository(context);
                PictogramRelation Relation1 = context.PictogramRelations.FirstOrDefault(PR => PR.ActivityId == 1);
                PictogramRelation Relation2 = context.PictogramRelations.FirstOrDefault(PR => PR.ActivityId == 2);
                PictogramRelation Relation3 = context.PictogramRelations.FirstOrDefault(PR => PR.ActivityId == 5);
                Pictogram Pictogram1 = await Repository.getPictogramMatchingRelation(Relation1);
                Pictogram Pictogram2 = await Repository.getPictogramMatchingRelation(Relation2);
                Pictogram Pictogram3 = await Repository.getPictogramMatchingRelation(Relation3);
                Assert.Equal("Apple", Pictogram1.Title);
                Assert.Equal("Kiwi", Pictogram3.Title);
                Assert.Same(Pictogram1, Pictogram2);
                
            }
        }
        //Tests whether you can get pictograms with names
        [Fact]
        public async void can_get_pictogram_with_name()
        {
            using (var context = new GirafDbContext(ContextOptions))
            {
                var Repository = new PictogramRepository(context);
                Pictogram Pictogram1 = await Repository.GetPictogramWithName("Banana");
                Pictogram Pictogram2 = await Repository.GetPictogramWithName("Orange");
                Assert.NotNull(Pictogram1);
                Assert.NotNull(Pictogram2);
                Assert.Equal("BananaHash", Pictogram1.ImageHash);
                Assert.Equal("OrangeHash", Pictogram2.ImageHash);
            }
        }
        [Fact]
        public async void can_add_pictogram_with_NO_Imagehash()
        {
            using(var context = new GirafDbContext(ContextOptions))
            {
                var Repository = new PictogramRepository(context);
                Assert.Null(await Repository.GetPictogramWithName("Tomato"));
                int thingsChanged = await Repository.AddPictogramWith_NO_ImageHash("Tomato", AccessLevel.PUBLIC);
                Assert.Equal(1, thingsChanged);
                Assert.NotNull(await Repository.GetPictogramWithName("Tomato"));

            }
        }
        [Fact]
        public async void can_find_resourse_from_resourseIdDTO()
        {
            using (var context = new GirafDbContext(ContextOptions))
            {
                var Repository = new PictogramRepository(context);
                ResourceIdDTO Resourse = new ResourceIdDTO() { Id = 1 };
                Pictogram pictogram = await Repository.FindResource(Resourse);
                Assert.Equal("Apple", pictogram.Title);
                
            }
        }

        [Fact]
        public async void can_find_pictogram_from_ID()
        {
            using (var context = new GirafDbContext(ContextOptions))
            {
                var Repository = new PictogramRepository(context);

                Pictogram pictogram1 = await Repository.GetPictogramWithID(1);
                Pictogram pictogram2 = await Repository.GetPictogramWithID(2);
                Pictogram pictogram3 = await Repository.GetPictogramWithID(3);
                Assert.Equal("Apple", pictogram1.Title);
                Assert.Equal("Banana", pictogram2.Title);
                Assert.Equal("Orange", pictogram3.Title);

            }
        }
    }
}
