using Microsoft.Win32;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TaskManager.BusinessLogic;
using TaskManager.Models;
using Microsoft.Win32;
namespace TaskManager.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly TaskService _taskService = new TaskService();
        private readonly DataService _dataService = new DataService();
        public MainWindow()
        {
            InitializeComponent();
            var tasks = _dataService.LoadFromJson();
            _taskService.LoadTasks(tasks);
            var settings = new SettingsService().LoadSettings();
            ThemeManager.ApplyTheme(settings.Theme);
            RefreshGrid();
        }
        private void NewTask_Click(object sender, RoutedEventArgs e)
        {
            var taskWindow = new TaskWindow();
            if (taskWindow.ShowDialog() == true)
            {
                _taskService.AddTask(taskWindow.task);
                _dataService.SaveToJson(_taskService.GetAllTasks());
                RefreshGrid();
            }
        }
        private void RefreshGrid()
        {
            taskList.ItemsSource = null;
            taskList.ItemsSource = _taskService.GetAllTasks();
        }
        private void OpenFile(object sender, RoutedEventArgs e) 
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON Files (*json)|*.json";
            if (openFileDialog.ShowDialog() == true)
            {
                string filepath = openFileDialog.FileName;
                var tasks = _dataService.LoadFromJson(filepath);
                _taskService.LoadTasks(tasks);
                RefreshGrid();
            }
        }
        private void Export(object sender, RoutedEventArgs e)
        {

            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "CSV files (*csv)|*.csv";
            save.FileName = "tasks";
            save.InitialDirectory = @"C:\";
            if (save.ShowDialog() == true) {
                var tasksToExport = _taskService.GetAllTasks();
                _dataService.ExportToCsv(tasksToExport, save.FileName);
            }
        }
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }
    }
}