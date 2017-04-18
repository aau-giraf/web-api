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

namespace GirafRest.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class ManageController : Controller
    {
        private readonly SignInManager<GirafUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly GirafController _giraf;

        public ManageController(
          GirafDbContext context,
          UserManager<GirafUser> userManager,
          SignInManager<GirafUser> signInManager,
          IEmailSender emailSender,
          ILoggerFactory loggerFactory)
        {
            _giraf = new GirafController(context, userManager, loggerFactory.CreateLogger<ManageController>());
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
        [Authorize]
        public async Task<IActionResult> CreateUserIcon() {
            var usr = await _giraf._userManager.GetUserAsync(HttpContext.User);
            if(usr.UserIcon != null) return BadRequest("The user already has an icon - please PUT instead.");
            byte[] image = await _giraf.ReadRequestImage(HttpContext.Request.Body);
            usr.UserIcon = image;
            await _giraf._context.SaveChangesAsync();

            return Ok(usr);
        }
        [HttpPut("icon")]
        [Authorize]
        public async Task<IActionResult> UpdateUserIcon() {
            var usr = await _giraf._userManager.GetUserAsync(HttpContext.User);
            if(usr.UserIcon == null) return BadRequest("The user does not have an icon - please POST instead.");
            byte[] image = await _giraf.ReadRequestImage(HttpContext.Request.Body);
            usr.UserIcon = image;
            await _giraf._context.SaveChangesAsync();

            return Ok(usr);
        }
        [HttpDelete("icon")]
        [Authorize]
        public async Task<IActionResult> DeleteUserIcon() {
            var usr = await _giraf._userManager.GetUserAsync(HttpContext.User);
            usr.UserIcon = null;
            await _giraf._context.SaveChangesAsync();

            return Ok(usr);
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
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
