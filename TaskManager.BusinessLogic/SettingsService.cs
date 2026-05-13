using System.IO;
using System.Text.Json;
using TaskManager.Models;

namespace TaskManager.BusinessLogic
{
    public class SettingsService
    {
        private readonly string settingsPath = "settings.json";

        public AppSettings LoadSettings()
        {
            try
            {
                if (!File.Exists(settingsPath))
                    return new AppSettings();

                string json = File.ReadAllText(settingsPath);
                return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            }
            catch
            {
                return new AppSettings();
            }
        }

        public void SaveSettings(AppSettings settings)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(settings, options);
                File.WriteAllText(settingsPath, json);
            }
            catch { }
        }
    }
}