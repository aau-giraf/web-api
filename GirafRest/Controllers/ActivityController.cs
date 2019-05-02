using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GirafRest.Services;
using GirafRest.Models.Responses;

namespace GirafRest.Controllers
{
    [Route("v2/activity")]
    public class ActivityController : Controller
    {
        private readonly IGirafService _giraf;

        private readonly IAuthenticationService _authentication;

        public ActivityController(IGirafService giraf, ILoggerFactory loggerFactory, IAuthenticationService authentication)
        {
            _giraf = giraf;
            _giraf._logger = loggerFactory.CreateLogger("Week");
            _authentication = authentication;
        }

        // Update Activity
        [HttpPut("update")]
        [Authorize]
        public async Task<Response<ActivityDTO>> UpdateActivity([FromBody] ActivityDTO activity)
        {
            if (activity == null) return new ErrorResponse<ActivityDTO>(ErrorCode.MissingProperties);

            Activity updateActivity = _giraf._context.Activities.FirstOrDefault(a => a.Key == activity.Id);

            if (updateActivity == null)
            {
                return new ErrorResponse<ActivityDTO>(ErrorCode.NotFound);
            }

            updateActivity.Key = activity.Id;
            updateActivity.Order = activity.Order;
            updateActivity.State = activity.State;
            //updateActivity.Pictogram = new Pictogram() { Id = activity.Pictogram.Id, };

            await _giraf._context.SaveChangesAsync();

            return new Response<ActivityDTO>(new ActivityDTO(updateActivity, activity.Pictogram));
        }
    }
}
