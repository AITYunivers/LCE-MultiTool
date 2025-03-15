using LCE_MultiTool.GUI.Memory;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;

namespace LCE_MultiTool.Data.FourJUserInterface
{
    public class FUIBitmap
    {
        public int SymbolIndex;
        public FUIBitmapFormat Format;
        public int Width;
        public int Height;
        public int DataOffset;
        public int DataSize;
        public int ZlibOffset;
        public int TextureHandle;

        public byte[] Data = [];

        public FUIBitmap(ByteReader reader)
        {
            SymbolIndex = reader.ReadInt();             // 0x00 + 4 |
            Format = (FUIBitmapFormat)reader.ReadInt(); // 0x04 + 4 |
            Width = reader.ReadInt();                   // 0x08 + 4 |
            Height = reader.ReadInt();                  // 0x0C + 4 |
            DataOffset = reader.ReadInt();              // 0x10 + 4 |
            DataSize = reader.ReadInt();                // 0x14 + 4 |
            ZlibOffset = reader.ReadInt();              // 0x18 + 4 |
            TextureHandle = reader.ReadInt();           // 0x1C + 4 = 0x20
        }

        public void LoadData(ByteReader reader, long bitmapBaseOffset)
        {
            reader.Seek(bitmapBaseOffset + DataOffset);
            Data = reader.ReadBytes(DataSize);
        }

        public enum FUIBitmapFormat
        {
            PNG_WITH_ALPHA_DATA = 1,
            PNG_NO_ALPHA_DATA = 3,
            JPEG_NO_ALPHA_DATA = 6,
            JPEG_UNKNOWN = 7,
            JPEG_WITH_ALPHA_DATA = 8
        }
    }
}
