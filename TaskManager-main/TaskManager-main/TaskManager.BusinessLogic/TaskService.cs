using System;
using System.Collections.Generic;
using System.Linq;
using TaskManager.Models;

namespace TaskManager.BusinessLogic;

/// <summary>
/// Serviciu central de business pentru gestionarea sarcinilor.
/// Combină depozitul (<see cref="ITaskRepository"/>) cu logica de manipulare
/// in-memory (validare, căutare după <c>Id</c>, aplicare filtre).
/// </summary>
/// <remarks>
/// <para>
/// <b>Responsabilități:</b>
/// </para>
/// <list type="bullet">
///   <item>Încărcare sarcini din depozit la pornirea aplicației (<see cref="LoadFromRepository"/>).</item>
///   <item>Operații CRUD asupra colecției in-memory (<see cref="Add"/>, <see cref="Update"/>, <see cref="Delete"/>).</item>
///   <item>Validarea câmpurilor obligatorii înainte de a accepta o sarcină.</item>
///   <item>Filtrare/sortare/căutare prin <see cref="TaskFilter"/>.</item>
///   <item>Persistare în depozit la cerere (<see cref="SaveToRepository"/>).</item>
/// </list>
/// <para>
/// <b>Decizia de design „in-memory + flush”:</b> toate operațiile CRUD modifică
/// doar colecția din memorie. Salvarea pe disc se face explicit (de UI) sau
/// periodic. Acest lucru reduce numărul de scrieri pe disc și permite operații
/// rapide chiar pentru mii de sarcini.
/// </para>
/// </remarks>
public class TaskService
{
    /// <summary>
    /// Depozitul folosit pentru persistarea sarcinilor.
    /// Setat în constructor și nu se modifică ulterior.
    /// </summary>
    private readonly ITaskRepository _repository;

    /// <summary>
    /// Lista internă de sarcini, încărcată din depozit la inițializare.
    /// Toate operațiile CRUD lucrează direct asupra acestei liste.
    /// </summary>
    private readonly List<TaskItem> _tasks;

    /// <summary>
    /// Inițializează serviciul cu depozitul specificat și încarcă sarcinile.
    /// </summary>
    /// <param name="repository">Implementarea <see cref="ITaskRepository"/> de folosit.</param>
    /// <exception cref="ArgumentNullException">Dacă <paramref name="repository"/> este <c>null</c>.</exception>
    /// <remarks>
    /// În cazul în care încărcarea inițială eșuează (fișier corupt, drepturi),
    /// excepția se propagă în sus pentru ca apelantul (UI) să poată decide
    /// cum o tratează (mesaj utilizator, recuperare, ieșire).
    /// </remarks>
    public TaskService(ITaskRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _tasks = _repository.Load();
    }

    /// <summary>
    /// Adaugă o sarcină nouă în colecție.
    /// </summary>
    /// <param name="task">Sarcina de adăugat. Nu poate fi <c>null</c>.</param>
    /// <exception cref="ArgumentNullException">Dacă <paramref name="task"/> este <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Dacă <see cref="TaskItem.Title"/> este gol.</exception>
    /// <remarks>
    /// Validare minimă: doar titlul este obligatoriu. Descrierea, termenul
    /// și celelalte câmpuri pot fi setate la valorile lor implicite.
    /// </remarks>
    public void Add(TaskItem task)
    {
        ArgumentNullException.ThrowIfNull(task);

        if (string.IsNullOrWhiteSpace(task.Title))
            throw new ArgumentException(
                "Titlul sarcinii este obligatoriu și nu poate fi gol.",
                nameof(task));

        // Asigurăm că Id-ul este unic — dacă apelantul a uitat să-l seteze
        // (Guid.Empty), generăm unul nou.
        if (task.Id == Guid.Empty)
            task.Id = Guid.NewGuid();

        _tasks.Add(task);
    }

    /// <summary>
    /// Actualizează o sarcină existentă identificată prin <see cref="TaskItem.Id"/>.
    /// </summary>
    /// <param name="updated">Obiectul ce conține valorile noi.</param>
    /// <exception cref="ArgumentNullException">Dacă <paramref name="updated"/> este <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Dacă titlul este gol.</exception>
    /// <exception cref="KeyNotFoundException">
    /// Dacă nicio sarcină nu are <c>Id</c> egal cu <c>updated.Id</c>.
    /// </exception>
    /// <remarks>
    /// Identificarea se face strict după <see cref="TaskItem.Id"/>.
    /// <see cref="TaskItem.CreatedAt"/> nu se modifică (rămâne cea originală).
    /// </remarks>
    public void Update(TaskItem updated)
    {
        ArgumentNullException.ThrowIfNull(updated);

        if (string.IsNullOrWhiteSpace(updated.Title))
            throw new ArgumentException(
                "Titlul sarcinii este obligatoriu și nu poate fi gol.",
                nameof(updated));

        var existing = _tasks.FirstOrDefault(t => t.Id == updated.Id)
            ?? throw new KeyNotFoundException(
                $"Sarcina cu Id={updated.Id} nu a fost găsită.");

        existing.CopyEditableFieldsFrom(updated);
    }

