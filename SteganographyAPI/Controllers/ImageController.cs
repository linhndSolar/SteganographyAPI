using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SteganographyAPI.Controllers
{
    [Route("api/[controller]")]
    public class ImageController : Controller
    {
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            Byte[] b = System.IO.File.ReadAllBytes(FileManager.imageFolder()+"/"+id+".bmp");
            return File(b, "image/bmp");
        }
    }
}
