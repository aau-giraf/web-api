/*
using System.Linq;
using Xunit;
using GirafRest.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using GirafRest.Controllers;
using Xunit.Abstractions;
using GirafRest.Test.Mocks;
using static GirafRest.Test.UnitTestExtensions;
using GirafRest.Models.DTOs;
using System.IO;
using GirafRest.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting.Internal;

namespace GirafRest.Test
{   
    public class PictogramControllerTest
    {
#pragma warning disable IDE0051 // Remove unused private members
        private const int NEW_PICTOGRAM_ID = 400;
        private const int ADMIN_DEP_ONE = 0;
        private const int GUARDIAN_DEP_TWO = 1;
        private const int PUBLIC_PICTOGRAM = 2;
        private const int EXISTING_PICTOGRAM = 0;
        private const int ADMIN_PRIVATE_PICTOGRAM = 3;
        private const int DEP_ONE_PROTECTED_PICTOGRAM = 5;
        private const int NONEXISTING_PICTOGRAM = 999;
        private readonly string PNG_FILEPATH;
        private readonly string JPEG_FILEPATH;
        private const int GUARDIAN_DEP_ONE = 7;
        private const int CITIZEN_DEP_ONE = 8;
        private const int ADMIN_NO_DEP = 4;
#pragma warning restore IDE0051 // Remove unused private members


        private HostingEnvironment _hostEnv;
        private TestContext _testContext;
        private string _pictogramFolderPath;
        private const string _pathToPictogramFolder = "/../pictograms/";
        
        public PictogramControllerTest()
        {
            PNG_FILEPATH = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "Mocks", "MockImage.png");
            JPEG_FILEPATH = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "Mocks", "MockImage.jpeg");
            _hostEnv = new HostingEnvironment();
            _hostEnv.ContentRootPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.ToString();
            _pictogramFolderPath = _hostEnv.ContentRootPath + _pathToPictogramFolder;
            
            // This folder is used to mock the file store of the production server.
            if (!Directory.Exists(_pictogramFolderPath))
            {
                Directory.CreateDirectory(_pictogramFolderPath);
            }
        }

        private PictogramController initializeTest()
        {
            _testContext = new TestContext();
           
            var pc = new PictogramController(
                new MockGirafService(
                    _testContext.MockDbContext.Object,
                    _testContext.MockUserManager
                ), 
                _testContext.MockLoggerFactory.Object,
                _hostEnv
                );
            _testContext.MockHttpContext = pc.MockHttpContext();
            _testContext.MockHttpContext.MockQuery("limit", int.MaxValue.ToString());
            _testContext.MockHttpContext.MockQuery("start_from", "0");
            _preparePictogramFolder(ADMIN_DEP_ONE);
            _preparePictogramFolder(PUBLIC_PICTOGRAM);
            _preparePictogramFolder(GUARDIAN_DEP_TWO);
            _preparePictogramFolder(ADMIN_PRIVATE_PICTOGRAM);
            _preparePictogramFolder(DEP_ONE_PROTECTED_PICTOGRAM);
            return pc;
        }

        #region ReadPictogram(id)
        [Fact]
        public void ReadPictogram_NoLoginGetExistingPublic_Success()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            var res = pc.ReadPictogram(PUBLIC_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as SuccessResponse<WeekPictogramDTO>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            Assert.IsType<OkObjectResult>(res);
            //Check data
            Assert.True(body.Data.Title == _testContext.MockPictograms.FirstOrDefault(a => a.Id == PUBLIC_PICTOGRAM)?.Title);
        }

        [Fact]
        public void ReadPictogram_LoginGetExistingPublic_Success()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = pc.ReadPictogram(PUBLIC_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as SuccessResponse<WeekPictogramDTO>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            Assert.IsType<OkObjectResult>(res);
            //Check data
            Assert.True(body.Data.Title == _testContext.MockPictograms.FirstOrDefault(a => a.Id == PUBLIC_PICTOGRAM)?.Title);
        }

        [Fact]
        public void ReadPictogram_NoLoginGetExistingPrivate_UserNotFound() {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            var res = pc.ReadPictogram(ADMIN_PRIVATE_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status401Unauthorized, res.StatusCode);
            Assert.Equal(ErrorCode.UserNotFound, body.ErrorCode);
        }

        [Fact]
        public void ReadPictogram_NoLoginGetExistingProtected_UserNotFound() {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            var res = pc.ReadPictogram(DEP_ONE_PROTECTED_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status401Unauthorized, res.StatusCode);
            Assert.Equal(ErrorCode.UserNotFound, body.ErrorCode);
        }

        [Fact]
        public void ReadPictogram_LoginGetOwnPrivate_Success() {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = pc.ReadPictogram(ADMIN_PRIVATE_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as SuccessResponse<WeekPictogramDTO>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            // check we got the correct ressource back
            Assert.True(body.Data.Id == ADMIN_PRIVATE_PICTOGRAM);
        }

        [Fact]
        public void ReadPictogram_LoginGetProtectedInOwnDepartment_Success() {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = pc.ReadPictogram(DEP_ONE_PROTECTED_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as SuccessResponse<WeekPictogramDTO>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            // check we got the correct ressource back
            Assert.True(body.Data.Id == DEP_ONE_PROTECTED_PICTOGRAM);
        }

        [Fact]
        public void ReadPictogram_LoginGetProtectedInAnotherDepartment_Unauthorized() {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var res = pc.ReadPictogram(DEP_ONE_PROTECTED_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
            Assert.Equal(ErrorCode.NotAuthorized, body.ErrorCode);
        }

        [Fact]
        public void ReadPictogram_LoginGetExistingPrivateAnotherUser_Unauthorized() {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var res = pc.ReadPictogram(ADMIN_PRIVATE_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
            Assert.Equal(ErrorCode.NotAuthorized, body.ErrorCode);
        }

        [Fact]
        public void ReadPictogram_LoginGetNonexistingPictogram_NotFound() {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = pc.ReadPictogram(NONEXISTING_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status404NotFound, res.StatusCode);
            Assert.Equal(ErrorCode.PictogramNotFound, body.ErrorCode);
        }

        [Fact]
        public void ReadPictogram_NoLoginGetNonexistingPictogram_NotFound() {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            var res = pc.ReadPictogram(NONEXISTING_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status404NotFound, res.StatusCode);
            Assert.Equal(ErrorCode.PictogramNotFound, body.ErrorCode);
        }
        #endregion

        #region ReadPictograms()
        [Fact]
        public void ReadPictograms_NoLoginGetAll_Success()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            _testContext.MockHttpContext.MockClearQueries();
            var res = pc.ReadPictograms(null, 1, 20).Result as ObjectResult;
            var body = res.Value as SuccessResponse<List<WeekPictogramDTO>>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            // Check that all resulting pictograms are public
            Assert.True(_testContext.MockPictograms.Count(m => m.AccessLevel == AccessLevel.PUBLIC) == body.Data.Count);
        }

        [Fact]
        public void ReadPictograms_LoginGetAll_Success()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockClearQueries();
            var res = pc.ReadPictograms("", 1, 5).Result as ObjectResult;
            var body = res.Value as SuccessResponse<List<WeekPictogramDTO>>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            // Check that we get exactly five pictograms back
            Assert.True(5 == body.Data.Count);
        }

        [Fact]
        public void ReadPictograms_NoLoginLongSearchQuery_Success()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            _testContext.MockHttpContext.MockClearQueries();
            // Check that the algorithm works on long queries
            var res = pc.ReadPictograms("NoTestWithoutHorsePleaseDontRaiseAnException", 1, 100).Result as ObjectResult;
            var body = res.Value as SuccessResponse<List<WeekPictogramDTO>>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
        }

        [Fact]
        public void ReadPictograms_LoginGetAllWithValidQuery_Success()
        {
            var pc = initializeTest();
            var pictTitle = "picto1";
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = pc.ReadPictograms(pictTitle,1,1).Result as ObjectResult;
            var body = res.Value as SuccessResponse<List<WeekPictogramDTO>>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            Assert.True(1 == body.Data.Count);
            // check that we actually got the right pictogram
            // Picto 1 is the title closest to our search query
            Assert.Equal("Picto 1", body.Data[0].Title);
        }

        [Fact]
        public void ReadPictograms_NoUser_Success()
        {
            var pc = initializeTest();
            var pictTitle = "cat";
            var res = pc.ReadPictograms(pictTitle, 1, 2).Result as ObjectResult;
            var body = res.Value as SuccessResponse<List<WeekPictogramDTO>>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            Assert.True(2 == body.Data.Count);
            // We expect to get the pictograms with title cat and then cat1 and these are closest to the query
            Assert.Equal("cat", body.Data[0].Title);
            Assert.Equal("cat1", body.Data[1].Title);
        }
        
        [Fact]
        public void ReadPictograms_LoginWithNoDepartment_Success()
        {
            // When the user has no department, the search should not return any protected picotograms
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_NO_DEP]);
            var res = pc.ReadPictograms("", 1, 100).Result as ObjectResult;
            var body = res.Value as SuccessResponse<List<WeekPictogramDTO>>;

            // Determine if there are any Protected pictograms in the result
            bool result = body.Data.Any(pictogram => pictogram.AccessLevel == AccessLevel.PROTECTED);
            
            // Expect there to be no protected pictograms in the result
            Assert.False(result);
        }

        [Fact]
        public void ReadPictograms_NoUserSearchIsNotCaseSensitive_Success()
        {
            // Testing that upper or lower case letters do not change the result of the search
            /*
             *  The Mocked pictograms relevant for this test are titled:
             *  "casesensitive",
             *  "CASESENSITIVE1"
             *
             *  If the search ignores whether letters are written in capital then the search should always
             *  produce the same order.
             #1#
            var pc = initializeTest();
            const string lowercaseSearchQuery = "casesensitive";
            var resForLowercase = pc.ReadPictograms(lowercaseSearchQuery, 1, 2).Result as ObjectResult;
            var bodyForLowercase = resForLowercase.Value as SuccessResponse<List<WeekPictogramDTO>>;

            const string uppercaseSearchQuery = "CASESENSITIVE";
            var resForUppercase = pc.ReadPictograms(uppercaseSearchQuery, 1, 2).Result as ObjectResult;
            var bodyForUppercase = resForUppercase.Value as SuccessResponse<List<WeekPictogramDTO>>;
            
            // As there are only two results, testing that the first element is the same should be sufficient
            Assert.Equal(bodyForLowercase.Data[0].Title, bodyForUppercase.Data[0].Title);
        }
        
        [Fact]
        public void ReadPictograms_SearchResultInCorrectOrder_Success()
        {
            // Testing that upper or lower case letters do not change the result of the search
            /*
             *  The Mocked pictograms relevant for this test are titled:
             *  "primus",
             *  "mus"
             *
             *  If the search ignores whether letters are written in capital then the search should always
             *  produce the same order.
             #1#
            var pc = initializeTest();
            const string startsWithQuery = "m";
            var resForStartsWithQuery = pc.ReadPictograms(startsWithQuery, 1, 2).Result as ObjectResult;
            var bodyForStartsWithQuery = resForStartsWithQuery.Value as SuccessResponse<List<WeekPictogramDTO>>;

            // As there are only two results, testing that the first element is the same should be sufficient
            Assert.True(bodyForStartsWithQuery.Data[0].Title== "mus" && bodyForStartsWithQuery.Data[1].Title== "primus");
        }

        [Fact]
        public void ReadPictograms_InvalidPageSize_InvalidProperties()
        {
            var pc = initializeTest();
            var res = pc.ReadPictograms("", 1, 0).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal(ErrorCode.InvalidProperties, body.ErrorCode);
        }
        
        [Fact]
        public void ReadPictograms_InvalidNumberOfPages_InvalidProperties()
        {
            var pc = initializeTest();
            var res = pc.ReadPictograms("", 0, 10).Result as ObjectResult;
            var body = res.Value as ErrorResponse;


            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal(ErrorCode.InvalidProperties, body.ErrorCode);
        }

        #endregion

        #region Create Pictogram
        private const string pictogramName = "TP";

        [Fact]
        public void CreatePictogram_LoginValidPublicDTO_Success()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var dto = new PictogramDTO()
            {
                AccessLevel = AccessLevel.PUBLIC,
                Title = "Public " + pictogramName,
            };
            var res = pc.CreatePictogram(dto).Result as ObjectResult;
            var body = res.Value as SuccessResponse<WeekPictogramDTO>;

            Assert.Equal(StatusCodes.Status201Created, res.StatusCode);
            // check that title is set correctly
            Assert.Equal("Public " + pictogramName, body.Data.Title);
            // check accesslevel
            Assert.Equal(AccessLevel.PUBLIC, body.Data.AccessLevel);
        }

        [Fact]
        public void CreatePictogram_LoginValidPrivateDTO_Success()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var dto = new PictogramDTO()
            {
                AccessLevel = AccessLevel.PRIVATE,
                Title = "Private " + pictogramName,
            };
            var res = pc.CreatePictogram(dto).Result as ObjectResult;
            var body = res.Value as SuccessResponse<WeekPictogramDTO>;

            Assert.Equal(StatusCodes.Status201Created, res.StatusCode);
            // check that title is set correctly
            Assert.Equal("Private " + pictogramName, body.Data.Title);
            // check accesslevel
            Assert.Equal(AccessLevel.PRIVATE, body.Data.AccessLevel);
        }

        [Fact]
        public void CreatePictogram_LoginValidProtectedDTO_Success()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var dto = new PictogramDTO()
            {
                AccessLevel = AccessLevel.PROTECTED,
                Title = "Protected " + pictogramName
            };
            var res = pc.CreatePictogram(dto).Result as ObjectResult;
            var body = res.Value as SuccessResponse<WeekPictogramDTO>;

            Assert.Equal(StatusCodes.Status201Created, res.StatusCode);
            // check that title is set correctly
            Assert.Equal("Protected " + pictogramName, body.Data.Title);
            // check accesslevel
            Assert.Equal(AccessLevel.PROTECTED, body.Data.AccessLevel);
        }

        [Fact]
        public void CreatePictogram_LoginInvalidDTO_MissingProperties()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            PictogramDTO dto = null;
            var res = pc.CreatePictogram(dto).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal(ErrorCode.MissingProperties, body.ErrorCode);
        }

        [Fact]
        public void CreatePictogram_NoAccessLevel_MissingProperties()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var dto = new PictogramDTO()
            {
                Title = "newpictogram"
            };
            var res = pc.CreatePictogram(dto).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal(ErrorCode.MissingProperties, body.ErrorCode);
        }

        #endregion

        #region UpdatePictogramInfo
        [Fact]
        public void UpdatePictogramInfo_NoLoginPrivate_NotFound()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();

            var dto = new PictogramDTO()
            {
                Title = "Updated Pictogram",
                AccessLevel = AccessLevel.PRIVATE
            };
            var res = pc.UpdatePictogramInfo(ADMIN_PRIVATE_PICTOGRAM, dto).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status401Unauthorized, res.StatusCode);
            Assert.Equal(ErrorCode.UserNotFound, body.ErrorCode);
        }

        [Fact]
        public void UpdatePictogramInfo_NoLoginProtected_NotFound()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();

            var dto = new PictogramDTO()
            {
                Title = "Updated Pictogram",
                AccessLevel = AccessLevel.PROTECTED
            };
            var res = pc.UpdatePictogramInfo(DEP_ONE_PROTECTED_PICTOGRAM, dto).Result as ObjectResult;
            var body = res.Value as ErrorResponse;


            Assert.Equal(StatusCodes.Status401Unauthorized, res.StatusCode);
            Assert.Equal(ErrorCode.UserNotFound, body.ErrorCode);
        }

        [Fact]
        public void UpdatePictogramInfo_LoginOwnPublic_Success()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var title = "Updated Pictogram";

            var dto = new PictogramDTO()
            {
                Title = title,
                AccessLevel = AccessLevel.PUBLIC,
            };
            var res = pc.UpdatePictogramInfo(dto.Id, dto).Result as ObjectResult;
            var body = res.Value as SuccessResponse<WeekPictogramDTO>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            // check that pictogram was updated correctly
            Assert.Equal(title, body.Data.Title);
            // check access level was updated correctly
            Assert.Equal(AccessLevel.PUBLIC, body.Data.AccessLevel);

        }

        [Fact]
        public void UpdatePictogramInfo_LoginOwnProtected_Success()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var title = "Updated Pictogram";

            var dto = new PictogramDTO()
            {
                Title = title,
                AccessLevel = AccessLevel.PROTECTED,
            };
            var res = pc.UpdatePictogramInfo(dto.Id, dto).Result as ObjectResult;
            var body = res.Value as SuccessResponse<WeekPictogramDTO>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            // check data
            Assert.Equal(title, body.Data.Title);
            Assert.Equal(AccessLevel.PROTECTED, body.Data.AccessLevel);
        }

        [Fact]
        public void UpdatePictogramInfo_LoginOwnPrivate_Success()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var title = "Updated Pictogram";
                
            var dto = new PictogramDTO()
            {
                Title = title,
                AccessLevel = AccessLevel.PRIVATE,
            };
            var res = pc.UpdatePictogramInfo(dto.Id, dto).Result as ObjectResult;
            var body = res.Value as SuccessResponse<WeekPictogramDTO>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            // check data
            Assert.Equal(title, body.Data.Title);
            Assert.Equal(AccessLevel.PRIVATE, body.Data.AccessLevel);
        }

        [Fact]
        public void UpdatePictogramInfo_LoginAnotherPrivate_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
                
            var dto = new PictogramDTO()
            {
                Title = "Updated Pictogram",
                AccessLevel = AccessLevel.PRIVATE
            };
            var res = pc.UpdatePictogramInfo(ADMIN_PRIVATE_PICTOGRAM, dto).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
            Assert.Equal(ErrorCode.NotAuthorized, body.ErrorCode);
        }

        [Fact]
        public void UpdatePictogramInfo_LoginAnotherProtected_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var dto = new PictogramDTO()
            {
                Title = "Updated Pictogram",
                AccessLevel = AccessLevel.PRIVATE
            };
            var res = pc.UpdatePictogramInfo(DEP_ONE_PROTECTED_PICTOGRAM, dto).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
            Assert.Equal(ErrorCode.NotAuthorized, body.ErrorCode);
        }

        [Fact]
        public void UpdatePictogramInfo_LoginNonexisting_NotFound()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);

            var dto = new PictogramDTO()
            {
                Title = "Updated Pictogram",
                AccessLevel = AccessLevel.PRIVATE
            };
            var res = pc.UpdatePictogramInfo(NONEXISTING_PICTOGRAM, dto).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status404NotFound, res.StatusCode);
            Assert.Equal(ErrorCode.PictogramNotFound, body.ErrorCode);
        }

        [Fact]
        public void UpdatePictogramInfo_LoginInvalidDTO_MissingProperties()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            PictogramDTO dto = null;
            var res = pc.UpdatePictogramInfo(PUBLIC_PICTOGRAM, dto).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.Equal(ErrorCode.MissingProperties, body.ErrorCode);
        }

        [Fact]
        public void UpdatePictogramInfo_PictogramOwnerModifyAccessLevel_Success()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);

            var dto = new PictogramDTO()
            {
                Title = "Updated Pictogram",
                AccessLevel = AccessLevel.PUBLIC
            };
            var res = pc.UpdatePictogramInfo(ADMIN_PRIVATE_PICTOGRAM, dto).Result as ObjectResult;
            var body = res.Value as SuccessResponse<WeekPictogramDTO>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            // check that acess level is changed correctly
            Assert.Equal(AccessLevel.PUBLIC, body.Data.AccessLevel);
        }

        #endregion

        #region DeletePictogram
        [Fact]
        public void DeletePictogram_NoLoginProtected_UserNotFound()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            var res = pc.DeletePictogram(DEP_ONE_PROTECTED_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as ErrorResponse ;

            Assert.Equal(StatusCodes.Status401Unauthorized, res.StatusCode);
            Assert.Equal(ErrorCode.UserNotFound, body.ErrorCode);
        }

        [Fact]
        public void DeletePictogram_NoLoginPrivate_UserNotFound()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            var res = pc.DeletePictogram(ADMIN_PRIVATE_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status401Unauthorized, res.StatusCode);
            Assert.Equal(ErrorCode.UserNotFound, body.ErrorCode);
        }

        [Fact]
        public void DeletePictogram_LoginPublic_Success()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = pc.DeletePictogram(PUBLIC_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as SuccessResponse;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
        }

        [Fact]
        public void DeletePictogram_LoginOwnProtected_Success()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = pc.DeletePictogram(DEP_ONE_PROTECTED_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as SuccessResponse;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
        }

        [Fact]
        public void DeletePictogram_LoginOwnPrivate_Success()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = pc.DeletePictogram(ADMIN_PRIVATE_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as SuccessResponse;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
        }

        [Fact]
        public void DeletePictogram_LoginAnotherProtected_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var res = pc.DeletePictogram(DEP_ONE_PROTECTED_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
            Assert.Equal(ErrorCode.NotAuthorized, body.ErrorCode);
        }

        [Fact]
        public void DeletePictogram_LoginAnotherPrivate_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var res = pc.DeletePictogram(ADMIN_PRIVATE_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
            Assert.Equal(ErrorCode.NotAuthorized, body.ErrorCode);
        }

        [Fact]
        public void DeletePictogram_LoginNonexisting_NotFound()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = pc.DeletePictogram(NONEXISTING_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status404NotFound, res.StatusCode);
            Assert.Equal(ErrorCode.PictogramNotFound, body.ErrorCode);
        }
        #endregion

        #region CreateImage
        [Fact]
        public void CreateImage_NoLoginProtected_UserNotFound()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            var res = pc.SetPictogramImage(DEP_ONE_PROTECTED_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status401Unauthorized, res.StatusCode);
            Assert.Equal(ErrorCode.UserNotFound, body.ErrorCode);
        }

        [Fact]
        public void CreateImage_NoLoginPrivate_UserNotFound()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLogout();
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            var res = pc.SetPictogramImage(ADMIN_PRIVATE_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status401Unauthorized, res.StatusCode);
            Assert.Equal(ErrorCode.UserNotFound, body.ErrorCode);
        }

        [Fact]
        public void CreateImage_LoginPublic_Success()
        {
            var pc = initializeTest();
            
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            
            var res = pc.SetPictogramImage(PUBLIC_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as SuccessResponse<WeekPictogramDTO>;
            
            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            Assert.NotNull(body.Data);

            byte[] testImageAsBytes = File.ReadAllBytes(PNG_FILEPATH);
            byte[] actualImage = _loadPictogramFromDisk(PUBLIC_PICTOGRAM);
            // Check that the images is equivalent
            Assert.Equal(testImageAsBytes, actualImage);

        }

        [Fact]
        public void CreateImage_LoginPrivate_Success()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            var res = pc.SetPictogramImage(ADMIN_PRIVATE_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as SuccessResponse<WeekPictogramDTO>; 

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            Assert.NotNull(body.Data);

            byte[] testImageAsBytes = File.ReadAllBytes(PNG_FILEPATH);
            byte[] actualImage = _loadPictogramFromDisk(ADMIN_PRIVATE_PICTOGRAM);
            // Check that the images is equivalent
            Assert.Equal(testImageAsBytes, actualImage);
        }

        [Fact]
        public void CreateImage_LoginProtected_Success()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);

            var res = pc.SetPictogramImage(DEP_ONE_PROTECTED_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as SuccessResponse<WeekPictogramDTO>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            Assert.NotNull(body.Data);

            byte[] testImageAsBytes = File.ReadAllBytes(PNG_FILEPATH);
            byte[] actualImage = _loadPictogramFromDisk(DEP_ONE_PROTECTED_PICTOGRAM);
            // Check that the images is equivalent
            Assert.Equal(testImageAsBytes, actualImage);
        }

        [Fact]
        public void CreateImage_LoginAnotherProtected_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            var res = pc.SetPictogramImage(DEP_ONE_PROTECTED_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
            Assert.Equal(ErrorCode.NotAuthorized, body.ErrorCode);
        }

        [Fact]
        public void CreateImage_LoginAnotherPrivate_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            var res = pc.SetPictogramImage(ADMIN_PRIVATE_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
            Assert.Equal(ErrorCode.NotAuthorized, body.ErrorCode);
        }

        [Fact]
        public void CreateImage_LoginNonexisting_NotFound()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            var res = pc.SetPictogramImage(NONEXISTING_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status404NotFound, res.StatusCode);
            Assert.Equal(ErrorCode.PictogramNotFound, body.ErrorCode);
        }

        [Fact]
        public void CreateImage_PublicJpeg_Success()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[CITIZEN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(JPEG_FILEPATH);

            var res = pc.SetPictogramImage(PUBLIC_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as SuccessResponse<WeekPictogramDTO>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            Assert.NotNull(body.Data);

            byte[] testImageAsBytes = File.ReadAllBytes(JPEG_FILEPATH);
            byte[] actualImage = _loadPictogramFromDisk(PUBLIC_PICTOGRAM);
            
            // Check that the images is equivalent
            Assert.Equal(testImageAsBytes, actualImage);
        }

        #endregion

        #region UpdatePictogramImage
        [Fact]
        public void UpdatePictogramImage_NoLoginProtected_UserNotFound()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);

            var imgRes = pc.SetPictogramImage(DEP_ONE_PROTECTED_PICTOGRAM).Result as ObjectResult;

            Assert.Equal(StatusCodes.Status200OK, imgRes.StatusCode);

            _testContext.MockUserManager.MockLogout();
            var res = pc.SetPictogramImage(DEP_ONE_PROTECTED_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status401Unauthorized, res.StatusCode);
            Assert.Equal(ErrorCode.UserNotFound, body.ErrorCode);
        }

        [Fact]
        public void UpdatePictogramImage_NoLoginPrivate_UserNotFound()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);

            var img = pc.SetPictogramImage(ADMIN_PRIVATE_PICTOGRAM);

            _testContext.MockUserManager.MockLogout();
            var res = pc.SetPictogramImage(ADMIN_PRIVATE_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as ErrorResponse;
            
            Assert.Equal(StatusCodes.Status401Unauthorized, res.StatusCode);
            Assert.Equal(ErrorCode.UserNotFound, body.ErrorCode);
        }

        [Fact]
        public void UpdatePictogramImage_LoginPublic_Success()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            var res = pc.SetPictogramImage(PUBLIC_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as SuccessResponse<WeekPictogramDTO>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            Assert.NotNull(body.Data);

            byte[] testImageAsBytes = File.ReadAllBytes(PNG_FILEPATH);
            byte[] actualImage = _loadPictogramFromDisk(PUBLIC_PICTOGRAM);
            // Check that the images is equivalent
            Assert.Equal(testImageAsBytes, actualImage);
        }


        [Fact]
        public void UpdatePictogramImage_LoginPrivate_Success()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            var res = pc.SetPictogramImage(ADMIN_PRIVATE_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as SuccessResponse<WeekPictogramDTO>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            Assert.NotNull(body.Data);

            byte[] testImageAsBytes = File.ReadAllBytes(PNG_FILEPATH);
            byte[] actualImage = _loadPictogramFromDisk(ADMIN_PRIVATE_PICTOGRAM);
            // Check that the images is equivalent
            Assert.Equal(testImageAsBytes, actualImage);
        }


        [Fact]
        public void UpdatePictogramImage_LoginProtected_Success()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            var res = pc.SetPictogramImage(ADMIN_PRIVATE_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as SuccessResponse<WeekPictogramDTO>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            Assert.NotNull(body.Data);

            byte[] testImageAsBytes = File.ReadAllBytes(PNG_FILEPATH);
            byte[] actualImage = _loadPictogramFromDisk(ADMIN_PRIVATE_PICTOGRAM);
            // Check that the images is equivalent
            Assert.Equal(testImageAsBytes, actualImage);
        }


        [Fact]
        public void UpdatePictogramImage_LoginAnotherPrivate_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            var img = pc.SetPictogramImage(ADMIN_PRIVATE_PICTOGRAM);

            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var res = pc.SetPictogramImage(ADMIN_PRIVATE_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
            Assert.Equal(ErrorCode.NotAuthorized, body.ErrorCode);
        }


        [Fact]
        public void UpdatePictogramImage_LoginAnotherProtected_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            var img = pc.SetPictogramImage(DEP_ONE_PROTECTED_PICTOGRAM);
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var res = pc.SetPictogramImage(DEP_ONE_PROTECTED_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
            Assert.Equal(ErrorCode.NotAuthorized, body.ErrorCode);
        }


        [Fact]
        public void UpdatePictogramImage_SuperAdminUpdatePublicPicto_Success()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            var img = pc.SetPictogramImage(PUBLIC_PICTOGRAM);
            _testContext.MockHttpContext.MockRequestNoImage();
            var res = pc.SetPictogramImage(PUBLIC_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as SuccessResponse<WeekPictogramDTO>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
        }


        [Fact]
        public void UpdatePictogramImage_LoginNonexistingPicto_NotFound()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);           
            
            var res = pc.SetPictogramImage(NONEXISTING_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as ErrorResponse;
            
            Assert.Equal(StatusCodes.Status404NotFound, res.StatusCode);
            Assert.Equal(ErrorCode.PictogramNotFound, body.ErrorCode);
        }

        [Fact]
        public void UpdatePictogramImage_PublicJpegToJpeg_Success()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(JPEG_FILEPATH);
            var img = pc.SetPictogramImage(PUBLIC_PICTOGRAM).Result;
            img = pc.SetPictogramImage(PUBLIC_PICTOGRAM).Result; /*The test will check if it is possible to update
            an pictogram that already has an image#1#
            _testContext.MockHttpContext.MockRequestImage(JPEG_FILEPATH);

            var res = pc.SetPictogramImage(PUBLIC_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as SuccessResponse<WeekPictogramDTO>;
            
            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);

            byte[] testImageAsBytes = File.ReadAllBytes(JPEG_FILEPATH);
            byte[] actualImage = _loadPictogramFromDisk(PUBLIC_PICTOGRAM);
            // Check that the images is equivalent
            Assert.Equal(testImageAsBytes, actualImage);
        }

        [Fact]
        public void UpdatePictogramImage_PublicPngToJpeg_Success()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);

            var img = pc.SetPictogramImage(PUBLIC_PICTOGRAM).Result;

            byte[] pngTestImage = File.ReadAllBytes(PNG_FILEPATH);
            byte[] actualImage = _loadPictogramFromDisk(PUBLIC_PICTOGRAM);
            // Check that we have correctly updated the image to a PNG image
            Assert.Equal(pngTestImage, actualImage);

            img = pc.SetPictogramImage(PUBLIC_PICTOGRAM).Result; 

            _testContext.MockHttpContext.MockRequestImage(JPEG_FILEPATH);

            var res = pc.SetPictogramImage(PUBLIC_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as SuccessResponse<WeekPictogramDTO>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);

            byte[] testImageAsBytes = File.ReadAllBytes(JPEG_FILEPATH);
            byte[] actualImage2 = _loadPictogramFromDisk(PUBLIC_PICTOGRAM);
            // Check that we have correctly updated the png image to the jpeg image
            Assert.Equal(testImageAsBytes, actualImage2);
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
            var res = pc.ReadPictogramImage(DEP_ONE_PROTECTED_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status401Unauthorized, res.StatusCode);
            Assert.Equal(ErrorCode.NotAuthorized, body.ErrorCode);
        }
        
        [Fact]
        public void ReadPictogramImage_NoLoginPrivate_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            var img = pc.SetPictogramImage(ADMIN_PRIVATE_PICTOGRAM).Result;

            _testContext.MockUserManager.MockLogout();
            var res = pc.ReadPictogramImage(ADMIN_PRIVATE_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status401Unauthorized, res.StatusCode);
            Assert.Equal(ErrorCode.NotAuthorized, body.ErrorCode);
        }
        
        [Fact]
        public void ReadPictogramImage_LoginPublic_Success()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            var img = pc.SetPictogramImage(PUBLIC_PICTOGRAM).Result;
            
            var res = pc.ReadPictogramImage(PUBLIC_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as SuccessResponse<byte[]>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            Assert.NotNull(body.Data);

            byte[] testImageAsBytes = File.ReadAllBytes(PNG_FILEPATH);
            // Check that we have correctly read the image - that is that the byte arrays of the expected and actual image is equal
            Assert.Equal(testImageAsBytes, body.Data);
        }
        
        [Fact]
        public void ReadPictogramImage_LoginProtected_Success()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            var img = pc.SetPictogramImage(DEP_ONE_PROTECTED_PICTOGRAM);
            var res = pc.ReadPictogramImage(DEP_ONE_PROTECTED_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as SuccessResponse<byte[]>;

            Assert.Equal(StatusCodes.Status200OK, res.StatusCode);
            Assert.True(body.Data != null);

            byte[] testImageAsBytes = File.ReadAllBytes(PNG_FILEPATH);
            // Check that we have correctly read the image - that is that the byte arrays of the expected and actual image is equal
            Assert.Equal(testImageAsBytes, body.Data);
        }
        
        [Fact]
        public void ReadPictogramImage_LoginAnotherPrivate_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            var img = pc.SetPictogramImage(ADMIN_PRIVATE_PICTOGRAM);
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var res = pc.ReadPictogramImage(ADMIN_PRIVATE_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
            Assert.Equal(ErrorCode.NotAuthorized, body.ErrorCode);
        }

        [Fact]
        public void ReadPictogramImage_LoginAnotherProtected_Unauthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockHttpContext.MockRequestImage(PNG_FILEPATH);
            var img = pc.SetPictogramImage(DEP_ONE_PROTECTED_PICTOGRAM);
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[GUARDIAN_DEP_TWO]);
            var res = pc.ReadPictogramImage(DEP_ONE_PROTECTED_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status403Forbidden, res.StatusCode);
            Assert.Equal(ErrorCode.NotAuthorized, body.ErrorCode);
        }

        [Fact]
        public void ReadPictogramImage_LoginPublicNoImage_ErrorPictogramHasNoImage()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = pc.ReadPictogramImage(PUBLIC_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status404NotFound, res.StatusCode);
            Assert.Equal(ErrorCode.PictogramHasNoImage, body.ErrorCode);
        }

        [Fact]
        public void ReadPictogramImage_LoginNonexisting_NotFound()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var img = pc.SetPictogramImage(NONEXISTING_PICTOGRAM);
            var res = pc.ReadPictogramImage(NONEXISTING_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status404NotFound, res.StatusCode);
            Assert.Equal(ErrorCode.PictogramNotFound, body.ErrorCode);
        }

        [Fact]
        public void ReadPictogramImage_GetPublicJpeg_NotAuthorized()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockUserManager.MockLogout();
            _testContext.MockHttpContext.MockRequestImage(JPEG_FILEPATH);
            var img = pc.SetPictogramImage(PUBLIC_PICTOGRAM);
            var res = pc.ReadPictogramImage(PUBLIC_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as ErrorResponse;


            Assert.Equal(StatusCodes.Status401Unauthorized, res.StatusCode);
            Assert.Equal(ErrorCode.NotAuthorized, body.ErrorCode);
        }

        [Fact]
        public void ReadRawPictogramImage_GetPrivate_Success()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = pc.ReadRawPictogramImage(ADMIN_PRIVATE_PICTOGRAM);
    
            Assert.True(res.IsCompleted);
            Assert.IsType<PhysicalFileResult>(res.Result);
    
            var fileContent = ((PhysicalFileResult) res.Result);
            
            Assert.Equal("image/png", fileContent.ContentType);

            var expectedImage = _loadPictogramFromDisk(ADMIN_PRIVATE_PICTOGRAM);
            // Check that we read the correct image
            Assert.Equal(expectedImage, File.ReadAllBytes(fileContent.FileName));
        }

        [Fact]
        public void ReadRawPictogramImage_GetPublic_Success()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            var res = pc.ReadRawPictogramImage(DEP_ONE_PROTECTED_PICTOGRAM);

            Assert.True(res.IsCompleted);
            Assert.IsType<PhysicalFileResult>(res.Result);

            var fileContent = ((PhysicalFileResult)res.Result);

            Assert.Equal("image/png", fileContent.ContentType);
            
            var expectedImage = _loadPictogramFromDisk(DEP_ONE_PROTECTED_PICTOGRAM);
            // Check that we read the correct image
            Assert.Equal(expectedImage, File.ReadAllBytes(fileContent.FileName));
          
        }

        [Fact]
        public void ReadRawPictogramImage_NoLoginPrivate_NotFound()
        {
            var pc = initializeTest();
            _testContext.MockUserManager.MockLoginAsUser(_testContext.MockUsers[ADMIN_DEP_ONE]);
            _testContext.MockUserManager.MockLogout();
            var res = pc.ReadRawPictogramImage(ADMIN_PRIVATE_PICTOGRAM).Result as ObjectResult;
            var body = res.Value as ErrorResponse;

            Assert.Equal(StatusCodes.Status401Unauthorized, res.StatusCode);
            Assert.Equal(ErrorCode.UserNotFound, body.ErrorCode);
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
        private void _preparePictogramFolder(int publicPictogram)
        {
            using (FileStream fs = new FileStream(
                _pictogramFolderPath+publicPictogram+".png", 
                FileMode.OpenOrCreate)
            )
            {
                fs.Write(new byte[2]);
            }
        }
        
        private byte[] _loadPictogramFromDisk(int publicPictogram)
        {
            return File.ReadAllBytes(_pictogramFolderPath + publicPictogram + ".png");
        }

        #endregion
    }
}
*/
