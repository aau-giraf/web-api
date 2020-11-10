using System;
using System.Linq;
using GirafRest.Models;
using GirafRest.Services;
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
        
        /// <summary>
        /// Constructor for controller
        /// </summary>
        /// <param name="girafService">Service Injection</param>
        /// <param name="lFactory">Service Injection</param>
        /// <param name="authentication">Service Injection</param>
        public AlternateNameController(IGirafService girafService, ILoggerFactory lFactory, IAuthenticationService authentication)
        {
            _giraf = girafService;
            _giraf._logger = lFactory.CreateLogger("AlternateName");
            _authentication = authentication;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("{picId}/{userId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType((StatusCodes.Status200OK))]
        public async Task<ActionResult> GetName(long picId, string userId)
        {
            GirafUser user = await _giraf._context.Users.FirstOrDefaultAsync(us => us.Id == userId);
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
            
            Pictogram pic = await _giraf._context.Pictograms.FirstOrDefaultAsync(id => id.Id == picId);
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

            return StatusCode(StatusCodes.Status200OK, new SuccessResponse(
                new AlternateNameDTO(an).ToString()));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="an"></param>
        /// <returns></returns>
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
            // check access rights
            if (!(await _authentication.HasEditOrReadUserAccess(
                await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return StatusCode(StatusCodes.Status403Forbidden,
                    new ErrorResponse(ErrorCode.NotAuthorized, "User does not have permission"));
            
            if (string.IsNullOrEmpty(an.Name))
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                    new ErrorResponse(ErrorCode.MissingProperties, "Name cannot be empty"));
            }
            
            oldAn.CitizenId = user.Id;
            oldAn.Citizen = user;
            oldAn.PictogramId = an.Pictogram;
            oldAn.Pictogram = await _giraf._context.Pictograms.FirstOrDefaultAsync(pic => pic.Id == oldAn.PictogramId);
            oldAn.Name = an.Name;

            _giraf._context.AlternateNames.Update(oldAn);
            await _giraf._context.SaveChangesAsync();
            
            return StatusCode(StatusCodes.Status200OK, new SuccessResponse(
                an.ToString()));
        }
        
        
    }
}