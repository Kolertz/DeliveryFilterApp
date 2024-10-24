using DeliveryFilterApp.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DeliveryFilterApp.Helpers
{
    public class OrderFileService(ILogger<OrderFileService> _logger, IConfiguration _configuration) : IOrderFileService
    {
        public IEnumerable<OrderModel> LoadOrdersFromFile(string filePath)
        {
            _logger.LogInformation("Загрузка заказов из файла: {FilePath}", filePath);
            var orders = new List<OrderModel>();

            try
            {
                foreach (var line in File.ReadLines(filePath))
                {
                    var parts = line.Split(',');
                    var order = new OrderModel
                    {
                        OrderId = parts[0],
                        Weight = double.Parse(parts[1]),
                        CityDistrict = parts[2],
                        DeliveryDateTime = DateTime.ParseExact(parts[3], _configuration["DeliveryServiceSettings:DateFormat"]!, null)
                    };
                    orders.Add(order);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при чтении файла заказов.");
            }

            return orders;
        }
    }
}