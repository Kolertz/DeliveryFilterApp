using DeliveryFilterApp.Models;

namespace DeliveryFilterApp.Helpers
{
    public interface IDeliveryService
    {
        IEnumerable<OrderModel> FilterOrdersByDistrictAndTime(
            IEnumerable<OrderModel> orders, string district, DateTime firstDeliveryTime);
    }
}