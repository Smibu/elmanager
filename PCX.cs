using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace Elmanager
{
    /// <summary>
    ///   Reads PCX files.
    /// </summary>
    internal class Pcx
    {
        internal byte BitsPerPixel;
        internal short BytesPerLine;
        internal byte[] Colormap = new byte[48];
        internal byte Encoding;
        internal short HDpi;
        internal int Height;
        internal int HscreenSize;
        internal byte Manufacturer;
        internal byte NumPlanes;
        internal short PaletteInfo;
        internal int[] PixelData;
        internal short VDpi;
        internal byte Version;
        internal int VscreenSize;

        internal int Width;
        internal short Xmax;
        internal short Xmin;
        internal short Ymax;
        internal short Ymin;

        internal Pcx(string pcxFile)
            : this(new FileStream(pcxFile, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
        }

        internal unsafe Pcx(Stream pcxStream)
        {
            BinaryReader pb = new BinaryReader(pcxStream);
            pcxStream.Seek(0, SeekOrigin.Begin);
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
            int totalBytes = NumPlanes * BytesPerLine;
            byte[] uncompressedData = new byte[totalBytes * Height];
            fixed (byte* uncompressedDataPtr = &uncompressedData[0])
            {
                int total = 0;
                for (int i = 0; i < Height; i++)
                {
                    int subTotal = 0;
                    while (subTotal != totalBytes)
                    {
                        byte data = pb.ReadByte();
                        if (data >= 192)
                        {
                            int count = data & 0x3F;
                            data = pb.ReadByte();
                            for (int k = 1; k <= count; k++)
                            {
                                uncompressedDataPtr[total] = data;
                                total++;
                            }

                            subTotal += count;
                        }
                        else
                        {
                            uncompressedDataPtr[total] = data;
                            subTotal++;
                            total++;
                        }
                    }
                }

                pb.ReadByte();
                if (Version == 5)
                {
                    int[] palette = new int[256];
                    PixelData = new int[Width * Height];
                    fixed (int* pixelDataPtr = &PixelData[0], palettePtr = &palette[0])
                    {
                        for (int i = 0; i < 256; i++)
                        {
                            int red = pb.ReadByte();
                            int green = pb.ReadByte();
                            int blue = pb.ReadByte();
                            palettePtr[i] = (red << 16) + (green << 8) + (blue << 0);
                        }

                        for (int i = 0; i < Height; i++)
                        for (int j = 0; j < Width; j++)
                            pixelDataPtr[i * Width + j] = palettePtr[uncompressedDataPtr[i * BytesPerLine + j]];
                    }
                }
                else
                    throw (new Exception("This version of PCX file is not supported!"));
            }

            pb.Close();
        }

        /// <summary>
        ///   Converts this PCX instance to a System.Drawing.Bitmap.
        /// </summary>
        /// <returns>This instance converted to a System.Drawing.Bitmap.</returns>
        internal Bitmap ToBitmap()
        {
            Bitmap bmp = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
            BitmapData bmpData = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(PixelData, 0, bmpData.Scan0, PixelData.Length);
            bmp.UnlockBits(bmpData);
            return bmp;
        }
    }
}