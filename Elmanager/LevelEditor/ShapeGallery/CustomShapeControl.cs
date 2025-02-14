using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Color = System.Drawing.Color;
using Pen = System.Drawing.Pen;
using UserControl = System.Windows.Forms;
using System.Windows.Shapes;
using Elmanager.Lev;
using Elmanager.Rendering;
using OpenTK.GLControl;
using OpenTK.Windowing.Desktop;
using Path = System.IO.Path;
using OpenTK.Windowing.Common;

namespace Elmanager.LevelEditor.ShapeGallery;

internal partial class CustomShapeControl : UserControl.UserControl
{
    private bool _isSelected;
    private bool _isHighlighted;
    private Color _borderColor = Color.Transparent;
    private bool _isPressed;

    public CustomShapeControl(GLControl sharedContext, SceneSettings sceneSettings, RenderingSettings renderingSettings, ElmaRenderer elmaRenderer, Level? level=null)
    {
        InitializeComponent();

        shapeLevelControl = new LevelControl(sharedContext, sceneSettings, renderingSettings, elmaRenderer, level);
        shapeLevelControl.API = OpenTK.Windowing.Common.ContextAPI.OpenGL;
        shapeLevelControl.APIVersion = new Version(3, 3, 0, 0);
        shapeLevelControl.Flags = OpenTK.Windowing.Common.ContextFlags.Default;
        shapeLevelControl.Location = new System.Drawing.Point(0, 0);
        shapeLevelControl.Name = "shapeLevelControl";
        //shapeLevelControl.Margin = new Padding(3, 3, 3, 3);
        shapeLevelControl.Dock = DockStyle.Fill;
        shapeLevelControl.Profile = OpenTK.Windowing.Common.ContextProfile.Compatability;
        shapeLevelControl.TabIndex = 0;
        shapeLevelControl.TabStop = false;
        tableLayoutPanel1.Controls.Add(shapeLevelControl, 0, 0);

        // Attach the same event handler to the Click event of all relevant components
        shapeLevelControl.Click += OnComponentClick;
        lblShapeName.Click += OnComponentClick;
        this.Click += OnComponentClick;

        // Attach mouse enter and leave events
        shapeLevelControl.MouseEnter += OnMouseEnter;
        lblShapeName.MouseEnter += OnMouseEnter;
        this.MouseEnter += OnMouseEnter;

        shapeLevelControl.MouseLeave += OnMouseLeave;
        lblShapeName.MouseLeave += OnMouseLeave;
        this.MouseLeave += OnMouseLeave;

        // Attach mouse down and up events
        shapeLevelControl.MouseDown += OnMouseDown;
        lblShapeName.MouseDown += OnMouseDown;
        this.MouseDown += OnMouseDown;

        shapeLevelControl.MouseUp += OnMouseUp;
        lblShapeName.MouseUp += OnMouseUp;
        this.MouseUp += OnMouseUp;

        shapeLevelControl.DoubleClick += CustomShapeControl_DoubleClick;
        lblShapeName.DoubleClick += CustomShapeControl_DoubleClick;
        this.DoubleClick += CustomShapeControl_DoubleClick;
    }

    // Property for the full path of the shape
    public string ShapeFullPath { get; set; } = String.Empty;

    // Property for the shape name
    public string ShapeName
    {
        get => lblShapeName.Text;
        set => lblShapeName.Text = value;
    }

    // Event for clicking the shape
    public event EventHandler? ShapeClicked;

    // Event for loading shape data
    public event EventHandler<ShapeDataDto>? ShapeDataLoaded;

    private void OnComponentClick(object? sender, EventArgs e)
    {
        ShapeClicked?.Invoke(this, e);
        LoadShape();
    }

    public void LoadShape()
    {
        if (ShapeName.Length == 0)
        {
            return;
        }

        var levFilePath = Path.ChangeExtension(ShapeFullPath, ".lev");

        // Check if the JSON file exists
        if (!File.Exists(levFilePath))
        {
            MessageBox.Show($@"Corresponding LEV file not found: {levFilePath}", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (File.Exists(levFilePath))
        {
            var shapeData = CustomShapeSerializer.DeserializeShapeDataLev(levFilePath);
            ShapeDataLoaded?.Invoke(this, shapeData);
        }
    }

    public void Highlight(bool isSelected)
    {
        this._isSelected = isSelected;
        this.BackColor = isSelected ? Color.FromArgb(204, 228, 247) : Color.Transparent;
        _borderColor = isSelected ? Color.FromArgb(0, 84, 153) : Color.Transparent; // Darker blue for selection
        this.Invalidate(); // Trigger a repaint to update the border
    }

    private void OnMouseEnter(object? sender, EventArgs e)
    {
        if (!_isSelected)
        {
            this.BackColor = Color.FromArgb(229, 241, 251);
            _borderColor = Color.FromArgb(0, 120, 215); // Darker blue for highlight
            _isHighlighted = true;
            this.Invalidate(); // Trigger a repaint to update the border
        }
    }

    private void OnMouseLeave(object? sender, EventArgs e)
    {
        if (!_isSelected)
        {
            this.BackColor = Color.Transparent;
            _borderColor = Color.Transparent;
            _isHighlighted = false;
            this.Invalidate(); // Trigger a repaint to update the border
        }
    }
    private void OnMouseDown(object? sender, MouseEventArgs e)
    {
        _isPressed = true;
        this.BackColor = Color.FromArgb(153, 204, 255); // Lighter blue for pressed state
        _borderColor = Color.FromArgb(0, 84, 153); // Darker blue for pressed state
        this.Invalidate(); // Trigger a repaint to update the border
        OnComponentClick(sender, e);
    }
    private void OnMouseUp(object? sender, MouseEventArgs e)
    {
        _isPressed = false;
        this.BackColor = _isSelected ? Color.FromArgb(204, 228, 247) : (_isHighlighted ? Color.FromArgb(229, 241, 251) : Color.Transparent);
        _borderColor = _isSelected ? Color.FromArgb(0, 84, 153) : (_isHighlighted ? Color.FromArgb(0, 120, 215) : Color.Transparent);
        this.Invalidate(); // Trigger a repaint to update the border
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        if (_isSelected || _isHighlighted || _isPressed)
        {
            using (Pen pen = new Pen(_borderColor, 2))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 2, this.Height - 3);
            }
        }
    }

    internal void UpdateContent(string filepath, string shapeName)
    {
        Level level = Level.FromPath(filepath).Obj;
        ShapeFullPath = filepath;
        ShapeName = shapeName;
        SetLevel(level);
    }

    internal void SetLevel(Level? level)
    {
        shapeLevelControl.SetLevel(level);
    }

    internal void DisableLevelRendering(bool disable)
    {
        shapeLevelControl.DisableRendering = disable;
    }

    private void CustomShapeControl_DoubleClick(object? sender, EventArgs e)
    {
        OnShapeDoubleClicked(EventArgs.Empty);
    }

    public event EventHandler? ShapeDoubleClicked;

    protected virtual void OnShapeDoubleClicked(EventArgs e)
    {
        ShapeDoubleClicked?.Invoke(this, e);
    }
}
