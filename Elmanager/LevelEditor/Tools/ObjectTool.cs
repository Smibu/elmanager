using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Elmanager.Geometry;
using Elmanager.Lev;
using Elmanager.Rendering;

namespace Elmanager.LevelEditor.Tools;

internal class ObjectTool : ToolBase, IEditorTool
{
    private ObjectType _currentObjectType = ObjectType.Apple;
    private int _animNum = 1;

    internal ObjectTool(LevelEditorForm editor)
        : base(editor)
    {
    }

    public void Activate()
    {
    }

    public void ExtraRendering()
    {
    }

    public LevVisualChange InActivate() => LevVisualChange.Objects;

    public TransientElements GetTransientElements(bool hasFocus) => hasFocus
        ? TransientElements.FromObjects(new List<LevObject>
            { new(CurrentPos, _currentObjectType, AppleType.Normal) })
        : TransientElements.Empty;

    public LevVisualChange KeyDown(KeyEventArgs key)
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

        return LevVisualChange.Objects;
    }

    public LevVisualChange MouseDown(MouseEventArgs mouseData)
    {
        if (mouseData.Button == MouseButtons.Left)
        {
            Lev.Objects.Add(new LevObject(CurrentPos, _currentObjectType, AppleType.Normal, _animNum));
            var mod = _currentObjectType switch
            {
                ObjectType.Apple => LevModification.Apples,
                ObjectType.Killer => LevModification.Killers,
                ObjectType.Flower => LevModification.Flowers,
                _ => LevModification.Nothing
            };
            LevEditor.SetModified(mod);
        }

        return LevVisualChange.Nothing;
    }

    public LevVisualChange MouseMove(Vector p)
    {
        CurrentPos = p;
        AdjustForGrid(ref CurrentPos);
        return LevVisualChange.Objects;
    }

    public LevVisualChange MouseOutOfEditor()
    {
        return LevVisualChange.Objects;
    }

    public void MouseUp()
    {
    }

    public string GetHelp()
    {
        var objName = _currentObjectType switch
        {
            ObjectType.Apple => "apple",
            ObjectType.Killer => "killer",
            ObjectType.Flower => "flower",
            _ => throw new Exception()
        };
        return $"LMouse: insert new object; Space: change object type ({objName})";
    }

    public override bool Busy => false;
}
