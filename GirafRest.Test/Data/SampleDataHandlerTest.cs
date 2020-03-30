using GirafRest.Models;
using GirafRest.Setup;
using GirafRest.Test.Mocks;
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
        //"..\\netcoreapp2.2\\Data\\samplesTest.json"
        private readonly string testJson = $".{Path.DirectorySeparatorChar}" +
            $"Data" +
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
            MockDbContext mockDb = _testContext.MockDbContext.Object;
            SampleDataHandler dataHandler = new SampleDataHandler(testJson);

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
            Assert.Equal(users[0].Name, "Admin");
            Assert.Equal(users[1].Name, "Guardian in dep 2");
            Assert.Equal(users[2].Name, "Citizen of dep 2");
            Assert.Equal(deps[0].Name, "Mock Department");
            Assert.Equal(deps[1].Name, "Mock Department2");
            Assert.Equal(pics[0].Title, "Picto 1");
            Assert.Equal(pics[1].Title, "Public Picto2");
            Assert.Equal(pics[2].Title, "No restrictions");
            Assert.Equal(weeks[0].Name, "My awesome week");
            Assert.Equal(weeks[1].Name, "My not so awesome week");
            Assert.Equal(weekTemplates[0].Name, "Template1");
            Assert.Equal(weekTemplates[1].Name, "Template2");
            Assert.Equal(weekdays[0].Day, Days.Monday);
            Assert.Equal(weekdays[1].Day, Days.Tuesday);
            Assert.Equal(weekdays[2].Day, Days.Wednesday);
        }

        [Fact]
        public async void SerializeDataTest()
        {
            _testContext = new TestContext();
            SampleDataHandler dataHandler = new SampleDataHandler(testJson);
            MockDbContext mockDb = _testContext.MockDbContext.Object;


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