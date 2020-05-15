using GirafRest.Data;
using GirafRest.Models;
using GirafRest.Setup;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using Xunit.Abstractions;
using static GirafRest.Test.UnitTestExtensions;

namespace GirafRest.Test
{
    public class SampleDataHandlerTest
    {
        private readonly ITestOutputHelper _outputHelper;
        private TestContext _testContext;

        private static readonly string dataDir = $"{Directory.GetCurrentDirectory()}" +
            $"{Path.DirectorySeparatorChar}" +
            $"Data";

        private readonly string testJson = dataDir +
            $"{Path.DirectorySeparatorChar}" +
            $"samplesTest.json";

        public SampleDataHandlerTest(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }
        
        [Fact]
        public async void DeserializeDataTest()
        {
            _testContext = new TestContext();
            GirafDbContext mockDb = _testContext.MockDbContext.Object;
            SampleDataHandler dataHandler = new SampleDataHandler(testJson);

            if (!Directory.Exists(dataDir))
            {
                Directory.CreateDirectory(dataDir);
            }
            if (!File.Exists(testJson))
            {
                await dataHandler.SerializeDataAsync(mockDb, _testContext.MockUserManager);
            }

            SampleData data = dataHandler.DeserializeData();

            List<SampleGirafUser> users = data.UserList;
            List<SampleDepartment> deps = data.DepartmentList;
            List<SamplePictogram> pics = data.PictogramList;
            List<SampleWeek> weeks = data.WeekList;
            List<SampleWeekTemplate> weekTemplates = data.WeekTemplateList;
            List<SampleWeekday> weekdays = data.WeekdayList;

            Assert.NotNull(data);
            Assert.Equal("Admin", users[0].Name);
            Assert.Equal("Guardian in dep 2", users[1].Name);
            Assert.Equal("Citizen of dep 2",users[2].Name);
            Assert.Equal("Mock Department", deps[0].Name);
            Assert.Equal("Mock Department2", deps[1].Name);
            Assert.Equal("Picto 1", pics[0].Title);
            Assert.Equal("Public Picto2", pics[1].Title);
            Assert.Equal("No restrictions", pics[2].Title);
            Assert.Equal("My awesome week", weeks[0].Name);
            Assert.Equal("My not so awesome week", weeks[1].Name);
            Assert.Equal("Template1",weekTemplates[0].Name);
            Assert.Equal("Template1", weekTemplates[1].Name);
            Assert.Equal(Day.Monday, weekdays[0].Day);
            Assert.Equal(Day.Tuesday, weekdays[1].Day);
            Assert.Equal(Day.Wednesday, weekdays[2].Day);
        }

        [Fact]
        public async void SerializeDataTest()
        {
            _testContext = new TestContext();
            SampleDataHandler dataHandler = new SampleDataHandler(testJson);
            GirafDbContext mockDb = _testContext.MockDbContext.Object;


            if (File.Exists(testJson))
            {
                DateTime oldCreationTime = File.GetLastAccessTime(testJson);
                File.Delete(testJson);
                await dataHandler.SerializeDataAsync(mockDb, _testContext.MockUserManager);

                DateTime newCreationTime = File.GetLastAccessTime(testJson);
                _outputHelper.WriteLine("OLD TIME: " + oldCreationTime);
                _outputHelper.WriteLine("NEW TIME: " + newCreationTime);

                Assert.True(DateTime.Compare(oldCreationTime, newCreationTime) < 0);
            }
            else
            {
                await dataHandler.SerializeDataAsync(mockDb, _testContext.MockUserManager);
                Assert.True(File.Exists(testJson));
            }
        }
    }
}