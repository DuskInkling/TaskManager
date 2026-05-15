using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TaskManager.Models;
using TaskManager.BusinessLogic;
using Microsoft.Win32;
using System.IO;
namespace TaskManager.UI
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public AppSettings settings { get; private set; }
        public readonly SettingsService _settingsService = new SettingsService();
        private AppSettings originalSettings;
        public SettingsWindow()
        {
            InitializeComponent();
            LoadCurrentSettings();
        }
        private void LoadCurrentSettings()
        {
            settings = _settingsService.LoadSettings();
            originalSettings = new AppSettings
            {
                Theme = settings.Theme,
                DefaultSort = settings.DefaultSort,
                NotifyDays = settings.NotifyDays,
                SaveLocation = settings.SaveLocation
            };
            cmbTheme.SelectedIndex = settings.Theme switch
            {
                "DarkTheme" => 1,
                "LightTheme" => 2,
                _ => 0
            };
            cmbDefaultSort.SelectedIndex = settings.DefaultSort switch
            {
                "Priority" => 1,
                "Deadline" => 2,
                _ => 0
            };
            cmbNotifyDays.SelectedIndex = settings.NotifyDays switch
            {
                1 => 0,
                5 => 2,
                7 => 3,
                _ => 1
            };
            txtSavePath.Text = settings.SaveLocation;
        }
        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Json files (*json)|*.json";
            dialog.FileName = "tasks";
            if (dialog.ShowDialog() == true)
                txtSavePath.Text = dialog.FileName;
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            settings = new AppSettings
            {
                Theme = (cmbTheme.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString() ?? "PurpleTheme",
                DefaultSort = (cmbDefaultSort.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString() ?? "Date",
                NotifyDays = cmbNotifyDays.SelectedIndex switch { 0 => 1, 2 => 5, 3 => 7, _ => 3 },
                SaveLocation = txtSavePath.Text
            };
            _settingsService.SaveSettings(settings);
            DialogResult = true;
            Close();
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _settingsService.SaveSettings(originalSettings);
            ThemeManager.ApplyTheme(originalSettings.Theme);
            DialogResult = false;
            Close();
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            _settingsService.SaveSettings(originalSettings);
            ThemeManager.ApplyTheme(originalSettings.Theme);
        }
        private void cmbTheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var theme = (cmbTheme.SelectedItem as ComboBoxItem)?.Content.ToString();
            ThemeManager.ApplyTheme(theme);
        }
    }
}

