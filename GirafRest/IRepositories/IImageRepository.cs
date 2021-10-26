using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GirafRest.Models;
namespace GirafRest.IRepositories
{
    
    public interface IImageRepository : IRepository<Byte[]>
    {
        Task<byte[]> ReadRequestImage(Stream bodyStream);
    }
}
