using System;
using System.Windows.Forms;
using TaskManager.Models;
// Alias pentru a rezolva conflictul cu System.Threading.Tasks.TaskStatus
using TaskStatus = TaskManager.Models.TaskStatus;

namespace TaskManager.UI;

/// <summary>
/// Formular dialog pentru adăugarea sau modificarea unei sarcini.
/// </summary>
/// <remarks>
/// <para>
/// <b>Două moduri de lucru:</b>
/// </para>
/// <list type="bullet">
///   <item><b>Adăugare</b> — constructor apelat cu <c>existing = null</c>. Câmpurile se inițializează cu valorile implicite.</item>
///   <item><b>Editare</b> — constructor apelat cu <c>existing != null</c>. Câmpurile se completează cu valorile sarcinii curente.</item>
/// </list>
/// <para>
/// La închiderea cu OK, proprietatea <see cref="Result"/> conține obiectul
/// <see cref="TaskItem"/> populat. La Cancel, <see cref="Result"/> rămâne <c>null</c>.
/// </para>
/// <para>
/// Formularul lucrează pe o copie a sarcinii (<see cref="TaskItem.Clone"/>),
/// astfel încât anularea să nu afecteze originalul.
/// </para>
/// </remarks>
public partial class TaskEditForm : Form
{
    /// <summary>
    /// Sarcina rezultată după confirmare. <c>null</c> dacă utilizatorul a anulat.
    /// </summary>
    public TaskItem? Result { get; private set; }

    /// <summary>
    /// Sarcina originală (în mod „Editare”) sau <c>null</c> (în mod „Adăugare”).
    /// Folosit pentru a păstra <see cref="TaskItem.Id"/> și <see cref="TaskItem.CreatedAt"/>.
    /// </summary>
    private readonly TaskItem? _original;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="existing">
    /// Sarcina existentă pentru editare. Dacă este <c>null</c>, se intră în mod „Adăugare”
    /// (se va crea o sarcină nouă la confirmare).
    /// </param>
    public TaskEditForm(TaskItem? existing)
    {
        InitializeComponent();
        PopulateCombos();

        _original = existing;

        if (existing == null)
        {
            // Mod „Adăugare” — valori implicite.
            Text = "Adăugare sarcină";
            dueDateEnabledCheck.Checked = false;
            dueDatePicker.Value = DateTime.Today.AddDays(1);
            SelectComboItem(priorityCombo, Priority.Medium);
            SelectComboItem(categoryCombo, Category.Other);
            SelectComboItem(statusCombo, TaskStatus.New);
        }
        else
        {
            // Mod „Editare” — populăm câmpurile.
            Text = "Modificare sarcină";
            titleTextBox.Text = existing.Title;
            descriptionTextBox.Text = existing.Description;

            dueDateEnabledCheck.Checked = existing.DueDate.HasValue;
            dueDatePicker.Value = existing.DueDate ?? DateTime.Today.AddDays(1);

            SelectComboItem(priorityCombo, existing.Priority);
            SelectComboItem(categoryCombo, existing.Category);
            SelectComboItem(statusCombo, existing.Status);
        }

        UpdateDueDateEnabled();
    }

    /// <summary>
    /// Populează combobox-urile pentru prioritate, categorie și stare.
    /// </summary>
    /// <remarks>
    /// Folosim <see cref="ComboItem{T}"/> pentru a păstra valoarea enum
    /// alături de textul afișat, fără conversii string ↔ enum.
    /// </remarks>
    private void PopulateCombos()
    {
        priorityCombo.Items.Add(new ComboItem<Priority>(Priority.Low, "Scăzută"));
        priorityCombo.Items.Add(new ComboItem<Priority>(Priority.Medium, "Medie"));
        priorityCombo.Items.Add(new ComboItem<Priority>(Priority.High, "Înaltă"));

        categoryCombo.Items.Add(new ComboItem<Category>(Category.Work, "Serviciu"));
        categoryCombo.Items.Add(new ComboItem<Category>(Category.Study, "Studii"));
        categoryCombo.Items.Add(new ComboItem<Category>(Category.Home, "Casă"));
        categoryCombo.Items.Add(new ComboItem<Category>(Category.Personal, "Personal"));
        categoryCombo.Items.Add(new ComboItem<Category>(Category.Other, "Altele"));

        statusCombo.Items.Add(new ComboItem<TaskStatus>(TaskStatus.New, "Nouă"));
        statusCombo.Items.Add(new ComboItem<TaskStatus>(TaskStatus.InProgress, "În lucru"));
        statusCombo.Items.Add(new ComboItem<TaskStatus>(TaskStatus.Completed, "Finalizată"));
    }

    /// <summary>
    /// Selectează în combobox elementul cu valoarea specificată.
    /// </summary>
    /// <typeparam name="T">Tipul valorii.</typeparam>
    /// <param name="combo">Combobox-ul.</param>
    /// <param name="value">Valoarea de selectat.</param>
    private static void SelectComboItem<T>(ComboBox combo, T value) where T : struct
    {
        for (int i = 0; i < combo.Items.Count; i++)
        {
            if (combo.Items[i] is ComboItem<T> item && item.Value.Equals(value))
            {
                combo.SelectedIndex = i;
                return;
            }
        }
        if (combo.Items.Count > 0)
            combo.SelectedIndex = 0;
    }

    /// <summary>
    /// Activează/dezactivează picker-ul de termen în funcție de checkbox.
    /// </summary>
    private void UpdateDueDateEnabled()
    {
        dueDatePicker.Enabled = dueDateEnabledCheck.Checked;
    }

    private void DueDateEnabledCheck_CheckedChanged(object? sender, EventArgs e)
        => UpdateDueDateEnabled();

    /// <summary>
    /// Confirmă editarea: validează datele și construiește <see cref="Result"/>.
    /// </summary>
    private void OkButton_Click(object? sender, EventArgs e)
    {
        // Validare titlu obligatoriu.
        if (string.IsNullOrWhiteSpace(titleTextBox.Text))
        {
            MessageBox.Show("Titlul este obligatoriu.",
                "Validare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            titleTextBox.Focus();
            return;
        }

        // Citim valorile din combobox-uri.
        var priority = ((ComboItem<Priority>)priorityCombo.SelectedItem!).Value;
        var category = ((ComboItem<Category>)categoryCombo.SelectedItem!).Value;
        var status = ((ComboItem<TaskStatus>)statusCombo.SelectedItem!).Value;

        // Construim rezultatul. Dacă editare — păstrăm Id și CreatedAt.
        Result = new TaskItem
        {
            Id = _original?.Id ?? Guid.NewGuid(),
            Title = titleTextBox.Text.Trim(),
            Description = descriptionTextBox.Text.Trim(),
            CreatedAt = _original?.CreatedAt ?? DateTime.Now,
            DueDate = dueDateEnabledCheck.Checked ? dueDatePicker.Value : null,
            Priority = priority,
            Category = category,
            Status = status
        };

        DialogResult = DialogResult.OK;
        Close();
    }
}
