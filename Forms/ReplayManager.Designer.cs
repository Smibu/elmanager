using System.Windows.Forms;
using Elmanager.CustomControls;

namespace Elmanager.Forms
{
	public partial class ReplayManager : System.Windows.Forms.Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReplayManager));
            this.ContextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SelectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.InvertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RemoveFromListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CopyToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MoveToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenLevelMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RenameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RenamePatternToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CompareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenViewerMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MergeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveListToTextFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DeleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uploadToZworqyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zworqyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.k10xnetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SearchButton = new System.Windows.Forms.Button();
            this.PatternLabel = new System.Windows.Forms.Label();
            this.PatternBox = new System.Windows.Forms.TextBox();
            this.LevPatternBox = new System.Windows.Forms.TextBox();
            this.SearchSpecificLabel = new System.Windows.Forms.Label();
            this.Label7 = new System.Windows.Forms.Label();
            this.Label1 = new System.Windows.Forms.Label();
            this.SelectedReplaysLabel = new System.Windows.Forms.Label();
            this.SaveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.RList = new BrightIdeasSoftware.ObjectListView();
            this.OlvColumn1 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.OlvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.OlvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.OlvColumn4 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.OlvColumn5 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.OlvColumn6 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.OlvColumn7 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn8 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn9 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ConfigButton = new System.Windows.Forms.Button();
            this.FolderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.TabControl1 = new System.Windows.Forms.TabControl();
            this.TabPage1 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.singleMultiSelect = new Elmanager.CustomControls.TriSelect();
            this.finishedSelect = new Elmanager.CustomControls.TriSelect();
            this.elmaAcrossSelect = new Elmanager.CustomControls.TriSelect();
            this.intExtSelect = new Elmanager.CustomControls.TriSelect();
            this.fastestSlowestSelect = new Elmanager.CustomControls.TriSelect();
            this.panel1 = new System.Windows.Forms.Panel();
            this.minFileSizeBox = new Elmanager.CustomControls.NumericTextBox();
            this.TimeMaxBox = new Elmanager.CustomControls.TimeTextBox();
            this.TimeMinBox = new Elmanager.CustomControls.TimeTextBox();
            this.maxDateTime = new System.Windows.Forms.DateTimePicker();
            this.maxFileSizeBox = new Elmanager.CustomControls.NumericTextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.minDateTime = new System.Windows.Forms.DateTimePicker();
            this.label25 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.TabPage2 = new System.Windows.Forms.TabPage();
            this.ResetButton = new System.Windows.Forms.Button();
            this.Label15 = new System.Windows.Forms.Label();
            this.Label16 = new System.Windows.Forms.Label();
            this.Label17 = new System.Windows.Forms.Label();
            this.Label18 = new System.Windows.Forms.Label();
            this.Label19 = new System.Windows.Forms.Label();
            this.Label20 = new System.Windows.Forms.Label();
            this.Label14 = new System.Windows.Forms.Label();
            this.Label13 = new System.Windows.Forms.Label();
            this.Label12 = new System.Windows.Forms.Label();
            this.Label11 = new System.Windows.Forms.Label();
            this.Label10 = new System.Windows.Forms.Label();
            this.Label9 = new System.Windows.Forms.Label();
            this.Label8 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.Label6 = new System.Windows.Forms.Label();
            this.Label5 = new System.Windows.Forms.Label();
            this.Label4 = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.Label21 = new System.Windows.Forms.Label();
            this.Label22 = new System.Windows.Forms.Label();
            this.TextBox13 = new Elmanager.CustomControls.NumericTextBox();
            this.TextBox12 = new Elmanager.CustomControls.NumericTextBox();
            this.TextBox15 = new Elmanager.CustomControls.NumericTextBox();
            this.TextBox16 = new Elmanager.CustomControls.NumericTextBox();
            this.TextBox5 = new Elmanager.CustomControls.NumericTextBox();
            this.TextBox18 = new Elmanager.CustomControls.NumericTextBox();
            this.TextBox7 = new Elmanager.CustomControls.NumericTextBox();
            this.TextBox20 = new Elmanager.CustomControls.NumericTextBox();
            this.TextBox9 = new Elmanager.CustomControls.NumericTextBox();
            this.TextBox22 = new Elmanager.CustomControls.NumericTextBox();
            this.TextBox10 = new Elmanager.CustomControls.NumericTextBox();
            this.TextBox3 = new Elmanager.CustomControls.NumericTextBox();
            this.TextBox24 = new Elmanager.CustomControls.NumericTextBox();
            this.TextBox11 = new Elmanager.CustomControls.NumericTextBox();
            this.TextBox23 = new Elmanager.CustomControls.NumericTextBox();
            this.TextBox14 = new Elmanager.CustomControls.NumericTextBox();
            this.TextBox8 = new Elmanager.CustomControls.NumericTextBox();
            this.TextBox17 = new Elmanager.CustomControls.NumericTextBox();
            this.TextBox6 = new Elmanager.CustomControls.NumericTextBox();
            this.TextBox19 = new Elmanager.CustomControls.NumericTextBox();
            this.TextBox4 = new Elmanager.CustomControls.NumericTextBox();
            this.TextBox21 = new Elmanager.CustomControls.NumericTextBox();
            this.TextBox2 = new Elmanager.CustomControls.NumericTextBox();
            this.TextBox1 = new Elmanager.CustomControls.NumericTextBox();
            this.TabPage3 = new System.Windows.Forms.TabPage();
            this.ReplaysIncorrectLevButton = new System.Windows.Forms.Button();
            this.ReplaysWithoutLevFileButton = new System.Windows.Forms.Button();
            this.DuplicateFilenameButton = new System.Windows.Forms.Button();
            this.DuplicateButton = new System.Windows.Forms.Button();
            this.StatusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.ToolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.ContextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RList)).BeginInit();
            this.TabControl1.SuspendLayout();
            this.TabPage1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.TabPage2.SuspendLayout();
            this.TabPage3.SuspendLayout();
            this.StatusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ContextMenuStrip1
            // 
            this.ContextMenuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.ContextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SelectAllToolStripMenuItem,
            this.InvertToolStripMenuItem,
            this.RemoveFromListToolStripMenuItem,
            this.CopyToToolStripMenuItem,
            this.MoveToToolStripMenuItem,
            this.OpenLevelMenuItem,
            this.RenameToolStripMenuItem,
            this.RenamePatternToolStripMenuItem,
            this.CompareToolStripMenuItem,
            this.OpenViewerMenuItem,
            this.MergeToolStripMenuItem,
            this.SaveListToTextFileToolStripMenuItem,
            this.DeleteToolStripMenuItem,
            this.uploadToZworqyToolStripMenuItem});
            this.ContextMenuStrip1.Name = "ContextMenuStrip1";
            this.ContextMenuStrip1.Size = new System.Drawing.Size(357, 508);
            // 
            // SelectAllToolStripMenuItem
            // 
            this.SelectAllToolStripMenuItem.Name = "SelectAllToolStripMenuItem";
            this.SelectAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.SelectAllToolStripMenuItem.Size = new System.Drawing.Size(356, 36);
            this.SelectAllToolStripMenuItem.Text = "Select all";
            this.SelectAllToolStripMenuItem.Click += new System.EventHandler(this.SelectAll);
            // 
            // InvertToolStripMenuItem
            // 
            this.InvertToolStripMenuItem.Name = "InvertToolStripMenuItem";
            this.InvertToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.InvertToolStripMenuItem.Size = new System.Drawing.Size(356, 36);
            this.InvertToolStripMenuItem.Text = "Invert selection";
            this.InvertToolStripMenuItem.Click += new System.EventHandler(this.InvertSelection);
            // 
            // RemoveFromListToolStripMenuItem
            // 
            this.RemoveFromListToolStripMenuItem.Name = "RemoveFromListToolStripMenuItem";
            this.RemoveFromListToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.RemoveFromListToolStripMenuItem.Size = new System.Drawing.Size(356, 36);
            this.RemoveFromListToolStripMenuItem.Text = "Remove from list";
            this.RemoveFromListToolStripMenuItem.Click += new System.EventHandler(this.RemoveReplays);
            // 
            // CopyToToolStripMenuItem
            // 
            this.CopyToToolStripMenuItem.Name = "CopyToToolStripMenuItem";
            this.CopyToToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.CopyToToolStripMenuItem.Size = new System.Drawing.Size(356, 36);
            this.CopyToToolStripMenuItem.Text = "Copy to...";
            this.CopyToToolStripMenuItem.Click += new System.EventHandler(this.MoveOrCopy);
            // 
            // MoveToToolStripMenuItem
            // 
            this.MoveToToolStripMenuItem.Name = "MoveToToolStripMenuItem";
            this.MoveToToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.MoveToToolStripMenuItem.Size = new System.Drawing.Size(356, 36);
            this.MoveToToolStripMenuItem.Text = "Move to...";
            this.MoveToToolStripMenuItem.Click += new System.EventHandler(this.MoveOrCopy);
            // 
            // OpenLevelMenuItem
            // 
            this.OpenLevelMenuItem.Name = "OpenLevelMenuItem";
            this.OpenLevelMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.OpenLevelMenuItem.Size = new System.Drawing.Size(356, 36);
            this.OpenLevelMenuItem.Text = "Open level file";
            this.OpenLevelMenuItem.Click += new System.EventHandler(this.OpenLevelMenuItemClick);
            // 
            // RenameToolStripMenuItem
            // 
            this.RenameToolStripMenuItem.Name = "RenameToolStripMenuItem";
            this.RenameToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.RenameToolStripMenuItem.Size = new System.Drawing.Size(356, 36);
            this.RenameToolStripMenuItem.Text = "Rename";
            this.RenameToolStripMenuItem.Click += new System.EventHandler(this.Rename);
            // 
            // RenamePatternToolStripMenuItem
            // 
            this.RenamePatternToolStripMenuItem.Name = "RenamePatternToolStripMenuItem";
            this.RenamePatternToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.RenamePatternToolStripMenuItem.Size = new System.Drawing.Size(356, 36);
            this.RenamePatternToolStripMenuItem.Text = "Rename pattern...";
            this.RenamePatternToolStripMenuItem.Click += new System.EventHandler(this.RenamePattern);
            // 
            // CompareToolStripMenuItem
            // 
            this.CompareToolStripMenuItem.Name = "CompareToolStripMenuItem";
            this.CompareToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.CompareToolStripMenuItem.Size = new System.Drawing.Size(356, 36);
            this.CompareToolStripMenuItem.Text = "Compare";
            this.CompareToolStripMenuItem.Click += new System.EventHandler(this.Compare);
            // 
            // OpenViewerMenuItem
            // 
            this.OpenViewerMenuItem.Name = "OpenViewerMenuItem";
            this.OpenViewerMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.OpenViewerMenuItem.Size = new System.Drawing.Size(356, 36);
            this.OpenViewerMenuItem.Text = "Replay viewer";
            this.OpenViewerMenuItem.Click += new System.EventHandler(this.OpenViewer);
            // 
            // MergeToolStripMenuItem
            // 
            this.MergeToolStripMenuItem.Name = "MergeToolStripMenuItem";
            this.MergeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
            this.MergeToolStripMenuItem.Size = new System.Drawing.Size(356, 36);
            this.MergeToolStripMenuItem.Text = "Merge";
            this.MergeToolStripMenuItem.Click += new System.EventHandler(this.MergeReplays);
            // 
            // SaveListToTextFileToolStripMenuItem
            // 
            this.SaveListToTextFileToolStripMenuItem.Name = "SaveListToTextFileToolStripMenuItem";
            this.SaveListToTextFileToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.SaveListToTextFileToolStripMenuItem.Size = new System.Drawing.Size(356, 36);
            this.SaveListToTextFileToolStripMenuItem.Text = "Save to textfile";
            this.SaveListToTextFileToolStripMenuItem.Click += new System.EventHandler(this.SaveListToTextFile);
            // 
            // DeleteToolStripMenuItem
            // 
            this.DeleteToolStripMenuItem.Name = "DeleteToolStripMenuItem";
            this.DeleteToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.DeleteToolStripMenuItem.Size = new System.Drawing.Size(356, 36);
            this.DeleteToolStripMenuItem.Text = "Delete";
            this.DeleteToolStripMenuItem.Click += new System.EventHandler(this.DeleteReplays);
            // 
            // uploadToZworqyToolStripMenuItem
            // 
            this.uploadToZworqyToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zworqyToolStripMenuItem,
            this.k10xnetToolStripMenuItem});
            this.uploadToZworqyToolStripMenuItem.Name = "uploadToZworqyToolStripMenuItem";
            this.uploadToZworqyToolStripMenuItem.Size = new System.Drawing.Size(356, 36);
            this.uploadToZworqyToolStripMenuItem.Text = "Upload to";
            // 
            // zworqyToolStripMenuItem
            // 
            this.zworqyToolStripMenuItem.Name = "zworqyToolStripMenuItem";
            this.zworqyToolStripMenuItem.Size = new System.Drawing.Size(365, 38);
            this.zworqyToolStripMenuItem.Text = "zworqy.com/up";
            this.zworqyToolStripMenuItem.Click += new System.EventHandler(this.UploadToZworqyToolStripMenuItemClick);
            // 
            // k10xnetToolStripMenuItem
            // 
            this.k10xnetToolStripMenuItem.Name = "k10xnetToolStripMenuItem";
            this.k10xnetToolStripMenuItem.Size = new System.Drawing.Size(365, 38);
            this.k10xnetToolStripMenuItem.Text = "www.jappe2.net/upload";
            this.k10xnetToolStripMenuItem.Click += new System.EventHandler(this.K10XnetToolStripMenuItemClick);
            // 
            // SearchButton
            // 
            this.SearchButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.SearchButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SearchButton.Location = new System.Drawing.Point(334, 425);
            this.SearchButton.Margin = new System.Windows.Forms.Padding(6);
            this.SearchButton.Name = "SearchButton";
            this.SearchButton.Size = new System.Drawing.Size(194, 46);
            this.SearchButton.TabIndex = 13;
            this.SearchButton.Text = "Search";
            this.SearchButton.UseVisualStyleBackColor = true;
            this.SearchButton.Click += new System.EventHandler(this.ToggleSearch);
            // 
            // PatternLabel
            // 
            this.PatternLabel.AutoSize = true;
            this.PatternLabel.Location = new System.Drawing.Point(6, 207);
            this.PatternLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.PatternLabel.Name = "PatternLabel";
            this.PatternLabel.Size = new System.Drawing.Size(172, 25);
            this.PatternLabel.TabIndex = 14;
            this.PatternLabel.Text = "Replay filename:";
            // 
            // PatternBox
            // 
            this.PatternBox.Location = new System.Drawing.Point(190, 204);
            this.PatternBox.Margin = new System.Windows.Forms.Padding(6);
            this.PatternBox.Name = "PatternBox";
            this.PatternBox.Size = new System.Drawing.Size(240, 31);
            this.PatternBox.TabIndex = 15;
            this.toolTip1.SetToolTip(this.PatternBox, "Enter regular expression");
            // 
            // LevPatternBox
            // 
            this.LevPatternBox.Location = new System.Drawing.Point(190, 251);
            this.LevPatternBox.Margin = new System.Windows.Forms.Padding(6);
            this.LevPatternBox.Name = "LevPatternBox";
            this.LevPatternBox.Size = new System.Drawing.Size(156, 31);
            this.LevPatternBox.TabIndex = 42;
            this.toolTip1.SetToolTip(this.LevPatternBox, "Enter regular expression");
            // 
            // SearchSpecificLabel
            // 
            this.SearchSpecificLabel.AutoSize = true;
            this.SearchSpecificLabel.Location = new System.Drawing.Point(6, 253);
            this.SearchSpecificLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.SearchSpecificLabel.Name = "SearchSpecificLabel";
            this.SearchSpecificLabel.Size = new System.Drawing.Size(157, 25);
            this.SearchSpecificLabel.TabIndex = 41;
            this.SearchSpecificLabel.Text = "Level filename:";
            // 
            // Label7
            // 
            this.Label7.AutoSize = true;
            this.Label7.Location = new System.Drawing.Point(209, 297);
            this.Label7.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(30, 25);
            this.Label7.TabIndex = 22;
            this.Label7.Text = "to";
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(5, 297);
            this.Label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(65, 25);
            this.Label1.TabIndex = 20;
            this.Label1.Text = "Time:";
            // 
            // SelectedReplaysLabel
            // 
            this.SelectedReplaysLabel.AutoSize = true;
            this.SelectedReplaysLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SelectedReplaysLabel.Location = new System.Drawing.Point(18, 27);
            this.SelectedReplaysLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.SelectedReplaysLabel.Name = "SelectedReplaysLabel";
            this.SelectedReplaysLabel.Size = new System.Drawing.Size(142, 25);
            this.SelectedReplaysLabel.TabIndex = 49;
            this.SelectedReplaysLabel.Text = "0 of 0 replays";
            this.toolTip1.SetToolTip(this.SelectedReplaysLabel, "Click to toggle between 2 and 3 decimal view.");
            this.SelectedReplaysLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChangeTotalTimeDisplay);
            // 
            // SaveFileDialog1
            // 
            this.SaveFileDialog1.DefaultExt = "png";
            this.SaveFileDialog1.Filter = "Portable Network Graphics (*.png)|*.png";
            // 
            // RList
            // 
            this.RList.AllColumns.Add(this.OlvColumn1);
            this.RList.AllColumns.Add(this.OlvColumn2);
            this.RList.AllColumns.Add(this.OlvColumn3);
            this.RList.AllColumns.Add(this.OlvColumn4);
            this.RList.AllColumns.Add(this.OlvColumn5);
            this.RList.AllColumns.Add(this.OlvColumn6);
            this.RList.AllColumns.Add(this.OlvColumn7);
            this.RList.AllColumns.Add(this.olvColumn8);
            this.RList.AllColumns.Add(this.olvColumn9);
            this.RList.AllowColumnReorder = true;
            this.RList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RList.CellEditUseWholeCell = false;
            this.RList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.OlvColumn1,
            this.OlvColumn2,
            this.OlvColumn3,
            this.OlvColumn4,
            this.OlvColumn5,
            this.OlvColumn6,
            this.OlvColumn7,
            this.olvColumn8,
            this.olvColumn9});
            this.RList.ContextMenuStrip = this.ContextMenuStrip1;
            this.RList.Cursor = System.Windows.Forms.Cursors.Default;
            this.RList.EmptyListMsg = "";
            this.RList.FullRowSelect = true;
            this.RList.HideSelection = false;
            this.RList.Location = new System.Drawing.Point(24, 73);
            this.RList.Margin = new System.Windows.Forms.Padding(6);
            this.RList.Name = "RList";
            this.RList.OwnerDrawnHeader = true;
            this.RList.SelectAllOnControlA = false;
            this.RList.ShowGroups = false;
            this.RList.Size = new System.Drawing.Size(1116, 213);
            this.RList.TabIndex = 50;
            this.RList.UpdateSpaceFillingColumnsWhenDraggingColumnDivider = false;
            this.RList.UseCompatibleStateImageBehavior = false;
            this.RList.UseOverlays = false;
            this.RList.View = System.Windows.Forms.View.Details;
            this.RList.CellEditFinishing += new BrightIdeasSoftware.CellEditEventHandler(this.CellEditFinishing);
            this.RList.CellEditStarting += new BrightIdeasSoftware.CellEditEventHandler(this.CellEditStarting);
            this.RList.SelectionChanged += new System.EventHandler(this.ReplaylistSelectionChanged);
            this.RList.DoubleClick += new System.EventHandler(this.OpenViewer);
            // 
            // OlvColumn1
            // 
            this.OlvColumn1.AspectName = "";
            this.OlvColumn1.Text = "";
            this.OlvColumn1.Width = 120;
            // 
            // OlvColumn2
            // 
            this.OlvColumn2.AspectName = "";
            this.OlvColumn2.IsEditable = false;
            this.OlvColumn2.Text = "";
            this.OlvColumn2.Width = 56;
            // 
            // OlvColumn3
            // 
            this.OlvColumn3.AspectName = "";
            this.OlvColumn3.IsEditable = false;
            this.OlvColumn3.Text = "";
            // 
            // OlvColumn4
            // 
            this.OlvColumn4.AspectName = "";
            this.OlvColumn4.IsEditable = false;
            this.OlvColumn4.Text = "";
            // 
            // OlvColumn5
            // 
            this.OlvColumn5.AspectName = "";
            this.OlvColumn5.IsEditable = false;
            this.OlvColumn5.Text = "";
            this.OlvColumn5.Width = 50;
            // 
            // OlvColumn6
            // 
            this.OlvColumn6.AspectName = "";
            this.OlvColumn6.IsEditable = false;
            this.OlvColumn6.Text = "";
            this.OlvColumn6.Width = 75;
            // 
            // OlvColumn7
            // 
            this.OlvColumn7.AspectName = "";
            this.OlvColumn7.IsEditable = false;
            this.OlvColumn7.Text = "";
            this.OlvColumn7.Width = 85;
            // 
            // olvColumn8
            // 
            this.olvColumn8.IsEditable = false;
            this.olvColumn8.Text = "";
            // 
            // olvColumn9
            // 
            this.olvColumn9.IsEditable = false;
            this.olvColumn9.Text = "";
            // 
            // ConfigButton
            // 
            this.ConfigButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ConfigButton.Location = new System.Drawing.Point(934, 15);
            this.ConfigButton.Margin = new System.Windows.Forms.Padding(6);
            this.ConfigButton.Name = "ConfigButton";
            this.ConfigButton.Size = new System.Drawing.Size(206, 46);
            this.ConfigButton.TabIndex = 52;
            this.ConfigButton.Text = "Configuration";
            this.ConfigButton.UseVisualStyleBackColor = true;
            this.ConfigButton.Click += new System.EventHandler(this.DisplayConfiguration);
            // 
            // TabControl1
            // 
            this.TabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TabControl1.Controls.Add(this.TabPage1);
            this.TabControl1.Controls.Add(this.TabPage2);
            this.TabControl1.Controls.Add(this.TabPage3);
            this.TabControl1.Location = new System.Drawing.Point(24, 298);
            this.TabControl1.Margin = new System.Windows.Forms.Padding(6);
            this.TabControl1.Name = "TabControl1";
            this.TabControl1.SelectedIndex = 0;
            this.TabControl1.Size = new System.Drawing.Size(1116, 550);
            this.TabControl1.TabIndex = 54;
            // 
            // TabPage1
            // 
            this.TabPage1.AutoScroll = true;
            this.TabPage1.Controls.Add(this.tableLayoutPanel1);
            this.TabPage1.Location = new System.Drawing.Point(8, 39);
            this.TabPage1.Margin = new System.Windows.Forms.Padding(6);
            this.TabPage1.Name = "TabPage1";
            this.TabPage1.Padding = new System.Windows.Forms.Padding(6);
            this.TabPage1.Size = new System.Drawing.Size(1100, 503);
            this.TabPage1.TabIndex = 0;
            this.TabPage1.Text = "Search options";
            this.TabPage1.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(9, 14);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 483F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 483F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1082, 483);
            this.tableLayoutPanel1.TabIndex = 65;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Controls.Add(this.singleMultiSelect, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.finishedSelect, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.elmaAcrossSelect, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.intExtSelect, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.fastestSlowestSelect, 0, 3);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 5;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(535, 338);
            this.tableLayoutPanel2.TabIndex = 65;
            // 
            // singleMultiSelect
            // 
            this.singleMultiSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.singleMultiSelect.Location = new System.Drawing.Point(0, 268);
            this.singleMultiSelect.Margin = new System.Windows.Forms.Padding(0);
            this.singleMultiSelect.Name = "singleMultiSelect";
            this.singleMultiSelect.Option1Text = "Multiplayer";
            this.singleMultiSelect.Option2Text = "Singleplayer";
            this.singleMultiSelect.Option3Text = "Both";
            this.singleMultiSelect.SelectedOption = 2;
            this.singleMultiSelect.Size = new System.Drawing.Size(535, 70);
            this.singleMultiSelect.TabIndex = 62;
            // 
            // finishedSelect
            // 
            this.finishedSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.finishedSelect.Location = new System.Drawing.Point(0, 134);
            this.finishedSelect.Margin = new System.Windows.Forms.Padding(0);
            this.finishedSelect.Name = "finishedSelect";
            this.finishedSelect.Option1Text = "Finished";
            this.finishedSelect.Option2Text = "Not finished";
            this.finishedSelect.Option3Text = "Both";
            this.finishedSelect.SelectedOption = 2;
            this.finishedSelect.Size = new System.Drawing.Size(535, 67);
            this.finishedSelect.TabIndex = 61;
            // 
            // elmaAcrossSelect
            // 
            this.elmaAcrossSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.elmaAcrossSelect.Location = new System.Drawing.Point(0, 67);
            this.elmaAcrossSelect.Margin = new System.Windows.Forms.Padding(0);
            this.elmaAcrossSelect.Name = "elmaAcrossSelect";
            this.elmaAcrossSelect.Option1Text = "Across levels";
            this.elmaAcrossSelect.Option2Text = "Elma levels";
            this.elmaAcrossSelect.Option3Text = "Both";
            this.elmaAcrossSelect.SelectedOption = 2;
            this.elmaAcrossSelect.Size = new System.Drawing.Size(535, 67);
            this.elmaAcrossSelect.TabIndex = 63;
            // 
            // intExtSelect
            // 
            this.intExtSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.intExtSelect.Location = new System.Drawing.Point(0, 0);
            this.intExtSelect.Margin = new System.Windows.Forms.Padding(0);
            this.intExtSelect.Name = "intExtSelect";
            this.intExtSelect.Option1Text = "Internal replays";
            this.intExtSelect.Option2Text = "External replays";
            this.intExtSelect.Option3Text = "Both";
            this.intExtSelect.SelectedOption = 2;
            this.intExtSelect.Size = new System.Drawing.Size(535, 67);
            this.intExtSelect.TabIndex = 60;
            // 
            // fastestSlowestSelect
            // 
            this.fastestSlowestSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fastestSlowestSelect.Location = new System.Drawing.Point(0, 201);
            this.fastestSlowestSelect.Margin = new System.Windows.Forms.Padding(0);
            this.fastestSlowestSelect.Name = "fastestSlowestSelect";
            this.fastestSlowestSelect.Option1Text = "Fastest replays";
            this.fastestSlowestSelect.Option2Text = "Slowest replays";
            this.fastestSlowestSelect.Option3Text = "All";
            this.fastestSlowestSelect.SelectedOption = 2;
            this.fastestSlowestSelect.Size = new System.Drawing.Size(535, 67);
            this.fastestSlowestSelect.TabIndex = 64;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.minFileSizeBox);
            this.panel1.Controls.Add(this.Label1);
            this.panel1.Controls.Add(this.SearchButton);
            this.panel1.Controls.Add(this.TimeMaxBox);
            this.panel1.Controls.Add(this.LevPatternBox);
            this.panel1.Controls.Add(this.TimeMinBox);
            this.panel1.Controls.Add(this.Label7);
            this.panel1.Controls.Add(this.PatternLabel);
            this.panel1.Controls.Add(this.SearchSpecificLabel);
            this.panel1.Controls.Add(this.PatternBox);
            this.panel1.Controls.Add(this.maxDateTime);
            this.panel1.Controls.Add(this.maxFileSizeBox);
            this.panel1.Controls.Add(this.label26);
            this.panel1.Controls.Add(this.label23);
            this.panel1.Controls.Add(this.minDateTime);
            this.panel1.Controls.Add(this.label25);
            this.panel1.Controls.Add(this.label24);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(544, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(535, 477);
            this.panel1.TabIndex = 1;
            // 
            // minFileSizeBox
            // 
            this.minFileSizeBox.BackColor = System.Drawing.SystemColors.Window;
            this.minFileSizeBox.DefaultValue = 0D;
            this.minFileSizeBox.Location = new System.Drawing.Point(149, 27);
            this.minFileSizeBox.Margin = new System.Windows.Forms.Padding(6);
            this.minFileSizeBox.Name = "minFileSizeBox";
            this.minFileSizeBox.Size = new System.Drawing.Size(102, 31);
            this.minFileSizeBox.TabIndex = 53;
            this.minFileSizeBox.Text = "0";
            // 
            // TimeMaxBox
            // 
            this.TimeMaxBox.BackColor = System.Drawing.SystemColors.Window;
            this.TimeMaxBox.DefaultValue = 0D;
            this.TimeMaxBox.Location = new System.Drawing.Point(251, 294);
            this.TimeMaxBox.Margin = new System.Windows.Forms.Padding(6);
            this.TimeMaxBox.MaxLength = 9;
            this.TimeMaxBox.Name = "TimeMaxBox";
            this.TimeMaxBox.Size = new System.Drawing.Size(108, 31);
            this.TimeMaxBox.TabIndex = 23;
            this.TimeMaxBox.Text = "99:00,000";
            // 
            // TimeMinBox
            // 
            this.TimeMinBox.BackColor = System.Drawing.SystemColors.Window;
            this.TimeMinBox.DefaultValue = 0D;
            this.TimeMinBox.Location = new System.Drawing.Point(89, 294);
            this.TimeMinBox.Margin = new System.Windows.Forms.Padding(6);
            this.TimeMinBox.MaxLength = 9;
            this.TimeMinBox.Name = "TimeMinBox";
            this.TimeMinBox.Size = new System.Drawing.Size(108, 31);
            this.TimeMinBox.TabIndex = 21;
            this.TimeMinBox.Text = "00:00,000";
            // 
            // maxDateTime
            // 
            this.maxDateTime.CustomFormat = "dd.MM.yyyy HH:mm:ss";
            this.maxDateTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.maxDateTime.Location = new System.Drawing.Point(11, 159);
            this.maxDateTime.Margin = new System.Windows.Forms.Padding(6);
            this.maxDateTime.Name = "maxDateTime";
            this.maxDateTime.Size = new System.Drawing.Size(302, 31);
            this.maxDateTime.TabIndex = 59;
            this.maxDateTime.Value = new System.DateTime(9000, 1, 1, 0, 0, 0, 0);
            // 
            // maxFileSizeBox
            // 
            this.maxFileSizeBox.BackColor = System.Drawing.SystemColors.Window;
            this.maxFileSizeBox.DefaultValue = 0D;
            this.maxFileSizeBox.Location = new System.Drawing.Point(297, 27);
            this.maxFileSizeBox.Margin = new System.Windows.Forms.Padding(6);
            this.maxFileSizeBox.Name = "maxFileSizeBox";
            this.maxFileSizeBox.Size = new System.Drawing.Size(102, 31);
            this.maxFileSizeBox.TabIndex = 55;
            this.maxFileSizeBox.Text = "10000";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(329, 118);
            this.label26.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(30, 25);
            this.label26.TabIndex = 58;
            this.label26.Text = "to";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(6, 30);
            this.label23.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(143, 25);
            this.label23.TabIndex = 52;
            this.label23.Text = "File size (kB):";
            // 
            // minDateTime
            // 
            this.minDateTime.CustomFormat = "dd.MM.yyyy HH:mm:ss";
            this.minDateTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.minDateTime.Location = new System.Drawing.Point(11, 109);
            this.minDateTime.Margin = new System.Windows.Forms.Padding(6);
            this.minDateTime.Name = "minDateTime";
            this.minDateTime.Size = new System.Drawing.Size(302, 31);
            this.minDateTime.TabIndex = 57;
            this.minDateTime.Value = new System.DateTime(1800, 1, 1, 0, 0, 0, 0);
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(5, 78);
            this.label25.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(150, 25);
            this.label25.TabIndex = 56;
            this.label25.Text = "Date modified:";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(263, 30);
            this.label24.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(30, 25);
            this.label24.TabIndex = 54;
            this.label24.Text = "to";
            // 
            // TabPage2
            // 
            this.TabPage2.Controls.Add(this.ResetButton);
            this.TabPage2.Controls.Add(this.Label15);
            this.TabPage2.Controls.Add(this.Label16);
            this.TabPage2.Controls.Add(this.Label17);
            this.TabPage2.Controls.Add(this.Label18);
            this.TabPage2.Controls.Add(this.Label19);
            this.TabPage2.Controls.Add(this.Label20);
            this.TabPage2.Controls.Add(this.Label14);
            this.TabPage2.Controls.Add(this.Label13);
            this.TabPage2.Controls.Add(this.Label12);
            this.TabPage2.Controls.Add(this.Label11);
            this.TabPage2.Controls.Add(this.Label10);
            this.TabPage2.Controls.Add(this.Label9);
            this.TabPage2.Controls.Add(this.Label8);
            this.TabPage2.Controls.Add(this.Label2);
            this.TabPage2.Controls.Add(this.Label6);
            this.TabPage2.Controls.Add(this.Label5);
            this.TabPage2.Controls.Add(this.Label4);
            this.TabPage2.Controls.Add(this.Label3);
            this.TabPage2.Controls.Add(this.Label21);
            this.TabPage2.Controls.Add(this.Label22);
            this.TabPage2.Controls.Add(this.TextBox13);
            this.TabPage2.Controls.Add(this.TextBox12);
            this.TabPage2.Controls.Add(this.TextBox15);
            this.TabPage2.Controls.Add(this.TextBox16);
            this.TabPage2.Controls.Add(this.TextBox5);
            this.TabPage2.Controls.Add(this.TextBox18);
            this.TabPage2.Controls.Add(this.TextBox7);
            this.TabPage2.Controls.Add(this.TextBox20);
            this.TabPage2.Controls.Add(this.TextBox9);
            this.TabPage2.Controls.Add(this.TextBox22);
            this.TabPage2.Controls.Add(this.TextBox10);
            this.TabPage2.Controls.Add(this.TextBox3);
            this.TabPage2.Controls.Add(this.TextBox24);
            this.TabPage2.Controls.Add(this.TextBox11);
            this.TabPage2.Controls.Add(this.TextBox23);
            this.TabPage2.Controls.Add(this.TextBox14);
            this.TabPage2.Controls.Add(this.TextBox8);
            this.TabPage2.Controls.Add(this.TextBox17);
            this.TabPage2.Controls.Add(this.TextBox6);
            this.TabPage2.Controls.Add(this.TextBox19);
            this.TabPage2.Controls.Add(this.TextBox4);
            this.TabPage2.Controls.Add(this.TextBox21);
            this.TabPage2.Controls.Add(this.TextBox2);
            this.TabPage2.Controls.Add(this.TextBox1);
            this.TabPage2.Location = new System.Drawing.Point(8, 39);
            this.TabPage2.Margin = new System.Windows.Forms.Padding(6);
            this.TabPage2.Name = "TabPage2";
            this.TabPage2.Padding = new System.Windows.Forms.Padding(6);
            this.TabPage2.Size = new System.Drawing.Size(1100, 503);
            this.TabPage2.TabIndex = 1;
            this.TabPage2.Text = "Advanced";
            this.TabPage2.UseVisualStyleBackColor = true;
            // 
            // ResetButton
            // 
            this.ResetButton.Location = new System.Drawing.Point(22, 363);
            this.ResetButton.Margin = new System.Windows.Forms.Padding(6);
            this.ResetButton.Name = "ResetButton";
            this.ResetButton.Size = new System.Drawing.Size(156, 48);
            this.ResetButton.TabIndex = 81;
            this.ResetButton.Text = "Reset fields";
            this.ResetButton.UseVisualStyleBackColor = true;
            this.ResetButton.Click += new System.EventHandler(this.ResetFields);
            // 
            // Label15
            // 
            this.Label15.AutoSize = true;
            this.Label15.Location = new System.Drawing.Point(650, 65);
            this.Label15.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.Label15.Name = "Label15";
            this.Label15.Size = new System.Drawing.Size(30, 25);
            this.Label15.TabIndex = 88;
            this.Label15.Text = "to";
            // 
            // Label16
            // 
            this.Label16.AutoSize = true;
            this.Label16.Location = new System.Drawing.Point(650, 162);
            this.Label16.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.Label16.Name = "Label16";
            this.Label16.Size = new System.Drawing.Size(30, 25);
            this.Label16.TabIndex = 87;
            this.Label16.Text = "to";
            // 
            // Label17
            // 
            this.Label17.AutoSize = true;
            this.Label17.Location = new System.Drawing.Point(650, 210);
            this.Label17.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.Label17.Name = "Label17";
            this.Label17.Size = new System.Drawing.Size(30, 25);
            this.Label17.TabIndex = 86;
            this.Label17.Text = "to";
            // 
            // Label18
            // 
            this.Label18.AutoSize = true;
            this.Label18.Location = new System.Drawing.Point(650, 258);
            this.Label18.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.Label18.Name = "Label18";
            this.Label18.Size = new System.Drawing.Size(30, 25);
            this.Label18.TabIndex = 85;
            this.Label18.Text = "to";
            // 
            // Label19
            // 
            this.Label19.AutoSize = true;
            this.Label19.Location = new System.Drawing.Point(650, 306);
            this.Label19.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.Label19.Name = "Label19";
            this.Label19.Size = new System.Drawing.Size(30, 25);
            this.Label19.TabIndex = 84;
            this.Label19.Text = "to";
            // 
            // Label20
            // 
            this.Label20.AutoSize = true;
            this.Label20.Location = new System.Drawing.Point(650, 113);
            this.Label20.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.Label20.Name = "Label20";
            this.Label20.Size = new System.Drawing.Size(30, 25);
            this.Label20.TabIndex = 83;
            this.Label20.Text = "to";
            // 
            // Label14
            // 
            this.Label14.AutoSize = true;
            this.Label14.Location = new System.Drawing.Point(298, 65);
            this.Label14.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.Label14.Name = "Label14";
            this.Label14.Size = new System.Drawing.Size(30, 25);
            this.Label14.TabIndex = 82;
            this.Label14.Text = "to";
            // 
            // Label13
            // 
            this.Label13.AutoSize = true;
            this.Label13.Location = new System.Drawing.Point(298, 162);
            this.Label13.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.Label13.Name = "Label13";
            this.Label13.Size = new System.Drawing.Size(30, 25);
            this.Label13.TabIndex = 80;
            this.Label13.Text = "to";
            // 
            // Label12
            // 
            this.Label12.AutoSize = true;
            this.Label12.Location = new System.Drawing.Point(298, 210);
            this.Label12.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.Label12.Name = "Label12";
            this.Label12.Size = new System.Drawing.Size(30, 25);
            this.Label12.TabIndex = 78;
            this.Label12.Text = "to";
            // 
            // Label11
            // 
            this.Label11.AutoSize = true;
            this.Label11.Location = new System.Drawing.Point(298, 258);
            this.Label11.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.Label11.Name = "Label11";
            this.Label11.Size = new System.Drawing.Size(30, 25);
            this.Label11.TabIndex = 76;
            this.Label11.Text = "to";
            // 
            // Label10
            // 
            this.Label10.AutoSize = true;
            this.Label10.Location = new System.Drawing.Point(298, 306);
            this.Label10.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.Label10.Name = "Label10";
            this.Label10.Size = new System.Drawing.Size(30, 25);
            this.Label10.TabIndex = 73;
            this.Label10.Text = "to";
            // 
            // Label9
            // 
            this.Label9.AutoSize = true;
            this.Label9.Location = new System.Drawing.Point(298, 113);
            this.Label9.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(30, 25);
            this.Label9.TabIndex = 72;
            this.Label9.Text = "to";
            // 
            // Label8
            // 
            this.Label8.AutoSize = true;
            this.Label8.Location = new System.Drawing.Point(16, 306);
            this.Label8.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(165, 25);
            this.Label8.TabIndex = 57;
            this.Label8.Text = "Groundtouches:";
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(108, 258);
            this.Label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(73, 25);
            this.Label2.TabIndex = 56;
            this.Label2.Text = "Turns:";
            // 
            // Label6
            // 
            this.Label6.AutoSize = true;
            this.Label6.Location = new System.Drawing.Point(62, 210);
            this.Label6.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(120, 25);
            this.Label6.TabIndex = 53;
            this.Label6.Text = "Supervolts:";
            // 
            // Label5
            // 
            this.Label5.AutoSize = true;
            this.Label5.Location = new System.Drawing.Point(62, 162);
            this.Label5.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(119, 25);
            this.Label5.TabIndex = 52;
            this.Label5.Text = "Right volts:";
            // 
            // Label4
            // 
            this.Label4.AutoSize = true;
            this.Label4.Location = new System.Drawing.Point(76, 113);
            this.Label4.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(105, 25);
            this.Label4.TabIndex = 49;
            this.Label4.Text = "Left volts:";
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(56, 65);
            this.Label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(125, 25);
            this.Label3.TabIndex = 48;
            this.Label3.Text = "Appletakes:";
            // 
            // Label21
            // 
            this.Label21.AutoSize = true;
            this.Label21.Location = new System.Drawing.Point(622, 6);
            this.Label21.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.Label21.Name = "Label21";
            this.Label21.Size = new System.Drawing.Size(91, 25);
            this.Label21.TabIndex = 46;
            this.Label21.Text = "Player 2";
            // 
            // Label22
            // 
            this.Label22.AutoSize = true;
            this.Label22.Location = new System.Drawing.Point(268, 6);
            this.Label22.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.Label22.Name = "Label22";
            this.Label22.Size = new System.Drawing.Size(91, 25);
            this.Label22.TabIndex = 44;
            this.Label22.Text = "Player 1";
            // 
            // TextBox13
            // 
            this.TextBox13.BackColor = System.Drawing.SystemColors.Window;
            this.TextBox13.DefaultValue = 0D;
            this.TextBox13.Location = new System.Drawing.Point(692, 300);
            this.TextBox13.Margin = new System.Windows.Forms.Padding(6);
            this.TextBox13.Name = "TextBox13";
            this.TextBox13.Size = new System.Drawing.Size(90, 31);
            this.TextBox13.TabIndex = 79;
            this.TextBox13.Text = "10000";
            // 
            // TextBox12
            // 
            this.TextBox12.BackColor = System.Drawing.SystemColors.Window;
            this.TextBox12.DefaultValue = 0D;
            this.TextBox12.Location = new System.Drawing.Point(546, 300);
            this.TextBox12.Margin = new System.Windows.Forms.Padding(6);
            this.TextBox12.Name = "TextBox12";
            this.TextBox12.Size = new System.Drawing.Size(90, 31);
            this.TextBox12.TabIndex = 77;
            this.TextBox12.Text = "0";
            // 
            // TextBox15
            // 
            this.TextBox15.BackColor = System.Drawing.SystemColors.Window;
            this.TextBox15.DefaultValue = 0D;
            this.TextBox15.Location = new System.Drawing.Point(692, 252);
            this.TextBox15.Margin = new System.Windows.Forms.Padding(6);
            this.TextBox15.Name = "TextBox15";
            this.TextBox15.Size = new System.Drawing.Size(90, 31);
            this.TextBox15.TabIndex = 75;
            this.TextBox15.Text = "10000";
            // 
            // TextBox16
            // 
            this.TextBox16.BackColor = System.Drawing.SystemColors.Window;
            this.TextBox16.DefaultValue = 0D;
            this.TextBox16.Location = new System.Drawing.Point(692, 60);
            this.TextBox16.Margin = new System.Windows.Forms.Padding(6);
            this.TextBox16.Name = "TextBox16";
            this.TextBox16.Size = new System.Drawing.Size(90, 31);
            this.TextBox16.TabIndex = 65;
            this.TextBox16.Text = "10000";
            // 
            // TextBox5
            // 
            this.TextBox5.BackColor = System.Drawing.SystemColors.Window;
            this.TextBox5.DefaultValue = 0D;
            this.TextBox5.Location = new System.Drawing.Point(546, 108);
            this.TextBox5.Margin = new System.Windows.Forms.Padding(6);
            this.TextBox5.Name = "TextBox5";
            this.TextBox5.Size = new System.Drawing.Size(90, 31);
            this.TextBox5.TabIndex = 66;
            this.TextBox5.Text = "0";
            // 
            // TextBox18
            // 
            this.TextBox18.BackColor = System.Drawing.SystemColors.Window;
            this.TextBox18.DefaultValue = 0D;
            this.TextBox18.Location = new System.Drawing.Point(692, 108);
            this.TextBox18.Margin = new System.Windows.Forms.Padding(6);
            this.TextBox18.Name = "TextBox18";
            this.TextBox18.Size = new System.Drawing.Size(90, 31);
            this.TextBox18.TabIndex = 67;
            this.TextBox18.Text = "10000";
            // 
            // TextBox7
            // 
            this.TextBox7.BackColor = System.Drawing.SystemColors.Window;
            this.TextBox7.DefaultValue = 0D;
            this.TextBox7.Location = new System.Drawing.Point(546, 156);
            this.TextBox7.Margin = new System.Windows.Forms.Padding(6);
            this.TextBox7.Name = "TextBox7";
            this.TextBox7.Size = new System.Drawing.Size(90, 31);
            this.TextBox7.TabIndex = 68;
            this.TextBox7.Text = "0";
            // 
            // TextBox20
            // 
            this.TextBox20.BackColor = System.Drawing.SystemColors.Window;
            this.TextBox20.DefaultValue = 0D;
            this.TextBox20.Location = new System.Drawing.Point(692, 156);
            this.TextBox20.Margin = new System.Windows.Forms.Padding(6);
            this.TextBox20.Name = "TextBox20";
            this.TextBox20.Size = new System.Drawing.Size(90, 31);
            this.TextBox20.TabIndex = 69;
            this.TextBox20.Text = "10000";
            // 
            // TextBox9
            // 
            this.TextBox9.BackColor = System.Drawing.SystemColors.Window;
            this.TextBox9.DefaultValue = 0D;
            this.TextBox9.Location = new System.Drawing.Point(546, 204);
            this.TextBox9.Margin = new System.Windows.Forms.Padding(6);
            this.TextBox9.Name = "TextBox9";
            this.TextBox9.Size = new System.Drawing.Size(90, 31);
            this.TextBox9.TabIndex = 70;
            this.TextBox9.Text = "0";
            // 
            // TextBox22
            // 
            this.TextBox22.BackColor = System.Drawing.SystemColors.Window;
            this.TextBox22.DefaultValue = 0D;
            this.TextBox22.Location = new System.Drawing.Point(692, 204);
            this.TextBox22.Margin = new System.Windows.Forms.Padding(6);
            this.TextBox22.Name = "TextBox22";
            this.TextBox22.Size = new System.Drawing.Size(90, 31);
            this.TextBox22.TabIndex = 71;
            this.TextBox22.Text = "10000";
            // 
            // TextBox10
            // 
            this.TextBox10.BackColor = System.Drawing.SystemColors.Window;
            this.TextBox10.DefaultValue = 0D;
            this.TextBox10.Location = new System.Drawing.Point(546, 252);
            this.TextBox10.Margin = new System.Windows.Forms.Padding(6);
            this.TextBox10.Name = "TextBox10";
            this.TextBox10.Size = new System.Drawing.Size(90, 31);
            this.TextBox10.TabIndex = 74;
            this.TextBox10.Text = "0";
            // 
            // TextBox3
            // 
            this.TextBox3.BackColor = System.Drawing.SystemColors.Window;
            this.TextBox3.DefaultValue = 0D;
            this.TextBox3.Location = new System.Drawing.Point(546, 60);
            this.TextBox3.Margin = new System.Windows.Forms.Padding(6);
            this.TextBox3.Name = "TextBox3";
            this.TextBox3.Size = new System.Drawing.Size(90, 31);
            this.TextBox3.TabIndex = 64;
            this.TextBox3.Text = "0";
            // 
            // TextBox24
            // 
            this.TextBox24.BackColor = System.Drawing.SystemColors.Window;
            this.TextBox24.DefaultValue = 0D;
            this.TextBox24.Location = new System.Drawing.Point(340, 300);
            this.TextBox24.Margin = new System.Windows.Forms.Padding(6);
            this.TextBox24.Name = "TextBox24";
            this.TextBox24.Size = new System.Drawing.Size(90, 31);
            this.TextBox24.TabIndex = 63;
            this.TextBox24.Text = "10000";
            // 
            // TextBox11
            // 
            this.TextBox11.BackColor = System.Drawing.SystemColors.Window;
            this.TextBox11.DefaultValue = 0D;
            this.TextBox11.Location = new System.Drawing.Point(194, 300);
            this.TextBox11.Margin = new System.Windows.Forms.Padding(6);
            this.TextBox11.Name = "TextBox11";
            this.TextBox11.Size = new System.Drawing.Size(90, 31);
            this.TextBox11.TabIndex = 62;
            this.TextBox11.Text = "0";
            // 
            // TextBox23
            // 
            this.TextBox23.BackColor = System.Drawing.SystemColors.Window;
            this.TextBox23.DefaultValue = 0D;
            this.TextBox23.Location = new System.Drawing.Point(340, 252);
            this.TextBox23.Margin = new System.Windows.Forms.Padding(6);
            this.TextBox23.Name = "TextBox23";
            this.TextBox23.Size = new System.Drawing.Size(90, 31);
            this.TextBox23.TabIndex = 61;
            this.TextBox23.Text = "10000";
            // 
            // TextBox14
            // 
            this.TextBox14.BackColor = System.Drawing.SystemColors.Window;
            this.TextBox14.DefaultValue = 0D;
            this.TextBox14.Location = new System.Drawing.Point(340, 60);
            this.TextBox14.Margin = new System.Windows.Forms.Padding(6);
            this.TextBox14.Name = "TextBox14";
            this.TextBox14.Size = new System.Drawing.Size(90, 31);
            this.TextBox14.TabIndex = 47;
            this.TextBox14.Text = "10000";
            // 
            // TextBox8
            // 
            this.TextBox8.BackColor = System.Drawing.SystemColors.Window;
            this.TextBox8.DefaultValue = 0D;
            this.TextBox8.Location = new System.Drawing.Point(194, 108);
            this.TextBox8.Margin = new System.Windows.Forms.Padding(6);
            this.TextBox8.Name = "TextBox8";
            this.TextBox8.Size = new System.Drawing.Size(90, 31);
            this.TextBox8.TabIndex = 50;
            this.TextBox8.Text = "0";
            // 
            // TextBox17
            // 
            this.TextBox17.BackColor = System.Drawing.SystemColors.Window;
            this.TextBox17.DefaultValue = 0D;
            this.TextBox17.Location = new System.Drawing.Point(340, 108);
            this.TextBox17.Margin = new System.Windows.Forms.Padding(6);
            this.TextBox17.Name = "TextBox17";
            this.TextBox17.Size = new System.Drawing.Size(90, 31);
            this.TextBox17.TabIndex = 51;
            this.TextBox17.Text = "10000";
            // 
            // TextBox6
            // 
            this.TextBox6.BackColor = System.Drawing.SystemColors.Window;
            this.TextBox6.DefaultValue = 0D;
            this.TextBox6.Location = new System.Drawing.Point(194, 156);
            this.TextBox6.Margin = new System.Windows.Forms.Padding(6);
            this.TextBox6.Name = "TextBox6";
            this.TextBox6.Size = new System.Drawing.Size(90, 31);
            this.TextBox6.TabIndex = 54;
            this.TextBox6.Text = "0";
            // 
            // TextBox19
            // 
            this.TextBox19.BackColor = System.Drawing.SystemColors.Window;
            this.TextBox19.DefaultValue = 0D;
            this.TextBox19.Location = new System.Drawing.Point(340, 156);
            this.TextBox19.Margin = new System.Windows.Forms.Padding(6);
            this.TextBox19.Name = "TextBox19";
            this.TextBox19.Size = new System.Drawing.Size(90, 31);
            this.TextBox19.TabIndex = 55;
            this.TextBox19.Text = "10000";
            // 
            // TextBox4
            // 
            this.TextBox4.BackColor = System.Drawing.SystemColors.Window;
            this.TextBox4.DefaultValue = 0D;
            this.TextBox4.Location = new System.Drawing.Point(194, 204);
            this.TextBox4.Margin = new System.Windows.Forms.Padding(6);
            this.TextBox4.Name = "TextBox4";
            this.TextBox4.Size = new System.Drawing.Size(90, 31);
            this.TextBox4.TabIndex = 58;
            this.TextBox4.Text = "0";
            // 
            // TextBox21
            // 
            this.TextBox21.BackColor = System.Drawing.SystemColors.Window;
            this.TextBox21.DefaultValue = 0D;
            this.TextBox21.Location = new System.Drawing.Point(340, 204);
            this.TextBox21.Margin = new System.Windows.Forms.Padding(6);
            this.TextBox21.Name = "TextBox21";
            this.TextBox21.Size = new System.Drawing.Size(90, 31);
            this.TextBox21.TabIndex = 59;
            this.TextBox21.Text = "10000";
            // 
            // TextBox2
            // 
            this.TextBox2.BackColor = System.Drawing.SystemColors.Window;
            this.TextBox2.DefaultValue = 0D;
            this.TextBox2.Location = new System.Drawing.Point(194, 252);
            this.TextBox2.Margin = new System.Windows.Forms.Padding(6);
            this.TextBox2.Name = "TextBox2";
            this.TextBox2.Size = new System.Drawing.Size(90, 31);
            this.TextBox2.TabIndex = 60;
            this.TextBox2.Text = "0";
            // 
            // TextBox1
            // 
            this.TextBox1.BackColor = System.Drawing.SystemColors.Window;
            this.TextBox1.DefaultValue = 0D;
            this.TextBox1.Location = new System.Drawing.Point(194, 60);
            this.TextBox1.Margin = new System.Windows.Forms.Padding(6);
            this.TextBox1.Name = "TextBox1";
            this.TextBox1.Size = new System.Drawing.Size(90, 31);
            this.TextBox1.TabIndex = 45;
            this.TextBox1.Text = "0";
            // 
            // TabPage3
            // 
            this.TabPage3.Controls.Add(this.ReplaysIncorrectLevButton);
            this.TabPage3.Controls.Add(this.ReplaysWithoutLevFileButton);
            this.TabPage3.Controls.Add(this.DuplicateFilenameButton);
            this.TabPage3.Controls.Add(this.DuplicateButton);
            this.TabPage3.Location = new System.Drawing.Point(8, 39);
            this.TabPage3.Margin = new System.Windows.Forms.Padding(6);
            this.TabPage3.Name = "TabPage3";
            this.TabPage3.Size = new System.Drawing.Size(1100, 503);
            this.TabPage3.TabIndex = 2;
            this.TabPage3.Text = "Other searches";
            this.TabPage3.UseVisualStyleBackColor = true;
            // 
            // ReplaysIncorrectLevButton
            // 
            this.ReplaysIncorrectLevButton.Location = new System.Drawing.Point(24, 196);
            this.ReplaysIncorrectLevButton.Margin = new System.Windows.Forms.Padding(6);
            this.ReplaysIncorrectLevButton.Name = "ReplaysIncorrectLevButton";
            this.ReplaysIncorrectLevButton.Size = new System.Drawing.Size(328, 46);
            this.ReplaysIncorrectLevButton.TabIndex = 56;
            this.ReplaysIncorrectLevButton.Text = "Replays with incorrect level";
            this.ReplaysIncorrectLevButton.UseVisualStyleBackColor = true;
            this.ReplaysIncorrectLevButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ToggleSearch);
            // 
            // ReplaysWithoutLevFileButton
            // 
            this.ReplaysWithoutLevFileButton.Location = new System.Drawing.Point(24, 138);
            this.ReplaysWithoutLevFileButton.Margin = new System.Windows.Forms.Padding(6);
            this.ReplaysWithoutLevFileButton.Name = "ReplaysWithoutLevFileButton";
            this.ReplaysWithoutLevFileButton.Size = new System.Drawing.Size(328, 46);
            this.ReplaysWithoutLevFileButton.TabIndex = 55;
            this.ReplaysWithoutLevFileButton.Text = "Replays without level file";
            this.ReplaysWithoutLevFileButton.UseVisualStyleBackColor = true;
            this.ReplaysWithoutLevFileButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ToggleSearch);
            // 
            // DuplicateFilenameButton
            // 
            this.DuplicateFilenameButton.Location = new System.Drawing.Point(24, 81);
            this.DuplicateFilenameButton.Margin = new System.Windows.Forms.Padding(6);
            this.DuplicateFilenameButton.Name = "DuplicateFilenameButton";
            this.DuplicateFilenameButton.Size = new System.Drawing.Size(328, 46);
            this.DuplicateFilenameButton.TabIndex = 54;
            this.DuplicateFilenameButton.Text = "Duplicate filename search";
            this.DuplicateFilenameButton.UseVisualStyleBackColor = true;
            this.DuplicateFilenameButton.Click += new System.EventHandler(this.DuplicateFilenameSearch);
            // 
            // DuplicateButton
            // 
            this.DuplicateButton.Location = new System.Drawing.Point(24, 23);
            this.DuplicateButton.Margin = new System.Windows.Forms.Padding(6);
            this.DuplicateButton.Name = "DuplicateButton";
            this.DuplicateButton.Size = new System.Drawing.Size(328, 46);
            this.DuplicateButton.TabIndex = 53;
            this.DuplicateButton.Text = "Duplicate replay search";
            this.DuplicateButton.UseVisualStyleBackColor = true;
            this.DuplicateButton.Click += new System.EventHandler(this.DuplicateReplaySearch);
            // 
            // StatusStrip1
            // 
            this.StatusStrip1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.StatusStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.StatusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel,
            this.ToolStripProgressBar1});
            this.StatusStrip1.Location = new System.Drawing.Point(0, 851);
            this.StatusStrip1.Name = "StatusStrip1";
            this.StatusStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 28, 0);
            this.StatusStrip1.Size = new System.Drawing.Size(1164, 37);
            this.StatusStrip1.TabIndex = 55;
            this.StatusStrip1.Text = "StatusStrip1";
            // 
            // statusLabel
            // 
            this.statusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(332, 32);
            this.statusLabel.Spring = true;
            this.statusLabel.Text = "Ready";
            // 
            // ToolStripProgressBar1
            // 
            this.ToolStripProgressBar1.Name = "ToolStripProgressBar1";
            this.ToolStripProgressBar1.Size = new System.Drawing.Size(800, 31);
            this.ToolStripProgressBar1.Step = 1;
            this.ToolStripProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // ReplayManager
            // 
            this.AcceptButton = this.SearchButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1164, 888);
            this.Controls.Add(this.StatusStrip1);
            this.Controls.Add(this.RList);
            this.Controls.Add(this.TabControl1);
            this.Controls.Add(this.ConfigButton);
            this.Controls.Add(this.SelectedReplaysLabel);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MinimumSize = new System.Drawing.Size(1160, 800);
            this.Name = "ReplayManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Replay manager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SaveSettings);
            this.Resize += new System.EventHandler(this.ResizeControls);
            this.ContextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.RList)).EndInit();
            this.TabControl1.ResumeLayout(false);
            this.TabPage1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.TabPage2.ResumeLayout(false);
            this.TabPage2.PerformLayout();
            this.TabPage3.ResumeLayout(false);
            this.StatusStrip1.ResumeLayout(false);
            this.StatusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		internal System.Windows.Forms.Button SearchButton;
		internal System.Windows.Forms.Label PatternLabel;
        internal System.Windows.Forms.TextBox PatternBox;
		internal System.Windows.Forms.Label Label7;
        internal Elmanager.CustomControls.TimeTextBox TimeMinBox;
		internal System.Windows.Forms.Label Label1;
        internal Elmanager.CustomControls.TimeTextBox TimeMaxBox;
		internal System.Windows.Forms.TextBox LevPatternBox;
		internal System.Windows.Forms.Label SearchSpecificLabel;
		internal System.Windows.Forms.Label SelectedReplaysLabel;
		internal System.Windows.Forms.SaveFileDialog SaveFileDialog1;
		internal System.Windows.Forms.ContextMenuStrip ContextMenuStrip1;
		internal System.Windows.Forms.ToolStripMenuItem SelectAllToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem InvertToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem CopyToToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem MoveToToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem RenamePatternToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem CompareToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem OpenViewerMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem MergeToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem SaveListToTextFileToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem RemoveFromListToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem DeleteToolStripMenuItem;
		internal System.Windows.Forms.ToolStripMenuItem OpenLevelMenuItem;
		internal BrightIdeasSoftware.ObjectListView RList;
		internal BrightIdeasSoftware.OLVColumn OlvColumn1;
		internal BrightIdeasSoftware.OLVColumn OlvColumn2;
		internal BrightIdeasSoftware.OLVColumn OlvColumn3;
		internal BrightIdeasSoftware.OLVColumn OlvColumn4;
		internal BrightIdeasSoftware.OLVColumn OlvColumn5;
		internal BrightIdeasSoftware.OLVColumn OlvColumn6;
		internal System.Windows.Forms.Button ConfigButton;
        internal System.Windows.Forms.FolderBrowserDialog FolderBrowserDialog1;
		internal BrightIdeasSoftware.OLVColumn OlvColumn7;
		internal System.Windows.Forms.TabControl TabControl1;
		internal System.Windows.Forms.TabPage TabPage1;
		internal System.Windows.Forms.TabPage TabPage2;
		internal System.Windows.Forms.TabPage TabPage3;
		internal System.Windows.Forms.Button DuplicateFilenameButton;
		internal System.Windows.Forms.Button DuplicateButton;
		internal System.Windows.Forms.Button ResetButton;
		internal System.Windows.Forms.Label Label15;
		internal System.Windows.Forms.Label Label16;
		internal System.Windows.Forms.Label Label17;
		internal System.Windows.Forms.Label Label18;
		internal System.Windows.Forms.Label Label19;
		internal System.Windows.Forms.Label Label20;
		internal Elmanager.CustomControls.NumericTextBox TextBox13;
		internal Elmanager.CustomControls.NumericTextBox TextBox12;
		internal Elmanager.CustomControls.NumericTextBox TextBox15;
		internal Elmanager.CustomControls.NumericTextBox TextBox16;
		internal Elmanager.CustomControls.NumericTextBox TextBox5;
		internal Elmanager.CustomControls.NumericTextBox TextBox18;
		internal Elmanager.CustomControls.NumericTextBox TextBox7;
		internal Elmanager.CustomControls.NumericTextBox TextBox20;
		internal Elmanager.CustomControls.NumericTextBox TextBox9;
		internal Elmanager.CustomControls.NumericTextBox TextBox22;
		internal Elmanager.CustomControls.NumericTextBox TextBox10;
		internal Elmanager.CustomControls.NumericTextBox TextBox3;
		internal System.Windows.Forms.Label Label14;
		internal System.Windows.Forms.Label Label13;
		internal System.Windows.Forms.Label Label12;
		internal System.Windows.Forms.Label Label11;
		internal System.Windows.Forms.Label Label10;
		internal System.Windows.Forms.Label Label9;
		internal Elmanager.CustomControls.NumericTextBox TextBox24;
		internal Elmanager.CustomControls.NumericTextBox TextBox11;
		internal Elmanager.CustomControls.NumericTextBox TextBox23;
		internal Elmanager.CustomControls.NumericTextBox TextBox14;
		internal Elmanager.CustomControls.NumericTextBox TextBox8;
		internal Elmanager.CustomControls.NumericTextBox TextBox17;
		internal Elmanager.CustomControls.NumericTextBox TextBox6;
		internal Elmanager.CustomControls.NumericTextBox TextBox19;
		internal Elmanager.CustomControls.NumericTextBox TextBox4;
		internal Elmanager.CustomControls.NumericTextBox TextBox21;
		internal Elmanager.CustomControls.NumericTextBox TextBox2;
		internal Elmanager.CustomControls.NumericTextBox TextBox1;
		internal System.Windows.Forms.Label Label8;
		internal System.Windows.Forms.Label Label2;
		internal System.Windows.Forms.Label Label6;
		internal System.Windows.Forms.Label Label5;
		internal System.Windows.Forms.Label Label4;
		internal System.Windows.Forms.Label Label3;
		internal System.Windows.Forms.Label Label21;
		internal System.Windows.Forms.Label Label22;
		internal System.Windows.Forms.StatusStrip StatusStrip1;
		internal System.Windows.Forms.ToolStripStatusLabel statusLabel;
		internal System.Windows.Forms.ToolStripProgressBar ToolStripProgressBar1;
		internal System.Windows.Forms.Button ReplaysIncorrectLevButton;
		internal System.Windows.Forms.Button ReplaysWithoutLevFileButton;
        internal System.Windows.Forms.ToolStripMenuItem RenameToolStripMenuItem;
        internal BrightIdeasSoftware.OLVColumn olvColumn8;
        internal BrightIdeasSoftware.OLVColumn olvColumn9;
        private System.ComponentModel.IContainer components;
        internal Elmanager.CustomControls.NumericTextBox maxFileSizeBox;
        internal Label label24;
        internal Elmanager.CustomControls.NumericTextBox minFileSizeBox;
        private Label label23;
        private DateTimePicker maxDateTime;
        internal Label label26;
        private DateTimePicker minDateTime;
        private Label label25;
        private TriSelect intExtSelect;
        private TriSelect singleMultiSelect;
        private TriSelect finishedSelect;
        private TriSelect fastestSlowestSelect;
        private TriSelect elmaAcrossSelect;
        private ToolStripMenuItem uploadToZworqyToolStripMenuItem;
        private ToolStripMenuItem zworqyToolStripMenuItem;
        private ToolStripMenuItem k10xnetToolStripMenuItem;
        private ToolTip toolTip1;
        private TableLayoutPanel tableLayoutPanel1;
        private Panel panel1;
        private TableLayoutPanel tableLayoutPanel2;
    }
	
}
