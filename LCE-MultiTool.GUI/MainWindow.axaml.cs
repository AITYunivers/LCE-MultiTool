using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using LCE_MultiTool.GUI.Data.Localization;
using LCE_MultiTool.GUI.Memory;
using Semi.Avalonia;
using System.IO;

namespace LCE_MultiTool.GUI
{
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; } = null!;

        public MainWindow()
        {
            Instance = this;
            InitializeComponent();

            if (Application.Current?.ActualThemeVariant == ThemeVariant.Light)
                Background = new SolidColorBrush(Color.FromArgb(0xC0, 0xFE, 0xFC, 0xFF));

            Opened += (s, e) =>
            {
                if (!File.Exists("ffmpeg.exe"))
                {
                    FfmpegDownloader ffmpegDownloader = new FfmpegDownloader();
                    ffmpegDownloader.ShowDialog(this);
                    ffmpegDownloader.Start();
                }
            };
        }

        private void Menu_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (sender is Border)
                BeginMoveDrag(e);
        }

        private void MenuItem_Click(object? sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender!;
            switch (item.Name)
            {
                case "OpenFile":
                    var file = TopLevel.GetTopLevel(this).StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                    {
                        Title = "Open File",
                        AllowMultiple = false
                    }).Result;

                    if (file.Count >= 1)
                        FileTree.LoadFromFile(file[0].Path.LocalPath);
                    break;
                case "OpenFolder":
                    var folder = TopLevel.GetTopLevel(this).StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
                    {
                        Title = "Open Folder",
                        AllowMultiple = false
                    }).Result;

                    if (folder.Count >= 1)
                        FileTree.LoadFromFolder(folder[0].Path.LocalPath.TrimEnd(Path.DirectorySeparatorChar));
                    break;
            }
        }
    }
}