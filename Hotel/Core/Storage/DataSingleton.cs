using Hotel.Model;
using System.Collections.Generic;

namespace Hotel.Core.Storage
{
    public static class DataSingleton
    {
        public static List<Booking> Bookings { get; } = new();
        public static List<Room> Rooms { get; } = new()
        {
            new Room { Type = "Одноместный", 
                        Price = 1000 },
            new Room { Type = "Двухместный", 
                        Price = 1800 },
            new Room { Type = "Люкс", 
                        Price = 3000 }
        };  
    }
}