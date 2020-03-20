using GirafRest.Models;
using GirafRest.Setup;
using GirafRest.Test.Mocks;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public SampleDataHandlerTest(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }
        
        [Fact]
        public void DeserializeDataTest()
        {
            SampleDataHandler dataHandler = new SampleDataHandler("..\\netcoreapp2.2\\Data\\samples.json");
            SampleData data = dataHandler.DeserializeData();
            
            List<GirafUser> users = data.userList;
            Console.WriteLine(data.userList.Count);
            List<Department> deps = data.departmentList;
            List<Pictogram> pics = data.pictogramList;

            Assert.NotNull(data);
            Assert.Equal(users[0].UserName, "Graatand");
            Assert.Equal(users[1].UserName, "Lee");
            Assert.Equal(users[2].UserName, "Tobias");
            Assert.Equal(deps[0].Name, "Bajer plejen");
            Assert.Equal(deps[1].Name, "Tobias' stue for godt humør");
            Assert.Equal(pics[0].Title, "Epik");
            Assert.Equal(pics[1].Title, "som");
            Assert.Equal(pics[2].Title, "slut");
        }

        [Fact]
        public void SerializeDataTest()
        {
            _testContext = new TestContext();
            string testJson = "..\\netcoreapp2.2\\Data\\samplesTest.json";

            SampleDataHandler dataHandler = new SampleDataHandler(testJson);
            MockDbContext mockDb = _testContext.MockDbContext.Object;


            if (File.Exists(testJson))
            {
                DateTime oldCreationTime = File.GetLastAccessTime(testJson);
                File.Delete(testJson);
                dataHandler.SerializeData(mockDb);

                DateTime newCreationTime = File.GetLastAccessTime(testJson);
                _outputHelper.WriteLine("OLD TIME: " + oldCreationTime);
                _outputHelper.WriteLine("NEW TIME: " + newCreationTime);

                Assert.True(DateTime.Compare(oldCreationTime, newCreationTime) < 0);
            }
            else
            {
                dataHandler.SerializeData(mockDb);
                Assert.True(File.Exists(testJson));
            }
        }

    }
}