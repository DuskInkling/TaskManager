namespace TaskManager.Models;

/// <summary>
/// Enumerare ce reprezintă nivelul de prioritate al unei sarcini.
/// </summary>
/// <remarks>
/// Valorile numerice sunt atribuite explicit pentru a garanta stabilitatea
/// la serializare în JSON. Modificarea valorilor numerice ar putea sparge
/// compatibilitatea cu fișierele de date existente, deci se recomandă
/// adăugarea de noi valori la sfârșit, fără a le schimba pe cele existente.
/// Ordinea valorilor (0, 1, 2) corespunde ordinii crescătoare de importanță,
/// ceea ce permite sortarea naturală prin <c>OrderBy(x =&gt; x.Priority)</c>.
/// </remarks>
public enum Priority
{
    /// <summary>
    /// Prioritate scăzută. Sarcina poate fi amânată fără consecințe semnificative.
    /// </summary>
    Low = 0,

    /// <summary>
    /// Prioritate medie. Valoare implicită pentru sarcinile noi.
    /// </summary>
    Medium = 1,

    /// <summary>
    /// Prioritate înaltă. Sarcina trebuie tratată cât mai curând posibil.
    /// </summary>
    High = 2
}
