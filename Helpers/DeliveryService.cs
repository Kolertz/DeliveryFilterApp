using DeliveryFilterApp.Models;
using Microsoft.Extensions.Logging;

namespace DeliveryFilterApp.Helpers
{
    public class DeliveryService(ILogger<DeliveryService> logger) : IDeliveryService
    {
        private readonly ILogger<DeliveryService> _logger = logger;

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

        public void SaveFilteredOrders(IEnumerable<OrderModel> filteredOrders, string filePath)
        {
            _logger.LogInformation("Сохранение отфильтрованных заказов в файл: {FilePath}", filePath);
            try
            {
                using var writer = new StreamWriter(filePath);
                foreach (var order in filteredOrders)
                {
                    writer.WriteLine($"{order.OrderId},{order.Weight},{order.CityDistrict},{order.DeliveryDateTime}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при записи файла заказов.");
            }
        }
    }
}