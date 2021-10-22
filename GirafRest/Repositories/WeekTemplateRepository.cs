using System.Collections.Generic;
using System.Linq;
using GirafRest.Models;
using GirafRest.IRepositories;
using GirafRest.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System;
using GirafRest.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using GirafRest.Models.Responses;
using Microsoft.AspNetCore.Http;
using GirafRest.Models.DTOs;
using System.Security.Claims;

namespace GirafRest.Repositories
{
    public class WeekTemplateRepository : Repository<WeekTemplate>, IWeekTemplateRepository
    {
        IGirafService _giraf;
        public WeekTemplateRepository(IGirafService giraf, GirafDbContext context) : base(context)
        {
            _giraf = giraf;
        }
        public async Task<WeekTemplate> GetUserWeekTemplateAsync(long templateID)
        {
            var template = await (_giraf._context.WeekTemplates
                .Include(weekTemplate => weekTemplate.Thumbnail)
                .Include(weekTemplate => weekTemplate.Weekdays)
                .ThenInclude(weekTemplate => weekTemplate.Activities)
                .ThenInclude(weekTemplate => weekTemplate.Pictograms)
                .ThenInclude(weekTemplate => weekTemplate.Pictogram)
                .Where(weekTemplate => weekTemplate.DepartmentKey == weekTemplate.DepartmentKey)
                .FirstOrDefaultAsync(weekTemplate => weekTemplate.Id == templateID));
            return template;
        }
        
         
        public async Task<WeekTemplateNameDTO[]> GetAllUserWeekTemplatesAsync(GirafUser user)
        {

            var templates = _giraf._context.WeekTemplates
                             .Where(weekTemplate => weekTemplate.DepartmentKey == user.DepartmentKey)
                             .Select(weekTemplate => new WeekTemplateNameDTO(weekTemplate.Name, weekTemplate.Id)).ToArray();
            return templates;
        }
        

        public async Task AddWeekTemplateToDbCtxAsync(WeekTemplate weekTemplate)
        {
            _giraf._context.WeekTemplates.Add(weekTemplate);
            await _giraf._context.SaveChangesAsync();
        }
        
        public async Task UpdateUserWeekTemplateAsync(WeekTemplate weekTemplate)
        {
            _giraf._context.WeekTemplates.Update(weekTemplate);
            await _giraf._context.SaveChangesAsync();
        }

        public async Task<GirafUser> GetUserAsync(ClaimsPrincipal principal)
        {
            return await _giraf.LoadBasicUserDataAsync(principal);

        }
        public new async Task Remove(WeekTemplate weekTemplate)
        {
            _giraf._context.WeekTemplates.Remove(weekTemplate);
            await _giraf._context.SaveChangesAsync();
        }

        public Department GetUserDepartmentAsync(GirafUser user)
        {
            var department = _giraf._context.Departments.FirstOrDefault(department => department.Key == user.DepartmentKey);
            return department;
        }
    }
}