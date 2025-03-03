using Elmanager.Rendering;
using OpenTK.GLControl;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Elmanager.IO;
using Elmanager.Lev;
using Path = System.IO.Path;
using NaturalSort.Extension;

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

        if (_lastSize is { } size)
        {
            Size = size;
        }
    }

    public static ElmaFileObject<SleShape>? ShowForm(GLControl sharedContext, ElmaRenderer elmaRenderer, string? selectedShapeFilePath = null)
    {
        var form = new ShapeSelectionForm(sharedContext, elmaRenderer, selectedShapeFilePath);
        
        if (form.ShowDialog() == DialogResult.OK && form.SelectedShapeFilePath != null)
        {
            return SleShape.LoadFromPath(form.SelectedShapeFilePath);
        }

        return null;
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
            
            shapeControl.ShapeClicked += ShapeControl_ShapeClicked;
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
        _lastSize = this.Size;

        this.Visible = false;
        this.SuspendLayout();

        base.OnFormClosing(e);
    }

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
            .OrderBy(shape => shape.Name, StringComparison.OrdinalIgnoreCase.WithNaturalSort())
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
        if (_selectedShapeControl != null)
        {
            _selectedShapeControl.Highlight(false);
        }

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
            if (SelectedShapeFilePath.Length == 0)
            {
                return;
            }

            var levFilePath = SelectedShapeFilePath;

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
        comboBoxSubfolders.Items.Clear();

        if (!Directory.Exists(ShapesFolderPath))
        {
            return;
        }

        var subfolders = Directory.GetDirectories(ShapesFolderPath);

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

        comboBoxSubfolders.SelectedIndexChanged += ComboBoxSubfolders_SelectedIndexChanged;
    }

    private void ComboBoxSubfolders_SelectedIndexChanged(object? sender, EventArgs e)
    {
        var selectedSubfolder = comboBoxSubfolders.SelectedItem?.ToString();

        _lastSelectedSubfolder = selectedSubfolder;

        if (selectedSubfolder != null)
        {
            if (selectedSubfolder == AllShapesOption)
            {
                PopulateShapesFromAllSubfolders();
            }
            else
            {
                var selectedFolderPath = Path.Combine(ShapesFolderPath, selectedSubfolder);

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
            .ThenBy(shape => shape.Name, StringComparison.OrdinalIgnoreCase.WithNaturalSort())
            .Select(shape => (shape.Name, shape.ShapePath))
            .ToList();

        SetupScrollBar(_shapes.Count);

        UpdateDisplayedControls(0);
    }

    private void ShapeControl_ShapeDoubleClicked(object? sender, EventArgs e)
    {
        if (sender is CustomShapeControl shapeControl)
        {
            ShapeControl_ShapeClicked(shapeControl, e);

            ButtonOk_Click(this, e);
        }
    }
}
