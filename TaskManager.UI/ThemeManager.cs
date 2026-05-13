using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TaskManager.BusinessLogic
{
    public class ThemeManager
    {
        public static bool isDarkTheme { get; private set; } = false;

        public void ToggleTheme()
        {
            isDarkTheme = !isDarkTheme;
        }
        public static string GetCurrentTheme()
        {
            return isDarkTheme ? "Dark theme" : "Light Theme" ;
        }
        public static void ApplyTheme(string themeName)
        {
            var dict = new ResourceDictionary
            {
                Source = new Uri($"Themes/{themeName}.xaml", UriKind.Relative)
            };

            var existing = Application.Current.Resources.MergedDictionaries.FirstOrDefault(d => d.Source?.OriginalString.Contains("Theme") == true);
            if (existing != null) 
            {
                Application.Current.Resources.MergedDictionaries.Remove(existing);
            }
            Application.Current.Resources.MergedDictionaries.Insert(0, dict);
        }
    }
}
