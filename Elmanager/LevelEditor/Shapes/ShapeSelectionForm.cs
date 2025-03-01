using Elmanager.Rendering;
using OpenTK.GLControl;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Elmanager.IO;
using Elmanager.Lev;
using Path = System.IO.Path;

namespace Elmanager.LevelEditor.Shapes;

internal partial class ShapeSelectionForm : Form
{
    // Selected control
    public string? SelectedShapeFilePath { get; private set; }
    private CustomShapeControl? _selectedShapeControl;

    // The asterisk (*) is used to ensure the "All Shapes" option does not conflict with any actual folder names.
    private const string AllShapesOption = "* All Shapes *";

    // Static fields
    private static string? _lastSelectedSubfolder;
    private static readonly string ShapesFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sle_shapes");
    private static Size? _lastSize; // Last size of the form

    // Constants
    private const int Columns = 4; // 3 rows × 4 columns
    private const int Rows = 3; // 3 rows × 4 columns

    // Rendering
    private readonly ElmaRenderer _renderer;
    private readonly SceneSettings _sceneSettings;
    private readonly RenderingSettings _renderingSettings;

    private readonly List<CustomShapeControl> _reusableControls;

    // List of currently active shapes
    private List<(string Name, string FilePath)> _shapes;

    public ShapeSelectionForm(GLControl sharedContext, ElmaRenderer elmaRenderer, string? selectedShapeFilePath = null)
    {
        SelectedShapeFilePath = selectedShapeFilePath;

        _renderingSettings = new LevelEditorRenderingSettings
        {
            ShowGrid = false,
            ShowObjects = false,
            ShowGrassEdges = true,
            ShowGround = false,
            ShowPictures = false,
            ShowGroundEdges = true,
            ShowTextures = false,
            ShowObjectFrames = true,
            ShowVertices = false,
            ShowTextureFrames = true,
            ShowPictureFrames = true,
            ShowGravityAppleArrows = true,
            LineWidth = 1,
            ShowGrass = false,
            DisableFrameBuffer = true
        };

        _shapes = new List<(string Name, string FilePath)>();

        _sceneSettings = new SceneSettings()
        {
            PicturesInBackground = true
        };

        _renderer = elmaRenderer;

        _reusableControls = new List<CustomShapeControl>();

        InitializeComponent();

        // Hack to get the DPI scaling factor
        float dpiScalingFactorX = buttonCancel.Width / 75.0f; // 75 is the default width of the Cancel button
        vScrollBar1.Width = (int) (vScrollBar1.Width * dpiScalingFactorX);

        PopulateSubfolderComboBox();

        SetupReusableControls(sharedContext, Columns * Rows);

        tableLayoutPanelShapes.MouseWheel += FlowLayoutPanelShapes_MouseWheel;

        vScrollBar1.ValueChanged += vScrollBar_ValueChanged;

        if (_lastSize.HasValue)
        {
            this.Size = _lastSize.Value;
        }
    }

    private void SetupScrollBar(int count)
    {
        int totalRows = (int)Math.Ceiling(1.0 * count / Columns);
        vScrollBar1.Minimum = 0;
        vScrollBar1.Maximum = Math.Max(totalRows - Rows, 0);  // Ensure the scrollbar has valid range
        vScrollBar1.LargeChange = 1;
        vScrollBar1.SmallChange = 1;
        vScrollBar1.Enabled = totalRows > Rows;
    }

    private void FlowLayoutPanelShapes_MouseWheel(object? sender, MouseEventArgs e)
    {
        // Adjust the scrollbar value based on the mouse wheel scroll direction
        if (e.Delta > 0)
        {
            // Scroll up
            if (vScrollBar1.Value > vScrollBar1.Minimum)
            {
                vScrollBar1.Value -= vScrollBar1.SmallChange;
            }
        }
        else if (e.Delta < 0)
        {
            // Scroll down
            if (vScrollBar1.Value < vScrollBar1.Maximum)
            {
                vScrollBar1.Value += vScrollBar1.SmallChange;
            }
        }
    }

    private void vScrollBar_ValueChanged(object? sender, EventArgs e)
    {
        UpdateDisplayedControls(vScrollBar1.Value);
    }

