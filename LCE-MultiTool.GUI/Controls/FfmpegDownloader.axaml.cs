using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace LCE_MultiTool;

public partial class FfmpegDownloader : Window
{
    private static string ffmpegDownload = @"https://www.gyan.dev/ffmpeg/builds/ffmpeg-release-essentials.zip";

    public FfmpegDownloader()
    {
        InitializeComponent();

        if (Application.Current?.ActualThemeVariant == ThemeVariant.Light)
            Background = new SolidColorBrush(Color.FromArgb(0xC0, 0xFE, 0xFC, 0xFF));
    }

    public void Start()
    {
        DownloadProgress.Maximum = GetFilesize() ?? 0;
        DownloadFfmpegAsync().ContinueWith(task =>
        {
            DecompressFfmpeg();
            Dispatcher.UIThread.Invoke(Close);
        });
    }

    private static long? GetFilesize()
    {
        using HttpClient client = new HttpClient();
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Head, ffmpegDownload);
        HttpResponseMessage response = client.Send(request);
        return response.Content.Headers.ContentLength;
    }

    private async Task DownloadFfmpegAsync()
    {
        using HttpClient client = new HttpClient();
        HttpResponseMessage response = await client.GetAsync(ffmpegDownload, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        var totalBytes = response.Content.Headers.ContentLength ?? 1;

        using var contentStream = await response.Content.ReadAsStreamAsync();
        using FileStream fileStream = File.Create("ffmpeg.zip");

        var buffer = new byte[8192];
        int bytesRead;

        while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            fileStream.Write(buffer, 0, bytesRead);
            Dispatcher.UIThread.Invoke(() => DownloadProgress.Value += bytesRead);
        }

        fileStream.Close();
        contentStream.Dispose();
    }

    private void DecompressFfmpeg()
    {
        if (!File.Exists("ffmpeg.zip"))
            return;

        ZipArchive zip = ZipFile.OpenRead("ffmpeg.zip");
        ZipArchiveEntry? ffmpeg = zip.GetEntry(zip.Entries[0].FullName + "bin/ffmpeg.exe");
        if (ffmpeg is not null)
            ffmpeg.ExtractToFile("ffmpeg.exe");

        zip.Dispose();
        File.Delete("ffmpeg.zip");
    }
}