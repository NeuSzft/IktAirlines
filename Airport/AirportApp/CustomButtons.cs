using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace AirportApp;

internal class CustomButton : ButtonBase {
    public Border Border { get; } = new() { BorderThickness = new(1), CornerRadius = new(4) };

    public SolidColorBrush Color { get; set; } = Brushes.LightGray;
    public SolidColorBrush SelectedColor { get; set; } = Brushes.LightSkyBlue;

    public float BackgroundColorAdjust { get; set; } = 0.3f;

    public CustomButton() {
        Content = Border;
        Loaded += (_, _) => {
            Border.Background = Color.AdjustAlpha(BackgroundColorAdjust);
            Border.BorderBrush = Color;
        };
    }

    public CustomButton(UIElement content) : this() => Border.Child = content;

    public CustomButton(string text, double fontSize = 11) : this(new TextBlock { Text = text, FontSize = fontSize, TextAlignment = TextAlignment.Center }) { }

    protected override void OnMouseEnter(MouseEventArgs e) {
        Border.Background = SelectedColor.AdjustAlpha(BackgroundColorAdjust);
        Border.BorderBrush = SelectedColor;
        base.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(MouseEventArgs e) {
        Border.Background = Color.AdjustAlpha(BackgroundColorAdjust);
        Border.BorderBrush = Color;
        base.OnMouseLeave(e);
    }

    protected override void OnMouseDown(MouseButtonEventArgs e) {
        Border.Background = SelectedColor;
        base.OnMouseDown(e);
    }

    protected override void OnMouseUp(MouseButtonEventArgs e) {
        Border.Background = SelectedColor.AdjustAlpha(BackgroundColorAdjust);
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
        Color = Color = Brushes.Transparent;
        Width = Height = size;

        IsEnabledChanged += (_, e) => text.Foreground = (e.NewValue is bool val && val) ? color : Brushes.Gray;
    }
}
