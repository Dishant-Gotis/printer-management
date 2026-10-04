using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using UniversalPrint.Core.Models;
using UniversalPrint.Core.Services;

namespace UniversalPrint.App;

public partial class MainWindow : Window
{
    private readonly ObservableCollection<PrintFile> _files = [];

    public MainWindow()
    {
        InitializeComponent();
        FilesList.ItemsSource = _files;
    }

    private void BrowseFiles_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Multiselect = true,
            Filter = "Supported files|*.jpg;*.jpeg;*.png;*.pdf;*.md|All files|*.*"
        };

        if (dialog.ShowDialog(this) == true)
        {
            AddFiles(dialog.FileNames);
        }
    }

    private void Window_DragOver(object sender, DragEventArgs e)
    {
        e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop)
            ? DragDropEffects.Copy
            : DragDropEffects.None;
        e.Handled = true;
    }

    private void Window_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetData(DataFormats.FileDrop) is string[] paths)
        {
            AddFiles(paths);
        }
    }

    private void AddFiles(IEnumerable<string> paths)
    {
        var added = 0;
        var rejected = 0;

        foreach (var path in paths)
        {
            if (!File.Exists(path) || !SupportedFileTypes.IsSupported(path))
            {
                rejected++;
                continue;
            }

            if (_files.Any(file => string.Equals(file.Path, path, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            var info = new FileInfo(path);
            _files.Add(new PrintFile(
                info.FullName,
                info.Name,
                info.Extension,
                info.Length));
            added++;
        }

        StatusText.Text = rejected == 0
            ? $"{_files.Count} file(s) ready. Added {added}."
            : $"{_files.Count} file(s) ready. Added {added}; rejected {rejected} unsupported or missing file(s).";
    }

    private void MoveUp_Click(object sender, RoutedEventArgs e)
    {
        MoveSelected(-1);
    }

    private void MoveDown_Click(object sender, RoutedEventArgs e)
    {
        MoveSelected(1);
    }

    private void MoveSelected(int direction)
    {
        var index = FilesList.SelectedIndex;
        var target = index + direction;

        if (index < 0 || target < 0 || target >= _files.Count)
        {
            return;
        }

        _files.Move(index, target);
        FilesList.SelectedIndex = target;
    }

    private void Remove_Click(object sender, RoutedEventArgs e)
    {
        if (FilesList.SelectedItem is PrintFile file)
        {
            _files.Remove(file);
            StatusText.Text = $"{_files.Count} file(s) ready.";
        }
    }

    private void Clear_Click(object sender, RoutedEventArgs e)
    {
        _files.Clear();
        StatusText.Text = "No files selected.";
    }

    private void Preview_Click(object sender, RoutedEventArgs e)
    {
        StatusText.Text = _files.Count == 0
            ? "Add at least one file before previewing."
            : "Preview pipeline will be connected in the document-processing phase.";
    }

    private void PrintAll_Click(object sender, RoutedEventArgs e)
    {
        StatusText.Text = _files.Count == 0
            ? "Add at least one file before printing."
            : "Print pipeline will be connected after document conversion and merging are implemented.";
    }
}