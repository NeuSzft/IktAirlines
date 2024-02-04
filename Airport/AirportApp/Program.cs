using AirportAPI.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shell;

namespace AirportApp;

internal class Wnd : Window {
    public Wnd() {
        RequestHelper helper = new("http://localhost:5000");

        TableTab<Airline> airlinesTab = new(
            "Airlines",
            new() {
                Get = () => helper.Get<Airline>("/airlines"),
                Post = (airline) => helper.Post("/airlines", airline),
                Put = (id, airline) => helper.Put($"/airlines/{id}", airline),
                Delete = (id) => helper.Delete($"/airlines/{id}"),
                NextId = () => helper.NextId("/next-id/airlines"),
                Modify = helper.Modify
            },
            new DataGridTextColumn { Header = "Id", IsReadOnly = true, Binding = new Binding("Id") },
            new DataGridTextColumn { Header = "Name", Binding = new Binding("Name") }
        );

        TableTab<City> citiesTab = new(
            "Cities",
            new() {
                Get = () => helper.Get<City>("/cities"),
                Post = (city) => helper.Post("/cities", city),
                Put = (id, city) => helper.Put($"/cities/{id}", city),
                Delete = (id) => helper.Delete($"/cities/{id}"),
                NextId = () => helper.NextId("/next-id/cities"),
                Modify = helper.Modify
            },
            new DataGridTextColumn { Header = "Id", IsReadOnly = true, Binding = new Binding("Id") },
            new DataGridTextColumn { Header = "Name", Binding = new Binding("Name") },
            new DataGridTextColumn { Header = "Population", Binding = new Binding("Population") }
        );

        CustomGridComboBoxColumn<Airline> airlineIdColumn = new(airlinesTab.Grid.ItemList) { Header = "Airline Id", SelectedValueBinding = new Binding("AirlineId") };
        CustomGridComboBoxColumn<City> originIdColumn = new(citiesTab.Grid.ItemList) { Header = "Origin City Id", SelectedValueBinding = new Binding("OriginId") };
        CustomGridComboBoxColumn<City> destinationIdColumn = new(citiesTab.Grid.ItemList) { Header = "Destination City Id", SelectedValueBinding = new Binding("DestinationId") };

        TableTab<Flight> flightsTab = new(
            "Flights",
            new() {
                Get = () => helper.Get<Flight>("/flights"),
                Post = (flight) => helper.Post("/flights", flight),
                Put = (id, flight) => helper.Put($"/flights/{id}", flight),
                Delete = (id) => helper.Delete($"/flights/{id}"),
                NextId = () => helper.NextId("/next-id/flights"),
                Modify = helper.Modify
            },
            new DataGridTextColumn { Header = "Id", IsReadOnly = true, Binding = new Binding("Id") },
            airlineIdColumn,
            originIdColumn,
            destinationIdColumn,
            new DataGridTextColumn { Header = "Distance", Binding = new Binding("Distance") },
            new DataGridTextColumn { Header = "Flight Time (minutes)", Binding = new Binding("FlightTime") },
            new DataGridTextColumn { Header = "Ft/Km", Binding = new Binding("HufPerKm") }
        );

        airlinesTab.Grid.ItemList.CollectionChanged += (_, _) => airlineIdColumn.ItemsSource = airlinesTab.Grid.ItemIds;
        citiesTab.Grid.ItemList.CollectionChanged += (_, _) => {
            originIdColumn.ItemsSource = citiesTab.Grid.ItemIds;
            destinationIdColumn.ItemsSource = citiesTab.Grid.ItemIds;
        };

        TabControl content = new() { Margin = new(0, 8, 0, 0) };
        content.Items.Add(new HomeTab(helper, airlinesTab.FillDataGrids, citiesTab.FillDataGrids, flightsTab.FillDataGrids));
        content.Items.Add(airlinesTab);
        content.Items.Add(citiesTab);
        content.Items.Add(flightsTab);

        Background = Brushes.Transparent;
        MinWidth = 500;
        MinHeight = 300;
        Content = content;
        Title = "Airport Database Manager";

        Closed += (_, _) => helper.Dispose();

        foreach (IInputElement element in content.Items)
            WindowChrome.SetIsHitTestVisibleInChrome(element, true);

        WindowChrome.SetWindowChrome(this, new() {
            ResizeBorderThickness = new(4),
            GlassFrameThickness = new(0, 32, 0, 0),
            CaptionHeight = 32
        });
    }
}

internal static class Program {
    [STAThread]
    private static void Main() => new Application().Run(new Wnd());
}
