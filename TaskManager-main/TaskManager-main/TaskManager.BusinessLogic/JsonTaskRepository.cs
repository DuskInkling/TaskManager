using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using TaskManager.Models;

namespace TaskManager.BusinessLogic;

/// <summary>
/// Implementare a <see cref="ITaskRepository"/> ce stochează lista de sarcini
/// într-un fișier JSON pe disc, folosind biblioteca standard
/// <see cref="System.Text.Json"/>.
/// </summary>
/// <remarks>
/// <para>
/// <b>Strategia de salvare „write-then-rename”:</b> metoda <see cref="Save"/>
/// scrie întâi datele într-un fișier temporar (cu sufixul <c>.tmp</c>),
/// apoi îl redenumește înlocuind fișierul original. Astfel, dacă procesul
/// eșuează la mijlocul scrierii (cădere de curent, eroare de I/O), fișierul
/// original rămâne neatins și datele nu se pierd.
/// </para>
/// <para>
/// <b>Tratarea cazurilor speciale:</b>
/// <list type="bullet">
///   <item>Fișier absent → <see cref="Load"/> returnează listă goală.</item>
///   <item>Fișier gol → <see cref="Load"/> returnează listă goală.</item>
///   <item>JSON corupt → se aruncă <see cref="JsonException"/>.</item>
///   <item>Drepturi insuficiente → se aruncă <see cref="IOException"/>.</item>
/// </list>
/// </para>
/// <para>
/// <b>Codificare:</b> se folosește <see cref="JavaScriptEncoder.UnsafeRelaxedJsonEscaping"/>
/// pentru a păstra în mod natural diacriticele române (ă, î, â, ș, ț) și chirilice,
/// fără a le transforma în secvențe escape Unicode (<c>\uXXXX</c>).
/// </para>
/// </remarks>
public class JsonTaskRepository : ITaskRepository
{
    /// <summary>
    /// Calea absolută către fișierul JSON care conține lista de sarcini.
    /// Setată o singură dată în constructor și nu se mai modifică ulterior.
    /// </summary>
    private readonly string _filePath;

    /// <summary>
    /// Opțiunile folosite la serializarea JSON.
    /// Cache-ate într-un câmp pentru a evita realocarea la fiecare apel <see cref="Save"/>.
    /// </summary>
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        // Formatare cu indentare pentru a face fișierul ușor de citit/editat manual.
        WriteIndented = true,

        // Codificator relaxat — păstrează diacriticele române și caracterele non-ASCII
        // în forma lor naturală în loc de a le transforma în \uXXXX.
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,

        // Permite ca proprietățile lipsă din JSON să nu blocheze deserializarea
        // (util la migrări de schemă).
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Inițializează un nou depozit JSON, asociat cu fișierul specificat.
    /// </summary>
    /// <param name="filePath">
    /// Calea către fișierul de date. Poate fi cale absolută sau relativă.
    /// Nu este obligatoriu ca fișierul să existe — la prima salvare va fi creat.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Aruncată dacă <paramref name="filePath"/> este <c>null</c> sau șir gol.
    /// </exception>
    /// <remarks>
    /// Constructorul nu deschide fișierul și nu face validări costisitoare —
    /// doar memorează calea. Operațiile I/O se execută explicit la
    /// <see cref="Load"/>/<see cref="Save"/>.
    /// </remarks>
    public JsonTaskRepository(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException(
                "Calea către fișier nu poate fi nulă sau goală.",
                nameof(filePath));

        _filePath = filePath;
    }

