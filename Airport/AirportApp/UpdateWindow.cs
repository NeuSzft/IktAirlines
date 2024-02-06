using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AirportApp
{
    public record UpdateInfo(List<string> AddedItems, List<string> UpdatedItems, List<string> RemovedItems);

    public class UpdateWindow : Window
    {
        public UpdateWindow(UpdateInfo updateInfo)
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
        }
    }
}
