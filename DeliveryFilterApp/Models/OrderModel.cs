namespace DeliveryFilterApp.Models
{
    public class OrderModel
    {
        public int OrderId { get; set; }
        public double Weight { get; set; }
        public required string CityDistrict { get; set; }
        public DateTime DeliveryDateTime { get; set; }
    }
}