using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Hotel.Core;
using Hotel.Core.Storage;
using Microsoft.Win32;

namespace Hotel.View.page
{
    /// <summary>
    /// Логика взаимодействия для CreateBookingPage.xaml
    /// </summary>
    public partial class CreateBookingPage : Page
    {
        private BitmapImage selectedImage;
        private readonly FileManager fileManager = new FileManager();

        private static readonly Dictionary<string, decimal> RoomPrices = new()
        {
            { "Одноместный", 1000m },
            { "Двухместный", 1800m },
            { "Люкс", 3000m }
        };
        public CreateBookingPage()
        {
            InitializeComponent();
            LoadRoomTypes();
        }
        private void LoadRoomTypes()
        {
            roomTypeComboBox.Items.Clear();
            foreach (var room in DataSingleton.Rooms)
            {
                roomTypeComboBox.Items.Add(new ComboBoxItem { Content = room.Type });
            }
        }
        private void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new()
            {
                Filter = "Изображения (*.png;*.jpg)|*.png;*.jpg"
            };

            if (dialog.ShowDialog() == true)
            {
                selectedImage = new BitmapImage(new Uri(dialog.FileName));
                clientImage.Source = selectedImage;
            }
        }
        private async void Book_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(fullNameBox.Text) ||
                roomTypeComboBox.SelectedItem == null ||
                !checkInDatePicker.SelectedDate.HasValue ||
                !checkOutDatePicker.SelectedDate.HasValue ||
                selectedImage == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля и загрузите изображение.");
                return;
            }

            DateTime checkIn = checkInDatePicker.SelectedDate.Value;
            DateTime checkOut = checkOutDatePicker.SelectedDate.Value;

            if (checkOut <= checkIn)
            {
                MessageBox.Show("Дата выезда должна быть позже даты заезда.");
                return;
            }

            string selectedRoom = (roomTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            decimal pricePerDay = RoomPrices[selectedRoom];
            int days = (checkOut - checkIn).Days;
            decimal total = days * pricePerDay;

            Booking booking = new()
            {
                Guid = Guid.NewGuid(), // Добавляем Guid
                FullName = fullNameBox.Text,
                RoomType = selectedRoom,
                CheckInDate = checkIn,
                CheckOutDate = checkOut,
                Image = selectedImage,
                TotalPrice = total
            };

            try
            {
                // Сохраняем в файл
                await fileManager.WriteOrder(booking);

                // Добавляем в память
                DataSingleton.Bookings.Add(booking);

                MessageBox.Show($"Бронирование успешно создано!\nСумма: {total:C}");

                // Сбрасываем форму
                fullNameBox.Text = "";
                roomTypeComboBox.SelectedIndex = -1;
                checkInDatePicker.SelectedDate = null;
                checkOutDatePicker.SelectedDate = null;
                clientImage.Source = null;
                selectedImage = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении бронирования: {ex.Message}");
            }
        }
    }
}