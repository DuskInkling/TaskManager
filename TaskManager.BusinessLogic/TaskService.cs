/// <summary>
/// Manages the in-memory list of tasks, providing CRUD operations, filtering, sorting, and search.
/// </summary>
namespace TaskManager.BusinessLogic
{
    public class TaskService
    {
        private List<TaskItem> tasks = new List<TaskItem>();
        private int nextId = 1;
        /// <summary>
        /// Adds a new task to the list and assigns it a unique ID.
        /// </summary>
        /// <param name="task">The <see cref="TaskItem"/> to add.</param>
        public void AddTask(TaskItem task) {
            task.Id = nextId++;
            tasks.Add(task);
        }
        /// <summary>
        /// Removes the task with the specified ID from the list.
        /// </summary>
        /// <param name="id">The unique identifier of the task to remove.</param>
        public void RemoveTask(int id) {
            var task = tasks.FirstOrDefault(x => x.Id == id);
            if (task != null)
            {
                tasks.Remove(task);
            }
        }
        /// <summary>
        /// Updates the fields of an existing task identified by ID with values from the updated object.
        /// </summary>
        /// <param name="id">The unique identifier of the task to update.</param>
        /// <param name="updated">A <see cref="TaskItem"/> containing the new field values.</param>
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
        /// <summary>
        /// Updates the fields of an existing task identified by ID with values from the updated object.
        /// </summary>
        /// <param name="id">The unique identifier of the task to update.</param>
        /// <param name="updated">A <see cref="TaskItem"/> containing the new field values.</param>
        public List<TaskItem> GetAllTasks() { return tasks; }
        /// <summary>
        /// Finds and returns a single task by its unique ID, or <c>null</c> if not found.
        /// </summary>
        /// <param name="id">The unique identifier of the task to retrieve.</param>
        /// <returns>The matching <see cref="TaskItem"/>, or <c>null</c> if no match exists.</returns>
        
        public TaskItem? GetTaskById(int id) => 
            tasks.FirstOrDefault(x => x.Id == id);
        /// <summary>
        /// Returns all tasks that match the specified state.
        /// </summary>
        /// <param name="state">The <see cref="State"/> to filter by.</param>
        /// <returns>A filtered <see cref="List{T}"/> of <see cref="TaskItem"/> objects.</returns>
        public List<TaskItem> FilterByState (State state) => 
            tasks.Where(x => x.State == state).ToList();
        /// <summary>
        /// Returns all tasks that match the specified priority level.
        /// </summary>
        /// <param name="priority">The <see cref="Priority"/> to filter by.</param>
        /// <returns>A filtered <see cref="List{T}"/> of <see cref="TaskItem"/> objects.</returns>

        public List<TaskItem> FilterByPriority(Priority priority) => 
            tasks.Where(x => x.Priority == priority).ToList();
        /// <summary>
        /// Returns all tasks that belong to the specified category.
        /// </summary>
        /// <param name="category">The <see cref="Category"/> to filter by.</param>
        /// <returns>A filtered <see cref="List{T}"/> of <see cref="TaskItem"/> objects.</returns>
        public List<TaskItem> FilterByCategory(Category category) => 
            tasks.Where(t => t.Category == category).ToList();
        /// <summary>
        /// Searches tasks whose title or description contains the given query string (case-insensitive).
        /// </summary>
        /// <param name="query">The search string to look for.</param>
        /// <returns>A <see cref="List{T}"/> of matching <see cref="TaskItem"/> objects.</returns>
        public List<TaskItem> Search(string query) =>
            tasks.Where(x =>
            x.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            x.Description.Contains(query, StringComparison.OrdinalIgnoreCase)).
            ToList();
        /// <summary>
        /// Returns all tasks sorted by their creation date.
        /// </summary>
        /// <param name="ascending">If <c>true</c>, sorts oldest first; if <c>false</c>, sorts newest first.</param>
        /// <returns>A sorted <see cref="List{T}"/> of <see cref="TaskItem"/> objects.</returns>
        public List<TaskItem> SortByDate(bool ascending = true) =>
            ascending
            ? tasks.OrderBy(x => x.CreationDate).ToList() 
            : tasks.OrderByDescending(x => x.CreationDate).ToList();
        /// <summary>
        /// Returns all tasks sorted by their deadline date.
        /// </summary>
        /// <param name="ascending">If <c>true</c>, sorts earliest deadline first; if <c>false</c>, latest first.</param>
        /// <returns>A sorted <see cref="List{T}"/> of <see cref="TaskItem"/> objects.</returns>
        public List<TaskItem> SortByDeadline(bool ascending = true) =>
            ascending
            ? tasks.OrderBy(x => x.Deadline).ToList()
            : tasks.OrderByDescending(x => x.Deadline).ToList();
        /// <summary>
        /// Returns all non-completed tasks whose deadline falls within the next specified number of days.
        /// </summary>
        /// <param name="withinDays">The number of days ahead to check. Defaults to 3.</param>
        /// <returns>A <see cref="List{T}"/> of expiring <see cref="TaskItem"/> objects, sorted by deadline.</returns>
        public List<TaskItem> GetExpiringTasks(int withinDays = 3) =>
            tasks.Where(x =>
            x.State != State.Completed &&
            x.Deadline >= DateTime.Now &&
            x.Deadline <= DateTime.Now.AddDays(withinDays))
            .OrderBy(x => x.Deadline).ToList();
        /// <summary>
        /// Replaces the current in-memory task list with a loaded list and recalculates the next available ID.
        /// </summary>
        /// <param name="loadedTasks">The list of <see cref="TaskItem"/> objects loaded from persistent storage.</param>
        public void LoadTasks(List<TaskItem> loadedTasks)
        {
            tasks = loadedTasks;
            nextId = tasks.Count > 0 ? tasks.Max(x => x.Id) + 1 : 1;
        }
    }
}
