using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public void AddTask(TaskItem task) {
            task.Id = nextId++;
            tasks.Add(task);
        }
        public void RemoveTask(int id) {
            var task = tasks.FirstOrDefault(x => x.Id == id);
            if (task != null)
            {
                tasks.Remove(task);
            }
        }
        public void UpdateTask(int id, TaskItem updated) {
            var task = tasks.FirstOrDefault(x=> x.Id == id);
            if (task != null) { 
                task.Title = updated.Title;
                task.Description = updated.Description;
                task.Deadline = updated.Deadline;
                task.Priority = updated.Priority;
                task.Category = updated.Category;
                task.State = updated.State;
            }
        }
        public List<TaskItem> GetAllTasks() { return tasks; }
        public TaskItem? GetTaskById(int id) => 
            tasks.FirstOrDefault(x => x.Id == id);
        public List<TaskItem> FilterByState (State state) => 
            tasks.Where(x => x.State == state).ToList();
        public List<TaskItem> FilterByPriority(Priority priority) => 
            tasks.Where(x => x.Priority == priority).ToList();
        public List<TaskItem> FilterByCategory(Category category) => 
            tasks.Where(t => t.Category == category).ToList();
        public List<TaskItem> Search(string query) =>
            tasks.Where(x =>
            x.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            x.Description.Contains(query, StringComparison.OrdinalIgnoreCase)).
            ToList();
        public List<TaskItem> SortByDate(bool ascending = true) =>
            ascending
            ? tasks.OrderBy(x => x.CreationDate).ToList() 
            : tasks.OrderByDescending(x => x.CreationDate).ToList();
        public List<TaskItem> SortByDeadline(bool ascending = true) =>
            ascending
            ? tasks.OrderBy(x => x.Deadline).ToList()
            : tasks.OrderByDescending(x => x.Deadline).ToList();
        public List<TaskItem> GetExpiringTasks(int withinDays = 3) =>
            tasks.Where(x =>
            x.State != State.Completed &&
            x.Deadline >= DateTime.Now &&
            x.Deadline <= DateTime.Now.AddDays(withinDays))
            .OrderBy(x => x.Deadline).ToList();
        public void LoadTasks(List<TaskItem> loadedTasks)
        {
            tasks = loadedTasks;
            nextId = tasks.Count > 0 ? tasks.Max(x => x.Id) + 1 : 1;
        }
    }
}
