namespace TaskManager.UI;

/// <summary>
/// Cod generat pentru construcția vizuală a <see cref="MainForm"/>.
/// </summary>
/// <remarks>
/// <para>
/// <b>Strategia de layout:</b> folosim <see cref="System.Windows.Forms.TableLayoutPanel"/>
/// pentru zona de filtre și <see cref="System.Windows.Forms.FlowLayoutPanel"/> pentru
/// bara de butoane. Acest layout se adaptează automat la dimensiunea ferestrei
/// și la setările DPI ale sistemului (100%, 125%, 150%, 200%) — controalele nu
/// se mai suprapun și textul nu este trunchiat.
/// </para>
/// <para>
/// Acest fișier ar fi în mod normal generat de designer-ul Visual Studio, dar îl
/// scriem manual pentru claritate didactică și pentru a permite folosirea
/// proiectului fără Visual Studio.
/// </para>
/// </remarks>
partial class MainForm
{
    /// <summary>
    /// Componenta de gestiune a resurselor non-UI (timere etc.).
    /// </summary>
    private System.ComponentModel.IContainer components = null!;

    // === Meniu ===
    private System.Windows.Forms.MenuStrip menuStrip = null!;
    private System.Windows.Forms.ToolStripMenuItem fileMenu = null!;
    private System.Windows.Forms.ToolStripMenuItem exportCsvMenuItem = null!;
    private System.Windows.Forms.ToolStripMenuItem exitMenuItem = null!;
    private System.Windows.Forms.ToolStripMenuItem viewMenu = null!;
    private System.Windows.Forms.ToolStripMenuItem lightThemeMenuItem = null!;
    private System.Windows.Forms.ToolStripMenuItem darkThemeMenuItem = null!;

    // === Zona de filtre (sus) ===
    private System.Windows.Forms.TableLayoutPanel filterTable = null!;
    private System.Windows.Forms.Label searchLabel = null!;
    private System.Windows.Forms.TextBox searchTextBox = null!;
    private System.Windows.Forms.Label statusFilterLabel = null!;
    private System.Windows.Forms.ComboBox statusFilterCombo = null!;
    private System.Windows.Forms.Label categoryFilterLabel = null!;
    private System.Windows.Forms.ComboBox categoryFilterCombo = null!;
    private System.Windows.Forms.Label priorityFilterLabel = null!;
    private System.Windows.Forms.ComboBox priorityFilterCombo = null!;
    private System.Windows.Forms.Label sortLabel = null!;
    private System.Windows.Forms.ComboBox sortCombo = null!;
    private System.Windows.Forms.ComboBox sortDirectionCombo = null!;
    private System.Windows.Forms.Button resetFiltersButton = null!;

    // === Grila de sarcini ===
    private System.Windows.Forms.DataGridView tasksGrid = null!;

    // === Bara de butoane (jos) ===
    private System.Windows.Forms.FlowLayoutPanel buttonsPanel = null!;
    private System.Windows.Forms.Button addButton = null!;
    private System.Windows.Forms.Button editButton = null!;
    private System.Windows.Forms.Button deleteButton = null!;

    // === Bara de stare ===
    private System.Windows.Forms.StatusStrip statusStrip = null!;
    private System.Windows.Forms.ToolStripStatusLabel statusLabel = null!;

    // === Timer pentru notificări ===
    private System.Windows.Forms.Timer notificationTimer = null!;

    /// <summary>
    /// Eliberează resursele neadministrate.
    /// </summary>
    /// <param name="disposing"><c>true</c> pentru a elibera atât resurse administrate cât și ne-administrate.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    /// <summary>
    /// Construiește controalele formularului folosind layout adaptiv.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();

        menuStrip = new System.Windows.Forms.MenuStrip();
        fileMenu = new System.Windows.Forms.ToolStripMenuItem();
        exportCsvMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        viewMenu = new System.Windows.Forms.ToolStripMenuItem();
        lightThemeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        darkThemeMenuItem = new System.Windows.Forms.ToolStripMenuItem();

        filterTable = new System.Windows.Forms.TableLayoutPanel();
        searchLabel = new System.Windows.Forms.Label();
        searchTextBox = new System.Windows.Forms.TextBox();
        statusFilterLabel = new System.Windows.Forms.Label();
        statusFilterCombo = new System.Windows.Forms.ComboBox();
        categoryFilterLabel = new System.Windows.Forms.Label();
        categoryFilterCombo = new System.Windows.Forms.ComboBox();
        priorityFilterLabel = new System.Windows.Forms.Label();
        priorityFilterCombo = new System.Windows.Forms.ComboBox();
        sortLabel = new System.Windows.Forms.Label();
        sortCombo = new System.Windows.Forms.ComboBox();
        sortDirectionCombo = new System.Windows.Forms.ComboBox();
        resetFiltersButton = new System.Windows.Forms.Button();

        tasksGrid = new System.Windows.Forms.DataGridView();

