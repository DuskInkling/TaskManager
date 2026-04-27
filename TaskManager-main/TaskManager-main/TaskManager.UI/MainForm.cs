using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TaskManager.BusinessLogic;
using TaskManager.Models;
// Alias pentru a rezolva conflictul cu System.Threading.Tasks.TaskStatus
using TaskStatus = TaskManager.Models.TaskStatus;

namespace TaskManager.UI;

/// <summary>
/// Formularul principal al aplicației Task Manager. Afișează lista de sarcini
/// într-un <see cref="DataGridView"/> și oferă comenzi pentru creare, editare,
/// ștergere, filtrare, sortare, căutare și exportul în CSV.
/// </summary>
/// <remarks>
/// <para>
/// Formularul primește prin constructor referințele la <see cref="TaskService"/>
/// și <see cref="NotificationService"/> — Dependency Injection manual,
/// fără container de tip Microsoft.Extensions.DependencyInjection.
/// </para>
/// <para>
/// Toate operațiile de afișare apelează <see cref="RefreshGrid"/>, care construiește
/// filtrul curent pe baza valorilor din controale și obține lista filtrată
/// de la serviciu.
/// </para>
/// </remarks>
public partial class MainForm : Form
{
    private readonly TaskService _service;
    private readonly NotificationService _notifications;

    /// <summary>
    /// Lista curentă afișată în grilă. Memorată pentru a putea referenția sarcina
    /// selectată în mod tipat.
    /// </summary>
    private List<TaskItem> _currentView = new();

    /// <summary>
    /// Tema curentă a aplicației.
    /// </summary>
    private AppTheme _currentTheme = AppTheme.Light;

    /// <summary>
    /// Constructor principal. Primește dependențele și inițializează componentele.
    /// </summary>
    /// <param name="service">Serviciul de business pentru sarcini.</param>
    /// <param name="notifications">Serviciul de notificări.</param>
    /// <exception cref="ArgumentNullException">Dacă vreun argument este <c>null</c>.</exception>
    public MainForm(TaskService service, NotificationService notifications)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _notifications = notifications ?? throw new ArgumentNullException(nameof(notifications));

        InitializeComponent();
        InitializeGridColumns();
        PopulateFilterCombos();

