using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GirafWebApi.Models;
using Microsoft.Data.Sqlite;
using GirafWebApi.Contexts;

namespace GirafWebApi.Controllers
{
    [Route("api/[controller]")]
    public class PublicPictogramController : Controller
    {
        GirafDbContext context;
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "There are no Pictograms" };
            
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "There is no pictogram with the ID " + id; //Pictogram.get(id);
            //context.Find(new Pictogram(), id);//Shiet no work
        }

        [HttpPost]
        public void Post([FromBody]string value)
        {
            context.Add(value);
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}