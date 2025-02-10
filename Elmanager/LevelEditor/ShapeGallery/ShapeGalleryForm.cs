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

namespace Elmanager.LevelEditor.ShapeGallery;

internal partial class ShapeGalleryForm : Form
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

    private int columns = 4;  // 3 rows × 4 columns
    private int rows = 3;  // 3 rows × 4 columns

    private double ClampAngle(double angle, double min, double max)
    {
        double range = max - min;
        return ((angle - min) % range + range) % range + min;
    }

    private readonly SceneSettings _sceneSettings;
    private RenderingSettings _renderingSettings;
    private readonly GLControl _sharedContext;
    private List<CustomShapeControl> reusableControls;
    private List<(string Name, string FilePath)> shapes;

    public ShapeGalleryForm(GLControl sharedContext, ElmaRenderer elmaRenderer, RenderingSettings renderingSettings, SceneSettings sceneSettings, string? selectedShapeName = null, double scalingFactor = 0.0, double rotationAngle = 0.0, ShapeMirrorOption mirrorOption = ShapeMirrorOption.None)
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
            ShowGrass = false
        };

        _sharedContext = sharedContext;
        _sceneSettings = sceneSettings;

        _renderer = elmaRenderer;

        InitializeComponent();

        scalingNumericUpDown.Value = (decimal)Double.Clamp(ScalingFactor, (double)scalingNumericUpDown.Minimum, (double)scalingNumericUpDown.Maximum);
        scalingNumericUpDown.ValueChanged += (sender, e) => ScalingFactor = (double)scalingNumericUpDown.Value;

        rotationNumericUpDown.Value = (decimal)ClampAngle(RotationAngle, (double)rotationNumericUpDown.Minimum, (double)rotationNumericUpDown.Maximum);
        rotationNumericUpDown.ValueChanged += (sender, e) => RotationAngle = (double)rotationNumericUpDown.Value;

        mirrorComboBox.Items.Clear();
        foreach (ShapeMirrorOption option in Enum.GetValues<ShapeMirrorOption>())
        {
            mirrorComboBox.Items.Add(option);
        }

        mirrorComboBox.SelectedIndexChanged += (sender, e) =>
        {
            ShapeMirrorOption = (ShapeMirrorOption)(mirrorComboBox.SelectedItem ?? ShapeMirrorOption.None);
        };
        mirrorComboBox.SelectedItem = ShapeMirrorOption;

        this.StartPosition = FormStartPosition.Manual;
        SetInitialLocation();
        PopulateSubfolderComboBox();

        // Pass the shared context to the PopulateShapeGallery method
        //PopulateShapeGalleryFromLastSelectedSubfolder(_sharedContext);
        
        SetupReusableControls(sharedContext, columns*rows);

        vScrollBar1.ValueChanged += vScrollBar_ValueChanged;
    }

    private void SetupScrollBar(int count)
    {
        int totalPages = (int)Math.Ceiling(1.0 * count / (columns * rows));
        vScrollBar1.Minimum = 0;
        vScrollBar1.Maximum = Math.Max(totalPages - 1, 0);  // Ensure the scrollbar has valid range
        vScrollBar1.LargeChange = 1;
        vScrollBar1.SmallChange = 1;
    }

    private void vScrollBar_ValueChanged(object sender, EventArgs e)
    {
        UpdateDisplayedControls(vScrollBar1.Value);
    }

    private void UpdateDisplayedControls(int startPage)
    {
        flowLayoutPanelShapes.SuspendLayout();
        int startIndex = startPage * columns;

        for (int i = 0; i < reusableControls.Count; i++)
        {
            int dataIndex = startIndex + i;
            if (dataIndex < shapes.Count)
            {
                reusableControls[i].UpdateContent(shapes[dataIndex].FilePath, shapes[dataIndex].Name);
                reusableControls[i].Visible = true;
            }
            else
            {
                reusableControls[i].Visible = false;
            }

            reusableControls[i].Invalidate();
            reusableControls[i].Refresh();
        }

        flowLayoutPanelShapes.ResumeLayout();
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
        reusableControls = new List<CustomShapeControl>();

        for (int i = 0; i < numControls; i++)
        {
            CustomShapeControl shapeControl = new CustomShapeControl(sharedContext, _sceneSettings, _renderingSettings, _renderer);
            // Handle shape click event
            shapeControl.ShapeClicked += ShapeControl_ShapeClicked;

            // Handle polygons loaded event
            shapeControl.ShapeDataLoaded += (sender, shapeDataDto) =>
            {
                ShapeDataLoaded?.Invoke(this, shapeDataDto);
            };

            flowLayoutPanelShapes.Controls.Add(shapeControl);
            reusableControls.Add(shapeControl);
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
        flowLayoutPanelShapes.SuspendLayout();

        if (!Directory.Exists(folderPath))
        {
            flowLayoutPanelShapes.ResumeLayout();
            return;
        }

        // Load images from the specified folder
        shapes = Directory.GetFiles(folderPath, "*.lev")
            .Select(filePath => (Name: Path.GetFileNameWithoutExtension(filePath), ImagePath: filePath))
            .ToList();

        SetupScrollBar(shapes.Count);

        UpdateDisplayedControls(0);

        flowLayoutPanelShapes.ResumeLayout();
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

    private void resetValuesButton_Click(object sender, EventArgs e)
    {
        ScalingFactor = 1.0;
        RotationAngle = 0.0;
        ShapeMirrorOption = ShapeMirrorOption.None;

        rotationNumericUpDown.Value = (decimal)RotationAngle;
        scalingNumericUpDown.Value = (decimal)ScalingFactor;
        mirrorComboBox.SelectedItem = ShapeMirrorOption;
    }
}
