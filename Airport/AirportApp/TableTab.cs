using AirportAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace AirportApp;

internal sealed class TableTab<T> : TabItem where T : IdModel {
    private Grid _content = new();
    private GlyphButton _deleteButton = new(0xE74D, Brushes.Red, 28) { Margin = new(2), IsEnabled = false };
    private GlyphButton _undoAllButton = new(0xE7A7, Brushes.RoyalBlue, 28) { Margin = new(2), ToolTip = "Undo All Changes" };
    private GlyphButton _updateButton = new(0xE898, Brushes.Green, 28) { Margin = new(2), ToolTip = "Upload Changes" };
    private GlyphButton _fetchButton = new(0xE72C, Brushes.RoyalBlue, 28) { Margin = new(2), ToolTip = "Fetch" };
    private TextBlock _fetchResult = new() { Margin = new(2), TextAlignment = TextAlignment.Right, VerticalAlignment = VerticalAlignment.Center };

    public CustomGrid<T> LocalData { get; } = new() { MinRowHeight = 22, AutoGenerateColumns = false, CanUserResizeRows = false };
    public DataGrid RemoteData { get; } = new() { MinRowHeight = 22, AutoGenerateColumns = false, CanUserResizeRows = false, CanUserAddRows = false };

    public Func<Task<IEnumerable<T>?>>? Get { get; set; }
    public Func<T, Task<bool>>? Post { get; set; }
    public Func<int, T, Task<bool>>? Put { get; set; }
    public Func<int, Task<bool>>? Delete { get; set; }
    public Func<Task<int?>>? NextId { get; set; }

    public TableTab(string header, params DataGridColumn[] columns) {
        DockPanel localPanel = new() { HorizontalAlignment = HorizontalAlignment.Left };
        localPanel.Children.Add(_deleteButton);
        localPanel.Children.Add(_undoAllButton);
        localPanel.Children.Add(_updateButton);

        DockPanel remotePanel = new() { HorizontalAlignment = HorizontalAlignment.Right };
        remotePanel.Children.Add(_fetchResult);
        remotePanel.Children.Add(_fetchButton);

        GroupBox _groupLocal = new() { Header = "Local", Margin = new(0, 0, 2, 0) };
        _groupLocal.Content = LocalData;

        GroupBox _groupRemote = new() { Header = "Remote", Margin = new(10, 0, 0, 0) };
        _groupRemote.Content = RemoteData;

        GridSplitter splitter = new() { Width = 8, Margin = new(0, 9, 0, 0), HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Stretch };

        Grid.SetColumn(remotePanel, 1);
        Grid.SetColumn(_groupRemote, 1);
        Grid.SetColumn(splitter, 1);
        Grid.SetRow(_groupLocal, 1);
        Grid.SetRow(_groupRemote, 1);
        Grid.SetRow(splitter, 1);

        _content.Children.Add(localPanel);
        _content.Children.Add(remotePanel);
        _content.Children.Add(_groupLocal);
        _content.Children.Add(_groupRemote);
        _content.Children.Add(splitter);

        _content.ColumnDefinitions.Add(new());
        _content.ColumnDefinitions.Add(new());
        _content.RowDefinitions.Add(new() { Height = new(32) });
        _content.RowDefinitions.Add(new());

        _deleteButton.Click += (_, _) => DeleteItems();
        _undoAllButton.Click += (_, _) => ResetItems();
        _updateButton.Click += (_, _) => _ = UpdateRemoteItems();
        _fetchButton.Click += (_, _) => _ = FetchRemoteItems();
        LocalData.SelectedCellsChanged += (_, _) => SelectionChanged();

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
        foreach (T item in LocalData.SelectedItems.OfType<T>().ToArray())
            LocalData.ItemList.Remove(item);
        LocalData.NextItemId = Math.Max(LocalData.ItemList.Max(x => x.Id) + 1, LocalData.RemoteNextItemId);
    }

    private void ResetItems() {
        LocalData.ItemList.Clear();
        foreach (T item in RemoteData.ItemsSource.Cast<T>())
            LocalData.ItemList.Add((T)item.Clone());
    }

    private async Task UpdateRemoteItems() {

    }

    private async Task<bool> FetchRemoteItems() {
        _fetchResult.Text = $"Fetching...";

        if (NextId is not null)
            LocalData.RemoteNextItemId = await NextId() ?? 0;

        if (Get is not null) {
            IEnumerable<T>? data = await Get();
            if (data is not null) {
                _fetchResult.Text = $"Last fetched at {DateTime.Now.ToLongTimeString()}";
                RemoteData.ItemsSource = data;
                return true;
            }

        }

        _fetchResult.Text = $"Failed to fetch at {DateTime.Now.ToLongTimeString()}";
        return false;
    }

    private void SelectionChanged() {
        IEnumerable<T> items = LocalData.SelectedItems.OfType<T>();
        _deleteButton.IsEnabled = items.Count() > 0;
        _deleteButton.ToolTip = $"Delete\n{string.Join("\n", items.Select(x => $"- {x}"))}";
    }

    private void CreateDataGridColumns(IEnumerable<DataGridColumn> columns) {
        foreach (DataGridColumn column in columns) {
            DataGridTextColumn remoteCol = new() { Header = column.Header, IsReadOnly = true };

            if (column is DataGridBoundColumn bc && bc.Binding is Binding bcb)
                remoteCol.Binding = new Binding(bcb.Path.Path);
            else if (column is DataGridComboBoxColumn cc && cc.SelectedValueBinding is Binding ccb)
                remoteCol.Binding = new Binding(ccb.Path.Path);

            LocalData.Columns.Add(column);
            RemoteData.Columns.Add(remoteCol);
        }
    }
}
