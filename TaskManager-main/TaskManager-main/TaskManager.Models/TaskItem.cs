using System;

namespace TaskManager.Models;

/// <summary>
/// Reprezintă o sarcină (task) personală în aplicația Task Manager.
/// Conține toate atributele necesare pentru a descrie, urmări și
/// persista o sarcină între sesiunile aplicației.
/// </summary>
/// <remarks>
/// <para>
/// Această clasă este un <i>model de date pur</i> (Plain Old CLR Object — POCO):
/// nu conține logică de business, validare complexă sau acces la fișiere.
/// Toate operațiile asupra sarcinilor (validare, filtrare, salvare) sunt
/// concentrate în stratul <c>TaskManager.BusinessLogic</c>.
/// </para>
/// <para>
/// Toate proprietățile sunt publice cu <c>get</c> și <c>set</c> pentru a permite
/// serializarea automată cu <c>System.Text.Json</c> și legarea bidirecțională
/// la controale WinForms (<c>BindingSource</c>, <c>DataGridView</c>).
/// </para>
/// <para>
/// Identificarea unică se face prin proprietatea <see cref="Id"/> de tip <see cref="Guid"/>,
/// generată automat la creare. Acest lucru permite editarea și ștergerea sigură
/// chiar dacă două sarcini au denumiri identice.
/// </para>
/// </remarks>
public class TaskItem
{
    /// <summary>
    /// Identificator unic al sarcinii.
    /// </summary>
    /// <remarks>
    /// Se inițializează automat cu <see cref="Guid.NewGuid"/> la construcția obiectului.
    /// Folosit ca cheie primară în colecția <c>List&lt;TaskItem&gt;</c> și ca referință
    /// în UI pentru identificarea rândului selectat în <c>DataGridView</c>.
    /// Nu trebuie modificat după creare.
    /// </remarks>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Denumirea (titlul) sarcinii. Câmp obligatoriu.
    /// </summary>
    /// <remarks>
    /// Validarea (lungime &gt; 0) se efectuează în stratul de business
    /// (<c>TaskService.Add</c> și <c>TaskService.Update</c>).
    /// Inițializat la șir gol pentru a evita <c>null</c> și a simplifica
    /// legarea cu <c>TextBox.DataBindings</c>.
    /// </remarks>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Descrierea detaliată a sarcinii. Câmp opțional.
    /// </summary>
    /// <remarks>
    /// Poate conține mai multe linii de text. Folosit la căutare împreună
    /// cu <see cref="Title"/>. Inițializat la șir gol pentru același motiv
    /// ca <see cref="Title"/>.
    /// </remarks>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Data și ora la care a fost creată sarcina.
    /// </summary>
    /// <remarks>
    /// Se completează automat cu <see cref="DateTime.Now"/> la construcția obiectului.
    /// Nu se modifică ulterior — chiar și la <c>Update</c> rămâne data inițială.
    /// Folosit la sortarea „cele mai noi sarcini primele”.
    /// </remarks>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// Termenul-limită până la care sarcina trebuie finalizată.
    /// Poate fi <c>null</c> dacă sarcina nu are dată impusă.
    /// </summary>
    /// <remarks>
    /// Tipul este <see cref="Nullable{DateTime}"/> pentru a permite explicit
    /// absența termenului. La sortare, sarcinile fără termen apar la sfârșit
    /// (sau la început, în funcție de direcția sortării).
    /// Folosit de <c>NotificationService</c> pentru a detecta sarcinile
    /// ce expiră în curând.
    /// </remarks>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Nivelul de prioritate al sarcinii.
    /// </summary>
    /// <remarks>
    /// Implicit <see cref="Models.Priority.Medium"/>. Vezi <see cref="Models.Priority"/>.
    /// </remarks>
    public Priority Priority { get; set; } = Priority.Medium;

    /// <summary>
    /// Categoria (domeniul) căreia îi aparține sarcina.
    /// </summary>
    /// <remarks>
    /// Implicit <see cref="Models.Category.Other"/>. Vezi <see cref="Models.Category"/>.
    /// </remarks>
    public Category Category { get; set; } = Category.Other;

    /// <summary>
    /// Starea curentă a sarcinii în fluxul de lucru.
    /// </summary>
    /// <remarks>
    /// Implicit <see cref="TaskStatus.New"/>. Vezi <see cref="TaskStatus"/>.
    /// </remarks>
    public TaskStatus Status { get; set; } = TaskStatus.New;

