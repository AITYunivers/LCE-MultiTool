using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Threading;
using LCE_MultiTool.Data;
using LCE_MultiTool.GUI.Memory;
using NAudio.Wave;
using SkiaSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Numerics;
using System.Threading.Tasks;

namespace LCE_MultiTool;

public partial class AudioPlayer : UserControl, IDisposable
{
    public WaveOutEvent SongOut = null!;
    public RawSourceWaveStream SongReader = null!;
    public DispatcherTimer TickTimer = null!;
    private bool _movingCursor = false;

    public AudioPlayer() : this(null) { }
    public AudioPlayer(FileData? fileData)
    {
        InitializeComponent();
        if (fileData == null || fileData.Type != EFileType.Audio)
            return;

        LoadSong(fileData).ContinueWith((t) =>
        {
            TickTimer = new DispatcherTimer(DispatcherPriority.MaxValue);
            TickTimer.Interval = TimeSpan.FromSeconds(0.1);
            TickTimer.Tick += Tick;
            TickTimer.Start();
        });
    }

    private void Tick(object? sender, EventArgs e)
    {
        if (SongOut is null || SongReader is null || TickTimer is null)
            return;

        // Controls
        string pauseValue = SongOut?.PlaybackState == PlaybackState.Playing ? "fa-pause" : "fa-play";
        if (PauseBtn.Value != pauseValue)
            PauseBtn.Value = pauseValue;

        // Progress
        if (!_movingCursor)
        {
            SongProgress.Value = SongReader.CurrentTime.TotalSeconds;
            ProgressCurTime.Text = SongReader.CurrentTime.Minutes + ":" + SongReader.CurrentTime.Seconds.ToString("D2");
            ProgressEndTime.Text = SongReader.TotalTime.Minutes + ":" + SongReader.TotalTime.Seconds.ToString("D2");
        }
    }

    public async Task LoadSong(FileData fileData)
    {
        if (fileData.Data is null && fileData.RealFilePath is not null)
            fileData.Data = new FileStream(fileData.RealFilePath, FileMode.Open);
        else if (fileData.Data is null)
            return;

        SongName.Text = CapitalizeName(Path.GetFileNameWithoutExtension(fileData.Name));

        ByteReader reader = new ByteReader(fileData.GetData()!, true);
        reader.Skip(5); // Header
        int channels = reader.ReadByte();
        int sampleRate = reader.ReadUShort();
        reader.Dispose();

        using (var ffmpegProcess = new Process())
        {
            ffmpegProcess.StartInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-nostdin -f binka -i pipe:0 -ac {channels} -ar {sampleRate} -f wav pipe:1",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            ffmpegProcess.Start();

            // Write MemoryStream data to FFmpeg's standard input asynchronously
            var writeTask = Task.Run(async () =>
            {
                await fileData.GetData()!.CopyToAsync(ffmpegProcess.StandardInput.BaseStream);
                ffmpegProcess.StandardInput.Close(); // Signal FFmpeg that the stream is done
            });

            // Read PCM data into memory
            var pcmMemoryStream = new MemoryStream();
            await ffmpegProcess.StandardOutput.BaseStream.CopyToAsync(pcmMemoryStream);
            pcmMemoryStream.Position = 0;

            await ffmpegProcess.WaitForExitAsync();

            SongReader = new RawSourceWaveStream(pcmMemoryStream, new WaveFormat(sampleRate, 16, channels));
            SongOut = new WaveOutEvent();
            SongOut.Init(SongReader); // Ready for playback, but not playing yet

            SongProgress.Maximum = SongReader.TotalTime.TotalSeconds;
        }
    }

    private void PauseBtnPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        if (SongOut.PlaybackState == PlaybackState.Playing)
            SongOut.Pause();
        else if (SongOut.PlaybackState != PlaybackState.Playing)
            SongOut.Play();
    }

    private void ProgressPressed(object? sender, PointerPressedEventArgs e)
    {
        _movingCursor = true;
        UpdateProgress(e.GetPosition(ProgressHit));
    }

    private void ProgressMoved(object? sender, PointerEventArgs e)
    {
        _movingCursor = e.GetCurrentPoint(sender as Control).Properties.IsLeftButtonPressed;
        if (_movingCursor)
            UpdateProgress(e.GetPosition(ProgressHit));
    }

    private void ProgressReleased(object? sender, PointerReleasedEventArgs e)
    {
        _movingCursor = false;
        UpdateProgress(e.GetPosition(ProgressHit), true);
    }

    private void UpdateProgress(Point pointerPos, bool setProgress = false)
    {
        double progress = Math.Clamp(pointerPos.X / ProgressHit.Bounds.Width, 0, 1);
        TimeSpan progressTime = SongReader.TotalTime * progress;
        SongProgress.Value = progressTime.TotalSeconds;
        ProgressCurTime.Text = progressTime.Minutes + ":" + progressTime.Seconds.ToString("D2");
        if (setProgress)
            SongReader.CurrentTime = progressTime;
    }

    public void Dispose()
    {
        SongOut?.Stop();
        SongOut?.Dispose();
        SongReader?.Dispose();
        TickTimer.Stop();
    }

    private string CapitalizeName(string name)
    {
        string[] strings = name.Split('_');
        for (int i = 0; i < strings.Length; i++)
            strings[i] = strings[i][..1].ToUpper() + strings[i][1..];
        return string.Join(' ', strings);
    }
}