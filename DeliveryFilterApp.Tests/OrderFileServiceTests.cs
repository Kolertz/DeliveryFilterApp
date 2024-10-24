using DeliveryFilterApp.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Moq;
using Microsoft.Extensions.Logging.Abstractions;

namespace DeliveryFilterApp.Tests
{
    public class OrderFileServiceTests
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly OrderFileService _orderFileService;

        public OrderFileServiceTests()
        {
            var logger = NullLogger<OrderFileService>.Instance;
            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(c => c["DeliveryServiceSettings:DateFormat"]).Returns("yyyy-MM-dd");

            _orderFileService = new OrderFileService(logger, _configurationMock.Object);
        }

        [Fact]
        public void LoadOrdersFromFile_ShouldReturnCorrectOrders_WhenFileIsValid()
        {
            // Arrange
            var testFilePath = "test_orders.csv";
            var testData = "1,5.5,Center,2024-10-24\n2,7.0,South,2024-10-25\n";
            File.WriteAllText(testFilePath, testData);

            // Act
            var result = _orderFileService.LoadOrdersFromFile(testFilePath).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("1", result[0].OrderId);
            Assert.Equal(5.5, result[0].Weight);
            Assert.Equal("Center", result[0].CityDistrict);
            Assert.Equal(new DateTime(2024, 10, 24), result[0].DeliveryDateTime);

            Assert.Equal("2", result[1].OrderId);
            Assert.Equal(7.0, result[1].Weight);
            Assert.Equal("South", result[1].CityDistrict);
            Assert.Equal(new DateTime(2024, 10, 25), result[1].DeliveryDateTime);

            // Clean up
            File.Delete(testFilePath);
        }

        [Fact]
        public void LoadOrdersFromFile_ShouldSkipInvalidLines()
        {
            // Arrange
            var testFilePath = "test_orders_invalid.csv";
            var testData = "1,5.5,Center,2024-10-24\n2,not_a_number,South,2024-10-25\n3,7.0,North,invalid_date\n";
            File.WriteAllText(testFilePath, testData);

            // Act
            var result = _orderFileService.LoadOrdersFromFile(testFilePath).ToList();
            Console.WriteLine(result);
            // Assert
            Assert.Single(result); // Ожидаем, что будет только 1 корректный заказ

            // Проверяем данные корректного заказа
            Assert.Equal("1", result[0].OrderId);
            Assert.Equal(5.5, result[0].Weight);
            Assert.Equal("Center", result[0].CityDistrict);
            Assert.Equal(new DateTime(2024, 10, 24), result[0].DeliveryDateTime);

            // Clean up
            File.Delete(testFilePath);
        }

        [Fact]
        public void LoadOrdersFromFile_ShouldLogError_WhenFileNotFound()
        {
            // Arrange
            var nonExistentFilePath = "non_existent_file.csv";

            // Act
            var result = _orderFileService.LoadOrdersFromFile(nonExistentFilePath).ToList();

            // Assert
            Assert.Empty(result); // Ожидаем, что результат пустой
        }
    }
}
