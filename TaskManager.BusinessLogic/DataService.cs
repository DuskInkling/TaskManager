using System.Text.Json;

namespace TaskManager.BusinessLogic
{
    /// <summary>
    /// Handles reading and writing task data to and from JSON files, and exporting to CSV.
    /// </summary>
    public class DataService
    {
        private readonly string filePath = "tasks.json";
        /// <summary>
        /// Serializes the list of tasks to JSON and saves it to the default file path.
        /// </summary>
        /// <param name="tasks">The list of <see cref="TaskItem"/> objects to save.</param>
        public void SaveToJson(List<TaskItem> tasks, string filePath)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(tasks, options);
                File.WriteAllText(filePath, json);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error saving tasks:{ex.Message}");
            }
        }
        /// <summary>
        /// Loads and deserializes the list of tasks from the default JSON file.
        /// Returns an empty list if the file does not exist or cannot be read.
        /// </summary>
        /// <returns>A <see cref="List{T}"/> of <see cref="TaskItem"/> objects, or an empty list on failure.</returns>
        /// <summary>
        public List<TaskItem> LoadFromJson()
        {
            try
            {
                if (!File.Exists(filePath))
                    return new List<TaskItem>();

                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<List<TaskItem>>(json) ?? new List<TaskItem>();
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error reading tasks {ex.Message}");
                return new List<TaskItem>();
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error accesing file{ex.Message}");
                return new List<TaskItem>();
            }   
        }

        /// Loads and deserializes the list of tasks from a specified JSON file path.
        /// Returns an empty list if the file does not exist or cannot be read.
        /// </summary>
        /// <param name="filePath">The full path to the JSON file to load from.</param>
        /// <returns>A <see cref="List{T}"/> of <see cref="TaskItem"/> objects, or an empty list on failure.</returns>
        public List<TaskItem> LoadFromJson(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return new List<TaskItem>();

                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<List<TaskItem>>(json) ?? new List<TaskItem>();
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error reading tasks {ex.Message}");
                return new List<TaskItem>();
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error accesing file{ex.Message}");
                return new List<TaskItem>();
            }
        }

        /// <summary>
        /// Exports the list of tasks to a CSV file at the specified path.
        /// Each row contains Id, Title, Description, CreationDate, Deadline, Priority, Category, and State.
        /// </summary>
        /// <param name="tasks">The list of <see cref="TaskItem"/> objects to export.</param>
        /// <param name="exportPath">The full file path where the CSV will be written.</param>
        public void ExportToCsv(List<TaskItem> tasks, string exportPath)
        {
            try
            {
                var lines = new List<string>();
                lines.Add("Id,Title,Description,CreationDate,Deadline,Priority,Category,State");
                foreach (var task in tasks)
                {
                    lines.Add($"{task.Id}," +
                        $"\"{task.Title}\"," +
                        $"\"{task.Description}\"," +
                        $"{task.CreationDate:yyyy-MM-dd}," +
                        $"{task.Deadline:yyyy-MM-dd}," +
                        $"{task.Priority}," +
                        $"{task.Category}," +
                        $"{task.State}");
                }
                var encoding = new System.Text.UTF8Encoding(true); // true = include BOM
                File.WriteAllLines(exportPath, lines, encoding);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Failed to export to CSV: {ex.Message}");
            }
        }
    }
}
