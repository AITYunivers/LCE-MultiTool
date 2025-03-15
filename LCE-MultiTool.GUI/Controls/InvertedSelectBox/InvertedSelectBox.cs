using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;

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
    public class InvertedSelectBox : ContentControl
    {
        public static readonly StyledProperty<bool> TransparencyAllowedProperty =
            AvaloniaProperty.Register<InvertedSelectBox, bool>(nameof(TransparencyAllowed), true);
        public bool TransparencyAllowed { get => GetValue(TransparencyAllowedProperty); set => SetValue(TransparencyAllowedProperty, value); }

        public static readonly StyledProperty<Color> BlurBackgroundProperty =
            AvaloniaProperty.Register<InvertedSelectBox, Color>(nameof(BlurBackground));
        public Color BlurBackground { get => GetValue(BlurBackgroundProperty); set => SetValue(BlurBackgroundProperty, value); }

        public static readonly StyledProperty<int> BlurProperty =
            AvaloniaProperty.Register<InvertedSelectBox, int>(nameof(Blur), 25);
        public int Blur { get => GetValue(BlurProperty); set => SetValue(BlurProperty, value); }

        public static readonly StyledProperty<Bitmap?> ImageProperty =
    AvaloniaProperty.Register<InvertedSelectBox, Bitmap?>(nameof(Image));
        public Bitmap? Image { get => GetValue(ImageProperty); set => SetValue(ImageProperty, value); }


        static InvertedSelectBox()
        {
            AffectsRender<InvertedSelectBox>(BlurBackgroundProperty);
            AffectsRender<InvertedSelectBox>(BlurProperty);
            AffectsRender<InvertedSelectBox>(TransparencyAllowedProperty);
            AffectsRender<InvertedSelectBox>(ImageProperty);
        }

        public override void Render(DrawingContext context)
        {
            if (TransparencyAllowed)
            {
                context.Custom(
                    new InvertRenderOperation(
                        this,
                        BlurBackground,
                        Blur,
                        new Rect(default, Bounds.Size),
                        Image));
                return;
            }
            base.Render(context);
        }
    }
}
