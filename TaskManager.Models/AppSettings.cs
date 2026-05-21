/// <summary>
/// Stores user-configurable application settings that are persisted between sessions.
/// </summary>
public class AppSettings
{
    /// <summary>Gets or sets the name of the active UI theme (e.g. "DarkTheme", "LightTheme", "PurpleTheme").</summary>
    public string Theme { get; set; } = "PurpleTheme";

    /// <summary>Gets or sets the default sort order applied to the task list on startup.</summary>
    public string DefaultSort { get; set; } = "Date";

    /// <summary>Gets or sets how many days before a deadline the user should be notified.</summary>
    public int NotifyDays { get; set; } = 3;

    /// <summary>Gets or sets the file path where tasks are saved and loaded from.</summary>
    public string SaveLocation { get; set; } = "tasks.json";
}