using GirafRest.Models;
using GirafRest.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;


namespace GirafRest.IRepositories
{
    public interface IWeekTemplateRepository : IRepository<GirafRest.Models.WeekTemplate>
    {
        public Task<GirafUser> GetUserAsync(ClaimsPrincipal principal);
        public Task UpdateUserWeekTemplateAsync(WeekTemplate weekTemplate);
        public Task<WeekTemplateNameDTO[]> GetAllUserWeekTemplatesAsync(GirafUser user);
        public Task<WeekTemplate> GetUserWeekTemplateAsync(long templateID);
        public Task AddWeekTemplateToDbCtxAsync(WeekTemplate weekTemplate);
        public Department GetUserDepartmentAsync(GirafUser user);
    }   
}