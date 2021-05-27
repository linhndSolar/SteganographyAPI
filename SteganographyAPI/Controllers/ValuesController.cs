using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SteganographyAPI.Common;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SteganographyAPI.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET: api/values
        [HttpGet]
        public IActionResult Get()
        {
            string key = SteganographyHelper.generateKey(128,128);
            string weight = SteganographyHelper.generateWeight(128,128);
            return Ok(new {
                key, weight
            });
        }
    }
}