    /// <summary>
    /// Șterge sarcina identificată prin <paramref name="id"/>.
    /// </summary>
    /// <param name="id">Identificatorul sarcinii.</param>
    /// <returns><c>true</c> dacă sarcina a fost găsită și ștearsă; <c>false</c> dacă nu există.</returns>
    /// <remarks>
    /// Returnarea unui <c>bool</c> permite UI-ului să afișeze un mesaj diferit
    /// (eroare vs. confirmare) fără a fi nevoie de excepție pentru un caz
    /// relativ așteptat (ștergere dublă din două ferestre etc.).
    /// </remarks>
    public bool Delete(Guid id)
    {
        var item = _tasks.FirstOrDefault(t => t.Id == id);
        if (item == null) return false;

        _tasks.Remove(item);
        return true;
    }

    /// <summary>
    /// Returnează toate sarcinile fără filtrare.
    /// </summary>
    /// <returns>O copie nouă a listei interne.</returns>
    /// <remarks>
    /// Se returnează o copie pentru a împiedica apelantul să modifice direct
    /// colecția internă (ar putea ocoli validarea din <see cref="Add"/>/<see cref="Update"/>).
    /// </remarks>
    public List<TaskItem> GetAll()
    {
        return _tasks.ToList();
    }

    /// <summary>
    /// Returnează sarcina cu <see cref="TaskItem.Id"/> specificat sau <c>null</c>.
    /// </summary>
    /// <param name="id">Identificatorul căutat.</param>
    /// <returns>Sarcina găsită sau <c>null</c>.</returns>
    public TaskItem? GetById(Guid id)
    {
        return _tasks.FirstOrDefault(t => t.Id == id);
    }

    /// <summary>
    /// Aplică un filtru asupra sarcinilor și returnează rezultatul.
    /// </summary>
    /// <param name="filter">Criteriile de filtrare. Dacă este <c>null</c>, returnează toate sarcinile.</param>
    /// <returns>Lista filtrată conform <paramref name="filter"/>.</returns>
    public List<TaskItem> GetFiltered(TaskFilter? filter)
    {
        if (filter == null)
            return _tasks.ToList();

        return filter.Apply(_tasks).ToList();
    }

    /// <summary>
    /// Returnează lista sarcinilor restante (depășit termen, nefinalizate).
    /// </summary>
    /// <returns>Sarcinile cu <see cref="TaskItem.IsOverdue"/> = <c>true</c>.</returns>
    public List<TaskItem> GetOverdue()
    {
        return _tasks.Where(t => t.IsOverdue()).ToList();
    }

    /// <summary>
    /// Returnează sarcinile cu termen ce expiră în următoarele <paramref name="hours"/> ore.
    /// </summary>
    /// <param name="hours">Numărul de ore de avans pentru avertizare.</param>
    /// <returns>Lista sarcinilor ce satisfac <see cref="TaskItem.IsDueSoon"/>.</returns>
    public List<TaskItem> GetDueSoon(int hours)
    {
        return _tasks.Where(t => t.IsDueSoon(hours)).ToList();
    }

    /// <summary>
    /// Salvează colecția curentă în depozit.
    /// </summary>
    /// <exception cref="System.IO.IOException">Probleme de scriere a fișierului.</exception>
    /// <remarks>
    /// Apelată explicit de UI la momentele potrivite: după Add/Update/Delete sau
    /// la închiderea aplicației.
    /// </remarks>
    public void SaveToRepository()
    {
        _repository.Save(_tasks);
    }

    /// <summary>
    /// Reîncarcă lista din depozit, ignorând modificările in-memory nesalvate.
    /// </summary>
    /// <remarks>
    /// Util când utilizatorul vrea să anuleze toate modificările făcute în sesiunea
    /// curentă și să revină la starea ultimei salvări.
    /// </remarks>
    public void LoadFromRepository()
    {
        _tasks.Clear();
        _tasks.AddRange(_repository.Load());
    }

    /// <summary>
    /// Numărul total de sarcini din colecție.
    /// </summary>
    public int Count => _tasks.Count;
}
