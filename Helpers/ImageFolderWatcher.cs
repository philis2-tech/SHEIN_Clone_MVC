using System;
using System.IO;

namespace SHEIN_Clone_MVC.Helpers
{
    public static class ImageFolderWatcher
    {
        private static FileSystemWatcher _watcher;

        public static void Start(string webRootPath)
        {
            if (_watcher != null)
                return; // prevent multiple watchers

            _watcher = new FileSystemWatcher
            {
                Path = webRootPath,
                Filter = "*.png",
                IncludeSubdirectories = true,
                EnableRaisingEvents = true
            };

            _watcher.Created += OnChanged;
            _watcher.Deleted += OnChanged;
            _watcher.Renamed += OnChanged;
            _watcher.Changed += OnChanged;

            Console.WriteLine("🔥 ImageFolderWatcher started...");
        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                Console.WriteLine($"📸 Image change detected: {e.Name}");

                // OPTIONAL: trigger resync here
                var products = ImageSyncService.GenerateProductsFromImages(
                    Path.GetDirectoryName(e.FullPath)
                );

                Console.WriteLine($"🔄 Regenerated products: {products?.Count ?? 0}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Watcher error: " + ex.Message);
            }
        }
    }
}