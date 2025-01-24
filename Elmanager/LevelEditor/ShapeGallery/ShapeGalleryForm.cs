using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Elmanager.LevelEditor.ShapeGallery;

internal partial class ShapeGalleryForm : Form
{
    public double ScalingFactor { get; private set; }
    public double RotationAngle { get; private set; }
    public ShapeMirrorOption ShapeMirrorOption { get; private set; }
    public string? SelectedShapeName { get; private set; }

    private string _galleryFolderPath = "Gallery"; // Path to the folder to monitor

    private CustomShapeControl? _selectedShapeControl;

    // Static field to remember the last selected subfolder
    private static string? _lastSelectedSubfolder;

    private double ClampAngle(double angle, double min, double max)
    {
        double range = max - min;
        return ((angle - min) % range + range) % range + min;
    }

    public ShapeGalleryForm(string? selectedShapeName = null, double scalingFactor = 0.0, double rotationAngle = 0.0, ShapeMirrorOption mirrorOption = ShapeMirrorOption.None)
    {
        ScalingFactor = scalingFactor;
        RotationAngle = rotationAngle;
        ShapeMirrorOption = mirrorOption;
        SelectedShapeName = selectedShapeName;

        InitializeComponent();

            
        scalingNumericUpDown.Value = (decimal)Double.Clamp(ScalingFactor, (double)scalingNumericUpDown.Minimum, (double)scalingNumericUpDown.Maximum);
        scalingNumericUpDown.ValueChanged += (sender, e) => ScalingFactor = (double)scalingNumericUpDown.Value;

        rotationNumericUpDown.Value = (decimal)ClampAngle(RotationAngle, (double)rotationNumericUpDown.Minimum, (double)rotationNumericUpDown.Maximum);
        rotationNumericUpDown.ValueChanged += (sender, e) => RotationAngle = (double)rotationNumericUpDown.Value;

        mirrorComboBox.DataSource = Enum.GetValues(typeof(ShapeMirrorOption));
        mirrorComboBox.SelectedIndexChanged += (sender, e) =>
        {
            ShapeMirrorOption = (ShapeMirrorOption)(mirrorComboBox.SelectedValue ?? ShapeMirrorOption.None);
        };
        mirrorComboBox.SelectedItem = ShapeMirrorOption;

        this.StartPosition = FormStartPosition.Manual;
        SetInitialLocation();
        PopulateSubfolderComboBox();
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
        Rectangle screenBounds = Screen.FromPoint(mousePosition).WorkingArea;

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

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        // Disable the form's painting
        this.Visible = false;
        this.SuspendLayout();

        base.OnFormClosing(e);
    }

    // Event for loading polygons
    public event EventHandler<ShapeDataDto>? ShapeDataLoaded;

    private void PopulateShapeGallery(string folderPath)
    {
        flowLayoutPanelShapes.SuspendLayout();

        // Clear any existing items
        flowLayoutPanelShapes.Controls.Clear();

        if (!Directory.Exists(folderPath))
        {
            flowLayoutPanelShapes.ResumeLayout();
            return;
        }

        // Load images from the specified folder
        var shapes = Directory.GetFiles(folderPath, "*.png")
            .Select(filePath => (Name: Path.GetFileNameWithoutExtension(filePath), ImagePath: filePath))
            .ToList();

        foreach (var shape in shapes)
        {
            var shapeControl = new CustomShapeControl()
            {
                ShapeName = shape.Name,
                ShapeFullPath = shape.ImagePath // Store the full path
            };

            using (var s = new FileStream(shapeControl.ShapeFullPath, FileMode.Open))
            {
                shapeControl.ShapeImage = Image.FromStream(s);
            }

            // Handle shape click event
            shapeControl.ShapeClicked += ShapeControl_ShapeClicked;

            // Handle polygons loaded event
            shapeControl.ShapeDataLoaded += (sender, shapeDataDto) =>
            {
                ShapeDataLoaded?.Invoke(this, shapeDataDto);
            };

            flowLayoutPanelShapes.Controls.Add(shapeControl);
        }

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
        PopulateShapeGalleryFromLastSelectedSubfolder();
        HighlightSelectedShape();
    }

    private void ButtonOk_Click(object sender, EventArgs e)
    {
        if (_selectedShapeControl != null)
        {
            // Raise the ShapeDataLoaded event with the selected shape data
            _selectedShapeControl.LoadShapeJson();
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

        // Get all subfolders in the gallery folder
        var subfolders = Directory.GetDirectories(_galleryFolderPath);

        // Add subfolders to the ComboBox
        foreach (var subfolder in subfolders)
        {
            comboBoxSubfolders.Items.Add(Path.GetFileName(subfolder));
        }

        // Handle ComboBox selection change event
        comboBoxSubfolders.SelectedIndexChanged += ComboBoxSubfolders_SelectedIndexChanged;

        // Set the default selected item to the last selected subfolder
        if (_lastSelectedSubfolder != null && comboBoxSubfolders.Items.Contains(_lastSelectedSubfolder))
        {
            comboBoxSubfolders.SelectedItem = _lastSelectedSubfolder;
        }
        else if (comboBoxSubfolders.Items.Count > 0)
        {
            comboBoxSubfolders.SelectedIndex = 0;
        }
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
            PopulateShapeGallery(selectedFolderPath);
        }
    }

    private void PopulateShapeGalleryFromLastSelectedSubfolder()
    {
        if (_lastSelectedSubfolder != null && comboBoxSubfolders.Items.Contains(_lastSelectedSubfolder))
        {
            var lastSubfolderPath = Path.Combine(_galleryFolderPath, _lastSelectedSubfolder);
            PopulateShapeGallery(lastSubfolderPath);
        }
        else if (comboBoxSubfolders.Items.Count > 0)
        {
            var firstSubfolder = comboBoxSubfolders.Items[0]?.ToString();
            if (firstSubfolder != null)
            {
                var firstSubfolderPath = Path.Combine(_galleryFolderPath, firstSubfolder);
                PopulateShapeGallery(firstSubfolderPath);
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