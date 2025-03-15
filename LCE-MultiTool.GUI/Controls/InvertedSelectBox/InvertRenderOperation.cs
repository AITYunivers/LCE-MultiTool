using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Avalonia.Threading;
using SkiaSharp;
using System;
using System.Diagnostics;
using System.IO;

/*
    ATTENTION EVERYONE READING THIS

    This is called "InvertedSelectBox" and has a lot of unused shit in it
    bc I ripped it out of another project of mine and just edited it
    and I'm too lazy to rename it and remove all the random shit

    If you wanna make a PR specifically to clean this code up
    please go right ahead lol
 */

namespace LCE_MultiTool.GUI
{
    public class InvertRenderOperation : ICustomDrawOperation
    {
        private readonly InvertedSelectBox _simpleBlur;
        private readonly Color _blurBackground;
        private readonly int _blur;
        private readonly Rect _bounds;
        private readonly Avalonia.Media.Imaging.Bitmap? _image;
        private bool _disposed;

        public InvertRenderOperation(InvertedSelectBox simpleBlur, Color blurBackground, int blur, Rect bounds, Avalonia.Media.Imaging.Bitmap? image)
        {
            _simpleBlur = simpleBlur;
            _blurBackground = blurBackground;
            _blur = blur;
            _bounds = bounds;
            _image = image;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
        }

        public bool HitTest(Point p) => _bounds.Contains(p);

        public void Render(ImmediateDrawingContext context)
        {
            Stopwatch sw = Stopwatch.StartNew();
            if (_disposed)
                throw new ObjectDisposedException(nameof(InvertRenderOperation));

            if (_image == null)
                return;

            var leaseFeature = context.PlatformImpl.GetFeature<ISkiaSharpApiLeaseFeature>();
            if (leaseFeature == null)
                return;

            using var lease = leaseFeature.Lease();

            if (!lease.SkCanvas.TotalMatrix.TryInvert(out SKMatrix currentInvertedTransform))
                return;

            using var stream = new MemoryStream();
            _image.Save(stream);
            stream.Seek(0, SeekOrigin.Begin);
            using var skBitmap = SKBitmap.Decode(stream);
            if (skBitmap == null)
                return;

            // Maintain aspect ratio while scaling
            float imageAspect = (float)skBitmap.Width / skBitmap.Height;
            float controlAspect = (float)_bounds.Width / (float)_bounds.Height;

            float scaleFactor;
            float destWidth, destHeight;
            float offsetX = 0, offsetY = 0;

            if (imageAspect > controlAspect)
            {
                // Image is wider than control, fit by width
                scaleFactor = (float)_bounds.Width / skBitmap.Width;
                destWidth = (int)_bounds.Width;
                destHeight = skBitmap.Height * scaleFactor;
                offsetY = (int)(_bounds.Height - destHeight) / 2;
            }
            else
            {
                // Image is taller than control, fit by height
                scaleFactor = (float)_bounds.Height / skBitmap.Height;
                destHeight = (int)_bounds.Height;
                destWidth = skBitmap.Width * scaleFactor;
                offsetX = (int)(_bounds.Width - destWidth) / 2;
            }

            SKRect srcRect = SKRect.Create(0, 0, skBitmap.Width, skBitmap.Height);
            SKRect destRect = SKRect.Create(offsetX, offsetY, destWidth, destHeight);

            using var paint = new SKPaint
            {
                ColorFilter = SKColorFilter.CreateColorMatrix(new float[]
                {
                        0, 0, 1, 0, 0,  // Swap Red and Blue
                        0, 1, 0, 0, 0,  // Keep Green
                        1, 0, 0, 0, 0,  // Swap Blue and Red
                        0, 0, 0, 1, 0   // Alpha remains unchanged
                })
            };

            lease.SkCanvas.SaveLayer(paint);
            lease.SkCanvas.DrawBitmap(skBitmap, srcRect, destRect);
            lease.SkCanvas.Restore();
            sw.Stop();
        }

        public Rect Bounds => _bounds;

        public bool Equals(ICustomDrawOperation? other)
        {
            return other is InvertRenderOperation op &&
                   op._bounds == _bounds &&
                   op._blurBackground.Equals(_blurBackground) &&
                   op._blur.Equals(_blur);
        }
    }
}
