using System;
using System.Collections.Generic;
using System.Linq;
using GirafRest.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using GirafRest.Test.Mocks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System.IO;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using GirafRest.Controllers;
using GirafRest.Services;

namespace GirafRest.Test
{
    public static class UnitTestExtensions
    {
        public class TestContext
        {
            #region Mock Data

            public const int PictogramPublic1 = 0;
            public const int PictogramPublic2 = 1;
            public const int PictogramNoRestrictions = 2;
            public const int PictogramPrivateUser0 = 3;
            public const int PictogramPrivateUser1 = 4;
            public const int PictogramDepartment1 = 5;
            public const int PictogramDepartment2 = 6;
            
            private List<Pictogram> _mockPictograms;
            public List<Pictogram> MockPictograms
            {
                get
                {
                    if (_mockPictograms == null)
                        _mockPictograms = new List<Pictogram> {
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

                    return _mockPictograms;
                }
            }

            public const int UserAdmin = 0;
            public const int UserGuardianDepartment2 = 1;
            public const int UserCitizenDepartment2 = 2;
            public const int UserCitizenNoWeeks = 3;
            public const int UserAdminNoDepartment = 4;
            public const int UserGuardian2Department2 = 5;
            public const int UserDepartment2 = 6;
            
            private List<GirafUser> mockUsers;
            public List<GirafUser> MockUsers
            {
                get
                {
                    if (mockUsers == null)
                        mockUsers = new List<GirafUser>() {
                        new GirafUser()
                        {
                            UserName = "Admin",
                            Id = "admin",
                            DepartmentKey = 1
                        },
                        new GirafUser()
                        {
                            UserName = "Guardian in dep 2",
                            Id = "guardian2",
                            DepartmentKey = 2
                        },
                        new GirafUser()
                        {
                            UserName = "Citizen of dep 2",
                            Id = "citizen2",
                            DepartmentKey = 2
                        },
                        new GirafUser()
                        {
                            UserName = "Citizen of dep 3",
                            Id = "citizen3",
                            DepartmentKey = 3
                        },
                        new GirafUser()
                        {
                            UserName = "Admin without Department",
                            Id = "nimda"
                        },
                        new GirafUser()
                        {
                            UserName = "Guardian 2 in dep 2",
                            Id = "guardian22",
                            DepartmentKey = 2
                        },
                        new GirafUser()
                        {
                            UserName = "Departmant in dep2",
                            Id = "department2",
                            DepartmentKey = 2
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
                    if (mockWeeks == null) {
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
                        MockUsers[3].WeekSchedule.Clear();
                    }
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
                                    MockUsers[1],
                                    MockUsers[2],
                                    MockUsers[5]
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
                            new Choice(
                                MockPictograms.Where(p => p.AccessLevel == AccessLevel.PUBLIC).Cast<Pictogram>().ToList(), 
                                "TestChoice")
                            {
                                Id = 0
                            },
                            //A private pictogram for mock user 0
                            new Choice(new List<Pictogram>() {
                                MockPictograms[3]
                            }, "TestChoice")
                            {
                                Id = 1
                            },
                            //A choice for department 0 (with id 1)
                            new Choice(new List<Pictogram>() {
                                MockPictograms[5]
                            }, "TestChoice")
                            {
                                Id = 2
                            },
                            //A choice for department 1 (with id 2)
                            new Choice(new List<Pictogram>()
                            {
                                MockPictograms[6]
                            }, "TestChoice")
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

            private List<GirafRole> mockRoles;
            public List<GirafRole> MockRoles
            {
                get
                {
                    if (mockRoles == null)
                        mockRoles = new List<GirafRole>()
                        {
                            new GirafRole(GirafRole.SuperUser) {
                                Id = GirafRole.SuperUser
                            },
                            new GirafRole(GirafRole.Guardian) {
                                Id = GirafRole.Guardian
                            },
                            new GirafRole(GirafRole.Citizen)
                            {
                                Id = GirafRole.Citizen
                            },
                            new GirafRole(GirafRole.Department)
                            {
                                Id = GirafRole.Department
                            }
                        };

                    return mockRoles;
                }
            }

            private List<IdentityUserRole<string>> mockUserRoles;
            public List<IdentityUserRole<string>> MockUserRoles {   
                get
                {
                    if (mockUserRoles == null)
                        mockUserRoles = new List<IdentityUserRole<string>>()
                        {
                            new IdentityUserRole<string> ()
                            {
                                UserId = MockUsers[0].Id,
                                RoleId = MockRoles[0].Id
                            },
                            new IdentityUserRole<string>()
                            {
                                UserId = MockUsers[1].Id,
                                RoleId = MockRoles[1].Id
                            },
                            new IdentityUserRole<string>()
                            {
                                UserId = MockUsers[2].Id,
                                RoleId = MockRoles[2].Id
                            },
                            new IdentityUserRole<string>()
                            {
                                UserId = MockUsers[3].Id,
                                RoleId = MockRoles[2].Id
                            },
                            new IdentityUserRole<string>()
                            {
                                UserId = MockUsers[6].Id,
                                RoleId = MockRoles[3].Id
                            }
                        };

                    return mockUserRoles;
                }
            }
            #endregion
            public readonly Mock<MockDbContext> MockDbContext;
            public readonly MockUserManager MockUserManager;
            public Mock<HttpContext> MockHttpContext { get; set; }
            public Mock<ILoggerFactory> MockLoggerFactory { get; private set;}

            public readonly Mock<MockRoleManager> MockRoleManager;
            

            public TestContext()
            {
                MockDbContext = CreateMockDbContext();
                MockUserManager = CreateMockUserManager(this);

                var mockLogger = new Mock<ILogger>();

                MockLoggerFactory = new Mock<ILoggerFactory>();
                MockLoggerFactory.Setup(lf => lf.CreateLogger(It.IsAny<string>()))
                    .Returns(mockLogger.Object);

                MockRoleManager = CreateMockRoleManager();
                MockRoleManager.Setup(m => m.Roles).Returns(MockRoles.AsQueryable());
;
            }

            private Mock<MockDbContext> CreateMockDbContext()
            {
                var mockSet = CreateMockDbSet(MockPictograms);
                var mockRelationSet = CreateMockDbSet(MockUserResources);
                var mockDepRes = CreateMockDbSet(MockDepartmentResources);
                var mockChoices = CreateMockDbSet(MockChoices);
                var mockDeps = CreateMockDbSet(MockDepartments);
                //var mockPF = CreateMockDbSet(MockPictograms.Cast<PictoFrame>().ToList());
                var mockUsers = CreateMockDbSet(MockUsers);
                var mockRoles = CreateMockDbSet(MockRoles);
                var mockUserRoles = CreateMockDbSet(MockUserRoles);
                var mockWeeks = CreateMockDbSet(MockWeeks);

                var dbMock = new Mock<MockDbContext>();
                dbMock.Setup(c => c.Pictograms).Returns(mockSet.Object);
                dbMock.Setup(c => c.UserResources).Returns(mockRelationSet.Object);
                dbMock.Setup(c => c.DepartmentResources).Returns(mockDepRes.Object);
                dbMock.Setup(c => c.Choices).Returns(mockChoices.Object);
                dbMock.Setup(c => c.Departments).Returns(mockDeps.Object);
                dbMock.Setup(c => c.Weeks).Returns(mockWeeks.Object);
                //dbMock.Setup(c => c.Users).Returns(mockUsers.Object);

                //Make sure that all references are setup - Entity does not handle it for us this time.
                MockUsers[0].Department = MockDepartments[0];
                MockUsers[1].Department = MockDepartments[1];

                return dbMock;
            }

            private MockUserManager CreateMockUserManager(TestContext tc)
            {
                var userStore = new Mock<IUserStore<GirafUser>>();
                var umMock = new MockUserManager(userStore.Object, tc);
                return umMock;
            }

            private Mock<MockRoleManager> CreateMockRoleManager()
            {
                var roleStore = new Mock<IRoleStore<GirafRole>>();
                var rlMock = new Mock<MockRoleManager>(roleStore.Object);
                return rlMock;
            }

            private Mock<ILoggerFactory> CreateMockLoggerFactory()
            {
                var logs = new List<string>();
                var lMock = new Mock<ILogger>();

                var lfMock = new Mock<ILoggerFactory>();
                lfMock.Setup(lf => lf.CreateLogger(It.IsAny<string>())).Returns(lMock.Object);

                return lfMock;
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