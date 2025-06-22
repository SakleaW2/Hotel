using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Hotel.Core
{
    public static class FrameManager
    {
        public static Frame MainFrame { get; set; }

        public static void SetSource<T>(T target) where T : Page
        {
            var contentType = MainFrame.Content?.GetType();

            if (contentType != typeof(T))
            {
                MainFrame.Navigate(target);

                MainFrame.NavigationService.RemoveBackEntry();

                GC.Collect();
            }
        }
    }
}
