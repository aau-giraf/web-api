using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Models;

namespace GirafRest.IRepositories
{
    public interface IPictogramRepository : IRepository<Models.Pictogram>
    {
        Models.Pictogram GetPictogramID(long pictogramID);
    }
}