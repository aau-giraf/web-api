using GirafRest.Interfaces;
using GirafRest.Models.DTOs;
using GirafRest.Models.Responses;
using GirafRest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GirafServices.WeekPlanner
{
    public interface IWeekService
    {
        public Task<ErrorResponse> SetWeekFromDTO(WeekBaseDTO weekDTO, WeekBase week, IGirafService _giraf);
        public Task<bool> AddPictogramsToWeekday(Weekday to, WeekdayDTO from, IGirafService _giraf);


    }
}
