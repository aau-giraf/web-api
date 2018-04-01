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
using GirafRest.Models.Responses;

namespace GirafRest.Test
{
    public class PictogramControllerTest
    {
        private const int NEW_PICTOGRAM_ID = 400;
        private const int ADMIN_DEP_ONE = 0;
        private const int GUARDIAN_DEP_TWO = 1;
        private const int PUBLIC_PICTOGRAM = 0;
        private const int EXISTING_PICTOGRAM = 0;
        private const int ADMIN_PRIVATE_PICTOGRAM = 3;
        private const int DEP_ONE_PROTECTED_PICTOGRAM = 5;
        private const int NONEXISTING_PICTOGRAM = 999;
        private readonly string PNG_FILEPATH;
        private readonly string JPEG_FILEPATH;
        private const int GUARDIAN_DEP_ONE = 7;
        private const int CITIZEN_DEP_ONE = 8;


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
            _testContext.MockHttpContext.MockQuery("limit", int.MaxValue.ToString());
            _testContext.MockHttpContext.MockQuery("start_from", "0");

            return pc;
        }

        #region ReadPictogram(id)
        [Fact]
        public void ReadPictogram_NoLoginGetExistingPublic_Ok()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            var res = pc.ReadPictogram(PUBLIC_PICTOGRAM).Result;

