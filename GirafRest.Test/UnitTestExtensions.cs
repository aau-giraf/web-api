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
        private static List<GirafUser> mockUsers;
        public static List<GirafUser> MockUsers
        {
            get
            {
                if(mockUsers == null)
                    mockUsers = new List<GirafUser>() {
                        new GirafUser("Mock User", 0) {
                            DepartmentKey = 1
                        },
                        new GirafUser("Owner of other privates", 1)
                        {
                            DepartmentKey = 2
                        }
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
        private static List<Choice> mockChoices;
        public static List<Choice> MockChoices
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

        public static void MockRequestImage(this Mock<HttpContext> context, string filepath)
        {
            context.Setup(hc => hc.Request.Body)
                .Returns(new FileStream(filepath, FileMode.Open, FileAccess.Read));
        }

        public static void MockRequestNoImage(this Mock<HttpContext> context)
        {
            context.Setup(hc => hc.Request.Body)
                .Returns(new MemoryStream());
        }

        public static Mock<MockDbContext> CreateMockDbContext()
        {
            var mockSet = CreateMockDbSet<Pictogram>(MockPictograms);
            var mockRelationSet = CreateMockDbSet<UserResource>(MockUserResources);
            var mockDepRes = CreateMockDbSet<DepartmentResource>(MockDepartmentResources);
            var mockChoices = CreateMockDbSet<Choice>(MockChoices);
            var mockDeps = CreateMockDbSet<Department>(MockDepartments);
            var mockPF = CreateMockDbSet<PictoFrame>(MockPictograms.Cast<PictoFrame>().ToList());

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

        public static void AddEmptyDepartmentList (Mock<MockDbContext> dbMock)
        {      
            dbMock.Reset();       
            var emptyDep = UnitTestExtensions.CreateMockDbSet<Department>(new List<Department>());
            dbMock.Setup(c => c.Departments).Returns(emptyDep.Object);
        }

        public static void AddSampleDepartmentList (Mock<MockDbContext> dbMock)
        {
            dbMock.Reset();             
            var mockDeps = MockDepartments;
            var mockDepartments = CreateMockDbSet<Department>(mockDeps);
            dbMock.Setup(c => c.Departments).Returns(mockDepartments.Object);
        }

        public static Mock<MockDbSet<T>> CreateMockDbSet<T>(List<T> dataList) 
            where T : class
        {
            IQueryable<T> data = dataList.AsQueryable();

            var mockSet = new Mock<MockDbSet<T>>();
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