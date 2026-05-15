using System;
using System.Linq;
using System.Windows;

namespace TaskManager.BusinessLogic
{
    public static class ThemeManager
    {
        public static string CurrentTheme { get; private set; } = "PurpleTheme";

        public static void ApplyTheme(string themeName)
        {
            //MessageBox.Show($"Applying theme: '{themeName}'");
            if (string.IsNullOrEmpty(themeName))
                themeName = "PurpleTheme";
            //if (!themeName.EndsWith("Theme"))
            //    themeName += "Theme";
            CurrentTheme = themeName;

            var dict = new ResourceDictionary
            {
                Source = new Uri($"pack://application:,,,/Themes/{themeName}.xaml", UriKind.Absolute)
            };
            var mergedDicts = Application.Current.Resources.MergedDictionaries;
            var existing = mergedDicts.FirstOrDefault(d => d.Source?.OriginalString.Contains("Theme") == true);

            if (existing != null)
                mergedDicts.Remove(existing);

            mergedDicts.Add(dict);
        }
    }
}