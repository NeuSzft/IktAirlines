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
        }
    }
}
