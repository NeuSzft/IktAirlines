using AirportAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AirportApp;

internal sealed class Functions<T> {
    public Func<Task<IEnumerable<T>?>>? Get { get; set; }
    public Func<T, Task>? Post { get; set; }
    public Func<int, T, Task>? Put { get; set; }
    public Func<int, Task>? Delete { get; set; }
    public Func<Task<int>>? NextId { get; set; }
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
    private UpdateWindow _updateWindow = null!;
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
        Content = _content;
        Template = new() {
            VisualTree = Utilities.CreateCustomTabItemFactory(header, (sender, _) => Header = sender)
        };

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
        Grid.ItemList.Clear();
        Grid.Changes.Clear();
        Grid.NextItemId = Grid.RemoteNextItemId;

        foreach (T item in FetchedData)
            Grid.ItemList.Add((T)item.Clone());
        SetItemCount();
    }

    private void ShowChanges() {
        UpdateInfo updateInfo = new(
            Grid.Changes.AddedItems.Select(item => item.ToString()).ToList()!,
            Grid.Changes.UpdatedItems.Select(item => item.ToString()).ToList()!,
            Grid.Changes.RemovedItems.Select(item => item.ToString()).ToList()!
        );

        if (_updateWindow != null && _updateWindow.IsLoaded) _updateWindow.Close();

        _updateWindow = new(updateInfo) { Owner = Window.GetWindow(this) };
        _updateWindow.Show();
    }

    private async Task UpdateRemoteItems() {
        _updateResult.Text = "Updating...";

        int successCount = 0;
        List<Exception> exceptions = new();
        List<T> failedAddedItems = new();
        List<T> failedUpdatedItems = new();
        List<T> failedRemovedItems = new();

        if (Functions.Post is not null)
            foreach (T item in Grid.Changes.AddedItems)
                try {
                    await Functions.Post(item);
                    successCount++;
                } catch (Exception e) {
                    failedAddedItems.Add(item);
                    exceptions.Add(e);
                }

        if (Functions.Put is not null)
            foreach (T item in Grid.Changes.UpdatedItems)
                try {
                    await Functions.Put(item.Id, item);
                    successCount++;
                } catch (Exception e) {
                    failedUpdatedItems.Add(item);
                    exceptions.Add(e);
                }

        if (Functions.Delete is not null)
            foreach (T item in Grid.Changes.RemovedItems)
                try {
                    await Functions.Delete(item.Id);
                    successCount++;
                } catch (Exception e) {
                    failedRemovedItems.Add(item);
                    exceptions.Add(e);
                }

        _updateResult.Text = $"{successCount}/{Grid.Changes.Count} operations successfully performed";

        await FillDataGrids();

        foreach (T failed in failedAddedItems) {
            Grid.ItemList.Add(failed);
            Grid.Changes.AddedItems.Add(failed);
        }

        foreach (T failed in failedUpdatedItems) {
            foreach (T item in Grid.ItemList.Where(x => x.Id == failed.Id)) {
                failed.CopyTo(item);
                Grid.Changes.UpdatedItems.Add(item);
            }
        }

        foreach (T failed in failedRemovedItems) {
            foreach (T item in Grid.ItemList.Where(x => x.Id == failed.Id))
                Grid.Changes.RemovedItems.Add(item);
        }

        Grid.InvalidateVisual();

        if (exceptions.Count > 0)
            new ErrorsWindow(exceptions).ShowDialog();
    }

    private async Task<bool> FetchRemoteItems() {
        _fetchResult.Text = "Fetching...";

        if (Functions.NextId is not null)
            try {
                Grid.RemoteNextItemId = await Functions.NextId();
            } catch (Exception e) {
                Grid.RemoteNextItemId = 0;
                Utilities.ShowErrorMessageBox(e);
            }

        if (Functions.Get is not null) {
            IEnumerable<T>? data = null;
            try {
                data = await Functions.Get();
            } catch (Exception e) {
                Utilities.ShowErrorMessageBox(e);
            }

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
