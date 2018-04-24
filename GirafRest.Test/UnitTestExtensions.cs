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
using System.Text;

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
            private const int DepartmentTwoUser = 6;
            private List<Pictogram> _mockPictograms;

            #region MockPictogramData
            public byte[] en = Encoding.ASCII.GetBytes("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");
            public byte[] epik = Encoding.ASCII.GetBytes("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");

            #endregion


            public List<Pictogram> MockPictograms
            {
                get
                {
                    if (_mockPictograms == null)
                        _mockPictograms = new List<Pictogram> {
                        new Pictogram("Picto 1", AccessLevel.PUBLIC) {
                            Id = 0
                        },
                        new Pictogram("Public Picto2", AccessLevel.PUBLIC) {
                            Id = 1
                        },
                        new Pictogram("No restrictions", AccessLevel.PUBLIC) {
                            Id = 2
                        },
                        new Pictogram("Private for user 0", AccessLevel.PRIVATE, en) {
                            Id = 3
                        },
                        new Pictogram("Private for user 1", AccessLevel.PRIVATE) {
                            Id = 4
                        },
                        new Pictogram("Protected for Dep 1", AccessLevel.PROTECTED, epik)
                        {
                            Id = 5
                        },
                        new Pictogram("Protected for Dep 2", AccessLevel.PROTECTED) {
                            Id = 6
                        },
                        new Pictogram("cat", AccessLevel.PUBLIC) {
                            Id = 7
                        },
                        new Pictogram("cap", AccessLevel.PUBLIC) {
                            Id = 8
                        },
                        new Pictogram("cat1", AccessLevel.PUBLIC) {
                            Id = 9
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
            public const int UserCitizenDepartment1 = 8;
            
            private List<GirafUser> mockUsers = null;
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
                            DepartmentKey = 2,
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
                            DepartmentKey = 2,
                        },
                        new GirafUser()
                        {
                            UserName = "Guardian 3 in dep 3",
                            Id = "guardian3",
                            DepartmentKey = 1
                        },
                        new GirafUser()
                        {
                            UserName = "Citizen of dep 1",
                            Id = "citizen4",
                            DepartmentKey = 1
                        }
                    };

                    return mockUsers;
                }
            }

            private List<GuardianRelation> mockGuardianRelations;
            public List<GuardianRelation> MockGuardianRelations
            {
                get
                {
                    if(mockGuardianRelations == null)
                    {
                        mockGuardianRelations = new List<GuardianRelation>()
                        {
                            new GuardianRelation()
                            {
                                Guardian = MockUsers[1],
                                Citizen = MockUsers[2],
                                GuardianId = MockUsers[1].Id,
                                CitizenId = MockUsers[2].Id
                            }
                        };

                        MockUsers[1].Citizens.Add(mockGuardianRelations[0]);
                        MockUsers[2].Guardians.Add(mockGuardianRelations[0]);
                    }
                    return mockGuardianRelations;
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
                                Id = 0,
                                WeekYear = 2018,
                                WeekNumber = 1,
                                Name = "My awesome week",
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
                                Id = 1,
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
                                    MockUsers[3],
                                    MockUsers[5]
                                }
                            }
                        };
                    if (mockUsers != null) { 
                        mockUsers[DepartmentTwoUser].Department = mockDepartments[1];
                    }
                    return mockDepartments;
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

            private List<Activity> mockWeekDayRessources;
            public IReadOnlyList<Activity> MockWeekDayRessources
            {
                get
                {
                    if (mockWeekDayRessources == null)
                        mockWeekDayRessources = new List<Activity>()
                        {
                            new Activity(MockWeeks[0].Weekdays[0], MockPictograms[5], 0),
                            new Activity(MockWeeks[0].Weekdays[1], MockPictograms[6], 1)
                        };

                    return mockWeekDayRessources;
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
                            },
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
                var mockWeekDayRes = CreateMockDbSet(MockWeekDayRessources);
                var mockDeps = CreateMockDbSet(MockDepartments);
                //var mockPF = CreateMockDbSet(MockPictograms.Cast<PictoFrame>().ToList());
                var mockUsers = CreateMockDbSet(MockUsers);
                var mockRoles = CreateMockDbSet(MockRoles);
                var mockUserRoles = CreateMockDbSet(MockUserRoles);
                var mockWeeks = CreateMockDbSet(MockWeeks);
                var mockGuardianRelations = CreateMockDbSet(MockGuardianRelations);
                var dbMock = new Mock<MockDbContext>();
                dbMock.Setup(c => c.Pictograms).Returns(mockSet.Object);
                dbMock.Setup(c => c.UserResources).Returns(mockRelationSet.Object);
                dbMock.Setup(c => c.DepartmentResources).Returns(mockDepRes.Object);
                dbMock.Setup(c => c.Activities).Returns(mockWeekDayRes.Object);
                dbMock.Setup(c => c.Departments).Returns(mockDeps.Object);
                dbMock.Setup(c => c.Weeks).Returns(mockWeeks.Object);
                dbMock.Setup(c => c.Users).Returns(mockUsers.Object);
                dbMock.Setup(c => c.Roles).Returns(mockRoles.Object);
                dbMock.Setup(c => c.UserRoles).Returns(mockUserRoles.Object);

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