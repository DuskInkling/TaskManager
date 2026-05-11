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
using TaskManager.BusinessLogic;
using TaskManager.Models;
namespace TaskManager.UI
{
    /// <summary>
    /// Interaction logic for TaskWindow.xaml
    /// </summary>
    public partial class TaskWindow : Window
    {
        public TaskItem task {  get; private set; }
        public TaskWindow()
        {
            InitializeComponent();
        }
            private void btnSaveClick(object sender, RoutedEventArgs e)
            {
            if (!Validate())
            {
                return;
            }
            task = new TaskItem
                {
                Title = txtTitle.Text,
                Description = txtDescription.Text,
                Deadline = dpDeadline.SelectedDate ?? DateTime.Now.AddDays(GetDaysFromComboBox()),
                Priority = (Priority)cmbPriority.SelectedIndex,
                Category = (Category)cmbCategory.SelectedIndex,
                State = (State)cmbState.SelectedIndex
                };
            DialogResult = true;
            Close();
        }
        private void btnCancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult= false;
            Close();
        }
        private bool Validate()
        {
            if (string.IsNullOrEmpty(txtTitle.Text))
            {
                MessageBox.Show("Title can't be empty", "Title Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (dpDeadline.SelectedDate < DateTime.Now ) {
                MessageBox.Show("Can't be in the past ", "Deadline Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;    
            }
            if (cmbDays.SelectedIndex <= 0 && dpDeadline.SelectedDate == null)
            {
                MessageBox.Show("Please select a deadline ", "Deadline Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }
        private int GetDaysFromComboBox()
        {
            return cmbDays.SelectedIndex switch
            {
                1 => 1,
                2 => 3,
                3 => 5,
                4 => 7,
                5 => 14,
                6 => 30,
                _ => 1
            };
        }
        private void cmbDays_SelectionChanged(object sender, SelectionChangedEventArgs e) { 
            if (cmbDays.SelectedIndex > 0)
            {
                dpDeadline.SelectedDate = null;
            }
        }
        private void dpDeadLine_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dpDeadline.SelectedDate != null)
            {
                cmbDays.SelectedIndex = 0;
            }
        }
    }
}