        _notifications.NotificationsRaised += OnNotificationsRaised;
    }

    /// <summary>
    /// Configurează coloanele <see cref="DataGridView"/> pentru afișarea sarcinilor.
    /// </summary>
    /// <remarks>
    /// Coloanele se setează manual (cu <c>AutoGenerateColumns = false</c>) pentru:
    /// <list type="bullet">
    ///   <item>Control complet asupra ordinii și lățimii.</item>
    ///   <item>Ascunderea câmpurilor interne (<c>Id</c>).</item>
    ///   <item>Formatare custom (în <c>CellFormatting</c>).</item>
    /// </list>
    /// </remarks>
    private void InitializeGridColumns()
    {
        tasksGrid.Columns.Clear();

        tasksGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(TaskItem.Title),
            HeaderText = "Titlu",
            Name = "colTitle",
            FillWeight = 30
        });
        tasksGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(TaskItem.Status),
            HeaderText = "Stare",
            Name = "colStatus",
            FillWeight = 10
        });
        tasksGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(TaskItem.Priority),
            HeaderText = "Prioritate",
            Name = "colPriority",
            FillWeight = 10
        });
        tasksGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(TaskItem.Category),
            HeaderText = "Categorie",
            Name = "colCategory",
            FillWeight = 12
        });
        tasksGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(TaskItem.DueDate),
            HeaderText = "Termen",
            Name = "colDueDate",
            FillWeight = 13,
            DefaultCellStyle = new DataGridViewCellStyle { Format = "dd.MM.yyyy HH:mm" }
        });
        tasksGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(TaskItem.CreatedAt),
            HeaderText = "Data creării",
            Name = "colCreatedAt",
            FillWeight = 13,
            DefaultCellStyle = new DataGridViewCellStyle { Format = "dd.MM.yyyy HH:mm" }
        });
        tasksGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = nameof(TaskItem.Description),
            HeaderText = "Descriere",
            Name = "colDescription",
            FillWeight = 12
        });
    }

    /// <summary>
    /// Populează combobox-urile de filtrare și sortare cu valorile posibile.
    /// </summary>
    /// <remarks>
    /// Pentru filtre, prima opțiune este „(toate)” care corespunde valorii <c>null</c>
    /// — adică „nu filtra după acest câmp”.
    /// </remarks>
    private void PopulateFilterCombos()
    {
        // Filtru stare.
        statusFilterCombo.Items.Add(new ComboItem<TaskStatus?>(null, "(toate stările)"));
        statusFilterCombo.Items.Add(new ComboItem<TaskStatus?>(TaskStatus.New, "Nouă"));
        statusFilterCombo.Items.Add(new ComboItem<TaskStatus?>(TaskStatus.InProgress, "În lucru"));
        statusFilterCombo.Items.Add(new ComboItem<TaskStatus?>(TaskStatus.Completed, "Finalizată"));
        statusFilterCombo.SelectedIndex = 0;

        // Filtru categorie.
        categoryFilterCombo.Items.Add(new ComboItem<Category?>(null, "(toate categoriile)"));
        categoryFilterCombo.Items.Add(new ComboItem<Category?>(Category.Work, "Serviciu"));
        categoryFilterCombo.Items.Add(new ComboItem<Category?>(Category.Study, "Studii"));
        categoryFilterCombo.Items.Add(new ComboItem<Category?>(Category.Home, "Casă"));
        categoryFilterCombo.Items.Add(new ComboItem<Category?>(Category.Personal, "Personal"));
        categoryFilterCombo.Items.Add(new ComboItem<Category?>(Category.Other, "Altele"));
        categoryFilterCombo.SelectedIndex = 0;

        // Filtru prioritate.
        priorityFilterCombo.Items.Add(new ComboItem<Priority?>(null, "(toate prioritățile)"));
        priorityFilterCombo.Items.Add(new ComboItem<Priority?>(Priority.Low, "Scăzută"));
        priorityFilterCombo.Items.Add(new ComboItem<Priority?>(Priority.Medium, "Medie"));
        priorityFilterCombo.Items.Add(new ComboItem<Priority?>(Priority.High, "Înaltă"));
        priorityFilterCombo.SelectedIndex = 0;

        // Sortare câmp.
        sortCombo.Items.Add(new ComboItem<SortField?>(null, "(fără sortare)"));
        sortCombo.Items.Add(new ComboItem<SortField?>(SortField.CreatedAt, "Data creării"));
        sortCombo.Items.Add(new ComboItem<SortField?>(SortField.DueDate, "Termen"));
        sortCombo.Items.Add(new ComboItem<SortField?>(SortField.Priority, "Prioritate"));
        sortCombo.Items.Add(new ComboItem<SortField?>(SortField.Title, "Titlu"));
        sortCombo.SelectedIndex = 0;

        // Direcție sortare.
        sortDirectionCombo.Items.Add(new ComboItem<SortDirection>(SortDirection.Ascending, "Crescător"));
        sortDirectionCombo.Items.Add(new ComboItem<SortDirection>(SortDirection.Descending, "Descrescător"));
        sortDirectionCombo.SelectedIndex = 0;
    }

    /// <summary>
    /// Construiește un <see cref="TaskFilter"/> pe baza valorilor curente din UI.
    /// </summary>
    /// <returns>Filtrul de aplicat.</returns>
    private TaskFilter BuildFilterFromUI()
    {
        return new TaskFilter
        {
            Status = (statusFilterCombo.SelectedItem as ComboItem<TaskStatus?>)?.Value,
            Category = (categoryFilterCombo.SelectedItem as ComboItem<Category?>)?.Value,
            Priority = (priorityFilterCombo.SelectedItem as ComboItem<Priority?>)?.Value,
            SearchText = searchTextBox.Text,
            SortBy = (sortCombo.SelectedItem as ComboItem<SortField?>)?.Value,
            SortDirection = (sortDirectionCombo.SelectedItem as ComboItem<SortDirection>)?.Value
                ?? SortDirection.Ascending
        };
    }

    /// <summary>
    /// Reîncarcă lista din serviciu, aplicând filtrul curent, și actualizează grila.
    /// </summary>
    private void RefreshGrid()
    {
        var filter = BuildFilterFromUI();
        _currentView = _service.GetFiltered(filter);

        // Forțăm reattach al sursei — DataGridView nu ascultă mutările colecției.
        tasksGrid.DataSource = null;
        tasksGrid.DataSource = _currentView;

        UpdateStatusBar();
    }

    /// <summary>
    /// Actualizează textul din bara de stare cu numărul de sarcini afișate / totale.
    /// </summary>
    private void UpdateStatusBar()
    {
        int total = _service.Count;
        int shown = _currentView.Count;
        int overdue = _service.GetOverdue().Count;

        statusLabel.Text = $"Afișate: {shown} / {total}   |   Restante: {overdue}";
    }

    // ============== EVENT HANDLERS ==============

    private void MainForm_Load(object? sender, EventArgs e)
    {
        ApplyTheme(_currentTheme);
        RefreshGrid();
        notificationTimer.Start();
        // Verificare imediată la pornire.
        _notifications.CheckNow();
    }

    /// <summary>
    /// La închiderea formularului, salvăm modificările pe disc.
    /// </summary>
    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        try
        {
            _service.SaveToRepository();
        }
        catch (Exception ex)
        {
            var result = MessageBox.Show(
                "Salvarea a eșuat:\n\n" + ex.Message + "\n\nÎnchideți oricum?",
                "Eroare salvare",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (result == DialogResult.No)
                e.Cancel = true;
        }
    }

    /// <summary>
    /// Eveniment generic pentru orice modificare a filtrelor (combobox, textbox).
    /// </summary>
    private void FilterChanged(object? sender, EventArgs e) => RefreshGrid();

    /// <summary>
    /// Resetează toate filtrele la valorile lor implicite.
    /// </summary>
    private void ResetFiltersButton_Click(object? sender, EventArgs e)
    {
        searchTextBox.Text = string.Empty;
        statusFilterCombo.SelectedIndex = 0;
        categoryFilterCombo.SelectedIndex = 0;
        priorityFilterCombo.SelectedIndex = 0;
        sortCombo.SelectedIndex = 0;
        sortDirectionCombo.SelectedIndex = 0;
        RefreshGrid();
    }

    /// <summary>
    /// Adăugare sarcină nouă — deschide <see cref="TaskEditForm"/> în modul „Adăugare”.
    /// </summary>
    private void AddButton_Click(object? sender, EventArgs e)
    {
        using var dlg = new TaskEditForm(null);
        ApplyThemeToDialog(dlg);
        if (dlg.ShowDialog(this) == DialogResult.OK && dlg.Result != null)
        {
            try
            {
                _service.Add(dlg.Result);
                _service.SaveToRepository();
                RefreshGrid();
            }
            catch (Exception ex)
            {
                ShowError("Adăugarea a eșuat", ex);
            }
        }
    }

    /// <summary>
    /// Modificare sarcină — deschide <see cref="TaskEditForm"/> în modul „Editare”.
    /// </summary>
    private void EditButton_Click(object? sender, EventArgs e)
    {
        var selected = GetSelectedTask();
        if (selected == null)
        {
            MessageBox.Show("Selectați mai întâi o sarcină din listă.",
                "Selecție lipsă", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dlg = new TaskEditForm(selected);
        ApplyThemeToDialog(dlg);
        if (dlg.ShowDialog(this) == DialogResult.OK && dlg.Result != null)
        {
            try
            {
                _service.Update(dlg.Result);
                _service.SaveToRepository();
                RefreshGrid();
            }
            catch (Exception ex)
            {
                ShowError("Modificarea a eșuat", ex);
            }
        }
    }

    /// <summary>
    /// Ștergere sarcină — cere confirmarea utilizatorului.
    /// </summary>
    private void DeleteButton_Click(object? sender, EventArgs e)
    {
        var selected = GetSelectedTask();
        if (selected == null)
        {
            MessageBox.Show("Selectați mai întâi o sarcină din listă.",
                "Selecție lipsă", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var result = MessageBox.Show(
            $"Sigur doriți să ștergeți sarcina „{selected.Title}”?",
            "Confirmare ștergere",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result != DialogResult.Yes) return;

        try
        {
            _service.Delete(selected.Id);
            _service.SaveToRepository();
            RefreshGrid();
        }
        catch (Exception ex)
        {
            ShowError("Ștergerea a eșuat", ex);
        }
    }

    /// <summary>
    /// Dublu-click pe rând = editare rapidă.
    /// </summary>
    private void TasksGrid_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        EditButton_Click(sender, e);
    }

    /// <summary>
    /// Formatare custom: traduce în română valorile enum afișate în grilă
    /// și colorează rândurile sarcinilor restante.
    /// </summary>
    private void TasksGrid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || e.RowIndex >= _currentView.Count) return;

        var task = _currentView[e.RowIndex];
        var col = tasksGrid.Columns[e.ColumnIndex];

        switch (col.Name)
        {
            case "colStatus":
                e.Value = TranslateStatus(task.Status);
                e.FormattingApplied = true;
                break;
            case "colPriority":
                e.Value = TranslatePriority(task.Priority);
                e.FormattingApplied = true;
                break;
            case "colCategory":
                e.Value = TranslateCategory(task.Category);
                e.FormattingApplied = true;
                break;
            case "colDueDate":
                if (!task.DueDate.HasValue)
                {
                    e.Value = "—";
                    e.FormattingApplied = true;
                }
                break;
        }

        // Colorăm întreg rândul dacă sarcina este restantă.
        if (task.IsOverdue())
        {
            tasksGrid.Rows[e.RowIndex].DefaultCellStyle.BackColor =
                System.Drawing.Color.FromArgb(255, 220, 220);
            tasksGrid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
        }
    }

    private void ExitMenuItem_Click(object? sender, EventArgs e) => Close();

    /// <summary>
    /// Export CSV — deschide dialogul de salvare și scrie fișierul.
    /// </summary>
    private void ExportCsvMenuItem_Click(object? sender, EventArgs e)
    {
        using var sfd = new SaveFileDialog
        {
            Filter = "Fișier CSV (*.csv)|*.csv|Toate fișierele (*.*)|*.*",
            FileName = "sarcini_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".csv",
            Title = "Export sarcini în CSV"
        };

        if (sfd.ShowDialog(this) != DialogResult.OK) return;

        try
        {
            // Exportăm exact lista filtrată (ce vede utilizatorul).
            CsvExporter.Export(_currentView, sfd.FileName);
            MessageBox.Show(
                $"Exportate {_currentView.Count} sarcini în:\n{sfd.FileName}",
                "Export reușit",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            ShowError("Exportul a eșuat", ex);
        }
    }

    private void LightThemeMenuItem_Click(object? sender, EventArgs e) => ApplyTheme(AppTheme.Light);

    private void DarkThemeMenuItem_Click(object? sender, EventArgs e) => ApplyTheme(AppTheme.Dark);

    /// <summary>
    /// Aplică tema asupra formularului curent și a meniului.
    /// </summary>
    /// <param name="theme">Tema de aplicat.</param>
    private void ApplyTheme(AppTheme theme)
    {
        _currentTheme = theme;
        ThemeManager.Apply(this, theme);

        // Bifa pe itemul de meniu activ.
        lightThemeMenuItem.Checked = theme == AppTheme.Light;
        darkThemeMenuItem.Checked = theme == AppTheme.Dark;
    }

    /// <summary>
    /// Aplică tema asupra unui dialog modal (de ex. <see cref="TaskEditForm"/>).
    /// </summary>
    /// <param name="dialog">Formularul-țintă.</param>
    private void ApplyThemeToDialog(Form dialog)
    {
        ThemeManager.Apply(dialog, _currentTheme);
    }

    /// <summary>
    /// Tick-ul timer-ului de notificări — declanșează verificarea periodică.
    /// </summary>
    private void NotificationTimer_Tick(object? sender, EventArgs e)
    {
        _notifications.CheckNow();
    }

    /// <summary>
    /// Handler pentru notificările ridicate de <see cref="NotificationService"/>.
    /// Afișează un mesaj utilizatorului.
    /// </summary>
    private void OnNotificationsRaised(object? sender, NotificationsEventArgs e)
    {
        if (!e.HasAny) return;

        var message = new System.Text.StringBuilder();

        if (e.Overdue.Count > 0)
        {
            message.AppendLine("Sarcini RESTANTE:");
            foreach (var t in e.Overdue.Take(5))
                message.AppendLine($"  • {t.Title} (termen {t.DueDate:dd.MM.yyyy})");
            if (e.Overdue.Count > 5)
                message.AppendLine($"  ... și încă {e.Overdue.Count - 5}");
            message.AppendLine();
        }

        if (e.DueSoon.Count > 0)
        {
            message.AppendLine($"Sarcini ce expiră în următoarele {_notifications.LookaheadHours} ore:");
            foreach (var t in e.DueSoon.Take(5))
                message.AppendLine($"  • {t.Title} (termen {t.DueDate:dd.MM.yyyy HH:mm})");
            if (e.DueSoon.Count > 5)
                message.AppendLine($"  ... și încă {e.DueSoon.Count - 5}");
        }

        MessageBox.Show(message.ToString(), "Notificare sarcini",
            MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    // ============== HELPERS ==============

    /// <summary>
    /// Returnează sarcina selectată în grilă sau <c>null</c>.
    /// </summary>
    private TaskItem? GetSelectedTask()
    {
        if (tasksGrid.CurrentRow == null) return null;
        int idx = tasksGrid.CurrentRow.Index;
        if (idx < 0 || idx >= _currentView.Count) return null;
        return _currentView[idx];
    }

    private void ShowError(string title, Exception ex)
    {
        MessageBox.Show(ex.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    /// <summary>
    /// Traduce <see cref="TaskStatus"/> în română pentru afișare.
    /// </summary>
    public static string TranslateStatus(TaskStatus s) => s switch
    {
        TaskStatus.New => "Nouă",
        TaskStatus.InProgress => "În lucru",
        TaskStatus.Completed => "Finalizată",
        _ => s.ToString()
    };

    /// <summary>
    /// Traduce <see cref="Priority"/> în română.
    /// </summary>
    public static string TranslatePriority(Priority p) => p switch
    {
        Priority.Low => "Scăzută",
        Priority.Medium => "Medie",
        Priority.High => "Înaltă",
        _ => p.ToString()
    };

    /// <summary>
    /// Traduce <see cref="Category"/> în română.
    /// </summary>
    public static string TranslateCategory(Category c) => c switch
    {
        Category.Work => "Serviciu",
        Category.Study => "Studii",
        Category.Home => "Casă",
        Category.Personal => "Personal",
        Category.Other => "Altele",
        _ => c.ToString()
    };
}

/// <summary>
/// Element generic pentru combobox: păstrează o valoare tipată și un text de afișare.
/// </summary>
/// <typeparam name="T">Tipul valorii.</typeparam>
/// <remarks>
/// Folosit pentru a evita conversii string ↔ enum repetate la citirea
/// valorii selectate. <see cref="ToString"/> returnează textul afișat.
/// </remarks>
internal class ComboItem<T>
{
    public T Value { get; }
    public string Display { get; }

    public ComboItem(T value, string display)
    {
        Value = value;
        Display = display;
    }

    public override string ToString() => Display;
}
