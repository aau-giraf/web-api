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

            String str1 = new String("Admin");
            String str2 = new String("Guardian in dep 2");
            String str3 = new String("Citizen of dep 2");
            String str4 = new String("Mock Department");
            String str5 = new String("Mock Department2");
            String str6 = new String("Picto 1");
            String str7 = new String("Public Picto2");
            String str8 = new String("No restrictions");
            String str9 = new String("My awesome week");
            String str10 = new String("My not so awesome week");
            String str11 = new String("Template1");
            String str12 = new String("Template2");
            

            Assert.NotNull(data);
            Assert.Equal(users[0].Name, str1);
            Assert.Equal(users[1].Name, str2);
            Assert.Equal(users[2].Name, str3);
            Assert.Equal(deps[0].Name, str4);
            Assert.Equal(deps[1].Name, str5);
            Assert.Equal(pics[0].Title, str6);
            Assert.Equal(pics[1].Title, str7);
            Assert.Equal(pics[2].Title, str8);
            Assert.Equal(weeks[0].Name, str9);
            Assert.Equal(weeks[1].Name, str10);
            Assert.Equal(weekTemplates[0].Name, str11);
            Assert.Equal(weekTemplates[1].Name, str12);
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