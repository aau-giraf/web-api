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

namespace GirafRest.Repositories
{
    public class WeekTemplateRepository :IWeekTemplateRepository
    {
        private readonly IGirafService          _giraf;
        private readonly GirafDbContext         _dbContext;
        private readonly Controller             _controllerInstance;
        public WeekTemplateRepository(IGirafService giraf, GirafDbContext dbContext, Controller controllerInstance)
        {
            _giraf                  =                  giraf;
            _dbContext              =              dbContext;
            _controllerInstance     =     controllerInstance;
        }
        public async Task<WeekTemplate> Get(long templateID)
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
        public WeekTemplateNameDTO[] GetAll()
        {
            // if (!await _authentication.HasTemplateAccess(_currentUser))
            //    return _controllerInstance.StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.NotAuthorized, "User does not have permission"));
            var currentUser = GetUserAsync().Result;
            var templates = _giraf._context.WeekTemplates
                             .Where(weekTemplate => weekTemplate.DepartmentKey == currentUser.DepartmentKey)
                             .Select(weekTemplate => new WeekTemplateNameDTO(weekTemplate.Name, weekTemplate.Id)).ToArray();
            return templates;
        }
        
        public async Task Add(WeekTemplate weekTemplate)
        {
            _giraf._context.WeekTemplates.Add(weekTemplate); 
            await _giraf._context.SaveChangesAsync();
        }
        public async Task UpdateTemplateAsync(WeekTemplate weekTemplate)
        {
            _giraf._context.WeekTemplates.Update(weekTemplate);
            await _giraf._context.SaveChangesAsync();
        }
      
        public async Task<GirafUser> GetUserAsync()
        {
            return await _giraf.LoadBasicUserDataAsync(_controllerInstance.User);
            
        }
        public async Task Remove(WeekTemplate weekTemplate)
        {
            _giraf._context.WeekTemplates.Remove(weekTemplate);
            await _giraf._context.SaveChangesAsync();
        }

        //Not used, implement when needed
        void RemoveRange(IEnumerable<WeekTemplate> entities)
        {
            throw new NotImplementedException();
        }
        //Not used, implement when needed
        IEnumerable<WeekTemplate> Find(Expression<Func<WeekTemplate, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public WeekTemplate Get(int id)
        {
            throw new NotImplementedException();
        }

        IEnumerable<WeekTemplate> IRepository<WeekTemplate>.GetAll()
        {
            throw new NotImplementedException();
        }

        IEnumerable<WeekTemplate> IRepository<WeekTemplate>.Find(Expression<Func<WeekTemplate, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        void IRepository<WeekTemplate>.Add(WeekTemplate entity)
        {
            throw new NotImplementedException();
        }

        public void AddRange(IEnumerable<WeekTemplate> entities)
        {
            throw new NotImplementedException();
        }

        void IRepository<WeekTemplate>.Remove(WeekTemplate entity)
        {
            throw new NotImplementedException();
        }

        void IRepository<WeekTemplate>.RemoveRange(IEnumerable<WeekTemplate> entities)
        {
            throw new NotImplementedException();
        }
    }
}
