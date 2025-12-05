using System;
using System.Text;

namespace AdapterPattern
{
    // ---------------------------------------------------------------------------
    // 1. Target (Цільовий інтерфейс)
    // Це інтерфейс, який очікує наш клієнтський код (нова система).
    // ---------------------------------------------------------------------------
    public interface IDeliveryService
    {
        void DeliverData(string itemId, double weightKg);
        double CalculateDeliveryCost(double distanceKm);
    }

    // ---------------------------------------------------------------------------
    // 2. Adaptee (Адаптовуваний клас)
    // Сторонній клас (стара система або бібліотека), який має інший інтерфейс.
    // Наприклад, працює з фунтами та милями, і методи називаються інакше
    // ---------------------------------------------------------------------------
    public class ExternalLogisticsService
    {
        // Відправка товару (вага в фунтах - lbs)
        public void ShipItem(string itemName, double weightLbs)
        {
            Console.WriteLine($"[ExternalLogistics] Item '{itemName}' shipped. Weight: {weightLbs:F2} lbs.");
        }

        // Розрахунок вартості (дистанція в милях - miles)
        // Тариф: 0.5$ за милю
        public double GetCostInUsd(double miles)
        {
            return miles * 0.5;
        }
    }

    // ---------------------------------------------------------------------------
    // 3. Adapter (Адаптер)
    // Реалізує цільовий інтерфейс і використовує об'єкт Adaptee.
    // Конвертує дані (Кг -> Фунти, Км -> Милі).
    // ---------------------------------------------------------------------------
    public class LogisticsAdapter : IDeliveryService
    {
        private readonly ExternalLogisticsService _externalService;

        // Конструктор приймає адаптовуваний об'єкт
        public LogisticsAdapter(ExternalLogisticsService externalService)
        {
            _externalService = externalService;
        }

        public void DeliverData(string itemId, double weightKg)
        {
            // Конвертація: 1 кг = 2.20462 фунтів
            double weightLbs = weightKg * 2.20462;
            
            Console.WriteLine($"\n>> Адаптер: Конвертуємо {weightKg} кг -> {weightLbs:F2} lbs");
            
            // Виклик методу старої системи
            _externalService.ShipItem(itemId, weightLbs);
        }

        public double CalculateDeliveryCost(double distanceKm)
        {
            // Конвертація: 1 км = 0.621371 миль
            double miles = distanceKm * 0.621371;

            Console.WriteLine($">> Адаптер: Конвертуємо {distanceKm} км -> {miles:F2} миль");

            // Отримуємо ціну в доларах
            double costUsd = _externalService.GetCostInUsd(miles);

            // Наприклад, конвертуємо в гривні (курс 41.5)
            double costUah = costUsd * 41.5;
            
            Console.WriteLine($">> Адаптер: Ціна ${costUsd:F2} -> {costUah:F2} грн");

            return costUah;
        }
    }

    // ---------------------------------------------------------------------------
    // 4. Client (Клієнт)
    // Працює з інтерфейсом IDeliveryService, не знаючи про ExternalLogisticsService.
    // ---------------------------------------------------------------------------
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("--- Лабораторна робота №6. Патерн Адаптер (Variant 6) ---\n");

            // Сценарій: У нас є товар і відстань
            string product = "Ноутбук Gaming X";
            double weightKg = 2.5;   // кг
            double distanceKm = 100; // км

            Console.WriteLine("1. Спроба використання старої системи напряму (незручно):");
            ExternalLogisticsService oldService = new ExternalLogisticsService();
            // Нам довелося б вручну переводити кг у фунти тут...
            oldService.ShipItem(product, weightKg * 2.20462); 

            Console.WriteLine("\n---------------------------------------------------------");
            Console.WriteLine("2. Використання через Адаптер (зручно):");

            // Створюємо адаптер, передаючи йому стару службу
            IDeliveryService deliverySystem = new LogisticsAdapter(oldService);

            // Клієнтський код просто викликає методи у зрозумілих одиницях (кг, км)
            deliverySystem.DeliverData(product, weightKg);
            
            double price = deliverySystem.CalculateDeliveryCost(distanceKm);
            Console.WriteLine($"\nФінальна вартість доставки: {price:F2} грн");

            Console.ReadKey();
        }
    }
}
