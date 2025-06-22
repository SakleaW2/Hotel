using Hotel.Core.Storage;
using Hotel.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace Hotel.Core
{
    public class FileManager
    {
        public readonly string _path = @"./Data/";
        public FileManager()
        {
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
            if (!Directory.Exists(Path.Combine(_path, "Documents/")))
            {
                Directory.CreateDirectory(Path.Combine(_path, "Documents/"));
            }

            if (!File.Exists(Path.Combine(_path, "Rooms.txt")))
            {
                File.Create(Path.Combine(_path, "Rooms.txt")).Close();

                DefaultData();
            }
            if (!File.Exists(Path.Combine(_path, "Orders.txt")))
            {
                File.Create(Path.Combine(_path, "Orders.txt")).Close();
            }
        }
        private async void DefaultData()
        {
            using (StreamWriter writer = new(Path.Combine(_path, "Rooms.txt"), false))
            {
                foreach (var room in DataSingleton.Rooms)
                {
                    await writer.WriteLineAsync($"{room.Type}, {room.Price}");
                }
            }
        }
        public async Task<string> ReadFile(string name)
        {
            var path = Path.Combine(_path, $"{name}.txt");

            if (!File.Exists(path))
            {
                throw new Exception();
            }

            using (StreamReader reader = new(path))
            {
                return await reader.ReadToEndAsync();
            }
        }
        public BitmapImage GetImage(Guid guid)
        {
            BitmapImage bitmap = new();
            try
            {
                string imagePath = Path.Combine(_path, "Documents", $"{guid}.png")
                       .Replace('\\', '/');

                if (!File.Exists(imagePath))
                {
                    throw new FileNotFoundException($"Файл не найден: {imagePath}");
                }

                var info = new FileInfo(imagePath);

                bitmap.BeginInit();
                bitmap.UriSource = new Uri(info.FullName);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                return bitmap;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка загрузки изображения: {ex.Message}");
            }
        }
        public async Task WriteOrder(Booking booking)
        {
            var path = Path.Combine(_path, "Orders.txt");

            if (!File.Exists(path))
            {
                throw new Exception();
            }

            using (StreamWriter writer = new(path, true))
            {
                await writer.WriteLineAsync($"{booking.Guid}, {booking.FullName}, {booking.RoomType}, {booking.CheckInDate}, {booking.CheckOutDate}, {booking.TotalPrice}");

                if (booking.Image is BitmapImage bitmapImage)
                {
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmapImage));

                    using (var fileStream = new FileStream(Path.Combine(_path, $"Documents/{booking.Guid}.png"), FileMode.Create))
                    {
                        encoder.Save(fileStream);
                    }
                }
                else
                {
                    throw new NotSupportedException("Тип изображения не поддерживается");
                }
            }
        }
        public async Task LoadInitialData()
        {
            try
            {
                string roomsData = await ReadFile("Rooms");
                if (!string.IsNullOrEmpty(roomsData))
                {
                    DataSingleton.Rooms.Clear();
                    foreach (var line in roomsData.Split('\n').Where(l => !string.IsNullOrWhiteSpace(l)))
                    {
                        var parts = line.Split(',');
                        if (parts.Length == 2 && decimal.TryParse(parts[1].Trim(), out decimal price))
                        {
                            DataSingleton.Rooms.Add(new Room { Type = parts[0].Trim(), Price = price });
                        }
                    }
                }

                string bookingsData = await ReadFile("Orders");
                if (!string.IsNullOrEmpty(bookingsData))
                {
                    DataSingleton.Bookings.Clear();
                    foreach (var line in bookingsData.Split('\n').Where(l => !string.IsNullOrWhiteSpace(l)))
                    {
                        var parts = line.Split(',');
                        if (parts.Length == 6 && Guid.TryParse(parts[0].Trim(), out Guid guid))
                        {
                            var booking = new Booking
                            {
                                Guid = guid,
                                FullName = parts[1].Trim(),
                                RoomType = parts[2].Trim(),
                                CheckInDate = DateTime.Parse(parts[3].Trim()),
                                CheckOutDate = DateTime.Parse(parts[4].Trim()),
                                TotalPrice = decimal.Parse(parts[5].Trim())
                            };

                            try
                            {
                                booking.Image = GetImage(booking.Guid);
                            }
                            catch
                            {
                                // Если изображение не загружено, оставляем null
                            }

                            DataSingleton.Bookings.Add(booking);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка загрузки данных", ex);
            }
        }
    }
}
