using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SteganographyAPI.Common;
using SteganographyAPI.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SteganographyAPI.Controllers
{
    [Route("api/[controller]")]
    public class DecryptController : Controller
    {
        [HttpPost]
        public IActionResult Post([FromBody] DecryptModel model)
        {
            var name = model.id + ".bmp";
            var key = model.key;
            var weight = model.weight;

            Exception exception;
            string message = SteganographyHelper.decrypt(name, key, weight, out exception);

            if (exception != null)
            {
                return StatusCode(500, $"Internal server error: {exception}");
            }

            return Ok(new { message });
        }
    }
}
