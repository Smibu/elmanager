using System.Windows.Forms;
using Elmanager.CustomControls;

namespace Elmanager.Forms
{
	public partial class LevelEditor : System.Windows.Forms.Form
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LevelEditor));
            this.MenuStrip1 = new System.Windows.Forms.MenuStrip();
            this.FileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsPictureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteLevMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ActionsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.QuickGrassToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DeleteAllGrassToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DeleteSelectedMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MirrorLevelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.UndoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RedoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.ZoomFillToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CheckTopologyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LevelPropertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.previousLevelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nextLevelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importLevelsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SelectionFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EnableAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DisableAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.GroundPolygonsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.GrassPolygonsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ApplesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.KillersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FlowersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PicturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TexturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ConfigurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MainConfigMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RenderingSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EditorControl = new System.Windows.Forms.Panel();
            this.OpenFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.StatusStrip1 = new System.Windows.Forms.StatusStrip();
            this.CoordinateLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.SelectionLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.HighlightLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.SaveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.ToolStripPanel1 = new System.Windows.Forms.ToolStripPanel();
            this.ToolStrip1 = new System.Windows.Forms.ToolStrip();
            this.NewButton = new System.Windows.Forms.ToolStripButton();
            this.OpenButton = new System.Windows.Forms.ToolStripButton();
            this.SaveButton = new System.Windows.Forms.ToolStripButton();
            this.SaveAsButton = new System.Windows.Forms.ToolStripButton();
            this.deleteButton = new System.Windows.Forms.ToolStripButton();
            this.ToolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.CheckTopologyButton = new System.Windows.Forms.ToolStripButton();
            this.ZoomFillButton = new System.Windows.Forms.ToolStripButton();
            this.ToolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.UndoButton = new System.Windows.Forms.ToolStripButton();
            this.RedoButton = new System.Windows.Forms.ToolStripButton();
            this.ToolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.PreviousButton = new System.Windows.Forms.ToolStripButton();
            this.NextButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel5 = new System.Windows.Forms.ToolStripLabel();
            this.filenameBox = new System.Windows.Forms.ToolStripTextBox();
            this.filenameOkButton = new System.Windows.Forms.ToolStripButton();
            this.filenameCancelButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.TitleBox = new System.Windows.Forms.ToolStripTextBox();
            this.ToolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.LGRBox = new System.Windows.Forms.ToolStripTextBox();
            this.ToolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.GroundComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.ToolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
            this.SkyComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.ToolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStrip2 = new System.Windows.Forms.ToolStrip();
            this.ShowGridButton = new System.Windows.Forms.ToolStripButton();
            this.snapToGridButton = new System.Windows.Forms.ToolStripButton();
            this.showCrossHairButton = new System.Windows.Forms.ToolStripButton();
            this.ShowGrassEdgesButton = new System.Windows.Forms.ToolStripButton();
            this.ShowGroundEdgesButton = new System.Windows.Forms.ToolStripButton();
            this.ShowVerticesButton = new System.Windows.Forms.ToolStripButton();
            this.ShowTextureFramesButton = new System.Windows.Forms.ToolStripButton();
            this.ShowPictureFramesButton = new System.Windows.Forms.ToolStripButton();
            this.ShowTexturesButton = new System.Windows.Forms.ToolStripButton();
            this.ShowPicturesButton = new System.Windows.Forms.ToolStripButton();
            this.ShowObjectFramesButton = new System.Windows.Forms.ToolStripButton();
            this.ShowObjectsButton = new System.Windows.Forms.ToolStripButton();
            this.ShowGravityAppleArrowsButton = new System.Windows.Forms.ToolStripButton();
            this.ShowGroundButton = new System.Windows.Forms.ToolStripButton();
            this.ShowGroundTextureButton = new System.Windows.Forms.ToolStripButton();
            this.ShowSkyTextureButton = new System.Windows.Forms.ToolStripButton();
            this.ZoomTexturesButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.BestTimeLabel = new System.Windows.Forms.ToolStripLabel();
            this.topologyList = new System.Windows.Forms.ToolStripDropDownButton();
            this.StatusStrip2 = new System.Windows.Forms.StatusStrip();
            this.InfoLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.EditorMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.CopyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TransformMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DeleteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.GrassMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.GravityNoneMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.GravityUpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.GravityDownMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.GravityLeftMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.GravityRightMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PicturePropertiesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bringToFrontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendToBackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.applesConvertItem = new System.Windows.Forms.ToolStripMenuItem();
            this.killersConvertItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flowersConvertItem = new System.Windows.Forms.ToolStripMenuItem();
            this.picturesConvertItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsPictureDialog = new System.Windows.Forms.SaveFileDialog();
            this.ToolPanel = new Elmanager.CustomControls.PanelMod();
            this.TextButton = new Elmanager.CustomControls.RadioButtonMod();
            this.PictureButton = new Elmanager.CustomControls.RadioButtonMod();
            this.AutoGrassButton = new Elmanager.CustomControls.RadioButtonMod();
            this.CutConnectButton = new Elmanager.CustomControls.RadioButtonMod();
            this.SmoothenButton = new Elmanager.CustomControls.RadioButtonMod();
            this.FrameButton = new Elmanager.CustomControls.RadioButtonMod();
            this.PolyOpButton = new Elmanager.CustomControls.RadioButtonMod();
            this.EllipseButton = new Elmanager.CustomControls.RadioButtonMod();
            this.ZoomButton = new Elmanager.CustomControls.RadioButtonMod();
            this.PipeButton = new Elmanager.CustomControls.RadioButtonMod();
            this.ObjectButton = new Elmanager.CustomControls.RadioButtonMod();
            this.DrawButton = new Elmanager.CustomControls.RadioButtonMod();
            this.VertexButton = new Elmanager.CustomControls.RadioButtonMod();
            this.SelectButton = new Elmanager.CustomControls.RadioButtonMod();
            this.importFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip1.SuspendLayout();
            this.StatusStrip1.SuspendLayout();
            this.ToolStripPanel1.SuspendLayout();
            this.ToolStrip1.SuspendLayout();
            this.ToolStrip2.SuspendLayout();
            this.StatusStrip2.SuspendLayout();
            this.EditorMenuStrip.SuspendLayout();
            this.ToolPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // MenuStrip1
            // 
            this.MenuStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.MenuStrip1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.MenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileToolStripMenuItem,
            this.ActionsMenuItem,
            this.SelectionFilterToolStripMenuItem,
            this.ConfigurationToolStripMenuItem});
            this.MenuStrip1.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip1.Name = "MenuStrip1";
            this.MenuStrip1.Size = new System.Drawing.Size(929, 24);
            this.MenuStrip1.TabIndex = 0;
            this.MenuStrip1.Text = "MenuStrip1";
            // 
            // FileToolStripMenuItem
            // 
            this.FileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NewToolStripMenuItem,
            this.OpenToolStripMenuItem,
            this.SaveToolStripMenuItem,
            this.SaveAsToolStripMenuItem,
            this.saveAsPictureToolStripMenuItem,
            this.deleteLevMenuItem,
            this.ExitToolStripMenuItem});
            this.FileToolStripMenuItem.Name = "FileToolStripMenuItem";
            this.FileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.FileToolStripMenuItem.Text = "File";
            // 
            // NewToolStripMenuItem
            // 
            this.NewToolStripMenuItem.Image = global::My.Resources.Resources.New16;
            this.NewToolStripMenuItem.Name = "NewToolStripMenuItem";
            this.NewToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.NewToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.NewToolStripMenuItem.Text = "New";
            this.NewToolStripMenuItem.Click += new System.EventHandler(this.NewLevel);
            // 
            // OpenToolStripMenuItem
            // 
            this.OpenToolStripMenuItem.Image = global::My.Resources.Resources.Open;
            this.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem";
            this.OpenToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.OpenToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.OpenToolStripMenuItem.Text = "Open";
            this.OpenToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItemClick);
            // 
            // SaveToolStripMenuItem
            // 
            this.SaveToolStripMenuItem.Enabled = false;
            this.SaveToolStripMenuItem.Image = global::My.Resources.Resources.Save;
            this.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem";
            this.SaveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.SaveToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.SaveToolStripMenuItem.Text = "Save";
            this.SaveToolStripMenuItem.Click += new System.EventHandler(this.SaveClicked);
            // 
            // SaveAsToolStripMenuItem
            // 
            this.SaveAsToolStripMenuItem.Image = global::My.Resources.Resources.SaveAs16;
            this.SaveAsToolStripMenuItem.Name = "SaveAsToolStripMenuItem";
            this.SaveAsToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.SaveAsToolStripMenuItem.Text = "Save as...";
            this.SaveAsToolStripMenuItem.Click += new System.EventHandler(this.SaveAs);
            // 
            // saveAsPictureToolStripMenuItem
            // 
            this.saveAsPictureToolStripMenuItem.Name = "saveAsPictureToolStripMenuItem";
            this.saveAsPictureToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.saveAsPictureToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.saveAsPictureToolStripMenuItem.Text = "Save as picture...";
            this.saveAsPictureToolStripMenuItem.Click += new System.EventHandler(this.saveAsPictureToolStripMenuItem_Click);
            // 
            // deleteLevMenuItem
            // 
            this.deleteLevMenuItem.Image = global::My.Resources.Resources.Delete;
            this.deleteLevMenuItem.Name = "deleteLevMenuItem";
            this.deleteLevMenuItem.Size = new System.Drawing.Size(192, 22);
            this.deleteLevMenuItem.Text = "Delete";
            this.deleteLevMenuItem.Click += new System.EventHandler(this.deleteLevMenuItem_Click);
            // 
            // ExitToolStripMenuItem
            // 
            this.ExitToolStripMenuItem.Image = global::My.Resources.Resources.Exit16;
            this.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem";
            this.ExitToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.ExitToolStripMenuItem.Text = "Exit";
            this.ExitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItemClick);
            // 
            // ActionsMenuItem
            // 
            this.ActionsMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.QuickGrassToolStripMenuItem,
            this.DeleteAllGrassToolStripMenuItem,
            this.DeleteSelectedMenuItem,
            this.MirrorLevelToolStripMenuItem,
            this.UndoToolStripMenuItem,
            this.RedoToolStripMenuItem,
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
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
            this.ActionsMenuItem.Size = new System.Drawing.Size(45, 20);
            this.ActionsMenuItem.Text = "Tools";
            // 
            // QuickGrassToolStripMenuItem
            // 
            this.QuickGrassToolStripMenuItem.Image = global::My.Resources.Resources.GrassAll;
            this.QuickGrassToolStripMenuItem.Name = "QuickGrassToolStripMenuItem";
            this.QuickGrassToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.QuickGrassToolStripMenuItem.Size = new System.Drawing.Size(239, 22);
            this.QuickGrassToolStripMenuItem.Text = "QuickGrass";
            this.QuickGrassToolStripMenuItem.Click += new System.EventHandler(this.QuickGrassToolStripMenuItemClick);
            // 
            // DeleteAllGrassToolStripMenuItem
            // 
            this.DeleteAllGrassToolStripMenuItem.Image = global::My.Resources.Resources.GrassDelete;
            this.DeleteAllGrassToolStripMenuItem.Name = "DeleteAllGrassToolStripMenuItem";
            this.DeleteAllGrassToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.DeleteAllGrassToolStripMenuItem.Size = new System.Drawing.Size(239, 22);
            this.DeleteAllGrassToolStripMenuItem.Text = "Delete all grass";
            this.DeleteAllGrassToolStripMenuItem.Click += new System.EventHandler(this.DeleteAllGrassToolStripMenuItemClick);
            // 
            // DeleteSelectedMenuItem
            // 
            this.DeleteSelectedMenuItem.Image = global::My.Resources.Resources.Delete;
            this.DeleteSelectedMenuItem.Name = "DeleteSelectedMenuItem";
            this.DeleteSelectedMenuItem.Size = new System.Drawing.Size(239, 22);
            this.DeleteSelectedMenuItem.Text = "Delete selected";
            this.DeleteSelectedMenuItem.Click += new System.EventHandler(this.DeleteSelected);
            // 
            // MirrorLevelToolStripMenuItem
            // 
            this.MirrorLevelToolStripMenuItem.Image = global::My.Resources.Resources.Mirror16;
            this.MirrorLevelToolStripMenuItem.Name = "MirrorLevelToolStripMenuItem";
            this.MirrorLevelToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
            this.MirrorLevelToolStripMenuItem.Size = new System.Drawing.Size(239, 22);
            this.MirrorLevelToolStripMenuItem.Text = "Mirror selected";
            this.MirrorLevelToolStripMenuItem.Click += new System.EventHandler(this.Mirror);
            // 
            // UndoToolStripMenuItem
            // 
            this.UndoToolStripMenuItem.Image = global::My.Resources.Resources.Undo;
            this.UndoToolStripMenuItem.Name = "UndoToolStripMenuItem";
            this.UndoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.UndoToolStripMenuItem.Size = new System.Drawing.Size(239, 22);
            this.UndoToolStripMenuItem.Text = "Undo";
            this.UndoToolStripMenuItem.Click += new System.EventHandler(this.Undo);
            // 
            // RedoToolStripMenuItem
            // 
            this.RedoToolStripMenuItem.Image = global::My.Resources.Resources.Redo;
            this.RedoToolStripMenuItem.Name = "RedoToolStripMenuItem";
            this.RedoToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.RedoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.RedoToolStripMenuItem.Size = new System.Drawing.Size(239, 22);
            this.RedoToolStripMenuItem.Text = "Redo";
            this.RedoToolStripMenuItem.Click += new System.EventHandler(this.Redo);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.toolStripMenuItem1.Size = new System.Drawing.Size(239, 22);
            this.toolStripMenuItem1.Text = "Copy";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.CopyMenuItemClick);
            // 
            // ToolStripSeparator8
            // 
            this.ToolStripSeparator8.Name = "ToolStripSeparator8";
            this.ToolStripSeparator8.Size = new System.Drawing.Size(236, 6);
            // 
            // ZoomFillToolStripMenuItem
            // 
            this.ZoomFillToolStripMenuItem.Image = global::My.Resources.Resources.ZoomFill16;
            this.ZoomFillToolStripMenuItem.Name = "ZoomFillToolStripMenuItem";
            this.ZoomFillToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.ZoomFillToolStripMenuItem.Size = new System.Drawing.Size(239, 22);
            this.ZoomFillToolStripMenuItem.Text = "Zoom fill";
            this.ZoomFillToolStripMenuItem.Click += new System.EventHandler(this.ZoomFillToolStripMenuItemClick);
            // 
            // CheckTopologyMenuItem
            // 
            this.CheckTopologyMenuItem.Image = global::My.Resources.Resources.Topology;
            this.CheckTopologyMenuItem.Name = "CheckTopologyMenuItem";
            this.CheckTopologyMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F6;
            this.CheckTopologyMenuItem.Size = new System.Drawing.Size(239, 22);
            this.CheckTopologyMenuItem.Text = "Check topology";
            this.CheckTopologyMenuItem.Click += new System.EventHandler(this.CheckTopologyAndUpdate);
            // 
            // LevelPropertiesToolStripMenuItem
            // 
            this.LevelPropertiesToolStripMenuItem.Name = "LevelPropertiesToolStripMenuItem";
            this.LevelPropertiesToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F4;
            this.LevelPropertiesToolStripMenuItem.Size = new System.Drawing.Size(239, 22);
            this.LevelPropertiesToolStripMenuItem.Text = "Level properties";
            this.LevelPropertiesToolStripMenuItem.Click += new System.EventHandler(this.LevelPropertiesToolStripMenuItemClick);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(236, 6);
            // 
            // previousLevelToolStripMenuItem
            // 
            this.previousLevelToolStripMenuItem.Image = global::My.Resources.Resources.Previous;
            this.previousLevelToolStripMenuItem.Name = "previousLevelToolStripMenuItem";
            this.previousLevelToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.previousLevelToolStripMenuItem.Size = new System.Drawing.Size(239, 22);
            this.previousLevelToolStripMenuItem.Text = "Previous level";
            // 
            // nextLevelToolStripMenuItem
            // 
            this.nextLevelToolStripMenuItem.Image = global::My.Resources.Resources.Next;
            this.nextLevelToolStripMenuItem.Name = "nextLevelToolStripMenuItem";
            this.nextLevelToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.nextLevelToolStripMenuItem.Size = new System.Drawing.Size(239, 22);
            this.nextLevelToolStripMenuItem.Text = "Next Level";
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(236, 6);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(239, 22);
            this.selectAllToolStripMenuItem.Text = "Select all";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.SelectAllToolStripMenuItemClick);
            // 
            // importLevelsToolStripMenuItem
            // 
            this.importLevelsToolStripMenuItem.Name = "importLevelsToolStripMenuItem";
            this.importLevelsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.importLevelsToolStripMenuItem.Size = new System.Drawing.Size(239, 22);
            this.importLevelsToolStripMenuItem.Text = "Import level(s)/image(s)...";
            this.importLevelsToolStripMenuItem.Click += new System.EventHandler(this.importLevelsToolStripMenuItem_Click);
            // 
            // SelectionFilterToolStripMenuItem
            // 
            this.SelectionFilterToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
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
            this.SelectionFilterToolStripMenuItem.Size = new System.Drawing.Size(85, 20);
            this.SelectionFilterToolStripMenuItem.Text = "Selection filter";
            // 
            // EnableAllToolStripMenuItem
            // 
            this.EnableAllToolStripMenuItem.Name = "EnableAllToolStripMenuItem";
            this.EnableAllToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.EnableAllToolStripMenuItem.Text = "Enable all";
            this.EnableAllToolStripMenuItem.Click += new System.EventHandler(this.SetAllFilters);
            // 
            // DisableAllToolStripMenuItem
            // 
            this.DisableAllToolStripMenuItem.Name = "DisableAllToolStripMenuItem";
            this.DisableAllToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.DisableAllToolStripMenuItem.Text = "Disable all";
            this.DisableAllToolStripMenuItem.Click += new System.EventHandler(this.SetAllFilters);
            // 
            // GroundPolygonsToolStripMenuItem
            // 
            this.GroundPolygonsToolStripMenuItem.Checked = true;
            this.GroundPolygonsToolStripMenuItem.CheckOnClick = true;
            this.GroundPolygonsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.GroundPolygonsToolStripMenuItem.Name = "GroundPolygonsToolStripMenuItem";
            this.GroundPolygonsToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.GroundPolygonsToolStripMenuItem.Text = "Ground polygons";
            this.GroundPolygonsToolStripMenuItem.CheckedChanged += new System.EventHandler(this.FilterChanged);
            // 
            // GrassPolygonsToolStripMenuItem
            // 
            this.GrassPolygonsToolStripMenuItem.Checked = true;
            this.GrassPolygonsToolStripMenuItem.CheckOnClick = true;
            this.GrassPolygonsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.GrassPolygonsToolStripMenuItem.Name = "GrassPolygonsToolStripMenuItem";
            this.GrassPolygonsToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.GrassPolygonsToolStripMenuItem.Text = "Grass polygons";
            this.GrassPolygonsToolStripMenuItem.CheckedChanged += new System.EventHandler(this.FilterChanged);
            // 
            // ApplesToolStripMenuItem
            // 
            this.ApplesToolStripMenuItem.Checked = true;
            this.ApplesToolStripMenuItem.CheckOnClick = true;
            this.ApplesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ApplesToolStripMenuItem.Name = "ApplesToolStripMenuItem";
            this.ApplesToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.ApplesToolStripMenuItem.Text = "Apples";
            this.ApplesToolStripMenuItem.CheckedChanged += new System.EventHandler(this.FilterChanged);
            // 
            // KillersToolStripMenuItem
            // 
            this.KillersToolStripMenuItem.Checked = true;
            this.KillersToolStripMenuItem.CheckOnClick = true;
            this.KillersToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.KillersToolStripMenuItem.Name = "KillersToolStripMenuItem";
            this.KillersToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.KillersToolStripMenuItem.Text = "Killers";
            this.KillersToolStripMenuItem.CheckedChanged += new System.EventHandler(this.FilterChanged);
            // 
            // FlowersToolStripMenuItem
            // 
            this.FlowersToolStripMenuItem.Checked = true;
            this.FlowersToolStripMenuItem.CheckOnClick = true;
            this.FlowersToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.FlowersToolStripMenuItem.Name = "FlowersToolStripMenuItem";
            this.FlowersToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.FlowersToolStripMenuItem.Text = "Flowers";
            this.FlowersToolStripMenuItem.CheckedChanged += new System.EventHandler(this.FilterChanged);
            // 
            // PicturesToolStripMenuItem
            // 
            this.PicturesToolStripMenuItem.Checked = true;
            this.PicturesToolStripMenuItem.CheckOnClick = true;
            this.PicturesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.PicturesToolStripMenuItem.Name = "PicturesToolStripMenuItem";
            this.PicturesToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.PicturesToolStripMenuItem.Text = "Pictures";
            this.PicturesToolStripMenuItem.CheckedChanged += new System.EventHandler(this.FilterChanged);
            // 
            // TexturesToolStripMenuItem
            // 
            this.TexturesToolStripMenuItem.Checked = true;
            this.TexturesToolStripMenuItem.CheckOnClick = true;
            this.TexturesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.TexturesToolStripMenuItem.Name = "TexturesToolStripMenuItem";
            this.TexturesToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.TexturesToolStripMenuItem.Text = "Textures";
            this.TexturesToolStripMenuItem.CheckedChanged += new System.EventHandler(this.FilterChanged);
            // 
            // ConfigurationToolStripMenuItem
            // 
            this.ConfigurationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MainConfigMenuItem,
            this.RenderingSettingsToolStripMenuItem});
            this.ConfigurationToolStripMenuItem.Name = "ConfigurationToolStripMenuItem";
            this.ConfigurationToolStripMenuItem.Size = new System.Drawing.Size(81, 20);
            this.ConfigurationToolStripMenuItem.Text = "Configuration";
            // 
            // MainConfigMenuItem
            // 
            this.MainConfigMenuItem.Name = "MainConfigMenuItem";
            this.MainConfigMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F7;
            this.MainConfigMenuItem.Size = new System.Drawing.Size(181, 22);
            this.MainConfigMenuItem.Text = "Main";
            this.MainConfigMenuItem.Click += new System.EventHandler(this.OpenConfig);
            // 
            // RenderingSettingsToolStripMenuItem
            // 
            this.RenderingSettingsToolStripMenuItem.Name = "RenderingSettingsToolStripMenuItem";
            this.RenderingSettingsToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F8;
            this.RenderingSettingsToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.RenderingSettingsToolStripMenuItem.Text = "Rendering settings";
            this.RenderingSettingsToolStripMenuItem.Click += new System.EventHandler(this.OpenRenderingSettings);
            // 
            // EditorControl
            // 
            this.EditorControl.AllowDrop = true;
            this.EditorControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EditorControl.Location = new System.Drawing.Point(84, 124);
            this.EditorControl.Name = "EditorControl";
            this.EditorControl.Size = new System.Drawing.Size(845, 331);
            this.EditorControl.TabIndex = 2;
            this.EditorControl.DragDrop += new System.Windows.Forms.DragEventHandler(this.LevelDropped);
            this.EditorControl.DragEnter += new System.Windows.Forms.DragEventHandler(this.StartingDrop);
            this.EditorControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MouseDownEvent);
            this.EditorControl.MouseLeave += new System.EventHandler(this.MouseLeaveEvent);
            this.EditorControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MouseMoveEvent);
            this.EditorControl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MouseUpEvent);
            // 
            // OpenFileDialog1
            // 
            this.OpenFileDialog1.Filter = "Elasto Mania level (*.lev)|*.lev";
            // 
            // StatusStrip1
            // 
            this.StatusStrip1.GripMargin = new System.Windows.Forms.Padding(0);
            this.StatusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CoordinateLabel,
            this.SelectionLabel,
            this.HighlightLabel});
            this.StatusStrip1.Location = new System.Drawing.Point(0, 455);
            this.StatusStrip1.Name = "StatusStrip1";
            this.StatusStrip1.ShowItemToolTips = true;
            this.StatusStrip1.Size = new System.Drawing.Size(929, 24);
            this.StatusStrip1.SizingGrip = false;
            this.StatusStrip1.TabIndex = 4;
            this.StatusStrip1.Text = "StatusStrip1";
            // 
            // CoordinateLabel
            // 
            this.CoordinateLabel.AutoSize = false;
            this.CoordinateLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.CoordinateLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.CoordinateLabel.Name = "CoordinateLabel";
            this.CoordinateLabel.Size = new System.Drawing.Size(190, 19);
            this.CoordinateLabel.Text = "Mouse X: Y:";
            this.CoordinateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SelectionLabel
            // 
            this.SelectionLabel.AutoSize = false;
            this.SelectionLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.SelectionLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.SelectionLabel.Name = "SelectionLabel";
            this.SelectionLabel.Size = new System.Drawing.Size(420, 19);
            this.SelectionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // HighlightLabel
            // 
            this.HighlightLabel.AutoSize = false;
            this.HighlightLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.HighlightLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.HighlightLabel.Name = "HighlightLabel";
            this.HighlightLabel.Size = new System.Drawing.Size(304, 19);
            this.HighlightLabel.Spring = true;
            this.HighlightLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SaveFileDialog1
            // 
            this.SaveFileDialog1.DefaultExt = "lev";
            this.SaveFileDialog1.FileName = "Untitled";
            this.SaveFileDialog1.Filter = "Elasto Mania level (*.lev)|*.lev";
            // 
            // ToolStripPanel1
            // 
            this.ToolStripPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.ToolStripPanel1.Controls.Add(this.ToolStrip1);
            this.ToolStripPanel1.Controls.Add(this.ToolStrip2);
            this.ToolStripPanel1.Controls.Add(this.StatusStrip2);
            this.ToolStripPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.ToolStripPanel1.Location = new System.Drawing.Point(0, 24);
            this.ToolStripPanel1.Name = "ToolStripPanel1";
            this.ToolStripPanel1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.ToolStripPanel1.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.ToolStripPanel1.Size = new System.Drawing.Size(929, 100);
            // 
            // ToolStrip1
            // 
            this.ToolStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.ToolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.ToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.ToolStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.ToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
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
            this.ToolStrip1.Location = new System.Drawing.Point(3, 0);
            this.ToolStrip1.Name = "ToolStrip1";
            this.ToolStrip1.Size = new System.Drawing.Size(926, 39);
            this.ToolStrip1.TabIndex = 14;
            this.ToolStrip1.Text = "ToolStrip1";
            // 
            // NewButton
            // 
            this.NewButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.NewButton.Image = global::My.Resources.Resources._New;
            this.NewButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.NewButton.Name = "NewButton";
            this.NewButton.Size = new System.Drawing.Size(36, 36);
            this.NewButton.Text = "New";
            this.NewButton.Click += new System.EventHandler(this.NewLevel);
            // 
            // OpenButton
            // 
            this.OpenButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.OpenButton.Image = global::My.Resources.Resources.Open;
            this.OpenButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.OpenButton.Name = "OpenButton";
            this.OpenButton.Size = new System.Drawing.Size(36, 36);
            this.OpenButton.Text = "Open";
            this.OpenButton.Click += new System.EventHandler(this.OpenToolStripMenuItemClick);
            // 
            // SaveButton
            // 
            this.SaveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.SaveButton.Enabled = false;
            this.SaveButton.Image = global::My.Resources.Resources.Save;
            this.SaveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(36, 36);
            this.SaveButton.Text = "Save";
            this.SaveButton.Click += new System.EventHandler(this.SaveClicked);
            // 
            // SaveAsButton
            // 
            this.SaveAsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.SaveAsButton.Image = global::My.Resources.Resources.SaveAs;
            this.SaveAsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SaveAsButton.Name = "SaveAsButton";
            this.SaveAsButton.Size = new System.Drawing.Size(36, 36);
            this.SaveAsButton.Text = "Save as...";
            this.SaveAsButton.Click += new System.EventHandler(this.SaveAs);
            // 
            // deleteButton
            // 
            this.deleteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deleteButton.Image = global::My.Resources.Resources.Delete;
            this.deleteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(36, 36);
            this.deleteButton.Text = "toolStripButton1";
            this.deleteButton.ToolTipText = "Delete this level";
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // ToolStripSeparator1
            // 
            this.ToolStripSeparator1.Name = "ToolStripSeparator1";
            this.ToolStripSeparator1.Size = new System.Drawing.Size(6, 39);
            // 
            // CheckTopologyButton
            // 
            this.CheckTopologyButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.CheckTopologyButton.Image = global::My.Resources.Resources.Topology;
            this.CheckTopologyButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.CheckTopologyButton.Name = "CheckTopologyButton";
            this.CheckTopologyButton.Size = new System.Drawing.Size(36, 36);
            this.CheckTopologyButton.Text = "Check topology";
            this.CheckTopologyButton.Click += new System.EventHandler(this.CheckTopologyAndUpdate);
            // 
            // ZoomFillButton
            // 
            this.ZoomFillButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ZoomFillButton.Image = global::My.Resources.Resources.ZoomFill;
            this.ZoomFillButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ZoomFillButton.Name = "ZoomFillButton";
            this.ZoomFillButton.Size = new System.Drawing.Size(36, 36);
            this.ZoomFillButton.Text = "Zoom fill";
            // 
            // ToolStripSeparator2
            // 
            this.ToolStripSeparator2.Name = "ToolStripSeparator2";
            this.ToolStripSeparator2.Size = new System.Drawing.Size(6, 39);
            // 
            // UndoButton
            // 
            this.UndoButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.UndoButton.Image = global::My.Resources.Resources.Undo;
            this.UndoButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.UndoButton.Name = "UndoButton";
            this.UndoButton.Size = new System.Drawing.Size(36, 36);
            this.UndoButton.Text = "Undo";
            this.UndoButton.Click += new System.EventHandler(this.Undo);
            // 
            // RedoButton
            // 
            this.RedoButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.RedoButton.Image = global::My.Resources.Resources.Redo;
            this.RedoButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.RedoButton.Name = "RedoButton";
            this.RedoButton.Size = new System.Drawing.Size(36, 36);
            this.RedoButton.Text = "Redo";
            this.RedoButton.Click += new System.EventHandler(this.Redo);
            // 
            // ToolStripSeparator3
            // 
            this.ToolStripSeparator3.Name = "ToolStripSeparator3";
            this.ToolStripSeparator3.Size = new System.Drawing.Size(6, 39);
            // 
            // PreviousButton
            // 
            this.PreviousButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.PreviousButton.Image = global::My.Resources.Resources.Previous;
            this.PreviousButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.PreviousButton.Name = "PreviousButton";
            this.PreviousButton.Size = new System.Drawing.Size(36, 36);
            this.PreviousButton.Text = "Previous level";
            this.PreviousButton.Click += new System.EventHandler(this.PrevNextButtonClick);
            // 
            // NextButton
            // 
            this.NextButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.NextButton.Image = global::My.Resources.Resources.Next;
            this.NextButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.NextButton.Name = "NextButton";
            this.NextButton.Size = new System.Drawing.Size(36, 36);
            this.NextButton.Text = "Next level";
            this.NextButton.Click += new System.EventHandler(this.PrevNextButtonClick);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Size = new System.Drawing.Size(6, 39);
            // 
            // toolStripLabel5
            // 
            this.toolStripLabel5.Name = "toolStripLabel5";
            this.toolStripLabel5.Size = new System.Drawing.Size(58, 36);
            this.toolStripLabel5.Text = "Filename:";
            // 
            // filenameBox
            // 
            this.filenameBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.filenameBox.Name = "filenameBox";
            this.filenameBox.Size = new System.Drawing.Size(100, 39);
            this.filenameBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.filenameBox_KeyDown);
            this.filenameBox.TextChanged += new System.EventHandler(this.filenameBox_TextChanged);
            // 
            // filenameOkButton
            // 
            this.filenameOkButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.filenameOkButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.filenameOkButton.Name = "filenameOkButton";
            this.filenameOkButton.Size = new System.Drawing.Size(27, 36);
            this.filenameOkButton.Text = "OK";
            this.filenameOkButton.Visible = false;
            this.filenameOkButton.Click += new System.EventHandler(this.filenameOkButton_Click);
            // 
            // filenameCancelButton
            // 
            this.filenameCancelButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.filenameCancelButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.filenameCancelButton.Name = "filenameCancelButton";
            this.filenameCancelButton.Size = new System.Drawing.Size(47, 36);
            this.filenameCancelButton.Text = "Cancel";
            this.filenameCancelButton.Visible = false;
            this.filenameCancelButton.Click += new System.EventHandler(this.filenameCancelButton_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(6, 39);
            // 
            // ToolStripLabel1
            // 
            this.ToolStripLabel1.Name = "ToolStripLabel1";
            this.ToolStripLabel1.Size = new System.Drawing.Size(33, 36);
            this.ToolStripLabel1.Text = "Title:";
            // 
            // TitleBox
            // 
            this.TitleBox.AutoSize = false;
            this.TitleBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TitleBox.MaxLength = 50;
            this.TitleBox.Name = "TitleBox";
            this.TitleBox.Size = new System.Drawing.Size(120, 23);
            this.TitleBox.TextChanged += new System.EventHandler(this.TitleBoxTextChanged);
            // 
            // ToolStripSeparator4
            // 
            this.ToolStripSeparator4.Name = "ToolStripSeparator4";
            this.ToolStripSeparator4.Size = new System.Drawing.Size(6, 39);
            // 
            // ToolStripLabel2
            // 
            this.ToolStripLabel2.Name = "ToolStripLabel2";
            this.ToolStripLabel2.Size = new System.Drawing.Size(52, 36);
            this.ToolStripLabel2.Text = "LGR File:";
            // 
            // LGRBox
            // 
            this.LGRBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LGRBox.MaxLength = 8;
            this.LGRBox.Name = "LGRBox";
            this.LGRBox.Size = new System.Drawing.Size(119, 23);
            // 
            // ToolStripSeparator5
            // 
            this.ToolStripSeparator5.Name = "ToolStripSeparator5";
            this.ToolStripSeparator5.Size = new System.Drawing.Size(6, 39);
            // 
            // ToolStripLabel3
            // 
            this.ToolStripLabel3.Name = "ToolStripLabel3";
            this.ToolStripLabel3.Size = new System.Drawing.Size(50, 15);
            this.ToolStripLabel3.Text = "Ground:";
            // 
            // GroundComboBox
            // 
            this.GroundComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.GroundComboBox.Name = "GroundComboBox";
            this.GroundComboBox.Size = new System.Drawing.Size(119, 23);
            this.GroundComboBox.DropDownClosed += new System.EventHandler(this.MoveFocus);
            this.GroundComboBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyHandlerDown);
            // 
            // ToolStripSeparator6
            // 
            this.ToolStripSeparator6.Name = "ToolStripSeparator6";
            this.ToolStripSeparator6.Size = new System.Drawing.Size(6, 39);
            // 
            // ToolStripLabel4
            // 
            this.ToolStripLabel4.Name = "ToolStripLabel4";
            this.ToolStripLabel4.Size = new System.Drawing.Size(28, 15);
            this.ToolStripLabel4.Text = "Sky:";
            // 
            // SkyComboBox
            // 
            this.SkyComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SkyComboBox.Name = "SkyComboBox";
            this.SkyComboBox.Size = new System.Drawing.Size(119, 23);
            this.SkyComboBox.DropDownClosed += new System.EventHandler(this.MoveFocus);
            this.SkyComboBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyHandlerDown);
            // 
            // ToolStripSeparator7
            // 
            this.ToolStripSeparator7.Name = "ToolStripSeparator7";
            this.ToolStripSeparator7.Size = new System.Drawing.Size(6, 39);
            // 
            // ToolStrip2
            // 
            this.ToolStrip2.BackColor = System.Drawing.SystemColors.Control;
            this.ToolStrip2.Dock = System.Windows.Forms.DockStyle.None;
            this.ToolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.ToolStrip2.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.ToolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
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
            this.topologyList});
            this.ToolStrip2.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.ToolStrip2.Location = new System.Drawing.Point(3, 39);
            this.ToolStrip2.Name = "ToolStrip2";
            this.ToolStrip2.Size = new System.Drawing.Size(795, 39);
            this.ToolStrip2.TabIndex = 15;
            // 
            // ShowGridButton
            // 
            this.ShowGridButton.CheckOnClick = true;
            this.ShowGridButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ShowGridButton.Image = global::My.Resources.Resources.Grid;
            this.ShowGridButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ShowGridButton.Name = "ShowGridButton";
            this.ShowGridButton.Size = new System.Drawing.Size(36, 36);
            this.ShowGridButton.Text = "S&how grid";
            // 
            // snapToGridButton
            // 
            this.snapToGridButton.CheckOnClick = true;
            this.snapToGridButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.snapToGridButton.Image = global::My.Resources.Resources.Snap;
            this.snapToGridButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.snapToGridButton.Name = "snapToGridButton";
            this.snapToGridButton.Size = new System.Drawing.Size(36, 36);
            this.snapToGridButton.Text = "Snap to grid";
            // 
            // showCrossHairButton
            // 
            this.showCrossHairButton.CheckOnClick = true;
            this.showCrossHairButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.showCrossHairButton.Image = global::My.Resources.Resources.Crosshair2;
            this.showCrossHairButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.showCrossHairButton.Name = "showCrossHairButton";
            this.showCrossHairButton.Size = new System.Drawing.Size(36, 36);
            this.showCrossHairButton.Text = "Show crosshair";
            // 
            // ShowGrassEdgesButton
            // 
            this.ShowGrassEdgesButton.CheckOnClick = true;
            this.ShowGrassEdgesButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ShowGrassEdgesButton.Image = global::My.Resources.Resources.GrassEdges;
            this.ShowGrassEdgesButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ShowGrassEdgesButton.Name = "ShowGrassEdgesButton";
            this.ShowGrassEdgesButton.Size = new System.Drawing.Size(36, 36);
            this.ShowGrassEdgesButton.Text = "Show grass edges";
            // 
            // ShowGroundEdgesButton
            // 
            this.ShowGroundEdgesButton.CheckOnClick = true;
            this.ShowGroundEdgesButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ShowGroundEdgesButton.Image = global::My.Resources.Resources.Edges;
            this.ShowGroundEdgesButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ShowGroundEdgesButton.Name = "ShowGroundEdgesButton";
            this.ShowGroundEdgesButton.Size = new System.Drawing.Size(36, 36);
            this.ShowGroundEdgesButton.Text = "Show ground edges";
            // 
            // ShowVerticesButton
            // 
            this.ShowVerticesButton.CheckOnClick = true;
            this.ShowVerticesButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ShowVerticesButton.Image = global::My.Resources.Resources.Vertices;
            this.ShowVerticesButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ShowVerticesButton.Name = "ShowVerticesButton";
            this.ShowVerticesButton.Size = new System.Drawing.Size(36, 36);
            this.ShowVerticesButton.Text = "Show vertices";
            // 
            // ShowTextureFramesButton
            // 
            this.ShowTextureFramesButton.CheckOnClick = true;
            this.ShowTextureFramesButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ShowTextureFramesButton.Image = global::My.Resources.Resources.TextureFrame;
            this.ShowTextureFramesButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ShowTextureFramesButton.Name = "ShowTextureFramesButton";
            this.ShowTextureFramesButton.Size = new System.Drawing.Size(36, 36);
            this.ShowTextureFramesButton.Text = "Show texture frames";
            // 
            // ShowPictureFramesButton
            // 
            this.ShowPictureFramesButton.CheckOnClick = true;
            this.ShowPictureFramesButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ShowPictureFramesButton.Image = global::My.Resources.Resources.PictureFrame;
            this.ShowPictureFramesButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ShowPictureFramesButton.Name = "ShowPictureFramesButton";
            this.ShowPictureFramesButton.Size = new System.Drawing.Size(36, 36);
            this.ShowPictureFramesButton.Text = "Show picture frames";
            // 
            // ShowTexturesButton
            // 
            this.ShowTexturesButton.CheckOnClick = true;
            this.ShowTexturesButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ShowTexturesButton.Image = global::My.Resources.Resources.Texture;
            this.ShowTexturesButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ShowTexturesButton.Name = "ShowTexturesButton";
            this.ShowTexturesButton.Size = new System.Drawing.Size(36, 36);
            this.ShowTexturesButton.Text = "Show textures";
            // 
            // ShowPicturesButton
            // 
            this.ShowPicturesButton.CheckOnClick = true;
            this.ShowPicturesButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ShowPicturesButton.Image = global::My.Resources.Resources.Picture;
            this.ShowPicturesButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ShowPicturesButton.Name = "ShowPicturesButton";
            this.ShowPicturesButton.Size = new System.Drawing.Size(36, 36);
            this.ShowPicturesButton.Text = "Show pictures";
            // 
            // ShowObjectFramesButton
            // 
            this.ShowObjectFramesButton.CheckOnClick = true;
            this.ShowObjectFramesButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ShowObjectFramesButton.Image = global::My.Resources.Resources.ObjectFrame;
            this.ShowObjectFramesButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ShowObjectFramesButton.Name = "ShowObjectFramesButton";
            this.ShowObjectFramesButton.Size = new System.Drawing.Size(36, 36);
            this.ShowObjectFramesButton.Text = "Show object frames";
            // 
            // ShowObjectsButton
            // 
            this.ShowObjectsButton.CheckOnClick = true;
            this.ShowObjectsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ShowObjectsButton.Image = global::My.Resources.Resources._Object;
            this.ShowObjectsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ShowObjectsButton.Name = "ShowObjectsButton";
            this.ShowObjectsButton.Size = new System.Drawing.Size(36, 36);
            this.ShowObjectsButton.Text = "Show objects";
            // 
            // ShowGravityAppleArrowsButton
            // 
            this.ShowGravityAppleArrowsButton.CheckOnClick = true;
            this.ShowGravityAppleArrowsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ShowGravityAppleArrowsButton.Image = global::My.Resources.Resources.AppleArrow;
            this.ShowGravityAppleArrowsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ShowGravityAppleArrowsButton.Name = "ShowGravityAppleArrowsButton";
            this.ShowGravityAppleArrowsButton.Size = new System.Drawing.Size(36, 36);
            this.ShowGravityAppleArrowsButton.Text = "Show gravity apple arrows";
            // 
            // ShowGroundButton
            // 
            this.ShowGroundButton.CheckOnClick = true;
            this.ShowGroundButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ShowGroundButton.Image = global::My.Resources.Resources.GroundFill;
            this.ShowGroundButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ShowGroundButton.Name = "ShowGroundButton";
            this.ShowGroundButton.Size = new System.Drawing.Size(36, 36);
            this.ShowGroundButton.Text = "Show ground";
            // 
            // ShowGroundTextureButton
            // 
            this.ShowGroundTextureButton.CheckOnClick = true;
            this.ShowGroundTextureButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ShowGroundTextureButton.Image = global::My.Resources.Resources.Ground;
            this.ShowGroundTextureButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ShowGroundTextureButton.Name = "ShowGroundTextureButton";
            this.ShowGroundTextureButton.Size = new System.Drawing.Size(36, 36);
            this.ShowGroundTextureButton.Text = "Show ground texture";
            // 
            // ShowSkyTextureButton
            // 
            this.ShowSkyTextureButton.CheckOnClick = true;
            this.ShowSkyTextureButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ShowSkyTextureButton.Image = global::My.Resources.Resources.Sky;
            this.ShowSkyTextureButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ShowSkyTextureButton.Name = "ShowSkyTextureButton";
            this.ShowSkyTextureButton.Size = new System.Drawing.Size(36, 36);
            this.ShowSkyTextureButton.Text = "Show sky texture";
            // 
            // ZoomTexturesButton
            // 
            this.ZoomTexturesButton.CheckOnClick = true;
            this.ZoomTexturesButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ZoomTexturesButton.Image = global::My.Resources.Resources.ZoomTexture;
            this.ZoomTexturesButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ZoomTexturesButton.Name = "ZoomTexturesButton";
            this.ZoomTexturesButton.Size = new System.Drawing.Size(36, 36);
            this.ZoomTexturesButton.Text = "Zoom textures";
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(6, 39);
            // 
            // BestTimeLabel
            // 
            this.BestTimeLabel.AutoSize = false;
            this.BestTimeLabel.Name = "BestTimeLabel";
            this.BestTimeLabel.Size = new System.Drawing.Size(170, 36);
            this.BestTimeLabel.Text = "Best time: None";
            this.BestTimeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // topologyList
            // 
            this.topologyList.AutoToolTip = false;
            this.topologyList.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.topologyList.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.topologyList.Name = "topologyList";
            this.topologyList.ShowDropDownArrow = false;
            this.topologyList.Size = new System.Drawing.Size(4, 36);
            // 
            // StatusStrip2
            // 
            this.StatusStrip2.BackColor = System.Drawing.SystemColors.Control;
            this.StatusStrip2.Dock = System.Windows.Forms.DockStyle.None;
            this.StatusStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.InfoLabel});
            this.StatusStrip2.Location = new System.Drawing.Point(0, 78);
            this.StatusStrip2.Name = "StatusStrip2";
            this.StatusStrip2.ShowItemToolTips = true;
            this.StatusStrip2.Size = new System.Drawing.Size(929, 22);
            this.StatusStrip2.SizingGrip = false;
            this.StatusStrip2.TabIndex = 16;
            // 
            // InfoLabel
            // 
            this.InfoLabel.AutoToolTip = true;
            this.InfoLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.InfoLabel.Name = "InfoLabel";
            this.InfoLabel.Size = new System.Drawing.Size(42, 17);
            this.InfoLabel.Text = "Ready.";
            this.InfoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // EditorMenuStrip
            // 
            this.EditorMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
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
            this.convertToToolStripMenuItem});
            this.EditorMenuStrip.Name = "SelectedMenuStrip";
            this.EditorMenuStrip.Size = new System.Drawing.Size(168, 290);
            // 
            // CopyMenuItem
            // 
            this.CopyMenuItem.Name = "CopyMenuItem";
            this.CopyMenuItem.Size = new System.Drawing.Size(167, 22);
            this.CopyMenuItem.Text = "Copy";
            this.CopyMenuItem.Click += new System.EventHandler(this.CopyMenuItemClick);
            // 
            // TransformMenuItem
            // 
            this.TransformMenuItem.Name = "TransformMenuItem";
            this.TransformMenuItem.Size = new System.Drawing.Size(167, 22);
            this.TransformMenuItem.Text = "Transform";
            this.TransformMenuItem.Click += new System.EventHandler(this.TransformMenuItemClick);
            // 
            // DeleteMenuItem
            // 
            this.DeleteMenuItem.Name = "DeleteMenuItem";
            this.DeleteMenuItem.Size = new System.Drawing.Size(167, 22);
            this.DeleteMenuItem.Text = "Delete";
            this.DeleteMenuItem.Click += new System.EventHandler(this.DeleteSelected);
            // 
            // GrassMenuItem
            // 
            this.GrassMenuItem.CheckOnClick = true;
            this.GrassMenuItem.Name = "GrassMenuItem";
            this.GrassMenuItem.Size = new System.Drawing.Size(167, 22);
            this.GrassMenuItem.Text = "Grass";
            this.GrassMenuItem.Click += new System.EventHandler(this.HandleGrassMenu);
            // 
            // GravityNoneMenuItem
            // 
            this.GravityNoneMenuItem.Checked = true;
            this.GravityNoneMenuItem.CheckOnClick = true;
            this.GravityNoneMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.GravityNoneMenuItem.Name = "GravityNoneMenuItem";
            this.GravityNoneMenuItem.Size = new System.Drawing.Size(167, 22);
            this.GravityNoneMenuItem.Text = "Gravity none";
            this.GravityNoneMenuItem.Click += new System.EventHandler(this.HandleGravityMenu);
            // 
            // GravityUpMenuItem
            // 
            this.GravityUpMenuItem.CheckOnClick = true;
            this.GravityUpMenuItem.Name = "GravityUpMenuItem";
            this.GravityUpMenuItem.Size = new System.Drawing.Size(167, 22);
            this.GravityUpMenuItem.Text = "Gravity up";
            this.GravityUpMenuItem.Click += new System.EventHandler(this.HandleGravityMenu);
            // 
            // GravityDownMenuItem
            // 
            this.GravityDownMenuItem.CheckOnClick = true;
            this.GravityDownMenuItem.Name = "GravityDownMenuItem";
            this.GravityDownMenuItem.Size = new System.Drawing.Size(167, 22);
            this.GravityDownMenuItem.Text = "Gravity down";
            this.GravityDownMenuItem.Click += new System.EventHandler(this.HandleGravityMenu);
            // 
            // GravityLeftMenuItem
            // 
            this.GravityLeftMenuItem.CheckOnClick = true;
            this.GravityLeftMenuItem.Name = "GravityLeftMenuItem";
            this.GravityLeftMenuItem.Size = new System.Drawing.Size(167, 22);
            this.GravityLeftMenuItem.Text = "Gravity left";
            this.GravityLeftMenuItem.Click += new System.EventHandler(this.HandleGravityMenu);
            // 
            // GravityRightMenuItem
            // 
            this.GravityRightMenuItem.CheckOnClick = true;
            this.GravityRightMenuItem.Name = "GravityRightMenuItem";
            this.GravityRightMenuItem.Size = new System.Drawing.Size(167, 22);
            this.GravityRightMenuItem.Text = "Gravity right";
            this.GravityRightMenuItem.Click += new System.EventHandler(this.HandleGravityMenu);
            // 
            // PicturePropertiesMenuItem
            // 
            this.PicturePropertiesMenuItem.Name = "PicturePropertiesMenuItem";
            this.PicturePropertiesMenuItem.Size = new System.Drawing.Size(167, 22);
            this.PicturePropertiesMenuItem.Text = "Picture properties";
            this.PicturePropertiesMenuItem.Click += new System.EventHandler(this.PicturePropertiesToolStripMenuItemClick);
            // 
            // bringToFrontToolStripMenuItem
            // 
            this.bringToFrontToolStripMenuItem.Name = "bringToFrontToolStripMenuItem";
            this.bringToFrontToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.bringToFrontToolStripMenuItem.Text = "Bring to front";
            this.bringToFrontToolStripMenuItem.Click += new System.EventHandler(this.BringToFrontToolStripMenuItemClick);
            // 
            // sendToBackToolStripMenuItem
            // 
            this.sendToBackToolStripMenuItem.Name = "sendToBackToolStripMenuItem";
            this.sendToBackToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.sendToBackToolStripMenuItem.Text = "Send to back";
            this.sendToBackToolStripMenuItem.Click += new System.EventHandler(this.SendToBackToolStripMenuItemClick);
            // 
            // convertToToolStripMenuItem
            // 
            this.convertToToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.applesConvertItem,
            this.killersConvertItem,
            this.flowersConvertItem,
            this.picturesConvertItem});
            this.convertToToolStripMenuItem.Name = "convertToToolStripMenuItem";
            this.convertToToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.convertToToolStripMenuItem.Text = "Convert to";
            // 
            // applesConvertItem
            // 
            this.applesConvertItem.Name = "applesConvertItem";
            this.applesConvertItem.Size = new System.Drawing.Size(116, 22);
            this.applesConvertItem.Text = "Apples";
            this.applesConvertItem.Click += new System.EventHandler(this.ConvertClicked);
            // 
            // killersConvertItem
            // 
            this.killersConvertItem.Name = "killersConvertItem";
            this.killersConvertItem.Size = new System.Drawing.Size(116, 22);
            this.killersConvertItem.Text = "Killers";
            this.killersConvertItem.Click += new System.EventHandler(this.ConvertClicked);
            // 
            // flowersConvertItem
            // 
            this.flowersConvertItem.Name = "flowersConvertItem";
            this.flowersConvertItem.Size = new System.Drawing.Size(116, 22);
            this.flowersConvertItem.Text = "Flowers";
            this.flowersConvertItem.Click += new System.EventHandler(this.ConvertClicked);
            // 
            // picturesConvertItem
            // 
            this.picturesConvertItem.Name = "picturesConvertItem";
            this.picturesConvertItem.Size = new System.Drawing.Size(116, 22);
            this.picturesConvertItem.Text = "Pictures";
            this.picturesConvertItem.Click += new System.EventHandler(this.ConvertClicked);
            // 
            // saveAsPictureDialog
            // 
            this.saveAsPictureDialog.DefaultExt = "png";
            this.saveAsPictureDialog.FileName = "Untitled";
            this.saveAsPictureDialog.Filter = "Portable Network Graphics (*.png)|*.png";
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
            this.ToolPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.ToolPanel.Location = new System.Drawing.Point(0, 124);
            this.ToolPanel.Name = "ToolPanel";
            this.ToolPanel.Size = new System.Drawing.Size(84, 331);
            this.ToolPanel.TabIndex = 3;
            this.ToolPanel.Text = "Tools";
            // 
            // TextButton
            // 
            this.TextButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.TextButton.AutoSize = true;
            this.TextButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.TextButton.Location = new System.Drawing.Point(0, 299);
            this.TextButton.Name = "TextButton";
            this.TextButton.Size = new System.Drawing.Size(84, 23);
            this.TextButton.TabIndex = 16;
            this.TextButton.Text = "&Text";
            this.TextButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.TextButton.UseVisualStyleBackColor = true;
            this.TextButton.CheckedChanged += new System.EventHandler(this.TextButton_CheckedChanged);
            // 
            // PictureButton
            // 
            this.PictureButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.PictureButton.AutoSize = true;
            this.PictureButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.PictureButton.Location = new System.Drawing.Point(0, 276);
            this.PictureButton.Name = "PictureButton";
            this.PictureButton.Size = new System.Drawing.Size(84, 23);
            this.PictureButton.TabIndex = 15;
            this.PictureButton.Text = "P&icture";
            this.PictureButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.PictureButton.UseVisualStyleBackColor = true;
            // 
            // AutoGrassButton
            // 
            this.AutoGrassButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.AutoGrassButton.AutoSize = true;
            this.AutoGrassButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.AutoGrassButton.Location = new System.Drawing.Point(0, 253);
            this.AutoGrassButton.Name = "AutoGrassButton";
            this.AutoGrassButton.Size = new System.Drawing.Size(84, 23);
            this.AutoGrassButton.TabIndex = 14;
            this.AutoGrassButton.Text = "&AutoGrass";
            this.AutoGrassButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.AutoGrassButton.UseVisualStyleBackColor = true;
            // 
            // CutConnectButton
            // 
            this.CutConnectButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.CutConnectButton.AutoSize = true;
            this.CutConnectButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.CutConnectButton.Location = new System.Drawing.Point(0, 230);
            this.CutConnectButton.Name = "CutConnectButton";
            this.CutConnectButton.Size = new System.Drawing.Size(84, 23);
            this.CutConnectButton.TabIndex = 13;
            this.CutConnectButton.Text = "&Cut/connect";
            this.CutConnectButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.CutConnectButton.UseVisualStyleBackColor = true;
            // 
            // SmoothenButton
            // 
            this.SmoothenButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.SmoothenButton.AutoSize = true;
            this.SmoothenButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.SmoothenButton.Location = new System.Drawing.Point(0, 207);
            this.SmoothenButton.Name = "SmoothenButton";
            this.SmoothenButton.Size = new System.Drawing.Size(84, 23);
            this.SmoothenButton.TabIndex = 12;
            this.SmoothenButton.Text = "(Un)s&moothen";
            this.SmoothenButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.SmoothenButton.UseVisualStyleBackColor = true;
            // 
            // FrameButton
            // 
            this.FrameButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.FrameButton.AutoSize = true;
            this.FrameButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.FrameButton.Location = new System.Drawing.Point(0, 184);
            this.FrameButton.Name = "FrameButton";
            this.FrameButton.Size = new System.Drawing.Size(84, 23);
            this.FrameButton.TabIndex = 11;
            this.FrameButton.Text = "&Frame";
            this.FrameButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.FrameButton.UseVisualStyleBackColor = true;
            // 
            // PolyOpButton
            // 
            this.PolyOpButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.PolyOpButton.AutoSize = true;
            this.PolyOpButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.PolyOpButton.Location = new System.Drawing.Point(0, 161);
            this.PolyOpButton.Name = "PolyOpButton";
            this.PolyOpButton.Size = new System.Drawing.Size(84, 23);
            this.PolyOpButton.TabIndex = 9;
            this.PolyOpButton.Text = "Po&lyOp";
            this.PolyOpButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.PolyOpButton.UseVisualStyleBackColor = true;
            // 
            // EllipseButton
            // 
            this.EllipseButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.EllipseButton.AutoSize = true;
            this.EllipseButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.EllipseButton.Location = new System.Drawing.Point(0, 138);
            this.EllipseButton.Name = "EllipseButton";
            this.EllipseButton.Size = new System.Drawing.Size(84, 23);
            this.EllipseButton.TabIndex = 8;
            this.EllipseButton.Text = "&Ellipse";
            this.EllipseButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.EllipseButton.UseVisualStyleBackColor = true;
            // 
            // ZoomButton
            // 
            this.ZoomButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.ZoomButton.AutoSize = true;
            this.ZoomButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.ZoomButton.Location = new System.Drawing.Point(0, 115);
            this.ZoomButton.Name = "ZoomButton";
            this.ZoomButton.Size = new System.Drawing.Size(84, 23);
            this.ZoomButton.TabIndex = 7;
            this.ZoomButton.Text = "&Zoom";
            this.ZoomButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ZoomButton.UseVisualStyleBackColor = true;
            // 
            // PipeButton
            // 
            this.PipeButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.PipeButton.AutoSize = true;
            this.PipeButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.PipeButton.Location = new System.Drawing.Point(0, 92);
            this.PipeButton.Name = "PipeButton";
            this.PipeButton.Size = new System.Drawing.Size(84, 23);
            this.PipeButton.TabIndex = 6;
            this.PipeButton.Text = "&Pipe";
            this.PipeButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.PipeButton.UseVisualStyleBackColor = true;
            // 
            // ObjectButton
            // 
            this.ObjectButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.ObjectButton.AutoSize = true;
            this.ObjectButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.ObjectButton.Location = new System.Drawing.Point(0, 69);
            this.ObjectButton.Name = "ObjectButton";
            this.ObjectButton.Size = new System.Drawing.Size(84, 23);
            this.ObjectButton.TabIndex = 3;
            this.ObjectButton.Text = "&Object";
            this.ObjectButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ObjectButton.UseVisualStyleBackColor = true;
            // 
            // DrawButton
            // 
            this.DrawButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.DrawButton.AutoSize = true;
            this.DrawButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.DrawButton.Location = new System.Drawing.Point(0, 46);
            this.DrawButton.Name = "DrawButton";
            this.DrawButton.Size = new System.Drawing.Size(84, 23);
            this.DrawButton.TabIndex = 2;
            this.DrawButton.Text = "&Draw";
            this.DrawButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.DrawButton.UseVisualStyleBackColor = true;
            // 
            // VertexButton
            // 
            this.VertexButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.VertexButton.AutoSize = true;
            this.VertexButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.VertexButton.Location = new System.Drawing.Point(0, 23);
            this.VertexButton.Name = "VertexButton";
            this.VertexButton.Size = new System.Drawing.Size(84, 23);
            this.VertexButton.TabIndex = 1;
            this.VertexButton.Text = "&Vertex";
            this.VertexButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.VertexButton.UseVisualStyleBackColor = true;
            // 
            // SelectButton
            // 
            this.SelectButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.SelectButton.AutoSize = true;
            this.SelectButton.Checked = true;
            this.SelectButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.SelectButton.Location = new System.Drawing.Point(0, 0);
            this.SelectButton.Name = "SelectButton";
            this.SelectButton.Size = new System.Drawing.Size(84, 23);
            this.SelectButton.TabIndex = 0;
            this.SelectButton.TabStop = true;
            this.SelectButton.Text = "&Select";
            this.SelectButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.SelectButton.UseVisualStyleBackColor = true;
            // 
            // importFileDialog
            // 
            this.importFileDialog.Filter = "Elasto Mania level or image (*.lev, *.bmp, *.png, *.gif, *.tiff, *.exif)|*.lev;*." +
    "bmp;*.png;*.gif;*.tiff;*.exif";
            this.importFileDialog.Multiselect = true;
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.C)));
            this.toolStripMenuItem2.Size = new System.Drawing.Size(239, 22);
            this.toolStripMenuItem2.Text = "Copy and snap to grid";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.CopyMenuItemClick);
            // 
            // LevelEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(929, 479);
            this.Controls.Add(this.EditorControl);
            this.Controls.Add(this.ToolPanel);
            this.Controls.Add(this.StatusStrip1);
            this.Controls.Add(this.ToolStripPanel1);
            this.Controls.Add(this.MenuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.MenuStrip1;
            this.Name = "LevelEditor";
            this.Text = "SLE";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConfirmClose);
            this.Load += new System.EventHandler(this.RefreshOnOpen);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyHandlerDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.KeyHandlerUp);
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
            this.StatusStrip2.ResumeLayout(false);
            this.StatusStrip2.PerformLayout();
            this.EditorMenuStrip.ResumeLayout(false);
            this.ToolPanel.ResumeLayout(false);
            this.ToolPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		internal System.Windows.Forms.MenuStrip MenuStrip1;
		internal System.Windows.Forms.ToolStripMenuItem FileToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem NewToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem OpenToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem SaveToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem SaveAsToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem ExitToolStripMenuItem;
		internal System.Windows.Forms.Panel EditorControl;
		internal System.Windows.Forms.OpenFileDialog OpenFileDialog1;
		internal PanelMod ToolPanel;
		internal RadioButtonMod EllipseButton;
		internal RadioButtonMod ObjectButton;
		internal RadioButtonMod VertexButton;
		internal RadioButtonMod PipeButton;
		internal RadioButtonMod SmoothenButton;
		internal RadioButtonMod CutConnectButton;
		internal RadioButtonMod SelectButton;
		internal System.Windows.Forms.StatusStrip StatusStrip1;
		internal System.Windows.Forms.ToolStripStatusLabel CoordinateLabel;
		internal System.Windows.Forms.SaveFileDialog SaveFileDialog1;
		internal System.Windows.Forms.ToolStripPanel ToolStripPanel1;
		internal RadioButtonMod ZoomButton;
		internal System.Windows.Forms.ToolStripMenuItem ConfigurationToolStripMenuItem;
		internal RadioButtonMod PolyOpButton;
		internal RadioButtonMod DrawButton;
		internal System.Windows.Forms.ContextMenuStrip EditorMenuStrip;
		internal System.Windows.Forms.ToolStripMenuItem CopyMenuItem;
		internal RadioButtonMod FrameButton;
		internal System.Windows.Forms.ToolStripMenuItem TransformMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem ActionsMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem QuickGrassToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem UndoToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem RedoToolStripMenuItem;
		internal RadioButtonMod AutoGrassButton;
		internal System.Windows.Forms.ToolStripMenuItem DeleteAllGrassToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem DeleteMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem MirrorLevelToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem GrassMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem GravityNoneMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem GravityUpMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem GravityDownMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem GravityLeftMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem GravityRightMenuItem;
		internal RadioButtonMod PictureButton;
		internal System.Windows.Forms.ToolStripMenuItem PicturePropertiesMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem SelectionFilterToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem EnableAllToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem DisableAllToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem GroundPolygonsToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem GrassPolygonsToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem ApplesToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem KillersToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem FlowersToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem PicturesToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem TexturesToolStripMenuItem;
		internal System.Windows.Forms.ToolStrip ToolStrip1;
		internal System.Windows.Forms.ToolStripButton NewButton;
		internal System.Windows.Forms.ToolStripButton OpenButton;
		internal System.Windows.Forms.ToolStripButton SaveButton;
		internal System.Windows.Forms.ToolStripButton SaveAsButton;
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator1;
		internal System.Windows.Forms.ToolStripButton CheckTopologyButton;
		internal System.Windows.Forms.ToolStripButton ZoomFillButton;
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator2;
		internal System.Windows.Forms.ToolStripButton UndoButton;
		internal System.Windows.Forms.ToolStripButton RedoButton;
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator3;
		internal System.Windows.Forms.ToolStripTextBox TitleBox;
		internal System.Windows.Forms.ToolStripLabel ToolStripLabel1;
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator4;
		internal System.Windows.Forms.ToolStripLabel ToolStripLabel2;
		internal System.Windows.Forms.ToolStripTextBox LGRBox;
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator5;
		internal System.Windows.Forms.ToolStripLabel ToolStripLabel3;
		internal System.Windows.Forms.ToolStripComboBox GroundComboBox;
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator6;
		internal System.Windows.Forms.ToolStripLabel ToolStripLabel4;
		internal System.Windows.Forms.ToolStripComboBox SkyComboBox;
		internal System.Windows.Forms.ToolStrip ToolStrip2;
		internal System.Windows.Forms.ToolStripButton ShowGrassEdgesButton;
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator7;
		internal System.Windows.Forms.ToolStripStatusLabel SelectionLabel;
		internal System.Windows.Forms.ToolStripButton ShowGroundEdgesButton;
		internal System.Windows.Forms.ToolStripButton ShowGridButton;
		internal System.Windows.Forms.ToolStripButton ShowVerticesButton;
		internal System.Windows.Forms.ToolStripButton ShowTextureFramesButton;
		internal System.Windows.Forms.ToolStripButton ShowPictureFramesButton;
		internal System.Windows.Forms.ToolStripButton ShowTexturesButton;
		internal System.Windows.Forms.ToolStripButton ShowPicturesButton;
		internal System.Windows.Forms.ToolStripButton ShowObjectFramesButton;
		internal System.Windows.Forms.ToolStripButton ShowObjectsButton;
		internal System.Windows.Forms.ToolStripButton ShowGroundButton;
		internal System.Windows.Forms.ToolStripButton ShowGroundTextureButton;
		internal System.Windows.Forms.ToolStripButton ShowSkyTextureButton;
		internal System.Windows.Forms.ToolStripMenuItem DeleteSelectedMenuItem;
		internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator8;
		internal System.Windows.Forms.ToolStripMenuItem ZoomFillToolStripMenuItem;
		internal System.Windows.Forms.ToolStripStatusLabel HighlightLabel;
		internal System.Windows.Forms.StatusStrip StatusStrip2;
        internal System.Windows.Forms.ToolStripStatusLabel InfoLabel;
		internal System.Windows.Forms.ToolStripMenuItem CheckTopologyMenuItem;
		internal System.Windows.Forms.ToolStripButton ZoomTexturesButton;
		internal System.Windows.Forms.ToolStripMenuItem MainConfigMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem RenderingSettingsToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem LevelPropertiesToolStripMenuItem;
        private System.ComponentModel.IContainer components;
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
        private ToolStripMenuItem toolStripMenuItem1;
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
        private ToolStripMenuItem toolStripMenuItem2;
    }
	
}
