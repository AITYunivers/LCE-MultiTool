using LCE_MultiTool.Data.Archive;
using LCE_MultiTool.Data.FourJUserInterface;
using LCE_MultiTool.Data.Package;
using LCE_MultiTool.Data.SoundBank;
using LCE_MultiTool.GUI.Memory;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace LCE_MultiTool.Data
{
    public class FileData
    {
        public string Name = string.Empty;
        public string? RealFilePath = null;
        public EFileType Type = EFileType.Unknown;
        public Stream? Data = null;

        public FileData(string name, EFileType type, Stream? data)
        {
            Name = name;
            Type = type;
            Data = data;
        }

        public FileData(string path, Stream? data)
        {
            Name = Path.GetFileName(path) ?? Path.GetDirectoryName(path) ?? string.Empty;
            RealFilePath = path;
            Type = GetFileType(path);
            Data = data;
        }

        public FileData(string path)
        {
            Name = Path.GetFileName(path) ?? Path.GetDirectoryName(path) ?? string.Empty;
            RealFilePath = path;
            Type = GetFileType(path);
            if (ShouldLoadData() && File.Exists(path))
                Data = new FileStream(path, FileMode.Open);
        }

        public FileData(ArchiveEntry arcEntry)
        {
            Name = Path.GetFileName(arcEntry.Name);
            Type = GetFileType(arcEntry.Name);
            Data = new MemoryStream(arcEntry.Data);
        }

        public FileData(PackageFile pck, PackageEntry pckEntry)
        {
            Name = GetPackageName(pck, pckEntry.Name, pckEntry.Type);
            Type = GetFileType(pckEntry.Type);
            Data = new MemoryStream(pckEntry.Data);
        }

        public FileData(SoundBankEntry msscmpEntry)
        {
            Name = Path.GetFileName(msscmpEntry.FilePath + ".binka");
            Type = EFileType.Audio;
            RealFilePath = msscmpEntry.FilePath + ".binka";
            Data = new MemoryStream(msscmpEntry.Data);
        }

        public FileData(FUIFile fui, FUIBitmap fuiBitmap)
        {
            Name = fuiBitmap.SymbolIndex != -1 ? fui.Symbols[fuiBitmap.SymbolIndex].SymbolName : "Image[" + Array.IndexOf(fui.Bitmaps, fuiBitmap) + "]";
            Type = EFileType.BGRImage;
            Data = new MemoryStream(fuiBitmap.Data);
        }

        private EFileType GetFileType(string path)
        {
            if (Directory.Exists(path))
                return EFileType.Folder;

            string ext = Path.GetExtension(path);
            switch (ext)
            {
                default:
                    return EFileType.Unknown;
                case ".loc":
                    return EFileType.Localization;
                case ".txt":
                case ".xml":
                    return EFileType.Text;
                case ".arc":
                    return EFileType.Archive;
                case ".png":
                case ".jpg":
                case ".jpeg":
                case ".ico":
                    return EFileType.Image;
                case ".binka":
                    return EFileType.Audio;
                case ".pck":
                    return EFileType.Package;
                case ".msscmp":
                    return EFileType.SoundBank;
                case ".ttf":
                    return EFileType.Font;
                case ".fui":
                    return EFileType.FourJUserInterface;
            }
        }

        private string GetPackageName(PackageFile pck, string pckName, EPackageType pckType)
        {
            switch (pckType)
            {

                default:
                    return Path.GetFileName(pckName);
            }
        }

        private EFileType GetFileType(EPackageType type)
        {
            switch (type)
            {
                default:
                    throw new NotImplementedException("Can't convert EPackageType " + type + " to EFileType");

                case EPackageType.CapeFile:
                case EPackageType.SkinFile:
                case EPackageType.TextureFile:
                    return EFileType.Image;

                case EPackageType.LocalizationFile:
                    return EFileType.Localization;

                case EPackageType.SkinDataFile:
                case EPackageType.TexturePackInfoFile:
                    return EFileType.Package;

                // TEMP FOR INDEXING
                case EPackageType.GameRulesFile:
                case EPackageType.GameRulesHeader:
                case EPackageType.BehavioursFile:
                case EPackageType.ModelsFile:
                case EPackageType.AudioFile:
                case EPackageType.ColorTableFile:
                case EPackageType.MaterialFile:
                case EPackageType.InfoFile:
                    return EFileType.Unknown;
            }
        }

        private bool ShouldLoadData()
        {
            return Type == EFileType.Localization ||
                   Type == EFileType.Archive ||
                   Type == EFileType.Package ||
                   Type == EFileType.SoundBank ||
                   Type == EFileType.FourJUserInterface;
        }

        public Stream? GetData()
        {
            if (Data is not null)
                Data.Position = 0;
            return Data;
        }

        public async Task ExtractAudioAsync(string path, bool wav)
        {
            if (Type != EFileType.Audio)
                return;

            if (wav)
            {
                ByteReader reader = new ByteReader(GetData()!, true);
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
                        await GetData()!.CopyToAsync(ffmpegProcess.StandardInput.BaseStream);
                        ffmpegProcess.StandardInput.Close(); // Signal FFmpeg that the stream is done
                    });

                    // Read PCM data into file
                    FileStream stream = File.Create(path);
                    await ffmpegProcess.StandardOutput.BaseStream.CopyToAsync(stream);
                    await ffmpegProcess.WaitForExitAsync();
                    stream.Close();
                }
            }
            else
            {
                // Read BINKA data into file
                FileStream stream = File.Create(path);
                await GetData()!.CopyToAsync(stream);
                stream.Close();
            }
        }
    }
}
