using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Elmanager.LevelEditor.ShapeGallery;

public partial class MainForm : Form
{
    private FlowLayoutPanel flowLayoutPanel;
    private VScrollBar vScrollBar;
    private List<CustomShapeControl> reusableControls;
    private List<string> imageFilePaths;
    private int itemsPerPage = 9;  // 3 rows × 3 columns

    public MainForm()
    {
        //InitializeComponent();
        SetupUI();
        LoadImageFilePaths(@"C:\path\to\your\images");  // Change to your directory
        SetupReusableControls(9);  // Create 9 reusable user controls
        SetupScrollBar();
        UpdateDisplayedControls(0);  // Initialize with the first page
    }

    private void SetupUI()
    {
        flowLayoutPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = true,
            FlowDirection = FlowDirection.LeftToRight
        };

        vScrollBar = new VScrollBar
        {
            Dock = DockStyle.Right,
            Width = 20
        };
        vScrollBar.ValueChanged += vScrollBar_ValueChanged;

        this.Controls.Add(flowLayoutPanel);
        this.Controls.Add(vScrollBar);
    }

    private void LoadImageFilePaths(string directoryPath)
    {
        if (Directory.Exists(directoryPath))
        {
            imageFilePaths = Directory.EnumerateFiles(directoryPath, "*.jpg")
                                      .Concat(Directory.EnumerateFiles(directoryPath, "*.png"))
                                      .Concat(Directory.EnumerateFiles(directoryPath, "*.jpeg"))
                                      .ToList();
        }
        else
        {
            MessageBox.Show("Directory not found!");
            imageFilePaths = new List<string>();
        }
    }

    private void SetupReusableControls(int numControls)
    {
        reusableControls = new List<CustomShapeControl>();

        for (int i = 0; i < numControls; i++)
        {
            //var uc = new CustomShapeControl();
            //flowLayoutPanel.Controls.Add(uc);
            //reusableControls.Add(uc);
        }
    }

    private void SetupScrollBar()
    {
        int totalPages = (int)Math.Ceiling(1.0 * imageFilePaths.Count / itemsPerPage);
        vScrollBar.Minimum = 0;
        vScrollBar.Maximum = Math.Max(totalPages - 1, 0);  // Ensure the scrollbar has valid range
        vScrollBar.LargeChange = 1;
        vScrollBar.SmallChange = 1;
    }

    private void vScrollBar_ValueChanged(object sender, EventArgs e)
    {
        UpdateDisplayedControls(vScrollBar.Value);
    }

    private void UpdateDisplayedControls(int startPage)
    {
        int startIndex = startPage * itemsPerPage;

        for (int i = 0; i < reusableControls.Count; i++)
        {
            int dataIndex = startIndex + i;
            if (dataIndex < imageFilePaths.Count)
            {
                reusableControls[i].Visible = true;
                //reusableControls[i].UpdateContent(imageFilePaths[dataIndex], $"Item {dataIndex}");
            }
            else
            {
                reusableControls[i].Visible = false;
            }
        }
    }
}
