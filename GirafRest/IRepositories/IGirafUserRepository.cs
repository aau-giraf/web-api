using GirafRest.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GirafRest.IRepositories
{
    public interface IGirafUserRepository : IRepository<GirafRest.Models.GirafUser> {
        /// <summary>
        /// Loads the user with week schedules.
        /// </summary>
        /// <returns>The user with week schedules.</returns>
        /// <param name="id">Identifier of <see cref="GirafUser"/></param>
        public Task<GirafUser> GetWithWeekSchedules(string id);
    }
}
