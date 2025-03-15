using Avalonia.Controls;
using Avalonia.Media.Imaging;
using LCE_MultiTool.Data;
using SixLabors.Fonts;
using SixLabors.Fonts.Unicode;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Linq;
using ISImage = SixLabors.ImageSharp.Image;

namespace LCE_MultiTool;

public partial class FontViewer : UserControl, IDisposable
{
    FontFamily fontFamily;
    Font? FontSize64;
    Font? FontSize32;
    Font? FontSize16;
    Font? FontSize8;

    Size oldSize = Size.Empty;

    public FontViewer() : this(null) {}
    public FontViewer(FileData? fileData)
    {
        InitializeComponent();

        if (fileData == null || !File.Exists(fileData.RealFilePath))
            return;

        FontCollection fonts = new FontCollection();
        fontFamily = fonts.Add(fileData.RealFilePath);
        FontSize64 = fontFamily.CreateFont(64, SixLabors.Fonts.FontStyle.Regular);
        FontSize32 = fontFamily.CreateFont(32, SixLabors.Fonts.FontStyle.Regular);
        FontSize16 = fontFamily.CreateFont(16, SixLabors.Fonts.FontStyle.Regular);
        FontSize8 = fontFamily.CreateFont(8, SixLabors.Fonts.FontStyle.Regular);

        SizeChanged += (s, e) => GenerateImage((int)e.NewSize.Width, (int)e.NewSize.Height);
    }

    private static string[] ExampleTexts =
    [
        "The Quick Brown Fox Jumps Over The Lazy Dog", // English
        "乔装打扮的狐狸跳过懒狗。", // Chinese (Simplified)
        "視野無限廣，窗外有藍天。", // Chinese (Traditional)
        "いろはにほへと ちりぬるを わかよたれそ つねならむ", // Japanese
        "키스의 고유 조건은 입술끼리 만나야 하고 특별한 기술은 필요치 않다." // Korean
    ];
    private void GenerateImage(int width, int height)
    {
        if (oldSize.Width > width && oldSize.Height > height)
            return;

        oldSize = new Size((int)Math.Ceiling(width / 250.0) * 250, (int)Math.Ceiling(height / 250.0) * 250);
        using ISImage img = new Image<Rgba32>(oldSize.Width, oldSize.Height);

        int yPos = 0;
        for (int i = 0; i < ExampleTexts.Length; i++)
        {
            if (!FontSize64!.TryGetGlyphs(new CodePoint(ExampleTexts[i][0]), out var glyphs) || !glyphs.Any(x => x.GlyphMetrics.GlyphId > 0))
                continue;

            img.Mutate(ctx => ctx.DrawText(ExampleTexts[i], FontSize64!, Color.White, new Point(0, yPos + 0)));
            img.Mutate(ctx => ctx.DrawText(ExampleTexts[i], FontSize32!, Color.White, new Point(0, yPos + 74)));
            img.Mutate(ctx => ctx.DrawText(ExampleTexts[i], FontSize16!, Color.White, new Point(0, yPos + 116)));
            img.Mutate(ctx => ctx.DrawText(ExampleTexts[i], FontSize8!, Color.White, new Point(0, yPos + 142)));
            yPos += 160;
        }

        using Stream stream = new MemoryStream();
        img.Save(stream, new PngEncoder());
        stream.Position = 0;

        ((Bitmap?)ViewerImage.Source)?.Dispose();
        ViewerImage.Source = new Bitmap(stream);
    }

    public void Dispose()
    {
        ((Bitmap?)ViewerImage.Source)?.Dispose();
    }
}