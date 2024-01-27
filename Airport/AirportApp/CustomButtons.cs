using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace AirportApp;

internal class CustomButton : ButtonBase {
    public Border Border { get; } = new() { BorderThickness = new(1), CornerRadius = new(4) };

    public Brush BackgroundColor { get; set; } = Brushes.WhiteSmoke;
    public Brush BorderColor { get; set; } = Brushes.LightGray;
    public Brush SelectedBackgroundColor { get; set; } = Brushes.AliceBlue;
    public Brush SelectedBorderColor { get; set; } = Brushes.LightSkyBlue;

    public CustomButton() {
        Content = Border;
        Loaded += (_, _) => {
            Border.Background = BackgroundColor;
            Border.BorderBrush = BorderColor;
        };
    }

    public CustomButton(UIElement content) : this() => Border.Child = content;

    public CustomButton(string text, double fontSize = 11) : this(new TextBlock { Text = text, FontSize = fontSize, TextAlignment = TextAlignment.Center }) { }

    protected override void OnMouseEnter(MouseEventArgs e) {
        Border.Background = SelectedBackgroundColor;
        Border.BorderBrush = SelectedBorderColor;
        base.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(MouseEventArgs e) {
        Border.Background = BackgroundColor;
        Border.BorderBrush = BorderColor;
        base.OnMouseLeave(e);
    }

    protected override void OnMouseDown(MouseButtonEventArgs e) {
        Border.Background = SelectedBorderColor;
        base.OnMouseDown(e);
    }

    protected override void OnMouseUp(MouseButtonEventArgs e) {
        Border.Background = SelectedBackgroundColor;
        base.OnMouseUp(e);
    }
}

internal sealed class GlyphButton : CustomButton {
    public GlyphButton(int glyph, Brush color, double size = double.NaN) {
        TextBlock text = new() {
            Text = Convert.ToChar(glyph).ToString(),
            FontFamily = new("Segoe MDL2 Assets"),
            Foreground = color,
            TextAlignment = TextAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };

        if (size is not double.NaN)
            text.FontSize = size * 0.6;

        Border.Child = text;
        BackgroundColor = BorderColor = Brushes.Transparent;
        Width = Height = size;

        IsEnabledChanged += (_, e) => text.Foreground = (e.NewValue is bool val && val) ? color : Brushes.Gray;
    }
}
