using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Shapes;
using BrightIdeasSoftware;
using Elmanager.Lev;
using Elmanager.Rendering;
using OpenTK.GLControl;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Path = System.IO.Path;

namespace Elmanager.LevelEditor.Shapes;

internal partial class ShapeSelectionForm : Form
{
    public double ScalingFactor { get; private set; }
    public double RotationAngle { get; private set; }
    public ShapeMirrorOption ShapeMirrorOption { get; private set; }
    public string? SelectedShapeName { get; private set; }

    private string _galleryFolderPath = "Gallery"; // Path to the folder to monitor

    private CustomShapeControl? _selectedShapeControl;

    private ElmaRenderer _renderer;

    // Static field to remember the last selected subfolder
    private static string? _lastSelectedSubfolder;

    private const int Columns = 4; // 3 rows × 4 columns
    private const int Rows = 3; // 3 rows × 4 columns

    private readonly SceneSettings _sceneSettings;
    private RenderingSettings _renderingSettings;
    private readonly GLControl _sharedContext;
    private List<CustomShapeControl> _reusableControls;
    private List<(string Name, string FilePath)> _shapes;

    public ShapeSelectionForm(GLControl sharedContext, ElmaRenderer elmaRenderer, string? selectedShapeName = null, double scalingFactor = 0.0, double rotationAngle = 0.0, ShapeMirrorOption mirrorOption = ShapeMirrorOption.None)
    {
        ScalingFactor = scalingFactor;
        RotationAngle = rotationAngle;
        ShapeMirrorOption = mirrorOption;
        SelectedShapeName = selectedShapeName;
        
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

        _sharedContext = sharedContext;
        _sceneSettings = new SceneSettings()
        {
            PicturesInBackground = true
        };

        _renderer = elmaRenderer;

        _reusableControls = new List<CustomShapeControl>();

        InitializeComponent();

        this.StartPosition = FormStartPosition.Manual;
        SetInitialLocation();
        PopulateSubfolderComboBox();

        // Pass the shared context to the PopulateShapeGallery method
        //PopulateShapeGalleryFromLastSelectedSubfolder(_sharedContext);
        
        SetupReusableControls(sharedContext, Columns*Rows);

        flowLayoutPanelShapes.MouseWheel += FlowLayoutPanelShapes_MouseWheel;

        vScrollBar1.ValueChanged += vScrollBar_ValueChanged;
    }

    private void SetupScrollBar(int count)
    {
        int totalPages = (int)Math.Ceiling(1.0 * count / (Columns * Rows));
        vScrollBar1.Minimum = 0;
        vScrollBar1.Maximum = Math.Max(totalPages - 1, 0);  // Ensure the scrollbar has valid range
        vScrollBar1.LargeChange = 1;
        vScrollBar1.SmallChange = 1;
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
        int startIndex = startPage * Columns;

        for (int i = 0; i < _reusableControls.Count; i++)
        {
            _reusableControls[i].DisableLevelRendering(true);
        }

        for (int i = 0; i < _reusableControls.Count; i++)
        {
            int dataIndex = startIndex + i;
            if (dataIndex < _shapes.Count)
            {
                _reusableControls[i].UpdateContent(_shapes[dataIndex].FilePath, _shapes[dataIndex].Name);
                _reusableControls[i].Visible = true;
            }
            else
            {
                _reusableControls[i].Visible = false;
            }
        }

        for (int i = 0; i < _reusableControls.Count; i++)
        {
            _reusableControls[i].DisableLevelRendering(false);
        }

        flowLayoutPanelShapes.Invalidate();
        flowLayoutPanelShapes.Refresh();
    }

    private void HighlightSelectedShape()
    {
        if (SelectedShapeName == null)
        {
            return;
        }

        foreach (Control control in flowLayoutPanelShapes.Controls)
        {
            if (control is CustomShapeControl shapeControl && shapeControl.ShapeName == SelectedShapeName)
            {
                shapeControl.Highlight(true);
                _selectedShapeControl = shapeControl;
                break;
            }
        }
    }

    private void SetInitialLocation()
    {
        // Define an offset value
        int offsetX = 5;
        int offsetY = 5;

        // Set the form's location to the current mouse position with an offset
        Point mousePosition = Cursor.Position;
        this.Location = new Point(mousePosition.X + offsetX, mousePosition.Y + offsetY);

        // Get the screen bounds
        System.Drawing.Rectangle screenBounds = Screen.FromPoint(mousePosition).WorkingArea;

        // Adjust the position to ensure the dialog is fully visible
        if (this.Right > screenBounds.Right)
        {
            this.Left = screenBounds.Right - this.Width;
        }
        if (this.Bottom > screenBounds.Bottom)
        {
            this.Top = screenBounds.Bottom - this.Height;
        }
        if (this.Left < screenBounds.Left)
        {
            this.Left = screenBounds.Left;
        }
        if (this.Top < screenBounds.Top)
        {
            this.Top = screenBounds.Top;
        }
    }