    /// <summary>
    /// Constructor implicit. Necesar pentru deserializarea cu <c>System.Text.Json</c>
    /// și pentru legarea cu controale WinForms (<c>BindingSource</c> apelează
    /// constructorul fără parametri).
    /// </summary>
    public TaskItem()
    {
    }

    /// <summary>
    /// Creează o copie superficială (shallow copy) a sarcinii curente.
    /// </summary>
    /// <returns>O instanță nouă <see cref="TaskItem"/> cu toate câmpurile copiate.</returns>
    /// <remarks>
    /// Util în UI: la deschiderea formularului de editare se lucrează pe o copie,
    /// iar dacă utilizatorul apasă „Anulare” modificările nu afectează originalul.
    /// Toate câmpurile sunt tipuri valoare sau <see cref="string"/> (imutabil),
    /// deci o copie superficială este suficientă — nu este nevoie de copie profundă.
    /// </remarks>
    public TaskItem Clone()
    {
        return new TaskItem
        {
            Id = this.Id,
            Title = this.Title,
            Description = this.Description,
            CreatedAt = this.CreatedAt,
            DueDate = this.DueDate,
            Priority = this.Priority,
            Category = this.Category,
            Status = this.Status
        };
    }

    /// <summary>
    /// Copiază valorile tuturor câmpurilor editabile din alt obiect <see cref="TaskItem"/>
    /// în obiectul curent. <see cref="Id"/> și <see cref="CreatedAt"/> NU sunt copiate.
    /// </summary>
    /// <param name="source">Obiectul-sursă din care se preiau valorile.</param>
    /// <exception cref="ArgumentNullException">
    /// Aruncată dacă <paramref name="source"/> este <c>null</c>.
    /// </exception>
    /// <remarks>
    /// Folosit la confirmarea editării (butonul „OK” din <c>TaskEditForm</c>):
    /// se preiau noile valori introduse de utilizator în obiectul existent
    /// fără a modifica identitatea (<see cref="Id"/>) sau data creării.
    /// </remarks>
    public void CopyEditableFieldsFrom(TaskItem source)
    {
        ArgumentNullException.ThrowIfNull(source);

        Title = source.Title;
        Description = source.Description;
        DueDate = source.DueDate;
        Priority = source.Priority;
        Category = source.Category;
        Status = source.Status;
    }

    /// <summary>
    /// Verifică dacă sarcina este restantă (depășit termenul-limită) la momentul curent.
    /// </summary>
    /// <returns>
    /// <c>true</c> dacă <see cref="DueDate"/> este definit, este în trecut,
    /// iar sarcina nu este în starea <see cref="TaskStatus.Completed"/>;
    /// altfel <c>false</c>.
    /// </returns>
    /// <remarks>
    /// O sarcină finalizată nu este considerată restantă, chiar dacă termenul a trecut.
    /// </remarks>
    public bool IsOverdue()
    {
        return DueDate.HasValue
               && DueDate.Value.Date < DateTime.Today
               && Status != TaskStatus.Completed;
    }

    /// <summary>
    /// Verifică dacă sarcina expiră în următoarele <paramref name="hours"/> ore.
    /// </summary>
    /// <param name="hours">
    /// Numărul de ore de avans pentru avertizare. Trebuie să fie pozitiv.
    /// </param>
    /// <returns>
    /// <c>true</c> dacă <see cref="DueDate"/> este definit, se află între
    /// <c>DateTime.Now</c> și <c>DateTime.Now + hours</c>, iar sarcina nu este finalizată.
    /// </returns>
    /// <remarks>
    /// Folosit de <c>NotificationService</c> pentru a alerta utilizatorul
    /// despre sarcinile cu termen ce se apropie. Comparația se face pe momentul exact
    /// (oră + minute), nu doar pe dată calendaristică.
    /// </remarks>
    public bool IsDueSoon(int hours)
    {
        if (!DueDate.HasValue) return false;
        if (Status == TaskStatus.Completed) return false;
        if (hours <= 0) return false;

        var now = DateTime.Now;
        var threshold = now.AddHours(hours);
        return DueDate.Value >= now && DueDate.Value <= threshold;
    }

    /// <summary>
    /// Returnează o reprezentare text scurtă a sarcinii, utilă la depanare
    /// și la afișarea în controale care apelează implicit <see cref="object.ToString"/>
    /// (cum ar fi <c>ListBox</c> fără <c>DisplayMember</c>).
    /// </summary>
    /// <returns>Șir de forma „Titlu [Stare, Prioritate]”.</returns>
    public override string ToString()
    {
        return $"{Title} [{Status}, {Priority}]";
    }
}
