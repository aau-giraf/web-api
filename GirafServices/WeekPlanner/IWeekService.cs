﻿using GirafEntities.Responses;
using GirafEntities.WeekPlanner;
using GirafEntities.WeekPlanner.DTOs;
using GirafServices.User;

namespace GirafServices.WeekPlanner
{
    public interface IWeekService
    {
        public Task<ErrorResponse> SetWeekFromDTO(WeekBaseDTO weekDTO, WeekBase week);
        public Task<bool> AddPictogramsToWeekday(Weekday to, WeekdayDTO from);
    }
}
