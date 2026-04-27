using System;
using System.Collections.Generic;
using System.Linq;
using TaskManager.Models;
// Alias pentru a rezolva conflictul cu System.Threading.Tasks.TaskStatus
// (introdusă automat prin ImplicitUsings în .NET 8).
using TaskStatus = TaskManager.Models.TaskStatus;

namespace TaskManager.BusinessLogic;

/// <summary>
/// Reprezintă un set de criterii de filtrare, căutare și sortare aplicabil
/// unei colecții de <see cref="TaskItem"/>.
/// </summary>
/// <remarks>
/// <para>
/// Toate proprietățile sunt opționale (<c>nullable</c>). Dacă o proprietate
/// rămâne <c>null</c> sau șir gol, criteriul respectiv nu se aplică (este
/// considerat „orice valoare”).
/// </para>
/// <para>
/// Filtrul se construiește în UI pe baza valorilor selectate de utilizator
/// în controale (<c>ComboBox</c>, <c>TextBox</c>) și se aplică prin
/// <see cref="Apply"/>, care folosește LINQ pentru construirea pipeline-ului.
/// </para>
/// <para>
/// Această clasă este <i>imutabilă din punct de vedere semantic</i> — se
/// recomandă crearea unei instanțe noi la fiecare modificare a UI, mai degrabă
/// decât mutarea unei singure instanțe. Totuși proprietățile sunt setabile
/// pentru a permite construirea pas cu pas (object initializer).
/// </para>
/// </remarks>
public class TaskFilter
{
    /// <summary>
    /// Filtrare după stare. Dacă <c>null</c>, sunt acceptate toate stările.
    /// </summary>
    public TaskStatus? Status { get; set; }

    /// <summary>
    /// Filtrare după categorie. Dacă <c>null</c>, sunt acceptate toate categoriile.
    /// </summary>
    public Category? Category { get; set; }

    /// <summary>
    /// Filtrare după prioritate. Dacă <c>null</c>, sunt acceptate toate prioritățile.
    /// </summary>
    public Priority? Priority { get; set; }

    /// <summary>
    /// Text de căutare aplicat asupra <see cref="TaskItem.Title"/> și
    /// <see cref="TaskItem.Description"/>. Dacă este <c>null</c> sau șir gol,
    /// căutarea nu se aplică.
    /// </summary>
    /// <remarks>
    /// Comparația este case-insensitive și se face cu <c>Contains</c> +
    /// <see cref="StringComparison.OrdinalIgnoreCase"/> pentru performanță maximă.
    /// </remarks>
    public string? SearchText { get; set; }

    /// <summary>
    /// Câmpul după care se face sortarea. Dacă este <c>null</c>, ordinea
    /// rămâne cea originală a colecției (de obicei ordinea de inserare).
    /// </summary>
    public SortField? SortBy { get; set; }

    /// <summary>
    /// Direcția sortării. Are efect doar dacă <see cref="SortBy"/> are valoare.
    /// </summary>
    public SortDirection SortDirection { get; set; } = SortDirection.Ascending;

    /// <summary>
    /// Aplică criteriile curente asupra unei colecții de sarcini și returnează
    /// rezultatul filtrat și sortat.
    /// </summary>
    /// <param name="source">Colecția-sursă. Nu poate fi <c>null</c>.</param>
    /// <returns>
    /// O secvență <see cref="IEnumerable{TaskItem}"/> ce conține sarcinile
    /// care satisfac toate criteriile, sortate conform <see cref="SortBy"/>
    /// și <see cref="SortDirection"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Aruncată dacă <paramref name="source"/> este <c>null</c>.
    /// </exception>
    /// <remarks>
    /// <para>
    /// Pipeline-ul folosește LINQ și execută filtrarea „lazy” (la enumerare).
    /// Dacă apelantul materializează rezultatul (<c>.ToList()</c>), apelarea
    /// se face o singură dată. Sortarea este stabilă (<c>OrderBy</c> garantează
    /// stabilitatea) — sarcinile cu aceeași cheie își păstrează ordinea inițială.
    /// </para>
    /// <para>
    /// La sortarea după <see cref="SortField.DueDate"/>, sarcinile fără termen
    /// (<c>DueDate == null</c>) sunt plasate la sfârșit la ascendent / la început
    /// la descendent — folosim <c>DateTime.MaxValue</c>/<c>MinValue</c> ca substitute.
    /// </para>
    /// </remarks>
    public IEnumerable<TaskItem> Apply(IEnumerable<TaskItem> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        IEnumerable<TaskItem> query = source;

        // Filtrare după stare.
        if (Status.HasValue)
        {
            var s = Status.Value;
            query = query.Where(t => t.Status == s);
        }

        // Filtrare după categorie.
        if (Category.HasValue)
        {
            var c = Category.Value;
            query = query.Where(t => t.Category == c);
        }

        // Filtrare după prioritate.
        if (Priority.HasValue)
        {
            var p = Priority.Value;
            query = query.Where(t => t.Priority == p);
        }

        // Căutare text — doar dacă utilizatorul a introdus ceva non-trivial.
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var needle = SearchText.Trim();
            query = query.Where(t =>
                (t.Title != null &&
                 t.Title.Contains(needle, StringComparison.OrdinalIgnoreCase))
                ||
                (t.Description != null &&
                 t.Description.Contains(needle, StringComparison.OrdinalIgnoreCase)));
        }

        // Sortare — doar dacă a fost cerută explicit.
        if (SortBy.HasValue)
        {
            query = SortBy.Value switch
            {
                SortField.CreatedAt =>
                    SortDirection == SortDirection.Ascending
                        ? query.OrderBy(t => t.CreatedAt)
                        : query.OrderByDescending(t => t.CreatedAt),

                SortField.DueDate =>
                    SortDirection == SortDirection.Ascending
                        ? query.OrderBy(t => t.DueDate ?? DateTime.MaxValue)
                        : query.OrderByDescending(t => t.DueDate ?? DateTime.MinValue),

                SortField.Priority =>
                    SortDirection == SortDirection.Ascending
                        ? query.OrderBy(t => t.Priority)
                        : query.OrderByDescending(t => t.Priority),

                SortField.Title =>
                    SortDirection == SortDirection.Ascending
                        ? query.OrderBy(t => t.Title, StringComparer.CurrentCultureIgnoreCase)
                        : query.OrderByDescending(t => t.Title, StringComparer.CurrentCultureIgnoreCase),

                _ => query
            };
        }

        return query;
    }
}
