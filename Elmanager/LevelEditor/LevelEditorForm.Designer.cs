using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Elmanager.IO;
using Elmanager.Properties;
using Elmanager.UI;
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
            this.components = new Container();
            ComponentResourceManager resources = new ComponentResourceManager(typeof(LevelEditorForm));
            this.MenuStrip1 = new MenuStrip();
            this.FileToolStripMenuItem = new ToolStripMenuItem();
            this.NewToolStripMenuItem = new ToolStripMenuItem();
            this.OpenToolStripMenuItem = new ToolStripMenuItem();
            this.SaveToolStripMenuItem = new ToolStripMenuItem();
            this.SaveAsToolStripMenuItem = new ToolStripMenuItem();
            this.saveAsPictureToolStripMenuItem = new ToolStripMenuItem();
            this.deleteLevMenuItem = new ToolStripMenuItem();
            this.ExitToolStripMenuItem = new ToolStripMenuItem();
            this.ActionsMenuItem = new ToolStripMenuItem();
            this.QuickGrassToolStripMenuItem = new ToolStripMenuItem();
            this.DeleteAllGrassToolStripMenuItem = new ToolStripMenuItem();
            this.UndoToolStripMenuItem = new ToolStripMenuItem();
            this.RedoToolStripMenuItem = new ToolStripMenuItem();
            this.ToolStripSeparator8 = new ToolStripSeparator();
            this.ZoomFillToolStripMenuItem = new ToolStripMenuItem();
            this.CheckTopologyMenuItem = new ToolStripMenuItem();
            this.LevelPropertiesToolStripMenuItem = new ToolStripMenuItem();
            this.toolStripSeparator10 = new ToolStripSeparator();
            this.previousLevelToolStripMenuItem = new ToolStripMenuItem();
            this.nextLevelToolStripMenuItem = new ToolStripMenuItem();
            this.toolStripSeparator11 = new ToolStripSeparator();
            this.selectAllToolStripMenuItem = new ToolStripMenuItem();
            this.importLevelsToolStripMenuItem = new ToolStripMenuItem();
            this.selectionToolStripMenuItem = new ToolStripMenuItem();
            this.toolStripMenuItem1 = new ToolStripMenuItem();
            this.copyAndSnapToGridMenuItem = new ToolStripMenuItem();
            this.MirrorHorizontallyToolStripMenuItem = new ToolStripMenuItem();
            this.MirrorVerticallyToolStripMenuItem = new ToolStripMenuItem();
            this.DeleteSelectedMenuItem = new ToolStripMenuItemMod();
            this.toolStripSeparator14 = new ToolStripSeparator();
            this.unionToolStripMenuItem = new ToolStripMenuItemMod();
            this.differenceToolStripMenuItem = new ToolStripMenuItemMod();
            this.intersectionToolStripMenuItem = new ToolStripMenuItemMod();
            this.symmetricDifferenceToolStripMenuItem = new ToolStripMenuItemMod();
            this.toolStripSeparator15 = new ToolStripSeparator();
            this.texturizeMenuItem = new ToolStripMenuItemMod();
            this.SelectionFilterToolStripMenuItem = new ToolStripMenuItem();
            this.EnableAllToolStripMenuItem = new ToolStripMenuItem();
            this.DisableAllToolStripMenuItem = new ToolStripMenuItem();
            this.GroundPolygonsToolStripMenuItem = new ToolStripMenuItem();
            this.GrassPolygonsToolStripMenuItem = new ToolStripMenuItem();
            this.ApplesToolStripMenuItem = new ToolStripMenuItem();
            this.KillersToolStripMenuItem = new ToolStripMenuItem();
            this.FlowersToolStripMenuItem = new ToolStripMenuItem();
            this.PicturesToolStripMenuItem = new ToolStripMenuItem();
            this.TexturesToolStripMenuItem = new ToolStripMenuItem();
            this.ConfigurationToolStripMenuItem = new ToolStripMenuItem();
            this.MainConfigMenuItem = new ToolStripMenuItem();
            this.RenderingSettingsToolStripMenuItem = new ToolStripMenuItem();
            this.EditorControl = new GLControl();
            this.OpenFileDialog1 = new OpenFileDialog();
            this.StatusStrip1 = new StatusStrip();
            this.CoordinateLabel = new ToolStripStatusLabel();
            this.SelectionLabel = new ToolStripStatusLabel();
            this.HighlightLabel = new ToolStripStatusLabel();
            this.SaveFileDialog1 = new SaveFileDialog();
            this.ToolStripPanel1 = new ToolStripPanel();
            this.ToolStrip1 = new ToolStrip();
            this.NewButton = new ToolStripButton();
            this.OpenButton = new ToolStripButton();
            this.SaveButton = new ToolStripButton();
            this.SaveAsButton = new ToolStripButton();
            this.deleteButton = new ToolStripButton();
            this.ToolStripSeparator1 = new ToolStripSeparator();
            this.CheckTopologyButton = new ToolStripButton();
            this.ZoomFillButton = new ToolStripButton();
            this.ToolStripSeparator2 = new ToolStripSeparator();
            this.UndoButton = new ToolStripButton();
            this.RedoButton = new ToolStripButton();
            this.ToolStripSeparator3 = new ToolStripSeparator();
            this.PreviousButton = new ToolStripButton();
            this.NextButton = new ToolStripButton();
            this.toolStripSeparator13 = new ToolStripSeparator();
            this.toolStripLabel5 = new ToolStripLabel();
            this.filenameBox = new ToolStripTextBox();
            this.filenameOkButton = new ToolStripButton();
            this.filenameCancelButton = new ToolStripButton();
            this.toolStripSeparator9 = new ToolStripSeparator();
            this.ToolStripLabel1 = new ToolStripLabel();
            this.TitleBox = new ToolStripTextBox();
            this.ToolStripSeparator4 = new ToolStripSeparator();
            this.ToolStripLabel2 = new ToolStripLabel();
            this.LGRBox = new ToolStripTextBox();
            this.ToolStripSeparator5 = new ToolStripSeparator();
            this.ToolStripLabel3 = new ToolStripLabel();
            this.GroundComboBox = new ToolStripComboBox();
            this.ToolStripSeparator6 = new ToolStripSeparator();
            this.ToolStripLabel4 = new ToolStripLabel();
            this.SkyComboBox = new ToolStripComboBox();
            this.ToolStripSeparator7 = new ToolStripSeparator();
            this.ToolStrip2 = new ToolStrip();
            this.ShowGridButton = new ToolStripButton();
            this.snapToGridButton = new ToolStripButton();
            this.showCrossHairButton = new ToolStripButton();
            this.ShowGrassEdgesButton = new ToolStripButton();
            this.ShowGroundEdgesButton = new ToolStripButton();
            this.ShowVerticesButton = new ToolStripButton();
            this.ShowTextureFramesButton = new ToolStripButton();
            this.ShowPictureFramesButton = new ToolStripButton();
            this.ShowTexturesButton = new ToolStripButton();
            this.ShowPicturesButton = new ToolStripButton();
            this.ShowObjectFramesButton = new ToolStripButton();
            this.ShowObjectsButton = new ToolStripButton();
            this.ShowGravityAppleArrowsButton = new ToolStripButton();
            this.ShowGroundButton = new ToolStripButton();
            this.ShowGroundTextureButton = new ToolStripButton();
            this.ShowSkyTextureButton = new ToolStripButton();
            this.ZoomTexturesButton = new ToolStripButton();
            this.toolStripSeparator12 = new ToolStripSeparator();
            this.BestTimeLabel = new ToolStripLabel();
            this.PlayTimeLabel = new ToolStripLabel();
            this.topologyList = new ToolStripDropDownButton();
            this.toolStrip3 = new ToolStrip();
            this.InfoLabel = new ToolStripLabel();
            this.EditorMenuStrip = new ContextMenuStrip(this.components);
            this.CopyMenuItem = new ToolStripMenuItem();
            this.TransformMenuItem = new ToolStripMenuItem();
            this.DeleteMenuItem = new ToolStripMenuItem();
            this.GrassMenuItem = new ToolStripMenuItem();
            this.GravityNoneMenuItem = new ToolStripMenuItem();
            this.GravityUpMenuItem = new ToolStripMenuItem();
            this.GravityDownMenuItem = new ToolStripMenuItem();
            this.GravityLeftMenuItem = new ToolStripMenuItem();
            this.GravityRightMenuItem = new ToolStripMenuItem();
            this.PicturePropertiesMenuItem = new ToolStripMenuItem();
            this.bringToFrontToolStripMenuItem = new ToolStripMenuItem();
            this.sendToBackToolStripMenuItem = new ToolStripMenuItem();
            this.convertToToolStripMenuItem = new ToolStripMenuItem();
            this.applesConvertItem = new ToolStripMenuItem();
            this.killersConvertItem = new ToolStripMenuItem();
            this.flowersConvertItem = new ToolStripMenuItem();
            this.picturesConvertItem = new ToolStripMenuItem();
            this.saveStartPositionToolStripMenuItem = new ToolStripMenuItem();
            this.restoreStartPositionToolStripMenuItem = new ToolStripMenuItem();
            this.moveStartHereToolStripMenuItem = new ToolStripMenuItem();
            this.saveAsPictureDialog = new SaveFileDialog();
            this.importFileDialog = new OpenFileDialog();
            this.ToolPanel = new PanelMod();
            this.TextButton = new RadioButtonMod();
            this.PictureButton = new RadioButtonMod();
            this.AutoGrassButton = new RadioButtonMod();
            this.CutConnectButton = new RadioButtonMod();
            this.SmoothenButton = new RadioButtonMod();
            this.FrameButton = new RadioButtonMod();
            this.PolyOpButton = new RadioButtonMod();
            this.EllipseButton = new RadioButtonMod();
            this.ZoomButton = new RadioButtonMod();
            this.PipeButton = new RadioButtonMod();
            this.ObjectButton = new RadioButtonMod();
            this.DrawButton = new RadioButtonMod();
            this.VertexButton = new RadioButtonMod();
            this.SelectButton = new RadioButtonMod();
            this.toolStripSeparator16 = new ToolStripSeparator();
            this.playButton = new SvgImageToolStripButton();
            this.stopButton = new SvgImageToolStripButton();
            this.settingsButton = new SvgImageToolStripButton();
            this.MenuStrip1.SuspendLayout();
            this.StatusStrip1.SuspendLayout();
            this.ToolStripPanel1.SuspendLayout();
            this.ToolStrip1.SuspendLayout();
            this.ToolStrip2.SuspendLayout();
            this.toolStrip3.SuspendLayout();
            this.EditorMenuStrip.SuspendLayout();
            this.ToolPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // MenuStrip1
            // 
            this.MenuStrip1.BackColor = SystemColors.Control;
            this.MenuStrip1.ImageScalingSize = new Size(15, 15);
            this.MenuStrip1.Items.AddRange(new ToolStripItem[] {
            this.FileToolStripMenuItem,
            this.ActionsMenuItem,
            this.selectionToolStripMenuItem,
            this.SelectionFilterToolStripMenuItem,
            this.ConfigurationToolStripMenuItem});
            this.MenuStrip1.Location = new Point(0, 0);
            this.MenuStrip1.Name = "MenuStrip1";
            this.MenuStrip1.Size = new Size(929, 24);
            this.MenuStrip1.TabIndex = 0;
            this.MenuStrip1.Text = "MenuStrip1";
            // 
            // FileToolStripMenuItem
            // 
            this.FileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
            this.NewToolStripMenuItem,
            this.OpenToolStripMenuItem,
            this.SaveToolStripMenuItem,
            this.SaveAsToolStripMenuItem,
            this.saveAsPictureToolStripMenuItem,
            this.deleteLevMenuItem,
            this.ExitToolStripMenuItem});
            this.FileToolStripMenuItem.Name = "FileToolStripMenuItem";
            this.FileToolStripMenuItem.Size = new Size(37, 20);
            this.FileToolStripMenuItem.Text = "File";
            // 
            // NewToolStripMenuItem
            // 
            this.NewToolStripMenuItem.Image = Resources.New16;
            this.NewToolStripMenuItem.Name = "NewToolStripMenuItem";
            this.NewToolStripMenuItem.ShortcutKeys = ((Keys)((Keys.Control | Keys.N)));
            this.NewToolStripMenuItem.Size = new Size(202, 22);
            this.NewToolStripMenuItem.Text = "New";
            this.NewToolStripMenuItem.Click += new EventHandler(this.NewLevel);
            // 
            // OpenToolStripMenuItem
            // 
            this.OpenToolStripMenuItem.Image = Resources.Open;
            this.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem";
            this.OpenToolStripMenuItem.ShortcutKeys = ((Keys)((Keys.Control | Keys.O)));
            this.OpenToolStripMenuItem.Size = new Size(202, 22);
            this.OpenToolStripMenuItem.Text = "Open";
            this.OpenToolStripMenuItem.Click += new EventHandler(this.OpenToolStripMenuItemClick);
            // 
            // SaveToolStripMenuItem
            // 
            this.SaveToolStripMenuItem.Enabled = false;
            this.SaveToolStripMenuItem.Image = Resources.Save;
            this.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem";
            this.SaveToolStripMenuItem.ShortcutKeys = ((Keys)((Keys.Control | Keys.S)));
            this.SaveToolStripMenuItem.Size = new Size(202, 22);
            this.SaveToolStripMenuItem.Text = "Save";
            this.SaveToolStripMenuItem.Click += new EventHandler(this.SaveClicked);
            // 
            // SaveAsToolStripMenuItem
            // 
            this.SaveAsToolStripMenuItem.Image = Resources.SaveAs16;
            this.SaveAsToolStripMenuItem.Name = "SaveAsToolStripMenuItem";
            this.SaveAsToolStripMenuItem.Size = new Size(202, 22);
            this.SaveAsToolStripMenuItem.Text = "Save as...";
            this.SaveAsToolStripMenuItem.Click += new EventHandler(this.SaveAs);
            // 
            // saveAsPictureToolStripMenuItem
            // 
            this.saveAsPictureToolStripMenuItem.Name = "saveAsPictureToolStripMenuItem";
            this.saveAsPictureToolStripMenuItem.ShortcutKeys = ((Keys)((Keys.Control | Keys.P)));
            this.saveAsPictureToolStripMenuItem.Size = new Size(202, 22);
            this.saveAsPictureToolStripMenuItem.Text = "Save as picture...";
            this.saveAsPictureToolStripMenuItem.Click += new EventHandler(this.saveAsPictureToolStripMenuItem_Click);
            // 
            // deleteLevMenuItem
            // 
            this.deleteLevMenuItem.Image = Resources.Delete;
            this.deleteLevMenuItem.Name = "deleteLevMenuItem";
            this.deleteLevMenuItem.Size = new Size(202, 22);
            this.deleteLevMenuItem.Text = "Delete";
            this.deleteLevMenuItem.Click += new EventHandler(this.deleteLevMenuItem_Click);
            // 
            // ExitToolStripMenuItem
            // 
            this.ExitToolStripMenuItem.Image = Resources.Exit16;
            this.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem";
            this.ExitToolStripMenuItem.Size = new Size(202, 22);
            this.ExitToolStripMenuItem.Text = "Exit";
            this.ExitToolStripMenuItem.Click += new EventHandler(this.ExitToolStripMenuItemClick);
            // 
            // ActionsMenuItem
            // 
            this.ActionsMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
            this.QuickGrassToolStripMenuItem,
            this.DeleteAllGrassToolStripMenuItem,
            this.UndoToolStripMenuItem,
            this.RedoToolStripMenuItem,
            this.ToolStripSeparator8,
            this.ZoomFillToolStripMenuItem,
            this.CheckTopologyMenuItem,
            this.LevelPropertiesToolStripMenuItem,
            this.toolStripSeparator10,
            this.previousLevelToolStripMenuItem,
            this.nextLevelToolStripMenuItem,
            this.toolStripSeparator11,
            this.selectAllToolStripMenuItem,
            this.importLevelsToolStripMenuItem});
            this.ActionsMenuItem.Name = "ActionsMenuItem";
            this.ActionsMenuItem.Size = new Size(46, 20);
            this.ActionsMenuItem.Text = "Tools";
            // 
            // QuickGrassToolStripMenuItem
            // 
            this.QuickGrassToolStripMenuItem.Image = Resources.GrassAll;
            this.QuickGrassToolStripMenuItem.Name = "QuickGrassToolStripMenuItem";
            this.QuickGrassToolStripMenuItem.ShortcutKeys = ((Keys)((Keys.Control | Keys.G)));
            this.QuickGrassToolStripMenuItem.Size = new Size(251, 22);
            this.QuickGrassToolStripMenuItem.Text = "QuickGrass";
            this.QuickGrassToolStripMenuItem.Click += new EventHandler(this.QuickGrassToolStripMenuItemClick);
            // 
            // DeleteAllGrassToolStripMenuItem
            // 
            this.DeleteAllGrassToolStripMenuItem.Image = Resources.GrassDelete;
            this.DeleteAllGrassToolStripMenuItem.Name = "DeleteAllGrassToolStripMenuItem";
            this.DeleteAllGrassToolStripMenuItem.ShortcutKeys = ((Keys)((Keys.Control | Keys.D)));
            this.DeleteAllGrassToolStripMenuItem.Size = new Size(251, 22);
            this.DeleteAllGrassToolStripMenuItem.Text = "Delete all grass";
            this.DeleteAllGrassToolStripMenuItem.Click += new EventHandler(this.DeleteAllGrassToolStripMenuItemClick);
            // 
            // UndoToolStripMenuItem
            // 
            this.UndoToolStripMenuItem.Image = Resources.Undo;
            this.UndoToolStripMenuItem.Name = "UndoToolStripMenuItem";
            this.UndoToolStripMenuItem.ShortcutKeys = ((Keys)((Keys.Control | Keys.Z)));
            this.UndoToolStripMenuItem.Size = new Size(251, 22);
            this.UndoToolStripMenuItem.Text = "Undo";
            this.UndoToolStripMenuItem.Click += new EventHandler(this.Undo);
            // 
            // RedoToolStripMenuItem
            // 
            this.RedoToolStripMenuItem.Image = Resources.Redo;
            this.RedoToolStripMenuItem.Name = "RedoToolStripMenuItem";
            this.RedoToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.RedoToolStripMenuItem.ShortcutKeys = ((Keys)((Keys.Control | Keys.Y)));
            this.RedoToolStripMenuItem.Size = new Size(251, 22);
            this.RedoToolStripMenuItem.Text = "Redo";
            this.RedoToolStripMenuItem.Click += new EventHandler(this.Redo);
            // 
            // ToolStripSeparator8
            // 
            this.ToolStripSeparator8.Name = "ToolStripSeparator8";
            this.ToolStripSeparator8.Size = new Size(248, 6);
            // 
            // ZoomFillToolStripMenuItem
            // 
            this.ZoomFillToolStripMenuItem.Image = Resources.ZoomFill16;
            this.ZoomFillToolStripMenuItem.Name = "ZoomFillToolStripMenuItem";
            this.ZoomFillToolStripMenuItem.ShortcutKeys = Keys.F5;
            this.ZoomFillToolStripMenuItem.Size = new Size(251, 22);
            this.ZoomFillToolStripMenuItem.Text = "Zoom fill";
            this.ZoomFillToolStripMenuItem.Click += new EventHandler(this.ZoomFillToolStripMenuItemClick);
            // 
            // CheckTopologyMenuItem
            // 
            this.CheckTopologyMenuItem.Image = Resources.Topology;
            this.CheckTopologyMenuItem.Name = "CheckTopologyMenuItem";
            this.CheckTopologyMenuItem.ShortcutKeys = Keys.F6;
            this.CheckTopologyMenuItem.Size = new Size(251, 22);
            this.CheckTopologyMenuItem.Text = "Check topology";
            this.CheckTopologyMenuItem.Click += new EventHandler(this.CheckTopologyAndUpdate);
            // 
            // LevelPropertiesToolStripMenuItem
            // 
            this.LevelPropertiesToolStripMenuItem.Name = "LevelPropertiesToolStripMenuItem";
            this.LevelPropertiesToolStripMenuItem.ShortcutKeys = Keys.F4;
            this.LevelPropertiesToolStripMenuItem.Size = new Size(251, 22);
            this.LevelPropertiesToolStripMenuItem.Text = "Level properties";
            this.LevelPropertiesToolStripMenuItem.Click += new EventHandler(this.LevelPropertiesToolStripMenuItemClick);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new Size(248, 6);
            // 
            // previousLevelToolStripMenuItem
            // 
            this.previousLevelToolStripMenuItem.Image = Resources.Previous;
            this.previousLevelToolStripMenuItem.Name = "previousLevelToolStripMenuItem";
            this.previousLevelToolStripMenuItem.ShortcutKeys = Keys.F2;
            this.previousLevelToolStripMenuItem.Size = new Size(251, 22);
            this.previousLevelToolStripMenuItem.Text = "Previous level";
            // 
            // nextLevelToolStripMenuItem
            // 
            this.nextLevelToolStripMenuItem.Image = Resources.Next;
            this.nextLevelToolStripMenuItem.Name = "nextLevelToolStripMenuItem";
            this.nextLevelToolStripMenuItem.ShortcutKeys = Keys.F3;
            this.nextLevelToolStripMenuItem.Size = new Size(251, 22);
            this.nextLevelToolStripMenuItem.Text = "Next Level";
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new Size(248, 6);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.ShortcutKeys = ((Keys)((Keys.Control | Keys.A)));
            this.selectAllToolStripMenuItem.Size = new Size(251, 22);
            this.selectAllToolStripMenuItem.Text = "Select all";
            this.selectAllToolStripMenuItem.Click += new EventHandler(this.SelectAllToolStripMenuItemClick);
            // 
            // importLevelsToolStripMenuItem
            // 
            this.importLevelsToolStripMenuItem.Name = "importLevelsToolStripMenuItem";
            this.importLevelsToolStripMenuItem.ShortcutKeys = ((Keys)((Keys.Control | Keys.V)));
            this.importLevelsToolStripMenuItem.Size = new Size(251, 22);
            this.importLevelsToolStripMenuItem.Text = "Import level(s)/image(s)...";
            this.importLevelsToolStripMenuItem.Click += new EventHandler(this.importLevelsToolStripMenuItem_Click);
            // 
            // selectionToolStripMenuItem
            // 
            this.selectionToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
            this.toolStripMenuItem1,
            this.copyAndSnapToGridMenuItem,
            this.MirrorHorizontallyToolStripMenuItem,
            this.MirrorVerticallyToolStripMenuItem,
            this.DeleteSelectedMenuItem,
            this.toolStripSeparator14,
            this.unionToolStripMenuItem,
            this.differenceToolStripMenuItem,
            this.intersectionToolStripMenuItem,
            this.symmetricDifferenceToolStripMenuItem,
            this.toolStripSeparator15,
            this.texturizeMenuItem});
            this.selectionToolStripMenuItem.Name = "selectionToolStripMenuItem";
            this.selectionToolStripMenuItem.Size = new Size(67, 20);
            this.selectionToolStripMenuItem.Text = "Selection";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.ShortcutKeys = ((Keys)((Keys.Control | Keys.C)));
            this.toolStripMenuItem1.Size = new Size(265, 22);
            this.toolStripMenuItem1.Text = "Copy";
            this.toolStripMenuItem1.Click += new EventHandler(this.CopyMenuItemClick);
            // 
            // copyAndSnapToGridMenuItem
            // 
            this.copyAndSnapToGridMenuItem.Name = "copyAndSnapToGridMenuItem";
            this.copyAndSnapToGridMenuItem.ShortcutKeys = ((Keys)(((Keys.Control | Keys.Shift) 
                                                                   | Keys.C)));
            this.copyAndSnapToGridMenuItem.Size = new Size(265, 22);
            this.copyAndSnapToGridMenuItem.Text = "Copy and snap to grid";
            this.copyAndSnapToGridMenuItem.Click += new EventHandler(this.CopyMenuItemClick);
            // 
            // MirrorHorizontallyToolStripMenuItem
            // 
            this.MirrorHorizontallyToolStripMenuItem.Image = Resources.Mirror16;
            this.MirrorHorizontallyToolStripMenuItem.Name = "MirrorHorizontallyToolStripMenuItem";
            this.MirrorHorizontallyToolStripMenuItem.ShortcutKeys = ((Keys)((Keys.Control | Keys.M)));
            this.MirrorHorizontallyToolStripMenuItem.Size = new Size(265, 22);
            this.MirrorHorizontallyToolStripMenuItem.Text = "Mirror horizontally";
            this.MirrorHorizontallyToolStripMenuItem.Click += new EventHandler(this.MirrorHorizontallyToolStripMenuItem_Click);
            // 
            // MirrorVerticallyToolStripMenuItem
            // 
            this.MirrorVerticallyToolStripMenuItem.Image = Resources.Mirror16;
            this.MirrorVerticallyToolStripMenuItem.Name = "MirrorVerticallyToolStripMenuItem";
            this.MirrorVerticallyToolStripMenuItem.ShortcutKeys = ((Keys)(((Keys.Control | Keys.Shift) 
                                                                           | Keys.M)));
            this.MirrorVerticallyToolStripMenuItem.Size = new Size(265, 22);
            this.MirrorVerticallyToolStripMenuItem.Text = "Mirror vertically";
            this.MirrorVerticallyToolStripMenuItem.Click += new EventHandler(this.MirrorVerticallyToolStripMenuItem_Click);
            // 
            // DeleteSelectedMenuItem
            // 
            this.DeleteSelectedMenuItem.Image = Resources.Delete;
            this.DeleteSelectedMenuItem.Name = "DeleteSelectedMenuItem";
            this.DeleteSelectedMenuItem.ShortcutText = "Del";
            this.DeleteSelectedMenuItem.Size = new Size(265, 22);
            this.DeleteSelectedMenuItem.Text = "Delete";
            this.DeleteSelectedMenuItem.Click += new EventHandler(this.DeleteSelected);
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            this.toolStripSeparator14.Size = new Size(262, 6);
            // 
            // unionToolStripMenuItem
            // 
            this.unionToolStripMenuItem.Name = "unionToolStripMenuItem";
            this.unionToolStripMenuItem.ShortcutText = ",";
            this.unionToolStripMenuItem.Size = new Size(265, 22);
            this.unionToolStripMenuItem.Text = "Union";
            this.unionToolStripMenuItem.Click += new EventHandler(this.unionToolStripMenuItem_Click);
            // 
            // differenceToolStripMenuItem
            // 
            this.differenceToolStripMenuItem.Name = "differenceToolStripMenuItem";
            this.differenceToolStripMenuItem.ShortcutText = ".";
            this.differenceToolStripMenuItem.Size = new Size(265, 22);
            this.differenceToolStripMenuItem.Text = "Difference";
            this.differenceToolStripMenuItem.Click += new EventHandler(this.differenceToolStripMenuItem_Click);
            // 
            // intersectionToolStripMenuItem
            // 
            this.intersectionToolStripMenuItem.Name = "intersectionToolStripMenuItem";
            this.intersectionToolStripMenuItem.ShortcutText = "Enter";
            this.intersectionToolStripMenuItem.Size = new Size(265, 22);
            this.intersectionToolStripMenuItem.Text = "Intersection";
            this.intersectionToolStripMenuItem.Click += new EventHandler(this.intersectionToolStripMenuItem_Click);
            // 
            // symmetricDifferenceToolStripMenuItem
            // 
            this.symmetricDifferenceToolStripMenuItem.Name = "symmetricDifferenceToolStripMenuItem";
            this.symmetricDifferenceToolStripMenuItem.ShortcutText = "\'";
            this.symmetricDifferenceToolStripMenuItem.Size = new Size(265, 22);
            this.symmetricDifferenceToolStripMenuItem.Text = "Symmetric difference";
            this.symmetricDifferenceToolStripMenuItem.Click += new EventHandler(this.symmetricDifferenceToolStripMenuItem_Click);
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            this.toolStripSeparator15.Size = new Size(262, 6);
            // 
            // texturizeMenuItem
            // 
            this.texturizeMenuItem.Name = "texturizeMenuItem";
            this.texturizeMenuItem.ShortcutText = "§";
            this.texturizeMenuItem.Size = new Size(265, 22);
            this.texturizeMenuItem.Text = "Texturize";
            this.texturizeMenuItem.Click += new EventHandler(this.texturizeMenuItem_Click);
            // 
            // SelectionFilterToolStripMenuItem
            // 
            this.SelectionFilterToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
            this.EnableAllToolStripMenuItem,
            this.DisableAllToolStripMenuItem,
            this.GroundPolygonsToolStripMenuItem,
            this.GrassPolygonsToolStripMenuItem,
            this.ApplesToolStripMenuItem,
            this.KillersToolStripMenuItem,
            this.FlowersToolStripMenuItem,
            this.PicturesToolStripMenuItem,
            this.TexturesToolStripMenuItem});
            this.SelectionFilterToolStripMenuItem.Name = "SelectionFilterToolStripMenuItem";
            this.SelectionFilterToolStripMenuItem.Size = new Size(94, 20);
            this.SelectionFilterToolStripMenuItem.Text = "Selection filter";
            // 
            // EnableAllToolStripMenuItem
            // 
            this.EnableAllToolStripMenuItem.Name = "EnableAllToolStripMenuItem";
            this.EnableAllToolStripMenuItem.Size = new Size(166, 22);
            this.EnableAllToolStripMenuItem.Text = "Enable all";
            this.EnableAllToolStripMenuItem.Click += new EventHandler(this.SetAllFilters);
            // 
            // DisableAllToolStripMenuItem
            // 
            this.DisableAllToolStripMenuItem.Name = "DisableAllToolStripMenuItem";
            this.DisableAllToolStripMenuItem.Size = new Size(166, 22);
            this.DisableAllToolStripMenuItem.Text = "Disable all";
            this.DisableAllToolStripMenuItem.Click += new EventHandler(this.SetAllFilters);
            // 
            // GroundPolygonsToolStripMenuItem
            // 
            this.GroundPolygonsToolStripMenuItem.Checked = true;
            this.GroundPolygonsToolStripMenuItem.CheckOnClick = true;
            this.GroundPolygonsToolStripMenuItem.CheckState = CheckState.Checked;
            this.GroundPolygonsToolStripMenuItem.Name = "GroundPolygonsToolStripMenuItem";
            this.GroundPolygonsToolStripMenuItem.Size = new Size(166, 22);
            this.GroundPolygonsToolStripMenuItem.Text = "Ground polygons";
            this.GroundPolygonsToolStripMenuItem.CheckedChanged += new EventHandler(this.FilterChanged);
            // 
            // GrassPolygonsToolStripMenuItem
            // 
            this.GrassPolygonsToolStripMenuItem.Checked = true;
            this.GrassPolygonsToolStripMenuItem.CheckOnClick = true;
            this.GrassPolygonsToolStripMenuItem.CheckState = CheckState.Checked;
            this.GrassPolygonsToolStripMenuItem.Name = "GrassPolygonsToolStripMenuItem";
            this.GrassPolygonsToolStripMenuItem.Size = new Size(166, 22);
            this.GrassPolygonsToolStripMenuItem.Text = "Grass polygons";
            this.GrassPolygonsToolStripMenuItem.CheckedChanged += new EventHandler(this.FilterChanged);
            // 
            // ApplesToolStripMenuItem
            // 
            this.ApplesToolStripMenuItem.Checked = true;
            this.ApplesToolStripMenuItem.CheckOnClick = true;
            this.ApplesToolStripMenuItem.CheckState = CheckState.Checked;
            this.ApplesToolStripMenuItem.Name = "ApplesToolStripMenuItem";
            this.ApplesToolStripMenuItem.Size = new Size(166, 22);
            this.ApplesToolStripMenuItem.Text = "Apples";
            this.ApplesToolStripMenuItem.CheckedChanged += new EventHandler(this.FilterChanged);
            // 
            // KillersToolStripMenuItem
            // 
            this.KillersToolStripMenuItem.Checked = true;
            this.KillersToolStripMenuItem.CheckOnClick = true;
            this.KillersToolStripMenuItem.CheckState = CheckState.Checked;
            this.KillersToolStripMenuItem.Name = "KillersToolStripMenuItem";
            this.KillersToolStripMenuItem.Size = new Size(166, 22);
            this.KillersToolStripMenuItem.Text = "Killers";
            this.KillersToolStripMenuItem.CheckedChanged += new EventHandler(this.FilterChanged);
            // 
            // FlowersToolStripMenuItem
            // 
            this.FlowersToolStripMenuItem.Checked = true;
            this.FlowersToolStripMenuItem.CheckOnClick = true;
            this.FlowersToolStripMenuItem.CheckState = CheckState.Checked;
            this.FlowersToolStripMenuItem.Name = "FlowersToolStripMenuItem";
            this.FlowersToolStripMenuItem.Size = new Size(166, 22);
            this.FlowersToolStripMenuItem.Text = "Flowers";
            this.FlowersToolStripMenuItem.CheckedChanged += new EventHandler(this.FilterChanged);
            // 
            // PicturesToolStripMenuItem
            // 
            this.PicturesToolStripMenuItem.Checked = true;
            this.PicturesToolStripMenuItem.CheckOnClick = true;
            this.PicturesToolStripMenuItem.CheckState = CheckState.Checked;
            this.PicturesToolStripMenuItem.Name = "PicturesToolStripMenuItem";
            this.PicturesToolStripMenuItem.Size = new Size(166, 22);
            this.PicturesToolStripMenuItem.Text = "Pictures";
            this.PicturesToolStripMenuItem.CheckedChanged += new EventHandler(this.FilterChanged);
            // 
            // TexturesToolStripMenuItem
            // 
            this.TexturesToolStripMenuItem.Checked = true;
            this.TexturesToolStripMenuItem.CheckOnClick = true;
            this.TexturesToolStripMenuItem.CheckState = CheckState.Checked;
            this.TexturesToolStripMenuItem.Name = "TexturesToolStripMenuItem";
            this.TexturesToolStripMenuItem.Size = new Size(166, 22);
            this.TexturesToolStripMenuItem.Text = "Textures";
            this.TexturesToolStripMenuItem.CheckedChanged += new EventHandler(this.FilterChanged);
            // 
            // ConfigurationToolStripMenuItem
            // 
            this.ConfigurationToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
            this.MainConfigMenuItem,
            this.RenderingSettingsToolStripMenuItem});
            this.ConfigurationToolStripMenuItem.Name = "ConfigurationToolStripMenuItem";
            this.ConfigurationToolStripMenuItem.Size = new Size(93, 20);
            this.ConfigurationToolStripMenuItem.Text = "Configuration";
            // 
            // MainConfigMenuItem
            // 
            this.MainConfigMenuItem.Name = "MainConfigMenuItem";
            this.MainConfigMenuItem.ShortcutKeys = Keys.F7;
            this.MainConfigMenuItem.Size = new Size(191, 22);
            this.MainConfigMenuItem.Text = "Main";
            this.MainConfigMenuItem.Click += new EventHandler(this.OpenConfig);
            // 
            // RenderingSettingsToolStripMenuItem
            // 
            this.RenderingSettingsToolStripMenuItem.Name = "RenderingSettingsToolStripMenuItem";
            this.RenderingSettingsToolStripMenuItem.ShortcutKeys = Keys.F8;
            this.RenderingSettingsToolStripMenuItem.Size = new Size(191, 22);
            this.RenderingSettingsToolStripMenuItem.Text = "Rendering settings";
            this.RenderingSettingsToolStripMenuItem.Click += new EventHandler(this.OpenRenderingSettings);
            // 
            // EditorControl
            // 
            this.EditorControl.AllowDrop = true;
            this.EditorControl.API = OpenTK.Windowing.Common.ContextAPI.OpenGL;
            this.EditorControl.APIVersion = new System.Version(3, 3, 0, 0);
            this.EditorControl.Dock = DockStyle.Fill;
            this.EditorControl.Flags = OpenTK.Windowing.Common.ContextFlags.Default;
            this.EditorControl.IsEventDriven = true;
            this.EditorControl.Location = new Point(84, 136);
            this.EditorControl.Name = "EditorControl";
            this.EditorControl.Profile = OpenTK.Windowing.Common.ContextProfile.Compatability;
            this.EditorControl.Size = new Size(845, 350);
            this.EditorControl.TabIndex = 2;
            this.EditorControl.DragDrop += new DragEventHandler(this.ItemsDropped);
            this.EditorControl.DragEnter += new DragEventHandler(this.StartingDrop);
            this.EditorControl.DragOver += new DragEventHandler(this.EditorControl_DragOver);
            this.EditorControl.DragLeave += new EventHandler(this.EditorControl_DragLeave);
            this.EditorControl.MouseDown += new MouseEventHandler(this.MouseDownEvent);
            this.EditorControl.MouseLeave += new EventHandler(this.MouseLeaveEvent);
            this.EditorControl.MouseMove += new MouseEventHandler(this.MouseMoveEvent);
            this.EditorControl.MouseUp += new MouseEventHandler(this.MouseUpEvent);
            // 
            // OpenFileDialog1
            // 
            this.OpenFileDialog1.Filter = "Elasto Mania level (*.lev, *.leb)|*.lev;*.leb";
            // 
            // StatusStrip1
            // 
            this.StatusStrip1.AutoSize = false;
            this.StatusStrip1.GripMargin = new Padding(0);
            this.StatusStrip1.ImageScalingSize = new Size(32, 32);
            this.StatusStrip1.Items.AddRange(new ToolStripItem[] {
            this.CoordinateLabel,
            this.SelectionLabel,
            this.HighlightLabel});
            this.StatusStrip1.Location = new Point(0, 486);
            this.StatusStrip1.Name = "StatusStrip1";
            this.StatusStrip1.ShowItemToolTips = true;
            this.StatusStrip1.Size = new Size(929, 23);
            this.StatusStrip1.SizingGrip = false;
            this.StatusStrip1.TabIndex = 4;
            this.StatusStrip1.Text = "StatusStrip1";
            // 
            // CoordinateLabel
            // 
            this.CoordinateLabel.AutoSize = false;
            this.CoordinateLabel.BorderSides = ((ToolStripStatusLabelBorderSides)((((ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Top) 
            | ToolStripStatusLabelBorderSides.Right) 
            | ToolStripStatusLabelBorderSides.Bottom)));
            this.CoordinateLabel.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.CoordinateLabel.Name = "CoordinateLabel";
            this.CoordinateLabel.Size = new Size(190, 18);
            this.CoordinateLabel.Text = "Mouse X: Y:";
            this.CoordinateLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // SelectionLabel
            // 
            this.SelectionLabel.AutoSize = false;
            this.SelectionLabel.BorderSides = ((ToolStripStatusLabelBorderSides)((((ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Top) 
            | ToolStripStatusLabelBorderSides.Right) 
            | ToolStripStatusLabelBorderSides.Bottom)));
            this.SelectionLabel.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.SelectionLabel.Name = "SelectionLabel";
            this.SelectionLabel.Size = new Size(420, 18);
            this.SelectionLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // HighlightLabel
            // 
            this.HighlightLabel.AutoSize = false;
            this.HighlightLabel.BorderSides = ((ToolStripStatusLabelBorderSides)((((ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Top) 
            | ToolStripStatusLabelBorderSides.Right) 
            | ToolStripStatusLabelBorderSides.Bottom)));
            this.HighlightLabel.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.HighlightLabel.Name = "HighlightLabel";
            this.HighlightLabel.Size = new Size(304, 18);
            this.HighlightLabel.Spring = true;
            this.HighlightLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // SaveFileDialog1
            // 
            this.SaveFileDialog1.DefaultExt = "lev";
            this.SaveFileDialog1.FileName = "Untitled";
            this.SaveFileDialog1.Filter = "Elasto Mania level (*.lev)|*.lev";
            // 
            // ToolStripPanel1
            // 
            this.ToolStripPanel1.BackColor = SystemColors.Control;
            this.ToolStripPanel1.Controls.Add(this.ToolStrip1);
            this.ToolStripPanel1.Controls.Add(this.ToolStrip2);
            this.ToolStripPanel1.Controls.Add(this.toolStrip3);
            this.ToolStripPanel1.Dock = DockStyle.Top;
            this.ToolStripPanel1.ForeColor = SystemColors.ControlText;
            this.ToolStripPanel1.Location = new Point(0, 24);
            this.ToolStripPanel1.Name = "ToolStripPanel1";
            this.ToolStripPanel1.Orientation = Orientation.Horizontal;
            this.ToolStripPanel1.RowMargin = new Padding(0, 3, 0, 0);
            this.ToolStripPanel1.Size = new Size(929, 112);
            // 
            // ToolStrip1
            // 
            this.ToolStrip1.BackColor = SystemColors.Control;
            this.ToolStrip1.Dock = DockStyle.None;
            this.ToolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            this.ToolStrip1.ImageScalingSize = new Size(32, 32);
            this.ToolStrip1.Items.AddRange(new ToolStripItem[] {
            this.NewButton,
            this.OpenButton,
            this.SaveButton,
            this.SaveAsButton,
            this.deleteButton,
            this.ToolStripSeparator1,
            this.CheckTopologyButton,
            this.ZoomFillButton,
            this.ToolStripSeparator2,
            this.UndoButton,
            this.RedoButton,
            this.ToolStripSeparator3,
            this.PreviousButton,
            this.NextButton,
            this.toolStripSeparator13,
            this.toolStripLabel5,
            this.filenameBox,
            this.filenameOkButton,
            this.filenameCancelButton,
            this.toolStripSeparator9,
            this.ToolStripLabel1,
            this.TitleBox,
            this.ToolStripSeparator4,
            this.ToolStripLabel2,
            this.LGRBox,
            this.ToolStripSeparator5,
            this.ToolStripLabel3,
            this.GroundComboBox,
            this.ToolStripSeparator6,
            this.ToolStripLabel4,
            this.SkyComboBox,
            this.ToolStripSeparator7});
            this.ToolStrip1.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.ToolStrip1.Location = new Point(0, 3);
            this.ToolStrip1.Name = "ToolStrip1";
            this.ToolStrip1.Size = new Size(929, 39);
            this.ToolStrip1.TabIndex = 14;
            this.ToolStrip1.Text = "ToolStrip1";
            // 
            // NewButton
            // 
            this.NewButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.NewButton.Image = Resources._New;
            this.NewButton.ImageTransparentColor = Color.Magenta;
            this.NewButton.Name = "NewButton";
            this.NewButton.Size = new Size(36, 36);
            this.NewButton.Text = "New";
            this.NewButton.Click += new EventHandler(this.NewLevel);
            // 
            // OpenButton
            // 
            this.OpenButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.OpenButton.Image = Resources.Open;
            this.OpenButton.ImageTransparentColor = Color.Magenta;
            this.OpenButton.Name = "OpenButton";
            this.OpenButton.Size = new Size(36, 36);
            this.OpenButton.Text = "Open";
            this.OpenButton.Click += new EventHandler(this.OpenToolStripMenuItemClick);
            // 
            // SaveButton
            // 
            this.SaveButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.SaveButton.Enabled = false;
            this.SaveButton.Image = Resources.Save;
            this.SaveButton.ImageTransparentColor = Color.Magenta;
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new Size(36, 36);
            this.SaveButton.Text = "Save";
            this.SaveButton.Click += new EventHandler(this.SaveClicked);
            // 
            // SaveAsButton
            // 
            this.SaveAsButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.SaveAsButton.Image = Resources.SaveAs;
            this.SaveAsButton.ImageTransparentColor = Color.Magenta;
            this.SaveAsButton.Name = "SaveAsButton";
            this.SaveAsButton.Size = new Size(36, 36);
            this.SaveAsButton.Text = "Save as...";
            this.SaveAsButton.Click += new EventHandler(this.SaveAs);
            // 
            // deleteButton
            // 
            this.deleteButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.deleteButton.Image = Resources.Delete;
            this.deleteButton.ImageTransparentColor = Color.Magenta;
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new Size(36, 36);
            this.deleteButton.Text = "toolStripButton1";
            this.deleteButton.ToolTipText = "Delete this level";
            this.deleteButton.Click += new EventHandler(this.deleteButton_Click);
            // 
            // ToolStripSeparator1
            // 
            this.ToolStripSeparator1.Name = "ToolStripSeparator1";
            this.ToolStripSeparator1.Size = new Size(6, 39);
            // 
            // CheckTopologyButton
            // 
            this.CheckTopologyButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.CheckTopologyButton.Image = Resources.Topology;
            this.CheckTopologyButton.ImageTransparentColor = Color.Magenta;
            this.CheckTopologyButton.Name = "CheckTopologyButton";
            this.CheckTopologyButton.Size = new Size(36, 36);
            this.CheckTopologyButton.Text = "Check topology";
            this.CheckTopologyButton.Click += new EventHandler(this.CheckTopologyAndUpdate);
            // 
            // ZoomFillButton
            // 
            this.ZoomFillButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.ZoomFillButton.Image = Resources.ZoomFill;
            this.ZoomFillButton.ImageTransparentColor = Color.Magenta;
            this.ZoomFillButton.Name = "ZoomFillButton";
            this.ZoomFillButton.Size = new Size(36, 36);
            this.ZoomFillButton.Text = "Zoom fill";
            // 
            // ToolStripSeparator2
            // 
            this.ToolStripSeparator2.Name = "ToolStripSeparator2";
            this.ToolStripSeparator2.Size = new Size(6, 39);
            // 
            // UndoButton
            // 
            this.UndoButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.UndoButton.Image = Resources.Undo;
            this.UndoButton.ImageTransparentColor = Color.Magenta;
            this.UndoButton.Name = "UndoButton";
            this.UndoButton.Size = new Size(36, 36);
            this.UndoButton.Text = "Undo";
            this.UndoButton.Click += new EventHandler(this.Undo);
            // 
            // RedoButton
            // 
            this.RedoButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.RedoButton.Image = Resources.Redo;
            this.RedoButton.ImageTransparentColor = Color.Magenta;
            this.RedoButton.Name = "RedoButton";
            this.RedoButton.Size = new Size(36, 36);
            this.RedoButton.Text = "Redo";
            this.RedoButton.Click += new EventHandler(this.Redo);
            // 
            // ToolStripSeparator3
            // 
            this.ToolStripSeparator3.Name = "ToolStripSeparator3";
            this.ToolStripSeparator3.Size = new Size(6, 39);
            // 
            // PreviousButton
            // 
            this.PreviousButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.PreviousButton.Image = Resources.Previous;
            this.PreviousButton.ImageTransparentColor = Color.Magenta;
            this.PreviousButton.Name = "PreviousButton";
            this.PreviousButton.Size = new Size(36, 36);
            this.PreviousButton.Text = "Previous level";
            this.PreviousButton.Click += new EventHandler(this.PrevNextButtonClick);
            // 
            // NextButton
            // 
            this.NextButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.NextButton.Image = Resources.Next;
            this.NextButton.ImageTransparentColor = Color.Magenta;
            this.NextButton.Name = "NextButton";
            this.NextButton.Size = new Size(36, 36);
            this.NextButton.Text = "Next level";
            this.NextButton.Click += new EventHandler(this.PrevNextButtonClick);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Size = new Size(6, 39);
            // 
            // toolStripLabel5
            // 
            this.toolStripLabel5.Name = "toolStripLabel5";
            this.toolStripLabel5.Size = new Size(58, 36);
            this.toolStripLabel5.Text = "Filename:";
            // 
            // filenameBox
            // 
            this.filenameBox.AutoSize = false;
            this.filenameBox.BorderStyle = BorderStyle.FixedSingle;
            this.filenameBox.Name = "filenameBox";
            this.filenameBox.Size = new Size(100, 23);
            this.filenameBox.KeyDown += new KeyEventHandler(this.filenameBox_KeyDown);
            this.filenameBox.TextChanged += new EventHandler(this.filenameBox_TextChanged);
            // 
            // filenameOkButton
            // 
            this.filenameOkButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.filenameOkButton.ImageTransparentColor = Color.Magenta;
            this.filenameOkButton.Name = "filenameOkButton";
            this.filenameOkButton.Size = new Size(27, 36);
            this.filenameOkButton.Text = "OK";
            this.filenameOkButton.Visible = false;
            this.filenameOkButton.Click += new EventHandler(this.filenameOkButton_Click);
            // 
            // filenameCancelButton
            // 
            this.filenameCancelButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.filenameCancelButton.ImageTransparentColor = Color.Magenta;
            this.filenameCancelButton.Name = "filenameCancelButton";
            this.filenameCancelButton.Size = new Size(47, 36);
            this.filenameCancelButton.Text = "Cancel";
            this.filenameCancelButton.Visible = false;
            this.filenameCancelButton.Click += new EventHandler(this.filenameCancelButton_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new Size(6, 39);
            // 
            // ToolStripLabel1
            // 
            this.ToolStripLabel1.Name = "ToolStripLabel1";
            this.ToolStripLabel1.Size = new Size(32, 36);
            this.ToolStripLabel1.Text = "Title:";
            // 
            // TitleBox
            // 
            this.TitleBox.AutoSize = false;
            this.TitleBox.BorderStyle = BorderStyle.FixedSingle;
            this.TitleBox.MaxLength = 50;
            this.TitleBox.Name = "TitleBox";
            this.TitleBox.Size = new Size(120, 23);
            this.TitleBox.TextChanged += new EventHandler(this.TitleBoxTextChanged);
            // 
            // ToolStripSeparator4
            // 
            this.ToolStripSeparator4.Name = "ToolStripSeparator4";
            this.ToolStripSeparator4.Size = new Size(6, 39);
            // 
            // ToolStripLabel2
            // 
            this.ToolStripLabel2.Name = "ToolStripLabel2";
            this.ToolStripLabel2.Size = new Size(52, 36);
            this.ToolStripLabel2.Text = "LGR File:";
            // 
            // LGRBox
            // 
            this.LGRBox.AutoSize = false;
            this.LGRBox.BorderStyle = BorderStyle.FixedSingle;
            this.LGRBox.MaxLength = 8;
            this.LGRBox.Name = "LGRBox";
            this.LGRBox.Size = new Size(119, 23);
            // 
            // ToolStripSeparator5
            // 
            this.ToolStripSeparator5.Name = "ToolStripSeparator5";
            this.ToolStripSeparator5.Size = new Size(6, 39);
            // 
            // ToolStripLabel3
            // 
            this.ToolStripLabel3.Name = "ToolStripLabel3";
            this.ToolStripLabel3.Size = new Size(50, 15);
            this.ToolStripLabel3.Text = "Ground:";
            // 
            // GroundComboBox
            // 
            this.GroundComboBox.AutoSize = false;
            this.GroundComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this.GroundComboBox.Name = "GroundComboBox";
            this.GroundComboBox.Size = new Size(119, 23);
            this.GroundComboBox.DropDownClosed += new EventHandler(this.MoveFocus);
            this.GroundComboBox.KeyDown += new KeyEventHandler(this.KeyHandlerDown);
            // 
            // ToolStripSeparator6
            // 
            this.ToolStripSeparator6.Name = "ToolStripSeparator6";
            this.ToolStripSeparator6.Size = new Size(6, 39);
            // 
            // ToolStripLabel4
            // 
            this.ToolStripLabel4.Name = "ToolStripLabel4";
            this.ToolStripLabel4.Size = new Size(28, 15);
            this.ToolStripLabel4.Text = "Sky:";
            // 
            // SkyComboBox
            // 
            this.SkyComboBox.AutoSize = false;
            this.SkyComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this.SkyComboBox.Name = "SkyComboBox";
            this.SkyComboBox.Size = new Size(119, 23);
            this.SkyComboBox.DropDownClosed += new EventHandler(this.MoveFocus);
            this.SkyComboBox.KeyDown += new KeyEventHandler(this.KeyHandlerDown);
            // 
            // ToolStripSeparator7
            // 
            this.ToolStripSeparator7.Name = "ToolStripSeparator7";
            this.ToolStripSeparator7.Size = new Size(6, 39);
            // 
            // ToolStrip2
            // 
            this.ToolStrip2.BackColor = SystemColors.Control;
            this.ToolStrip2.Dock = DockStyle.None;
            this.ToolStrip2.GripStyle = ToolStripGripStyle.Hidden;
            this.ToolStrip2.ImageScalingSize = new Size(32, 32);
            this.ToolStrip2.Items.AddRange(new ToolStripItem[] {
            this.ShowGridButton,
            this.snapToGridButton,
            this.showCrossHairButton,
            this.ShowGrassEdgesButton,
            this.ShowGroundEdgesButton,
            this.ShowVerticesButton,
            this.ShowTextureFramesButton,
            this.ShowPictureFramesButton,
            this.ShowTexturesButton,
            this.ShowPicturesButton,
            this.ShowObjectFramesButton,
            this.ShowObjectsButton,
            this.ShowGravityAppleArrowsButton,
            this.ShowGroundButton,
            this.ShowGroundTextureButton,
            this.ShowSkyTextureButton,
            this.ZoomTexturesButton,
            this.toolStripSeparator12,
            this.BestTimeLabel,
            this.topologyList,
            this.toolStripSeparator16,
            this.playButton,
            this.stopButton,
            this.settingsButton,
            this.PlayTimeLabel,
            });
            this.ToolStrip2.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.ToolStrip2.Location = new Point(0, 45);
            this.ToolStrip2.Name = "ToolStrip2";
            this.ToolStrip2.Size = new Size(865, 39);
            this.ToolStrip2.TabIndex = 15;
            this.ToolStrip2.Text = "ToolStrip2";
            // 
            // ShowGridButton
            // 
            this.ShowGridButton.CheckOnClick = true;
            this.ShowGridButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.ShowGridButton.Image = Resources.Grid;
            this.ShowGridButton.ImageTransparentColor = Color.Magenta;
            this.ShowGridButton.Name = "ShowGridButton";
            this.ShowGridButton.Size = new Size(36, 36);
            this.ShowGridButton.Text = "S&how grid";
            // 
            // snapToGridButton
            // 
            this.snapToGridButton.CheckOnClick = true;
            this.snapToGridButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.snapToGridButton.Image = Resources.Snap;
            this.snapToGridButton.ImageTransparentColor = Color.Magenta;
            this.snapToGridButton.Name = "snapToGridButton";
            this.snapToGridButton.Size = new Size(36, 36);
            this.snapToGridButton.Text = "Snap to grid";
            // 
            // showCrossHairButton
            // 
            this.showCrossHairButton.CheckOnClick = true;
            this.showCrossHairButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.showCrossHairButton.Image = Resources.Crosshair2;
            this.showCrossHairButton.ImageTransparentColor = Color.Magenta;
            this.showCrossHairButton.Name = "showCrossHairButton";
            this.showCrossHairButton.Size = new Size(36, 36);
            this.showCrossHairButton.Text = "Show crosshair";
            // 
            // ShowGrassEdgesButton
            // 
            this.ShowGrassEdgesButton.CheckOnClick = true;
            this.ShowGrassEdgesButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.ShowGrassEdgesButton.Image = Resources.GrassEdges;
            this.ShowGrassEdgesButton.ImageTransparentColor = Color.Magenta;
            this.ShowGrassEdgesButton.Name = "ShowGrassEdgesButton";
            this.ShowGrassEdgesButton.Size = new Size(36, 36);
            this.ShowGrassEdgesButton.Text = "Show grass edges";
            // 
            // ShowGroundEdgesButton
            // 
            this.ShowGroundEdgesButton.CheckOnClick = true;
            this.ShowGroundEdgesButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.ShowGroundEdgesButton.Image = Resources.Edges;
            this.ShowGroundEdgesButton.ImageTransparentColor = Color.Magenta;
            this.ShowGroundEdgesButton.Name = "ShowGroundEdgesButton";
            this.ShowGroundEdgesButton.Size = new Size(36, 36);
            this.ShowGroundEdgesButton.Text = "Show ground edges";
            // 
            // ShowVerticesButton
            // 
            this.ShowVerticesButton.CheckOnClick = true;
            this.ShowVerticesButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.ShowVerticesButton.Image = Resources.Vertices;
            this.ShowVerticesButton.ImageTransparentColor = Color.Magenta;
            this.ShowVerticesButton.Name = "ShowVerticesButton";
            this.ShowVerticesButton.Size = new Size(36, 36);
            this.ShowVerticesButton.Text = "Show vertices";
            // 
            // ShowTextureFramesButton
            // 
            this.ShowTextureFramesButton.CheckOnClick = true;
            this.ShowTextureFramesButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.ShowTextureFramesButton.Image = Resources.TextureFrame;
            this.ShowTextureFramesButton.ImageTransparentColor = Color.Magenta;
            this.ShowTextureFramesButton.Name = "ShowTextureFramesButton";
            this.ShowTextureFramesButton.Size = new Size(36, 36);
            this.ShowTextureFramesButton.Text = "Show texture frames";
            // 
            // ShowPictureFramesButton
            // 
            this.ShowPictureFramesButton.CheckOnClick = true;
            this.ShowPictureFramesButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.ShowPictureFramesButton.Image = Resources.PictureFrame;
            this.ShowPictureFramesButton.ImageTransparentColor = Color.Magenta;
            this.ShowPictureFramesButton.Name = "ShowPictureFramesButton";
            this.ShowPictureFramesButton.Size = new Size(36, 36);
            this.ShowPictureFramesButton.Text = "Show picture frames";
            // 
            // ShowTexturesButton
            // 
            this.ShowTexturesButton.CheckOnClick = true;
            this.ShowTexturesButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.ShowTexturesButton.Image = Resources.Texture;
            this.ShowTexturesButton.ImageTransparentColor = Color.Magenta;
            this.ShowTexturesButton.Name = "ShowTexturesButton";
            this.ShowTexturesButton.Size = new Size(36, 36);
            this.ShowTexturesButton.Text = "Show textures";
            // 
            // ShowPicturesButton
            // 
            this.ShowPicturesButton.CheckOnClick = true;
            this.ShowPicturesButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.ShowPicturesButton.Image = Resources.Picture;
            this.ShowPicturesButton.ImageTransparentColor = Color.Magenta;
            this.ShowPicturesButton.Name = "ShowPicturesButton";
            this.ShowPicturesButton.Size = new Size(36, 36);
            this.ShowPicturesButton.Text = "Show pictures";
            // 
            // ShowObjectFramesButton
            // 
            this.ShowObjectFramesButton.CheckOnClick = true;
            this.ShowObjectFramesButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.ShowObjectFramesButton.Image = Resources.ObjectFrame;
            this.ShowObjectFramesButton.ImageTransparentColor = Color.Magenta;
            this.ShowObjectFramesButton.Name = "ShowObjectFramesButton";
            this.ShowObjectFramesButton.Size = new Size(36, 36);
            this.ShowObjectFramesButton.Text = "Show object frames";
            // 
            // ShowObjectsButton
            // 
            this.ShowObjectsButton.CheckOnClick = true;
            this.ShowObjectsButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.ShowObjectsButton.Image = Resources._Object;
            this.ShowObjectsButton.ImageTransparentColor = Color.Magenta;
            this.ShowObjectsButton.Name = "ShowObjectsButton";
            this.ShowObjectsButton.Size = new Size(36, 36);
            this.ShowObjectsButton.Text = "Show objects";
            // 
            // ShowGravityAppleArrowsButton
            // 
            this.ShowGravityAppleArrowsButton.CheckOnClick = true;
            this.ShowGravityAppleArrowsButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.ShowGravityAppleArrowsButton.Image = Resources.AppleArrow;
            this.ShowGravityAppleArrowsButton.ImageTransparentColor = Color.Magenta;
            this.ShowGravityAppleArrowsButton.Name = "ShowGravityAppleArrowsButton";
            this.ShowGravityAppleArrowsButton.Size = new Size(36, 36);
            this.ShowGravityAppleArrowsButton.Text = "Show gravity apple arrows";
            // 
            // ShowGroundButton
            // 
            this.ShowGroundButton.CheckOnClick = true;
            this.ShowGroundButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.ShowGroundButton.Image = Resources.GroundFill;
            this.ShowGroundButton.ImageTransparentColor = Color.Magenta;
            this.ShowGroundButton.Name = "ShowGroundButton";
            this.ShowGroundButton.Size = new Size(36, 36);
            this.ShowGroundButton.Text = "Show ground";
            // 
            // ShowGroundTextureButton
            // 
            this.ShowGroundTextureButton.CheckOnClick = true;
            this.ShowGroundTextureButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.ShowGroundTextureButton.Image = Resources.Ground;
            this.ShowGroundTextureButton.ImageTransparentColor = Color.Magenta;
            this.ShowGroundTextureButton.Name = "ShowGroundTextureButton";
            this.ShowGroundTextureButton.Size = new Size(36, 36);
            this.ShowGroundTextureButton.Text = "Show ground texture";
            // 
            // ShowSkyTextureButton
            // 
            this.ShowSkyTextureButton.CheckOnClick = true;
            this.ShowSkyTextureButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.ShowSkyTextureButton.Image = Resources.Sky;
            this.ShowSkyTextureButton.ImageTransparentColor = Color.Magenta;
            this.ShowSkyTextureButton.Name = "ShowSkyTextureButton";
            this.ShowSkyTextureButton.Size = new Size(36, 36);
            this.ShowSkyTextureButton.Text = "Show sky texture";
            // 
            // ZoomTexturesButton
            // 
            this.ZoomTexturesButton.CheckOnClick = true;
            this.ZoomTexturesButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.ZoomTexturesButton.Image = Resources.ZoomTexture;
            this.ZoomTexturesButton.ImageTransparentColor = Color.Magenta;
            this.ZoomTexturesButton.Name = "ZoomTexturesButton";
            this.ZoomTexturesButton.Size = new Size(36, 36);
            this.ZoomTexturesButton.Text = "Zoom textures";
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new Size(6, 39);
            // 
            // BestTimeLabel
            // 
            this.BestTimeLabel.AutoSize = false;
            this.BestTimeLabel.Name = "BestTimeLabel";
            this.BestTimeLabel.Size = new Size(170, 36);
            this.BestTimeLabel.Text = "Best time: None";
            this.BestTimeLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // PlayTimeLabel
            // 
            this.PlayTimeLabel.AutoSize = false;
            this.PlayTimeLabel.Name = "PlayTimeLabel";
            this.PlayTimeLabel.Size = new Size(170, 36);
            this.PlayTimeLabel.Text = "";
            this.PlayTimeLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // topologyList
            // 
            this.topologyList.AutoToolTip = false;
            this.topologyList.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.topologyList.ImageTransparentColor = Color.Magenta;
            this.topologyList.Name = "topologyList";
            this.topologyList.ShowDropDownArrow = false;
            this.topologyList.Size = new Size(4, 36);
            // 
            // toolStrip3
            // 
            this.toolStrip3.BackColor = SystemColors.Control;
            this.toolStrip3.Dock = DockStyle.None;
            this.toolStrip3.GripStyle = ToolStripGripStyle.Hidden;
            this.toolStrip3.Items.AddRange(new ToolStripItem[] {
            this.InfoLabel});
            this.toolStrip3.Location = new Point(0, 87);
            this.toolStrip3.Name = "toolStrip3";
            this.toolStrip3.Size = new Size(929, 25);
            this.toolStrip3.Stretch = true;
            this.toolStrip3.TabIndex = 17;
            // 
            // InfoLabel
            // 
            this.InfoLabel.BackColor = SystemColors.Control;
            this.InfoLabel.Name = "InfoLabel";
            this.InfoLabel.Size = new Size(42, 22);
            this.InfoLabel.Text = "Ready.";
            // 
            // EditorMenuStrip
            // 
            this.EditorMenuStrip.ImageScalingSize = new Size(15, 15);
            this.EditorMenuStrip.Items.AddRange(new ToolStripItem[] {
            this.CopyMenuItem,
            this.TransformMenuItem,
            this.DeleteMenuItem,
            this.GrassMenuItem,
            this.GravityNoneMenuItem,
            this.GravityUpMenuItem,
            this.GravityDownMenuItem,
            this.GravityLeftMenuItem,
            this.GravityRightMenuItem,
            this.PicturePropertiesMenuItem,
            this.bringToFrontToolStripMenuItem,
            this.sendToBackToolStripMenuItem,
            this.convertToToolStripMenuItem,
            this.saveStartPositionToolStripMenuItem,
            this.restoreStartPositionToolStripMenuItem,
            this.moveStartHereToolStripMenuItem});
            this.EditorMenuStrip.Name = "SelectedMenuStrip";
            this.EditorMenuStrip.Size = new Size(186, 356);
            this.EditorMenuStrip.Opening += new CancelEventHandler(this.EditorMenuStrip_Opening);
            // 
            // CopyMenuItem
            // 
            this.CopyMenuItem.Name = "CopyMenuItem";
            this.CopyMenuItem.Size = new Size(185, 22);
            this.CopyMenuItem.Text = "Copy";
            this.CopyMenuItem.Click += new EventHandler(this.CopyMenuItemClick);
            // 
            // TransformMenuItem
            // 
            this.TransformMenuItem.Name = "TransformMenuItem";
            this.TransformMenuItem.Size = new Size(185, 22);
            this.TransformMenuItem.Text = "Transform";
            this.TransformMenuItem.Click += new EventHandler(this.TransformMenuItemClick);
            // 
            // DeleteMenuItem
            // 
            this.DeleteMenuItem.Name = "DeleteMenuItem";
            this.DeleteMenuItem.Size = new Size(185, 22);
            this.DeleteMenuItem.Text = "Delete";
            this.DeleteMenuItem.Click += new EventHandler(this.DeleteSelected);
            // 
            // GrassMenuItem
            // 
            this.GrassMenuItem.Name = "GrassMenuItem";
            this.GrassMenuItem.Size = new Size(185, 22);
            this.GrassMenuItem.Text = "Toggle grass";
            this.GrassMenuItem.Click += new EventHandler(this.HandleGrassMenu);
            // 
            // GravityNoneMenuItem
            // 
            this.GravityNoneMenuItem.Checked = true;
            this.GravityNoneMenuItem.CheckOnClick = true;
            this.GravityNoneMenuItem.CheckState = CheckState.Checked;
            this.GravityNoneMenuItem.Name = "GravityNoneMenuItem";
            this.GravityNoneMenuItem.Size = new Size(185, 22);
            this.GravityNoneMenuItem.Text = "Gravity none";
            this.GravityNoneMenuItem.Click += new EventHandler(this.HandleGravityMenu);
            // 
            // GravityUpMenuItem
            // 
            this.GravityUpMenuItem.CheckOnClick = true;
            this.GravityUpMenuItem.Name = "GravityUpMenuItem";
            this.GravityUpMenuItem.Size = new Size(185, 22);
            this.GravityUpMenuItem.Text = "Gravity up";
            this.GravityUpMenuItem.Click += new EventHandler(this.HandleGravityMenu);
            // 
            // GravityDownMenuItem
            // 
            this.GravityDownMenuItem.CheckOnClick = true;
            this.GravityDownMenuItem.Name = "GravityDownMenuItem";
            this.GravityDownMenuItem.Size = new Size(185, 22);
            this.GravityDownMenuItem.Text = "Gravity down";
            this.GravityDownMenuItem.Click += new EventHandler(this.HandleGravityMenu);
            // 
            // GravityLeftMenuItem
            // 
            this.GravityLeftMenuItem.CheckOnClick = true;
            this.GravityLeftMenuItem.Name = "GravityLeftMenuItem";
            this.GravityLeftMenuItem.Size = new Size(185, 22);
            this.GravityLeftMenuItem.Text = "Gravity left";
            this.GravityLeftMenuItem.Click += new EventHandler(this.HandleGravityMenu);
            // 
            // GravityRightMenuItem
            // 
            this.GravityRightMenuItem.CheckOnClick = true;
            this.GravityRightMenuItem.Name = "GravityRightMenuItem";
            this.GravityRightMenuItem.Size = new Size(185, 22);
            this.GravityRightMenuItem.Text = "Gravity right";
            this.GravityRightMenuItem.Click += new EventHandler(this.HandleGravityMenu);
            // 
            // PicturePropertiesMenuItem
            // 
            this.PicturePropertiesMenuItem.Name = "PicturePropertiesMenuItem";
            this.PicturePropertiesMenuItem.Size = new Size(185, 22);
            this.PicturePropertiesMenuItem.Text = "Picture properties";
            this.PicturePropertiesMenuItem.Click += new EventHandler(this.PicturePropertiesToolStripMenuItemClick);
            // 
            // bringToFrontToolStripMenuItem
            // 
            this.bringToFrontToolStripMenuItem.Name = "bringToFrontToolStripMenuItem";
            this.bringToFrontToolStripMenuItem.Size = new Size(185, 22);
            this.bringToFrontToolStripMenuItem.Text = "Bring to front";
            this.bringToFrontToolStripMenuItem.Click += new EventHandler(this.BringToFrontToolStripMenuItemClick);
            // 
            // sendToBackToolStripMenuItem
            // 
            this.sendToBackToolStripMenuItem.Name = "sendToBackToolStripMenuItem";
            this.sendToBackToolStripMenuItem.Size = new Size(185, 22);
            this.sendToBackToolStripMenuItem.Text = "Send to back";
            this.sendToBackToolStripMenuItem.Click += new EventHandler(this.SendToBackToolStripMenuItemClick);
            // 
            // convertToToolStripMenuItem
            // 
            this.convertToToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
            this.applesConvertItem,
            this.killersConvertItem,
            this.flowersConvertItem,
            this.picturesConvertItem});
            this.convertToToolStripMenuItem.Name = "convertToToolStripMenuItem";
            this.convertToToolStripMenuItem.Size = new Size(185, 22);
            this.convertToToolStripMenuItem.Text = "Convert to";
            // 
            // applesConvertItem
            // 
            this.applesConvertItem.Name = "applesConvertItem";
            this.applesConvertItem.Size = new Size(116, 22);
            this.applesConvertItem.Text = "Apples";
            this.applesConvertItem.Click += new EventHandler(this.ConvertClicked);
            // 
            // killersConvertItem
            // 
            this.killersConvertItem.Name = "killersConvertItem";
            this.killersConvertItem.Size = new Size(116, 22);
            this.killersConvertItem.Text = "Killers";
            this.killersConvertItem.Click += new EventHandler(this.ConvertClicked);
            // 
            // flowersConvertItem
            // 
            this.flowersConvertItem.Name = "flowersConvertItem";
            this.flowersConvertItem.Size = new Size(116, 22);
            this.flowersConvertItem.Text = "Flowers";
            this.flowersConvertItem.Click += new EventHandler(this.ConvertClicked);
            // 
            // picturesConvertItem
            // 
            this.picturesConvertItem.Name = "picturesConvertItem";
            this.picturesConvertItem.Size = new Size(116, 22);
            this.picturesConvertItem.Text = "Pictures";
            this.picturesConvertItem.Click += new EventHandler(this.ConvertClicked);
            // 
            // saveStartPositionToolStripMenuItem
            // 
            this.saveStartPositionToolStripMenuItem.Name = "saveStartPositionToolStripMenuItem";
            this.saveStartPositionToolStripMenuItem.Size = new Size(185, 22);
            this.saveStartPositionToolStripMenuItem.Text = "Save start position";
            this.saveStartPositionToolStripMenuItem.Click += new EventHandler(this.SaveStartPosition);
            // 
            // restoreStartPositionToolStripMenuItem
            // 
            this.restoreStartPositionToolStripMenuItem.Name = "restoreStartPositionToolStripMenuItem";
            this.restoreStartPositionToolStripMenuItem.Size = new Size(185, 22);
            this.restoreStartPositionToolStripMenuItem.Text = "Restore start position";
            this.restoreStartPositionToolStripMenuItem.Click += new EventHandler(this.restoreStartPositionToolStripMenuItem_Click);
            // 
            // moveStartHereToolStripMenuItem
            // 
            this.moveStartHereToolStripMenuItem.Name = "moveStartHereToolStripMenuItem";
            this.moveStartHereToolStripMenuItem.Size = new Size(185, 22);
            this.moveStartHereToolStripMenuItem.Text = "Move start here";
            this.moveStartHereToolStripMenuItem.Click += new EventHandler(this.MoveStartHereToolStripMenuItem_Click);
            // 
            // saveAsPictureDialog
            // 
            this.saveAsPictureDialog.DefaultExt = "png";
            this.saveAsPictureDialog.FileName = "Untitled";
            this.saveAsPictureDialog.Filter = "Portable Network Graphics (*.png)|*.png|Scalable Vector Graphics (*.svg)|*.svg";
            // 
            // importFileDialog
            // 
            this.importFileDialog.Filter = "Elasto Mania level or image (*.lev, *.bmp, *.png, *.gif, *.tiff, *.exif, *.svg, *" +
    ".svgz)|*.lev;*.bmp;*.png;*.gif;*.tiff;*.exif;*.svg;*.svgz";
            this.importFileDialog.Multiselect = true;
            // 
            // ToolPanel
            // 
            this.ToolPanel.AutoScroll = true;
            this.ToolPanel.Controls.Add(this.TextButton);
            this.ToolPanel.Controls.Add(this.PictureButton);
            this.ToolPanel.Controls.Add(this.AutoGrassButton);
            this.ToolPanel.Controls.Add(this.CutConnectButton);
            this.ToolPanel.Controls.Add(this.SmoothenButton);
            this.ToolPanel.Controls.Add(this.FrameButton);
            this.ToolPanel.Controls.Add(this.PolyOpButton);
            this.ToolPanel.Controls.Add(this.EllipseButton);
            this.ToolPanel.Controls.Add(this.ZoomButton);
            this.ToolPanel.Controls.Add(this.PipeButton);
            this.ToolPanel.Controls.Add(this.ObjectButton);
            this.ToolPanel.Controls.Add(this.DrawButton);
            this.ToolPanel.Controls.Add(this.VertexButton);
            this.ToolPanel.Controls.Add(this.SelectButton);
            this.ToolPanel.Dock = DockStyle.Left;
            this.ToolPanel.Location = new Point(0, 136);
            this.ToolPanel.Name = "ToolPanel";
            this.ToolPanel.Size = new Size(84, 350);
            this.ToolPanel.TabIndex = 3;
            this.ToolPanel.Text = "Tools";
            // 
            // TextButton
            // 
            this.TextButton.Appearance = Appearance.Button;
            this.TextButton.AutoSize = true;
            this.TextButton.Dock = DockStyle.Top;
            this.TextButton.Location = new Point(0, 325);
            this.TextButton.Name = "TextButton";
            this.TextButton.Size = new Size(84, 25);
            this.TextButton.TabIndex = 16;
            this.TextButton.Text = "&Text";
            this.TextButton.TextAlign = ContentAlignment.MiddleCenter;
            this.TextButton.UseVisualStyleBackColor = true;
            this.TextButton.CheckedChanged += new EventHandler(this.TextButton_CheckedChanged);
            // 
            // PictureButton
            // 
            this.PictureButton.Appearance = Appearance.Button;
            this.PictureButton.AutoSize = true;
            this.PictureButton.Dock = DockStyle.Top;
            this.PictureButton.Location = new Point(0, 300);
            this.PictureButton.Name = "PictureButton";
            this.PictureButton.Size = new Size(84, 25);
            this.PictureButton.TabIndex = 15;
            this.PictureButton.Text = "P&icture";
            this.PictureButton.TextAlign = ContentAlignment.MiddleCenter;
            this.PictureButton.UseVisualStyleBackColor = true;
            // 
            // AutoGrassButton
            // 
            this.AutoGrassButton.Appearance = Appearance.Button;
            this.AutoGrassButton.AutoSize = true;
            this.AutoGrassButton.Dock = DockStyle.Top;
            this.AutoGrassButton.Location = new Point(0, 275);
            this.AutoGrassButton.Name = "AutoGrassButton";
            this.AutoGrassButton.Size = new Size(84, 25);
            this.AutoGrassButton.TabIndex = 14;
            this.AutoGrassButton.Text = "&AutoGrass";
            this.AutoGrassButton.TextAlign = ContentAlignment.MiddleCenter;
            this.AutoGrassButton.UseVisualStyleBackColor = true;
            // 
            // CutConnectButton
            // 
            this.CutConnectButton.Appearance = Appearance.Button;
            this.CutConnectButton.AutoSize = true;
            this.CutConnectButton.Dock = DockStyle.Top;
            this.CutConnectButton.Location = new Point(0, 250);
            this.CutConnectButton.Name = "CutConnectButton";
            this.CutConnectButton.Size = new Size(84, 25);
            this.CutConnectButton.TabIndex = 13;
            this.CutConnectButton.Text = "&Cut/connect";
            this.CutConnectButton.TextAlign = ContentAlignment.MiddleCenter;
            this.CutConnectButton.UseVisualStyleBackColor = true;
            // 
            // SmoothenButton
            // 
            this.SmoothenButton.Appearance = Appearance.Button;
            this.SmoothenButton.AutoSize = true;
            this.SmoothenButton.Dock = DockStyle.Top;
            this.SmoothenButton.Location = new Point(0, 225);
            this.SmoothenButton.Name = "SmoothenButton";
            this.SmoothenButton.Size = new Size(84, 25);
            this.SmoothenButton.TabIndex = 12;
            this.SmoothenButton.Text = "S&moothen";
            this.SmoothenButton.TextAlign = ContentAlignment.MiddleCenter;
            this.SmoothenButton.UseVisualStyleBackColor = true;
            // 
            // FrameButton
            // 
            this.FrameButton.Appearance = Appearance.Button;
            this.FrameButton.AutoSize = true;
            this.FrameButton.Dock = DockStyle.Top;
            this.FrameButton.Location = new Point(0, 200);
            this.FrameButton.Name = "FrameButton";
            this.FrameButton.Size = new Size(84, 25);
            this.FrameButton.TabIndex = 11;
            this.FrameButton.Text = "&Frame";
            this.FrameButton.TextAlign = ContentAlignment.MiddleCenter;
            this.FrameButton.UseVisualStyleBackColor = true;
            // 
            // PolyOpButton
            // 
            this.PolyOpButton.Appearance = Appearance.Button;
            this.PolyOpButton.AutoSize = true;
            this.PolyOpButton.Dock = DockStyle.Top;
            this.PolyOpButton.Location = new Point(0, 175);
            this.PolyOpButton.Name = "PolyOpButton";
            this.PolyOpButton.Size = new Size(84, 25);
            this.PolyOpButton.TabIndex = 9;
            this.PolyOpButton.Text = "Po&lyOp";
            this.PolyOpButton.TextAlign = ContentAlignment.MiddleCenter;
            this.PolyOpButton.UseVisualStyleBackColor = true;
            // 
            // EllipseButton
            // 
            this.EllipseButton.Appearance = Appearance.Button;
            this.EllipseButton.AutoSize = true;
            this.EllipseButton.Dock = DockStyle.Top;
            this.EllipseButton.Location = new Point(0, 150);
            this.EllipseButton.Name = "EllipseButton";
            this.EllipseButton.Size = new Size(84, 25);
            this.EllipseButton.TabIndex = 8;
            this.EllipseButton.Text = "&Ellipse";
            this.EllipseButton.TextAlign = ContentAlignment.MiddleCenter;
            this.EllipseButton.UseVisualStyleBackColor = true;
            // 
            // ZoomButton
            // 
            this.ZoomButton.Appearance = Appearance.Button;
            this.ZoomButton.AutoSize = true;
            this.ZoomButton.Dock = DockStyle.Top;
            this.ZoomButton.Location = new Point(0, 125);
            this.ZoomButton.Name = "ZoomButton";
            this.ZoomButton.Size = new Size(84, 25);
            this.ZoomButton.TabIndex = 7;
            this.ZoomButton.Text = "&Zoom";
            this.ZoomButton.TextAlign = ContentAlignment.MiddleCenter;
            this.ZoomButton.UseVisualStyleBackColor = true;
            // 
            // PipeButton
            // 
            this.PipeButton.Appearance = Appearance.Button;
            this.PipeButton.AutoSize = true;
            this.PipeButton.Dock = DockStyle.Top;
            this.PipeButton.Location = new Point(0, 100);
            this.PipeButton.Name = "PipeButton";
            this.PipeButton.Size = new Size(84, 25);
            this.PipeButton.TabIndex = 6;
            this.PipeButton.Text = "&Pipe";
            this.PipeButton.TextAlign = ContentAlignment.MiddleCenter;
            this.PipeButton.UseVisualStyleBackColor = true;
            // 
            // ObjectButton
            // 
            this.ObjectButton.Appearance = Appearance.Button;
            this.ObjectButton.AutoSize = true;
            this.ObjectButton.Dock = DockStyle.Top;
            this.ObjectButton.Location = new Point(0, 75);
            this.ObjectButton.Name = "ObjectButton";
            this.ObjectButton.Size = new Size(84, 25);
            this.ObjectButton.TabIndex = 3;
            this.ObjectButton.Text = "&Object";
            this.ObjectButton.TextAlign = ContentAlignment.MiddleCenter;
            this.ObjectButton.UseVisualStyleBackColor = true;
            // 
            // DrawButton
            // 
            this.DrawButton.Appearance = Appearance.Button;
            this.DrawButton.AutoSize = true;
            this.DrawButton.Dock = DockStyle.Top;
            this.DrawButton.Location = new Point(0, 50);
            this.DrawButton.Name = "DrawButton";
            this.DrawButton.Size = new Size(84, 25);
            this.DrawButton.TabIndex = 2;
            this.DrawButton.Text = "&Draw";
            this.DrawButton.TextAlign = ContentAlignment.MiddleCenter;
            this.DrawButton.UseVisualStyleBackColor = true;
            // 
            // VertexButton
            // 
            this.VertexButton.Appearance = Appearance.Button;
            this.VertexButton.AutoSize = true;
            this.VertexButton.Dock = DockStyle.Top;
            this.VertexButton.Location = new Point(0, 25);
            this.VertexButton.Name = "VertexButton";
            this.VertexButton.Size = new Size(84, 25);
            this.VertexButton.TabIndex = 1;
            this.VertexButton.Text = "&Vertex";
            this.VertexButton.TextAlign = ContentAlignment.MiddleCenter;
            this.VertexButton.UseVisualStyleBackColor = true;
            // 
            // SelectButton
            // 
            this.SelectButton.Appearance = Appearance.Button;
            this.SelectButton.AutoSize = true;
            this.SelectButton.Checked = true;
            this.SelectButton.Dock = DockStyle.Top;
            this.SelectButton.Location = new Point(0, 0);
            this.SelectButton.Name = "SelectButton";
            this.SelectButton.Size = new Size(84, 25);
            this.SelectButton.TabIndex = 0;
            this.SelectButton.TabStop = true;
            this.SelectButton.Text = "&Select";
            this.SelectButton.TextAlign = ContentAlignment.MiddleCenter;
            this.SelectButton.UseVisualStyleBackColor = true;
            // 
            // toolStripSeparator16
            // 
            this.toolStripSeparator16.Name = "toolStripSeparator16";
            this.toolStripSeparator16.Size = new Size(6, 39);
            // 
            // playButton
            // 
            this.playButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.playButton.SvgData = Resources.Play;
            this.playButton.Name = "playButton";
            this.playButton.Size = new Size(33, 36);
            this.playButton.Click += new EventHandler(this.playButton_Click);
            this.playButton.ToolTipText = "Play";
            // 
            // stopButton
            // 
            this.stopButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.stopButton.SvgData = Resources.Stop;
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new Size(33, 36);
            this.stopButton.Click += new EventHandler(this.stopButton_Click);
            this.stopButton.Enabled = false;
            this.stopButton.ToolTipText = "Stop";
            // 
            // settingsButton
            // 
            this.settingsButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.settingsButton.SvgData = Resources.Settings;
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.Size = new Size(33, 36);
            this.settingsButton.Click += new EventHandler(this.settingsButton_Click);
            this.settingsButton.ToolTipText = "Settings";
            // 
            // LevelEditor
            // 
            this.AutoScaleDimensions = new SizeF(96F, 96F);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.ClientSize = new Size(929, 509);
            this.Controls.Add(this.EditorControl);
            this.Controls.Add(this.ToolPanel);
            this.Controls.Add(this.StatusStrip1);
            this.Controls.Add(this.ToolStripPanel1);
            this.Controls.Add(this.MenuStrip1);
            this.Font = new Font("Segoe UI", 9F);
            this.MainMenuStrip = this.MenuStrip1;
            this.Name = "LevelEditorForm";
            this.Text = "SLE";
            this.FormClosing += new FormClosingEventHandler(this.ConfirmClose);
            this.Load += new EventHandler(this.RefreshOnOpen);
            this.KeyDown += new KeyEventHandler(this.KeyHandlerDown);
            this.KeyUp += new KeyEventHandler(this.KeyHandlerUp);
            this.MenuStrip1.ResumeLayout(false);
            this.MenuStrip1.PerformLayout();
            this.StatusStrip1.ResumeLayout(false);
            this.StatusStrip1.PerformLayout();
            this.ToolStripPanel1.ResumeLayout(false);
            this.ToolStripPanel1.PerformLayout();
            this.ToolStrip1.ResumeLayout(false);
            this.ToolStrip1.PerformLayout();
            this.ToolStrip2.ResumeLayout(false);
            this.ToolStrip2.PerformLayout();
            this.toolStrip3.ResumeLayout(false);
            this.toolStrip3.PerformLayout();
            this.EditorMenuStrip.ResumeLayout(false);
            this.ToolPanel.ResumeLayout(false);
            this.ToolPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
		internal RadioButtonMod ZoomButton;
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
		internal ToolStripTextBox LGRBox;
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
        private SvgImageToolStripButton playButton;
        private SvgImageToolStripButton stopButton;
        private SvgImageToolStripButton settingsButton;
        }
	
}
