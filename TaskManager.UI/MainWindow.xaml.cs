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
            InitializeComponent();
            var tasks = _dataService.LoadFromJson();
            _taskService.LoadTasks(tasks);
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
    }
}