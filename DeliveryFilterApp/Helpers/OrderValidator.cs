using DeliveryFilterApp.Models;
using Microsoft.Extensions.Logging;

namespace DeliveryFilterApp.Helpers
{
    public class OrderValidator(ILogger<OrderValidator> _logger) : IOrderValidator
    {
        public bool ValidateOrder(OrderModel order)
        {
            if (order.OrderId <= 0)
            {
                _logger.LogWarning("У заказа отсутствует идентификатор.");
                return false;
            }

            if (order.Weight <= 0)
            {
                _logger.LogWarning("Некорректный вес заказа: {Weight}", order.Weight);
                return false;
            }

            if (string.IsNullOrEmpty(order.CityDistrict))
            {
                _logger.LogWarning("У заказа отсутствует район.");
                return false;
            }

            if (order.DeliveryDateTime == default)
            {
                _logger.LogWarning("Некорректное время доставки.");
                return false;
            }

            return true;
        }
    }
}