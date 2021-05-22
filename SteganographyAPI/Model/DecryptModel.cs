using System;
namespace SteganographyAPI.Model
{
    public class DecryptModel
    {
        public string id { get; set;}
        public string key { get; set; }
        public string weight { get; set; }

        public DecryptModel()
        {
        }
    }
}
