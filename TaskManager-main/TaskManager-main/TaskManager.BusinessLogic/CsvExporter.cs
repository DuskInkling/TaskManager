using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using TaskManager.Models;

namespace TaskManager.BusinessLogic;

/// <summary>
/// Utilitar pentru exportul listei de sarcini într-un fișier CSV
/// (Comma-Separated Values).
/// </summary>
/// <remarks>
/// <para>
/// <b>Format CSV produs:</b>
/// </para>
/// <list type="bullet">
///   <item>Separator de câmp: virgulă <c>,</c>.</item>
///   <item>Separator de linie: <c>\r\n</c> (CRLF — standardul RFC 4180, recunoscut de Excel).</item>
///   <item>Codificare: UTF-8 cu BOM (preambul) — Excel pe Windows are nevoie de BOM ca să
///   detecteze automat UTF-8 și să afișeze corect diacriticele române.</item>
///   <item>Câmpurile ce conțin virgulă, ghilimele sau salt de linie sunt încadrate în
///   ghilimele duble; ghilimelele din interior se dublează (RFC 4180).</item>
///   <item>Datele sunt formatate ISO 8601 (<c>yyyy-MM-dd HH:mm</c>) pentru consistență.</item>
///   <item>Prima linie este antetul cu numele coloanelor.</item>
/// </list>
/// <para>
/// <b>Atenție la regiune:</b> Excel în limba română interpretează virgula ca separator
/// zecimal și folosește <c>;</c> drept separator CSV. Pentru a evita confuzia, fișierul
/// poate fi importat manual prin „Data → Import text” și ales separator virgulă.
/// </para>
/// </remarks>
public static class CsvExporter
{
    /// <summary>
    /// Exportă sarcinile în fișierul CSV specificat.
    /// </summary>
    /// <param name="tasks">Colecția de sarcini de exportat.</param>
    /// <param name="filePath">Calea fișierului-țintă. Va fi suprascris dacă există.</param>
    /// <exception cref="ArgumentNullException">Dacă <paramref name="tasks"/> sau <paramref name="filePath"/> este <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Dacă <paramref name="filePath"/> este șir gol.</exception>
    /// <exception cref="IOException">Probleme de scriere (drepturi, disc plin etc.).</exception>
    public static void Export(IEnumerable<TaskItem> tasks, string filePath)
    {
        ArgumentNullException.ThrowIfNull(tasks);
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Calea fișierului nu poate fi goală.", nameof(filePath));

        // StringBuilder este mai eficient decât concatenare cu „+” pentru
        // formarea unui șir mare în mai multe iterații.
        var sb = new StringBuilder();

        // Antet: lista coloanelor în ordinea câmpurilor.
        sb.AppendLine("Id,Titlu,Descriere,DataCrearii,Termen,Prioritate,Categorie,Stare");

        foreach (var t in tasks)
        {
            // Formăm rândul cu câmpurile escape-uite individual.
            sb.Append(EscapeCsv(t.Id.ToString())).Append(',');
            sb.Append(EscapeCsv(t.Title)).Append(',');
            sb.Append(EscapeCsv(t.Description)).Append(',');
            sb.Append(EscapeCsv(t.CreatedAt.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture))).Append(',');
            sb.Append(EscapeCsv(t.DueDate?.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture) ?? string.Empty)).Append(',');
            sb.Append(EscapeCsv(t.Priority.ToString())).Append(',');
            sb.Append(EscapeCsv(t.Category.ToString())).Append(',');
            sb.Append(EscapeCsv(t.Status.ToString()));
            sb.Append("\r\n");
        }

        // UTF-8 cu BOM pentru ca Excel să afișeze corect diacriticele.
        File.WriteAllText(filePath, sb.ToString(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
    }

    /// <summary>
    /// Aplică regulile de escape CSV asupra unui câmp:
    /// dublează ghilimelele și încadrează câmpul în ghilimele dacă conține
    /// virgulă, ghilimele sau salt de linie.
    /// </summary>
    /// <param name="field">Valoarea câmpului. Poate fi <c>null</c> — tratată ca șir gol.</param>
    /// <returns>Șirul gata de a fi scris în CSV.</returns>
    /// <remarks>
    /// Regulile sunt cele din RFC 4180. Decizia de a încadra în ghilimele
    /// se ia DOAR dacă este necesar — câmpurile simple rămân fără ghilimele
    /// pentru a face fișierul mai ușor de citit.
    /// </remarks>
    private static string EscapeCsv(string? field)
    {
        if (field == null) return string.Empty;

        bool needsQuoting =
            field.Contains(',') ||
            field.Contains('"') ||
            field.Contains('\r') ||
            field.Contains('\n');

        if (!needsQuoting)
            return field;

        // Dublăm ghilimelele și încadrăm tot șirul în ghilimele.
        return "\"" + field.Replace("\"", "\"\"") + "\"";
    }
}
