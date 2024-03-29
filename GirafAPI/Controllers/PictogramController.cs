using GirafEntities.Responses;
using GirafEntities.User;
using GirafEntities.WeekPlanner;
using GirafEntities.WeekPlanner.DTOs;
using GirafRepositories.Interfaces;
using GirafServices.User;
using GirafServices.WeekPlanner;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GirafAPI.Controllers
{
    /// <summary>
    /// Manages pictograms, CRUD-ish.
    /// </summary>
    /// 

    [Authorize]
    [Route("v1/[controller]")]
    public class PictogramController : Controller
    
    {
        private const string IMAGE_TYPE_PNG = "image/png";

        private readonly IUserService _userService;

        private readonly IHostEnvironment _hostingEnvironment;

        private readonly string imagePath;

        private readonly IPictogramRepository _pictogramRepository;
        private readonly IPictogramService _pictogramService;
        private readonly IImageService _imageService;

        /// <summary>
        /// Constructor for controller
        /// </summary>
        /// <param name="girafService">Service Injection</param>
        /// <param name="lFactory">Service Injection</param>
        /// <param name="hostingEnvironment">Service Injection</param>
        /// <param name="girafUserRepository">The <see cref="IGirafUserRepository"/> used to query Users</param>
        /// <param name="pictogramRepository">Pictogram Injection</param>
        public PictogramController(
            IUserService girafService,
            IHostEnvironment hostingEnvironment,
            IPictogramRepository pictogramRepository, 
            IImageService imageService, IPictogramService pictogramService)
        {
            _userService = girafService;
            _hostingEnvironment = hostingEnvironment;
            imagePath = _hostingEnvironment.ContentRootPath + "/../pictograms/";
            _pictogramRepository = pictogramRepository;
            _imageService = imageService;
            _pictogramService = pictogramService;
        }

        #region PictogramHandling

        /// <summary>
        /// Get all public <see cref="Pictogram"/> pictograms available to the user
        /// (i.e the public pictograms and those owned by the user (PRIVATE) and his department (PROTECTED)).
        /// </summary>
        /// <param name="query">The query string. pictograms are filtered based on this string if passed</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Number of pictograms per page</param>
        /// <returns> All the user's <see cref="Pictogram"/> pictograms on success else InvalidProperties or
        /// PictogramNotFound </returns>
        [HttpGet("", Name = "GetPictograms")]
        [ProducesResponseType(typeof(SuccessResponse<List<WeekPictogramDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ReadPictograms([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (pageSize < 1 || pageSize > 100)
            {
                return BadRequest(new ErrorResponse(ErrorCode.InvalidProperties, "pageSize must be in the range 1-100"));
            }

            if (page < 1)
            {
                return BadRequest(new ErrorResponse(ErrorCode.InvalidProperties, "Missing page"));
            }
            //Produce a list of all pictograms available to the user
            GirafUser user = await _userService.LoadUserWithDepartment(HttpContext.User).ConfigureAwait(false);
            var userPictograms = await _pictogramService.ReadAllPictograms(query, user);

            // This does not occur only when user has no pictograms, but when any error is caught in the previous call
            if (userPictograms == null)
            {
                return NotFound(new ErrorResponse(ErrorCode.PictogramNotFound, "User has no pictograms"));
               
            }
            return Ok(new SuccessResponse<List<WeekPictogramDTO>>(userPictograms.OfType<Pictogram>()
                   .Skip((page - 1) * pageSize).Take(pageSize).Select(_p => new WeekPictogramDTO(_p)).ToList()));
        }

        /// <summary>
        /// Read the <see cref="Pictogram"/> pictogram with the specified <paramref name="id"/> id and
        /// check if the user is authorized to see it.
        /// </summary>
        /// <param name="id">The id of the pictogram to fetch.</param>
        /// <returns> The <see cref="Pictogram"/> pictogram with the specified ID,
        /// NotFound  if no such <see cref="Pictogram"/> pictogram exists
        /// Else: PictogramNotFound, UserNotFound, Error, or NotAuthorized
        /// </returns>
        [HttpGet("{id}", Name = "GetPictogram")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(SuccessResponse<WeekPictogramDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ReadPictogram(long id)
        {
            //Fetch the pictogram and check that it actually exists
            var pictogram = await _pictogramRepository.GetPictogramsById(id);

            if (pictogram == null)
            {
                return this.ResourceNotFound(nameof(Pictogram), id);
            }
            if (pictogram.AccessLevel == AccessLevel.PUBLIC)
                return Ok(new SuccessResponse<WeekPictogramDTO>(new WeekPictogramDTO(pictogram)));

            var usr = await _userService.LoadBasicUserDataAsync(HttpContext.User);
            if (usr == null)
            {
                return Unauthorized(new ErrorResponse(ErrorCode.UserNotFound, "No user authorized"));
            }

            if (!_pictogramService.CheckOwnership(pictogram, usr).Result)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                        new ErrorResponse(ErrorCode.NotAuthorized, "User does not have rights to resource"));
            }

            return Ok(new SuccessResponse<WeekPictogramDTO>(new WeekPictogramDTO(pictogram)));
        }

        /// <summary>
        /// Create a new <see cref="Pictogram"/> pictogram.
        /// </summary>
        /// <param name="pictogram">A <see cref="PictogramDTO"/> with all relevant information about the new pictogram.</param>
        /// <returns>The new pictogram with all database-generated information.
        /// Else: Notfound, MissingProperties or InvalidProperties </returns>
        [HttpPost("")]
        [ProducesResponseType(typeof(SuccessResponse<WeekPictogramDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> CreatePictogram([FromBody] PictogramDTO pictogram)
        {
            var user = await _userService.LoadUserWithResources(HttpContext.User);

            if (user == null)
                return NotFound(new ErrorResponse(ErrorCode.NotFound, "User not found"));

            if (pictogram == null)
                return BadRequest(new ErrorResponse(ErrorCode.InvalidProperties,
                    "Could not read pictogram DTO. Please make sure not to include image data in this request. " +
                    "Use POST localhost/v1/pictogram/{id}/image instead."));

            if (string.IsNullOrEmpty(pictogram.Title))
                return BadRequest(new ErrorResponse(ErrorCode.InvalidProperties, "Invalid pictogram: Blank title"));

            if (!ModelState.IsValid)
                return BadRequest(new ErrorResponse(ErrorCode.InvalidProperties, "Model is not valid"));

            //Create the actual pictogram instance
            // if access level is not specified, missing properties
            if (pictogram.AccessLevel == null || !Enum.IsDefined(typeof(AccessLevel), pictogram.AccessLevel))
                return BadRequest(new ErrorResponse(ErrorCode.MissingProperties, "Missing access level"));

            Pictogram pict =
                new Pictogram(pictogram.Title, (AccessLevel)pictogram.AccessLevel);

            if (pictogram.AccessLevel == AccessLevel.PRIVATE)
            {
                //Add relation between pictogram and current user
                user.Resources.Add(new UserResource(user, pict));
            }
            else if (pictogram.AccessLevel == AccessLevel.PROTECTED)
            {
                //Add the pictogram to the user's department
                user.Department.Resources.Add(new DepartmentResource(user.Department, pict));
            }
            await _pictogramRepository.CreatePictorgram(pict);
            return CreatedAtRoute(
                "GetPictogram",
                new { id = pict.Id },
                new SuccessResponse<WeekPictogramDTO>(new WeekPictogramDTO(pict)));
        }

        /// <summary>
        /// Update info of a <see cref="Pictogram"/> pictogram.
        /// </summary>
        /// <param name="id">id in URL for Pictogram to update.</param>
        /// <param name="pictogram">A <see cref="PictogramDTO"/> with all new information to update with.
        /// The Id found in this DTO is the target pictogram.
        /// </param>
        /// <returns> The updated Pictogram else: MissingProperties, InvalidProperties, NotFound, PictogramNotFound, or
        /// NotAuthorized </returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(WeekPictogramDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdatePictogramInfo(long id, [FromBody] PictogramDTO pictogram)
        {
            if (pictogram == null)
            {
                return BadRequest(
                    new ErrorResponse(ErrorCode.MissingProperties,
                    "Could not read pictogram DTO.", "Please make sure not to include image data in this request. " +
                    "Use POST localhost/v1/pictogram/{id}/image instead."));
            }

            if (pictogram.AccessLevel == null || !Enum.IsDefined(typeof(AccessLevel), pictogram.AccessLevel))
            {
                return BadRequest(new ErrorResponse(ErrorCode.MissingProperties, "Missing access level"));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse(ErrorCode.InvalidProperties, "Invalid model"));
            }

            var usr = await _userService.LoadBasicUserDataAsync(HttpContext.User);

            if (usr == null)
            {
                return Unauthorized(new ErrorResponse(ErrorCode.UserNotFound, "User not found"));
            }

            //Fetch the pictogram from the database and check that it exists
            var pict = await _pictogramRepository.GetPictogramsById(id);

            if (pict == null)
            {
                return NotFound(new ErrorResponse(ErrorCode.PictogramNotFound, "Pictogram not found"));
            }

            if (!_pictogramService.CheckOwnership(pict, usr).Result)
            {
                return this.MissingPropertyFromRequest(nameof(pict));
            }

            //Ensure that Id is not changed.
            pictogram.Id = id;
            //Update the existing database entry and save the changes
            pict.Merge(pictogram);
            await _pictogramRepository.UpdatePictogram(pict);

            return Ok(new SuccessResponse<WeekPictogramDTO>(new WeekPictogramDTO(pict)));
        }

        /// <summary>
        /// Delete the <see cref="Pictogram"/> pictogram with the specified id.
        /// </summary>
        /// <param name="id">The id of the pictogram to delete.</param>
        /// <returns>Ok if the pictogram was deleted,
        /// Else: UserNotFound,PictogramNotFound or NotAuthorized </returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeletePictogram(int id)
        {
            var usr = await _userService.LoadBasicUserDataAsync(HttpContext.User);
            if (usr == null)
            {
                return Unauthorized(new ErrorResponse(ErrorCode.UserNotFound, "User not found"));
            }
            //Fetch the pictogram from the database and check that it exists
            var pict = await _pictogramRepository.GetPictogramsById(id);

            if (pict == null)
            {
                return NotFound(new ErrorResponse(ErrorCode.PictogramNotFound, "Pictogram not found"));
            }

            var temp = _pictogramService.CheckOwnership(pict, usr).Result;
            if (!temp)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                   new ErrorResponse(ErrorCode.NotAuthorized, "User does not have permission"));

            }

            await _pictogramRepository.RemoveRelations(pict);

            // Now we can safely delete the pictogram
            await _pictogramRepository.RemovePictogram(pict);


            return Ok(new SuccessResponse("Pictogram deleted"));
        }

        #endregion PictogramHandling

        #region ImageHandling

        /// <summary>
        /// Update the image of a <see cref="Pictogram"/> pictogram with the given Id.
        /// </summary>
        /// <param name="id">Id of the pictogram to update the image for.</param>
        /// <returns>The updated pictogram along with its image.
        /// Else: UserNotFound, PictogramNotFound or NotAuthorized</returns>

        [HttpPut("{id}/image")]
        [ProducesResponseType(typeof(SuccessResponse<WeekPictogramDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> SetPictogramImage(long id)
        {
            GirafUser user = await _userService.LoadBasicUserDataAsync(HttpContext.User);
        
            if (user == null)
            {
                return Unauthorized(new ErrorResponse(ErrorCode.UserNotFound, "User not found"));
            }
        
        
            //Attempt to fetch the pictogram from the database.
            Pictogram pictogram = await _pictogramRepository.GetPictogramsById(id);
        
            if (pictogram == null)
            {
                return NotFound(new ErrorResponse(ErrorCode.PictogramNotFound, "Pictogram not found"));
            }
        
            if (!_pictogramService.CheckOwnership(pictogram, user).Result)
            {
                return this.MissingPropertyFromRequest(nameof(pictogram));
            }
        
            //Update the image
            byte[] image = await _imageService.ReadRequestImage(HttpContext.Request.Body);
        
            // This sets the path that the system looks for when retrieving a pictogram
            string path = imagePath + pictogram.Id + ".png";
        
            if (image.Length > 0)
            {
                try
                {
                    using (FileStream fs = new FileStream(path, FileMode.Create))
                    {
                        fs.Write(image);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    //Consider if the errorcode is the most appropriate one here
                    return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.Forbidden, "The server does not have permission to write this file"));
                }
        
                pictogram.ImageHash = _imageService.GetHash(image);
            }
            _pictogramRepository.SaveState();
            return Ok(new SuccessResponse<WeekPictogramDTO>(new WeekPictogramDTO(pictogram)));
        }

        /// <summary>
        /// Read the image of a given pictogram as a sequence of bytes.
        /// </summary>
        /// <param name="id">The id of the pictogram to read the image of.</param>
        /// <returns>A The image else: PictogramNotFound, NotAuthorized, PictogramHasNoImage,
        /// or NotAuthorized </returns>
        [HttpGet("{id}/image")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(SuccessResponse<byte[]>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ReadPictogramImage(long id)
        {
            //Fetch the pictogram and check that it actually exists and has an image.
            var picto = await _pictogramRepository.GetPictogramsById(id);

            if (picto == null)
            {
                return NotFound(new ErrorResponse(ErrorCode.PictogramNotFound, "Pictogram not found"));
            }


            if (picto.AccessLevel != AccessLevel.PUBLIC)
            {
                var usr = await _userService.LoadBasicUserDataAsync(HttpContext.User);
                if (usr == null)
                    return Unauthorized(new ErrorResponse(ErrorCode.NotAuthorized, "User not authorized"));
                if (!_pictogramService.CheckOwnership(picto, usr).Result)
                    return StatusCode(StatusCodes.Status403Forbidden,
                        new ErrorResponse(ErrorCode.NotAuthorized, "User does not have permission"));
            }

            if (picto.ImageHash == null)
            {
                return NotFound(new ErrorResponse(ErrorCode.PictogramHasNoImage, "Pictogram has no image"));
            }

            var pictoPath = $"{imagePath}{picto.Id}.png";

            //At this time, there is no '.NET native' way to check file permissions on Linux, so instead we catch an exception, if current (OS) user does not have read permission
            try
            {
                byte[] data = System.IO.File.ReadAllBytes(pictoPath);
                return Ok(new SuccessResponse<byte[]>(data));
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.NotAuthorized, "The server can not access the specified image"));
            }
            catch (FileNotFoundException)
            {
                return StatusCode(StatusCodes.Status404NotFound, new ErrorResponse(ErrorCode.NotAuthorized, "The server can not find the specified image"));
            }
        }

        /// <summary>
        /// Reads the raw pictogram image.
        /// You are allowed to read all public pictograms aswell as your own pictograms
        ///  or any pictograms shared within the department
        /// </summary>
        /// <returns>The raw pictogram image.</returns>
        /// <param name="id">Identifier.</param>
        [HttpGet("{id}/image/raw")]
        [AllowAnonymous]
        [Produces(IMAGE_TYPE_PNG)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ReadRawPictogramImage(long id)
        {
            // fetch current authenticated user


            var picto = await _pictogramRepository.GetPictogramsById(id);


            if (picto == null)
            {
                return this.ResourceNotFound(nameof(Pictogram), id);
            }

            if (picto.AccessLevel == AccessLevel.PUBLIC){
                return PhysicalFile($"{imagePath}{picto.Id}.png", IMAGE_TYPE_PNG);
            }
            
            var usr = await _userService.LoadBasicUserDataAsync(HttpContext.User);
            if (usr == null)
                return Unauthorized(new ErrorResponse(ErrorCode.UserNotFound, "User not found"));
            
            if (picto.ImageHash == null)
            {
                return NotFound(new ErrorResponse(ErrorCode.PictogramHasNoImage, "Pictogram has no image"));
            }

            // you can get all public pictograms
            if (!_pictogramService.CheckOwnership(picto, usr).Result)
            {
                return NotFound();

            }
            return PhysicalFile($"{imagePath}{picto.Id}.png", IMAGE_TYPE_PNG);
        }

        #endregion ImageHandling
    }
}