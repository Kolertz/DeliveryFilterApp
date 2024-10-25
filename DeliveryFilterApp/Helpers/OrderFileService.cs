using DeliveryFilterApp.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Globalization;

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

                    // Проверка, что строка содержит все необходимые части
                    if (parts.Length < 4)
                    {
                        _logger.LogWarning("Строка пропущена из-за недостаточного количества полей: {Line}", line);
                        continue;
                    }

                    try
                    {
                        var order = new OrderModel
                        {
                            OrderId = parts[0],

                            Weight = double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var weight)
                                ? weight
                                : throw new FormatException("Некорректный формат веса"),

                            CityDistrict = parts[2],

                            DeliveryDateTime = DateTime.TryParseExact(
                                parts[3],
                                _configuration["DeliveryServiceSettings:DateFormat"]!,
                                null,
                                DateTimeStyles.None,
                                out var deliveryDateTime)
                                ? deliveryDateTime
                                : throw new FormatException("Некорректный формат даты")
                        };

                        orders.Add(order);
                    }
                    catch (Exception innerEx)
                    {
                        // Логируем ошибку и продолжаем выполнение с пропуском этой строки
                        _logger.LogWarning(innerEx, "Ошибка в данных строки: {Line}. Строка пропущена.", line);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при чтении файла заказов.");
                throw;
            }

            return orders;
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
                throw;
            }
        }
    }
}