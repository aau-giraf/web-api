using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using GirafRest.Models;
using GirafRest.Models.ManageViewModels;
using GirafRest.Services;
using GirafRest.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using GirafRest.Models.DTOs;
using GirafRest.Models.AccountViewModels;

namespace GirafRest.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly SignInManager<GirafUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IGirafService _giraf;

        public UserController(
            IGirafService giraf,
          SignInManager<GirafUser> signInManager,
          IEmailSender emailSender,
          ILoggerFactory loggerFactory)
        {
            _giraf = giraf;
            _giraf._logger = loggerFactory.CreateLogger("Manage");
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var user = await _giraf._userManager.GetUserAsync(HttpContext.User);
            if (user != null)
            {
                var result = await _giraf._userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _giraf._logger.LogInformation(3, "User changed their password successfully.");
                    return Ok("Your password was changed.");
                }
            }
            return BadRequest();
        }

        [HttpPost("icon")]
        public async Task<IActionResult> CreateUserIcon() {
            var usr = await _giraf._userManager.GetUserAsync(HttpContext.User);
            if(usr.UserIcon != null) return BadRequest("The user already has an icon - please PUT instead.");
            byte[] image = await _giraf.ReadRequestImage(HttpContext.Request.Body);
            usr.UserIcon = image;
            await _giraf._context.SaveChangesAsync();

            return Ok(new GirafUserDTO(usr));
        }
        [HttpPut("icon")]
        public async Task<IActionResult> UpdateUserIcon() {
            var usr = await _giraf._userManager.GetUserAsync(HttpContext.User);
            if(usr.UserIcon == null) return BadRequest("The user does not have an icon - please POST instead.");
            byte[] image = await _giraf.ReadRequestImage(HttpContext.Request.Body);
            usr.UserIcon = image;
            await _giraf._context.SaveChangesAsync();

            return Ok(new GirafUserDTO(usr));
        }
        [HttpDelete("icon")]
        public async Task<IActionResult> DeleteUserIcon() {
            var usr = await _giraf._userManager.GetUserAsync(HttpContext.User);
            usr.UserIcon = null;
            await _giraf._context.SaveChangesAsync();

            return Ok(new GirafUserDTO(usr));
        }
        
        [HttpPost("applications/{userId}")]
        public async Task<IActionResult> AddApplication(string userId, [FromBody] ApplicationOption application)
        {
            //Check that an application has been specified
            if (application == null)
                return BadRequest("No application was specified in the request body.");

            //Fetch the target user and check that he exists
            var user = await _giraf._context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.AvailableApplications)
                .FirstOrDefaultAsync();
            if (user == null)
                return NotFound($"There is no user with id: {userId}");

            //Add the application for the user to see
            user.AvailableApplications.Add(application);
            await _giraf._context.SaveChangesAsync();
            return Ok(new GirafUserDTO(user));
        }
        
        [HttpDelete("applications/{userId}")]
        public async Task<IActionResult> DeleteApplication(string userId, [FromBody] ApplicationOption application)
        {
            //Check if the caller has specified an application to remove
            if (application == null)
                return BadRequest("No application was specified in the request body.");

            //Fetch the user and check that he exists
            var user = await _giraf._context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.AvailableApplications)
                .FirstOrDefaultAsync();
            if (user == null)
                return NotFound($"There is no user with id: {userId}");

            //Check if the given application was previously available to the user
            var app = user.AvailableApplications.Where(a => a.Id == application.Id).FirstOrDefault();
            if (app == null)
                return NotFound("The user did not have an ApplicationOption with id " + application.Id);

            //Remove it and save changes
            user.AvailableApplications.Remove(app);
            await _giraf._context.SaveChangesAsync();
            return Ok(new GirafUserDTO(user));
        }

        [HttpPut("display-name")]
        public async Task<IActionResult> UpdateDisplayName([FromBody] string displayName)
        {
            if (string.IsNullOrEmpty(displayName))
                return BadRequest("You need to specify a new display name");

            var user = await _giraf.LoadUserAsync(HttpContext.User);

            user.DisplayName = displayName;
            await _giraf._context.SaveChangesAsync();
            return Ok(new GirafUserDTO(user));
        }

        [HttpPost("add-resource/{userId}")]
        public async Task<IActionResult> AddUserResource(string userId, [FromBody] long? resourceId)
        {
            //Check if valid parameters have been specified in the call
            if (string.IsNullOrEmpty(userId))
                return BadRequest("You need to specify an id of a user.");
            if (resourceId == null)
                return BadRequest("You need to specify a resourceId in the body of the request.");

            //Find the resource and check that it actually does exist - also verify that the resource is private
            var resource = await _giraf._context.PictoFrames
                .Where(pf => pf.Id == resourceId)
                .FirstOrDefaultAsync();
            if (resource == null)
                return NotFound("The is no resource with id " + resourceId);
            if (resource.AccessLevel != AccessLevel.PRIVATE)
                return BadRequest("Resources must be PRIVATE (2) in order for users to own them.");

            //Check that the currently authenticated user owns the resource
            var resourceOwnedByCaller = await _giraf.CheckPrivateOwnership(resource, HttpContext);
            if (!resourceOwnedByCaller)
                return Unauthorized();

            //Attempt to find the target user and check that he exists
            var user = await _giraf._context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.Resources)
                .FirstOrDefaultAsync();
            if (user == null)
                return NotFound("There is no user with id " + userId);

            //Check if the target user already owns the resource
            if (user.Resources.Where(ur => ur.ResourceKey == resourceId).Any())
                return BadRequest("The user already owns the resource.");

            //Create the relation and save changes.
            var userResource = new UserResource(user, resource);
            await _giraf._context.UserResources.AddAsync(userResource);
            await _giraf._context.SaveChangesAsync();
            return Ok(new GirafUserDTO(user));
        }

        [HttpPost("remove-resource/{userId}")]
        public async Task<IActionResult> RemoveResource(string userId, [FromBody] long? resourceId)
        {
            //Check that valid parameters have been specified in the call
            if (resourceId == null)
                return BadRequest("The body of the request must contain a resourceId");
            if (string.IsNullOrEmpty(userId))
                return BadRequest("Please specify a userId to add the pictogram to");

            //Fetch the user and check that it exists.
            var user = await _giraf._context.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null) return NotFound($"There is no department with Id {userId}.");
            
            //Fetch the resource with the given id, check that it exists.
            var resource = await _giraf._context.Frames
                .Where(f => f.Id == resourceId)
                .FirstOrDefaultAsync();
            if (resource == null) return NotFound($"There is no resource with id {resourceId}.");

            //Check if the caller owns the resource
            var resourceOwned = await _giraf.CheckPrivateOwnership(resource, HttpContext);
            if (!resourceOwned) return Unauthorized();

            //Check if the user already owns the resource and remove if so.
            var drrelation = await _giraf._context.UserResources
                .Where(ur => ur.ResourceKey == resource.Id && ur.OtherKey == user.Id)
                .FirstOrDefaultAsync();
            if (drrelation == null) return BadRequest("The user does not own the given resource.");
            user.Resources.Remove(drrelation);
            await _giraf._context.SaveChangesAsync();

            //Return Ok and the user - the resource is now visible in user.Resources
            return Ok(new GirafUserDTO(user));
        }

        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            var user = await _giraf.LoadUserAsync(HttpContext.User);

            return Ok(new GirafUserDTO(user));
        }
        
        [HttpPost("grayscale/{enabled}")]
        public async Task<IActionResult> ToggleGrayscale(bool enabled)
        {
            var user = await _giraf.LoadUserAsync(HttpContext.User);

            user.UseGrayscale = enabled;
            await _giraf._context.SaveChangesAsync();
            return Ok(new GirafUserDTO(user));
        }

        [HttpPost("launcher_animations/{enabled}")]
        public async Task<IActionResult> ToggleAnimations(bool enabled)
        {
            var user = await _giraf.LoadUserAsync(HttpContext.User);

            user.DisplayLauncherAnimations = enabled;
            await _giraf._context.SaveChangesAsync();
            return Ok(new GirafUserDTO(user));
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost("SetPassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            var user = await _giraf._userManager.GetUserAsync(HttpContext.User);
            if (user != null)
            {
                var result = await _giraf._userManager.AddPasswordAsync(user, model.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return Ok("Your password was set.");
                }
                AddErrors(result);
            }
            return BadRequest();
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                _giraf._logger.LogError(error.Description);
            }
        }

        #endregion
    }
}
