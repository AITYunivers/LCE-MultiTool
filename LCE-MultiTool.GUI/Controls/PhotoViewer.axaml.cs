using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Server;
using LCE_MultiTool.Data;
using LCE_MultiTool.GUI;
using SkiaSharp;
using System;
using System.IO;

namespace LCE_MultiTool;

public partial class PhotoViewer : UserControl, IDisposable
{
    public PhotoViewer() : this(null) {}
    public PhotoViewer(FileData? fileData)
    {
        InitializeComponent();

        if (fileData == null)
            return;

        if (fileData.Type == EFileType.Image)
        {
            if (fileData.Data == null && File.Exists(fileData.RealFilePath))
                ViewerImage.Source = new Bitmap(fileData.RealFilePath);
            else if (fileData.Data != null)
                ViewerImage.Source = new Bitmap(fileData.GetData()!);
        }
        else if (fileData.Type == EFileType.BGRImage)
        {
            InvertedSelectBox bgrImage = new InvertedSelectBox();
            bgrImage.Image = new Bitmap(fileData.GetData()!);
            ContentBorder.Child = bgrImage;
        }
    }

    public void Dispose()
    {
        ((Bitmap?)ViewerImage.Source)?.Dispose();
    }
}