using System.Text.Json;
namespace TaskManager.BusinessLogic
{
    /// <summary>
    /// Handles loading and saving application settings to a JSON file.
    /// </summary>
    public class SettingsService
    {
        private readonly string settingsPath = "settings.json";
        /// <summary>
        /// Loads application settings from the settings JSON file.
        /// Returns default settings if the file does not exist or an error occurs.
        /// </summary>
        /// <returns>An <see cref="AppSettings"/> object with the loaded or default values.</returns>

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

        /// <summary>
        /// Serializes and saves the provided settings object to the settings JSON file.
        /// </summary>
        /// <param name="settings">The <see cref="AppSettings"/> object to persist.</param>
        public void SaveSettings(AppSettings settings)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(settings, options);
                File.WriteAllText(settingsPath, json);
            }
            catch (Exception ex)
            {
                File.WriteAllText("debug_save_error.txt", ex.Message);
            }
        }
    }
}