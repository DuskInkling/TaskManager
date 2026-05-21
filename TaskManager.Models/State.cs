/// <summary>
/// Defines the possible states a task can be in during its lifecycle.
/// </summary>
public enum State
{
    /// <summary>The task has been created but work has not started.</summary>
    New,

    /// <summary>The task is currently being worked on.</summary>
    InProgress,

    /// <summary>The task has been finished.</summary>
    Completed
}