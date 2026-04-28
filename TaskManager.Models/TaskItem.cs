using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Models
{
    public class TaskItem
    {
        private int id;
        private string title;
        private string description;
        private DateTime creationDate;
        private DateTime deadline;
        public Priority priority;
        public Category category;
        public State state;
    }
}
