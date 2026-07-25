using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SHEIN_Clone_MVC.Helpers
{
    public static class ImageScanner
    {
        private static readonly string ImageRoot =
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

        public static void ScanForMissingImages()
        {
            if (!Directory.Exists(ImageRoot))
            {
                Console.WriteLine("⚠️ Image folder not found: " + ImageRoot);
                return;
            }

            Console.WriteLine("🔍 Scanning images at: " + ImageRoot);

            var files = Directory.GetFiles(ImageRoot, "*.png", SearchOption.AllDirectories);

            Console.WriteLine($"📦 Total PNG images found: {files.Length}");

            var issues = new List<string>();

            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);

                // check unsafe characters
                if (fileName.Contains(" ") || fileName.Contains("'"))
                {
                    issues.Add($"⚠️ Unsafe filename: {fileName}");
                }

                // check empty files (corruption check)
                if (new FileInfo(file).Length == 0)
                {
                    issues.Add($"❌ Empty file: {fileName}");
                }
            }

            if (issues.Any())
            {
                Console.WriteLine("❌ Issues found:");
                foreach (var issue in issues)
                    Console.WriteLine(issue);
            }
            else
            {
                Console.WriteLine("✅ All images are valid PNG files.");
            }
        }
    }
}