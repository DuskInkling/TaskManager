using System;
using TaskManager.Models;

namespace TaskManager.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public DateTime Deadline { get; set; }
        public Priority Priority { get; set; }
        public Category Category { get; set; }
        public State State { get; set; } = State.New;
    }
}