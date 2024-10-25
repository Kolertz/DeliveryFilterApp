using DeliveryFilterApp.Helpers;
using DeliveryFilterApp.Models;
using Microsoft.Extensions.Logging.Abstractions;

namespace DeliveryFilterApp.Tests
{
    public class DeliveryServiceTests
    {
        private readonly DeliveryService _deliveryService;

        public DeliveryServiceTests()
        {
            // Используем NullLogger для заглушки логов в тестах
            var logger = NullLogger<DeliveryService>.Instance;
            _deliveryService = new DeliveryService(logger);
        }

        [Fact]
        public void FilterOrdersByDistrictAndTime_ShouldFilterCorrectly()
        {
            // Arrange
            var orders = new List<OrderModel>
            {
                new() { OrderId = 1, CityDistrict = "Center", DeliveryDateTime = new DateTime(2024, 10, 22, 10, 0, 0) },
                new() { OrderId = 2, CityDistrict = "Center", DeliveryDateTime = new DateTime(2024, 10, 22, 10, 15, 0) },
                new() { OrderId = 3, CityDistrict = "Center", DeliveryDateTime = new DateTime(2024, 10, 22, 10, 45, 0) },
                new() { OrderId = 4, CityDistrict = "North", DeliveryDateTime = new DateTime(2024, 10, 22, 10, 15, 0) }
            };

            var firstDeliveryTime = new DateTime(2024, 10, 22, 10, 0, 0);

            // Act
            var result = _deliveryService.FilterOrdersByDistrictAndTime(orders, "Center", firstDeliveryTime).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, order => order.OrderId == 1);
            Assert.Contains(result, order => order.OrderId == 2);
            Assert.DoesNotContain(result, order => order.OrderId == 3);  // Время за пределами 30 минут
            Assert.DoesNotContain(result, order => order.OrderId == 4);  // Другой район
        }
    }
}