using AirportAPI.Models;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AirportApp;

internal static class Utilities {
    public static FrameworkElementFactory CreateCustomTabItemFactory(string header, RoutedEventHandler? onLoaded = null) {
        FrameworkElementFactory textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
        textBlockFactory.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center);
        textBlockFactory.SetValue(TextBlock.TextAlignmentProperty, TextAlignment.Center);
        textBlockFactory.SetValue(TextBlock.TextProperty, header);

        FrameworkElementFactory borderFactory = new FrameworkElementFactory(typeof(Border));
        borderFactory.SetValue(FrameworkElement.WidthProperty, 128.0);
        borderFactory.SetValue(FrameworkElement.HeightProperty, 24.0);
        borderFactory.SetValue(Panel.BackgroundProperty, Brushes.White);
        borderFactory.SetValue(Border.BorderBrushProperty, Brushes.LightGray);
        borderFactory.SetValue(Border.BorderThicknessProperty, new Thickness(1, 1, 1, 0));
        borderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(4, 4, 0, 0));

        borderFactory.AppendChild(textBlockFactory);

        borderFactory.AddHandler(UIElement.MouseEnterEvent, new MouseEventHandler((sender, _) => {
            Border border = (Border)sender;
            if (border.Background != Brushes.LightSkyBlue) {
                border.Background = Brushes.LightSkyBlue.AdjustAlpha(0.3);
                border.BorderBrush = Brushes.LightSkyBlue;
            }
        }));
        borderFactory.AddHandler(UIElement.MouseLeaveEvent, new MouseEventHandler((sender, _) => {
            Border border = (Border)sender;
            if (border.Background != Brushes.LightSkyBlue) {
                border.Background = Brushes.White;
                border.BorderBrush = Brushes.LightGray;
            }
        }));
        if (onLoaded is not null)
            borderFactory.AddHandler(FrameworkElement.LoadedEvent, onLoaded);

        return borderFactory;
    }

    public static void ShowErrorMessageBox(Exception exception, string? title = null) {
        MessageBox.Show(exception.ToString(), title ?? exception.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public static void CopyTo<T>(this T original, T other) where T : IdModel {
        foreach (PropertyInfo info in typeof(T).GetProperties().Where(x => x.Name != "Id"))
            info.SetValue(other, info.GetValue(original));
    }

    public static SolidColorBrush AdjustAlpha(this SolidColorBrush brush, double multiplier) {
        Color color = brush.Color;
        color.A = (byte)(color.A * multiplier);
        return new(color);
    }
}
