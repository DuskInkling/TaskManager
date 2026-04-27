using System;
using System.IO;
using System.Windows.Forms;
using TaskManager.BusinessLogic;

namespace TaskManager.UI;

/// <summary>
/// Punctul de intrare al aplicației Task Manager.
/// </summary>
/// <remarks>
/// <para>
/// <b>Responsabilități:</b>
/// </para>
/// <list type="bullet">
///   <item>Configurarea sistemului de stiluri vizuale și DPI.</item>
///   <item>Determinarea căii fișierului de date (în <c>%AppData%\TaskManager\tasks.json</c>).</item>
///   <item>Construirea grafului de dependențe (<see cref="JsonTaskRepository"/> →
///   <see cref="TaskService"/> → <see cref="MainForm"/>).</item>
///   <item>Tratarea globală a excepțiilor neprevăzute.</item>
/// </list>
/// </remarks>
internal static class Program
{
    /// <summary>
    /// Numele subdirectorului din <c>%AppData%</c> în care se stochează datele aplicației.
    /// </summary>
    private const string AppDataFolderName = "TaskManager";

    /// <summary>
    /// Numele fișierului JSON cu sarcinile.
    /// </summary>
    private const string DataFileName = "tasks.json";

    /// <summary>
    /// Punctul de intrare al aplicației.
    /// </summary>
    /// <remarks>
    /// Atributul <c>[STAThread]</c> este obligatoriu pentru WinForms — garantează
    /// că thread-ul UI este Single-Threaded Apartment, necesar pentru
    /// componentele COM folosite de unele controale (de ex. dialogul de fișier).
    /// </remarks>
    [STAThread]
    private static void Main()
    {
        // Setare DPI awareness înainte de orice creare de control.
        // PerMonitorV2 = scalare per-monitor, calitate optimă pe ecrane high-DPI
        // (laptop modern, monitor 4K). Necesar pentru ca TableLayoutPanel-urile
        // să se adapteze corect.
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);

        // Configurare stiluri vizuale și GDI compatibility (apel obligatoriu în .NET 6+).
        ApplicationConfiguration.Initialize();

        // Tratare globală a excepțiilor pe thread-ul UI.
        Application.ThreadException += (sender, e) =>
        {
            MessageBox.Show(
                "A apărut o eroare neașteptată:\n\n" + e.Exception.Message,
                "Eroare",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        };

        // Tratare globală a excepțiilor pe alte thread-uri.
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            if (e.ExceptionObject is Exception ex)
            {
                MessageBox.Show(
                    "Eroare critică:\n\n" + ex.Message,
                    "Eroare critică",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        };

        // Calea fișierului de date: %AppData%\TaskManager\tasks.json
        // %AppData% = C:\Users\<utilizator>\AppData\Roaming
        string appDataDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            AppDataFolderName);
        string dataFilePath = Path.Combine(appDataDir, DataFileName);

        try
        {
            // Compoziția obiectelor — Dependency Injection manual.
            var repository = new JsonTaskRepository(dataFilePath);
            var service = new TaskService(repository);
            var notifications = new NotificationService(service);

            // Pornirea formularului principal.
            Application.Run(new MainForm(service, notifications));
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Aplicația nu a putut porni:\n\n" + ex.Message,
                "Eroare la pornire",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
