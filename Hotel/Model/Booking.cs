using System.Windows.Media;

public class Booking
{
    public Guid Guid { get; set; }
    public string FullName { get; set; }
    public string RoomType { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public ImageSource Image { get; set; }
    public decimal TotalPrice { get; set; }
}
