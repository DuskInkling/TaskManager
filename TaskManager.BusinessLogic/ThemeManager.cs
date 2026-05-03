using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
