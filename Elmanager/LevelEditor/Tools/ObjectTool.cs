using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Elmanager.Geometry;
using Elmanager.Lev;

namespace Elmanager.LevelEditor.Tools;

internal class ObjectTool : ToolBase, IEditorTool
{
    private ObjectType _currentObjectType = ObjectType.Apple;
    private bool _hasFocus;
    private int _animNum = 1;

    internal ObjectTool(LevelEditorForm editor)
        : base(editor)
    {
    }

    public void Activate()
    {
        UpdateHelp();
    }

    public void ExtraRendering()
    {
    }

    public void InActivate()
    {
    }

    public TransientElements GetTransientElements() => _hasFocus
        ? TransientElements.FromObjects(new List<LevObject>
            { new(CurrentPos, _currentObjectType, AppleType.Normal) })
        : TransientElements.Empty;

    public void KeyDown(KeyEventArgs key)
    {
        switch (key.KeyCode)
        {
            case Keys.Space:
                _currentObjectType = _currentObjectType switch
                {
                    ObjectType.Apple => ObjectType.Killer,
                    ObjectType.Killer => ObjectType.Flower,
                    ObjectType.Flower => ObjectType.Apple,
                    _ => _currentObjectType
                };

                UpdateHelp();
                break;
            case Keys.D1:
                _animNum = 1;
                break;
            case Keys.D2:
                _animNum = 2;
                break;
            case Keys.D3:
                _animNum = 3;
                break;
            case Keys.D4:
                _animNum = 4;
                break;
            case Keys.D5:
                _animNum = 5;
                break;
            case Keys.D6:
                _animNum = 6;
                break;
            case Keys.D7:
                _animNum = 7;
                break;
            case Keys.D8:
                _animNum = 8;
                break;
            case Keys.D9:
                _animNum = 9;
                break;
        }
    }

    public void MouseDown(MouseEventArgs mouseData)
    {
        if (mouseData.Button != MouseButtons.Left) return;
        Lev.Objects.Add(new LevObject(CurrentPos, _currentObjectType, AppleType.Normal, _animNum));
        LevEditor.SetModified(LevModification.Objects);
    }

    public void MouseMove(Vector p)
    {
        CurrentPos = p;
        _hasFocus = true;
        AdjustForGrid(ref CurrentPos);
    }

    public void MouseOutOfEditor()
    {
        _hasFocus = false;
    }

    public void MouseUp()
    {
    }

    public void UpdateHelp()
    {
        var objName = _currentObjectType switch
        {
            ObjectType.Apple => "apple",
            ObjectType.Killer => "killer",
            ObjectType.Flower => "flower",
            _ => throw new Exception()
        };
        LevEditor.InfoLabel.Text = $"LMouse: insert new object; Space: change object type ({objName})";
    }

    public override bool Busy => false;
}