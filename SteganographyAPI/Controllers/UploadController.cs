using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SteganographyAPI.Controllers
{
    [Route("api/[controller]")]
    public class UploadController : Controller
    {
        [HttpPost, DisableRequestSizeLimit]
        public IActionResult Post()
        {
            try
            {
                Image image;
                var file = Request.Form.Files[0];
                var folderName = Path.Combine("Resources", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (file.Length > 0)
                {
                    // var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var id = DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss");
                    var fileName = id +".bmp";
                    var fullPath = Path.Combine(pathToSave, fileName);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }

                    Save(file, fullPath);
                    return Ok(new { id });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        public static async void Save(IFormFile image, string pathImage)
        {
            using (var stream = image.OpenReadStream())
            using (var imgIS = Image.Load(stream, out IImageFormat format))
            using (var memoryStream = new MemoryStream())
            using (var fileStream = new FileStream(pathImage, FileMode.OpenOrCreate))
            {
                imgIS.SaveAsBmp(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                await memoryStream.CopyToAsync(fileStream).ConfigureAwait(false);
                fileStream.Flush();
                memoryStream.Close();
                fileStream.Close();
            }
        }
    }
}
