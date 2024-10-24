using DeliveryFilterApp.Models;

namespace DeliveryFilterApp.Helpers
{
    public interface IOrderFileService
    {
        IEnumerable<OrderModel> LoadOrdersFromFile(string filePath);
    }
}