namespace TaskManager.UI;

/// <summary>
/// Cod generat pentru construcția vizuală a <see cref="TaskEditForm"/>.
/// Folosește layout adaptiv (<see cref="System.Windows.Forms.TableLayoutPanel"/>)
/// pentru a se adapta automat la DPI și la traducerile de etichete.
/// </summary>
partial class TaskEditForm
{
    private System.ComponentModel.IContainer components = null!;

    private System.Windows.Forms.TableLayoutPanel mainTable = null!;

    private System.Windows.Forms.Label titleLabel = null!;
    private System.Windows.Forms.TextBox titleTextBox = null!;

    private System.Windows.Forms.Label descriptionLabel = null!;
    private System.Windows.Forms.TextBox descriptionTextBox = null!;

    private System.Windows.Forms.Label dueDateLabel = null!;
    private System.Windows.Forms.FlowLayoutPanel dueDatePanel = null!;
    private System.Windows.Forms.CheckBox dueDateEnabledCheck = null!;
    private System.Windows.Forms.DateTimePicker dueDatePicker = null!;

    private System.Windows.Forms.Label priorityLabel = null!;
    private System.Windows.Forms.ComboBox priorityCombo = null!;

    private System.Windows.Forms.Label categoryLabel = null!;
    private System.Windows.Forms.ComboBox categoryCombo = null!;

    private System.Windows.Forms.Label statusLabel = null!;
    private System.Windows.Forms.ComboBox statusCombo = null!;

    private System.Windows.Forms.FlowLayoutPanel buttonsPanel = null!;
    private System.Windows.Forms.Button okButton = null!;
    private System.Windows.Forms.Button cancelButton = null!;

    /// <summary>
    /// Eliberează resursele neadministrate.
    /// </summary>
    /// <param name="disposing"><c>true</c> pentru eliberarea resurselor administrate.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    /// <summary>
    /// Construiește controalele formularului folosind un TableLayoutPanel
    /// cu 2 coloane (Label + Control) și 7 rânduri (un câmp pe rând + butoane).
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();

        mainTable = new System.Windows.Forms.TableLayoutPanel();

        titleLabel = new System.Windows.Forms.Label();
        titleTextBox = new System.Windows.Forms.TextBox();

        descriptionLabel = new System.Windows.Forms.Label();
        descriptionTextBox = new System.Windows.Forms.TextBox();

        dueDateLabel = new System.Windows.Forms.Label();
        dueDatePanel = new System.Windows.Forms.FlowLayoutPanel();
        dueDateEnabledCheck = new System.Windows.Forms.CheckBox();
        dueDatePicker = new System.Windows.Forms.DateTimePicker();

        priorityLabel = new System.Windows.Forms.Label();
        priorityCombo = new System.Windows.Forms.ComboBox();

        categoryLabel = new System.Windows.Forms.Label();
        categoryCombo = new System.Windows.Forms.ComboBox();

        statusLabel = new System.Windows.Forms.Label();
        statusCombo = new System.Windows.Forms.ComboBox();

        buttonsPanel = new System.Windows.Forms.FlowLayoutPanel();
        okButton = new System.Windows.Forms.Button();
        cancelButton = new System.Windows.Forms.Button();

