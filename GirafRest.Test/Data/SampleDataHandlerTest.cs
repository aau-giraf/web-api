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
            Assert.Equal(users[0].Name, new string("Admin"));
            Assert.Equal(users[1].Name, new string("Guardian in dep 2"));
            Assert.Equal(users[2].Name, new string( "Citizen of dep 2"));
            Assert.Equal(deps[0].Name, new string("Mock Department"));
            Assert.Equal(deps[1].Name, new string("Mock Department2"));
            Assert.Equal(pics[0].Title, new string("Picto 1"));
            Assert.Equal(pics[1].Title, new string("Public Picto2"));
            Assert.Equal(pics[2].Title, new string("No restrictions"));
            Assert.Equal(weeks[0].Name, new string( "My awesome week"));
            Assert.Equal(weeks[1].Name, new string("My not so awesome week"));
            Assert.Equal(weekTemplates[0].Name, new string("Template1"));
            Assert.Equal(weekTemplates[1].Name, new string("Template2"));
            Assert.Equal(weekdays[0].Day, Day.Monday);
            Assert.Equal(weekdays[1].Day, Day.Tuesday);
            Assert.Equal(weekdays[2].Day, Day.Wednesday);
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