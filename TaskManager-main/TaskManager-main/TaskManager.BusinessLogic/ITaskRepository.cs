using System.Collections.Generic;
using TaskManager.Models;

namespace TaskManager.BusinessLogic;

/// <summary>
/// Contract pentru orice depozit (repository) de sarcini.
/// Abstractizează modul concret de stocare a datelor (JSON, XML, bază de date)
/// pentru ca stratul de business să nu depindă de o implementare anume.
/// </summary>
/// <remarks>
/// <para>
/// Acest interfaț urmează principiul <b>Dependency Inversion</b> (D din SOLID):
/// <see cref="TaskService"/> nu cunoaște dacă datele sunt stocate într-un fișier
/// JSON, XML sau o bază de date — el doar invocă metodele acestui contract.
/// </para>
/// <para>
/// În aplicația curentă singura implementare este <see cref="JsonTaskRepository"/>,
/// dar la nevoie se pot adăuga alte implementări (de exemplu <c>XmlTaskRepository</c>
/// sau <c>SqliteTaskRepository</c>) fără modificări în <see cref="TaskService"/>.
/// </para>
/// </remarks>
public interface ITaskRepository
{
    /// <summary>
    /// Încarcă lista completă de sarcini din depozit.
    /// </summary>
    /// <returns>
    /// O listă <see cref="List{T}"/> de sarcini. Dacă fișierul nu există,
    /// implementarea trebuie să returneze o listă goală (nu <c>null</c>).
    /// </returns>
    /// <exception cref="System.IO.IOException">
    /// Aruncată când există fișierul dar nu poate fi citit (drepturi insuficiente,
    /// fișier blocat de alt proces etc.).
    /// </exception>
    /// <exception cref="System.Text.Json.JsonException">
    /// Aruncată când conținutul fișierului nu este JSON valid.
    /// </exception>
    List<TaskItem> Load();

    /// <summary>
    /// Salvează lista completă de sarcini în depozit, suprascriind conținutul existent.
    /// </summary>
    /// <param name="tasks">
    /// Lista de sarcini de salvat. Dacă este <c>null</c>, implementarea poate
    /// arunca <see cref="System.ArgumentNullException"/> sau o trata ca listă goală
    /// (în funcție de strategia aleasă; varianta recomandată este să arunce excepția).
    /// </param>
    /// <exception cref="System.IO.IOException">
    /// Aruncată dacă scrierea în fișier eșuează (lipsa drepturilor, disc plin etc.).
    /// </exception>
    /// <remarks>
    /// Operația trebuie să fie cât mai atomică posibil — implementările uzuale
    /// scriu mai întâi într-un fișier temporar, apoi îl redenumesc, pentru a evita
    /// pierderea datelor în caz de eroare la mijlocul scrierii.
    /// </remarks>
    void Save(IEnumerable<TaskItem> tasks);
}
