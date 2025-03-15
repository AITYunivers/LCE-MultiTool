using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using AvaloniaEdit.Indentation.CSharp;
using AvaloniaEdit.TextMate;
using LCE_MultiTool.Data;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using TextMateSharp.Grammars;

namespace LCE_MultiTool;

public partial class TextDisplay : UserControl
{
    private readonly TextMate.Installation _textMateInstallation;
    private RegistryOptions? _registryOptions;

    public TextDisplay() : this(null) { }
    public TextDisplay(FileData fileData)
    {
        InitializeComponent();

        Editor.Background = Brushes.Transparent;
        Editor.ShowLineNumbers = true;
        Editor.TextArea.Background = Background;
        Editor.Options.AllowToggleOverstrikeMode = true;
        Editor.Options.ShowBoxForControlCharacters = true;
        Editor.Options.ColumnRulerPositions = new List<int>() { 80, 100 };
        Editor.TextArea.IndentationStrategy = new CSharpIndentationStrategy(Editor.Options);
        Editor.TextArea.RightClickMovesCaret = true;
        Editor.Options.HighlightCurrentLine = true;

        _registryOptions = new RegistryOptions(ThemeName.DarkPlus);

        if (fileData is not null)
        {
            _textMateInstallation = Editor.InstallTextMate(_registryOptions);
            _textMateInstallation.AppliedTheme += TextMateInstallationOnAppliedTheme;
            Language? lang = _registryOptions.GetLanguageByExtension(Path.GetExtension(fileData.Name));
            if (lang != null)
            {
                string langId = lang.Id;
                string langScope = _registryOptions.GetScopeByLanguageId(langId);
                _textMateInstallation.SetGrammar(langScope);
            }

            if (fileData.Data is null && fileData.RealFilePath is not null)
                fileData.Data = new FileStream(fileData.RealFilePath, FileMode.Open);
            else if (fileData.Data is null)
                return;

            StreamReader reader = new StreamReader(fileData.GetData()!, leaveOpen: true);
            Editor.Text = reader.ReadToEnd();
            reader.Dispose();
        }

        AddHandler(PointerWheelChangedEvent, (o, i) =>
        {
            if (i.KeyModifiers != KeyModifiers.Control) return;
            if (i.Delta.Y > 0) Editor.FontSize++;
            else Editor.FontSize = Editor.FontSize > 1 ? Editor.FontSize - 1 : 1;
        }, RoutingStrategies.Bubble, true);
    }

    private void TextMateInstallationOnAppliedTheme(object? sender, TextMate.Installation e)
    {
        ApplyThemeColorsToEditor(e);
    }

    void ApplyThemeColorsToEditor(TextMate.Installation e)
    {
        ApplyBrushAction(e, "editor.background", brush => Editor.Background = brush);
        ApplyBrushAction(e, "editor.foreground", brush => Editor.Foreground = brush);

        if (!ApplyBrushAction(e, "editor.selectionBackground",
                brush => Editor.TextArea.SelectionBrush = brush))
        {
            if (Application.Current!.TryGetResource("TextAreaSelectionBrush", out var resourceObject))
            {
                if (resourceObject is IBrush brush)
                {
                    Editor.TextArea.SelectionBrush = brush;
                }
            }
        }

        if (!ApplyBrushAction(e, "editor.lineHighlightBackground",
                brush =>
                {
                    Editor.TextArea.TextView.CurrentLineBackground = brush;
                    Editor.TextArea.TextView.CurrentLineBorder = new Pen(brush);
                }))
        {
            Editor.TextArea.TextView.SetDefaultHighlightLineColors();
        }

        if (!ApplyBrushAction(e, "editorLineNumber.foreground",
                brush => Editor.LineNumbersForeground = brush))
        {
            Editor.LineNumbersForeground = Editor.Foreground;
        }
    }

    static bool ApplyBrushAction(TextMate.Installation e, string colorKeyNameFromJson, Action<IBrush> applyColorAction)
    {
        if (!e.TryGetThemeColor(colorKeyNameFromJson, out var colorString))
            return false;

        if (!Color.TryParse(colorString, out Color color))
            return false;

        var colorBrush = new SolidColorBrush(color);
        applyColorAction(colorBrush);
        return true;
    }
}