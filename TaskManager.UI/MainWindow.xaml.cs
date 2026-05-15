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
            var settings = new SettingsService().LoadSettings();
            ThemeManager.ApplyTheme(settings.Theme);
            InitializeComponent();
            var tasks = _dataService.LoadFromJson();
            _taskService.LoadTasks(tasks);
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
        private void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }
        public void ApplyFilters()
        {
            var tasks = _taskService.GetAllTasks();
            if (_taskService == null || Search == null || cmbState == null || cmbCategory == null || cmbPriority == null || cmbSort == null || taskList == null)
                return;
            if (cmbState.SelectedIndex > 0)
            {
                State state = (State)(cmbState.SelectedIndex - 1);
                tasks = tasks.Where(x => x.State == state).ToList();
            }
            if (cmbCategory.SelectedIndex > 0)
            {
                Category category = (Category)(cmbCategory.SelectedIndex - 1);
                tasks = tasks.Where(x => x.Category == category).ToList();
            }
            if (cmbPriority.SelectedIndex > 0)
            {
                Priority priority = (Priority)(cmbPriority.SelectedIndex - 1);
                tasks = tasks.Where(x => x.Priority == priority).ToList();
            }
            if (!string.IsNullOrEmpty(Search.Text) && Search.Text != "Search")
            {
                tasks = _taskService.Search(Search.Text);
            }
            tasks = cmbSort.SelectedIndex switch
            {
                1 => tasks.OrderByDescending(x => x.Priority).ToList(),
                2 => tasks.OrderByDescending(x => x.Deadline).ToList(),
                _ => tasks.OrderByDescending(x => x.CreationDate).ToList()
            };

            taskList.ItemsSource = null;
            taskList.ItemsSource = tasks;
        }
        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }
        private void EditTask_Click(object sender, RoutedEventArgs e) {
            int id = (int)((Button)sender).Tag;
            var task = _taskService.GetTaskById(id);
            if (task == null) return;

            var taskWindow = new TaskWindow(task);
            if (taskWindow.ShowDialog() == true)
            {
                _taskService.UpdateTask(id, taskWindow.task);
                _dataService.SaveToJson(_taskService.GetAllTasks());
                RefreshGrid();
            }
        }
        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).Tag;
            var task = _taskService.GetTaskById(id);
            if (task == null) return;

            MessageBoxResult result = MessageBox.Show($"Delete: '{task.Title}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                _taskService.RemoveTask(id);
                _dataService.SaveToJson(_taskService.GetAllTasks());
                RefreshGrid();
            }
        }
    }
}