    private void UpdateDisplayedControls(int startPage)
    {
        tableLayoutPanelShapes.SuspendLayout();

        int startIndex = startPage * Columns;

        foreach (var customShapeControl in _reusableControls)
        {
            customShapeControl.DisableLevelRendering(true);
        }

        for (int i = 0; i < _reusableControls.Count; i++)
        {
            int dataIndex = startIndex + i;
            if (dataIndex < _shapes.Count)
            {
                _reusableControls[i].Highlight(false);
                _reusableControls[i].UpdateContent(_shapes[dataIndex].FilePath, _shapes[dataIndex].Name);
                _reusableControls[i].Visible = true;
            }
            else
            {
                _reusableControls[i].Visible = false;
            }
        }

        foreach (var customShapeControl in _reusableControls)
        {
            customShapeControl.DisableLevelRendering(false);
        }

        HighlightSelectedShape();

        tableLayoutPanelShapes.ResumeLayout();
        tableLayoutPanelShapes.Invalidate();
        tableLayoutPanelShapes.Refresh();
    }

    private void HighlightSelectedShape()
    {
        if (SelectedShapeFilePath == null)
        {
            return;
        }

        foreach (System.Windows.Forms.Control control in tableLayoutPanelShapes.Controls)
        {
            if (control is CustomShapeControl shapeControl && shapeControl.ShapeFullPath == SelectedShapeFilePath)
            {
                shapeControl.Highlight(true);
                _selectedShapeControl = shapeControl;
                break;
            }
        }
    }

    private void SetupReusableControls(GLControl sharedContext, int numControls)
    {
        for (int i = 0; i < numControls; i++)
        {
            CustomShapeControl shapeControl = new CustomShapeControl(sharedContext, _sceneSettings, _renderingSettings, _renderer, new Level());
            // Handle shape click event
            shapeControl.ShapeClicked += ShapeControl_ShapeClicked;

            // Handle shape double-click event
            shapeControl.ShapeDoubleClicked += ShapeControl_ShapeDoubleClicked;

            shapeControl.Visible = false;
            shapeControl.DisableLevelRendering(true);
            shapeControl.Dock = DockStyle.Fill;
            tableLayoutPanelShapes.Controls.Add(shapeControl, i % 4, i / 4); // (column, row)
            _reusableControls.Add(shapeControl);
        }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        // Save the current size
        _lastSize = this.Size;

        // Disable the form's painting
        this.Visible = false;
        this.SuspendLayout();

        base.OnFormClosing(e);
    }

    // Event for loading polygons
    public event EventHandler<ElmaFileObject<SleShape>>? ShapeDataLoaded;

    private void PopulateShapes(string folderPath)
    {
        if (!Directory.Exists(folderPath))
        {
            return;
        }

        // Load shapes from the specified folder
        _shapes = Directory.GetFiles(folderPath, "*.lev")
            .Select(filePath => (Name: Path.GetFileNameWithoutExtension(filePath), ShapePath: filePath))
            .OrderBy(shape => shape.Name, new CustomComparer<string>(CompareNatural))
            .ToList();

        SetupScrollBar(_shapes.Count);

        UpdateDisplayedControls(0);
    }

    private void ScrollToSelectedShape()
    {
        if (SelectedShapeFilePath == null)
        {
            return;
        }

        int selectedIndex = _shapes.FindIndex(shape => shape.FilePath == SelectedShapeFilePath);
        if (selectedIndex == -1)
        {
            return;
        }

        int selectedRow = selectedIndex / Columns;
        vScrollBar1.Value = Math.Min(selectedRow, vScrollBar1.Maximum);
    }


    private void ShapeControl_ShapeClicked(object? sender, EventArgs e)
    {
        // Deselect the previously selected shape
        if (_selectedShapeControl != null)
        {
            _selectedShapeControl.Highlight(false);
        }

        // Select the new shape
        _selectedShapeControl = sender as CustomShapeControl;
        if (_selectedShapeControl != null)
        {
            _selectedShapeControl.Highlight(true);
            SelectedShapeFilePath = _selectedShapeControl.ShapeFullPath;
        }
    }

    private void ShapeSelectionForm_Load(object sender, EventArgs e)
    {
        PopulateShapeSelectionFromLastSelectedSubfolder();
        HighlightSelectedShape();
    }

