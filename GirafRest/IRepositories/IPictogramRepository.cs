using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GirafRest.Models;
using GirafRest.Models.DTOs;

namespace GirafRest.IRepositories
{
    public interface IPictogramRepository : IRepository<Models.Pictogram>
    {
        Task<Pictogram> FindResource(ResourceIdDTO resourceIdDTO);
        Task<Pictogram> FetchResourceWithId(ResourceIdDTO resourceIdDTO);
    }
}