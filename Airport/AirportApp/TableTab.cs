using AirportAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AirportApp;

internal sealed class Functions<T> {
    public Func<Task<IEnumerable<T>?>>? Get { get; set; }
    public Func<T, Task<bool>>? Post { get; set; }
    public Func<int, T, Task<bool>>? Put { get; set; }
    public Func<int, Task<bool>>? Delete { get; set; }
    public Func<Task<int?>>? NextId { get; set; }
    public Func<IEnumerable<OperationInfo>, Task<string>>? Modify { get; set; }
}

internal sealed class TableTab<T> : TabItem where T : IdModel, IEquatable<T> {
    private DockPanel _content = new();

    private GlyphButton _deleteButton = new(0xE74D, Brushes.Red, 28) { Margin = new(2), IsEnabled = false };
    private GlyphButton _undoAllButton = new(0xE7A7, Brushes.RoyalBlue, 28) { Margin = new(2), ToolTip = "Undo All Changes" };
    private GlyphButton _showChangesButton = new(0xE94D, Brushes.Goldenrod, 28) { Margin = new(2), ToolTip = "Show Changes" };
    private GlyphButton _updateButton = new(0xE898, Brushes.Green, 28) { Margin = new(2), ToolTip = "Upload Changes" };
    private TextBlock _updateResult = new() { Margin = new(2), VerticalAlignment = VerticalAlignment.Center };

    private TextBlock _fetchResult = new() { Margin = new(2), TextAlignment = TextAlignment.Right, VerticalAlignment = VerticalAlignment.Center };
    private GlyphButton _fetchButton = new(0xE72C, Brushes.RoyalBlue, 28) { Margin = new(2), ToolTip = "Fetch" };

    private GroupBox GridBox = new();

    public CustomGrid<T> Grid { get; } = new() { MinRowHeight = 22, AutoGenerateColumns = false, CanUserResizeRows = false };

    public Functions<T> Functions { get; } = new();
    public List<T> FetchedData { get; private set; } = new();

    public TableTab(string header, Functions<T> functions, params DataGridColumn[] columns) {
        DockPanel topPanel = new() { HorizontalAlignment = HorizontalAlignment.Stretch };
        topPanel.Children.Add(_deleteButton);
        topPanel.Children.Add(_undoAllButton);
        topPanel.Children.Add(_showChangesButton);
        topPanel.Children.Add(_updateButton);
        topPanel.Children.Add(_updateResult);

        DockPanel.SetDock(_fetchResult, Dock.Right);
        DockPanel.SetDock(_fetchButton, Dock.Right);
        topPanel.Children.Add(_fetchButton);
        topPanel.Children.Add(_fetchResult);

        GridBox.Content = Grid;

        DockPanel.SetDock(topPanel, Dock.Top);
        DockPanel.SetDock(GridBox, Dock.Bottom);
        _content.Children.Add(topPanel);
        _content.Children.Add(GridBox);

        _deleteButton.Click += (_, _) => DeleteItems();
        _undoAllButton.Click += (_, _) => ResetItems();
        _showChangesButton.Click += (_, _) => ShowChanges();
        _updateButton.Click += (_, _) => _ = UpdateRemoteItems();
        _fetchButton.Click += (_, _) => _ = FetchRemoteItems();
        Grid.SelectedCellsChanged += (_, _) => SelectionChanged();
        Grid.ItemList.CollectionChanged += (_, _) => SetItemCount();

        Functions = functions;
        Header = new TextBlock { Text = header, Width = 64, Height = 16, TextAlignment = TextAlignment.Center };
        Content = _content;

        CreateDataGridColumns(columns);
    }

    public async Task<bool> FillDataGrids() {
        bool success = await FetchRemoteItems();
        ResetItems();
        return success;
    }

    private void DeleteItems() {
        Grid.MarkItemsAsDeleted(Grid.SelectedItems.OfType<T>());
        Grid.NextItemId = Math.Max(Grid.ItemList.Max(x => x.Id) + 1, Grid.RemoteNextItemId);
        SetItemCount();
    }

    private void ResetItems() {
        Grid.NextItemId = Grid.RemoteNextItemId;
        Grid.ItemList.Clear();
        foreach (T item in FetchedData)
            Grid.ItemList.Add((T)item.Clone());
        Grid.Changes.Clear();
        SetItemCount();
    }

    private void ShowChanges() {
        StringBuilder sb = new();
        sb.AppendLine("New Items:");
        foreach (T item in Grid.Changes.AddedItems)
            sb.AppendLine($"  {item}");

        sb.AppendLine();
        sb.AppendLine("Updated Items:");
        foreach (T item in Grid.Changes.UpdatedItems)
            sb.AppendLine($"  {item}");

        sb.AppendLine();
        sb.AppendLine("Removed Items:");
        foreach (T item in Grid.Changes.RemovedItems)
            sb.AppendLine($"  {item}");

        MessageBox.Show(sb.ToString(), $"{(Header as TextBlock)?.Text} Changes");
    }

    private async Task UpdateRemoteItems() {
        _updateResult.Text = "Updating...";

        IEnumerable<Operation> added = Grid.Changes.AddedItems.Select(x => new AddOperation<T>(x));
        IEnumerable<Operation> updated = Grid.Changes.UpdatedItems.Select(x => new UpdateOperation<T>(x.Id, x));
        IEnumerable<Operation> removed = Grid.Changes.RemovedItems.Select(x => new RemoveOperation<T>(x.Id));
        IEnumerable<Operation> operations = added.Concat(updated).Concat(removed);

        if (Functions.Modify is null)
            return;
        string result = await Functions.Modify(operations.Select(x => x.GetInfo()));
        result = Regex.Unescape(result);

        _updateResult.Text = result.Split('\n', '\r').FirstOrDefault() ?? result;
        await FillDataGrids();

        MessageBox.Show(result);
    }

    private async Task<bool> FetchRemoteItems() {
        _fetchResult.Text = "Fetching...";

        if (Functions.NextId is not null)
            Grid.RemoteNextItemId = await Functions.NextId() ?? 0;

        if (Functions.Get is not null) {
            IEnumerable<T>? data = await Functions.Get();
            if (data is not null) {
                _fetchResult.Text = $"Last fetched at {DateTime.Now.ToLongTimeString()}";
                FetchedData = new(data);
                return true;
            }
        }

        _fetchResult.Text = $"Failed to fetch at {DateTime.Now.ToLongTimeString()}";
        return false;
    }

    private void SelectionChanged() {
        IEnumerable<T> items = Grid.SelectedItems.OfType<T>();
        _deleteButton.IsEnabled = items.Count() > 0;
        _deleteButton.ToolTip = $"Delete\n{string.Join("\n", items.Select(x => $"- {x}"))}";
    }

    private void SetItemCount() {
        int added = Grid.Changes.AddedItems.Count;
        int removed = Grid.Changes.RemovedItems.Count;
        int count = Grid.ItemList.Count - removed;
        GridBox.Header = $"{count} items ({added} added {removed} removed)";
    }

    private void CreateDataGridColumns(IEnumerable<DataGridColumn> columns) {
        foreach (DataGridColumn column in columns)
            Grid.Columns.Add(column);
    }
}
