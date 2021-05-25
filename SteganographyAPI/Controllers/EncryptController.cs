using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SteganographyAPI.Common;
using SteganographyAPI.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SteganographyAPI.Controllers
{
    [Route("api/[controller]")]
    public class EncryptController : Controller
    {
        [HttpPost]
        public IActionResult Post([FromBody] EncryptModel model)
        {
            var name = model.id + ".bmp";
            var message = model.message;
            var key = model.key;
            var weight = model.weight;

            Exception exception;
            Bitmap bitmap = SteganographyHelper.encrypt(name, message, key, weight, out exception);
            //SteganographyHelper.encryptAndDecrypt(name, message, key, weight, out exception);
            //SteganographyHelper.fake(name, "a", "fake", "fake", out exception);

            if (exception != null)
            {
                return StatusCode(500, $"Internal server error: {exception}");
            }

            string id = SteganographyHelper.MD5Hash(DateTime.Now.ToString());
            bitmap.Save(Path.Combine(FileManager.resultFolder(), id + ".bmp"));
            return Ok(new { id, messageBinary = SteganographyHelper.textToBin(message, 8)});
        }
    }
}
