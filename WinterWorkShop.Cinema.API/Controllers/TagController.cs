using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WinterWorkShop.Cinema.API.Controllers
{
    [Route("api/[controller]")]
    public class TagController : Controller
    {
        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<string> GetMoviesByTag()
        {



            return new string[] { "value1", "value2" };
        }

        
    }
}
