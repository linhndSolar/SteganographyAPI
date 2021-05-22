using System;
using System.IO;

namespace SteganographyAPI
{
    public class FileManager
    {
        public FileManager()
        {
        }

        public static string imageFolder()
        {
            var folderName = Path.Combine("Resources", "Images");
            var path = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            return path;
        }

        public static string keyFolder()
        {
            var folderName = Path.Combine("Resources", "Keys");
            var path = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            return path;
        }

        public static string weightFolder()
        {
            var folderName = Path.Combine("Resources", "Weights");
            var path = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            return path;
        }

        public static string resultFolder()
        {
            var folderName = Path.Combine("Resources", "Result");
            var path = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            return path;
        }
    }
}
