namespace TaskManager.Models;

/// <summary>
/// Enumerare ce reprezintă categoria (domeniul) căreia îi aparține o sarcină.
/// </summary>
/// <remarks>
/// <para>
/// Set predefinit de categorii uzuale pentru un manager personal de sarcini.
/// Implementarea folosește <c>enum</c> (și nu <c>string</c>) pentru a beneficia
/// de validare la compilare și de siguranța tipurilor în filtrele LINQ.
/// </para>
/// <para>
/// Pentru extindere ulterioară (de exemplu adăugarea categoriei „Sport”
/// sau „Familie”), se adaugă o valoare nouă la sfârșit. Nu se schimbă
/// niciodată valorile numerice existente — fișierele JSON salvate anterior
/// păstrează aceste numere și ar deveni invalide.
/// </para>
/// </remarks>
public enum Category
{
    /// <summary>
    /// Serviciu / muncă profesională. Sarcini legate de locul de muncă.
    /// </summary>
    Work = 0,

    /// <summary>
    /// Studii. Teme, examene, cursuri, materiale de citit.
    /// </summary>
    Study = 1,

    /// <summary>
    /// Casă / treburi domestice. Cumpărături, curățenie, reparații.
    /// </summary>
    Home = 2,

    /// <summary>
    /// Personal. Sănătate, hobby, dezvoltare personală.
    /// </summary>
    Personal = 3,

    /// <summary>
    /// Altele. Categorie de rezervă pentru sarcini ce nu se încadrează
    /// în niciuna dintre cele de mai sus.
    /// </summary>
    Other = 4
}
