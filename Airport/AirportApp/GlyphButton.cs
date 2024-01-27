using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace AirportApp;

internal sealed class GlyphButton : Button {
    public GlyphButton(int glyph, Brush color, double size = double.NaN) {
        TextBlock content = new() {
            Text = Convert.ToChar(glyph).ToString(),
            FontFamily = new("Segoe MDL2 Assets"),
            Foreground = color
        };

        if (size is not double.NaN)
            content.FontSize = size * 0.6;

        Content = content;
        Width = Height = size;
        Background = Brushes.Transparent;
        BorderThickness = new(0);

        IsEnabledChanged += (_, e) => content.Foreground = (e.NewValue is bool val && val) ? color : Brushes.Gray;
    }
}
