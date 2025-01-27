using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Elmanager.IO;
using Elmanager.Properties;
using Elmanager.UI;
using OpenTK.Windowing.Common;
using OpenTK.WinForms;

namespace Elmanager.LevelEditor
{
    internal partial class LevelEditorForm
    {

        //Form overrides dispose to clean up the component list.
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && components != null)
                {
                    components.Dispose();
                }
                Renderer?.Dispose();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        //Required by the Windows Form Designer

        //The following procedure is required by the Windows Form Designer
        //It can be modified using the Windows Form Designer.
        //Do not modify it using the code editor.
        private void InitializeComponent()
        {
            components = new Container();
            MenuStrip1 = new MenuStrip();
            FileToolStripMenuItem = new ToolStripMenuItem();
            NewToolStripMenuItem = new ToolStripMenuItem();
            OpenToolStripMenuItem = new ToolStripMenuItem();
            openInternalToolStripMenuItem = new ToolStripMenuItem();
            openInternalPart1ToolStripMenuItem = new ToolStripMenuItem();
            openInternalPart2ToolStripMenuItem = new ToolStripMenuItem();
            SaveToolStripMenuItem = new ToolStripMenuItem();
            SaveAsToolStripMenuItem = new ToolStripMenuItem();
            saveAsPictureToolStripMenuItem = new ToolStripMenuItem();
            deleteLevMenuItem = new ToolStripMenuItem();
            ExitToolStripMenuItem = new ToolStripMenuItem();
            ActionsMenuItem = new ToolStripMenuItem();
            QuickGrassToolStripMenuItem = new ToolStripMenuItem();
            DeleteAllGrassToolStripMenuItem = new ToolStripMenuItem();
            UndoToolStripMenuItem = new ToolStripMenuItem();
            RedoToolStripMenuItem = new ToolStripMenuItem();
            ToolStripSeparator8 = new ToolStripSeparator();
            ZoomFillToolStripMenuItem = new ToolStripMenuItem();
            CheckTopologyMenuItem = new ToolStripMenuItem();
            LevelPropertiesToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator10 = new ToolStripSeparator();
            previousLevelToolStripMenuItem = new ToolStripMenuItem();
            nextLevelToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator11 = new ToolStripSeparator();
            selectAllToolStripMenuItem = new ToolStripMenuItem();
            importLevelsToolStripMenuItem = new ToolStripMenuItem();
            selectionToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripMenuItem();
            copyAndSnapToGridMenuItem = new ToolStripMenuItem();
            createCustomShapeToolStripMenuItem = new ToolStripMenuItem();
            MirrorHorizontallyToolStripMenuItem = new ToolStripMenuItem();
            MirrorVerticallyToolStripMenuItem = new ToolStripMenuItem();
            DeleteSelectedMenuItem = new ToolStripMenuItemMod();
            toolStripSeparator14 = new ToolStripSeparator();
            unionToolStripMenuItem = new ToolStripMenuItemMod();
            differenceToolStripMenuItem = new ToolStripMenuItemMod();
            intersectionToolStripMenuItem = new ToolStripMenuItemMod();
            symmetricDifferenceToolStripMenuItem = new ToolStripMenuItemMod();
            fixSelfIntersectionsMenuItem = new ToolStripMenuItemMod();
            toolStripSeparator17 = new ToolStripSeparator();
            texturizeMenuItem = new ToolStripMenuItemMod();
            toolStripSeparator15 = new ToolStripSeparator();
            deselectGroundPolygonsToolStripMenuItem = new ToolStripMenuItem();
            deselectGrassPolygonsToolStripMenuItem = new ToolStripMenuItem();
            deselectApplesToolStripMenuItem = new ToolStripMenuItem();
            deselectKillersToolStripMenuItem = new ToolStripMenuItem();
            deselectFlowersToolStripMenuItem = new ToolStripMenuItem();
            deselectPicturesToolStripMenuItem = new ToolStripMenuItem();
            deselectTexturesToolStripMenuItem = new ToolStripMenuItem();
            SelectionFilterToolStripMenuItem = new ToolStripMenuItem();
            EnableAllToolStripMenuItem = new ToolStripMenuItem();
            DisableAllToolStripMenuItem = new ToolStripMenuItem();
            GroundPolygonsToolStripMenuItem = new ToolStripMenuItem();
            GrassPolygonsToolStripMenuItem = new ToolStripMenuItem();
            ApplesToolStripMenuItem = new ToolStripMenuItem();
            KillersToolStripMenuItem = new ToolStripMenuItem();
            FlowersToolStripMenuItem = new ToolStripMenuItem();
            StartToolStripMenuItem = new ToolStripMenuItem();
            PicturesToolStripMenuItem = new ToolStripMenuItem();
            TexturesToolStripMenuItem = new ToolStripMenuItem();
            ConfigurationToolStripMenuItem = new ToolStripMenuItem();
            MainConfigMenuItem = new ToolStripMenuItem();
            RenderingSettingsToolStripMenuItem = new ToolStripMenuItem();
            var settings = new GLControlSettings
            {
                APIVersion = new Version(3, 3),
                Profile = ContextProfile.Core,
                Flags = ContextFlags.ForwardCompatible
            };
            EditorControl = new GLControl(settings);
            OpenFileDialog1 = new OpenFileDialog();
            StatusStrip1 = new StatusStrip();
            zoomLabel = new ToolStripStatusLabel();
            CoordinateLabel = new ToolStripStatusLabel();
            SelectionLabel = new ToolStripStatusLabel();
            HighlightLabel = new ToolStripStatusLabel();
            SaveFileDialog1 = new SaveFileDialog();
            ToolStripPanel1 = new ToolStripPanel();
            ToolStrip1 = new ToolStrip();
            NewButton = new ToolStripButton();
            OpenButton = new ToolStripButton();
            SaveButton = new ToolStripButton();
            SaveAsButton = new ToolStripButton();
            deleteButton = new ToolStripButton();
            ToolStripSeparator1 = new ToolStripSeparator();
            CheckTopologyButton = new ToolStripButton();
            ZoomFillButton = new ToolStripButton();
            ToolStripSeparator2 = new ToolStripSeparator();
            UndoButton = new ToolStripButton();
            RedoButton = new ToolStripButton();
            ToolStripSeparator3 = new ToolStripSeparator();
            PreviousButton = new ToolStripButton();
            NextButton = new ToolStripButton();
            toolStripSeparator13 = new ToolStripSeparator();
            toolStripLabel5 = new ToolStripLabel();
            filenameBox = new ToolStripTextBox();
            filenameOkButton = new ToolStripButton();
            filenameCancelButton = new ToolStripButton();
            toolStripSeparator9 = new ToolStripSeparator();
            ToolStripLabel1 = new ToolStripLabel();
            TitleBox = new ToolStripTextBox();
            ToolStripSeparator4 = new ToolStripSeparator();
            ToolStripLabel2 = new ToolStripLabel();
            LGRBox = new ToolStripComboBox();
            ToolStripSeparator5 = new ToolStripSeparator();
            ToolStripLabel3 = new ToolStripLabel();
            GroundComboBox = new ToolStripComboBox();
            ToolStripSeparator6 = new ToolStripSeparator();
            ToolStripLabel4 = new ToolStripLabel();
            SkyComboBox = new ToolStripComboBox();
            ToolStripSeparator7 = new ToolStripSeparator();
            ToolStrip2 = new ToolStrip();
            ShowGridButton = new ToolStripButton();
            snapToGridButton = new ToolStripButton();
            lockGridButton = new ToolStripButton();
            showCrossHairButton = new ToolStripButton();
            ShowGrassEdgesButton = new ToolStripButton();
            showGrassButton = new ToolStripButton();
            ShowGroundEdgesButton = new ToolStripButton();
            ShowVerticesButton = new ToolStripButton();
            ShowTextureFramesButton = new ToolStripButton();
            ShowPictureFramesButton = new ToolStripButton();
            ShowTexturesButton = new ToolStripButton();
            ShowPicturesButton = new ToolStripButton();
            ShowObjectFramesButton = new ToolStripButton();
            ShowObjectsButton = new ToolStripButton();
            ShowGravityAppleArrowsButton = new ToolStripButton();
            ShowGroundButton = new ToolStripButton();
            ShowGroundTextureButton = new ToolStripButton();
            ShowSkyTextureButton = new ToolStripButton();
            ZoomTexturesButton = new ToolStripButton();
            toolStripSeparator12 = new ToolStripSeparator();
            BestTimeLabel = new ToolStripLabel();
            topologyList = new ToolStripDropDownButton();
            toolStripSeparator16 = new ToolStripSeparator();
            playButton = new ToolStripButton();
            stopButton = new ToolStripButton();
            settingsButton = new ToolStripButton();
            PlayTimeLabel = new ToolStripLabel();
            toolStrip3 = new ToolStrip();
            InfoLabel = new ToolStripLabel();
            EditorMenuStrip = new ContextMenuStrip(components);
            CopyMenuItem = new ToolStripMenuItem();
            TransformMenuItem = new ToolStripMenuItem();
            DeleteMenuItem = new ToolStripMenuItem();
            GrassMenuItem = new ToolStripMenuItem();
            GravityNoneMenuItem = new ToolStripMenuItem();
            GravityUpMenuItem = new ToolStripMenuItem();
            GravityDownMenuItem = new ToolStripMenuItem();
            GravityLeftMenuItem = new ToolStripMenuItem();
            GravityRightMenuItem = new ToolStripMenuItem();
            PicturePropertiesMenuItem = new ToolStripMenuItem();
            bringToFrontToolStripMenuItem = new ToolStripMenuItem();
            sendToBackToolStripMenuItem = new ToolStripMenuItem();
            convertToToolStripMenuItem = new ToolStripMenuItem();
            applesConvertItem = new ToolStripMenuItem();
            killersConvertItem = new ToolStripMenuItem();
            flowersConvertItem = new ToolStripMenuItem();
            picturesConvertItem = new ToolStripMenuItem();
            saveStartPositionToolStripMenuItem = new ToolStripMenuItem();
            restoreStartPositionToolStripMenuItem = new ToolStripMenuItem();
            moveStartHereToolStripMenuItem = new ToolStripMenuItem();
            createCustomShapeMenuItem = new ToolStripMenuItem();
            saveAsPictureDialog = new SaveFileDialog();
            importFileDialog = new OpenFileDialog();
            ToolPanel = new PanelMod();
            CustomShapeButton = new RadioButtonMod();
            TextButton = new RadioButtonMod();
            PictureButton = new RadioButtonMod();
            AutoGrassButton = new RadioButtonMod();
            CutConnectButton = new RadioButtonMod();
            SmoothenButton = new RadioButtonMod();
            FrameButton = new RadioButtonMod();
            PolyOpButton = new RadioButtonMod();
            EllipseButton = new RadioButtonMod();
            PipeButton = new RadioButtonMod();
            ObjectButton = new RadioButtonMod();
            DrawButton = new RadioButtonMod();
            VertexButton = new RadioButtonMod();
            SelectButton = new RadioButtonMod();
            MenuStrip1.SuspendLayout();
            StatusStrip1.SuspendLayout();
            ToolStripPanel1.SuspendLayout();
            ToolStrip1.SuspendLayout();
            ToolStrip2.SuspendLayout();
            toolStrip3.SuspendLayout();
            EditorMenuStrip.SuspendLayout();
            ToolPanel.SuspendLayout();
            SuspendLayout();
            // 
            // MenuStrip1
            // 
            MenuStrip1.BackColor = SystemColors.Control;
            MenuStrip1.ImageScalingSize = new Size(15, 15);
            MenuStrip1.Items.AddRange(new ToolStripItem[] { FileToolStripMenuItem, ActionsMenuItem, selectionToolStripMenuItem, SelectionFilterToolStripMenuItem, ConfigurationToolStripMenuItem });
            MenuStrip1.Location = new Point(0, 0);
            MenuStrip1.Name = "MenuStrip1";
            MenuStrip1.Padding = new Padding(12, 4, 0, 4);
            MenuStrip1.Size = new Size(1858, 44);
            MenuStrip1.TabIndex = 0;
            MenuStrip1.Text = "MenuStrip1";
            // 
            // FileToolStripMenuItem
            // 
            FileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { NewToolStripMenuItem, OpenToolStripMenuItem, openInternalToolStripMenuItem, SaveToolStripMenuItem, SaveAsToolStripMenuItem, saveAsPictureToolStripMenuItem, deleteLevMenuItem, ExitToolStripMenuItem });
            FileToolStripMenuItem.Name = "FileToolStripMenuItem";
            FileToolStripMenuItem.Size = new Size(71, 38);
            FileToolStripMenuItem.Text = "File";
            // 
            // NewToolStripMenuItem
            // 
            NewToolStripMenuItem.Image = Resources._New;
            NewToolStripMenuItem.Name = "NewToolStripMenuItem";
            NewToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.N;
            NewToolStripMenuItem.Size = new Size(402, 44);
            NewToolStripMenuItem.Text = "New";
            NewToolStripMenuItem.Click += NewLevel;
            // 
            // OpenToolStripMenuItem
            // 
            OpenToolStripMenuItem.Image = Resources.Open;
            OpenToolStripMenuItem.Name = "OpenToolStripMenuItem";
            OpenToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.O;
            OpenToolStripMenuItem.Size = new Size(402, 44);
            OpenToolStripMenuItem.Text = "Open";
            OpenToolStripMenuItem.Click += OpenToolStripMenuItemClick;
            // 
            // openInternalToolStripMenuItem
            // 
            openInternalToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openInternalPart1ToolStripMenuItem, openInternalPart2ToolStripMenuItem });
            openInternalToolStripMenuItem.Name = "openInternalToolStripMenuItem";
            openInternalToolStripMenuItem.Size = new Size(402, 44);
            openInternalToolStripMenuItem.Text = "Open internal";
            // 
            // openInternalPart1ToolStripMenuItem
            // 
            openInternalPart1ToolStripMenuItem.Name = "openInternalPart1ToolStripMenuItem";
            openInternalPart1ToolStripMenuItem.Size = new Size(223, 44);
            openInternalPart1ToolStripMenuItem.Text = "1 - 28";
            // 
            // openInternalPart2ToolStripMenuItem
            // 
            openInternalPart2ToolStripMenuItem.Name = "openInternalPart2ToolStripMenuItem";
            openInternalPart2ToolStripMenuItem.Size = new Size(223, 44);
            openInternalPart2ToolStripMenuItem.Text = "29 - 55";
            // 
            // SaveToolStripMenuItem
            // 
            SaveToolStripMenuItem.Enabled = false;
            SaveToolStripMenuItem.Image = Resources.Save;
            SaveToolStripMenuItem.Name = "SaveToolStripMenuItem";
            SaveToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            SaveToolStripMenuItem.Size = new Size(402, 44);
            SaveToolStripMenuItem.Text = "Save";
            SaveToolStripMenuItem.Click += SaveClicked;
            // 
            // SaveAsToolStripMenuItem
            // 
            SaveAsToolStripMenuItem.Image = Resources.SaveAs;
            SaveAsToolStripMenuItem.Name = "SaveAsToolStripMenuItem";
            SaveAsToolStripMenuItem.Size = new Size(402, 44);
            SaveAsToolStripMenuItem.Text = "Save as...";
            SaveAsToolStripMenuItem.Click += SaveAs;
            // 
            // saveAsPictureToolStripMenuItem
            // 
            saveAsPictureToolStripMenuItem.Name = "saveAsPictureToolStripMenuItem";
            saveAsPictureToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.P;
            saveAsPictureToolStripMenuItem.Size = new Size(402, 44);
            saveAsPictureToolStripMenuItem.Text = "Save as picture...";
            saveAsPictureToolStripMenuItem.Click += saveAsPictureToolStripMenuItem_Click;
            // 
            // deleteLevMenuItem
            // 
            deleteLevMenuItem.Image = Resources.Delete;
            deleteLevMenuItem.Name = "deleteLevMenuItem";
            deleteLevMenuItem.Size = new Size(402, 44);
            deleteLevMenuItem.Text = "Delete";
            deleteLevMenuItem.Click += deleteLevMenuItem_Click;
            // 
            // ExitToolStripMenuItem
            // 
            ExitToolStripMenuItem.Image = Resources._Exit;
            ExitToolStripMenuItem.Name = "ExitToolStripMenuItem";
            ExitToolStripMenuItem.Size = new Size(402, 44);
            ExitToolStripMenuItem.Text = "Exit";
            ExitToolStripMenuItem.Click += ExitToolStripMenuItemClick;
            // 
            // ActionsMenuItem
            // 
            ActionsMenuItem.DropDownItems.AddRange(new ToolStripItem[] { QuickGrassToolStripMenuItem, DeleteAllGrassToolStripMenuItem, UndoToolStripMenuItem, RedoToolStripMenuItem, ToolStripSeparator8, ZoomFillToolStripMenuItem, CheckTopologyMenuItem, LevelPropertiesToolStripMenuItem, toolStripSeparator10, previousLevelToolStripMenuItem, nextLevelToolStripMenuItem, toolStripSeparator11, selectAllToolStripMenuItem, importLevelsToolStripMenuItem });
            ActionsMenuItem.Name = "ActionsMenuItem";
            ActionsMenuItem.Size = new Size(89, 38);
            ActionsMenuItem.Text = "Tools";
            // 
            // QuickGrassToolStripMenuItem
            // 
            QuickGrassToolStripMenuItem.Image = Resources.GrassAll;
            QuickGrassToolStripMenuItem.Name = "QuickGrassToolStripMenuItem";
            QuickGrassToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.G;
            QuickGrassToolStripMenuItem.Size = new Size(495, 44);
            QuickGrassToolStripMenuItem.Text = "QuickGrass";
            QuickGrassToolStripMenuItem.Click += QuickGrassToolStripMenuItemClick;
            // 
            // DeleteAllGrassToolStripMenuItem
            // 
            DeleteAllGrassToolStripMenuItem.Image = Resources.GrassDelete;
            DeleteAllGrassToolStripMenuItem.Name = "DeleteAllGrassToolStripMenuItem";
            DeleteAllGrassToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.D;
            DeleteAllGrassToolStripMenuItem.Size = new Size(495, 44);
            DeleteAllGrassToolStripMenuItem.Text = "Delete all grass";
            DeleteAllGrassToolStripMenuItem.Click += DeleteAllGrassToolStripMenuItemClick;
            // 
            // UndoToolStripMenuItem
            // 
            UndoToolStripMenuItem.Image = Resources.Undo;
            UndoToolStripMenuItem.Name = "UndoToolStripMenuItem";
            UndoToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Z;
            UndoToolStripMenuItem.Size = new Size(495, 44);
            UndoToolStripMenuItem.Text = "Undo";
            UndoToolStripMenuItem.Click += Undo;
            // 
            // RedoToolStripMenuItem
            // 
            RedoToolStripMenuItem.Image = Resources.Redo;
            RedoToolStripMenuItem.Name = "RedoToolStripMenuItem";
            RedoToolStripMenuItem.ShortcutKeyDisplayString = "";
            RedoToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Y;
            RedoToolStripMenuItem.Size = new Size(495, 44);
            RedoToolStripMenuItem.Text = "Redo";
            RedoToolStripMenuItem.Click += Redo;
            // 
            // ToolStripSeparator8
            // 
            ToolStripSeparator8.Name = "ToolStripSeparator8";
            ToolStripSeparator8.Size = new Size(492, 6);
            // 
            // ZoomFillToolStripMenuItem
            // 
            ZoomFillToolStripMenuItem.Image = Resources.ZoomFill;
            ZoomFillToolStripMenuItem.Name = "ZoomFillToolStripMenuItem";
            ZoomFillToolStripMenuItem.ShortcutKeys = Keys.F5;
            ZoomFillToolStripMenuItem.Size = new Size(495, 44);
            ZoomFillToolStripMenuItem.Text = "Zoom fill";
            ZoomFillToolStripMenuItem.Click += ZoomFillToolStripMenuItemClick;
            // 
            // CheckTopologyMenuItem
            // 
            CheckTopologyMenuItem.Image = Resources.Topology;
            CheckTopologyMenuItem.Name = "CheckTopologyMenuItem";
            CheckTopologyMenuItem.ShortcutKeys = Keys.F6;
            CheckTopologyMenuItem.Size = new Size(495, 44);
            CheckTopologyMenuItem.Text = "Check topology";
            CheckTopologyMenuItem.Click += CheckTopologyAndUpdate;
            // 
            // LevelPropertiesToolStripMenuItem
            // 
            LevelPropertiesToolStripMenuItem.Name = "LevelPropertiesToolStripMenuItem";
            LevelPropertiesToolStripMenuItem.ShortcutKeys = Keys.F4;
            LevelPropertiesToolStripMenuItem.Size = new Size(495, 44);
            LevelPropertiesToolStripMenuItem.Text = "Level properties";
            LevelPropertiesToolStripMenuItem.Click += LevelPropertiesToolStripMenuItemClick;
            // 
            // toolStripSeparator10
            // 
            toolStripSeparator10.Name = "toolStripSeparator10";
            toolStripSeparator10.Size = new Size(492, 6);
            // 
            // previousLevelToolStripMenuItem
            // 
            previousLevelToolStripMenuItem.Image = Resources.Previous;
            previousLevelToolStripMenuItem.Name = "previousLevelToolStripMenuItem";
            previousLevelToolStripMenuItem.ShortcutKeys = Keys.F2;
            previousLevelToolStripMenuItem.Size = new Size(495, 44);
            previousLevelToolStripMenuItem.Text = "Previous level";
            // 
            // nextLevelToolStripMenuItem
            // 
            nextLevelToolStripMenuItem.Image = Resources.Next;
            nextLevelToolStripMenuItem.Name = "nextLevelToolStripMenuItem";
            nextLevelToolStripMenuItem.ShortcutKeys = Keys.F3;
            nextLevelToolStripMenuItem.Size = new Size(495, 44);
            nextLevelToolStripMenuItem.Text = "Next Level";
            // 
            // toolStripSeparator11
            // 
            toolStripSeparator11.Name = "toolStripSeparator11";
            toolStripSeparator11.Size = new Size(492, 6);
            // 
            // selectAllToolStripMenuItem
            // 
            selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            selectAllToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.A;
            selectAllToolStripMenuItem.Size = new Size(495, 44);
            selectAllToolStripMenuItem.Text = "Select all";
            selectAllToolStripMenuItem.Click += SelectAllToolStripMenuItemClick;
            // 
            // importLevelsToolStripMenuItem
            // 
            importLevelsToolStripMenuItem.Name = "importLevelsToolStripMenuItem";
            importLevelsToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.V;
            importLevelsToolStripMenuItem.Size = new Size(495, 44);
            importLevelsToolStripMenuItem.Text = "Import level(s)/image(s)...";
            importLevelsToolStripMenuItem.Click += importLevelsToolStripMenuItem_Click;
            // 
            // selectionToolStripMenuItem
            // 
            selectionToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItem1, copyAndSnapToGridMenuItem, createCustomShapeToolStripMenuItem, MirrorHorizontallyToolStripMenuItem, MirrorVerticallyToolStripMenuItem, DeleteSelectedMenuItem, toolStripSeparator14, unionToolStripMenuItem, differenceToolStripMenuItem, intersectionToolStripMenuItem, symmetricDifferenceToolStripMenuItem, fixSelfIntersectionsMenuItem, toolStripSeparator17, texturizeMenuItem, toolStripSeparator15, deselectGroundPolygonsToolStripMenuItem, deselectGrassPolygonsToolStripMenuItem, deselectApplesToolStripMenuItem, deselectKillersToolStripMenuItem, deselectFlowersToolStripMenuItem, deselectPicturesToolStripMenuItem, deselectTexturesToolStripMenuItem });
            selectionToolStripMenuItem.Name = "selectionToolStripMenuItem";
            selectionToolStripMenuItem.Size = new Size(132, 36);
            selectionToolStripMenuItem.Text = "Selection";
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.ShortcutKeys = Keys.Control | Keys.C;
            toolStripMenuItem1.Size = new Size(531, 44);
            toolStripMenuItem1.Text = "Copy";
            toolStripMenuItem1.Click += CopyMenuItemClick;
            // 
            // copyAndSnapToGridMenuItem
            // 
            copyAndSnapToGridMenuItem.Name = "copyAndSnapToGridMenuItem";
            copyAndSnapToGridMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.C;
            copyAndSnapToGridMenuItem.Size = new Size(531, 44);
            copyAndSnapToGridMenuItem.Text = "Copy and snap to grid";
            copyAndSnapToGridMenuItem.Click += CopyMenuItemClick;
            // 
            // createCustomShapeToolStripMenuItem
            // 
            createCustomShapeToolStripMenuItem.Name = "createCustomShapeToolStripMenuItem";
            createCustomShapeToolStripMenuItem.Size = new Size(265, 22);
            createCustomShapeToolStripMenuItem.Text = "Create Custom Shape";
            createCustomShapeToolStripMenuItem.Click += createCustomShapeMenuItem_Click;
            // 
            // MirrorHorizontallyToolStripMenuItem
            // 
            MirrorHorizontallyToolStripMenuItem.Image = Resources.Mirror;
            MirrorHorizontallyToolStripMenuItem.Name = "MirrorHorizontallyToolStripMenuItem";
            MirrorHorizontallyToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.M;
            MirrorHorizontallyToolStripMenuItem.Size = new Size(531, 44);
            MirrorHorizontallyToolStripMenuItem.Text = "Mirror horizontally";
            MirrorHorizontallyToolStripMenuItem.Click += MirrorHorizontallyToolStripMenuItem_Click;
            // 
            // MirrorVerticallyToolStripMenuItem
            // 
            MirrorVerticallyToolStripMenuItem.Image = Resources.Mirror;
            MirrorVerticallyToolStripMenuItem.Name = "MirrorVerticallyToolStripMenuItem";
            MirrorVerticallyToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.M;
            MirrorVerticallyToolStripMenuItem.Size = new Size(531, 44);
            MirrorVerticallyToolStripMenuItem.Text = "Mirror vertically";
            MirrorVerticallyToolStripMenuItem.Click += MirrorVerticallyToolStripMenuItem_Click;
            // 
            // DeleteSelectedMenuItem
            // 
            DeleteSelectedMenuItem.Image = Resources.Delete;
            DeleteSelectedMenuItem.Name = "DeleteSelectedMenuItem";
            DeleteSelectedMenuItem.ShortcutText = "Del";
            DeleteSelectedMenuItem.Size = new Size(531, 44);
            DeleteSelectedMenuItem.Text = "Delete";
            DeleteSelectedMenuItem.Click += DeleteSelected;
            // 
            // toolStripSeparator14
            // 
            toolStripSeparator14.Name = "toolStripSeparator14";
            toolStripSeparator14.Size = new Size(528, 6);
            // 
            // unionToolStripMenuItem
            // 
            unionToolStripMenuItem.Name = "unionToolStripMenuItem";
            unionToolStripMenuItem.ShortcutText = ",";
            unionToolStripMenuItem.Size = new Size(531, 44);
            unionToolStripMenuItem.Text = "Union";
            unionToolStripMenuItem.Click += unionToolStripMenuItem_Click;
            // 
            // differenceToolStripMenuItem
            // 
            differenceToolStripMenuItem.Name = "differenceToolStripMenuItem";
            differenceToolStripMenuItem.ShortcutText = ".";
            differenceToolStripMenuItem.Size = new Size(531, 44);
            differenceToolStripMenuItem.Text = "Difference";
            differenceToolStripMenuItem.Click += differenceToolStripMenuItem_Click;
            // 
            // intersectionToolStripMenuItem
            // 
            intersectionToolStripMenuItem.Name = "intersectionToolStripMenuItem";
            intersectionToolStripMenuItem.ShortcutText = "Enter";
            intersectionToolStripMenuItem.Size = new Size(531, 44);
            intersectionToolStripMenuItem.Text = "Intersection";
            intersectionToolStripMenuItem.Click += intersectionToolStripMenuItem_Click;
            // 
            // symmetricDifferenceToolStripMenuItem
            // 
            symmetricDifferenceToolStripMenuItem.Name = "symmetricDifferenceToolStripMenuItem";
            symmetricDifferenceToolStripMenuItem.ShortcutText = "'";
            symmetricDifferenceToolStripMenuItem.Size = new Size(531, 44);
            symmetricDifferenceToolStripMenuItem.Text = "Symmetric difference";
            symmetricDifferenceToolStripMenuItem.Click += symmetricDifferenceToolStripMenuItem_Click;
            // 
            // fixSelfIntersectionsMenuItem
            // 
            fixSelfIntersectionsMenuItem.Name = "fixSelfIntersectionsMenuItem";
            fixSelfIntersectionsMenuItem.ShortcutKeys = Keys.F9;
            fixSelfIntersectionsMenuItem.ShortcutText = "";
            fixSelfIntersectionsMenuItem.Size = new Size(531, 44);
            fixSelfIntersectionsMenuItem.Text = "Fix self-intersections";
            fixSelfIntersectionsMenuItem.Click += fixSelfIntersectionsMenuItem_Click;
            // 
            // toolStripSeparator17
            // 
            toolStripSeparator17.Name = "toolStripSeparator17";
            toolStripSeparator17.Size = new Size(528, 6);
            // 
            // texturizeMenuItem
            // 
            texturizeMenuItem.Name = "texturizeMenuItem";
            texturizeMenuItem.ShortcutText = "§";
            texturizeMenuItem.Size = new Size(531, 44);
            texturizeMenuItem.Text = "Texturize";
            texturizeMenuItem.Click += texturizeMenuItem_Click;
            // 
            // toolStripSeparator15
            // 
            toolStripSeparator15.Name = "toolStripSeparator15";
            toolStripSeparator15.Size = new Size(528, 6);
            // 
            // deselectGroundPolygonsToolStripMenuItem
            // 
            deselectGroundPolygonsToolStripMenuItem.Name = "deselectGroundPolygonsToolStripMenuItem";
            deselectGroundPolygonsToolStripMenuItem.Size = new Size(531, 44);
            deselectGroundPolygonsToolStripMenuItem.Text = "Deselect ground polygons";
            deselectGroundPolygonsToolStripMenuItem.Click += deselectGroundPolygonsToolStripMenuItem_Click;
            // 
            // deselectGrassPolygonsToolStripMenuItem
            // 
            deselectGrassPolygonsToolStripMenuItem.Name = "deselectGrassPolygonsToolStripMenuItem";
            deselectGrassPolygonsToolStripMenuItem.Size = new Size(531, 44);
            deselectGrassPolygonsToolStripMenuItem.Text = "Deselect grass polygons";
            deselectGrassPolygonsToolStripMenuItem.Click += deselectGrassPolygonsToolStripMenuItem_Click;
            // 
            // deselectApplesToolStripMenuItem
            // 
            deselectApplesToolStripMenuItem.Name = "deselectApplesToolStripMenuItem";
            deselectApplesToolStripMenuItem.Size = new Size(531, 44);
            deselectApplesToolStripMenuItem.Text = "Deselect apples";
            deselectApplesToolStripMenuItem.Click += deselectApplesToolStripMenuItem_Click;
            // 
            // deselectKillersToolStripMenuItem
            // 
            deselectKillersToolStripMenuItem.Name = "deselectKillersToolStripMenuItem";
            deselectKillersToolStripMenuItem.Size = new Size(531, 44);
            deselectKillersToolStripMenuItem.Text = "Deselect killers";
            deselectKillersToolStripMenuItem.Click += deselectKillersToolStripMenuItem_Click;
            // 
            // deselectFlowersToolStripMenuItem
            // 
            deselectFlowersToolStripMenuItem.Name = "deselectFlowersToolStripMenuItem";
            deselectFlowersToolStripMenuItem.Size = new Size(531, 44);
            deselectFlowersToolStripMenuItem.Text = "Deselect flowers";
            deselectFlowersToolStripMenuItem.Click += deselectFlowersToolStripMenuItem_Click;
            // 
            // deselectPicturesToolStripMenuItem
            // 
            deselectPicturesToolStripMenuItem.Name = "deselectPicturesToolStripMenuItem";
            deselectPicturesToolStripMenuItem.Size = new Size(531, 44);
            deselectPicturesToolStripMenuItem.Text = "Deselect pictures";
            deselectPicturesToolStripMenuItem.Click += deselectPicturesToolStripMenuItem_Click;
            // 
            // deselectTexturesToolStripMenuItem
            // 
            deselectTexturesToolStripMenuItem.Name = "deselectTexturesToolStripMenuItem";
            deselectTexturesToolStripMenuItem.Size = new Size(531, 44);
            deselectTexturesToolStripMenuItem.Text = "Deselect textures";
            deselectTexturesToolStripMenuItem.Click += deselectTexturesToolStripMenuItem_Click;
            // 
            // SelectionFilterToolStripMenuItem
            // 
            SelectionFilterToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { EnableAllToolStripMenuItem, DisableAllToolStripMenuItem, GroundPolygonsToolStripMenuItem, GrassPolygonsToolStripMenuItem, ApplesToolStripMenuItem, KillersToolStripMenuItem, FlowersToolStripMenuItem, StartToolStripMenuItem, PicturesToolStripMenuItem, TexturesToolStripMenuItem });
            SelectionFilterToolStripMenuItem.Name = "SelectionFilterToolStripMenuItem";
            SelectionFilterToolStripMenuItem.Size = new Size(188, 38);
            SelectionFilterToolStripMenuItem.Text = "Selection filter";
            // 
            // EnableAllToolStripMenuItem
            // 
            EnableAllToolStripMenuItem.Name = "EnableAllToolStripMenuItem";
            EnableAllToolStripMenuItem.Size = new Size(332, 44);
            EnableAllToolStripMenuItem.Text = "Enable all";
            EnableAllToolStripMenuItem.Click += SetAllFilters;
            // 
            // DisableAllToolStripMenuItem
            // 
            DisableAllToolStripMenuItem.Name = "DisableAllToolStripMenuItem";
            DisableAllToolStripMenuItem.Size = new Size(332, 44);
            DisableAllToolStripMenuItem.Text = "Disable all";
            DisableAllToolStripMenuItem.Click += SetAllFilters;
            // 
            // GroundPolygonsToolStripMenuItem
            // 
            GroundPolygonsToolStripMenuItem.Checked = true;
            GroundPolygonsToolStripMenuItem.CheckOnClick = true;
            GroundPolygonsToolStripMenuItem.CheckState = CheckState.Checked;
            GroundPolygonsToolStripMenuItem.Name = "GroundPolygonsToolStripMenuItem";
            GroundPolygonsToolStripMenuItem.Size = new Size(332, 44);
            GroundPolygonsToolStripMenuItem.Text = "Ground polygons";
            GroundPolygonsToolStripMenuItem.CheckedChanged += FilterChanged;
            // 
            // GrassPolygonsToolStripMenuItem
            // 
            GrassPolygonsToolStripMenuItem.Checked = true;
            GrassPolygonsToolStripMenuItem.CheckOnClick = true;
            GrassPolygonsToolStripMenuItem.CheckState = CheckState.Checked;
            GrassPolygonsToolStripMenuItem.Name = "GrassPolygonsToolStripMenuItem";
            GrassPolygonsToolStripMenuItem.Size = new Size(332, 44);
            GrassPolygonsToolStripMenuItem.Text = "Grass polygons";
            GrassPolygonsToolStripMenuItem.CheckedChanged += FilterChanged;
            // 
            // ApplesToolStripMenuItem
            // 
            ApplesToolStripMenuItem.Checked = true;
            ApplesToolStripMenuItem.CheckOnClick = true;
            ApplesToolStripMenuItem.CheckState = CheckState.Checked;
            ApplesToolStripMenuItem.Name = "ApplesToolStripMenuItem";
            ApplesToolStripMenuItem.Size = new Size(332, 44);
            ApplesToolStripMenuItem.Text = "Apples";
            ApplesToolStripMenuItem.CheckedChanged += FilterChanged;
            // 
            // KillersToolStripMenuItem
            // 
            KillersToolStripMenuItem.Checked = true;
            KillersToolStripMenuItem.CheckOnClick = true;
            KillersToolStripMenuItem.CheckState = CheckState.Checked;
            KillersToolStripMenuItem.Name = "KillersToolStripMenuItem";
            KillersToolStripMenuItem.Size = new Size(332, 44);
            KillersToolStripMenuItem.Text = "Killers";
            KillersToolStripMenuItem.CheckedChanged += FilterChanged;
            // 
            // FlowersToolStripMenuItem
            // 
            FlowersToolStripMenuItem.Checked = true;
            FlowersToolStripMenuItem.CheckOnClick = true;
            FlowersToolStripMenuItem.CheckState = CheckState.Checked;
            FlowersToolStripMenuItem.Name = "FlowersToolStripMenuItem";
            FlowersToolStripMenuItem.Size = new Size(332, 44);
            FlowersToolStripMenuItem.Text = "Flowers";
            FlowersToolStripMenuItem.CheckedChanged += FilterChanged;
            // 
            // StartToolStripMenuItem
            // 
            StartToolStripMenuItem.Checked = true;
            StartToolStripMenuItem.CheckOnClick = true;
            StartToolStripMenuItem.CheckState = CheckState.Checked;
            StartToolStripMenuItem.Name = "StartToolStripMenuItem";
            StartToolStripMenuItem.Size = new Size(332, 44);
            StartToolStripMenuItem.Text = "Start";
            StartToolStripMenuItem.CheckedChanged += FilterChanged;
            // 
            // PicturesToolStripMenuItem
            // 
            PicturesToolStripMenuItem.Checked = true;
            PicturesToolStripMenuItem.CheckOnClick = true;
            PicturesToolStripMenuItem.CheckState = CheckState.Checked;
            PicturesToolStripMenuItem.Name = "PicturesToolStripMenuItem";
            PicturesToolStripMenuItem.Size = new Size(332, 44);
            PicturesToolStripMenuItem.Text = "Pictures";
            PicturesToolStripMenuItem.CheckedChanged += FilterChanged;
            // 
            // TexturesToolStripMenuItem
            // 
            TexturesToolStripMenuItem.Checked = true;
            TexturesToolStripMenuItem.CheckOnClick = true;
            TexturesToolStripMenuItem.CheckState = CheckState.Checked;
            TexturesToolStripMenuItem.Name = "TexturesToolStripMenuItem";
            TexturesToolStripMenuItem.Size = new Size(332, 44);
            TexturesToolStripMenuItem.Text = "Textures";
            TexturesToolStripMenuItem.CheckedChanged += FilterChanged;
            // 
            // ConfigurationToolStripMenuItem
            // 
            ConfigurationToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { MainConfigMenuItem, RenderingSettingsToolStripMenuItem });
            ConfigurationToolStripMenuItem.Name = "ConfigurationToolStripMenuItem";
            ConfigurationToolStripMenuItem.Size = new Size(181, 38);
            ConfigurationToolStripMenuItem.Text = "Configuration";
            // 
            // MainConfigMenuItem
            // 
            MainConfigMenuItem.Name = "MainConfigMenuItem";
            MainConfigMenuItem.ShortcutKeys = Keys.F7;
            MainConfigMenuItem.Size = new Size(385, 44);
            MainConfigMenuItem.Text = "Main";
            MainConfigMenuItem.Click += OpenConfig;
            // 
            // RenderingSettingsToolStripMenuItem
            // 
            RenderingSettingsToolStripMenuItem.Name = "RenderingSettingsToolStripMenuItem";
            RenderingSettingsToolStripMenuItem.ShortcutKeys = Keys.F8;
            RenderingSettingsToolStripMenuItem.Size = new Size(385, 44);
            RenderingSettingsToolStripMenuItem.Text = "Rendering settings";
            RenderingSettingsToolStripMenuItem.Click += OpenRenderingSettings;
            // 
            // EditorControl
            // 
            EditorControl.AllowDrop = true;
            EditorControl.API = OpenTK.Windowing.Common.ContextAPI.OpenGL;
            EditorControl.APIVersion = new Version(3, 3, 0, 0);
            EditorControl.Dock = DockStyle.Fill;
            EditorControl.Flags = OpenTK.Windowing.Common.ContextFlags.Default;
            EditorControl.IsEventDriven = true;
            EditorControl.Location = new Point(168, 175);
            EditorControl.Margin = new Padding(6);
            EditorControl.Name = "EditorControl";
            EditorControl.Profile = OpenTK.Windowing.Common.ContextProfile.Compatability;
            EditorControl.Size = new Size(1690, 797);
            EditorControl.TabIndex = 2;
            EditorControl.DragDrop += ItemsDropped;
            EditorControl.DragEnter += StartingDrop;
            EditorControl.DragOver += EditorControl_DragOver;
            EditorControl.DragLeave += EditorControl_DragLeave;
            EditorControl.MouseDown += MouseDownEvent;
            EditorControl.MouseLeave += MouseLeaveEvent;
            EditorControl.MouseMove += MouseMoveEvent;
            EditorControl.MouseUp += MouseUpEvent;
            // 
            // OpenFileDialog1
            // 
            OpenFileDialog1.Filter = "Elasto Mania level (*.lev, *.leb)|*.lev;*.leb";
            // 
            // StatusStrip1
            // 
            StatusStrip1.AutoSize = false;
            StatusStrip1.GripMargin = new Padding(0);
            StatusStrip1.ImageScalingSize = new Size(32, 32);
            StatusStrip1.Items.AddRange(new ToolStripItem[] { zoomLabel, CoordinateLabel, SelectionLabel, HighlightLabel });
            StatusStrip1.Location = new Point(0, 972);
            StatusStrip1.Name = "StatusStrip1";
            StatusStrip1.Padding = new Padding(2, 0, 28, 0);
            StatusStrip1.ShowItemToolTips = true;
            StatusStrip1.Size = new Size(1858, 46);
            StatusStrip1.SizingGrip = false;
            StatusStrip1.TabIndex = 4;
            StatusStrip1.Text = "StatusStrip1";
            // 
            // zoomLabel
            // 
            zoomLabel.AutoSize = false;
            zoomLabel.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Top | ToolStripStatusLabelBorderSides.Right | ToolStripStatusLabelBorderSides.Bottom;
            zoomLabel.DisplayStyle = ToolStripItemDisplayStyle.Text;
            zoomLabel.Name = "zoomLabel";
            zoomLabel.Size = new Size(190, 36);
            zoomLabel.Text = "Zoom:";
            zoomLabel.TextAlign = ContentAlignment.MiddleLeft;
            zoomLabel.Click += zoomLabel_Click;
            // 
            // CoordinateLabel
            // 
            CoordinateLabel.AutoSize = false;
            CoordinateLabel.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Top | ToolStripStatusLabelBorderSides.Right | ToolStripStatusLabelBorderSides.Bottom;
            CoordinateLabel.DisplayStyle = ToolStripItemDisplayStyle.Text;
            CoordinateLabel.Name = "CoordinateLabel";
            CoordinateLabel.Size = new Size(190, 36);
            CoordinateLabel.Text = "Mouse X: Y:";
            CoordinateLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // SelectionLabel
            // 
            SelectionLabel.AutoSize = false;
            SelectionLabel.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Top | ToolStripStatusLabelBorderSides.Right | ToolStripStatusLabelBorderSides.Bottom;
            SelectionLabel.DisplayStyle = ToolStripItemDisplayStyle.Text;
            SelectionLabel.Name = "SelectionLabel";
            SelectionLabel.Size = new Size(470, 36);
            SelectionLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // HighlightLabel
            // 
            HighlightLabel.AutoSize = false;
            HighlightLabel.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Top | ToolStripStatusLabelBorderSides.Right | ToolStripStatusLabelBorderSides.Bottom;
            HighlightLabel.DisplayStyle = ToolStripItemDisplayStyle.Text;
            HighlightLabel.Name = "HighlightLabel";
            HighlightLabel.Size = new Size(978, 36);
            HighlightLabel.Spring = true;
            HighlightLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // SaveFileDialog1
            // 
            SaveFileDialog1.DefaultExt = "lev";
            SaveFileDialog1.FileName = "Untitled";
            SaveFileDialog1.Filter = "Elasto Mania level (*.lev)|*.lev";
            // 
            // ToolStripPanel1
            // 
            ToolStripPanel1.BackColor = SystemColors.Control;
            ToolStripPanel1.Controls.Add(ToolStrip1);
            ToolStripPanel1.Controls.Add(ToolStrip2);
            ToolStripPanel1.Controls.Add(toolStrip3);
            ToolStripPanel1.Dock = DockStyle.Top;
            ToolStripPanel1.ForeColor = SystemColors.ControlText;
            ToolStripPanel1.Location = new Point(0, 44);
            ToolStripPanel1.Name = "ToolStripPanel1";
            ToolStripPanel1.Orientation = Orientation.Horizontal;
            ToolStripPanel1.RowMargin = new Padding(0, 3, 0, 0);
            ToolStripPanel1.Size = new Size(1858, 131);
            // 
            // ToolStrip1
            // 
            ToolStrip1.BackColor = SystemColors.Control;
            ToolStrip1.Dock = DockStyle.None;
            ToolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            ToolStrip1.ImageScalingSize = new Size(32, 32);
            ToolStrip1.Items.AddRange(new ToolStripItem[] { NewButton, OpenButton, SaveButton, SaveAsButton, deleteButton, ToolStripSeparator1, CheckTopologyButton, ZoomFillButton, ToolStripSeparator2, UndoButton, RedoButton, ToolStripSeparator3, PreviousButton, NextButton, toolStripSeparator13, toolStripLabel5, filenameBox, filenameOkButton, filenameCancelButton, toolStripSeparator9, ToolStripLabel1, TitleBox, ToolStripSeparator4, ToolStripLabel2, LGRBox, ToolStripSeparator5, ToolStripLabel3, GroundComboBox, ToolStripSeparator6, ToolStripLabel4, SkyComboBox, ToolStripSeparator7 });
            ToolStrip1.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            ToolStrip1.Location = new Point(0, 3);
            ToolStrip1.Name = "ToolStrip1";
            ToolStrip1.Size = new Size(1681, 42);
            ToolStrip1.TabIndex = 14;
            ToolStrip1.Text = "ToolStrip1";
            // 
            // NewButton
            // 
            NewButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            NewButton.Image = Resources._New;
            NewButton.ImageTransparentColor = Color.Magenta;
            NewButton.Name = "NewButton";
            NewButton.Size = new Size(46, 36);
            NewButton.Text = "New";
            NewButton.Click += NewLevel;
            // 
            // OpenButton
            // 
            OpenButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            OpenButton.Image = Resources.Open;
            OpenButton.ImageTransparentColor = Color.Magenta;
            OpenButton.Name = "OpenButton";
            OpenButton.Size = new Size(46, 36);
            OpenButton.Text = "Open";
            OpenButton.Click += OpenToolStripMenuItemClick;
            // 
            // SaveButton
            // 
            SaveButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            SaveButton.Enabled = false;
            SaveButton.Image = Resources.Save;
            SaveButton.ImageTransparentColor = Color.Magenta;
            SaveButton.Name = "SaveButton";
            SaveButton.Size = new Size(46, 36);
            SaveButton.Text = "Save";
            SaveButton.Click += SaveClicked;
            // 
            // SaveAsButton
            // 
            SaveAsButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            SaveAsButton.Image = Resources.SaveAs;
            SaveAsButton.ImageTransparentColor = Color.Magenta;
            SaveAsButton.Name = "SaveAsButton";
            SaveAsButton.Size = new Size(46, 36);
            SaveAsButton.Text = "Save as...";
            SaveAsButton.Click += SaveAs;
            // 
            // deleteButton
            // 
            deleteButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            deleteButton.Image = Resources.Delete;
            deleteButton.ImageTransparentColor = Color.Magenta;
            deleteButton.Name = "deleteButton";
            deleteButton.Size = new Size(46, 36);
            deleteButton.Text = "toolStripButton1";
            deleteButton.ToolTipText = "Delete this level";
            deleteButton.Click += deleteButton_Click;
            // 
            // ToolStripSeparator1
            // 
            ToolStripSeparator1.Name = "ToolStripSeparator1";
            ToolStripSeparator1.Size = new Size(6, 42);
            // 
            // CheckTopologyButton
            // 
            CheckTopologyButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            CheckTopologyButton.Image = Resources.Topology;
            CheckTopologyButton.ImageTransparentColor = Color.Magenta;
            CheckTopologyButton.Name = "CheckTopologyButton";
            CheckTopologyButton.Size = new Size(46, 36);
            CheckTopologyButton.Text = "Check topology";
            CheckTopologyButton.Click += CheckTopologyAndUpdate;
            // 
            // ZoomFillButton
            // 
            ZoomFillButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ZoomFillButton.Image = Resources.ZoomFill;
            ZoomFillButton.ImageTransparentColor = Color.Magenta;
            ZoomFillButton.Name = "ZoomFillButton";
            ZoomFillButton.Size = new Size(46, 36);
            ZoomFillButton.Text = "Zoom fill";
            // 
            // ToolStripSeparator2
            // 
            ToolStripSeparator2.Name = "ToolStripSeparator2";
            ToolStripSeparator2.Size = new Size(6, 42);
            // 
            // UndoButton
            // 
            UndoButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            UndoButton.Image = Resources.Undo;
            UndoButton.ImageTransparentColor = Color.Magenta;
            UndoButton.Name = "UndoButton";
            UndoButton.Size = new Size(46, 36);
            UndoButton.Text = "Undo";
            UndoButton.Click += Undo;
            // 
            // RedoButton
            // 
            RedoButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            RedoButton.Image = Resources.Redo;
            RedoButton.ImageTransparentColor = Color.Magenta;
            RedoButton.Name = "RedoButton";
            RedoButton.Size = new Size(46, 36);
            RedoButton.Text = "Redo";
            RedoButton.Click += Redo;
            // 
            // ToolStripSeparator3
            // 
            ToolStripSeparator3.Name = "ToolStripSeparator3";
            ToolStripSeparator3.Size = new Size(6, 42);
            // 
            // PreviousButton
            // 
            PreviousButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            PreviousButton.Image = Resources.Previous;
            PreviousButton.ImageTransparentColor = Color.Magenta;
            PreviousButton.Name = "PreviousButton";
            PreviousButton.Size = new Size(46, 36);
            PreviousButton.Text = "Previous level";
            PreviousButton.Click += PrevNextButtonClick;
            // 
            // NextButton
            // 
            NextButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            NextButton.Image = Resources.Next;
            NextButton.ImageTransparentColor = Color.Magenta;
            NextButton.Name = "NextButton";
            NextButton.Size = new Size(46, 36);
            NextButton.Text = "Next level";
            NextButton.Click += PrevNextButtonClick;
            // 
            // toolStripSeparator13
            // 
            toolStripSeparator13.Name = "toolStripSeparator13";
            toolStripSeparator13.Size = new Size(6, 42);
            // 
            // toolStripLabel5
            // 
            toolStripLabel5.Name = "toolStripLabel5";
            toolStripLabel5.Size = new Size(116, 36);
            toolStripLabel5.Text = "Filename:";
            // 
            // filenameBox
            // 
            filenameBox.AutoSize = false;
            filenameBox.BorderStyle = BorderStyle.FixedSingle;
            filenameBox.Name = "filenameBox";
            filenameBox.Size = new Size(100, 39);
            filenameBox.KeyDown += filenameBox_KeyDown;
            filenameBox.TextChanged += filenameBox_TextChanged;
            // 
            // filenameOkButton
            // 
            filenameOkButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            filenameOkButton.ImageTransparentColor = Color.Magenta;
            filenameOkButton.Name = "filenameOkButton";
            filenameOkButton.Size = new Size(50, 36);
            filenameOkButton.Text = "OK";
            filenameOkButton.Visible = false;
            filenameOkButton.Click += filenameOkButton_Click;
            // 
            // filenameCancelButton
            // 
            filenameCancelButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            filenameCancelButton.ImageTransparentColor = Color.Magenta;
            filenameCancelButton.Name = "filenameCancelButton";
            filenameCancelButton.Size = new Size(89, 36);
            filenameCancelButton.Text = "Cancel";
            filenameCancelButton.Visible = false;
            filenameCancelButton.Click += filenameCancelButton_Click;
            // 
            // toolStripSeparator9
            // 
            toolStripSeparator9.Name = "toolStripSeparator9";
            toolStripSeparator9.Size = new Size(6, 42);
            // 
            // ToolStripLabel1
            // 
            ToolStripLabel1.Name = "ToolStripLabel1";
            ToolStripLabel1.Size = new Size(65, 36);
            ToolStripLabel1.Text = "Title:";
            // 
            // TitleBox
            // 
            TitleBox.AutoSize = false;
            TitleBox.BorderStyle = BorderStyle.FixedSingle;
            TitleBox.MaxLength = 50;
            TitleBox.Name = "TitleBox";
            TitleBox.Size = new Size(120, 39);
            TitleBox.TextChanged += TitleBoxTextChanged;
            // 
            // ToolStripSeparator4
            // 
            ToolStripSeparator4.Name = "ToolStripSeparator4";
            ToolStripSeparator4.Size = new Size(6, 42);
            // 
            // ToolStripLabel2
            // 
            ToolStripLabel2.Name = "ToolStripLabel2";
            ToolStripLabel2.Size = new Size(103, 36);
            ToolStripLabel2.Text = "LGR File:";
            // 
            // LGRBox
            // 
            LGRBox.AutoSize = false;
            LGRBox.DropDownStyle = ComboBoxStyle.DropDownList;
            LGRBox.Name = "LGRBox";
            LGRBox.Size = new Size(200, 40);
            // 
            // ToolStripSeparator5
            // 
            ToolStripSeparator5.Name = "ToolStripSeparator5";
            ToolStripSeparator5.Size = new Size(6, 42);
            // 
            // ToolStripLabel3
            // 
            ToolStripLabel3.Name = "ToolStripLabel3";
            ToolStripLabel3.Size = new Size(99, 36);
            ToolStripLabel3.Text = "Ground:";
            // 
            // GroundComboBox
            // 
            GroundComboBox.AutoSize = false;
            GroundComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            GroundComboBox.Name = "GroundComboBox";
            GroundComboBox.Size = new Size(119, 40);
            GroundComboBox.DropDownClosed += MoveFocus;
            GroundComboBox.KeyDown += KeyHandlerDown;
            // 
            // ToolStripSeparator6
            // 
            ToolStripSeparator6.Name = "ToolStripSeparator6";
            ToolStripSeparator6.Size = new Size(6, 42);
            // 
            // ToolStripLabel4
            // 
            ToolStripLabel4.Name = "ToolStripLabel4";
            ToolStripLabel4.Size = new Size(56, 36);
            ToolStripLabel4.Text = "Sky:";
            // 
            // SkyComboBox
            // 
            SkyComboBox.AutoSize = false;
            SkyComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            SkyComboBox.Name = "SkyComboBox";
            SkyComboBox.Size = new Size(119, 40);
            SkyComboBox.DropDownClosed += MoveFocus;
            SkyComboBox.KeyDown += KeyHandlerDown;
            // 
            // ToolStripSeparator7
            // 
            ToolStripSeparator7.Name = "ToolStripSeparator7";
            ToolStripSeparator7.Size = new Size(6, 42);
            // 
            // ToolStrip2
            // 
            ToolStrip2.BackColor = SystemColors.Control;
            ToolStrip2.Dock = DockStyle.None;
            ToolStrip2.GripStyle = ToolStripGripStyle.Hidden;
            ToolStrip2.ImageScalingSize = new Size(32, 32);
            ToolStrip2.Items.AddRange(new ToolStripItem[] { ShowGridButton, snapToGridButton, lockGridButton, showCrossHairButton, ShowGrassEdgesButton, showGrassButton, ShowGroundEdgesButton, ShowVerticesButton, ShowTextureFramesButton, ShowPictureFramesButton, ShowTexturesButton, ShowPicturesButton, ShowObjectFramesButton, ShowObjectsButton, ShowGravityAppleArrowsButton, ShowGroundButton, ShowGroundTextureButton, ShowSkyTextureButton, ZoomTexturesButton, toolStripSeparator12, BestTimeLabel, topologyList, toolStripSeparator16, playButton, stopButton, settingsButton, PlayTimeLabel });
            ToolStrip2.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            ToolStrip2.Location = new Point(0, 48);
            ToolStrip2.Name = "ToolStrip2";
            ToolStrip2.Size = new Size(1372, 42);
            ToolStrip2.TabIndex = 15;
            ToolStrip2.Text = "ToolStrip2";
            // 
            // ShowGridButton
            // 
            ShowGridButton.CheckOnClick = true;
            ShowGridButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ShowGridButton.Image = Resources.Grid;
            ShowGridButton.ImageTransparentColor = Color.Magenta;
            ShowGridButton.Name = "ShowGridButton";
            ShowGridButton.Size = new Size(46, 36);
            ShowGridButton.Text = "S&how grid";
            // 
            // snapToGridButton
            // 
            snapToGridButton.CheckOnClick = true;
            snapToGridButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            snapToGridButton.Image = Resources.Snap;
            snapToGridButton.ImageTransparentColor = Color.Magenta;
            snapToGridButton.Name = "snapToGridButton";
            snapToGridButton.Size = new Size(46, 36);
            snapToGridButton.Text = "Snap to grid";
            // 
            // lockGridButton
            // 
            lockGridButton.CheckOnClick = true;
            lockGridButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            lockGridButton.Image = Resources.LockGrid;
            lockGridButton.ImageTransparentColor = Color.Magenta;
            lockGridButton.Name = "lockGridButton";
            lockGridButton.Size = new Size(46, 36);
            lockGridButton.Text = "Lock grid";
            // 
            // showCrossHairButton
            // 
            showCrossHairButton.CheckOnClick = true;
            showCrossHairButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            showCrossHairButton.Image = Resources.Crosshair2;
            showCrossHairButton.ImageTransparentColor = Color.Magenta;
            showCrossHairButton.Name = "showCrossHairButton";
            showCrossHairButton.Size = new Size(46, 36);
            showCrossHairButton.Text = "Show crosshair";
            // 
            // ShowGrassEdgesButton
            // 
            ShowGrassEdgesButton.CheckOnClick = true;
            ShowGrassEdgesButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ShowGrassEdgesButton.Image = Resources.GrassEdges;
            ShowGrassEdgesButton.ImageTransparentColor = Color.Magenta;
            ShowGrassEdgesButton.Name = "ShowGrassEdgesButton";
            ShowGrassEdgesButton.Size = new Size(46, 36);
            ShowGrassEdgesButton.Text = "Show grass edges";
            // 
            // showGrassButton
            // 
            showGrassButton.CheckOnClick = true;
            showGrassButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            showGrassButton.Image = Resources.Grass;
            showGrassButton.ImageTransparentColor = Color.Magenta;
            showGrassButton.Name = "showGrassButton";
            showGrassButton.Size = new Size(46, 36);
            showGrassButton.Text = "Show grass";
            // 
            // ShowGroundEdgesButton
            // 
            ShowGroundEdgesButton.CheckOnClick = true;
            ShowGroundEdgesButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ShowGroundEdgesButton.Image = Resources.Edges;
            ShowGroundEdgesButton.ImageTransparentColor = Color.Magenta;
            ShowGroundEdgesButton.Name = "ShowGroundEdgesButton";
            ShowGroundEdgesButton.Size = new Size(46, 36);
            ShowGroundEdgesButton.Text = "Show ground edges";
            // 
            // ShowVerticesButton
            // 
            ShowVerticesButton.CheckOnClick = true;
            ShowVerticesButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ShowVerticesButton.Image = Resources.Vertices;
            ShowVerticesButton.ImageTransparentColor = Color.Magenta;
            ShowVerticesButton.Name = "ShowVerticesButton";
            ShowVerticesButton.Size = new Size(46, 36);
            ShowVerticesButton.Text = "Show vertices";
            // 
            // ShowTextureFramesButton
            // 
            ShowTextureFramesButton.CheckOnClick = true;
            ShowTextureFramesButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ShowTextureFramesButton.Image = Resources.TextureFrame;
            ShowTextureFramesButton.ImageTransparentColor = Color.Magenta;
            ShowTextureFramesButton.Name = "ShowTextureFramesButton";
            ShowTextureFramesButton.Size = new Size(46, 36);
            ShowTextureFramesButton.Text = "Show texture frames";
            // 
            // ShowPictureFramesButton
            // 
            ShowPictureFramesButton.CheckOnClick = true;
            ShowPictureFramesButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ShowPictureFramesButton.Image = Resources.PictureFrame;
            ShowPictureFramesButton.ImageTransparentColor = Color.Magenta;
            ShowPictureFramesButton.Name = "ShowPictureFramesButton";
            ShowPictureFramesButton.Size = new Size(46, 36);
            ShowPictureFramesButton.Text = "Show picture frames";
            // 
            // ShowTexturesButton
            // 
            ShowTexturesButton.CheckOnClick = true;
            ShowTexturesButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ShowTexturesButton.Image = Resources.Texture;
            ShowTexturesButton.ImageTransparentColor = Color.Magenta;
            ShowTexturesButton.Name = "ShowTexturesButton";
            ShowTexturesButton.Size = new Size(46, 36);
            ShowTexturesButton.Text = "Show textures";
            // 
            // ShowPicturesButton
            // 
            ShowPicturesButton.CheckOnClick = true;
            ShowPicturesButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ShowPicturesButton.Image = Resources.Picture;
            ShowPicturesButton.ImageTransparentColor = Color.Magenta;
            ShowPicturesButton.Name = "ShowPicturesButton";
            ShowPicturesButton.Size = new Size(46, 36);
            ShowPicturesButton.Text = "Show pictures";
            // 
            // ShowObjectFramesButton
            // 
            ShowObjectFramesButton.CheckOnClick = true;
            ShowObjectFramesButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ShowObjectFramesButton.Image = Resources.ObjectFrame;
            ShowObjectFramesButton.ImageTransparentColor = Color.Magenta;
            ShowObjectFramesButton.Name = "ShowObjectFramesButton";
            ShowObjectFramesButton.Size = new Size(46, 36);
            ShowObjectFramesButton.Text = "Show object frames";
            // 
            // ShowObjectsButton
            // 
            ShowObjectsButton.CheckOnClick = true;
            ShowObjectsButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ShowObjectsButton.Image = Resources._Object;
            ShowObjectsButton.ImageTransparentColor = Color.Magenta;
            ShowObjectsButton.Name = "ShowObjectsButton";
            ShowObjectsButton.Size = new Size(46, 36);
            ShowObjectsButton.Text = "Show objects";
            // 
            // ShowGravityAppleArrowsButton
            // 
            ShowGravityAppleArrowsButton.CheckOnClick = true;
            ShowGravityAppleArrowsButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ShowGravityAppleArrowsButton.Image = Resources.AppleArrow;
            ShowGravityAppleArrowsButton.ImageTransparentColor = Color.Magenta;
            ShowGravityAppleArrowsButton.Name = "ShowGravityAppleArrowsButton";
            ShowGravityAppleArrowsButton.Size = new Size(46, 36);
            ShowGravityAppleArrowsButton.Text = "Show gravity apple arrows";
            // 
            // ShowGroundButton
            // 
            ShowGroundButton.CheckOnClick = true;
            ShowGroundButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ShowGroundButton.Image = Resources.GroundFill;
            ShowGroundButton.ImageTransparentColor = Color.Magenta;
            ShowGroundButton.Name = "ShowGroundButton";
            ShowGroundButton.Size = new Size(46, 36);
            ShowGroundButton.Text = "Show ground";
            // 
            // ShowGroundTextureButton
            // 
            ShowGroundTextureButton.CheckOnClick = true;
            ShowGroundTextureButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ShowGroundTextureButton.Image = Resources.Ground;
            ShowGroundTextureButton.ImageTransparentColor = Color.Magenta;
            ShowGroundTextureButton.Name = "ShowGroundTextureButton";
            ShowGroundTextureButton.Size = new Size(46, 36);
            ShowGroundTextureButton.Text = "Show ground texture";
            // 
            // ShowSkyTextureButton
            // 
            ShowSkyTextureButton.CheckOnClick = true;
            ShowSkyTextureButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ShowSkyTextureButton.Image = Resources.Sky;
            ShowSkyTextureButton.ImageTransparentColor = Color.Magenta;
            ShowSkyTextureButton.Name = "ShowSkyTextureButton";
            ShowSkyTextureButton.Size = new Size(46, 36);
            ShowSkyTextureButton.Text = "Show sky texture";
            // 
            // ZoomTexturesButton
            // 
            ZoomTexturesButton.CheckOnClick = true;
            ZoomTexturesButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ZoomTexturesButton.Image = Resources.ZoomTexture;
            ZoomTexturesButton.ImageTransparentColor = Color.Magenta;
            ZoomTexturesButton.Name = "ZoomTexturesButton";
            ZoomTexturesButton.Size = new Size(46, 36);
            ZoomTexturesButton.Text = "Zoom textures";
            // 
            // toolStripSeparator12
            // 
            toolStripSeparator12.Name = "toolStripSeparator12";
            toolStripSeparator12.Size = new Size(6, 42);
            // 
            // BestTimeLabel
            // 
            BestTimeLabel.AutoSize = false;
            BestTimeLabel.Name = "BestTimeLabel";
            BestTimeLabel.Size = new Size(170, 36);
            BestTimeLabel.Text = "Best time: None";
            BestTimeLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // topologyList
            // 
            topologyList.AutoToolTip = false;
            topologyList.DisplayStyle = ToolStripItemDisplayStyle.Text;
            topologyList.ImageTransparentColor = Color.Magenta;
            topologyList.Name = "topologyList";
            topologyList.ShowDropDownArrow = false;
            topologyList.Size = new Size(4, 36);
            // 
            // toolStripSeparator16
            // 
            toolStripSeparator16.Name = "toolStripSeparator16";
            toolStripSeparator16.Size = new Size(6, 42);
            // 
            // playButton
            // 
            playButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            playButton.Image = Resources.Play;
            playButton.Name = "playButton";
            playButton.Size = new Size(46, 36);
            playButton.ToolTipText = "Play";
            playButton.Click += playButton_Click;
            // 
            // stopButton
            // 
            stopButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            stopButton.Enabled = false;
            stopButton.Image = Resources.Stop;
            stopButton.Name = "stopButton";
            stopButton.Size = new Size(46, 36);
            stopButton.ToolTipText = "Stop";
            stopButton.Click += stopButton_Click;
            // 
            // settingsButton
            // 
            settingsButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            settingsButton.Image = Resources.Settings;
            settingsButton.Name = "settingsButton";
            settingsButton.Size = new Size(46, 36);
            settingsButton.ToolTipText = "Settings";
            settingsButton.Click += settingsButton_Click;
            // 
            // PlayTimeLabel
            // 
            PlayTimeLabel.AutoSize = false;
            PlayTimeLabel.Name = "PlayTimeLabel";
            PlayTimeLabel.Size = new Size(170, 36);
            PlayTimeLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // toolStrip3
            // 
            toolStrip3.BackColor = SystemColors.Control;
            toolStrip3.Dock = DockStyle.None;
            toolStrip3.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip3.ImageScalingSize = new Size(32, 32);
            toolStrip3.Items.AddRange(new ToolStripItem[] { InfoLabel });
            toolStrip3.Location = new Point(0, 93);
            toolStrip3.Name = "toolStrip3";
            toolStrip3.Size = new Size(1858, 38);
            toolStrip3.Stretch = true;
            toolStrip3.TabIndex = 17;
            // 
            // InfoLabel
            // 
            InfoLabel.BackColor = SystemColors.Control;
            InfoLabel.Name = "InfoLabel";
            InfoLabel.Size = new Size(83, 32);
            InfoLabel.Text = "Ready.";
            // 
            // EditorMenuStrip
            // 
            EditorMenuStrip.ImageScalingSize = new Size(15, 15);
            EditorMenuStrip.Items.AddRange(new ToolStripItem[] { CopyMenuItem, TransformMenuItem, DeleteMenuItem, GrassMenuItem, GravityNoneMenuItem, GravityUpMenuItem, GravityDownMenuItem, GravityLeftMenuItem, GravityRightMenuItem, PicturePropertiesMenuItem, bringToFrontToolStripMenuItem, sendToBackToolStripMenuItem, convertToToolStripMenuItem, saveStartPositionToolStripMenuItem, restoreStartPositionToolStripMenuItem, moveStartHereToolStripMenuItem, createCustomShapeMenuItem });
            EditorMenuStrip.Name = "SelectedMenuStrip";
            EditorMenuStrip.Size = new Size(314, 644);
            EditorMenuStrip.Opening += EditorMenuStrip_Opening;
            // 
            // CopyMenuItem
            // 
            CopyMenuItem.Name = "CopyMenuItem";
            CopyMenuItem.Size = new Size(313, 40);
            CopyMenuItem.Text = "Copy";
            CopyMenuItem.Click += CopyMenuItemClick;
            // 
            // TransformMenuItem
            // 
            TransformMenuItem.Name = "TransformMenuItem";
            TransformMenuItem.Size = new Size(313, 40);
            TransformMenuItem.Text = "Transform";
            TransformMenuItem.Click += TransformMenuItemClick;
            // 
            // DeleteMenuItem
            // 
            DeleteMenuItem.Name = "DeleteMenuItem";
            DeleteMenuItem.Size = new Size(313, 40);
            DeleteMenuItem.Text = "Delete";
            DeleteMenuItem.Click += DeleteSelected;
            // 
            // GrassMenuItem
            // 
            GrassMenuItem.Name = "GrassMenuItem";
            GrassMenuItem.Size = new Size(313, 40);
            GrassMenuItem.Text = "Toggle grass";
            GrassMenuItem.Click += HandleGrassMenu;
            // 
            // GravityNoneMenuItem
            // 
            GravityNoneMenuItem.Checked = true;
            GravityNoneMenuItem.CheckOnClick = true;
            GravityNoneMenuItem.CheckState = CheckState.Checked;
            GravityNoneMenuItem.Name = "GravityNoneMenuItem";
            GravityNoneMenuItem.Size = new Size(313, 40);
            GravityNoneMenuItem.Text = "Gravity none";
            GravityNoneMenuItem.Click += HandleGravityMenu;
            // 
            // GravityUpMenuItem
            // 
            GravityUpMenuItem.CheckOnClick = true;
            GravityUpMenuItem.Name = "GravityUpMenuItem";
            GravityUpMenuItem.Size = new Size(313, 40);
            GravityUpMenuItem.Text = "Gravity up";
            GravityUpMenuItem.Click += HandleGravityMenu;
            // 
            // GravityDownMenuItem
            // 
            GravityDownMenuItem.CheckOnClick = true;
            GravityDownMenuItem.Name = "GravityDownMenuItem";
            GravityDownMenuItem.Size = new Size(313, 40);
            GravityDownMenuItem.Text = "Gravity down";
            GravityDownMenuItem.Click += HandleGravityMenu;
            // 
            // GravityLeftMenuItem
            // 
            GravityLeftMenuItem.CheckOnClick = true;
            GravityLeftMenuItem.Name = "GravityLeftMenuItem";
            GravityLeftMenuItem.Size = new Size(313, 40);
            GravityLeftMenuItem.Text = "Gravity left";
            GravityLeftMenuItem.Click += HandleGravityMenu;
            // 
            // GravityRightMenuItem
            // 
            GravityRightMenuItem.CheckOnClick = true;
            GravityRightMenuItem.Name = "GravityRightMenuItem";
            GravityRightMenuItem.Size = new Size(313, 40);
            GravityRightMenuItem.Text = "Gravity right";
            GravityRightMenuItem.Click += HandleGravityMenu;
            // 
            // PicturePropertiesMenuItem
            // 
            PicturePropertiesMenuItem.Name = "PicturePropertiesMenuItem";
            PicturePropertiesMenuItem.Size = new Size(313, 40);
            PicturePropertiesMenuItem.Text = "Picture properties";
            PicturePropertiesMenuItem.Click += PicturePropertiesToolStripMenuItemClick;
            // 
            // bringToFrontToolStripMenuItem
            // 
            bringToFrontToolStripMenuItem.Name = "bringToFrontToolStripMenuItem";
            bringToFrontToolStripMenuItem.Size = new Size(313, 40);
            bringToFrontToolStripMenuItem.Text = "Bring to front";
            bringToFrontToolStripMenuItem.Click += BringToFrontToolStripMenuItemClick;
            // 
            // sendToBackToolStripMenuItem
            // 
            sendToBackToolStripMenuItem.Name = "sendToBackToolStripMenuItem";
            sendToBackToolStripMenuItem.Size = new Size(313, 40);
            sendToBackToolStripMenuItem.Text = "Send to back";
            sendToBackToolStripMenuItem.Click += SendToBackToolStripMenuItemClick;
            // 
            // convertToToolStripMenuItem
            // 
            convertToToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { applesConvertItem, killersConvertItem, flowersConvertItem, picturesConvertItem });
            convertToToolStripMenuItem.Name = "convertToToolStripMenuItem";
            convertToToolStripMenuItem.Size = new Size(313, 40);
            convertToToolStripMenuItem.Text = "Convert to";
            // 
            // applesConvertItem
            // 
            applesConvertItem.Name = "applesConvertItem";
            applesConvertItem.Size = new Size(230, 44);
            applesConvertItem.Text = "Apples";
            applesConvertItem.Click += ConvertClicked;
            // 
            // killersConvertItem
            // 
            killersConvertItem.Name = "killersConvertItem";
            killersConvertItem.Size = new Size(230, 44);
            killersConvertItem.Text = "Killers";
            killersConvertItem.Click += ConvertClicked;
            // 
            // flowersConvertItem
            // 
            flowersConvertItem.Name = "flowersConvertItem";
            flowersConvertItem.Size = new Size(230, 44);
            flowersConvertItem.Text = "Flowers";
            flowersConvertItem.Click += ConvertClicked;
            // 
            // picturesConvertItem
            // 
            picturesConvertItem.Name = "picturesConvertItem";
            picturesConvertItem.Size = new Size(230, 44);
            picturesConvertItem.Text = "Pictures";
            picturesConvertItem.Click += ConvertClicked;
            // 
            // saveStartPositionToolStripMenuItem
            // 
            saveStartPositionToolStripMenuItem.Name = "saveStartPositionToolStripMenuItem";
            saveStartPositionToolStripMenuItem.Size = new Size(313, 40);
            saveStartPositionToolStripMenuItem.Text = "Save start position";
            saveStartPositionToolStripMenuItem.Click += SaveStartPosition_Click;
            // 
            // restoreStartPositionToolStripMenuItem
            // 
            restoreStartPositionToolStripMenuItem.Name = "restoreStartPositionToolStripMenuItem";
            restoreStartPositionToolStripMenuItem.Size = new Size(313, 40);
            restoreStartPositionToolStripMenuItem.Text = "Restore start position";
            restoreStartPositionToolStripMenuItem.Click += restoreStartPositionToolStripMenuItem_Click;
            // 
            // moveStartHereToolStripMenuItem
            // 
            moveStartHereToolStripMenuItem.Name = "moveStartHereToolStripMenuItem";
            moveStartHereToolStripMenuItem.Size = new Size(313, 40);
            moveStartHereToolStripMenuItem.Text = "Move start here";
            moveStartHereToolStripMenuItem.Click += MoveStartHereToolStripMenuItem_Click;
            // 
            // createCustomShapeMenuItem
            // 
            createCustomShapeMenuItem.Name = "createCustomShapeMenuItem";
            createCustomShapeMenuItem.Size = new Size(188, 22);
            createCustomShapeMenuItem.Text = "Create Custom Shape";
            createCustomShapeMenuItem.Click += createCustomShapeMenuItem_Click;
            // 
            // saveAsPictureDialog
            // 
            saveAsPictureDialog.DefaultExt = "png";
            saveAsPictureDialog.FileName = "Untitled";
            saveAsPictureDialog.Filter = "Portable Network Graphics (*.png)|*.png|Scalable Vector Graphics (*.svg)|*.svg";
            // 
            // importFileDialog
            // 
            importFileDialog.Filter = "Elasto Mania level or image (*.lev, *.bmp, *.png, *.gif, *.tiff, *.exif, *.svg, *.svgz)|*.lev;*.bmp;*.png;*.gif;*.tiff;*.exif;*.svg;*.svgz";
            importFileDialog.Multiselect = true;
            // 
            // ToolPanel
            // 
            ToolPanel.AutoScroll = true;
            ToolPanel.Controls.Add(CustomShapeButton);
            ToolPanel.Controls.Add(TextButton);
            ToolPanel.Controls.Add(PictureButton);
            ToolPanel.Controls.Add(AutoGrassButton);
            ToolPanel.Controls.Add(CutConnectButton);
            ToolPanel.Controls.Add(SmoothenButton);
            ToolPanel.Controls.Add(FrameButton);
            ToolPanel.Controls.Add(PolyOpButton);
            ToolPanel.Controls.Add(EllipseButton);
            ToolPanel.Controls.Add(PipeButton);
            ToolPanel.Controls.Add(ObjectButton);
            ToolPanel.Controls.Add(DrawButton);
            ToolPanel.Controls.Add(VertexButton);
            ToolPanel.Controls.Add(SelectButton);
            ToolPanel.Dock = DockStyle.Left;
            ToolPanel.Location = new Point(0, 175);
            ToolPanel.Margin = new Padding(6);
            ToolPanel.Name = "ToolPanel";
            ToolPanel.Size = new Size(168, 797);
            ToolPanel.TabIndex = 3;
            ToolPanel.Text = "Tools";
            // 
            // TextButton
            // 
            TextButton.Appearance = Appearance.Button;
            TextButton.AutoSize = true;
            TextButton.Dock = DockStyle.Top;
            TextButton.Location = new Point(0, 504);
            TextButton.Margin = new Padding(6);
            TextButton.Name = "TextButton";
            TextButton.Size = new Size(168, 42);
            TextButton.TabIndex = 16;
            TextButton.Text = "&Text";
            TextButton.TextAlign = ContentAlignment.MiddleCenter;
            TextButton.UseVisualStyleBackColor = true;
            TextButton.CheckedChanged += TextButton_CheckedChanged;
            // 
            // PictureButton
            // 
            PictureButton.Appearance = Appearance.Button;
            PictureButton.AutoSize = true;
            PictureButton.Dock = DockStyle.Top;
            PictureButton.Location = new Point(0, 462);
            PictureButton.Margin = new Padding(6);
            PictureButton.Name = "PictureButton";
            PictureButton.Size = new Size(168, 42);
            PictureButton.TabIndex = 15;
            PictureButton.Text = "P&icture";
            PictureButton.TextAlign = ContentAlignment.MiddleCenter;
            PictureButton.UseVisualStyleBackColor = true;
            // 
            // AutoGrassButton
            // 
            AutoGrassButton.Appearance = Appearance.Button;
            AutoGrassButton.AutoSize = true;
            AutoGrassButton.Dock = DockStyle.Top;
            AutoGrassButton.Location = new Point(0, 420);
            AutoGrassButton.Margin = new Padding(6);
            AutoGrassButton.Name = "AutoGrassButton";
            AutoGrassButton.Size = new Size(168, 42);
            AutoGrassButton.TabIndex = 14;
            AutoGrassButton.Text = "&AutoGrass";
            AutoGrassButton.TextAlign = ContentAlignment.MiddleCenter;
            AutoGrassButton.UseVisualStyleBackColor = true;
            // 
            // CutConnectButton
            // 
            CutConnectButton.Appearance = Appearance.Button;
            CutConnectButton.AutoSize = true;
            CutConnectButton.Dock = DockStyle.Top;
            CutConnectButton.Location = new Point(0, 378);
            CutConnectButton.Margin = new Padding(6);
            CutConnectButton.Name = "CutConnectButton";
            CutConnectButton.Size = new Size(168, 42);
            CutConnectButton.TabIndex = 13;
            CutConnectButton.Text = "&Cut/connect";
            CutConnectButton.TextAlign = ContentAlignment.MiddleCenter;
            CutConnectButton.UseVisualStyleBackColor = true;
            // 
            // SmoothenButton
            // 
            SmoothenButton.Appearance = Appearance.Button;
            SmoothenButton.AutoSize = true;
            SmoothenButton.Dock = DockStyle.Top;
            SmoothenButton.Location = new Point(0, 336);
            SmoothenButton.Margin = new Padding(6);
            SmoothenButton.Name = "SmoothenButton";
            SmoothenButton.Size = new Size(168, 42);
            SmoothenButton.TabIndex = 12;
            SmoothenButton.Text = "S&moothen";
            SmoothenButton.TextAlign = ContentAlignment.MiddleCenter;
            SmoothenButton.UseVisualStyleBackColor = true;
            // 
            // FrameButton
            // 
            FrameButton.Appearance = Appearance.Button;
            FrameButton.AutoSize = true;
            FrameButton.Dock = DockStyle.Top;
            FrameButton.Location = new Point(0, 294);
            FrameButton.Margin = new Padding(6);
            FrameButton.Name = "FrameButton";
            FrameButton.Size = new Size(168, 42);
            FrameButton.TabIndex = 11;
            FrameButton.Text = "&Frame";
            FrameButton.TextAlign = ContentAlignment.MiddleCenter;
            FrameButton.UseVisualStyleBackColor = true;
            // 
            // PolyOpButton
            // 
            PolyOpButton.Appearance = Appearance.Button;
            PolyOpButton.AutoSize = true;
            PolyOpButton.Dock = DockStyle.Top;
            PolyOpButton.Location = new Point(0, 252);
            PolyOpButton.Margin = new Padding(6);
            PolyOpButton.Name = "PolyOpButton";
            PolyOpButton.Size = new Size(168, 42);
            PolyOpButton.TabIndex = 9;
            PolyOpButton.Text = "Po&lyOp";
            PolyOpButton.TextAlign = ContentAlignment.MiddleCenter;
            PolyOpButton.UseVisualStyleBackColor = true;
            // 
            // EllipseButton
            // 
            EllipseButton.Appearance = Appearance.Button;
            EllipseButton.AutoSize = true;
            EllipseButton.Dock = DockStyle.Top;
            EllipseButton.Location = new Point(0, 210);
            EllipseButton.Margin = new Padding(6);
            EllipseButton.Name = "EllipseButton";
            EllipseButton.Size = new Size(168, 42);
            EllipseButton.TabIndex = 8;
            EllipseButton.Text = "&Ellipse";
            EllipseButton.TextAlign = ContentAlignment.MiddleCenter;
            EllipseButton.UseVisualStyleBackColor = true;
            // 
            // PipeButton
            // 
            PipeButton.Appearance = Appearance.Button;
            PipeButton.AutoSize = true;
            PipeButton.Dock = DockStyle.Top;
            PipeButton.Location = new Point(0, 168);
            PipeButton.Margin = new Padding(6);
            PipeButton.Name = "PipeButton";
            PipeButton.Size = new Size(168, 42);
            PipeButton.TabIndex = 6;
            PipeButton.Text = "&Pipe";
            PipeButton.TextAlign = ContentAlignment.MiddleCenter;
            PipeButton.UseVisualStyleBackColor = true;
            // 
            // ObjectButton
            // 
            ObjectButton.Appearance = Appearance.Button;
            ObjectButton.AutoSize = true;
            ObjectButton.Dock = DockStyle.Top;
            ObjectButton.Location = new Point(0, 126);
            ObjectButton.Margin = new Padding(6);
            ObjectButton.Name = "ObjectButton";
            ObjectButton.Size = new Size(168, 42);
            ObjectButton.TabIndex = 3;
            ObjectButton.Text = "&Object";
            ObjectButton.TextAlign = ContentAlignment.MiddleCenter;
            ObjectButton.UseVisualStyleBackColor = true;
            // 
            // DrawButton
            // 
            DrawButton.Appearance = Appearance.Button;
            DrawButton.AutoSize = true;
            DrawButton.Dock = DockStyle.Top;
            DrawButton.Location = new Point(0, 84);
            DrawButton.Margin = new Padding(6);
            DrawButton.Name = "DrawButton";
            DrawButton.Size = new Size(168, 42);
            DrawButton.TabIndex = 2;
            DrawButton.Text = "&Draw";
            DrawButton.TextAlign = ContentAlignment.MiddleCenter;
            DrawButton.UseVisualStyleBackColor = true;
            // 
            // VertexButton
            // 
            VertexButton.Appearance = Appearance.Button;
            VertexButton.AutoSize = true;
            VertexButton.Dock = DockStyle.Top;
            VertexButton.Location = new Point(0, 42);
            VertexButton.Margin = new Padding(6);
            VertexButton.Name = "VertexButton";
            VertexButton.Size = new Size(168, 42);
            VertexButton.TabIndex = 1;
            VertexButton.Text = "&Vertex";
            VertexButton.TextAlign = ContentAlignment.MiddleCenter;
            VertexButton.UseVisualStyleBackColor = true;
            // 
            // SelectButton
            // 
            SelectButton.Appearance = Appearance.Button;
            SelectButton.AutoSize = true;
            SelectButton.Checked = true;
            SelectButton.Dock = DockStyle.Top;
            SelectButton.Location = new Point(0, 0);
            SelectButton.Margin = new Padding(6);
            SelectButton.Name = "SelectButton";
            SelectButton.Size = new Size(168, 42);
            SelectButton.TabIndex = 0;
            SelectButton.TabStop = true;
            SelectButton.Text = "&Select";
            SelectButton.TextAlign = ContentAlignment.MiddleCenter;
            SelectButton.UseVisualStyleBackColor = true;
            // 
            // CustomShapeButton
            // 
            CustomShapeButton.Appearance = Appearance.Button;
            CustomShapeButton.AutoSize = true;
            CustomShapeButton.Dock = DockStyle.Top;
            CustomShapeButton.Location = new Point(0, 325);
            CustomShapeButton.Name = "CustomShapeButton";
            CustomShapeButton.Size = new Size(84, 25);
            CustomShapeButton.TabIndex = 17;
            CustomShapeButton.Text = "Shape";
            CustomShapeButton.TextAlign = ContentAlignment.MiddleCenter;
            CustomShapeButton.UseVisualStyleBackColor = true;
            // 
            // LevelEditorForm
            // 
            AutoScaleDimensions = new SizeF(192F, 192F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(1858, 1018);
            Controls.Add(EditorControl);
            Controls.Add(ToolPanel);
            Controls.Add(StatusStrip1);
            Controls.Add(ToolStripPanel1);
            Controls.Add(MenuStrip1);
            Font = new Font("Segoe UI", 9F);
            MainMenuStrip = MenuStrip1;
            Margin = new Padding(6);
            Name = "LevelEditorForm";
            Text = "SLE";
            FormClosing += ConfirmClose;
            Load += RefreshOnOpen;
            KeyDown += KeyHandlerDown;
            KeyUp += KeyHandlerUp;
            MenuStrip1.ResumeLayout(false);
            MenuStrip1.PerformLayout();
            StatusStrip1.ResumeLayout(false);
            StatusStrip1.PerformLayout();
            ToolStripPanel1.ResumeLayout(false);
            ToolStripPanel1.PerformLayout();
            ToolStrip1.ResumeLayout(false);
            ToolStrip1.PerformLayout();
            ToolStrip2.ResumeLayout(false);
            ToolStrip2.PerformLayout();
            toolStrip3.ResumeLayout(false);
            toolStrip3.PerformLayout();
            EditorMenuStrip.ResumeLayout(false);
            ToolPanel.ResumeLayout(false);
            ToolPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        internal MenuStrip MenuStrip1;
        internal ToolStripMenuItem FileToolStripMenuItem;
        internal ToolStripMenuItem NewToolStripMenuItem;
        internal ToolStripMenuItem OpenToolStripMenuItem;
        internal ToolStripMenuItem SaveToolStripMenuItem;
        internal ToolStripMenuItem SaveAsToolStripMenuItem;
        internal ToolStripMenuItem ExitToolStripMenuItem;
        internal GLControl EditorControl;
        internal OpenFileDialog OpenFileDialog1;
        internal PanelMod ToolPanel;
        internal RadioButtonMod EllipseButton;
        internal RadioButtonMod ObjectButton;
        internal RadioButtonMod VertexButton;
        internal RadioButtonMod PipeButton;
        internal RadioButtonMod SmoothenButton;
        internal RadioButtonMod CutConnectButton;
        internal RadioButtonMod SelectButton;
        internal StatusStrip StatusStrip1;
        internal ToolStripStatusLabel CoordinateLabel;
        internal SaveFileDialog SaveFileDialog1;
        internal ToolStripPanel ToolStripPanel1;
        internal ToolStripMenuItem ConfigurationToolStripMenuItem;
        internal RadioButtonMod PolyOpButton;
        internal RadioButtonMod DrawButton;
        internal ContextMenuStrip EditorMenuStrip;
        internal ToolStripMenuItem CopyMenuItem;
        internal RadioButtonMod FrameButton;
        internal ToolStripMenuItem TransformMenuItem;
        internal ToolStripMenuItem ActionsMenuItem;
        internal ToolStripMenuItem QuickGrassToolStripMenuItem;
        internal ToolStripMenuItem UndoToolStripMenuItem;
        internal ToolStripMenuItem RedoToolStripMenuItem;
        internal RadioButtonMod AutoGrassButton;
        internal ToolStripMenuItem DeleteAllGrassToolStripMenuItem;
        internal ToolStripMenuItem DeleteMenuItem;
        internal ToolStripMenuItem GrassMenuItem;
        internal ToolStripMenuItem GravityNoneMenuItem;
        internal ToolStripMenuItem GravityUpMenuItem;
        internal ToolStripMenuItem GravityDownMenuItem;
        internal ToolStripMenuItem GravityLeftMenuItem;
        internal ToolStripMenuItem GravityRightMenuItem;
        internal RadioButtonMod PictureButton;
        internal ToolStripMenuItem PicturePropertiesMenuItem;
        internal ToolStripMenuItem SelectionFilterToolStripMenuItem;
        internal ToolStripMenuItem EnableAllToolStripMenuItem;
        internal ToolStripMenuItem DisableAllToolStripMenuItem;
        internal ToolStripMenuItem GroundPolygonsToolStripMenuItem;
        internal ToolStripMenuItem GrassPolygonsToolStripMenuItem;
        internal ToolStripMenuItem ApplesToolStripMenuItem;
        internal ToolStripMenuItem KillersToolStripMenuItem;
        internal ToolStripMenuItem FlowersToolStripMenuItem;
        internal ToolStripMenuItem PicturesToolStripMenuItem;
        internal ToolStripMenuItem TexturesToolStripMenuItem;
        internal ToolStrip ToolStrip1;
        internal ToolStripButton NewButton;
        internal ToolStripButton OpenButton;
        internal ToolStripButton SaveButton;
        internal ToolStripButton SaveAsButton;
        internal ToolStripSeparator ToolStripSeparator1;
        internal ToolStripButton CheckTopologyButton;
        internal ToolStripButton ZoomFillButton;
        internal ToolStripSeparator ToolStripSeparator2;
        internal ToolStripButton UndoButton;
        internal ToolStripButton RedoButton;
        internal ToolStripSeparator ToolStripSeparator3;
        internal ToolStripTextBox TitleBox;
        internal ToolStripLabel ToolStripLabel1;
        internal ToolStripSeparator ToolStripSeparator4;
        internal ToolStripLabel ToolStripLabel2;
        internal ToolStripComboBox LGRBox;
        internal ToolStripSeparator ToolStripSeparator5;
        internal ToolStripLabel ToolStripLabel3;
        internal ToolStripComboBox GroundComboBox;
        internal ToolStripSeparator ToolStripSeparator6;
        internal ToolStripLabel ToolStripLabel4;
        internal ToolStripComboBox SkyComboBox;
        internal ToolStrip ToolStrip2;
        internal ToolStripButton ShowGrassEdgesButton;
        internal ToolStripSeparator ToolStripSeparator7;
        internal ToolStripStatusLabel SelectionLabel;
        internal ToolStripButton ShowGroundEdgesButton;
        internal ToolStripButton ShowGridButton;
        internal ToolStripButton ShowVerticesButton;
        internal ToolStripButton ShowTextureFramesButton;
        internal ToolStripButton ShowPictureFramesButton;
        internal ToolStripButton ShowTexturesButton;
        internal ToolStripButton ShowPicturesButton;
        internal ToolStripButton ShowObjectFramesButton;
        internal ToolStripButton ShowObjectsButton;
        internal ToolStripButton ShowGroundButton;
        internal ToolStripButton ShowGroundTextureButton;
        internal ToolStripButton ShowSkyTextureButton;
        internal ToolStripSeparator ToolStripSeparator8;
        internal ToolStripMenuItem ZoomFillToolStripMenuItem;
        internal ToolStripStatusLabel HighlightLabel;
        internal ToolStripMenuItem CheckTopologyMenuItem;
        internal ToolStripButton ZoomTexturesButton;
        internal ToolStripMenuItem MainConfigMenuItem;
        internal ToolStripMenuItem RenderingSettingsToolStripMenuItem;
        internal ToolStripMenuItem LevelPropertiesToolStripMenuItem;
        private IContainer components;
        private ToolStripButton PreviousButton;
        private ToolStripButton NextButton;
        private ToolStripSeparator toolStripSeparator9;
        private ToolStripSeparator toolStripSeparator10;
        private ToolStripMenuItem previousLevelToolStripMenuItem;
        private ToolStripMenuItem nextLevelToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator11;
        private ToolStripMenuItem selectAllToolStripMenuItem;
        private ToolStripButton snapToGridButton;
        private ToolStripButton lockGridButton;
        private ToolStripMenuItem bringToFrontToolStripMenuItem;
        private ToolStripMenuItem sendToBackToolStripMenuItem;
        private ToolStripButton showCrossHairButton;
        private ToolStripMenuItem importLevelsToolStripMenuItem;
        private ToolStripMenuItem saveAsPictureToolStripMenuItem;
        private SaveFileDialog saveAsPictureDialog;
        private ToolStripMenuItem convertToToolStripMenuItem;
        private ToolStripMenuItem applesConvertItem;
        private ToolStripMenuItem killersConvertItem;
        private ToolStripMenuItem flowersConvertItem;
        private ToolStripMenuItem picturesConvertItem;
        internal RadioButtonMod TextButton;
        internal ToolStripButton ShowGravityAppleArrowsButton;
        private ToolStripLabel BestTimeLabel;
        private ToolStripLabel PlayTimeLabel;
        private ToolStripDropDownButton topologyList;
        private ToolStripSeparator toolStripSeparator12;
        private ToolStripMenuItem deleteLevMenuItem;
        private ToolStripButton deleteButton;
        private ToolStripSeparator toolStripSeparator13;
        private ToolStripLabel toolStripLabel5;
        private ToolStripTextBox filenameBox;
        private ToolStripButton filenameOkButton;
        private ToolStripButton filenameCancelButton;
        private OpenFileDialog importFileDialog;
        private ToolStripMenuItem selectionToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem copyAndSnapToGridMenuItem;
        internal ToolStripMenuItem MirrorHorizontallyToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator14;
        private ToolStripMenuItemMod unionToolStripMenuItem;
        private ToolStripMenuItemMod differenceToolStripMenuItem;
        private ToolStripMenuItemMod intersectionToolStripMenuItem;
        private ToolStripMenuItemMod symmetricDifferenceToolStripMenuItem;
        internal ToolStripMenuItemMod DeleteSelectedMenuItem;
        private ToolStripSeparator toolStripSeparator15;
        private ToolStripMenuItemMod texturizeMenuItem;
        private ToolStripMenuItem saveStartPositionToolStripMenuItem;
        private ToolStripMenuItem restoreStartPositionToolStripMenuItem;
        internal ToolStripMenuItem MirrorVerticallyToolStripMenuItem;
        private ToolStripMenuItem moveStartHereToolStripMenuItem;
        private ToolStrip toolStrip3;
        internal ToolStripLabel InfoLabel;
        private ToolStripSeparator toolStripSeparator16;
        private ToolStripButton playButton;
        private ToolStripButton stopButton;
        private ToolStripButton settingsButton;
        private ToolStripMenuItem deselectGroundPolygonsToolStripMenuItem;
        private ToolStripMenuItem deselectGrassPolygonsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator17;
        private ToolStripMenuItem deselectApplesToolStripMenuItem;
        private ToolStripMenuItem deselectKillersToolStripMenuItem;
        private ToolStripMenuItem deselectFlowersToolStripMenuItem;
        private ToolStripMenuItem deselectPicturesToolStripMenuItem;
        private ToolStripMenuItem deselectTexturesToolStripMenuItem;
        internal ToolStripButton showGrassButton;
        private ToolStripMenuItem openInternalToolStripMenuItem;
        private ToolStripMenuItem openInternalPart1ToolStripMenuItem;
        private ToolStripMenuItem openInternalPart2ToolStripMenuItem;
        internal ToolStripStatusLabel zoomLabel;
        internal ToolStripMenuItem StartToolStripMenuItem;
        private ToolStripMenuItemMod fixSelfIntersectionsMenuItem;
        private ToolStripMenuItem createCustomShapeToolStripMenuItem;
        private ToolStripMenuItem createCustomShapeMenuItem;
        internal RadioButtonMod CustomShapeButton;
    }

}
