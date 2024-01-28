using AirportAPI.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace AirportApp;

internal class CustomGrid<T> : DataGrid where T : IdModel, IEquatable<T> {
    internal class ItemListChanges {
        public HashSet<T> AddedItems { get; } = new();
        public HashSet<T> UpdatedItems { get; } = new();
        public HashSet<T> RemovedItems { get; } = new();

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
        };

        base.ItemsSource = ItemList;
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
            LastEditedItemValue = item.Clone() as T;
        base.OnBeginningEdit(e);
    }

    protected override void OnExecutedCommitEdit(ExecutedRoutedEventArgs e) {
        base.OnExecutedCommitEdit(e);
        if (e.OriginalSource is DataGridCell cell && cell.DataContext is T item && !item.Equals(LastEditedItemValue))
            Changes.UpdatedItems.Add(item);
    }
}
