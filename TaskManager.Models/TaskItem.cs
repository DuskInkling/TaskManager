/// <summary>
/// Represents a single task in the task manager, containing all relevant metadata.
/// </summary>
public class TaskItem
{
    /// <summary>Gets or sets the unique identifier of the task.</summary>
    public int Id { get; set; }

    /// <summary>Gets or sets the title (name) of the task.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Gets or sets the detailed description of the task.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Gets or sets the date and time when the task was created.</summary>
    public DateTime CreationDate { get; set; } = DateTime.Now;

    /// <summary>Gets or sets the deadline by which the task must be completed.</summary>
    public DateTime Deadline { get; set; }

    /// <summary>Gets or sets the priority level of the task (Low, Medium, High).</summary>
    public Priority Priority { get; set; }

    /// <summary>Gets or sets the category the task belongs to (e.g. Work, Study, Home).</summary>
    public Category Category { get; set; }

    /// <summary>Gets or sets the current state of the task (New, InProgress, Completed).</summary>
    public State State { get; set; } = State.New;
}