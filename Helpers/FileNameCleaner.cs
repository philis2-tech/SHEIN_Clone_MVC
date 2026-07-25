using System.Text.RegularExpressions;

public static class FileNameCleaner
{
    public static void CleanAllImages(string webRootPath)
    {
        var files = Directory.GetFiles(webRootPath, "*.png", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            var directory = Path.GetDirectoryName(file);
            var fileName = Path.GetFileName(file);

            var cleanName = Normalize(fileName);

            if (fileName != cleanName)
            {
                var newPath = Path.Combine(directory, cleanName);

                if (!File.Exists(newPath))
                {
                    File.Move(file, newPath);
                    Console.WriteLine($"Renamed: {fileName} → {cleanName}");
                }
            }
        }
    }

    private static string Normalize(string fileName)
    {
        var name = fileName.ToLower();

        // remove apostrophes
        name = name.Replace("'", "");

        // replace spaces with dash
        name = name.Replace(" ", "-");

        // remove invalid characters
        name = Regex.Replace(name, @"[^a-z0-9\-.]", "");

        // fix multiple dashes
        name = Regex.Replace(name, @"-+", "-");

        return name;
    }
}
