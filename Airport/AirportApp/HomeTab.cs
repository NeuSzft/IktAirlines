using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AirportApp;

internal sealed class HomeTab : TabItem {
    TextBox _addressBox = new() { Margin = new(16, 16, 16, 0), FontSize = 15, BorderThickness = new(2) };
    Button _okButton = new() { Margin = new(16), Content = new TextBlock { Text = "Set", FontSize = 15 } };
    TextBlock _resultText = new() { Margin = new(16, 0, 16, 16), Text = "Not set yet" };

    public HomeTab(RequestHelper helper, params Func<Task<bool>>[] tableFetchers) {
        _addressBox.Text = helper.BaseAddress.ToString();

        _okButton.Click += (_, _) => _ = SetBaseAddressAndFetch(helper, tableFetchers);

        StackPanel panel = new() {
            MinWidth = 256,
            Margin = new(2)
        };
        panel.Children.Add(_addressBox);
        panel.Children.Add(_okButton);
        panel.Children.Add(_resultText);

        LinearGradientBrush brush = new(Color.FromRgb(220, 220, 225), Color.FromRgb(200, 200, 225), 90);

        Border border = new() {
            CornerRadius = new(8),
            Background = Brushes.White,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Child = panel
        };

        DockPanel content = new() {
            Background = brush
        };
        content.Children.Add(border);

        Margin = new(4, 0, 0, 0);
        Header = new TextBlock { Text = "Home", Width = 64, Height = 16, TextAlignment = TextAlignment.Center };
        Content = content;
    }

    private async Task SetBaseAddressAndFetch(RequestHelper helper, Func<Task<bool>>[] tableFetchers) {
        helper.SetBaseAddress(_addressBox.Text);

        _addressBox.BorderBrush = Brushes.Gold;
        _resultText.Text = "Pinging...";

        if (!await helper.Ping()) {
            _addressBox.BorderBrush = Brushes.Red;
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

        _addressBox.BorderBrush = success == tableFetchers.Length ? Brushes.Green : Brushes.Red;
        _resultText.Text = $"Successfully set and queried ({success}/{tableFetchers.Length}) tables";
    }
}
