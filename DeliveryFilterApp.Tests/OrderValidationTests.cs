using DeliveryFilterApp.Helpers;
using DeliveryFilterApp.Models;
using Microsoft.Extensions.Logging.Abstractions;

namespace DeliveryFilterApp.Tests
{
    public class OrderValidatorTests
    {
        private readonly OrderValidator _orderValidator;

        public OrderValidatorTests()
        {
            // Используем NullLogger для заглушки логов в тестах
            var logger = NullLogger<OrderValidator>.Instance;
            _orderValidator = new OrderValidator(logger);
        }

        [Fact]
        public void ValidateOrder_ShouldReturnTrueForValidOrder()
        {
            // Arrange
            var validOrder = new OrderModel
            {
                OrderId = "1",
                Weight = 10.5,
                CityDistrict = "Center",
                DeliveryDateTime = new DateTime(2024, 10, 22, 10, 0, 0)
            };

            // Act
            var result = _orderValidator.ValidateOrder(validOrder);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateOrder_ShouldReturnFalseForInvalidOrderId()
        {
            // Arrange
            var invalidOrder = new OrderModel
            {
                OrderId = "",  // Некорректный ID
                Weight = 10.5,
                CityDistrict = "Center",
                DeliveryDateTime = new DateTime(2024, 10, 22, 10, 0, 0)
            };

            // Act
            var result = _orderValidator.ValidateOrder(invalidOrder);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateOrder_ShouldReturnFalseForNegativeWeight()
        {
            // Arrange
            var invalidOrder = new OrderModel
            {
                OrderId = "2",
                Weight = -5,  // Некорректный вес
                CityDistrict = "Center",
                DeliveryDateTime = new DateTime(2024, 10, 22, 10, 0, 0)
            };

            // Act
            var result = _orderValidator.ValidateOrder(invalidOrder);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateOrder_ShouldReturnFalseForEmptyCityDistrict()
        {
            // Arrange
            var invalidOrder = new OrderModel
            {
                OrderId = "3",
                Weight = 5,
                CityDistrict = "",  // Некорректный район
                DeliveryDateTime = new DateTime(2024, 10, 22, 10, 0, 0)
            };

            // Act
            var result = _orderValidator.ValidateOrder(invalidOrder);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateOrder_ShouldReturnFalseForInvalidDeliveryDateTime()
        {
            // Arrange
            var invalidOrder = new OrderModel
            {
                OrderId = "4",
                Weight = 5,
                CityDistrict = "Center",
                DeliveryDateTime = default  // Некорректная дата
            };

            // Act
            var result = _orderValidator.ValidateOrder(invalidOrder);

            // Assert
            Assert.False(result);
        }
    }
}