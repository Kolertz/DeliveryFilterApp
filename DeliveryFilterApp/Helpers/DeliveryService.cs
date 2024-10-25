using DeliveryFilterApp.Models;
using Microsoft.Extensions.Logging;

namespace DeliveryFilterApp.Helpers
{
    public class DeliveryService(ILogger<DeliveryService> _logger) : IDeliveryService
    {
        public IEnumerable<OrderModel> FilterOrdersByDistrictAndTime(
            IEnumerable<OrderModel> orders, string district, DateTime firstDeliveryTime)
        {
            _logger.LogInformation("Фильтрация заказов по району: {District} и времени после: {FirstDeliveryTime}", district, firstDeliveryTime);

            var endTime = firstDeliveryTime.AddMinutes(30);
            return orders
                .Where(order => order.CityDistrict.Equals(district, StringComparison.OrdinalIgnoreCase) &&
                                order.DeliveryDateTime >= firstDeliveryTime &&
                                order.DeliveryDateTime <= endTime);
        }
    }
}