    private void SetupReusableControls(GLControl sharedContext, int numControls)
    {
        for (int i = 0; i < numControls; i++)
        {
            CustomShapeControl shapeControl = new CustomShapeControl(sharedContext, _sceneSettings, _renderingSettings, _renderer);
            // Handle shape click event
            shapeControl.ShapeClicked += ShapeControl_ShapeClicked;

            // Handle shape double-click event
            shapeControl.ShapeDoubleClicked += ShapeControl_ShapeDoubleClicked;

            // Handle polygons loaded event
            shapeControl.ShapeDataLoaded += (sender, shapeDataDto) =>
            {
                ShapeDataLoaded?.Invoke(this, shapeDataDto);
            };

            shapeControl.Visible = false;
            shapeControl.DisableLevelRendering(true);
            flowLayoutPanelShapes.Controls.Add(shapeControl);
            _reusableControls.Add(shapeControl);
        }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        // Disable the form's painting
        this.Visible = false;
        this.SuspendLayout();

        base.OnFormClosing(e);
    }

    // Event for loading polygons
    public event EventHandler<ShapeDataDto>? ShapeDataLoaded;

    private void PopulateShapeGallery(string folderPath, GLControl sharedContext)
    {
        if (!Directory.Exists(folderPath))
        {
            return;
        }

        // Load shapes from the specified folder
        _shapes = Directory.GetFiles(folderPath, "*.lev")
            .Select(filePath => (Name: Path.GetFileNameWithoutExtension(filePath), ShapePath: filePath))
            .ToList();

        SetupScrollBar(_shapes.Count);

        UpdateDisplayedControls(0);
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
            SelectedShapeName = _selectedShapeControl.ShapeName;
        }
    }

    private void ShapeGalleryForm_Load(object sender, EventArgs e)
    {
        //InitializeFileSystemWatcher();
        PopulateShapeGalleryFromLastSelectedSubfolder(_sharedContext);
        HighlightSelectedShape();
    }

    private void ButtonOk_Click(object sender, EventArgs e)
    {
        if (_selectedShapeControl != null)
        {
            // Raise the ShapeDataLoaded event with the selected shape data
            _selectedShapeControl.LoadShape();
            SelectedShapeName = _selectedShapeControl.ShapeName;
        }
        this.DialogResult = DialogResult.OK;
        this.Close();
    }

    private void ButtonCancel_Click(object sender, EventArgs e)
    {
        this.DialogResult = DialogResult.Cancel;
        this.Close();
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == Keys.Escape)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
            return true;
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }

    private void PopulateSubfolderComboBox()
    {
        // Clear existing items
        comboBoxSubfolders.Items.Clear();

        if (!Directory.Exists(_galleryFolderPath))
        {
            return;
        }

        // Get all subfolders in the gallery folder
        var subfolders = Directory.GetDirectories(_galleryFolderPath);

        // Add subfolders to the ComboBox
        foreach (var subfolder in subfolders)
        {
            comboBoxSubfolders.Items.Add(Path.GetFileName(subfolder));
        }
        
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
            var selectedFolderPath = Path.Combine(_galleryFolderPath, selectedSubfolder);

            // Populate the shape gallery with shapes from the selected subfolder
            PopulateShapeGallery(selectedFolderPath, _sharedContext);
        }
    }

    private void PopulateShapeGalleryFromLastSelectedSubfolder(GLControl sharedContext)
    {
        if (_lastSelectedSubfolder != null && comboBoxSubfolders.Items.Contains(_lastSelectedSubfolder))
        {
            var lastSubfolderPath = Path.Combine(_galleryFolderPath, _lastSelectedSubfolder);
            PopulateShapeGallery(lastSubfolderPath, sharedContext);
        }
        else if (comboBoxSubfolders.Items.Count > 0)
        {
            var firstSubfolder = comboBoxSubfolders.Items[0]?.ToString();
            if (firstSubfolder != null)
            {
                var firstSubfolderPath = Path.Combine(_galleryFolderPath, firstSubfolder);
                PopulateShapeGallery(firstSubfolderPath, sharedContext);
            }
        }
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
}
