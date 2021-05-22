using System;
namespace SteganographyAPI.Model
{
    public class EncryptModel
    {
        public string id { get; set; }
        public string message { get; set; }
        public string key { get; set; }
        public string weight { get; set; }

        public EncryptModel()
        {
        }
    }
}
