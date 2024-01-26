using AirportAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AirportApp;

internal sealed class TableTab<T> : TabItem where T : IdModel {
    private Grid _content = new();
    private Button _deleteButton = new() { Width = 28, Margin = new(2), Content = new Image { Source = GetIcon(131) }, IsEnabled = false };
    private Button _undoAllButton = new() { Width = 28, Margin = new(2), Content = new Image { Source = GetIcon(297) }, ToolTip = "Undo All Changes" };
    private Button _updateButton = new() { Width = 28, Margin = new(2), Content = new Image { Source = GetIcon(299) }, ToolTip = "Upload Changes" };
    private Button _fetchButton = new() { Width = 28, Margin = new(2), Content = new Image { Source = GetIcon(238) }, ToolTip = "Fetch" };
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

        GroupBox _groupLocal = new() { Header = "Local" };
        _groupLocal.Content = LocalData;

        GroupBox _groupRemote = new() { Header = "Remote", Margin = new(8, 0, 0, 0) };
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

        Header = header;
        Content = _content;

        CreateDataGridColumns(columns);
        Loaded += (_, _) => _ = FillDataGrids();
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

    private async Task FetchRemoteItems() {
        _fetchResult.Text = $"Fetching...";

        if (NextId is not null)
            LocalData.RemoteNextItemId = await NextId() ?? 0;

        if (Get is not null) {
            IEnumerable<T>? data = await Get();
            if (data is not null) {
                _fetchResult.Text = $"Last fetched at {DateTime.Now.ToLongTimeString()}";
                RemoteData.ItemsSource = data;
                return;
            }

        }
        _fetchResult.Text = $"Failed to fetch at {DateTime.Now.ToLongTimeString()}";
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

    private async Task FillDataGrids() {
        await FetchRemoteItems();
        ResetItems();
    }

    private static ImageSource GetIcon(int index, bool forceSmall = false) {
        string resourcePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "SystemResources", "shell32.dll.mun");
        IntPtr small, large;
        WinApi.ExtractIconEx(resourcePath, index, out large, out small, 1);
        return Imaging.CreateBitmapSourceFromHIcon(large == IntPtr.Zero || forceSmall ? small : large, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
    }
}
