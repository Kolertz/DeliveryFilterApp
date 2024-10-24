using DeliveryFilterApp.Models;

namespace DeliveryFilterApp.Helpers
{
    public interface IOrderValidator
    {
        bool ValidateOrder(OrderModel order);
    }
}