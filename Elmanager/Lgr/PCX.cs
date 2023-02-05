using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace Elmanager.Lgr;

internal class Pcx
{
    private byte BitsPerPixel { get; }
    private short BytesPerLine { get; }
    private byte[] Colormap { get; } = new byte[48];
    private byte Encoding { get; }
    private short HDpi { get; }
    private int Height { get; }
    private int HscreenSize { get; }
    private byte Manufacturer { get; }
    private byte NumPlanes { get; }
    private short PaletteInfo { get; }
    private int[] PixelData { get; }
    private short VDpi { get; }
    private byte Version { get; }
    private int VscreenSize { get; }

    private int Width { get; }
    private short Xmax { get; }
    private short Xmin { get; }
    private short Ymax { get; }
    private short Ymin { get; }

    internal Pcx(Stream pcxStream)
    {
        var pb = new BinaryReader(pcxStream);
        // pcxStream.Seek(0, SeekOrigin.Begin);
        Manufacturer = pb.ReadByte();
        Version = pb.ReadByte();
        Encoding = pb.ReadByte();
        BitsPerPixel = pb.ReadByte();
        Xmin = pb.ReadInt16();
        Ymin = pb.ReadInt16();
        Xmax = pb.ReadInt16();
        Ymax = pb.ReadInt16();
        HDpi = pb.ReadInt16();
        VDpi = pb.ReadInt16();
        pb.Read(Colormap, 0, 48);
        pb.ReadByte();
        NumPlanes = pb.ReadByte();
        BytesPerLine = pb.ReadInt16();
        PaletteInfo = pb.ReadInt16();
        HscreenSize = pb.ReadInt16();
        VscreenSize = pb.ReadInt16();
        pb.BaseStream.Seek(54, SeekOrigin.Current);
        Width = Xmax - Xmin + 1;
        Height = Ymax - Ymin + 1;
        var totalBytes = NumPlanes * BytesPerLine;
        var uncompressedData = new byte[totalBytes * Height];
        var total = 0;
        for (var i = 0; i < Height; i++)
        {
            var subTotal = 0;
            while (subTotal != totalBytes)
            {
                var data = pb.ReadByte();
                if (data >= 192)
                {
                    var count = data & 0x3F;
                    data = pb.ReadByte();
                    for (var k = 1; k <= count; k++)
                    {
                        uncompressedData[total] = data;
                        total++;
                    }

                    subTotal += count;
                }
                else
                {
                    uncompressedData[total] = data;
                    subTotal++;
                    total++;
                }
            }
        }

        pb.ReadByte();
        if (Version == 5)
        {
            var palette = new int[256];
            PixelData = new int[Width * Height];

            for (var i = 0; i < 256; i++)
            {
                int red = pb.ReadByte();
                int green = pb.ReadByte();
                int blue = pb.ReadByte();
                palette[i] = (red << 16) + (green << 8) + (blue << 0);
            }

            for (var i = 0; i < Height; i++)
                for (var j = 0; j < Width; j++)
                    PixelData[i * Width + j] = palette[uncompressedData[i * BytesPerLine + j]];
        }
        else
            throw new Exception("This version of PCX file is not supported!");


        // pb.Close();
    }

    internal Bitmap ToBitmap()
    {
        var bmp = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
        var bmpData = bmp.LockBits(
            new Rectangle(0, 0, bmp.Width, bmp.Height),
            ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
        Marshal.Copy(PixelData, 0, bmpData.Scan0, PixelData.Length);
        bmp.UnlockBits(bmpData);
        return bmp;
    }
}