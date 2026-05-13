namespace TaskManager.Models
{
    public class AppSettings
    {
        public string Theme { get; set; } = "Purple";
        public string DefaultSort { get; set; } = "Date";
        public int NotifyDays { get; set; } = 3;
        public string SaveLocation { get; set; } = "tasks.json";
    }
}