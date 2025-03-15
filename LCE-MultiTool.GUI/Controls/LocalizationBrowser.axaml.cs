using Avalonia.Controls;
using LCE_MultiTool.GUI.Data.Localization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LCE_MultiTool.GUI;

public partial class LocalizationBrowser : UserControl
{
    public ObservableCollection<LocalizationEntry> Entries { get; set; } = [];
    public LocalizationFile? LocFile;

    public LocalizationBrowser() : this(null) {}
    public LocalizationBrowser(LocalizationFile? locFile)
    {
        LocFile = locFile;
        InitializeComponent();
        DataContext = this;

        if (LocFile is not null)
        {
            Languages.ItemsSource = LocFile.Languages;
            if (LocFile.Languages.Contains("en-EN"))
                Languages.SelectedIndex = LocFile.Languages.IndexOf("en-EN");
        }
    }

    private void ComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox comboBox)
        {
            if (comboBox.SelectedItem is string language)
            {
                Entries.Clear();
                foreach (KeyValuePair<string, string> entry in LocFile!.LanguageKeyValues[language])
                {
                    Entries.Add(new LocalizationEntry()
                    {
                        ID = entry.Key,
                        Text = entry.Value
                    });
                }
                extGrid.ItemsSource = Entries;
            }
        }
    }

    private void TextBox_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox)
            extGrid.ItemsSource = Entries.Where(x => x.Search(textBox.Text));
    }
}

public class LocalizationEntry
{
    public string ID { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;

    public bool Search(string? searchTerm)
    {
        if (string.IsNullOrEmpty(searchTerm))
            return true;
        return $"{ID} {Text}".Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
    }
}