    private void ButtonOk_Click(object sender, EventArgs e)
    {
        if (SelectedShapeFilePath != null)
        {
            // Raise the ShapeDataLoaded event with the selected shape data
            if (SelectedShapeFilePath.Length == 0)
            {
                return;
            }

            var levFilePath = SelectedShapeFilePath;

            // Check if the LEV file exists
            if (!File.Exists(levFilePath))
            {
                MessageBox.Show($@"Corresponding LEV file not found: {levFilePath}", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var shapeData = SleShape.LoadFromPath(levFilePath);
            ShapeDataLoaded?.Invoke(this, shapeData);
        }
        this.DialogResult = DialogResult.OK;
        this.Close();
    }

    private void PopulateSubfolderComboBox()
    {
        // Clear existing items
        comboBoxSubfolders.Items.Clear();

        if (!Directory.Exists(ShapesFolderPath))
        {
            return;
        }

        // Get all subfolders in the shapes folder
        var subfolders = Directory.GetDirectories(ShapesFolderPath);

        // Add subfolders to the ComboBox
        foreach (var subfolder in subfolders)
        {
            comboBoxSubfolders.Items.Add(Path.GetFileName(subfolder));
        }

        // Add an "All shapes" item last so users can view all shapes in one view
        comboBoxSubfolders.Items.Add(AllShapesOption);

        // Set the default selected item to the last selected subfolder
        if (_lastSelectedSubfolder != null && comboBoxSubfolders.Items.Contains(_lastSelectedSubfolder))
        {
            comboBoxSubfolders.SelectedItem = _lastSelectedSubfolder;
        }
        else if (comboBoxSubfolders.Items.Count > 0)
        {
            comboBoxSubfolders.SelectedIndex = 0;
        }

        // Handle ComboBox selection change event
        comboBoxSubfolders.SelectedIndexChanged += ComboBoxSubfolders_SelectedIndexChanged;
    }

    private void ComboBoxSubfolders_SelectedIndexChanged(object? sender, EventArgs e)
    {
        // Get the selected subfolder
        var selectedSubfolder = comboBoxSubfolders.SelectedItem?.ToString();

        // Update the last selected subfolder
        _lastSelectedSubfolder = selectedSubfolder;

        // Construct the full path to the selected subfolder
        if (selectedSubfolder != null)
        {
            if (selectedSubfolder == AllShapesOption)
            {
                // Populate with shapes from all subfolders
                PopulateShapesFromAllSubfolders();
            }
            else
            {
                var selectedFolderPath = Path.Combine(ShapesFolderPath, selectedSubfolder);

                // Populate with shapes from the selected subfolder
                PopulateShapes(selectedFolderPath);
            }
        }
    }

    private void PopulateShapeSelectionFromLastSelectedSubfolder()
    {
        if (_lastSelectedSubfolder != null && comboBoxSubfolders.Items.Contains(_lastSelectedSubfolder))
        {
            if (_lastSelectedSubfolder == AllShapesOption)
            {
                PopulateShapesFromAllSubfolders();
            }
            else
            {
                var lastSubfolderPath = Path.Combine(ShapesFolderPath, _lastSelectedSubfolder);
                PopulateShapes(lastSubfolderPath);
            }

            ScrollToSelectedShape();
        }
        else if (comboBoxSubfolders.Items.Count > 0)
        {
            var firstSubfolder = comboBoxSubfolders.Items[0]?.ToString();
            if (firstSubfolder != null)
            {
                var firstSubfolderPath = Path.Combine(ShapesFolderPath, firstSubfolder);
                PopulateShapes(firstSubfolderPath);
                ScrollToSelectedShape();
            }
        }
    }

    private void PopulateShapesFromAllSubfolders()
    {
        if (!Directory.Exists(ShapesFolderPath))
        {
            return;
        }

        // Get all shapes from all subfolders, grouped by subfolder
        _shapes = Directory.GetDirectories(ShapesFolderPath)
            .SelectMany(subfolder => Directory.GetFiles(subfolder, "*.lev")
                .Select(filePath => (Name: Path.GetFileNameWithoutExtension(filePath), ShapePath: filePath, Subfolder: Path.GetFileName(subfolder))))
            .OrderBy(shape => shape.Subfolder)
            .ThenBy(shape => shape.Name, new CustomComparer<string>(CompareNatural))
            .Select(shape => (shape.Name, shape.ShapePath))
            .ToList();

        SetupScrollBar(_shapes.Count);

        UpdateDisplayedControls(0);
    }

    private void ShapeControl_ShapeDoubleClicked(object? sender, EventArgs e)
    {
        if (sender is CustomShapeControl shapeControl)
        {
            // Reuse the existing shape click handler
            ShapeControl_ShapeClicked(shapeControl, e);

            // Reuse the existing OK button click handler to close the dialog
            ButtonOk_Click(this, e);
        }
    }

    // Taken from
    // https://stackoverflow.com/a/7048016
    public static int CompareNatural(string strA, string strB)
    {
        return CompareNatural(strA, strB, CultureInfo.CurrentCulture, CompareOptions.IgnoreCase);
    }

    public static int CompareNatural(string strA, string strB, CultureInfo culture, CompareOptions options)
    {
        CompareInfo cmp = culture.CompareInfo;
        int iA = 0;
        int iB = 0;
        int softResult = 0;
        int softResultWeight = 0;
        while (iA < strA.Length && iB < strB.Length)
        {
            bool isDigitA = Char.IsDigit(strA[iA]);
            bool isDigitB = Char.IsDigit(strB[iB]);
            if (isDigitA != isDigitB)
            {
                return cmp.Compare(strA, iA, strB, iB, options);
            }
            else if (!isDigitA && !isDigitB)
            {
                int jA = iA + 1;
                int jB = iB + 1;
                while (jA < strA.Length && !Char.IsDigit(strA[jA])) jA++;
                while (jB < strB.Length && !Char.IsDigit(strB[jB])) jB++;
                int cmpResult = cmp.Compare(strA, iA, jA - iA, strB, iB, jB - iB, options);
                if (cmpResult != 0)
                {
                    // Certain strings may be considered different due to "soft" differences that are
                    // ignored if more significant differences follow, e.g. a hyphen only affects the
                    // comparison if no other differences follow
                    string sectionA = strA.Substring(iA, jA - iA);
                    string sectionB = strB.Substring(iB, jB - iB);
                    if (cmp.Compare(sectionA + "1", sectionB + "2", options) ==
                        cmp.Compare(sectionA + "2", sectionB + "1", options))
                    {
                        return cmp.Compare(strA, iA, strB, iB, options);
                    }
                    else if (softResultWeight < 1)
                    {
                        softResult = cmpResult;
                        softResultWeight = 1;
                    }
                }
                iA = jA;
                iB = jB;
            }
            else
            {
                char zeroA = (char)(strA[iA] - (int)Char.GetNumericValue(strA[iA]));
                char zeroB = (char)(strB[iB] - (int)Char.GetNumericValue(strB[iB]));
                int jA = iA;
                int jB = iB;
                while (jA < strA.Length && strA[jA] == zeroA) jA++;
                while (jB < strB.Length && strB[jB] == zeroB) jB++;
                int resultIfSameLength = 0;
                do
                {
                    isDigitA = jA < strA.Length && Char.IsDigit(strA[jA]);
                    isDigitB = jB < strB.Length && Char.IsDigit(strB[jB]);
                    int numA = isDigitA ? (int)Char.GetNumericValue(strA[jA]) : 0;
                    int numB = isDigitB ? (int)Char.GetNumericValue(strB[jB]) : 0;
                    if (isDigitA && (char)(strA[jA] - numA) != zeroA) isDigitA = false;
                    if (isDigitB && (char)(strB[jB] - numB) != zeroB) isDigitB = false;
                    if (isDigitA && isDigitB)
                    {
                        if (numA != numB && resultIfSameLength == 0)
                        {
                            resultIfSameLength = numA < numB ? -1 : 1;
                        }
                        jA++;
                        jB++;
                    }
                }
                while (isDigitA && isDigitB);
                if (isDigitA != isDigitB)
                {
                    // One number has more digits than the other (ignoring leading zeros) - the longer
                    // number must be larger
                    return isDigitA ? 1 : -1;
                }
                else if (resultIfSameLength != 0)
                {
                    // Both numbers are the same length (ignoring leading zeros) and at least one of
                    // the digits differed - the first difference determines the result
                    return resultIfSameLength;
                }
                int lA = jA - iA;
                int lB = jB - iB;
                if (lA != lB)
                {
                    // Both numbers are equivalent but one has more leading zeros
                    return lA > lB ? -1 : 1;
                }
                else if (zeroA != zeroB && softResultWeight < 2)
                {
                    softResult = cmp.Compare(strA, iA, 1, strB, iB, 1, options);
                    softResultWeight = 2;
                }
                iA = jA;
                iB = jB;
            }
        }
        if (iA < strA.Length || iB < strB.Length)
        {
            return iA < strA.Length ? 1 : -1;
        }
        else if (softResult != 0)
        {
            return softResult;
        }
        return 0;
    }

    public class CustomComparer<T> : IComparer<T>
    {
        private readonly Comparison<T> _comparison;

        public CustomComparer(Comparison<T> comparison)
        {
            _comparison = comparison;
        }

        public int Compare(T? x, T? y)
        {
            if (x == null && y == null)
            {
                return 0;
            }

            if (x == null)
            {
                return -1;
            }

            if (y == null)
            {
                return 1;
            }

            return _comparison(x, y);
        }
    }
}
