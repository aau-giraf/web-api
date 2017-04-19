using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GirafRest.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using GirafRest.Test.Mocks;

namespace GirafRest.Test
{
    public static class UnitTestExtensions
    {
        #region Mock Data
        private static List<Pictogram> mockPictograms;
        public static List<Pictogram> MockPictograms
        {
            get
            {
                if (mockPictograms == null)
                    mockPictograms = new List<Pictogram> {
                        new Pictogram("Public Picto1", AccessLevel.PUBLIC),
                        new Pictogram("Public Picto2", AccessLevel.PUBLIC),
                        new Pictogram("No restrictions", AccessLevel.PUBLIC),
                        new Pictogram("Private for user 1 ", AccessLevel.PRIVATE),
                        new Pictogram("Private for user 2", AccessLevel.PRIVATE),
                        new Pictogram("Protected for Dep 1", AccessLevel.PROTECTED),
                        new Pictogram("Protected for Dep 2", AccessLevel.PROTECTED)
                    };

                return mockPictograms;
            }
        }
        private static List<GirafUser> mockUsers;
        public static List<GirafUser> MockUsers
        {
            get
            {
                if(mockUsers == null)
                    mockUsers = new List<GirafUser>() {
                        new GirafUser("Mock User", 0),
                        new GirafUser("Owner of other privates", 1)
                    };

                return mockUsers;
            }
        }
        private static List<Department> mockDepartments;
        public static List<Department> MockDepartments
        {
            get
            {
                if(mockDepartments == null)
                    mockDepartments = new List<Department>() {
                        new Department()
                        {
                            Name = "Mock Department",
                            Members = new List<GirafUser>()
                            {
                                MockUsers[0]
                            }
                        },
                        new Department()
                        {
                            Name = "Mock Department2",
                            Members = new List<GirafUser>()
                            {
                                MockUsers[1]
                            }
                        }
                    };

                return mockDepartments;
            }
        }
        private static List<UserResource> mockUserResources;
        public static List<UserResource> MockUserResources
        {
            get
            {
                if(mockUserResources == null)
                    mockUserResources = new List<UserResource>() {
                        new UserResource(MockUsers[0], MockPictograms[3]),
                        new UserResource(MockUsers[1], MockPictograms[4])
                    };

                return mockUserResources;
            }
        }
        private static List<DepartmentResource> mockDepartmentResources;
        public static List<DepartmentResource> MockDepartmentResources
        {
            get
            {
                if(mockDepartmentResources == null)
                    mockDepartmentResources = new List<DepartmentResource>()
                    {
                        new DepartmentResource(MockDepartments[0], MockPictograms[5]),
                        new DepartmentResource(MockDepartments[1], MockPictograms[6])
                    };

                return mockDepartmentResources;
            }
        }
        #endregion

        public static Mock<MockDbContext> CreateMockDbContext()
        {
            var mockDeps = MockDepartments;
            var pictograms = MockPictograms;
            var relations = MockUserResources;
            var depRelations = MockDepartmentResources;

            var mockSet = CreateMockDbSet<Pictogram>(pictograms);
            var mockRelationSet = CreateMockDbSet<UserResource>(relations);
            var mockDepartments = CreateMockDbSet<Department>(mockDeps);
            var mockDepRes = CreateMockDbSet<DepartmentResource>(depRelations);

            var dbMock = new Mock<MockDbContext>();
            dbMock.Setup(c => c.Pictograms).Returns(mockSet.Object);
            dbMock.Setup(c => c.UserResources).Returns(mockRelationSet.Object);
            dbMock.Setup(c => c.Departments).Returns(mockDepartments.Object);
            dbMock.Setup(c => c.DepartmentResources).Returns(mockDepRes.Object);

            return dbMock;
        }
        public static Mock<DbSet<T>> CreateMockDbSet<T>(List<T> dataList) 
            where T : class
        {
            IQueryable<T> data = dataList.AsQueryable();
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IAsyncEnumerable<T>>()
                .Setup(m => m.GetEnumerator())
                .Returns(new TestDbAsyncEnumerator<T>(data.GetEnumerator()));

            mockSet.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<T>(data.Provider));
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns (data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            return mockSet;
        }

        public static MockUserManager MockUserManager(Mock<IUserStore<GirafUser>> userStore) {
            var umMock = new MockUserManager (userStore.Object);
            return umMock;
        }

        public static Mock<ILoggerFactory> CreateMockLoggerFactory() {
            var logs = new List<string>();
            var lMock = new Mock<ILogger>();

            var lfMock = new Mock<ILoggerFactory>();
            lfMock.Setup(lf => lf.CreateLogger(It.IsAny<string>())).Returns(lMock.Object);

            return lfMock;
        }
    }
}