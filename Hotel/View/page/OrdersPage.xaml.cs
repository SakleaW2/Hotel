using System.Windows;
using System.Windows.Controls;

using Hotel.Core.Storage;

namespace Hotel.View.page
{

    public partial class OrdersPage : Page
    {
        private readonly Core.FileManager fileManager = new Core.FileManager();
        public OrdersPage()
        {
            InitializeComponent();
            LoadData();
        }
        private async void LoadData()
        {
            try
            {
                await fileManager.LoadInitialData();
                bookingsGrid.ItemsSource = DataSingleton.Bookings;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }
        private void bookingsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (bookingsGrid.SelectedItem is Booking selectedBooking && selectedBooking.Image != null)
            {
                selectedImage.Source = selectedBooking.Image;
            }
            else
            {
                selectedImage.Source = null;
            }
        }
    }
}
