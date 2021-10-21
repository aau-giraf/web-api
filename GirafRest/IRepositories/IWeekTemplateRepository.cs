using GirafRest.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace GirafRest.IRepositories
{
    public interface IWeekTemplateRepository : IRepository<GirafRest.Models.WeekTemplate>
    {
        public Task<GirafUser> GetUserAsync();
     }
}