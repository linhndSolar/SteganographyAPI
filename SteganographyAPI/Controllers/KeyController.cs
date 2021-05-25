using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SteganographyAPI.Controllers
{
    [Route("api/[controller]")]
    public class KeyController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            var keys = Directory.GetFiles(FileManager.keyFolder()).Select(x => x.ToString().Substring(x.ToString().LastIndexOf("/") + 1, x.ToString().LastIndexOf(".") - x.ToString().LastIndexOf("/") - 1));
            return Ok(new { keys });
        }
    }
}