    /// <summary>
    /// Încarcă toate sarcinile din fișierul JSON.
    /// </summary>
    /// <returns>
    /// O listă <see cref="List{TaskItem}"/> cu sarcinile citite. Dacă fișierul
    /// nu există sau este gol, returnează o listă goală.
    /// </returns>
    /// <exception cref="IOException">
    /// Probleme de citire (fișier blocat, drepturi insuficiente etc.).
    /// </exception>
    /// <exception cref="JsonException">
    /// Conținut JSON nevalid.
    /// </exception>
    /// <remarks>
    /// Metoda este sincronă pentru simplitate — pentru un manager personal
    /// volumul de date este mic (sute, max câteva mii de sarcini) și nu justifică
    /// complicarea cu API-ul asincron.
    /// </remarks>
    public List<TaskItem> Load()
    {
        // Cazul 1: fișierul nu există → prima rulare a aplicației.
        // Returnăm listă goală pentru a permite începutul de la zero.
        if (!File.Exists(_filePath))
        {
            return new List<TaskItem>();
        }

        // Citirea efectivă; pot apărea IOException (drepturi, blocaj) — propagate
        // în sus pentru ca UI să le poată afișa utilizatorului.
        string json = File.ReadAllText(_filePath);

        // Cazul 2: fișier gol sau cu doar spații albe.
        // Tratat la fel ca lipsa fișierului.
        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<TaskItem>();
        }

        // Deserializarea propriu-zisă.
        // System.Text.Json poate arunca JsonException dacă conținutul este corupt.
        var tasks = JsonSerializer.Deserialize<List<TaskItem>>(json, SerializerOptions);

        // Deserializer-ul poate returna null pentru un JSON literal "null".
        // Înlocuim cu listă goală pentru consistență.
        return tasks ?? new List<TaskItem>();
    }

    /// <summary>
    /// Salvează lista de sarcini în fișierul JSON, folosind strategia
    /// „write-then-rename” pentru atomicitate.
    /// </summary>
    /// <param name="tasks">Colecția de sarcini de salvat. Nu poate fi <c>null</c>.</param>
    /// <exception cref="ArgumentNullException">
    /// Aruncată dacă <paramref name="tasks"/> este <c>null</c>.
    /// </exception>
    /// <exception cref="IOException">
    /// Probleme de scriere (disc plin, drepturi insuficiente, fișier blocat).
    /// </exception>
    /// <remarks>
    /// <para>
    /// Pașii operațiunii:
    /// </para>
    /// <list type="number">
    ///   <item>Convertește colecția în <see cref="List{T}"/> (forțează enumerarea).</item>
    ///   <item>Creează directorul-părinte dacă nu există.</item>
    ///   <item>Serializează în JSON în memorie.</item>
    ///   <item>Scrie într-un fișier temporar <c>filePath + ".tmp"</c>.</item>
    ///   <item>Înlocuiește fișierul original cu cel temporar (<see cref="File.Move(string, string, bool)"/>).</item>
    /// </list>
    /// <para>
    /// Dacă apare o excepție în pașii 3-4, fișierul original rămâne neatins.
    /// Fișierul temporar este șters din block <c>finally</c> pentru a nu lăsa gunoi pe disc.
    /// </para>
    /// </remarks>
    public void Save(IEnumerable<TaskItem> tasks)
    {
        ArgumentNullException.ThrowIfNull(tasks);

        // Materializăm lista pentru a evita iterații multiple ale unei surse leneșe.
        var list = tasks.ToList();

        // Asigurăm existența directorului-părinte. Path.GetDirectoryName poate
        // returna null pentru o cale relativă fără separator (ex: "tasks.json").
        string? directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Calea fișierului temporar — la finalul operației îl redenumim.
        string tempPath = _filePath + ".tmp";

        try
        {
            // Serializarea în memorie. Dacă eșuează (din motive teoretice — TaskItem
            // este simplu și ar trebui să fie întotdeauna serializabil), nu am atins fișierul.
            string json = JsonSerializer.Serialize(list, SerializerOptions);

            // Scrierea în fișierul temporar.
            File.WriteAllText(tempPath, json);

            // Mutare atomică (pe sistemele de fișiere ce o suportă).
            // Parametrul `overwrite: true` permite înlocuirea fișierului original.
            File.Move(tempPath, _filePath, overwrite: true);
        }
        finally
        {
            // Curățare: dacă din orice motiv fișierul temporar a rămas, îl ștergem.
            // try/catch separat pentru a nu masca excepția originală.
            if (File.Exists(tempPath))
            {
                try { File.Delete(tempPath); }
                catch { /* înghițim: nu vrem să mascăm excepția originală */ }
            }
        }
    }
}
