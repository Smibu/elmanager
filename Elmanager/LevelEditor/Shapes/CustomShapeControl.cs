using Elmanager.IO;
using Elmanager.Rendering;
using OpenTK.GLControl;
using System;
using System.Drawing;
using System.Windows.Forms;
using Color = System.Drawing.Color;
using Pen = System.Drawing.Pen;

namespace Elmanager.LevelEditor.Shapes;

internal partial class CustomShapeControl : UserControl
{
    private bool _isHighlighted;
    private bool _isPressed;
    private bool _isSelected;

    private Color _borderColor = Color.Transparent;

    private static readonly Color SelectedBackColor = Color.FromArgb(204, 228, 247);
    private static readonly Color SelectedBorderColor = Color.FromArgb(0, 84, 153);
    private static readonly Color HighlightedBackColor = Color.FromArgb(229, 241, 251);
    private static readonly Color HighlightedBorderColor = Color.FromArgb(0, 120, 215);
    private static readonly Color PressedBackColor = Color.FromArgb(153, 204, 255);
    private static readonly Color TransparentColor = Color.Transparent;

    public CustomShapeControl(GLControl sharedContext, SceneSettings sceneSettings, RenderingSettings renderingSettings, ElmaRenderer elmaRenderer, SleShape shape)
    {
        InitializeComponent();

        shapeLevelControl = new LevelControl(sharedContext, sceneSettings, renderingSettings, elmaRenderer, shape.Level);
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

        shapeLevelControl.Click += OnComponentClick;
        lblShapeName.Click += OnComponentClick;
        Click += OnComponentClick;

        shapeLevelControl.MouseEnter += OnMouseEnter;
        lblShapeName.MouseEnter += OnMouseEnter;
        MouseEnter += OnMouseEnter;

        shapeLevelControl.MouseLeave += OnMouseLeave;
        lblShapeName.MouseLeave += OnMouseLeave;
        MouseLeave += OnMouseLeave;

        shapeLevelControl.MouseDown += OnMouseDown;
        lblShapeName.MouseDown += OnMouseDown;
        MouseDown += OnMouseDown;

        shapeLevelControl.MouseUp += OnMouseUp;
        lblShapeName.MouseUp += OnMouseUp;
        MouseUp += OnMouseUp;

        shapeLevelControl.DoubleClick += CustomShapeControl_DoubleClick;
        lblShapeName.DoubleClick += CustomShapeControl_DoubleClick;
        DoubleClick += CustomShapeControl_DoubleClick;
    }

    public string? ShapeFullPath { get; set; }

    public string ShapeName
    {
        get => lblShapeName.Text;
        set => lblShapeName.Text = value;
    }

    public event EventHandler? ShapeClicked;

    private void OnComponentClick(object? sender, EventArgs e)
    {
        ShapeClicked?.Invoke(this, e);
    }

    public void Highlight(bool isSelected)
    {
        _isSelected = isSelected;
        BackColor = isSelected ? SelectedBackColor : TransparentColor;
        _borderColor = isSelected ? SelectedBorderColor : TransparentColor;
        Invalidate();
    }

    private void OnMouseEnter(object? sender, EventArgs e)
    {
        if (!_isSelected)
        {
            BackColor = HighlightedBackColor;
            _borderColor = HighlightedBorderColor;
            _isHighlighted = true;
            Invalidate();
        }
    }

    private void OnMouseLeave(object? sender, EventArgs e)
    {
        if (!_isSelected)
        {
            BackColor = TransparentColor;
            _borderColor = TransparentColor;
            _isHighlighted = false;
            Invalidate();
        }
    }

    private void OnMouseDown(object? sender, MouseEventArgs e)
    {
        _isPressed = true;
        BackColor = PressedBackColor;
        _borderColor = SelectedBorderColor;
        Invalidate();
        OnComponentClick(sender, e);
    }

    private void OnMouseUp(object? sender, MouseEventArgs e)
    {
        _isPressed = false;
        BackColor = _isSelected ? SelectedBackColor : (_isHighlighted ? HighlightedBackColor : TransparentColor);
        _borderColor = _isSelected ? SelectedBorderColor : (_isHighlighted ? HighlightedBorderColor : TransparentColor);
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        if (_isSelected || _isHighlighted || _isPressed)
        {
            using Pen pen = new Pen(_borderColor, 2);
            e.Graphics.DrawRectangle(pen, 0, 0, Width - 2, Height - 3);
        }
    }

    internal void UpdateContent(string filepath, string shapeName)
    {
        ShapeFullPath = filepath;
        ShapeName = shapeName;
        
        ElmaFileObject<SleShape> shape = SleShape.LoadFromPath(filepath);
        SetShape(shape.Obj);
    }

    internal void SetShape(SleShape shape)
    {
        shapeLevelControl.SetLevel(shape.Level);
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
