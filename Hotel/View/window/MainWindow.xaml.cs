using System.Windows;
using System.Windows.Controls;
using Hotel.View.page;

namespace HotelTask
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Orders_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new OrdersPage());
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new CreateBookingPage());
        }
    }
}