        // ============== TableLayoutPanel principal ==============
        mainTable.Dock = System.Windows.Forms.DockStyle.Fill;
        mainTable.ColumnCount = 2;
        mainTable.RowCount = 7;
        mainTable.Padding = new System.Windows.Forms.Padding(15);
        mainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        mainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));

        // Rând 0: Titlu (înălțime AutoSize)
        mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        // Rând 1: Descriere (înălțime fixă pentru multiline)
        mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 110F));
        // Rând 2-5: Termen, Prioritate, Categorie, Stare (AutoSize)
        mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        // Rând 6: butoane (Percent 100% pentru a împinge totul în jos)
        mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));

        // ============== Câmpurile ==============

        // Titlu
        ConfigureFieldLabel(titleLabel, "Titlu *:");
        titleTextBox.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        titleTextBox.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
        titleTextBox.MinimumSize = new System.Drawing.Size(280, 0);

        // Descriere
        ConfigureFieldLabel(descriptionLabel, "Descriere:");
        descriptionTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
            | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        descriptionTextBox.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
        descriptionTextBox.Multiline = true;
        descriptionTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        descriptionTextBox.AcceptsReturn = true;

        // Termen — checkbox + DateTimePicker într-un FlowLayoutPanel
        ConfigureFieldLabel(dueDateLabel, "Termen:");
        dueDatePanel.AutoSize = true;
        dueDatePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        dueDatePanel.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
        dueDatePanel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
        dueDatePanel.Anchor = System.Windows.Forms.AnchorStyles.Left;
        dueDatePanel.Controls.Add(dueDateEnabledCheck);
        dueDatePanel.Controls.Add(dueDatePicker);

        dueDateEnabledCheck.Text = "Are termen";
        dueDateEnabledCheck.AutoSize = true;
        dueDateEnabledCheck.Anchor = System.Windows.Forms.AnchorStyles.Left;
        dueDateEnabledCheck.Margin = new System.Windows.Forms.Padding(3, 6, 12, 3);
        dueDateEnabledCheck.CheckedChanged += DueDateEnabledCheck_CheckedChanged;

        dueDatePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
        dueDatePicker.CustomFormat = "dd.MM.yyyy HH:mm";
        dueDatePicker.MinimumSize = new System.Drawing.Size(220, 0);
        dueDatePicker.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);

        // Prioritate
        ConfigureFieldLabel(priorityLabel, "Prioritate:");
        ConfigureFieldCombo(priorityCombo);

        // Categorie
        ConfigureFieldLabel(categoryLabel, "Categorie:");
        ConfigureFieldCombo(categoryCombo);

        // Stare
        ConfigureFieldLabel(statusLabel, "Stare:");
        ConfigureFieldCombo(statusCombo);

        // ============== Butoane (FlowLayoutPanel, aliniate la dreapta) ==============
        buttonsPanel.AutoSize = true;
        buttonsPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        buttonsPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
        buttonsPanel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
        buttonsPanel.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
        buttonsPanel.Controls.Add(cancelButton);
        buttonsPanel.Controls.Add(okButton);

        ConfigureDialogButton(okButton, "OK");
        okButton.DialogResult = System.Windows.Forms.DialogResult.None;
        okButton.Click += OkButton_Click;

        ConfigureDialogButton(cancelButton, "Anulare");
        cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;

        // ============== Adăugare în tabel ==============
        mainTable.Controls.Add(titleLabel,         0, 0);
        mainTable.Controls.Add(titleTextBox,       1, 0);
        mainTable.Controls.Add(descriptionLabel,   0, 1);
        mainTable.Controls.Add(descriptionTextBox, 1, 1);
        mainTable.Controls.Add(dueDateLabel,       0, 2);
        mainTable.Controls.Add(dueDatePanel,       1, 2);
        mainTable.Controls.Add(priorityLabel,      0, 3);
        mainTable.Controls.Add(priorityCombo,      1, 3);
        mainTable.Controls.Add(categoryLabel,      0, 4);
        mainTable.Controls.Add(categoryCombo,      1, 4);
        mainTable.Controls.Add(statusLabel,        0, 5);
        mainTable.Controls.Add(statusCombo,        1, 5);
        mainTable.Controls.Add(buttonsPanel,       1, 6);

        // ============== Form ==============
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(620, 430);
        MinimumSize = new System.Drawing.Size(540, 460);
        Controls.Add(mainTable);
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        AcceptButton = okButton;
        CancelButton = cancelButton;
    }

    /// <summary>
    /// Helper: configurează un Label de câmp (etichetă pentru un control de date).
    /// </summary>
    private static void ConfigureFieldLabel(System.Windows.Forms.Label label, string text)
    {
        label.Text = text;
        label.AutoSize = true;
        label.Anchor = System.Windows.Forms.AnchorStyles.Left;
        label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        label.Margin = new System.Windows.Forms.Padding(3, 10, 12, 6);
    }

    /// <summary>
    /// Helper: configurează un ComboBox de câmp.
    /// </summary>
    private static void ConfigureFieldCombo(System.Windows.Forms.ComboBox combo)
    {
        combo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        combo.Anchor = System.Windows.Forms.AnchorStyles.Left;
        combo.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
        combo.MinimumSize = new System.Drawing.Size(220, 0);
    }

    /// <summary>
    /// Helper: configurează un buton de dialog (OK / Anulare).
    /// </summary>
    private static void ConfigureDialogButton(System.Windows.Forms.Button button, string text)
    {
        button.Text = text;
        button.AutoSize = true;
        button.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        button.MinimumSize = new System.Drawing.Size(110, 34);
        button.Margin = new System.Windows.Forms.Padding(6, 3, 0, 3);
        button.Padding = new System.Windows.Forms.Padding(10, 4, 10, 4);
    }
}
