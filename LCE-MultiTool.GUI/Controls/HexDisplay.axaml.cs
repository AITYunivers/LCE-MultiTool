using Avalonia.Controls;
using AvaloniaHex.Document;
using LCE_MultiTool.Data;
using LCE_MultiTool.GUI.Memory;
using System;
using System.IO;

namespace LCE_MultiTool;

public partial class HexDisplay : UserControl, IDisposable
{
    public HexDisplay() : this(null) { }
    public HexDisplay(FileData fileData)
    {
        InitializeComponent();

        if (fileData is null)
            return;

        if (fileData.Data is null && fileData.RealFilePath is not null)
            Editor.Document = new MemoryBinaryDocument(File.ReadAllBytes(fileData.RealFilePath), true);
        else if (fileData.Data is not null)
            Editor.Document = new MemoryBinaryDocument(new ByteReader(fileData.GetData()!).ReadBytes(), true);
    }

    public void Dispose()
    {
        Editor.Document?.Dispose();
    }
}