        buttonsPanel = new System.Windows.Forms.FlowLayoutPanel();
        addButton = new System.Windows.Forms.Button();
        editButton = new System.Windows.Forms.Button();
        deleteButton = new System.Windows.Forms.Button();

        statusStrip = new System.Windows.Forms.StatusStrip();
        statusLabel = new System.Windows.Forms.ToolStripStatusLabel();

        notificationTimer = new System.Windows.Forms.Timer(components);

        // ============== MENIU ==============
        menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { fileMenu, viewMenu });
        menuStrip.Location = new System.Drawing.Point(0, 0);
        menuStrip.Name = "menuStrip";
        menuStrip.Padding = new System.Windows.Forms.Padding(6, 2, 0, 2);

        fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
        {
            exportCsvMenuItem,
            new System.Windows.Forms.ToolStripSeparator(),
            exitMenuItem
        });
        fileMenu.Text = "&Fișier";
        exportCsvMenuItem.Text = "Export &CSV...";
        exportCsvMenuItem.Click += ExportCsvMenuItem_Click;
        exitMenuItem.Text = "&Ieșire";
        exitMenuItem.Click += ExitMenuItem_Click;

        viewMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
        {
            lightThemeMenuItem,
            darkThemeMenuItem
        });
        viewMenu.Text = "&Aspect";
        lightThemeMenuItem.Text = "Temă &deschisă";
        lightThemeMenuItem.Click += LightThemeMenuItem_Click;
        darkThemeMenuItem.Text = "Temă &întunecată";
        darkThemeMenuItem.Click += DarkThemeMenuItem_Click;

        // ============== ZONA DE FILTRE (TableLayoutPanel) ==============
        // Layout-ul folosește TableLayoutPanel cu 8 coloane × 2 rânduri.
        // Coloanele pare (Label-uri) sunt AutoSize — se ajustează la lungimea
        // textului tradus (inclusiv diacritice). Coloanele impare (controale)
        // sunt Percent — împart spațiul rămas în mod egal.
        filterTable.Dock = System.Windows.Forms.DockStyle.Top;
        filterTable.AutoSize = true;
        filterTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        filterTable.ColumnCount = 8;
        filterTable.RowCount = 2;
        filterTable.Padding = new System.Windows.Forms.Padding(10, 8, 10, 8);
        filterTable.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.None;

        // 4 perechi de coloane: Label (AutoSize) + Control (33% din restul)
        filterTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        filterTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
        filterTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        filterTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
        filterTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        filterTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
        filterTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        filterTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));

        filterTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        filterTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));

        // ----- Rând 0: Căutare + Stare + Categorie + Prioritate -----
        ConfigureLabel(searchLabel, "Căutare:");
        searchTextBox.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        searchTextBox.PlaceholderText = "titlu sau descriere...";
        searchTextBox.Margin = new System.Windows.Forms.Padding(3, 6, 12, 6);
        searchTextBox.MinimumSize = new System.Drawing.Size(180, 0);
        searchTextBox.TextChanged += FilterChanged;

        ConfigureLabel(statusFilterLabel, "Stare:");
        ConfigureCombo(statusFilterCombo);
        statusFilterCombo.SelectedIndexChanged += FilterChanged;

        ConfigureLabel(categoryFilterLabel, "Categorie:");
        ConfigureCombo(categoryFilterCombo);
        categoryFilterCombo.SelectedIndexChanged += FilterChanged;

        ConfigureLabel(priorityFilterLabel, "Prioritate:");
        ConfigureCombo(priorityFilterCombo);
        priorityFilterCombo.SelectedIndexChanged += FilterChanged;

        // ----- Rând 1: Sortare + Direcție + Resetare -----
        ConfigureLabel(sortLabel, "Sortare:");
        ConfigureCombo(sortCombo);
        sortCombo.SelectedIndexChanged += FilterChanged;

        // Etichetă „Direcție:” pentru rândul al doilea (folosim statusFilterLabel.Clone? Nu — creăm o etichetă inline).
        var directionLabel = new System.Windows.Forms.Label();
        ConfigureLabel(directionLabel, "Direcție:");

        ConfigureCombo(sortDirectionCombo);
        sortDirectionCombo.SelectedIndexChanged += FilterChanged;

        // Buton Resetare — întins peste celulele rămase pe rândul 2 (Categorie + Prioritate).
        resetFiltersButton.Text = "Resetare filtre";
        resetFiltersButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
        resetFiltersButton.AutoSize = true;
        resetFiltersButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        resetFiltersButton.MinimumSize = new System.Drawing.Size(150, 28);
        resetFiltersButton.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
        resetFiltersButton.Click += ResetFiltersButton_Click;

        // Adăugare în tabel pe poziții explicite (col, row).
        filterTable.Controls.Add(searchLabel,           0, 0);
        filterTable.Controls.Add(searchTextBox,         1, 0);
        filterTable.Controls.Add(statusFilterLabel,     2, 0);
        filterTable.Controls.Add(statusFilterCombo,     3, 0);
        filterTable.Controls.Add(categoryFilterLabel,   4, 0);
        filterTable.Controls.Add(categoryFilterCombo,   5, 0);
        filterTable.Controls.Add(priorityFilterLabel,   6, 0);
        filterTable.Controls.Add(priorityFilterCombo,   7, 0);

        filterTable.Controls.Add(sortLabel,             0, 1);
        filterTable.Controls.Add(sortCombo,             1, 1);
        filterTable.Controls.Add(directionLabel,        2, 1);
        filterTable.Controls.Add(sortDirectionCombo,    3, 1);
        filterTable.Controls.Add(resetFiltersButton,    4, 1);
        filterTable.SetColumnSpan(resetFiltersButton, 4);

        // ============== GRILA DE SARCINI ==============
        tasksGrid.Dock = System.Windows.Forms.DockStyle.Fill;
        tasksGrid.AllowUserToAddRows = false;
        tasksGrid.AllowUserToDeleteRows = false;
        tasksGrid.ReadOnly = true;
        tasksGrid.AutoGenerateColumns = false;
        tasksGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
        tasksGrid.MultiSelect = false;
        tasksGrid.RowHeadersVisible = false;
        tasksGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
        tasksGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        tasksGrid.RowTemplate.Height = 28;
        tasksGrid.CellDoubleClick += TasksGrid_CellDoubleClick;
        tasksGrid.CellFormatting += TasksGrid_CellFormatting;

        // ============== BARA DE BUTOANE (FlowLayoutPanel) ==============
        // FlowLayoutPanel aliniază butoanele pe orizontală, fiecare cu AutoSize=true,
        // astfel textul nu este niciodată trunchiat indiferent de DPI.
        buttonsPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
        buttonsPanel.AutoSize = true;
        buttonsPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        buttonsPanel.Padding = new System.Windows.Forms.Padding(10);
        buttonsPanel.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
        buttonsPanel.Controls.Add(addButton);
        buttonsPanel.Controls.Add(editButton);
        buttonsPanel.Controls.Add(deleteButton);

        ConfigureActionButton(addButton, "+ Adăugare");
        addButton.Click += AddButton_Click;

        ConfigureActionButton(editButton, "Modificare");
        editButton.Click += EditButton_Click;

        ConfigureActionButton(deleteButton, "Ștergere");
        deleteButton.Click += DeleteButton_Click;

        // ============== BARA DE STARE ==============
        statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { statusLabel });
        statusStrip.Dock = System.Windows.Forms.DockStyle.Bottom;
        statusLabel.Text = "Gata";
        statusLabel.Spring = true;
        statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

        // ============== TIMER ==============
        // Verifică sarcinile cu termen ce se apropie odată la 5 minute (300 000 ms).
        notificationTimer.Interval = 300000;
        notificationTimer.Tick += NotificationTimer_Tick;

        // ============== FORM ==============
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(1100, 650);
        MinimumSize = new System.Drawing.Size(900, 500);

        // Ordinea de adăugare contează: Dock=Fill (grid) trebuie ULTIMUL adăugat
        // pentru a ocupa spațiul rămas DUPĂ celelalte controale ancorate.
        Controls.Add(tasksGrid);
        Controls.Add(buttonsPanel);
        Controls.Add(statusStrip);
        Controls.Add(filterTable);
        Controls.Add(menuStrip);
        MainMenuStrip = menuStrip;

        Text = "Manager de sarcini personale";
        StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        FormClosing += MainForm_FormClosing;
        Load += MainForm_Load;
    }

    /// <summary>
    /// Helper: configurează un Label cu text, AutoSize și aliniere verticală
    /// la mijloc (pentru a coincide vizual cu controlul TextBox/ComboBox alăturat).
    /// </summary>
    private static void ConfigureLabel(System.Windows.Forms.Label label, string text)
    {
        label.Text = text;
        label.AutoSize = true;
        label.Anchor = System.Windows.Forms.AnchorStyles.Left;
        label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        label.Margin = new System.Windows.Forms.Padding(8, 9, 4, 6);
    }

    /// <summary>
    /// Helper: configurează un ComboBox în mod uniform — DropDownList, ancorat
    /// la stânga și dreapta (se întinde), lățime minimă pentru a afișa textul integral.
    /// </summary>
    private static void ConfigureCombo(System.Windows.Forms.ComboBox combo)
    {
        combo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        combo.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        combo.Margin = new System.Windows.Forms.Padding(3, 6, 12, 6);
        combo.MinimumSize = new System.Drawing.Size(180, 0);
    }

    /// <summary>
    /// Helper: configurează un buton de acțiune (Adăugare / Modificare / Ștergere)
    /// cu AutoSize=true pentru a se adapta automat la lungimea textului.
    /// </summary>
    private static void ConfigureActionButton(System.Windows.Forms.Button button, string text)
    {
        button.Text = text;
        button.AutoSize = true;
        button.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        button.MinimumSize = new System.Drawing.Size(140, 34);
        button.Margin = new System.Windows.Forms.Padding(3, 3, 8, 3);
        button.Padding = new System.Windows.Forms.Padding(8, 4, 8, 4);
    }
}
