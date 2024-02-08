using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace AirportApp;

internal class ErrorsWindow : Window {
    public ErrorsWindow(IEnumerable<Exception> exceptions) {
        TreeView content = new();

        int n = 0;
        foreach (Exception exception in exceptions) {
            TreeViewItem item = CreateItem(exception);
            content.Items.Add(item);
            n++;
        }

        Content = content;
        Title = $"{n} errors";
        Width = SystemParameters.PrimaryScreenWidth * 0.6;
        Height = SystemParameters.PrimaryScreenHeight * 0.8;
    }

    private TreeViewItem CreateItem(Exception exception) {
        Type type = exception.GetType();
        TreeViewItem item = new();
        TextBlock header = new() { FontFamily = new("Consolas"), FontWeight = FontWeights.Bold, Text = type.FullName ?? type.Name };
        TextBox message = new() { FontFamily = new("Consolas"), Text = exception.Message, BorderThickness = new(0), IsReadOnly = true };

        item.Header = header;
        item.Items.Add(message);
        if (exception.InnerException is not null)
            item.Items.Add(CreateItem(exception.InnerException));

        return item;
    }
}
