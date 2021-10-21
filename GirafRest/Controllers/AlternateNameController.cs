using System;
using System.Linq;
using GirafRest.Models;
using GirafRest.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using GirafRest.Models.DTOs;
using GirafRest.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace GirafRest.Controllers
{

    /// <summary>
    /// Controller for managing <see cref="AlternateName"/>
    /// </summary>
    [Authorize]
    [Route("v2/[Controller]")]
    public class AlternateNameController : Controller
    {
        
        private readonly IGirafService _giraf;
        private readonly IAuthenticationService _authentication;
        private readonly IPictogramRepository _pictogramRepository;
        private readonly IGirafUserRepository _girafUserRepository;
        
        /// <summary>
        /// Constructor for controller
        /// </summary>
        /// <param name="girafService">Service Injection</param>
        /// <param name="lFactory">Service Injection</param>
        /// <param name="authentication">Service Injection</param>
        public AlternateNameController(IGirafService girafService, ILoggerFactory lFactory, IPictogramRepository pictogramRepository, IGirafUserRepository girafUserRepository)
        {
            _giraf = girafService;
            _giraf._logger = lFactory.CreateLogger("AlternateName");
            _authentication = authentication;
            _pictogramRepository = pictogramRepository;
            _girafUserRepository = girafUserRepository;

        }


        /// <summary>
        /// Get AlternateName for specified <see cref="GirafUser"/> and <see cref="Pictogram"/>
        /// </summary>
        /// <param name="userId">The id of the related user</param>
        /// <param name="picId">The id of the related pictogram</param>
        /// <returns></returns>
        [HttpGet("{userId}/{picId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType((StatusCodes.Status200OK))]
        public async Task<ActionResult> GetName(string userId, long picId )
        {
            GirafUser user = _girafUserRepository.GetUserByID(userId);
            if (user == null)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new ErrorResponse(ErrorCode.NotFound, "User not found"));
            }
            
            // check access rights
            if (!(await _authentication.HasEditOrReadUserAccess(
                await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return StatusCode(StatusCodes.Status403Forbidden,
                    new ErrorResponse(ErrorCode.NotAuthorized, "User does not have permission"));
            
            Pictogram pic = _pictogramRepository.GetPictogramByID(picId);
            if (pic == null)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new ErrorResponse(ErrorCode.NotFound, "Pictogram not found"));
            }
            
            AlternateName an =
                _giraf._context.AlternateNames.FirstOrDefault(
                    alt => alt.CitizenId == userId && alt.PictogramId == picId);
            if (an == null)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new ErrorResponse(ErrorCode.NotFound, "Alternate name not found"));
            }

            return StatusCode(StatusCodes.Status200OK, new SuccessResponse<AlternateNameDTO>(
                new AlternateNameDTO(an)));
        }

        /// <summary>
        /// Create a new AlternateName from AlternateNameDTO
        /// </summary>
        /// <param name="an"></param>
        /// <returns>StatusCode containing relevant error or success with the created AltenateNaem</returns>
        [HttpPost("")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> CreateAlternateName([FromBody] AlternateNameDTO an)
        {
            
            AlternateName newName = null;
            if (an == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                    new ErrorResponse(ErrorCode.MissingProperties, "AlternateName missing"));
            }
            
            GirafUser user = await _giraf._context.Users.FirstOrDefaultAsync(us => us.Id == an.Citizen);
            if (user == null)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new ErrorResponse(ErrorCode.NotFound, "User not found"));
            }
            
            // check access rights
            if (!(await _authentication.HasEditOrReadUserAccess(
                await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return StatusCode(StatusCodes.Status403Forbidden,
                    new ErrorResponse(ErrorCode.NotAuthorized, "User does not have permission"));
            
            Pictogram pic = _giraf._context.Pictograms.FirstOrDefault(id => id.Id == an.Pictogram);
            if (pic == null)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new ErrorResponse(ErrorCode.NotFound, "Pictogram not found"));
            }
        
            if (an.Name == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                    new ErrorResponse(ErrorCode.MissingProperties, "Name missing"));
            }

            if ( await _giraf._context.AlternateNames.FirstOrDefaultAsync(altnam =>
                altnam.Pictogram == pic && altnam.Citizen == user) != null)
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                    new ErrorResponse(ErrorCode.AlternateNameAlreadyExists,
                        "Alternate name exists already, should use put"));
            }

            newName = new AlternateName(user, pic, an.Name);

            await _giraf._context.AlternateNames.AddAsync(newName);

            await _giraf._context.SaveChangesAsync();
            
            return StatusCode(StatusCodes.Status201Created,
            new SuccessResponse<AlternateNameDTO>(
                new AlternateNameDTO(newName)
            ));
        }

        /// <summary>
        /// Edit an AlternateName
        /// </summary>
        /// <param name="id">The id of the AlternateName to edit</param>
        /// <param name="an">The AlternateNameDTO containing changes</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> EditAlternateName(int id, [FromBody] AlternateNameDTO an)
        {
            if (an == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                    new ErrorResponse(ErrorCode.MissingProperties, "Missing AlternateName"));
            }

            AlternateName oldAn = await _giraf._context.AlternateNames.FirstOrDefaultAsync(altnam => altnam.Id == id);

            if (oldAn == null)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new ErrorResponse(ErrorCode.NotFound, "AlternateName Not found"));
            }
            
            GirafUser user = await _giraf._context.Users.FirstOrDefaultAsync(us => us.Id == an.Citizen);
            if (user == null)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new ErrorResponse(ErrorCode.NotFound, "User not found"));
            }
            // check access rights
            if (!(await _authentication.HasEditOrReadUserAccess(
                await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return StatusCode(StatusCodes.Status403Forbidden,
                    new ErrorResponse(ErrorCode.NotAuthorized, "User does not have permission"));

            Pictogram pic = await _giraf._context.Pictograms.FirstOrDefaultAsync(p => p.Id == an.Pictogram);
            if (pic == null)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new ErrorResponse(ErrorCode.NotFound, "Pictogram not found"));
            }
            
            if (string.IsNullOrEmpty(an.Name))
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                    new ErrorResponse(ErrorCode.MissingProperties, "Name cannot be empty"));
            }

            if (oldAn.PictogramId != pic.Id || oldAn.CitizenId != user.Id)
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                    new ErrorResponse(ErrorCode.InvalidProperties, "Only name should be changed"));
            }
            
            oldAn.Name = an.Name;

            _giraf._context.AlternateNames.Update(oldAn);
            await _giraf._context.SaveChangesAsync();
            
            return StatusCode(StatusCodes.Status200OK, new SuccessResponse<AlternateNameDTO>(
                an));
        }
        
        
    }
}