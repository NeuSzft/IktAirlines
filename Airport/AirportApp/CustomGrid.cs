using AirportAPI.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

namespace AirportApp;

internal class CustomGrid<T> : DataGrid where T : IdModel {
    public ObservableCollection<T> ItemList { get; } = new();

    public IEnumerable<int> ItemIds => ItemList.Select(x => x.Id);

    public int NextItemId { get; set; }

    public int RemoteNextItemId { get; set; }

    public new IEnumerable ItemsSource => base.ItemsSource;

    public CustomGrid() => base.ItemsSource = ItemList;

    protected override void OnAddingNewItem(AddingNewItemEventArgs e) {
        if (NextItemId < RemoteNextItemId)
            NextItemId = RemoteNextItemId;

        T item = Activator.CreateInstance<T>();
        item.Id = NextItemId++;
        e.NewItem = item;

        base.OnAddingNewItem(e);
    }
}
