using System.Drawing;
using System.Windows.Forms;

namespace TaskManager.UI;

/// <summary>
/// Tipurile de teme suportate de aplicație.
/// </summary>
public enum AppTheme
{
    /// <summary>Temă deschisă (luminoasă) — fundal alb, text negru.</summary>
    Light = 0,

    /// <summary>Temă întunecată — fundal gri-închis, text alb.</summary>
    Dark = 1
}

/// <summary>
/// Aplică o temă vizuală (deschisă/întunecată) asupra unui formular și a controalelor sale.
/// </summary>
/// <remarks>
/// <para>
/// WinForms nu are suport nativ pentru teme. Această clasă parcurge recursiv
/// toate controalele unui formular și setează manual culorile <see cref="Control.BackColor"/>
/// și <see cref="Control.ForeColor"/>, cu excepții pentru controale specializate
/// (<see cref="DataGridView"/>, <see cref="Button"/>, <see cref="ComboBox"/>).
/// </para>
/// <para>
/// Pentru o experiență vizuală mai bogată (titlul ferestrei întunecat, scrollbar
/// întunecat) ar fi nevoie de apeluri Win32 (<c>DwmSetWindowAttribute</c>),
/// omise aici pentru simplitate didactică.
/// </para>
/// </remarks>
public static class ThemeManager
{
    /// <summary>
    /// Paleta temei deschise.
    /// </summary>
    private static class LightPalette
    {
        public static readonly Color Background = SystemColors.Control;        // gri deschis
        public static readonly Color Surface = Color.White;                    // fundal grilă, panouri
        public static readonly Color Foreground = Color.Black;
        public static readonly Color GridHeaderBack = Color.FromArgb(230, 230, 230);
        public static readonly Color GridHeaderFore = Color.Black;
        public static readonly Color GridSelectionBack = Color.FromArgb(0, 120, 215);
        public static readonly Color GridSelectionFore = Color.White;
    }

    /// <summary>
    /// Paleta temei întunecate.
    /// </summary>
    private static class DarkPalette
    {
        public static readonly Color Background = Color.FromArgb(45, 45, 48);      // gri-închis
        public static readonly Color Surface = Color.FromArgb(30, 30, 30);         // fundal grilă
        public static readonly Color Foreground = Color.FromArgb(240, 240, 240);
        public static readonly Color GridHeaderBack = Color.FromArgb(60, 60, 65);
        public static readonly Color GridHeaderFore = Color.FromArgb(240, 240, 240);
        public static readonly Color GridSelectionBack = Color.FromArgb(0, 120, 215);
        public static readonly Color GridSelectionFore = Color.White;
    }

    /// <summary>
    /// Aplică tema specificată asupra unui formular și a tuturor controalelor sale (recursiv).
    /// </summary>
    /// <param name="form">Formularul-țintă.</param>
    /// <param name="theme">Tema de aplicat.</param>
    public static void Apply(Form form, AppTheme theme)
    {
        if (form == null) return;

        bool dark = theme == AppTheme.Dark;
        Color back = dark ? DarkPalette.Background : LightPalette.Background;
        Color surface = dark ? DarkPalette.Surface : LightPalette.Surface;
        Color fore = dark ? DarkPalette.Foreground : LightPalette.Foreground;

        form.BackColor = back;
        form.ForeColor = fore;

        ApplyToControls(form.Controls, theme, back, surface, fore);
    }

    /// <summary>
    /// Aplică recursiv tema asupra unei colecții de controale.
    /// </summary>
    /// <param name="controls">Colecția de controale.</param>
    /// <param name="theme">Tema de aplicat.</param>
    /// <param name="back">Culoare de fundal pentru containere.</param>
    /// <param name="surface">Culoare pentru suprafețele de date (grile, textboxuri).</param>
    /// <param name="fore">Culoare text.</param>
    private static void ApplyToControls(Control.ControlCollection controls, AppTheme theme,
                                         Color back, Color surface, Color fore)
    {
        bool dark = theme == AppTheme.Dark;

        foreach (Control c in controls)
        {
            switch (c)
            {
                case DataGridView grid:
                    grid.BackgroundColor = surface;
                    grid.GridColor = dark ? Color.FromArgb(70, 70, 70) : Color.LightGray;
                    grid.DefaultCellStyle.BackColor = surface;
                    grid.DefaultCellStyle.ForeColor = fore;
                    grid.DefaultCellStyle.SelectionBackColor =
                        dark ? DarkPalette.GridSelectionBack : LightPalette.GridSelectionBack;
                    grid.DefaultCellStyle.SelectionForeColor =
                        dark ? DarkPalette.GridSelectionFore : LightPalette.GridSelectionFore;
                    grid.ColumnHeadersDefaultCellStyle.BackColor =
                        dark ? DarkPalette.GridHeaderBack : LightPalette.GridHeaderBack;
                    grid.ColumnHeadersDefaultCellStyle.ForeColor =
                        dark ? DarkPalette.GridHeaderFore : LightPalette.GridHeaderFore;
                    grid.EnableHeadersVisualStyles = false;
                    grid.RowHeadersDefaultCellStyle.BackColor =
                        dark ? DarkPalette.GridHeaderBack : LightPalette.GridHeaderBack;
                    grid.RowHeadersDefaultCellStyle.ForeColor =
                        dark ? DarkPalette.GridHeaderFore : LightPalette.GridHeaderFore;
                    break;

                case Button btn:
                    btn.BackColor = dark ? Color.FromArgb(63, 63, 70) : SystemColors.Control;
                    btn.ForeColor = fore;
                    btn.FlatStyle = dark ? FlatStyle.Flat : FlatStyle.Standard;
                    if (dark) btn.FlatAppearance.BorderColor = Color.FromArgb(90, 90, 90);
                    break;

                case TextBox or ComboBox or DateTimePicker or NumericUpDown:
                    c.BackColor = surface;
                    c.ForeColor = fore;
                    break;

                case Label or CheckBox or RadioButton or LinkLabel:
                    c.BackColor = back;
                    c.ForeColor = fore;
                    break;

                case GroupBox or Panel or TabPage or TabControl or ToolStrip or StatusStrip or MenuStrip:
                    c.BackColor = back;
                    c.ForeColor = fore;
                    break;

                default:
                    c.BackColor = back;
                    c.ForeColor = fore;
                    break;
            }

            // Recursiv pentru containere.
            if (c.HasChildren)
                ApplyToControls(c.Controls, theme, back, surface, fore);
        }
    }
}
