using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Models;
namespace TaskManager.BusinessLogic
{
    public class TaskService
    {
        private List<TaskItem> tasks = new List<TaskItem>();
        private int nextId = 1;
        public void AddTask() { }
        public void RemoveTask(int id) { }
        public void UpdateTask(int id) { }
        public List<TaskItem> GetAllTasks() { return tasks; }
    }
}
