using AirportAPI.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace AirportApp;

internal sealed class CustomGridComboBoxColumn<T> : DataGridComboBoxColumn where T : IdModel {
    private class ValueConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return ((IEnumerable<T>)parameter).FirstOrDefault(x => x.Id.Equals(value))?.ToString() ?? value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null!;
    }

    public IEnumerable<T> ForeignItems;

    public CustomGridComboBoxColumn(IEnumerable<T> foreignItems) => ForeignItems = foreignItems;

    private void SetTemplate(ComboBox box) {
        FrameworkElementFactory factory = new(typeof(TextBlock));
        factory.SetBinding(TextBlock.TextProperty, new Binding() {
            Converter = new ValueConverter(),
            ConverterParameter = ForeignItems,
        });
        box.ItemTemplate = new DataTemplate { VisualTree = factory };
    }

    protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem) {
        FrameworkElement element = base.GenerateElement(cell, dataItem);
        if (element is ComboBox box)
            SetTemplate(box);
        return element;
    }

    protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem) {
        FrameworkElement element = base.GenerateEditingElement(cell, dataItem);
        if (element is ComboBox box)
            SetTemplate(box);
        return element;
    }
}

internal sealed class CustomGrid<T> : DataGrid where T : IdModel, IEquatable<T> {
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
        AutoGenerateColumns = false;
        CanUserDeleteRows = false;
        CanUserResizeRows = false;
        VerticalAlignment = VerticalAlignment.Top;

        base.ItemsSource = ItemList;
    }

    public void MarkItemsAsDeleted(IEnumerable<T> items) {
        foreach (var item in items) {
            Changes.RemovedItems.Add(item);
            if (ItemContainerGenerator.ContainerFromItem(item) is DataGridRow row)
                row.Background = Brushes.Red.AdjustAlpha(0.5);
        }

        Changes.Cleanup();
    }

    protected override void OnAddingNewItem(AddingNewItemEventArgs e) {
        if (NextItemId < RemoteNextItemId)
            NextItemId = RemoteNextItemId;

        T item = Activator.CreateInstance<T>();
        item.Id = NextItemId++;
        e.NewItem = item;

        Changes.AddedItems.Add(item);
        Changes.Cleanup();

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
                    row.Background = Brushes.Yellow.AdjustAlpha(0.5);
            }
        }
    }

    protected override void OnLoadingRow(DataGridRowEventArgs e) {
        if (e.Row.DataContext is T item) {
            if (Changes.AddedItems.Contains(item))
                e.Row.Background = Brushes.Green.AdjustAlpha(0.5);
            else if (Changes.UpdatedItems.Contains(item))
                e.Row.Background = Brushes.Yellow.AdjustAlpha(0.5);
            else if (Changes.RemovedItems.Contains(item))
                e.Row.Background = Brushes.Red.AdjustAlpha(0.5);
            else
                e.Row.Background = Brushes.White;
        }

        base.OnLoadingRow(e);
    }
}
