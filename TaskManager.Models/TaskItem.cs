using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Models
{
    public class TaskItem
    {
        private int id { get; set; }
        private string title { get; set; }
        private string description { get; set; }
        private DateTime creationDate { get; set; }
        private DateTime deadline { get; set; }
        public Priority priority { get; set; }
        public Category category { get; set; }
        public State state { get; set; }
    }
}
