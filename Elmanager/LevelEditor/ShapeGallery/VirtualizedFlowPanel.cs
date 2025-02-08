using System;
using System.Collections.Generic;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using Image = System.Drawing.Image;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace Elmanager.LevelEditor.ShapeGallery;

public class VirtualizedFlowPanel : System.Windows.Controls.Panel
{
    private int totalItems;
    private int rows, columns;
    private List<LevelControl> virtualizedControls;
    private int startIndex = 0;

    private Func<int, (Image, string)> dataProvider;

    public int ItemHeight { get; set; } = 128;
    public int ItemWidth { get; set; } = 128;

    public void Initialize(int totalItems, Func<int, (Image, string)> dataProvider, int rows = 3, int columns = 3)
    {
        this.totalItems = totalItems;
        this.dataProvider = dataProvider;
        this.rows = rows;
        this.columns = columns;

        // Enable scrolling
        this.AutoScroll = true;
        this.VerticalScroll.SmallChange = ItemHeight;
        this.SetScrollBar();

        CreateVirtualControls();
        UpdateControls();
    }

    private void SetScrollBar()
    {
        int virtualHeight = (totalItems / columns) * ItemHeight;
        this.AutoScrollMinSize = new Size(0, virtualHeight);
    }

    private void CreateVirtualControls()
    {
        virtualizedControls = new List<ImageLabelControl>();

        for (int i = 0; i < rows * columns; i++)
        {
            var control = new ImageLabelControl();
            control.Size = new Size(ItemWidth, ItemHeight);
            control.Location = GetControlPosition(i);
            virtualizedControls.Add(control);
            this.Controls.Add(control);
        }
    }

    private Point GetControlPosition(int index)
    {
        int row = index / columns;
        int col = index % columns;
        return new Point(col * ItemWidth, row * ItemHeight);
    }

    private void UpdateControls()
    {
        int firstVisibleIndex = VerticalScroll.Value / ItemHeight * columns;

        for (int i = 0; i < virtualizedControls.Count; i++)
        {
            int dataIndex = firstVisibleIndex + i;
            if (dataIndex < totalItems)
            {
                var (image, text) = dataProvider(dataIndex);  // Get data from the provider
                virtualizedControls[i].Visible = true;
                virtualizedControls[i].Location = GetControlPosition(dataIndex - firstVisibleIndex);
                virtualizedControls[i].UpdateContent(image, text);
            }
            else
            {
                virtualizedControls[i].Visible = false;
            }
        }
    }

    protected override void OnScroll(ScrollEventArgs se)
    {
        base.OnScroll(se);
        UpdateControls();
    }
}