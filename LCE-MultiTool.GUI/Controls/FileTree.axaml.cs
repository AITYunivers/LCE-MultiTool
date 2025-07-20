using Avalonia.Controls;
using Avalonia.Platform.Storage;
using LCE_MultiTool.Data;
using LCE_MultiTool.Data.Archive;
using LCE_MultiTool.Data.FourJUserInterface;
using LCE_MultiTool.Data.Package;
using LCE_MultiTool.Data.SoundBank;
using LCE_MultiTool.GUI.Data.Localization;
using LCE_MultiTool.GUI.Memory;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LCE_MultiTool.GUI;

public partial class FileTree : UserControl
{
    public FileTree()
    {
        InitializeComponent();
    }

    private void TreeView_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
    {
        if (sender is TreeView treeView)
        {
            if (treeView.SelectedItem is TreeViewItem item)
            {
                if (item.Tag is FileData fileData)
                {
                    if (fileData.Type == EFileType.Folder)
                        return;

                    if (MainWindow.Instance.ContentPanel.Child is IDisposable disposable)
                        disposable.Dispose();

                    try
                    {
                        switch (fileData.Type)
                        {
                            case EFileType.Localization:
                                LocalizationFile loc = LocalizationFile.Load(new ByteReader(fileData.GetData()!));
                                MainWindow.Instance.ContentPanel.Child = new LocalizationBrowser(loc);
                                break;
                            case EFileType.Image:
                            case EFileType.BGRImage:
                                MainWindow.Instance.ContentPanel.Child = new PhotoViewer(fileData);
                                break;
                            case EFileType.Audio:
                                MainWindow.Instance.ContentPanel.Child = new AudioPlayer(fileData);
                                break;
                            case EFileType.Text:
                                MainWindow.Instance.ContentPanel.Child = new TextDisplay(fileData);
                                break;
                            case EFileType.Font:
                                MainWindow.Instance.ContentPanel.Child = new FontViewer(fileData);
                                break;
                            default:
                                if (fileData.Data is not null || fileData.RealFilePath is not null)
                                    MainWindow.Instance.ContentPanel.Child = new HexDisplay(fileData);
                                else
                                    MainWindow.Instance.ContentPanel.Child = null;
                                break;
                        }
                    }
                    catch
                    {
                        MainWindow.Instance.ContentPanel.Child = null;
                    }
                }
            }
        }
    }

    public void LoadFromFile(string path)
    {
        if (!File.Exists(path))
            return;

        FileData fileData = new FileData(path);
        TreeViewItem fileItem = CreateItem(fileData);

        // Mass Sorting
        SortTreeViewItems(fileItem.Items);

        FileTreeView.Items.Clear();
        FileTreeView.Items.Add(fileItem);
    }

    public void LoadFromFolder(string path)
    {
        if (!Directory.Exists(path))
            return;

        FileData fileData = new FileData(path);
        TreeViewItem fileItem = CreateItem(fileData);

        // Mass Sorting
        SortTreeViewItems(fileItem.Items);

        FileTreeView.Items.Clear();
        FileTreeView.Items.Add(fileItem);
    }

