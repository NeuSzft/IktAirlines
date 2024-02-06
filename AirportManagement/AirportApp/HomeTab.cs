using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AirportApp;

internal sealed class HomeTab : TabItem {
    Border _addressBorder = new() { Margin = new(16, 16, 16, 0), BorderThickness = new(1), CornerRadius = new(2) };
    TextBox _addressBox = new() { FontSize = 15, BorderThickness = new(0), Background = Brushes.Transparent };
    CustomButton _okButton = new("Set", 15) { Margin = new(16) };
    TextBlock _resultText = new() { Margin = new(16, 0, 16, 16), Text = "Not set yet" };

    public HomeTab(RequestHelper helper, params Func<Task<bool>>[] tableFetchers) {
        _addressBox.Text = helper.BaseAddress.ToString();
        _addressBorder.Child = _addressBox;

        _okButton.Click += (_, _) => _ = SetBaseAddressAndFetch(helper, tableFetchers);

        StackPanel panel = new() {
            MinWidth = 256,
            Margin = new(2)
        };
        panel.Children.Add(_addressBorder);
        panel.Children.Add(_okButton);
        panel.Children.Add(_resultText);

        Border border = new() {
            CornerRadius = new(8),
            Background = Brushes.White,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Child = panel
        };

        DockPanel content = new() {
            Background = new LinearGradientBrush(Color.FromRgb(240, 240, 250), Color.FromRgb(200, 200, 240), 90)
        };
        content.Children.Add(border);

        Content = content;
        Template = new() {
            VisualTree = Utilities.CreateCustomTabItemFactory("Home", (sender, _) => {
                Border border = (Border)sender;
                Header = border;
                border.Background = Brushes.LightSkyBlue;
                border.BorderBrush = Brushes.LightSkyBlue;
            })
        };

        SetAddressColor(Brushes.LightGray);
    }

    private void SetAddressColor(SolidColorBrush brush) {
        _addressBorder.BorderBrush = brush;
        _addressBorder.Background = brush.AdjustAlpha(0.3);
    }

    private async Task SetBaseAddressAndFetch(RequestHelper helper, Func<Task<bool>>[] tableFetchers) {
        helper.SetBaseAddress(_addressBox.Text);

        SetAddressColor(Brushes.Gold);
        _resultText.Text = "Pinging...";

        if (!await helper.Ping()) {
            SetAddressColor(Brushes.Red);
            _resultText.Text = $"Failed to reach API server";
            return;
        }

        _resultText.Text = "Fetching...";

        int success = 0;
        foreach (Func<Task<bool>> task in tableFetchers) {
            bool res = await task();
            if (res)
                success++;
        }

        SetAddressColor(success == tableFetchers.Length ? Brushes.LightGreen : Brushes.Red);
        _resultText.Text = $"Successfully set and queried ({success}/{tableFetchers.Length}) tables";
    }
}
