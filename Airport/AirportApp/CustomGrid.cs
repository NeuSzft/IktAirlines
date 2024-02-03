using AirportAPI.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AirportApp;

internal class CustomGrid<T> : DataGrid where T : IdModel, IEquatable<T> {
    internal class ItemListChanges {
        public HashSet<T> AddedItems { get; } = new();
        public HashSet<T> UpdatedItems { get; } = new();
        public HashSet<T> RemovedItems { get; } = new();

        public int Count => AddedItems.Count + UpdatedItems.Count + RemovedItems.Count;

        public void Cleanup() {
            T[] added = new T[AddedItems.Count];
            AddedItems.CopyTo(added);

            AddedItems.RemoveWhere(RemovedItems.Contains);
            UpdatedItems.RemoveWhere(RemovedItems.Union(added).Contains);
            RemovedItems.RemoveWhere(added.Contains);
        }

        public void Clear() {
            AddedItems.Clear();
            UpdatedItems.Clear();
            RemovedItems.Clear();
        }
    }

    public ObservableCollection<T> ItemList { get; } = new();

    public T? LastEditedItemValue { get; private set; }

    public ItemListChanges Changes { get; } = new();

    public IEnumerable<int> ItemIds => ItemList.Select(x => x.Id);

    public int NextItemId { get; set; }

    public int RemoteNextItemId { get; set; }

    public new IEnumerable ItemsSource => base.ItemsSource;

    public CustomGrid() {
        ItemList.CollectionChanged += (_, e) => {
            if (e.NewItems is not null)
                foreach (T item in e.NewItems.OfType<T>())
                    Changes.AddedItems.Add(item);

            if (e.OldItems is not null)
                foreach (T item in e.OldItems.OfType<T>())
                    Changes.RemovedItems.Add(item);

            Changes.Cleanup();
        };

        AutoGenerateColumns = false;
        VerticalAlignment = VerticalAlignment.Top;
        CanUserDeleteRows = false;
        CanUserResizeRows = false;

        base.ItemsSource = ItemList;
    }

    public void MarkItemsAsDeleted(IEnumerable<T> items) {
        foreach (var item in items) {
            Changes.RemovedItems.Add(item);
            if (ItemContainerGenerator.ContainerFromItem(item) is DataGridRow row)
                row.Background = DimmedBrush(Brushes.Red);
        }

        Changes.Cleanup();
    }

    protected override void OnAddingNewItem(AddingNewItemEventArgs e) {
        if (NextItemId < RemoteNextItemId)
            NextItemId = RemoteNextItemId;

        T item = Activator.CreateInstance<T>();
        item.Id = NextItemId++;
        e.NewItem = item;

        base.OnAddingNewItem(e);
    }

    protected override void OnBeginningEdit(DataGridBeginningEditEventArgs e) {
        if (e.Row.DataContext is T item)
            LastEditedItemValue = (T)item.Clone();

        base.OnBeginningEdit(e);
    }

    protected override void OnExecutedCommitEdit(ExecutedRoutedEventArgs e) {
        base.OnExecutedCommitEdit(e);

        if (e.OriginalSource is DataGridCell cell && cell.DataContext is T item && !item.Equals(LastEditedItemValue)) {
            Changes.UpdatedItems.Add(item);
            Changes.Cleanup();

            if (!Changes.AddedItems.Contains(item) && !Changes.RemovedItems.Contains(item)) {
                DependencyObject parent = VisualTreeHelper.GetParent(cell);

                while (parent is not null && parent is not DataGridRow)
                    parent = VisualTreeHelper.GetParent(parent);

                if (parent is not null && parent is DataGridRow row)
                    row.Background = DimmedBrush(Brushes.Yellow);
            }
        }
    }

    protected override void OnLoadingRow(DataGridRowEventArgs e) {
        if (e.Row.DataContext is T item) {
            if (Changes.AddedItems.Contains(item))
                e.Row.Background = DimmedBrush(Brushes.Green);
            else if (Changes.UpdatedItems.Contains(item))
                e.Row.Background = DimmedBrush(Brushes.Yellow);
            else if (Changes.RemovedItems.Contains(item))
                e.Row.Background = DimmedBrush(Brushes.Red);
            else
                e.Row.Background = Brushes.White;
        }

        base.OnLoadingRow(e);
    }

    private SolidColorBrush DimmedBrush(SolidColorBrush brush) {
        Color color = brush.Color;
        color.A /= 2;
        return new(color);
    }
}