    public TreeViewItem CreateItem(FileData fileData)
    {
        TreeViewItem fileItem = new TreeViewItem()
        {
            Header = fileData.Name,
            Tag = fileData
        };

        switch (fileData.Type)
        {
            case EFileType.Archive:
                ArchiveFile arc_FileData = ArchiveFile.Load(new ByteReader(fileData.GetData()!));
                foreach (ArchiveEntry arc_entry in arc_FileData.Entries)
                {
                    TreeViewItem arc_parent = fileItem;
                    string[] arc_splitName = arc_entry.Name.Split(Path.DirectorySeparatorChar);
                    for (int i = 0; i < arc_splitName.Length - 1; i++)
                    {
                        if (arc_parent.Items.Any(x => x is TreeViewItem xTvi && xTvi.Header is string xTviH && xTviH == arc_splitName[i]))
                            arc_parent = (TreeViewItem)arc_parent.Items.First(x => x is TreeViewItem xTvi && xTvi.Header is string xTviH && xTviH == arc_splitName[i])!;
                        else
                        {
                            FileData arc_newParentData = new FileData(arc_splitName[i], EFileType.Folder, null);
                            TreeViewItem arc_newParent = CreateItem(arc_newParentData);
                            arc_parent.Items.Add(arc_newParent);
                            arc_parent = arc_newParent;
                        }
                    }

                    FileData arc_entryData = new FileData(arc_entry);
                    TreeViewItem arc_entryItem = CreateItem(arc_entryData);
                    arc_parent.Items.Add(arc_entryItem);
                }
                break;
            case EFileType.Package:
                PackageFile pck_FileData = PackageFile.Load(new ByteReader(fileData.GetData()!));
                foreach (PackageEntry pck_entry in pck_FileData.Entries)
                {
                    TreeViewItem pck_parent = fileItem;
                    string[] pck_splitName = pck_entry.Name.Split(Path.DirectorySeparatorChar);
                    for (int i = 0; i < pck_splitName.Length - 1; i++)
                    {
                        if (pck_parent.Items.Any(x => x is TreeViewItem xTvi && xTvi.Header is string xTviH && xTviH == pck_splitName[i]))
                            pck_parent = (TreeViewItem)pck_parent.Items.First(x => x is TreeViewItem xTvi && xTvi.Header is string xTviH && xTviH == pck_splitName[i])!;
                        else
                        {
                            FileData pck_newParentData = new FileData(pck_splitName[i], EFileType.Folder, null);
                            TreeViewItem pck_newParent = CreateItem(pck_newParentData);
                            pck_parent.Items.Add(pck_newParent);
                            pck_parent = pck_newParent;
                        }
                    }

                    FileData pck_entryData = new FileData(pck_FileData, pck_entry);
                    TreeViewItem pck_entryItem = CreateItem(pck_entryData);
                    pck_parent.Items.Add(pck_entryItem);
                }
                break;
            case EFileType.SoundBank:
                SoundBankFile msscmp_FileData = SoundBankFile.Load(new ByteReader(fileData.GetData()!));
                foreach (SoundBankEntry msccmp_entry in msscmp_FileData.Entries)
                {
                    TreeViewItem msscmp_parent = fileItem;
                    string[] msscmp_splitName = msccmp_entry.FilePath.Split('/');
                    for (int i = 0; i < msscmp_splitName.Length - 1; i++)
                    {
                        if (msscmp_parent.Items.Any(x => x is TreeViewItem xTvi && xTvi.Header is string xTviH && xTviH == msscmp_splitName[i]))
                            msscmp_parent = (TreeViewItem)msscmp_parent.Items.First(x => x is TreeViewItem xTvi && xTvi.Header is string xTviH && xTviH == msscmp_splitName[i])!;
                        else
                        {
                            FileData msscmp_newParentData = new FileData(msscmp_splitName[i], EFileType.Folder, null);
                            TreeViewItem msscmp_newParent = CreateItem(msscmp_newParentData);
                            msscmp_parent.Items.Add(msscmp_newParent);
                            msscmp_parent = msscmp_newParent;
                        }
                    }

                    FileData msscmp_entryData = new FileData(msccmp_entry);
                    TreeViewItem msscmp_entryItem = CreateItem(msscmp_entryData);
                    msscmp_parent.Items.Add(msscmp_entryItem);
                }
                break;
            case EFileType.FourJUserInterface:
                FUIFile fui_FileData = FUIFile.Load(new ByteReader(fileData.GetData()!));
                foreach (FUIBitmap fui_Bitmap in fui_FileData.Bitmaps)
                {
                    FileData fui_EntryData = new FileData(fui_FileData, fui_Bitmap);
                    TreeViewItem fui_EntryItem = CreateItem(fui_EntryData);
                    fileItem.Items.Add(fui_EntryItem);
                }
                break;
            case EFileType.Folder:
                if (fileData.RealFilePath is not null && Directory.Exists(fileData.RealFilePath))
                {
                    foreach (string dir_dir in Directory.GetDirectories(fileData.RealFilePath))
                    {
                        FileData dir_dirData = new FileData(dir_dir);
                        TreeViewItem dir_dirItem = CreateItem(dir_dirData);
                        fileItem.Items.Add(dir_dirItem);
                    }
                    foreach (string dir_file in Directory.GetFiles(fileData.RealFilePath))
                    {
                        FileData dir_fileData = new FileData(dir_file);
                        TreeViewItem dir_fileItem = CreateItem(dir_fileData);
                        fileItem.Items.Add(dir_fileItem);
                    }
                }
                break;
            case EFileType.Audio:
                ContextMenu menu = new ContextMenu();
                MenuItem extractAsWav = new MenuItem();
                extractAsWav.Header = "Extract as .wav";
                extractAsWav.Tag = fileData;
                extractAsWav.Click += ExtractAudio;
                MenuItem extractAsBinka = new MenuItem();
                extractAsBinka.Header = "Extract as .binka";
                extractAsBinka.Tag = fileData;
                extractAsBinka.Click += ExtractAudio;
                menu.Items.Add(extractAsWav);
                menu.Items.Add(extractAsBinka);
                fileItem.ContextMenu = menu;
                break;
        }
        return fileItem;
    }

    private async void ExtractAudio(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (sender is not MenuItem menuItem)
            return;

        bool wav = ((string)menuItem.Header!).Contains(".wav");
        FileData fileData = ((FileData)menuItem.Tag!);

        // Get top level from the current control. Alternatively, you can use Window reference instead.
        var topLevel = TopLevel.GetTopLevel(this);

        // Start async operation to open the dialog.
        IStorageFile? file = await topLevel?.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save Audio",
            DefaultExtension = wav ? ".wav" : ".binka",
            SuggestedFileName = wav ? fileData.Name.Replace(".binka", ".wav") : fileData.Name,
            FileTypeChoices = [wav ? WAVFilePicker : BINKAFilePicker]
        })!;

        if (file is not null)
        {
            await fileData.ExtractAudioAsync(file.Path.LocalPath, wav);
        }
    }

    public void SortTreeViewItems(ItemCollection items)
    {
        List<object?> sortedItems = items
                                    .OrderBy(x => !(x is TreeViewItem xTvi && xTvi.Tag is FileData xTviFd && xTviFd.Type == EFileType.Folder)) // Folders first
                                    .ThenBy(x => x is TreeViewItem xTvi ? xTvi.Header.ToString() : string.Empty, StringComparer.Ordinal) // Alphabetical order
                                    .ToList();

        items.Clear();
        foreach (var item in sortedItems)
        {
            if (item is TreeViewItem xTvi && xTvi.Items.Count > 0)
                SortTreeViewItems(xTvi.Items);
            items.Add(item);
        }
    }

    public static FilePickerFileType WAVFilePicker { get; } = new("Waveform Audio File Format")
    {
        Patterns = ["*.wav"]
    };

    public static FilePickerFileType BINKAFilePicker { get; } = new("Bink Audio")
    {
        Patterns = ["*.binka"]
    };
}