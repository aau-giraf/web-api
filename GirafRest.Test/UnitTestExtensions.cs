using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GirafRest.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Moq;
using GirafRest.Test.Mocks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System.IO;
using Xunit.Abstractions;

namespace GirafRest.Test
{
    public static class UnitTestExtensions
    {
        public class TestContext : IDisposable
        {
            #region Mock Data
            private List<Pictogram> mockPictograms;
            public List<Pictogram> MockPictograms
            {
                get
                {
                    if (mockPictograms == null)
                        mockPictograms = new List<Pictogram> {
                        new Pictogram("Public Picto1", AccessLevel.PUBLIC) {
                            Id = 0
                        },
                        new Pictogram("Public Picto2", AccessLevel.PUBLIC) {
                            Id = 1
                        },
                        new Pictogram("No restrictions", AccessLevel.PUBLIC) {
                            Id = 2
                        },
                        new Pictogram("Private for user 0", AccessLevel.PRIVATE) {
                            Id = 3
                        },
                        new Pictogram("Private for user 1", AccessLevel.PRIVATE) {
                            Id = 4
                        },
                        new Pictogram("Protected for Dep 1", AccessLevel.PROTECTED)
                        {
                            Id = 5
                        },
                        new Pictogram("Protected for Dep 2", AccessLevel.PROTECTED) {
                            Id = 6
                        }
                    };

                    return mockPictograms;
                }
            }
            private List<GirafUser> mockUsers;
            public IReadOnlyList<GirafUser> MockUsers
            {
                get
                {
                    if (mockUsers == null)
                        mockUsers = new List<GirafUser>() {
                        new GirafUser("Mock User", 0) {
                            DepartmentKey = 1
                        },
                        new GirafUser("Owner of other privates", 1)
                        {
                            DepartmentKey = 2
                        },
                        new GirafUser("User with no weeks", 2)
                        {
                            DepartmentKey = 3
                        }
                    };

                    return mockUsers;
                }
            }
            private List<Week> mockWeeks;
            public List<Week> MockWeeks
            {
                get
                {
                    if(mockWeeks == null)
                        mockWeeks = new List<Week>()
                        {
                            new Week()
                            {
                                Weekdays = new List<Weekday>(){
                                    new Weekday(){
                                        Day = Days.Monday
                                    },
                                    new Weekday(){
                                        Day = Days.Tuesday
                                    },
                                    new Weekday(){
                                        Day = Days.Wednesday
                                    },
                                    new Weekday(){
                                        Day = Days.Thursday
                                    },
                                    new Weekday(){
                                        Day = Days.Friday
                                    },
                                    new Weekday(){
                                        Day = Days.Saturday
                                    },
                                    new Weekday(){
                                        Day = Days.Sunday
                                    }
                                }
                            },
                            new Week(){
                                Weekdays = new List<Weekday>(){
                                    new Weekday(){
                                        Day = Days.Monday
                                    },
                                    new Weekday(){
                                        Day = Days.Tuesday
                                    },
                                    new Weekday(){
                                        Day = Days.Wednesday
                                    },
                                    new Weekday(){
                                        Day = Days.Thursday
                                    },
                                    new Weekday(){
                                        Day = Days.Friday
                                    },
                                    new Weekday(){
                                        Day = Days.Saturday
                                    },
                                    new Weekday(){
                                        Day = Days.Sunday
                                    }
                                }
                            },
                        };
                    MockUsers[0].WeekSchedule.Add(mockWeeks[0]);
                    MockUsers[1].WeekSchedule.Add(mockWeeks[1]);
                    MockUsers[2].WeekSchedule.Clear();
                    return mockWeeks;
                }
            }
            private List<Department> mockDepartments;
            public IReadOnlyList<Department> MockDepartments
            {
                get
                    {
                        if (mockDepartments == null)
                            mockDepartments = new List<Department>() {
                            new Department()
                            {
                                Key = 1,
                                Name = "Mock Department",
                                Members = new List<GirafUser>()
                                {
                                    MockUsers[0]
                                }
                            },
                            new Department()
                            {
                                Key = 2,
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
            private List<Choice> mockChoices;
            public List<Choice> MockChoices
            {
                get
                    {
                        if (mockChoices == null)
                            mockChoices = new List<Choice>()
                        {
                            new Choice(MockPictograms.Where(p => p.AccessLevel == AccessLevel.PUBLIC).Cast<PictoFrame>().ToList())
                            {
                                Id = 0
                            },
                            //A private pictogram for mock user 0
                            new Choice(new List<PictoFrame>() {
                                MockPictograms[3]
                            })
                            {
                                Id = 1
                            },
                            //A choice for department 0 (with id 1)
                            new Choice(new List<PictoFrame>() {
                                MockPictograms[5]
                            })
                            {
                                Id = 2
                            },
                            //A choice for department 1 (with id 2)
                            new Choice(new List<PictoFrame>()
                            {
                                MockPictograms[6]
                            })
                            {
                                Id = 3
                            },
                        };
                        return mockChoices;
                    }
            }
            private List<UserResource> mockUserResources;
            public IReadOnlyList<UserResource> MockUserResources
            {
                get
                    {
                        if (mockUserResources == null)
                            mockUserResources = new List<UserResource>() {
                            new UserResource(MockUsers[0], MockPictograms[3]),
                            new UserResource(MockUsers[1], MockPictograms[4])
                        };

                        return mockUserResources;
                    }
            }
            private List<DepartmentResource> mockDepartmentResources;
            public IReadOnlyList<DepartmentResource> MockDepartmentResources
            {
                    get
                    {
                        if (mockDepartmentResources == null)
                            mockDepartmentResources = new List<DepartmentResource>()
                        {
                            new DepartmentResource(MockDepartments[0], MockPictograms[5]),
                            new DepartmentResource(MockDepartments[1], MockPictograms[6])
                        };

                        return mockDepartmentResources;
                    }
            }
            #endregion
            public readonly Mock<MockDbContext> MockDbContext;
            public readonly MockUserManager MockUserManager;
            public Mock<HttpContext> MockHttpContext { get; set; }
            public Mock<ILoggerFactory> MockLoggerFactory { get; private set;}

            public TestContext()
            {
                MockDbContext = CreateMockDbContext();
                MockUserManager = CreateMockUserManager();

                var mockLogger = new Mock<ILogger>();

                MockLoggerFactory = new Mock<ILoggerFactory>();
                MockLoggerFactory.Setup(lf => lf.CreateLogger(It.IsAny<string>()))
                    .Returns(mockLogger.Object);
            }
    
            private Mock<MockDbContext> CreateMockDbContext()
            {
                var mockSet = CreateMockDbSet(MockPictograms);
                var mockRelationSet = CreateMockDbSet(MockUserResources);
                var mockDepRes = CreateMockDbSet(MockDepartmentResources);
                var mockChoices = CreateMockDbSet(MockChoices);
                var mockDeps = CreateMockDbSet(MockDepartments);
                var mockPF = CreateMockDbSet(MockPictograms.Cast<PictoFrame>().ToList());

                var dbMock = new Mock<MockDbContext>();
                dbMock.Setup(c => c.Pictograms).Returns(mockSet.Object);
                dbMock.Setup(c => c.UserResources).Returns(mockRelationSet.Object);
                dbMock.Setup(c => c.DepartmentResources).Returns(mockDepRes.Object);
                dbMock.Setup(c => c.Choices).Returns(mockChoices.Object);
                dbMock.Setup(c => c.Departments).Returns(mockDeps.Object);
                dbMock.Setup(c => c.PictoFrames).Returns(mockPF.Object);

                //Make sure that all references are setup - Entity does not handle it for us this time.
                MockUsers[0].Department = MockDepartments[0];
                MockUsers[1].Department = MockDepartments[1];

                return dbMock;
            }

            private MockUserManager CreateMockUserManager()
            {
                var userStore = new Mock<IUserStore<GirafUser>>();
                var umMock = new MockUserManager(userStore.Object);
                return umMock;
            }

            private Mock<ILoggerFactory> CreateMockLoggerFactory()
            {
                var logs = new List<string>();
                var lMock = new Mock<ILogger>();

                var lfMock = new Mock<ILoggerFactory>();
                lfMock.Setup(lf => lf.CreateLogger(It.IsAny<string>())).Returns(lMock.Object);

                return lfMock;
            }

            public void Dispose()
            {

            }
        }

        public static Mock<HttpContext> MockHttpContext(this Controller controller)
        {
            var hContext = new Mock<HttpContext>();

            hContext
                .Setup(c => c.User)
                .Returns(new System.Security.Claims.ClaimsPrincipal());

            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = hContext.Object;

            return hContext;
        }

        public static void MockQuery(this Mock<HttpContext> context, string key, string value)
        {
            context.Setup(c => c.Request.Query[key])
                .Returns(value);
        }

        public static void MockClearQueries(this Mock<HttpContext> context)
        {
            context.Setup(c => c.Request.Query)
                .Returns(new QueryCollection());
        }

        public static void MockContentType (this Mock<HttpContext> context, string contentType)
        {
            context.Setup(c => c.Request.ContentType)
                .Returns(contentType);
        }

        public static void MockRequestImage(this Mock<HttpContext> context, string filepath)
        {
            context.Setup(hc => hc.Request.Body)
                .Returns(new FileStream(filepath, FileMode.Open, FileAccess.Read));
            if (filepath.Contains("png"))
                context.MockContentType("image/png");
            else if (filepath.Contains("jpg") || filepath.Contains("jpeg"))
                context.MockContentType("image/jpeg");
        }

        public static void MockRequestNoImage(this Mock<HttpContext> context)
        {
            context.Setup(hc => hc.Request.Body)
                .Returns(new MemoryStream());
        }
        public static Mock<DbSet<T>> CreateMockDbSet<T>(IReadOnlyList<T> dataList) 
            where T : class
        {
            var copyList = new List<T>();
            copyList.AddRange(dataList);

            IQueryable<T> data = copyList.AsQueryable();

            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IAsyncEnumerable<T>>()
                .Setup(m => m.GetEnumerator())
                .Returns(new TestDbAsyncEnumerator<T>(data.GetEnumerator()));
            

            mockSet.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<T>(data.Provider));

            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            return mockSet;
        }
    }
}