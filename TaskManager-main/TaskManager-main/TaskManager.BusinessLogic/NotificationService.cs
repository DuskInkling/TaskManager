using System;
using System.Collections.Generic;
using System.Linq;
using TaskManager.Models;

namespace TaskManager.BusinessLogic;

/// <summary>
/// Argumente pentru evenimentul <see cref="NotificationService.NotificationsRaised"/>.
/// </summary>
/// <remarks>
/// Conține lista sarcinilor pentru care s-a declanșat o notificare. UI-ul
/// folosește această listă pentru a afișa un pop-up sau o iconiță în tray.
/// </remarks>
public class NotificationsEventArgs : EventArgs
{
    /// <summary>
    /// Sarcinile cu termen ce expiră în curând.
    /// </summary>
    public IReadOnlyList<TaskItem> DueSoon { get; }

    /// <summary>
    /// Sarcinile deja restante (depășit termen, nefinalizate).
    /// </summary>
    public IReadOnlyList<TaskItem> Overdue { get; }

    /// <summary>
    /// Inițializează argumentele evenimentului.
    /// </summary>
    /// <param name="dueSoon">Lista sarcinilor ce expiră în curând.</param>
    /// <param name="overdue">Lista sarcinilor restante.</param>
    public NotificationsEventArgs(IReadOnlyList<TaskItem> dueSoon, IReadOnlyList<TaskItem> overdue)
    {
        DueSoon = dueSoon ?? Array.Empty<TaskItem>();
        Overdue = overdue ?? Array.Empty<TaskItem>();
    }

    /// <summary>
    /// <c>true</c> dacă există cel puțin o sarcină de raportat în vreuna din liste.
    /// </summary>
    public bool HasAny => DueSoon.Count > 0 || Overdue.Count > 0;
}

/// <summary>
/// Serviciu de notificări pentru sarcinile cu termen ce se apropie sau a trecut.
/// </summary>
/// <remarks>
/// <para>
/// Serviciul nu deține un timer propriu — UI-ul (sau apelantul) este responsabil
/// pentru a apela periodic <see cref="CheckNow"/> (de exemplu, dintr-un
/// <c>System.Windows.Forms.Timer</c> setat la fiecare 5 minute).
/// </para>
/// <para>
/// La fiecare verificare, dacă se găsesc sarcini noi în pragurile configurate,
/// se ridică evenimentul <see cref="NotificationsRaised"/>. Pentru a evita
/// notificări repetate pentru aceeași sarcină, se păstrează intern un set
/// cu <c>Id</c>-urile deja notificate.
/// </para>
/// </remarks>
public class NotificationService
{
    /// <summary>
    /// Numărul implicit de ore de avans pentru avertizare „due soon”.
    /// </summary>
    public const int DefaultLookaheadHours = 24;

    private readonly TaskService _taskService;
    private readonly HashSet<Guid> _alreadyNotified = new();

    /// <summary>
    /// Numărul de ore de avans (configurabil) pentru detectarea sarcinilor
    /// ce expiră în curând.
    /// </summary>
    public int LookaheadHours { get; set; } = DefaultLookaheadHours;

    /// <summary>
    /// Eveniment ridicat când există sarcini ce necesită atenția utilizatorului.
    /// </summary>
    /// <remarks>
    /// Argumentele evenimentului conțin DOAR sarcinile noi (nu și pe cele despre
    /// care utilizatorul a fost deja notificat). UI-ul nu trebuie să facă filtrare
    /// suplimentară — primește direct lista pentru afișare.
    /// </remarks>
    public event EventHandler<NotificationsEventArgs>? NotificationsRaised;

    /// <summary>
    /// Inițializează serviciul de notificări legat de un <see cref="TaskService"/>.
    /// </summary>
    /// <param name="taskService">Serviciul-sursă pentru lista de sarcini.</param>
    /// <exception cref="ArgumentNullException">Dacă <paramref name="taskService"/> este <c>null</c>.</exception>
    public NotificationService(TaskService taskService)
    {
        _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
    }

    /// <summary>
    /// Verifică imediat starea sarcinilor și ridică <see cref="NotificationsRaised"/>
    /// dacă există elemente noi de raportat.
    /// </summary>
    /// <remarks>
    /// Se apelează din UI pe un timer (de obicei la 5–15 minute). Sarcinile
    /// pentru care s-a notificat deja sunt ignorate până la <see cref="ResetNotificationHistory"/>.
    /// </remarks>
    public void CheckNow()
    {
        var dueSoonAll = _taskService.GetDueSoon(LookaheadHours);
        var overdueAll = _taskService.GetOverdue();

        // Filtrăm doar elementele despre care nu am notificat încă.
        var dueSoonNew = dueSoonAll.Where(t => !_alreadyNotified.Contains(t.Id)).ToList();
        var overdueNew = overdueAll.Where(t => !_alreadyNotified.Contains(t.Id)).ToList();

        if (dueSoonNew.Count == 0 && overdueNew.Count == 0)
            return;

        // Marcăm ca notificate.
        foreach (var t in dueSoonNew) _alreadyNotified.Add(t.Id);
        foreach (var t in overdueNew) _alreadyNotified.Add(t.Id);

        // Ridicăm evenimentul (dacă există abonați).
        var args = new NotificationsEventArgs(dueSoonNew, overdueNew);
        NotificationsRaised?.Invoke(this, args);
    }

    /// <summary>
    /// Resetează istoricul notificărilor — toate sarcinile vor fi din nou
    /// candidate pentru notificare la următoarea verificare.
    /// </summary>
    /// <remarks>
    /// Util la pornirea aplicației sau când utilizatorul vrea să fie reamintit.
    /// </remarks>
    public void ResetNotificationHistory()
    {
        _alreadyNotified.Clear();
    }
}
