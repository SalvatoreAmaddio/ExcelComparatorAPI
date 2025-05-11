using System.Text.Json;

namespace ExcelComparatorAPI.Utils
{
    public class FileManager
    {
        public static void DeleteFile(string path) 
        {
            File.Delete(path);
        }

        public static async Task SaveAsJSONAsync(object obj)
        {
            string json = JsonSerializer.Serialize(obj, new JsonSerializerOptions
            {
                WriteIndented = true // makes it more readable
            });

            // Get desktop path
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = Path.Combine(desktopPath, "ComparedPages.json");

            // Save to file
            await File.WriteAllTextAsync(filePath, json);
        }
    }
}