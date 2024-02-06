using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AirportApp
{
    public record UpdateInfo(List<string> AddedItems, List<string> UpdatedItems, List<string> RemovedItems);

    public class UpdateWindow : Window
    {
        private readonly UpdateInfo _updateInfo;
        public UpdateWindow(UpdateInfo updateInfo)
        {
            _updateInfo = updateInfo;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            Title = "Update Information";
            MinWidth = 300;
            MaxWidth = 900;
            MinHeight = 200;
            MaxHeight = 600;
            SizeToContent = SizeToContent.WidthAndHeight;
            ResizeMode = ResizeMode.CanMinimize;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            ScrollViewer _scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                Content = GenerateChangesContent(_updateInfo)
            };

            Content = _scrollViewer;
        }

        private UIElement GenerateChangesContent(UpdateInfo updateInfo)
        {
            StackPanel mainPanel = new StackPanel { Orientation = Orientation.Vertical, Margin = new Thickness(10) };

            StackPanel newItemsPanel = new StackPanel { Orientation = Orientation.Vertical, Margin = new Thickness(5) };
            newItemsPanel.Children.Add(new TextBlock { Text = "New Items:", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 5) });
            foreach (var item in updateInfo.AddedItems)
            {
                newItemsPanel.Children.Add(new TextBlock { Text = $"{item}", Background = Brushes.Green.AdjustAlpha(0.5) });
            }

            StackPanel updatedItemsPanel = new StackPanel { Orientation = Orientation.Vertical, Margin = new Thickness(5) };
            updatedItemsPanel.Children.Add(new TextBlock { Text = "Edited Items:", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 10, 0, 5) });
            foreach (var item in updateInfo.UpdatedItems)
            {
                updatedItemsPanel.Children.Add(new TextBlock { Text = $"{item}", Background = Brushes.Yellow.AdjustAlpha(0.5) });
            }

            StackPanel removedItemsPanel = new StackPanel { Orientation = Orientation.Vertical, Margin = new Thickness(5) };
            removedItemsPanel.Children.Add(new TextBlock { Text = "Removed Items:", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 10, 0, 5) });
            foreach (var item in updateInfo.RemovedItems)
            {
                removedItemsPanel.Children.Add(new TextBlock { Text = $"{item}", Background = Brushes.Red.AdjustAlpha(0.5) });
            }

            mainPanel.Children.Add(newItemsPanel);
            mainPanel.Children.Add(updatedItemsPanel);
            mainPanel.Children.Add(removedItemsPanel);

            return mainPanel;
        }
    }
}
