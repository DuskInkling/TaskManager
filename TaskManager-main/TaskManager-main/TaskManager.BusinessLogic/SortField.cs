namespace TaskManager.BusinessLogic;

/// <summary>
/// Enumerare ce indică câmpul după care se face sortarea în
/// <see cref="TaskFilter.Apply"/>.
/// </summary>
/// <remarks>
/// Folosit împreună cu <see cref="SortDirection"/> pentru a configura
/// comportamentul implicit al listei de sarcini afișate în UI.
/// </remarks>
public enum SortField
{
    /// <summary>
    /// Sortare după data creării (<see cref="TaskManager.Models.TaskItem.CreatedAt"/>).
    /// </summary>
    CreatedAt = 0,

    /// <summary>
    /// Sortare după termenul-limită (<see cref="TaskManager.Models.TaskItem.DueDate"/>).
    /// Sarcinile fără termen apar la sfârșit la sortare ascendentă.
    /// </summary>
    DueDate = 1,

    /// <summary>
    /// Sortare după prioritate (<see cref="TaskManager.Models.TaskItem.Priority"/>).
    /// Folosește ordinea numerică a enum-ului <c>Priority</c>.
    /// </summary>
    Priority = 2,

    /// <summary>
    /// Sortare alfabetică după denumirea sarcinii.
    /// </summary>
    Title = 3
}

/// <summary>
/// Direcția sortării (ascendentă / descendentă).
/// </summary>
public enum SortDirection
{
    /// <summary>
    /// Sortare ascendentă (de la mic la mare, de la A la Z, de la veche la nouă).
    /// </summary>
    Ascending = 0,

    /// <summary>
    /// Sortare descendentă (invers).
    /// </summary>
    Descending = 1
}