            Assert.IsType<Response<PictogramDTO>>(res);
            Assert.True(res.Success);
            Assert.Equal(ErrorCode.NoError,res.ErrorCode);
            //Check data
            Assert.True(res.Data.Title == _testContext.MockPictograms.FirstOrDefault(a => a.Id == PUBLIC_PICTOGRAM)?.Title);
        }

        [Fact]
        public void ReadPictogram_LoginGetExistingPublic_Ok()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = pc.ReadPictogram(PUBLIC_PICTOGRAM).Result;

            Assert.IsType<Response<PictogramDTO>>(res);
            Assert.True(res.Success);
            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            //Check data
            Assert.True(res.Data.Title == _testContext.MockPictograms.FirstOrDefault(a => a.Id == PUBLIC_PICTOGRAM)?.Title);
        }

        [Fact]
        public void ReadPictogram_NoLoginGetExistingPrivate_Unauthorized() {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            var res = pc.ReadPictogram(ADMIN_PRIVATE_PICTOGRAM).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.UserNotFound, res.ErrorCode);
        }

        [Fact]
        public void ReadPictogram_NoLoginGetExistingProtected_Unauthorized() {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            var res = pc.ReadPictogram(DEP_ONE_PROTECTED_PICTOGRAM).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.UserNotFound, res.ErrorCode);
        }

        [Fact]
        public void ReadPictogram_LoginGetOwnPrivate_Ok() {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = pc.ReadPictogram(ADMIN_PRIVATE_PICTOGRAM).Result;

            Assert.IsType<Response<PictogramDTO>>(res);
            Assert.True(res.Success);
            // check we got the correct ressource back
            Assert.True(res.Data.Id == ADMIN_PRIVATE_PICTOGRAM);
        }

        [Fact]
        public void ReadPictogram_LoginGetProtectedInOwnDepartment_Ok() {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = pc.ReadPictogram(DEP_ONE_PROTECTED_PICTOGRAM).Result;

            Assert.IsType<Response<PictogramDTO>>(res);
            Assert.True(res.Success);
            // check we got the correct ressource back
            Assert.True(res.Data.Id == DEP_ONE_PROTECTED_PICTOGRAM);
        }

        [Fact]
        public void ReadPictogram_LoginGetProtectedInAnotherDepartment_Unauthorized() {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var res = pc.ReadPictogram(DEP_ONE_PROTECTED_PICTOGRAM).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.NotAuthorized, res.ErrorCode);
        }

        [Fact]
        public void ReadPictogram_LoginGetExistingPrivateAnotherUser_Unauthorized() {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var res = pc.ReadPictogram(ADMIN_PRIVATE_PICTOGRAM).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.NotAuthorized, res.ErrorCode);
        }

        [Fact]
        public void ReadPictogram_LoginGetNonexistingPictogram_NotFound() {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = pc.ReadPictogram(NONEXISTING_PICTOGRAM).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.PictogramNotFound, res.ErrorCode);
        }

        [Fact]
        public void ReadPictogram_NoLoginGetNonexistingPictogram_NotFound() {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            var res = pc.ReadPictogram(NONEXISTING_PICTOGRAM).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.PictogramNotFound, res.ErrorCode);
        }
        #endregion

        #region ReadPictograms()
        [Fact]
        public void ReadPictograms_NoLoginGetAll_Ok6Pictograms()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            _testContext.MockHttpContext.MockClearQueries();
            var res = pc.ReadPictograms().Result;

            Assert.IsType<Response<List<PictogramDTO>>>(res);
            Assert.True(res.Success);
            // Do we get the expected amount?
            Assert.True(_testContext.MockPictograms.Count(m => m.AccessLevel == AccessLevel.PUBLIC) == res.Data.Count);
        }

        [Fact]
        public void ReadPictograms_LoginGetAll_Ok5Pictograms()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockClearQueries();
            var res = pc.ReadPictograms("", 1, 5).Result;

            Assert.IsType<Response<List<PictogramDTO>>>(res);
            Assert.True(res.Success);
            // Check that we get exactly five pictograms back
            Assert.True(5 == res.Data.Count);
        }

        [Fact]
        public void ReadPictograms_NoLoginLongSearchQuery_Success()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            _testContext.MockHttpContext.MockClearQueries();
            // Check that the algorithm works on long queries
            var res = pc.ReadPictograms("NoTestWithoutHorsePleaseDontRaiseAnException", 1, 100).Result;

            Assert.IsType<Response<List<PictogramDTO>>>(res);
            Assert.True(res.Success);
        }

        [Fact]
        public void ReadPictograms_LoginGetAllWithValidQuery_Ok1Pictogram()
        {
            var pc = initializeTest();
            var pictTitle = "picto1";
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = pc.ReadPictograms(pictTitle,1,1).Result;

            Assert.IsType<Response<List<PictogramDTO>>>(res);
            Assert.True(res.Success);
            Assert.True(1 == res.Data.Count);
            // check that we actually got the right pictogram
            // Picto 1 is the title closest to our search query
            Assert.Equal("Picto 1", res.Data[0].Title);
        }

        [Fact]
        public void ReadPictograms_NoUser_GetClosestTwoImanges()
        {
            var pc = initializeTest();
            var pictTitle = "cat";
            var res = pc.ReadPictograms(pictTitle, 1, 2).Result;

            Assert.IsType<Response<List<PictogramDTO>>>(res);
            Assert.True(res.Success);
            Assert.True(2 == res.Data.Count);
            // We expect to get the pictograms with title cat and then cat1 and these are closest to the query
            Assert.Equal("cat", res.Data[0].Title);
            Assert.Equal("cat1", res.Data[1].Title);
        }


        #endregion

        #region Create Pictogram
        private const string pictogramName = "TP";

        [Fact]
        public void CreatePictogram_LoginValidPublicDTO_Ok()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();

            var dto = new PictogramDTO()
            {
                AccessLevel = AccessLevel.PUBLIC,
                Title = "Public " + pictogramName,
            };
            var res = pc.CreatePictogram(dto).Result;

            Assert.IsType<Response<PictogramDTO>>(res);
            Assert.True(res.Success);
            // check that title is set correctly
            Assert.Equal("Public " + pictogramName, res.Data.Title);
            // check accesslevel
            Assert.Equal(AccessLevel.PUBLIC, res.Data.AccessLevel);
        }

        [Fact]
        public void CreatePictogram_LoginValidPrivateDTO_Ok()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var dto = new PictogramDTO()
            {
                AccessLevel = AccessLevel.PRIVATE,
                Title = "Private " + pictogramName,
            };
            var res = pc.CreatePictogram(dto).Result;

            Assert.IsType<Response<PictogramDTO>>(res);
            Assert.True(res.Success);
            // check that title is set correctly
            Assert.Equal("Private " + pictogramName, res.Data.Title);
            // check accesslevel
            Assert.Equal(AccessLevel.PRIVATE, res.Data.AccessLevel);
        }

        [Fact]
        public void CreatePictogram_LoginValidProtectedDTO_Ok()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var dto = new PictogramDTO()
            {
                AccessLevel = AccessLevel.PROTECTED,
                Title = "Protected " + pictogramName,
                Id = NEW_PICTOGRAM_ID
            };
            var res = pc.CreatePictogram(dto).Result;

            Assert.IsType<Response<PictogramDTO>>(res);
            Assert.True(res.Success);
            // check that title is set correctly
            Assert.Equal("Protected " + pictogramName, res.Data.Title);
            // check accesslevel
            Assert.Equal(AccessLevel.PROTECTED, res.Data.AccessLevel);
        }

        [Fact]
        public void CreatePictogram_LoginInvalidDTO_BadRequest()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            PictogramDTO dto = null;
            var res = pc.CreatePictogram(dto).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.MissingProperties, res.ErrorCode);
        }

        [Fact]
        public void CreatePictogram_NoAccessLevel_BadRequest()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var dto = new PictogramDTO()
            {
                Title = "newpictogram",
                Id = NEW_PICTOGRAM_ID
            };
            var res = pc.CreatePictogram(dto).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.MissingProperties, res.ErrorCode);
        }

        #endregion

        #region UpdatePictogramInfo
        [Fact]
        public void UpdatePictogramInfo_NoLoginPrivate_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();

            var dto = new PictogramDTO()
            {
                Title = "Updated Pictogram",
                AccessLevel = AccessLevel.PRIVATE,
                Id = ADMIN_PRIVATE_PICTOGRAM
            };
            var res = pc.UpdatePictogramInfo(dto.Id, dto).Result;

            Assert.IsType<ErrorResponse<PictogramDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.NotAuthorized, res.ErrorCode);
        }

        [Fact]
        public void UpdatePictogramInfo_NoLoginProtected_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();

            var dto = new PictogramDTO()
            {
                Title = "Updated Pictogram",
                AccessLevel = AccessLevel.PROTECTED,
                Id = DEP_ONE_PROTECTED_PICTOGRAM
            };
            var res = pc.UpdatePictogramInfo(dto.Id, dto).Result;

            Assert.IsType<ErrorResponse<PictogramDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.NotAuthorized, res.ErrorCode);
        }

        [Fact]
        public void UpdatePictogramInfo_LoginOwnPublic_Ok()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var title = "Updated Pictogram";

            var dto = new PictogramDTO()
            {
                Title = title,
                AccessLevel = AccessLevel.PUBLIC,
            };
            var res = pc.UpdatePictogramInfo(dto.Id, dto).Result;

            Assert.IsType<Response<PictogramDTO>>(res);
            Assert.True(res.Success);
            // check that pictogram was updated correctly
            Assert.Equal(title, res.Data.Title);
            // check access level was updated correctly
            Assert.Equal(AccessLevel.PUBLIC, res.Data.AccessLevel);

        }

        [Fact]
        public void UpdatePictogramInfo_LoginOwnProtected_Ok()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var title = "Updated Pictogram";

            var dto = new PictogramDTO()
            {
                Title = title,
                AccessLevel = AccessLevel.PROTECTED,
            };
            var res = pc.UpdatePictogramInfo(dto.Id, dto).Result;

            Assert.IsType<Response<PictogramDTO>>(res);
            Assert.True(res.Success);
            // check data
            Assert.Equal(title, res.Data.Title);
            Assert.Equal(AccessLevel.PROTECTED, res.Data.AccessLevel);
        }

        [Fact]
        public void UpdatePictogramInfo_LoginOwnPrivate_Ok()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var title = "Updated Pictogram";
                
            var dto = new PictogramDTO()
            {
                Title = title,
                AccessLevel = AccessLevel.PRIVATE,
            };
            var res = pc.UpdatePictogramInfo(dto.Id, dto).Result;

            Assert.IsType<Response<PictogramDTO>>(res);
            Assert.True(res.Success);
            // check data
            Assert.Equal(title, res.Data.Title);
            Assert.Equal(AccessLevel.PRIVATE, res.Data.AccessLevel);
        }

        [Fact]
        public void UpdatePictogramInfo_LoginAnotherPrivate_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
                
            var dto = new PictogramDTO()
            {
                Title = "Updated Pictogram",
                AccessLevel = AccessLevel.PRIVATE,
                Id = ADMIN_PRIVATE_PICTOGRAM
            };
            var res = pc.UpdatePictogramInfo(dto.Id, dto).Result;

            Assert.IsType<ErrorResponse<PictogramDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.NotAuthorized, res.ErrorCode);
        }

        [Fact]
        public void UpdatePictogramInfo_LoginAnotherProtected_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var dto = new PictogramDTO()
            {
                Title = "Updated Pictogram",
                AccessLevel = AccessLevel.PRIVATE,
                Id = DEP_ONE_PROTECTED_PICTOGRAM
            };
            var res = pc.UpdatePictogramInfo(dto.Id, dto).Result;

            Assert.IsType<ErrorResponse<PictogramDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.NotAuthorized, res.ErrorCode);
        }

        [Fact]
        public void UpdatePictogramInfo_LoginNonexisting_NotFound()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var dto = new PictogramDTO()
            {
                Title = "Updated Pictogram",
                AccessLevel = AccessLevel.PRIVATE,
                Id = NONEXISTING_PICTOGRAM
            };
            var res = pc.UpdatePictogramInfo(dto.Id, dto).Result;

            Assert.IsType<ErrorResponse<PictogramDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.PictogramNotFound, res.ErrorCode);
        }

        [Fact]
        public void UpdatePictogramInfo_LoginInvalidDTO_BadRequest()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            PictogramDTO dto = null;
            var res = pc.UpdatePictogramInfo(PUBLIC_PICTOGRAM, dto).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.MissingProperties, res.ErrorCode);
        }

        [Fact]
        public void UpdatePictogramInfo_PictogramOwnerModifyAccessLevel_Ok()
        {
            // ADMIN_DEP_ONE has pictogram with id = ADMIN_PRIVATE_PICTOGRAM, it is private, update to public

            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);

            var dto = new PictogramDTO()
            {
                Title = "Updated Pictogram",
                AccessLevel = AccessLevel.PUBLIC,
                Id = ADMIN_PRIVATE_PICTOGRAM
            };
            var res = pc.UpdatePictogramInfo(dto.Id, dto).Result;

            Assert.IsType<Response<PictogramDTO>>(res);
            Assert.True(res.Success);
            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            // check that acess level is changed correctly
            Assert.Equal(AccessLevel.PUBLIC, res.Data.AccessLevel);
        }

        #endregion

        #region DeletePictogram
        [Fact]
        public void DeletePictogram_NoLoginProtected_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            var res = pc.DeletePictogram(DEP_ONE_PROTECTED_PICTOGRAM).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.UserNotFound, res.ErrorCode);
        }

        [Fact]
        public void DeletePictogram_NoLoginPrivate_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            var res = pc.DeletePictogram(ADMIN_PRIVATE_PICTOGRAM).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.UserNotFound, res.ErrorCode);
        }

        [Fact]
        public void DeletePictogram_LoginPublic_Ok()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = pc.DeletePictogram(PUBLIC_PICTOGRAM).Result;

            Assert.IsType<Response>(res);
            Assert.True(res.Success);
        }

        [Fact]
        public void DeletePictogram_LoginOwnProtected_Ok()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = pc.DeletePictogram(DEP_ONE_PROTECTED_PICTOGRAM).Result;

            Assert.IsType<Response>(res);
            Assert.True(res.Success);
        }

        [Fact]
        public void DeletePictogram_LoginOwnPrivate_Ok()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = pc.DeletePictogram(ADMIN_PRIVATE_PICTOGRAM).Result;

            Assert.IsType<Response>(res);
            Assert.True(res.Success);
        }

        [Fact]
        public void DeletePictogram_LoginAnotherProtected_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var res = pc.DeletePictogram(DEP_ONE_PROTECTED_PICTOGRAM).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.NotAuthorized, res.ErrorCode);
        }

        [Fact]
        public void DeletePictogram_LoginAnotherPrivate_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var res = pc.DeletePictogram(ADMIN_PRIVATE_PICTOGRAM).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.NotAuthorized, res.ErrorCode);
        }

        [Fact]
        public void DeletePictogram_LoginNonexisting_NotFound()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = pc.DeletePictogram(NONEXISTING_PICTOGRAM).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.PictogramNotFound, res.ErrorCode);
        }
        #endregion

        #region CreateImage
        [Fact]
        public void CreateImage_NoLoginProtected_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            var res = pc.SetPictogramImage(DEP_ONE_PROTECTED_PICTOGRAM).Result;

            Assert.False(res.Success);
            Assert.IsType<ErrorResponse<PictogramDTO>>(res);
            Assert.Equal(ErrorCode.UserNotFound, res.ErrorCode);
        }

        [Fact]
        public void CreateImage_NoLoginPrivate_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            var res = pc.SetPictogramImage(ADMIN_PRIVATE_PICTOGRAM).Result;

            Assert.False(res.Success);
            Assert.IsType<ErrorResponse<PictogramDTO>>(res);
            Assert.Equal(ErrorCode.UserNotFound, res.ErrorCode);
        }

        [Fact]
        public void CreateImage_LoginPublic_Ok()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            var res = pc.SetPictogramImage(PUBLIC_PICTOGRAM).Result;

            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            Assert.IsType<Response<PictogramDTO>>(res);
            Assert.True(res.Success);
            Assert.True(res.Data.Image != null);

            byte[] testImageAsByes = File.ReadAllBytes(PNG_FILEPATH);
            // Check that the images is equivalent
            Assert.Equal(testImageAsByes, res.Data.Image);

        }

        [Fact]
        public void CreateImage_LoginPrivate_Ok()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            var res = pc.SetPictogramImage(ADMIN_PRIVATE_PICTOGRAM).Result;

            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            Assert.IsType<Response<PictogramDTO>>(res);
            Assert.True(res.Success);
            Assert.True(res.Data.Image != null);

            byte[] testImageAsByes = File.ReadAllBytes(PNG_FILEPATH);
            // Check that the images is equivalent
            Assert.Equal(testImageAsByes, res.Data.Image);
        }

        [Fact]
        public void CreateImage_LoginProtected_Ok()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);

            var res = pc.SetPictogramImage(DEP_ONE_PROTECTED_PICTOGRAM).Result;

            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            Assert.IsType<Response<PictogramDTO>>(res);
            Assert.True(res.Success);
            Assert.True(res.Data.Image != null);

            byte[] testImageAsByes = File.ReadAllBytes(PNG_FILEPATH);
            // Check that the images is equivalent
            Assert.Equal(testImageAsByes, res.Data.Image);
        }

        [Fact]
        public void CreateImage_LoginAnotherProtected_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            var res = pc.SetPictogramImage(DEP_ONE_PROTECTED_PICTOGRAM).Result;

            Assert.IsType<ErrorResponse<PictogramDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.NotAuthorized, res.ErrorCode);
        }

        [Fact]
        public void CreateImage_LoginAnotherPrivate_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            var res = pc.SetPictogramImage(ADMIN_PRIVATE_PICTOGRAM).Result;

            Assert.IsType<ErrorResponse<PictogramDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.NotAuthorized, res.ErrorCode);
        }

        [Fact]
        public void CreateImage_LoginNonexisting_NotFound()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            var res = pc.SetPictogramImage(NONEXISTING_PICTOGRAM).Result;

            Assert.IsType<ErrorResponse<PictogramDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.PictogramNotFound, res.ErrorCode);
        }

        [Fact]
        public void CreateImage_PublicJpeg_Ok()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(JPEG_FILEPATH);

            var res = pc.SetPictogramImage(PUBLIC_PICTOGRAM).Result;

            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            Assert.IsType<Response<PictogramDTO>>(res);
            Assert.True(res.Success);
            Assert.True(res.Data.Image != null);

            byte[] testImageAsByes = File.ReadAllBytes(JPEG_FILEPATH);
            // Check that the images is equivalent
            Assert.Equal(testImageAsByes, res.Data.Image);
        }

        #endregion

        #region UpdatePictogramImage
        [Fact]
        public void UpdatePictogramImage_NoLoginProtected_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);

            var img = pc.SetPictogramImage(DEP_ONE_PROTECTED_PICTOGRAM).Result;

            Assert.IsType<Response<PictogramDTO>>(img);
            Assert.True(img.Success);
            Assert.Equal(ErrorCode.NoError, img.ErrorCode);

            _testContext.MockUserManager.MockLogout();
            var res = pc.SetPictogramImage(DEP_ONE_PROTECTED_PICTOGRAM).Result;

            Assert.IsType<ErrorResponse<PictogramDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.UserNotFound, res.ErrorCode);
        }

        [Fact]
        public void UpdatePictogramImage_NoLoginPrivate_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);

            var img = pc.SetPictogramImage(ADMIN_PRIVATE_PICTOGRAM);

            _testContext.MockUserManager.MockLogout();
            var res = pc.SetPictogramImage(ADMIN_PRIVATE_PICTOGRAM).Result;

            Assert.IsType<ErrorResponse<PictogramDTO>>(res);
            Assert.False(res.Success);
            Assert.Equal(ErrorCode.UserNotFound, res.ErrorCode);
        }

        [Fact]
        public void UpdatePictogramImage_LoginPublic_Ok()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            var res = pc.SetPictogramImage(PUBLIC_PICTOGRAM).Result;

            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            Assert.IsType<Response<PictogramDTO>>(res);
            Assert.True(res.Success);
            Assert.True(res.Data.Image != null);

            byte[] testImageAsByes = File.ReadAllBytes(PNG_FILEPATH);
            // Check that the images is equivalent
            Assert.Equal(testImageAsByes, res.Data.Image);
        }


        [Fact]
        public void UpdatePictogramImage_LoginPrivate_Ok()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            var res = pc.SetPictogramImage(ADMIN_PRIVATE_PICTOGRAM).Result;

            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            Assert.IsType<Response<PictogramDTO>>(res);
            Assert.True(res.Success);
            Assert.True(res.Data.Image != null);

            byte[] testImageAsByes = File.ReadAllBytes(PNG_FILEPATH);
            // Check that the images is equivalent
            Assert.Equal(testImageAsByes, res.Data.Image);
        }


        [Fact]
        public void UpdatePictogramImage_LoginProtected_Ok()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            var res = pc.SetPictogramImage(ADMIN_PRIVATE_PICTOGRAM).Result;

            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            Assert.IsType<Response<PictogramDTO>>(res);
            Assert.True(res.Success);
            Assert.True(res.Data.Image != null);

            byte[] testImageAsByes = File.ReadAllBytes(PNG_FILEPATH);
            // Check that the images is equivalent
            Assert.Equal(testImageAsByes, res.Data.Image);
        }


        [Fact]
        public void UpdatePictogramImage_LoginAnotherPrivate_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            var img = pc.SetPictogramImage(ADMIN_PRIVATE_PICTOGRAM);

            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var res = pc.SetPictogramImage(ADMIN_PRIVATE_PICTOGRAM).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.NotAuthorized, res.ErrorCode);
        }


        [Fact]
        public void UpdatePictogramImage_LoginAnotherProtected_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            var img = pc.SetPictogramImage(DEP_ONE_PROTECTED_PICTOGRAM);
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var res = pc.SetPictogramImage(DEP_ONE_PROTECTED_PICTOGRAM).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.NotAuthorized, res.ErrorCode);
        }


        [Fact]
        public void UpdatePictogramImage_LoginNullBody_BadRequest()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            var img = pc.SetPictogramImage(PUBLIC_PICTOGRAM);
            _testContext.MockHttpContext.MockRequestNoImage();
            var res = pc.SetPictogramImage(PUBLIC_PICTOGRAM).Result;

            Assert.True(res.Success);
        }


        [Fact]
        public void UpdatePictogramImage_LoginNonexisting_NotFound()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            var res = pc.SetPictogramImage(NONEXISTING_PICTOGRAM).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.PictogramNotFound, res.ErrorCode);
        }

        [Fact]
        public void UpdatePictogramImage_PublicJpegToJpeg_Ok()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(JPEG_FILEPATH);
            var img = pc.SetPictogramImage(PUBLIC_PICTOGRAM).Result;
            img = pc.SetPictogramImage(PUBLIC_PICTOGRAM).Result; /*The test will check if it is possible to update
            an pictogram that already has an image*/
            _testContext.MockHttpContext.MockRequestImage(JPEG_FILEPATH);

            var res = pc.SetPictogramImage(PUBLIC_PICTOGRAM).Result;

            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            Assert.IsType<Response<PictogramDTO>>(res);
            Assert.True(res.Success);
            Assert.Equal(ErrorCode.NoError, res.ErrorCode);

            byte[] testImageAsByes = File.ReadAllBytes(JPEG_FILEPATH);
            // Check that the images is equivalent
            Assert.Equal(testImageAsByes, res.Data.Image);
        }

        [Fact]
        public void UpdatePictogramImage_PublicPngToJpeg_Ok()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);

            var img = pc.SetPictogramImage(PUBLIC_PICTOGRAM).Result;

            byte[] pngTestImage = File.ReadAllBytes(PNG_FILEPATH);
            // Check that we have correctly updated the image to a PNG image
            Assert.Equal(pngTestImage, img.Data.Image);

            img = pc.SetPictogramImage(PUBLIC_PICTOGRAM).Result;

            _testContext.MockHttpContext.MockRequestImage(JPEG_FILEPATH);

            var res = pc.SetPictogramImage(PUBLIC_PICTOGRAM).Result;

            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            Assert.IsType<Response<PictogramDTO>>(res);
            Assert.True(res.Success);
            Assert.Equal(ErrorCode.NoError, res.ErrorCode);

            byte[] testImageAsByes = File.ReadAllBytes(JPEG_FILEPATH);
            // Check that we have correctly updated the png image to the jpeg image
            Assert.Equal(testImageAsByes, res.Data.Image);
        }

        #endregion

        #region ReadPictogramImage
        [Fact]
        public void ReadPictogramImage_NoLoginProtected_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            var img = pc.SetPictogramImage(DEP_ONE_PROTECTED_PICTOGRAM).Result;

            _testContext.MockUserManager.MockLogout();
            var res = pc.ReadPictogramImage(DEP_ONE_PROTECTED_PICTOGRAM).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.NotAuthorized, res.ErrorCode);
        }
        
        [Fact]
        public void ReadPictogramImage_NoLoginPrivate_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            var img = pc.SetPictogramImage(ADMIN_PRIVATE_PICTOGRAM).Result;

            _testContext.MockUserManager.MockLogout();
            var res = pc.ReadPictogramImage(ADMIN_PRIVATE_PICTOGRAM).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.NotAuthorized, res.ErrorCode);
        }
        
        [Fact]
        public void ReadPictogramImage_LoginPublic_Ok()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            var img = pc.SetPictogramImage(PUBLIC_PICTOGRAM).Result;
            var res = pc.ReadPictogramImage(PUBLIC_PICTOGRAM).Result;

            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            Assert.True(res.Success);
            Assert.True(res.Data != null);

            byte[] testImageAsByes = File.ReadAllBytes(PNG_FILEPATH);
            // Check that we have correctly read the image - that is that the byte arrays of the expected and actual image is equal
            Assert.Equal(testImageAsByes, res.Data);
        }
        
        [Fact]
        public void ReadPictogramImage_LoginProtected_Ok()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            var img = pc.SetPictogramImage(DEP_ONE_PROTECTED_PICTOGRAM);
            var res = pc.ReadPictogramImage(DEP_ONE_PROTECTED_PICTOGRAM).Result;

            Assert.Equal(ErrorCode.NoError, res.ErrorCode);
            Assert.True(res.Success);
            Assert.True(res.Data != null);

            byte[] testImageAsByes = File.ReadAllBytes(PNG_FILEPATH);
            // Check that we have correctly read the image - that is that the byte arrays of the expected and actual image is equal
            Assert.Equal(testImageAsByes, res.Data);
        }
        
        [Fact]
        public void ReadPictogramImage_LoginAnotherPrivate_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            var img = pc.SetPictogramImage(ADMIN_PRIVATE_PICTOGRAM);
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var res = pc.ReadPictogramImage(ADMIN_PRIVATE_PICTOGRAM).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.NotAuthorized, res.ErrorCode);
        }

        [Fact]
        public void ReadPictogramImage_LoginAnotherProtected_Unauhtorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            var img = pc.SetPictogramImage(DEP_ONE_PROTECTED_PICTOGRAM);
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var res = pc.ReadPictogramImage(DEP_ONE_PROTECTED_PICTOGRAM).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.NotAuthorized, res.ErrorCode);
        }

        [Fact]
        public void ReadPictogramImage_LoginPublicNoImage_ErrorPictogramHasNoImage()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = pc.ReadPictogramImage(PUBLIC_PICTOGRAM).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.PictogramHasNoImage, res.ErrorCode);
        }

        [Fact]
        public void ReadPictogramImage_LoginNonexisting_NotFound()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var img = pc.SetPictogramImage(NONEXISTING_PICTOGRAM);
            var res = pc.ReadPictogramImage(NONEXISTING_PICTOGRAM).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.PictogramNotFound, res.ErrorCode);
        }

        [Fact]
        public void ReadPictogramImage_GetPublicJpeg_NotAuthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockUserManager.MockLogout();
            _testContext.MockHttpContext.MockRequestImage(JPEG_FILEPATH);
            var img = pc.SetPictogramImage(PUBLIC_PICTOGRAM);
            var res = pc.ReadPictogramImage(PUBLIC_PICTOGRAM).Result;

            Assert.False(res.Success);
            Assert.Equal(ErrorCode.NotAuthorized, res.ErrorCode);
        }

        [Fact]
        public void ReadRawPictogramImage_GetPrivate_OK()
        {
            try
            {
                var pc = initializeTest();
                _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
                var res = pc.ReadRawPictogramImage(ADMIN_PRIVATE_PICTOGRAM);

                Assert.True(res.IsCompleted);
                Assert.IsType<FileContentResult>(res.Result);

                var fileContent = ((FileContentResult) res.Result);
                
                // Get the expected image an convert to eight bit int array so we can compare with actual returned image
                Assert.Equal("image/png", fileContent.ContentType);
                var testImageTo8BitIntArray = Convert.FromBase64String(System.Text.Encoding.UTF8.GetString(_testContext
                    .MockPictograms
                    .FirstOrDefault(mp => mp.Id == ADMIN_PRIVATE_PICTOGRAM)?.Image));
                // Check that we read the correct image
                Assert.Equal(testImageTo8BitIntArray, fileContent.FileContents);
            }
            catch (Exception)
            {
                Assert.True(false);
            }
        }

        [Fact]
        public void ReadRawPictogramImage_GetPublic_OK()
        {
            try
            {
                var pc = initializeTest();
                _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
                var res = pc.ReadRawPictogramImage(DEP_ONE_PROTECTED_PICTOGRAM);

                Assert.True(res.IsCompleted);
                Assert.IsType<FileContentResult>(res.Result);

                var fileContent = ((FileContentResult)res.Result);

                // Get the expected image an convert to eight bit int array so we can compare with actual returned image
                Assert.Equal("image/png", fileContent.ContentType);
                var testImageTo8BitIntArray = Convert.FromBase64String(System.Text.Encoding.UTF8.GetString(_testContext
                    .MockPictograms
                    .FirstOrDefault(mp => mp.Id == DEP_ONE_PROTECTED_PICTOGRAM)?.Image));
                // Check that we read the correct image
                Assert.Equal(testImageTo8BitIntArray, fileContent.FileContents);
            }
            catch(Exception)
            {
                Assert.True(false);
            }
        }

        [Fact]
        public void ReadRawPictogramImage_NoLoginPrivate_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockUserManager.MockLogout();
            var res = pc.ReadRawPictogramImage(ADMIN_PRIVATE_PICTOGRAM).Result;
            
            Assert.IsType<NotFoundResult>(res);
        }

        #endregion

        #region FilterByTitle
        [Theory]
        [InlineData("PUBLIC", 2)]
        [InlineData("", 7)]
        [InlineData("YYY", 0)]
        [InlineData("Pr", 4)]
        [InlineData("Pu", 2)]
        [InlineData("P", 6)]
        public void FilterByTitle(string query, int expectedPictograms) {
            //var pc = initializeTest();
            
            //var res = pc.FilterByTitle(_testContext.MockPictograms.AsQueryable(), query);

            //Assert.Equal(expectedPictograms, res.ToList().Count);
        }
        #endregion

        #region Helpers
        private List<PictogramDTO> convertToListAndLogTestOutput(OkObjectResult result)
        {
            var list = result.Value as List<PictogramDTO>;
            list.ForEach(p => _testLogger.WriteLine(p.Title));

            return list;
        }
        #endregion
    }
}
