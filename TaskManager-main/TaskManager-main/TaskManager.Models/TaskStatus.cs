namespace TaskManager.Models;

/// <summary>
/// Enumerare ce descrie starea curentă a unei sarcini în fluxul de lucru.
/// </summary>
/// <remarks>
/// <para>
/// Valorile numerice sunt atribuite explicit pentru a asigura stabilitatea
/// la serializare/deserializare JSON. Adăugarea unei stări noi se face
/// la sfârșit, pentru a păstra compatibilitatea fișierelor de date vechi.
/// </para>
/// <para>
/// Numele tipului <c>TaskStatus</c> coincide cu cel din <c>System.Threading.Tasks</c>
/// (clasa <c>System.Threading.Tasks.TaskStatus</c>). Pentru a evita conflicte,
/// codul ce folosește acest tip trebuie să specifice <c>using TaskManager.Models;</c>
/// sau să utilizeze numele complet calificat <c>TaskManager.Models.TaskStatus</c>.
/// </para>
/// </remarks>
public enum TaskStatus
{
    /// <summary>
    /// Sarcină nouă, încă nepreluată în lucru. Stare implicită la creare.
    /// </summary>
    New = 0,

    /// <summary>
    /// Sarcină aflată în lucru. Utilizatorul a început execuția dar nu a finalizat-o.
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// Sarcină finalizată. Considerată închisă; nu mai apare în notificările
    /// despre termenele ce expiră.
    /// </summary>
    Completed = 2
}
