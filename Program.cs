using DeliveryFilterApp.Helpers;
using DeliveryFilterApp.Helpers.Logger;
using DeliveryFilterApp.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Globalization;

// Настройка конфигурации
var configuration = new ConfigurationBuilder()
    // Использованы switchMappings для соответствия стандарту конфигрурации PascalCase
    .AddCommandLine(args, new Dictionary<string, string>
    {
        { "-_cityDistrict", "DeliveryServiceSettings:CityDistrict" },                    // Указанный район для фильтрации заказов
        { "-_firstDeliveryDateTime", "DeliveryServiceSettings:FirstDeliveryDateTime" },  // Время первой доставки для фильтрации
        { "-_deliveryOrder", "DeliveryServiceSettings:DeliveryOrder" },                  // Путь для сохранения отфильтрованных заказов
        { "-_filteredOrdersFile", "DeliveryServiceSettings:OriginalOrder" },             // Путь к файлу с исходными данными заказов
        { "-_deliveryLog", "Logging:FileLogging:Path" },

        { "--_cityDistrict", "DeliveryServiceSettings:CityDistrict" },
        { "--_firstDeliveryDateTime", "DeliveryServiceSettings:FirstDeliveryDateTime" },
        { "--_deliveryOrder", "DeliveryServiceSettings:DeliveryOrder" },
        { "--_filteredOrdersFile", "DeliveryServiceSettings:OriginalOrder" },
        { "--_deliveryLog", "Logging:FileLogging:Path" }
    })
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)  // Чтение из файла конфигурации, файл обязателен т.к. там настроен формат даты DateFormat
    .Build();

// Получение параметров:
// Указанный район для фильтрации заказов
var cityDistrict = configuration["DeliveryServiceSettings:CityDistrict"]
    ?? throw new ArgumentException("Не указан район доставки через параметр _cityDistrict");

// Время первой доставки для фильтрации
var firstDeliveryDateTimeString = configuration["DeliveryServiceSettings:FirstDeliveryDateTime"]
    ?? throw new ArgumentException("Не указано время первого заказа через параметр _firstDeliveryDateTime");

// Путь к файлу логов
var logFilePath = configuration["Logging:FileLogging:Path"]
    ?? "delivery.log";

// Путь для сохранения отфильтрованных заказов
var orderFilePath = configuration["DeliveryServiceSettings:DeliveryOrder"]
    ?? "filtered_orders.csv";

// Путь к файлу с исходными данными заказов
var orderDataFilePath = configuration["DeliveryServiceSettings:OriginalOrder"]
    ?? "orders.csv";

// Регистрация зависимостей
var serviceProvider = new ServiceCollection()
    .AddLogging(builder =>
    {
        builder.AddConsole();
        builder.AddProvider(new FileLoggerProvider(logFilePath));  // Логирование в файл
    })
    .AddSingleton<IDeliveryService, DeliveryService>()
    .AddSingleton<IOrderValidator, OrderValidator>()
    .AddSingleton<IOrderFileService, OrderFileService>()
    .AddSingleton<IConfiguration>(configuration)
    .BuildServiceProvider();

var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

if (!DateTime.TryParseExact(firstDeliveryDateTimeString, configuration["DeliveryServiceSettings:DateFormat"], null, DateTimeStyles.None, out DateTime firstDeliveryDateTime))
{
    logger.LogError("Некорректный формат даты для параметра _firstDeliveryDateTime");
    return;
}

logger.LogInformation("Программа запущена");
try
{
    // Инициализация служб
    var deliveryService = serviceProvider.GetRequiredService<IDeliveryService>();
    var orderValidator = serviceProvider.GetRequiredService<IOrderValidator>();
    var orderFileService = serviceProvider.GetRequiredService<IOrderFileService>();

    // Загрузка данных заказов
    var orders = orderFileService.LoadOrdersFromFile(orderDataFilePath);

    // Валидация данных
    var validOrders = new List<OrderModel>();
    foreach (var order in orders)
    {
        if (orderValidator.ValidateOrder(order))
        {
            validOrders.Add(order);
        }
    }

    // Фильтрация заказов
    var filteredOrders = deliveryService.FilterOrdersByDistrictAndTime(validOrders, cityDistrict, firstDeliveryDateTime);

    // Сохранение результата фильтрации
    deliveryService.SaveFilteredOrders(filteredOrders, orderFilePath);

    logger.LogInformation("Программа завершена успешно");
}
catch (Exception ex)
{
    logger.LogError(ex, "Произошла ошибка в процессе выполнения программы");
}