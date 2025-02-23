using Elmanager.Lev;
using Elmanager.Rendering;
using OpenTK.GLControl;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Color = System.Drawing.Color;
using Pen = System.Drawing.Pen;
using UserControl = System.Windows.Forms;

namespace Elmanager.LevelEditor.Shapes;

internal partial class CustomShapeControl : UserControl.UserControl
{
    private bool _isHighlighted;
    private bool _isPressed;
    private bool _isSelected;

    private Color _borderColor = Color.Transparent;

    public CustomShapeControl(GLControl sharedContext, SceneSettings sceneSettings, RenderingSettings renderingSettings, ElmaRenderer elmaRenderer, Level? level=null)
    {
        InitializeComponent();

        if (level != null)
        {
            // Filter start object from level
            level.Objects = level.Objects.Where(o => o.Type != ObjectType.Start).ToList();
        }

        shapeLevelControl = new LevelControl(sharedContext, sceneSettings, renderingSettings, elmaRenderer, level);
        shapeLevelControl.API = OpenTK.Windowing.Common.ContextAPI.OpenGL;
        shapeLevelControl.APIVersion = new Version(3, 3, 0, 0);
        shapeLevelControl.Flags = OpenTK.Windowing.Common.ContextFlags.Default;
        shapeLevelControl.Location = new System.Drawing.Point(0, 0);
        shapeLevelControl.Size = new Size(102, 102);
        shapeLevelControl.Margin = new Padding(0);
        shapeLevelControl.Padding = new Padding(0);
        shapeLevelControl.Name = "shapeLevelControl";
        shapeLevelControl.Dock = DockStyle.None;
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

    private void OnComponentClick(object? sender, EventArgs e)
    {
        ShapeClicked?.Invoke(this, e);
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
            using Pen pen = new Pen(_borderColor, 2);
            e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 2, this.Height - 3);
        }
    }

    internal void UpdateContent(string filepath, string shapeName)
    {
        Level level = Level.FromPath(filepath).Obj;
        level.Objects = level.Objects.Where(o => o.Type != ObjectType.Start).ToList();
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

    // Manually resize the shape level control and label
    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);

        if (shapeLevelControl != null)
        {
            // Margins / Padding / Location doesn't scale consistently with DPI scaling
            // so we draw level control based on the size of the parent control without any margins

            shapeLevelControl.Size = new Size(Width, Height - lblShapeName.Height);
            shapeLevelControl.PerformLayout();
        }

        if (lblShapeName != null)
        {
            lblShapeName.Size = new Size(Width, lblShapeName.Height);
        }
    }
}
