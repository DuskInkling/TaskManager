using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Serialization;
using TaskManager.Models;

namespace TaskManager.BusinessLogic
{
    public class DataService
    {
        private readonly string filePath = "tasks.json";

        public void SaveToJson(List<TaskItem> tasks)
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
        public void ExportToCsv(List<TaskItem> tasks, string exportPath) { 
        try
            {
                var lines = new List<string>();
                lines.Add("Id,Title,Description,CreationDate,Deadline,Priority,Category,State");
                foreach (var task in tasks)
                {
                    lines.Add($"{task.Id}," +
                        $"{task.Title}," +
                        $"{task.Description}," +
                        $"{task.CreationDate}," +
                        $"{task.Deadline}," +
                        $"{task.Priority}," +
                        $"{task.Category}," +
                        $"{task.State},");
                }
                File.WriteAllLines(exportPath, lines);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Failed to export to CSV: {ex.Message}");
            }
        }
    }
}
