using System.Linq;
using Xunit;
using Moq;
using GirafRest.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using GirafRest.Controllers;
using System;
using Xunit.Abstractions;
using GirafRest.Test.Mocks;
using static GirafRest.Test.UnitTestExtensions;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using GirafRest.Models.DTOs;
using System.IO;

namespace GirafRest.Test
{
    public class PictogramControllerTest
    {
        private const int PUBLIC_PICTOGRAM = 0;
        private const int PRIVATE_PICTOGRAM_USER0 = 3;
        private const int PROTECTED_PICTOGRAM_USER0 = 5;
        private const int NONEXISTING_PICTOGRAM = 999;
        private readonly string PNG_FILEPATH;
        private readonly string JPEG_FILEPATH;

        private TestContext _testContext;
        
        private readonly ITestOutputHelper _testLogger;

        public PictogramControllerTest(ITestOutputHelper output)
        {
            _testLogger = output;
            PNG_FILEPATH = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "Mocks", "MockImage.png");
            JPEG_FILEPATH = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "Mocks", "MockImage.jpeg");
        }

        private PictogramController initializeTest()
        {
            _testContext = new TestContext();

            var pc = new PictogramController(
                new MockGirafService(_testContext.MockDbContext.Object,
                _testContext.MockUserManager), _testContext.MockLoggerFactory.Object);
            _testContext.MockHttpContext = pc.MockHttpContext();

            return pc;
        }

        #region ReadPictogram(id)
        [Fact]
        public void GetExistingPublic_NoLogin_ExpectOK()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            
            var res = pc.ReadPictogram(PUBLIC_PICTOGRAM);
            IActionResult aRes = res.Result;

            Assert.IsType<OkObjectResult>(aRes);
        }

        [Fact]
        public void GetExistingPublic_Login_ExpectOK()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            
            var res = pc.ReadPictogram(PUBLIC_PICTOGRAM);
            IActionResult aRes = res.Result;

            Assert.IsType<OkObjectResult>(aRes);
        }

        [Fact]
        public void GetExistingPrivate_NoLogin_ExpectUnauthorized() {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();

            var res = pc.ReadPictogram(PRIVATE_PICTOGRAM_USER0);
            IActionResult aRes = res.Result;

            Assert.IsType<UnauthorizedResult>(aRes);
        }

        [Fact]
        public void GetExistingProtected_NoLogin_ExpectUnauthorized() {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();

            var res = pc.ReadPictogram(PROTECTED_PICTOGRAM_USER0);
            IActionResult aRes = res.Result;

            Assert.IsType<UnauthorizedResult>(aRes);
        }

        [Fact]
        public void GetOwnPrivate_Login_ExpectOK() {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);

            var res = pc.ReadPictogram(PRIVATE_PICTOGRAM_USER0);
            IActionResult aRes = res.Result;

            Assert.IsType<OkObjectResult>(aRes);
        }

        [Fact]
        public void GetProtectedInOwnDepartment_Login_ExpectOK() {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            var res = pc.ReadPictogram(PROTECTED_PICTOGRAM_USER0).Result;

            if(res is ObjectResult)
            {
                var uRes = res as ObjectResult;
                _testLogger.WriteLine(uRes.Value.ToString());
            }

            Assert.IsType<OkObjectResult>(res);
        }

        [Fact]
        public void GetProtectedInAnotherDepartment_Login_ExpectUnauthorized() {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[1]);
            var tRes = pc.ReadPictogram(PROTECTED_PICTOGRAM_USER0);
            var res = tRes.Result;

            if (res is ObjectResult)
            {
                var uRes = res as ObjectResult;
                _testLogger.WriteLine(uRes.Value.ToString());
            }

            Assert.IsType<UnauthorizedResult>(res);
        }

        [Fact]
        public void GetExistingPrivateAnotherUser_Login_ExpectUnauthorized() {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[1]);
            var res = pc.ReadPictogram(PRIVATE_PICTOGRAM_USER0).Result;

            Assert.IsType<UnauthorizedResult>(res);
        }

        [Fact]
        public void GetNonexistingPictogram_Login_ExpectNotFound() {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);

            var res = pc.ReadPictogram(NONEXISTING_PICTOGRAM);
            var pRes = res.Result;
            Assert.IsAssignableFrom<NotFoundResult>(pRes);
        }

        [Fact]
        public void GetNonexistingPictogram_NoLogin_ExpectNotFound() {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();

            var res = pc.ReadPictogram(NONEXISTING_PICTOGRAM).Result;

            Assert.IsAssignableFrom<NotFoundResult>(res);
        }
        #endregion
        #region ReadPictograms()
        [Fact]
        public void GetAll_NoLogin_ExpectOk3Pictograms()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            _testContext.MockHttpContext.MockClearQueries();

            var res = pc.ReadPictograms().Result;
            var resList = convertToListAndLogTestOutput(res as OkObjectResult);

            Assert.True(3 == resList.Count);
        }

        [Fact]
        public void GetAll_Login_ExpectOk5Pictograms()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockClearQueries();

            var res = pc.ReadPictograms().Result;
            var resList = convertToListAndLogTestOutput(res as OkObjectResult);

            Assert.True(5 == resList.Count);
        }

        [Fact]
        public void GetAllWithValidQuery_NoLogin_ExpectOk1Pictogram()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            _testContext.MockHttpContext.MockQuery("title", "picto1");

            var res = pc.ReadPictograms().Result;
            var resList = convertToListAndLogTestOutput(res as OkObjectResult);

            Assert.True(1 == resList.Count);
        }

        [Fact]
        public void GetAllWithInvalidQuery_NoLogin_ExpectNotFound()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            _testContext.MockHttpContext.MockQuery("title", "invalid");

            var res = pc.ReadPictograms().Result;

            if (res is OkObjectResult)
            {
                convertToListAndLogTestOutput(res as OkObjectResult);
            }

            Assert.IsType<NotFoundResult>(res);
        }

        [Fact]
        public void GetAllWithValidQuery_Login_ExpectOk1Pictogram()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockQuery("title", "picto1");

            var res = pc.ReadPictograms().Result;

            var resList = convertToListAndLogTestOutput(res as OkObjectResult);

            Assert.True(1 == resList.Count);
        }

        [Fact]
        public void GetAllWithInvalidQuery_Login_ExpectNotFound()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[1]);
            _testContext.MockHttpContext.MockQuery("title", "invalid");

            var res = pc.ReadPictograms().Result;

            if (res is OkObjectResult)
            {
                convertToListAndLogTestOutput(res as OkObjectResult);
            }
            if(res is BadRequestObjectResult)
            {
                _testLogger.WriteLine((res as BadRequestObjectResult).Value.ToString());
            }

            Assert.IsType<NotFoundResult>(res);
        }

        [Fact]
        public void GetAllWithValidQueryOnAnotherUsersPrivate_Login_ExpectNotFound()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockQuery("title", "user 1");

            var res = pc.ReadPictograms().Result;

            if (res is OkObjectResult)
            {
                convertToListAndLogTestOutput(res as OkObjectResult);
            }

            Assert.IsType<NotFoundResult>(res);
        }

        [Fact]
        public void GetAllWithValidQueryOnPrivate_NoLogin_ExpectNotFound()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            _testContext.MockHttpContext.MockQuery("title", "user 1");

            var res = pc.ReadPictograms().Result;

            if (res is OkObjectResult)
            {
                convertToListAndLogTestOutput(res as OkObjectResult);
            }

            Assert.IsType<NotFoundResult>(res);
        }

        private List<PictogramDTO> convertToListAndLogTestOutput(OkObjectResult result)
        {
            var list = result.Value as List<PictogramDTO>;
            list.ForEach(p => _testLogger.WriteLine(p.Title));

            return list;
        }
        #endregion
        #region Create Pictogram
        private const string pictogramName = "TP";

        [Fact]
        public void CreatePictogram_LoginValidDTOPublic_ExpectOK()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();

            var dto = new PictogramDTO()
            {
                AccessLevel = AccessLevel.PUBLIC,
                Title = "Public " + pictogramName,
                Id = 400
            };

            var res = pc.CreatePictogram(dto).Result;
            Assert.IsType<OkObjectResult>(res);
        }

        [Fact]
        public void CreatePictogram_LoginValidDTOPrivate_ExpectOK()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[1]);

            var dto = new PictogramDTO()
            {
                AccessLevel = AccessLevel.PRIVATE,
                Title = "Private " + pictogramName,
                Id = 400
            };

            var res = pc.CreatePictogram(dto).Result;
            Assert.IsType<OkObjectResult>(res);
        }

        [Fact]
        public void CreatePictogram_LoginValidDTOProtected_ExpectOK()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[1]);

            var dto = new PictogramDTO()
            {
                AccessLevel = AccessLevel.PROTECTED,
                Title = "Protected " + pictogramName,
                Id = 400
            };

            var res = pc.CreatePictogram(dto).Result;

            _testLogger.WriteLine(((res as OkObjectResult).Value as PictogramDTO).Id.ToString());

            Assert.IsType<OkObjectResult>(res);
        }

        [Fact]
        public void CreatePictogram_LoginInvalidDTO_ExpectBadRequest()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[1]);

            PictogramDTO dto = null;

            var res = pc.CreatePictogram(dto).Result;

            if(res is BadRequestObjectResult)
            {
                _testLogger.WriteLine((res as BadRequestObjectResult).Value.ToString());
            }

            Assert.IsType<BadRequestObjectResult>(res);
        }
        #endregion
        #region UpdatePictogramInfo
        [Fact]
        public void Update_NoLoginPrivate_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();

            var dto = new PictogramDTO()
            {
                Title = "Updated Pictogram",
                AccessLevel = AccessLevel.PRIVATE,
                Id = PRIVATE_PICTOGRAM_USER0
            };

            var res = pc.UpdatePictogramInfo(dto).Result;
            if(res is ObjectResult)
                _testLogger.WriteLine((res as ObjectResult).Value.ToString());

            Assert.IsType<UnauthorizedResult>(res);
        }

        [Fact]
        public void Update_NoLoginProtected_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();

            var dto = new PictogramDTO()
            {
                Title = "Updated Pictogram",
                AccessLevel = AccessLevel.PROTECTED,
                Id = PROTECTED_PICTOGRAM_USER0
            };

            var res = pc.UpdatePictogramInfo(dto).Result;

            if(res is ObjectResult)
                _testLogger.WriteLine((res as ObjectResult).Value.ToString());

            Assert.IsType<UnauthorizedResult>(res);
        }

        [Fact]
        public void Update_LoginPublic_Ok()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);

            var dto = new PictogramDTO()
            {
                Title = "Updated Pictogram",
                AccessLevel = AccessLevel.PUBLIC,
                Id = PUBLIC_PICTOGRAM
            };

            var res = pc.UpdatePictogramInfo(dto).Result;

            if (res is ObjectResult)
                _testLogger.WriteLine((res as ObjectResult).Value.ToString());

            Assert.IsType<OkObjectResult>(res);
        }

        [Fact]
        public void Update_LoginOwnProtected_Ok()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);

            var dto = new PictogramDTO()
            {
                Title = "Updated Pictogram",
                AccessLevel = AccessLevel.PROTECTED,
                Id = PROTECTED_PICTOGRAM_USER0
            };

            var res = pc.UpdatePictogramInfo(dto).Result;

            _testLogger.WriteLine((res as ObjectResult).Value.ToString());

            Assert.IsType<OkObjectResult>(res);
        }

        [Fact]
        public void Update_LoginOwnPrivate_Ok()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);

            var dto = new PictogramDTO()
            {
                Title = "Updated Pictogram",
                AccessLevel = AccessLevel.PRIVATE,
                Id = PRIVATE_PICTOGRAM_USER0
            };
            
            var res = pc.UpdatePictogramInfo(dto).Result;

            if(res is ObjectResult)
                _testLogger.WriteLine((res as ObjectResult).Value.ToString());

            Assert.IsType<OkObjectResult>(res);
        }

        [Fact]
        public void Update_LoginAnotherPrivate_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[1]);

            var dto = new PictogramDTO()
            {
                Title = "Updated Pictogram",
                Id = PRIVATE_PICTOGRAM_USER0
            };

            var res = pc.UpdatePictogramInfo(dto).Result;

            Assert.IsType<UnauthorizedResult>(res);
        }

        [Fact]
        public void Update_LoginAnotherProtected_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[1]);

            var dto = new PictogramDTO()
            {
                Title = "Updated Pictogram",
                Id = PROTECTED_PICTOGRAM_USER0
            };

            var res = pc.UpdatePictogramInfo(dto).Result;

            Assert.IsType<UnauthorizedResult>(res);
        }

        [Fact]
        public void Update_LoginNonexisting_NotFound()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[1]);

            var dto = new PictogramDTO()
            {
                Title = "Updated Pictogram",
                Id = NONEXISTING_PICTOGRAM
            };

            var res = pc.UpdatePictogramInfo(dto).Result;

            if(res is ObjectResult)
                _testLogger.WriteLine((res as ObjectResult).Value.ToString());

            Assert.IsAssignableFrom<NotFoundResult>(res);
        }

        public void Update_LoginInvalidDTO_BadRequest()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[1]);

            PictogramDTO dto = null;

            var res = pc.UpdatePictogramInfo(dto).Result;

            _testLogger.WriteLine((res as ObjectResult).Value.ToString());

            Assert.IsType<BadRequestObjectResult>(res);
        }
        #endregion
        #region DeletePictogram
        [Fact]
        public void Delete_NoLoginProtected_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();

            var res = pc.DeletePictogram(PROTECTED_PICTOGRAM_USER0).Result;

            if (res is ObjectResult)
                _testLogger.WriteLine((res as ObjectResult).Value.ToString());

            Assert.IsType<UnauthorizedResult>(res);
        }

        [Fact]
        public void Delete_NoLoginPrivate_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();

            var res = pc.DeletePictogram(PRIVATE_PICTOGRAM_USER0).Result;

            if (res is ObjectResult)
                _testLogger.WriteLine((res as ObjectResult).Value.ToString());

            Assert.IsType<UnauthorizedResult>(res);
        }

        [Fact]
        public void Delete_LoginPublic_Ok()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);

            var res = pc.DeletePictogram(PUBLIC_PICTOGRAM).Result;

            if (res is ObjectResult)
                _testLogger.WriteLine((res as ObjectResult).Value.ToString());

            Assert.IsType<OkResult>(res);
        }

        [Fact]
        public void Delete_LoginOwnProtected_Ok()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);

            var res = pc.DeletePictogram(PROTECTED_PICTOGRAM_USER0).Result;

            if (res is ObjectResult)
                _testLogger.WriteLine((res as ObjectResult).Value.ToString());

            Assert.IsType<OkResult>(res);
        }

        [Fact]
        public void Delete_LoginOwnPrivate_Ok()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);

            var res = pc.DeletePictogram(PRIVATE_PICTOGRAM_USER0).Result;

            if (res is ObjectResult)
                _testLogger.WriteLine((res as ObjectResult).Value.ToString());

            Assert.IsType<OkResult>(res);
        }

        [Fact]
        public void Delete_LoginAnotherProtected_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[1]);

            var res = pc.DeletePictogram(PROTECTED_PICTOGRAM_USER0).Result;

            if (res is ObjectResult)
                _testLogger.WriteLine((res as ObjectResult).Value.ToString());

            Assert.IsType<UnauthorizedResult>(res);
        }

        [Fact]
        public void Delete_LoginAnotherPrivate_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[1]);

            var res = pc.DeletePictogram(PRIVATE_PICTOGRAM_USER0).Result;

            if (res is ObjectResult)
                _testLogger.WriteLine((res as ObjectResult).Value.ToString());

            Assert.IsType<UnauthorizedResult>(res);
        }

        [Fact]
        public void Delete_LoginNonexisting_NotFound()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);

            var res = pc.DeletePictogram(NONEXISTING_PICTOGRAM).Result;

            if (res is ObjectResult)
                _testLogger.WriteLine((res as ObjectResult).Value.ToString());

            Assert.IsAssignableFrom<NotFoundResult>(res);
        }
        #endregion
        #region CreateImage
        [Fact]
        public void CreateImage_NoLoginProtected_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);

            var res = pc.CreateImage(PROTECTED_PICTOGRAM_USER0).Result;

            if (res is ObjectResult)
                _testLogger.WriteLine((res as ObjectResult).Value.ToString());

            Assert.IsType<UnauthorizedResult>(res);
        }

        [Fact]
        public void CreateImage_NoLoginPrivate_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);

            var res = pc.CreateImage(PRIVATE_PICTOGRAM_USER0).Result;

            if (res is ObjectResult)
                _testLogger.WriteLine((res as ObjectResult).Value.ToString());

            Assert.IsType<UnauthorizedResult>(res);
        }

        [Fact]
        public void CreateImage_LoginPublic_Ok()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);

            var res = pc.CreateImage(PUBLIC_PICTOGRAM).Result;

            if (res is ObjectResult)
                _testLogger.WriteLine((res as ObjectResult).Value.ToString());

            Assert.IsType<OkObjectResult>(res);
        }

        [Fact]
        public void CreateImage_LoginPrivate_Ok()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);

            var res = pc.CreateImage(PRIVATE_PICTOGRAM_USER0).Result;

            if (res is ObjectResult)
                _testLogger.WriteLine((res as ObjectResult).Value.ToString());

            Assert.IsType<OkObjectResult>(res);
        }
        
        [Fact]
        public void CreateImage_LoginProtected_Ok()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);

            var res = pc.CreateImage(PROTECTED_PICTOGRAM_USER0).Result;

            if (res is ObjectResult)
                _testLogger.WriteLine((res as ObjectResult).Value.ToString());

            Assert.IsType<OkObjectResult>(res);
        }

        [Fact]
        public void CreateImage_LoginAnotherProtected_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[1]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);

            var res = pc.CreateImage(PROTECTED_PICTOGRAM_USER0).Result;

            if (res is ObjectResult)
                _testLogger.WriteLine((res as ObjectResult).Value.ToString());

            Assert.IsType<UnauthorizedResult>(res);
        }

        [Fact]
        public void CreateImage_LoginAnotherPrivate_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[1]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);

            var res = pc.CreateImage(PRIVATE_PICTOGRAM_USER0).Result;

            if (res is ObjectResult)
                _testLogger.WriteLine((res as ObjectResult).Value.ToString());

            Assert.IsType<UnauthorizedResult>(res);
        }

        [Fact]
        public void CreateImage_LoginNonexisting_NotFound()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);

            var res = pc.CreateImage(NONEXISTING_PICTOGRAM).Result;

            if (res is ObjectResult)
                _testLogger.WriteLine((res as ObjectResult).Value.ToString());

            Assert.IsAssignableFrom<NotFoundResult>(res);
        }

        [Fact]
        public void CreateImage_LoginPublicNullBody_BadRequest()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockRequestNoImage();

            var res = pc.CreateImage(PUBLIC_PICTOGRAM).Result;

            if (res is ObjectResult)
                _testLogger.WriteLine((res as ObjectResult).Value.ToString());

            Assert.IsType<BadRequestObjectResult>(res);
        }

        [Fact]
        public void CreateImage_LoginPublicExistingImage_BadRequest()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);

            pc.CreateImage(0);
            var res = pc.CreateImage(0).Result;

            if (res is ObjectResult)
                _testLogger.WriteLine((res as ObjectResult).Value.ToString());

            Assert.IsType<BadRequestObjectResult>(res);
        }

        [Fact]
        public void CreateImage_PublicJpeg_Ok()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            _testContext.MockHttpContext.MockRequestImage(JPEG_FILEPATH);

            var res = pc.CreateImage(PUBLIC_PICTOGRAM).Result;

            if (res is ObjectResult)
                _testLogger.WriteLine((res as ObjectResult).Value.ToString());

            Assert.IsType<OkObjectResult>(res);
        }

        #endregion
        #region UpdatePictogramImage
        [Fact]
        public void UpdateImage_NoLoginProtected_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);

            pc.CreateImage(PROTECTED_PICTOGRAM_USER0);

            _testContext.MockUserManager.MockLogout();
            var res = pc.UpdatePictogramImage(PROTECTED_PICTOGRAM_USER0).Result;

            if (res is ObjectResult)
                _testLogger.WriteLine((res as ObjectResult).Value.ToString());
            Assert.IsType<UnauthorizedResult>(res);
        }

        [Fact]
        public void UpdateImage_NoLoginPrivate_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);

            pc.CreateImage(PRIVATE_PICTOGRAM_USER0);

            _testContext.MockUserManager.MockLogout();
            var res = pc.UpdatePictogramImage(PRIVATE_PICTOGRAM_USER0).Result;

            if (res is ObjectResult)
                _testLogger.WriteLine((res as ObjectResult).Value.ToString());
            Assert.IsType<UnauthorizedResult>(res);
        }

        [Fact]
        public void UpdateImage_LoginPublic_Ok()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);

            pc.CreateImage(PUBLIC_PICTOGRAM);
            
            var res = pc.UpdatePictogramImage(PUBLIC_PICTOGRAM).Result;

            if (res is ObjectResult)
                _testLogger.WriteLine((res as ObjectResult).Value.ToString());
            Assert.IsType<OkObjectResult>(res);
        }


        [Fact]
        public void UpdateImage_LoginPrivate_Ok()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);

            pc.CreateImage(PRIVATE_PICTOGRAM_USER0);

            var res = pc.UpdatePictogramImage(PRIVATE_PICTOGRAM_USER0).Result;

            if (res is ObjectResult)
                _testLogger.WriteLine((res as ObjectResult).Value.ToString());
            Assert.IsType<OkObjectResult>(res);
        }


        [Fact]
        public void UpdateImage_LoginProtected_Ok()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);

            pc.CreateImage(PROTECTED_PICTOGRAM_USER0);

            var res = pc.UpdatePictogramImage(PROTECTED_PICTOGRAM_USER0).Result;

            if (res is ObjectResult)
                _testLogger.WriteLine((res as ObjectResult).Value.ToString());
            Assert.IsType<OkObjectResult>(res);
        }


        [Fact]
        public void UpdateImage_LoginAnotherPrivate_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);

            pc.CreateImage(PRIVATE_PICTOGRAM_USER0);

            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[1]);
            var res = pc.UpdatePictogramImage(PRIVATE_PICTOGRAM_USER0).Result;

            if (res is ObjectResult)
                _testLogger.WriteLine((res as ObjectResult).Value.ToString());
            Assert.IsType<UnauthorizedResult>(res);
        }


        [Fact]
        public void UpdateImage_LoginAnotherProtected_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);

            pc.CreateImage(PROTECTED_PICTOGRAM_USER0);

            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[1]);
            var res = pc.UpdatePictogramImage(PROTECTED_PICTOGRAM_USER0).Result;

            if (res is ObjectResult)
                _testLogger.WriteLine((res as ObjectResult).Value.ToString());
            Assert.IsType<UnauthorizedResult>(res);
        }


        [Fact]
        public void UpdateImage_LoginNullBody_BadRequest()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);

            pc.CreateImage(PUBLIC_PICTOGRAM);

            _testContext.MockHttpContext.MockRequestNoImage();
            var res = pc.UpdatePictogramImage(PUBLIC_PICTOGRAM).Result;

            if (res is ObjectResult)
                _testLogger.WriteLine((res as ObjectResult).Value.ToString());
            Assert.IsType<OkObjectResult>(res);
        }


        [Fact]
        public void UpdateImage_LoginNoImage_BadRequest()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            
            var res = pc.UpdatePictogramImage(PRIVATE_PICTOGRAM_USER0).Result;

            if (res is ObjectResult)
                _testLogger.WriteLine((res as ObjectResult).Value.ToString());
            Assert.IsType<BadRequestObjectResult>(res);
        }


        [Fact]
        public void UpdateImage_LoginNonexisting_NotFound()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            
            var res = pc.UpdatePictogramImage(NONEXISTING_PICTOGRAM).Result;

            if (res is ObjectResult)
                _testLogger.WriteLine((res as ObjectResult).Value.ToString());
            Assert.IsType<NotFoundResult>(res);
        }

        [Fact]
        public void UpdateImage_PublicJpegToJpeg_Ok()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            _testContext.MockHttpContext.MockRequestImage(JPEG_FILEPATH);

            pc.CreateImage(PUBLIC_PICTOGRAM);

            pc.UpdatePictogramImage(PUBLIC_PICTOGRAM);

            _testContext.MockHttpContext.MockRequestImage(JPEG_FILEPATH);

            var res = pc.UpdatePictogramImage(PUBLIC_PICTOGRAM).Result;

            if (res is ObjectResult)
                _testLogger.WriteLine((res as ObjectResult).Value.ToString());
            Assert.IsType<OkObjectResult>(res);
        }

        [Fact]
        public void UpdateImage_PublicPngToJpeg_Ok()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);

            pc.CreateImage(PUBLIC_PICTOGRAM);

            pc.UpdatePictogramImage(PUBLIC_PICTOGRAM);

            _testContext.MockHttpContext.MockRequestImage(JPEG_FILEPATH);

            var res = pc.UpdatePictogramImage(PUBLIC_PICTOGRAM).Result;

            if (res is ObjectResult)
                _testLogger.WriteLine((res as ObjectResult).Value.ToString());
            Assert.IsType<OkObjectResult>(res);
        }

        #endregion
        #region ReadPictogramImage
        [Fact]
        public void ReadImage_NoLoginProtected_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            pc.CreateImage(PROTECTED_PICTOGRAM_USER0);

            _testContext.MockUserManager.MockLogout();
            var res = pc.ReadPictogramImage(PROTECTED_PICTOGRAM_USER0).Result;

            Assert.IsType<UnauthorizedResult>(res);
        }
        
        [Fact]
        public void ReadImage_NoLoginPrivate_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            pc.CreateImage(PRIVATE_PICTOGRAM_USER0);

            _testContext.MockUserManager.MockLogout();
            var res = pc.ReadPictogramImage(PRIVATE_PICTOGRAM_USER0).Result;

            Assert.IsType<UnauthorizedResult>(res);
        }
        
        [Fact]
        public void ReadImage_LoginPublic_Ok()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            pc.CreateImage(PUBLIC_PICTOGRAM);
            
            var res = pc.ReadPictogramImage(PUBLIC_PICTOGRAM).Result;

            Assert.IsType<FileContentResult>(res);
        }
        
        [Fact]
        public void ReadImage_LoginProtected_Ok()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            pc.CreateImage(PROTECTED_PICTOGRAM_USER0);

            var res = pc.ReadPictogramImage(PROTECTED_PICTOGRAM_USER0).Result;

            Assert.IsType<FileContentResult>(res);
        }
        
        [Fact]
        public void ReadImage_LoginAnotherPrivate_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            pc.CreateImage(PRIVATE_PICTOGRAM_USER0);
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[1]);

            var res = pc.ReadPictogramImage(PRIVATE_PICTOGRAM_USER0).Result;

            Assert.IsType<UnauthorizedResult>(res);
        }

        [Fact]
        public void ReadImage_LoginAnotherProtected_Unauhtorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            pc.CreateImage(PROTECTED_PICTOGRAM_USER0);
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[1]);

            var res = pc.ReadPictogramImage(PROTECTED_PICTOGRAM_USER0).Result;

            Assert.IsType<UnauthorizedResult>(res);
        }

        [Fact]
        public void ReadImage_LoginPublicNoImage_NotFound()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);

            var res = pc.ReadPictogramImage(PUBLIC_PICTOGRAM).Result;

            if (res is ObjectResult)
                _testLogger.WriteLine((res as ObjectResult).Value.ToString());

            Assert.IsType<NotFoundObjectResult>(res);
        }

        [Fact]
        public void ReadImage_LoginNonexisting_NotFound()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[0]);
            pc.CreateImage(NONEXISTING_PICTOGRAM);

            var res = pc.ReadPictogramImage(NONEXISTING_PICTOGRAM).Result;

            if (res is ObjectResult)
                _testLogger.WriteLine((res as ObjectResult).Value.ToString());

            Assert.IsType<NotFoundObjectResult>(res);
        }

        [Fact]
        public void ReadImage_GetPublicJpeg_ok()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            _testContext.MockHttpContext.MockRequestImage(JPEG_FILEPATH);
            pc.CreateImage(PUBLIC_PICTOGRAM);

            var res = pc.ReadPictogramImage(PUBLIC_PICTOGRAM).Result;

            Assert.IsType<FileContentResult>(res);
        }

        #endregion
        #region FilterByTitle

        [Fact]
        public void FilterByTitle_QueryForP_6Pictograms()
        {
            var pc = initializeTest();
            string query = "P";

            var res = pc.FilterByTitle(_testContext.MockPictograms, query);

            Assert.Equal(6, res.Count);
        }


        [Fact]
        public void FilterByTitle_QueryForPu_2Pictograms()
        {
            var pc = initializeTest();
            string query = "Pu";

            var res = pc.FilterByTitle(_testContext.MockPictograms, query);

            Assert.Equal(2, res.Count);
        }


        [Fact]
        public void FilterByTitle_QueryForPr_4Pictograms()
        {
            var pc = initializeTest();
            string query = "Pr";

            var res = pc.FilterByTitle(_testContext.MockPictograms, query);

            Assert.Equal(4, res.Count);
        }


        [Fact]
        public void FilterByTitle_QueryForYYY_0Pictograms()
        {
            var pc = initializeTest();
            string query = "YYY";

            var res = pc.FilterByTitle(_testContext.MockPictograms, query);

            Assert.Equal(0, res.Count);
        }


        [Fact]
        public void FilterByTitle_QueryForEmptyString_0Pictograms()
        {
            var pc = initializeTest();
            string query = "";

            var res = pc.FilterByTitle(_testContext.MockPictograms, query);

            Assert.Equal(0, res.Count);
        }


        [Fact]
        public void FilterByTitle_QueryForPUBLIC_2Pictograms()
        {
            var pc = initializeTest();
            string query = "PUBLIC";

            var res = pc.FilterByTitle(_testContext.MockPictograms, query);

            Assert.Equal(2, res.Count);
        }

        #endregion
    }
}
