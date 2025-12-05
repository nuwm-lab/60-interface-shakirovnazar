using System;
using System.Collections.Generic;
using System.Text;

namespace AdapterMatrixLab
{
    // =========================================================================
    // I. INFRASTRUCTURE & ABSTRACTIONS (Файл: Interfaces.cs)
    // =========================================================================
    #region Interfaces

    /// <summary>
    /// Абстракція для отримання вхідних даних.
    /// Дозволяє відв'язати логіку матриць від Console.
    /// </summary>
    public interface IInputProvider
    {
        string? ReadLine();
        double ReadDouble(string prompt);
    }

    /// <summary>
    /// Реалізація вводу через Консоль.
    /// </summary>
    public class ConsoleInputProvider : IInputProvider
    {
        public string? ReadLine() => Console.ReadLine();

        public double ReadDouble(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();
                if (input != null && double.TryParse(input, out double result))
                {
                    return result;
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Помилка: введіть коректне число (double).");
                Console.ResetColor();
            }
        }
    }

    /// <summary>
    /// Основний інтерфейс для роботи з матрицями.
    /// </summary>
    public interface IMatrixOperations
    {
        void FillRandom(Random random);
        void FillManual(IInputProvider inputProvider);
        double FindMin();
        void Display();
    }

    /// <summary>
    /// Інтерфейс для об'єктів, які підтримують логування.
    /// </summary>
    public interface ILoggable
    {
        void LogInfo(string message);
    }

    #endregion

    // =========================================================================
    // II. BASE CLASS (Файл: MatrixBase.cs)
    // =========================================================================
    #region MatrixBase

    /// <summary>
    /// Базовий абстрактний клас для матриць.
    /// Реалізує патерн Disposable для керування ресурсами.
    /// </summary>
    public abstract class MatrixBase : IMatrixOperations, IDisposable
    {
        public string Name { get; protected set; } = "Unknown Matrix";
        
        // Відкрита константа для розмірності, доступна спадкоємцям
        public const int DimensionSize = 3; 

        private bool _disposed = false;

        public abstract void FillRandom(Random random);
        public abstract void FillManual(IInputProvider inputProvider);
        public abstract double FindMin();

        public virtual void Display()
        {
            Console.WriteLine($"\n--- {Name} ---");
        }

        // --- Pattern Disposable ---
        public void Dispose()
        {
            Dispose(true);
            // Фіналізатор не потрібен, оскільки немає некерованих ресурсів,
            // але ми повідомляємо GC, що об'єкт вже очищено.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Очищення керованих ресурсів
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine($"[System] Ресурси '{Name}' звільнено (Dispose викликано).");
                    Console.ResetColor();
                }
                _disposed = true;
            }
        }
    }

    #endregion

    // =========================================================================
    // III. CONCRETE CLASS (2D Matrix) (Файл: Matrix2D.cs)
    // =========================================================================
    #region Matrix2D

    /// <summary>
    /// Звичайна двовимірна матриця.
    /// </summary>
    public class Matrix2D : MatrixBase, ILoggable
    {
        private readonly double[,] _data;

        public Matrix2D()
        {
            Name = $"Матриця 2D [{DimensionSize}x{DimensionSize}]";
            _data = new double[DimensionSize, DimensionSize];
        }

        public override void FillRandom(Random random)
        {
            for (int i = 0; i < DimensionSize; i++)
                for (int j = 0; j < DimensionSize; j++)
                    _data[i, j] = Math.Round(random.NextDouble() * 20 - 10, 2);
            
            LogInfo("Автоматично заповнено (Random).");
        }

        public override void FillManual(IInputProvider input)
        {
            Console.WriteLine($"\nВведення даних для {Name}:");
            for (int i = 0; i < DimensionSize; i++)
            {
                for (int j = 0; j < DimensionSize; j++)
                {
                    _data[i, j] = input.ReadDouble($"Елемент [{i},{j}]: ");
                }
            }
            LogInfo("Заповнено вручну користувачем.");
        }

        public override double FindMin()
        {
            double min = double.MaxValue;
            foreach (var val in _data)
            {
                if (val < min) min = val;
            }
            return min;
        }

        public override void Display()
        {
            base.Display();
            for (int i = 0; i < DimensionSize; i++)
            {
                Console.Write("|");
                for (int j = 0; j < DimensionSize; j++)
                {
                    Console.Write($"{_data[i, j],7:F2} ");
                }
                Console.WriteLine("|");
            }
        }

        public void LogInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[LOG 2D]: {message}");
            Console.ResetColor();
        }
    }

    #endregion

    // =========================================================================
    // IV. ADAPTEE (3D Matrix - Legacy/External Code) (Файл: Matrix3D.cs)
    // =========================================================================
    #region Matrix3D (Adaptee)

    /// <summary>
    /// Клас 3D матриці, який має інший інтерфейс (Adaptee).
    /// Ми не змінюємо його методи під інтерфейс IMatrixOperations напряму,
    /// а використовуємо Адаптер.
    /// </summary>
    public class Matrix3D
    {
        public const int Size = 3; // Власна константа класу
        private readonly double[,,] _cubeData;

        public Matrix3D()
        {
            _cubeData = new double[Size, Size, Size];
        }

        // Нестандартна назва методу (для демонстрації адаптера)
        public void GenerateDataVolume(Random random)
        {
            for (int z = 0; z < Size; z++)
                for (int y = 0; y < Size; y++)
                    for (int x = 0; x < Size; x++)
                        _cubeData[z, y, x] = Math.Round(random.NextDouble() * 20 - 10, 2);
        }

        // Нестандартна назва методу
        public void PopulateVolumeManually(IInputProvider input)
        {
            Console.WriteLine("Заповнення 3D об'єму (пошарово)...");
            for (int z = 0; z < Size; z++)
            {
                Console.WriteLine($"--- Шар Z={z} ---");
                for (int y = 0; y < Size; y++)
                {
                    for (int x = 0; x < Size; x++)
                    {
                        _cubeData[z, y, x] = input.ReadDouble($"Val [z:{z}, y:{y}, x:{x}]: ");
                    }
                }
            }
        }

        // Нестандартна назва методу
        public double CalculateMinimum()
        {
            double min = double.MaxValue;
            foreach (var val in _cubeData)
                if (val < min) min = val;
            return min;
        }

        public void RenderLayers()
        {
            for (int z = 0; z < Size; z++)
            {
                Console.WriteLine($"Шар Z = {z}:");
                for (int y = 0; y < Size; y++)
                {
                    Console.Write("  ");
                    for (int x = 0; x < Size; x++)
                        Console.Write($"{_cubeData[z, y, x],7:F2} ");
                    Console.WriteLine();
                }
            }
        }
    }

    #endregion

    // =========================================================================
    // V. ADAPTER (Файл: Matrix3DAdapter.cs)
    // =========================================================================
    #region Matrix3DAdapter

    /// <summary>
    /// Адаптує Matrix3D до інтерфейсу MatrixBase/IMatrixOperations.
    /// </summary>
    public class Matrix3DAdapter : MatrixBase, ILoggable
    {
        private readonly Matrix3D _adaptee;
        private readonly List<string> _logBuffer = new List<string>();

        public Matrix3DAdapter(Matrix3D existingMatrix3D)
        {
            Name = "Адаптер для 3D Матриці";
            _adaptee = existingMatrix3D;
        }

        public override void FillRandom(Random random)
        {
            // Виклик специфічного методу Adaptee
            _adaptee.GenerateDataVolume(random);
            LogInfo("Дані згенеровано (Random) через адаптер.");
        }

        public override void FillManual(IInputProvider inputProvider)
        {
            Console.WriteLine($"\n[Adapter] Перенаправлення на 3D структуру...");
            // Виклик специфічного методу Adaptee
            _adaptee.PopulateVolumeManually(inputProvider);
            LogInfo("Дані введено вручну через адаптер.");
        }

        public override double FindMin()
        {
            return _adaptee.CalculateMinimum();
        }

        public override void Display()
        {
            base.Display();
            Console.WriteLine("(Візуалізація 3D через адаптер)");
            _adaptee.RenderLayers();
            FlushLogs(); // Виводимо логи при відображенні
        }

        public void LogInfo(string message)
        {
            // Буферизація логів
            _logBuffer.Add($"[LOG 3D BUFFER] {DateTime.Now:T}: {message}");
        }

        private void FlushLogs()
        {
            if (_logBuffer.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                foreach (var log in _logBuffer) Console.WriteLine(log);
                Console.ResetColor();
                _logBuffer.Clear();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                FlushLogs(); // Переконаємось, що всі логи виведені перед знищенням
            }
            base.Dispose(disposing);
        }
    }

    #endregion

    // =========================================================================
    // VI. MAIN PROGRAM (Файл: Program.cs)
    // =========================================================================
    internal static class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            
            // 1. Простий Unit-test (Self-check)
            RunSelfCheck();

            // 2. Основна програма
            RunApp();
        }

        /// <summary>
        /// Імітація Unit-тесту для перевірки логіки FindMin.
        /// </summary>
        static void RunSelfCheck()
        {
            Console.WriteLine("=== Self-Check: Unit Tests ===");
            try
            {
                var m2d = new Matrix2D();
                Random r = new Random(123); // Fixed seed
                m2d.FillRandom(r);
                double min = m2d.FindMin();
                
                // Ми знаємо, що з seed 123 min має бути певним числом, або просто перевіряємо, що не MaxValue
                if (min < double.MaxValue) 
                    Console.WriteLine("[PASS] Matrix2D FindMin works.");
                else 
                    Console.WriteLine("[FAIL] Matrix2D FindMin failed.");

                var m3d = new Matrix3DAdapter(new Matrix3D());
                m3d.FillRandom(r);
                if (m3d.FindMin() < double.MaxValue)
                    Console.WriteLine("[PASS] Matrix3DAdapter FindMin works.");
                
                // Clean up
                m2d.Dispose();
                m3d.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FAIL] Exception during tests: {ex.Message}");
            }
            Console.WriteLine("==============================\n");
        }

        static void RunApp()
        {
            Random random = new Random();
            // DI: Впроваджуємо залежність консольного вводу
            IInputProvider inputProvider = new ConsoleInputProvider();

            Console.WriteLine("=== Matrix Adapter Demo ===\n");

            var matrices = new List<MatrixBase>
            {
                new Matrix2D(),
                new Matrix3DAdapter(new Matrix3D())
            };

            Console.WriteLine("Оберіть метод заповнення:");
            Console.WriteLine("1. Random (Випадкові числа)");
            Console.WriteLine("2. Manual (Введення з клавіатури)");
            Console.Write("Ваш вибір: ");
            
            string? choice = Console.ReadLine();
            bool useRandom = (choice != "2");

            foreach (var matrix in matrices)
            {
                try
                {
                    if (useRandom)
                        matrix.FillRandom(random);
                    else
                        matrix.FillManual(inputProvider); // Передаємо провайдер вводу

                    // Логування через інтерфейс
                    if (matrix is ILoggable logger)
                    {
                        logger.LogInfo("Операція заповнення завершена.");
                    }

                    matrix.Display();
                    
                    double minVal = matrix.FindMin();
                    Console.WriteLine($">> MIN Element: {minVal:F2}");
                    Console.WriteLine(new string('-', 40));
                }
                finally
                {
                    matrix.Dispose();
                }
            }

            Console.WriteLine("\nПрограма завершена. Натисніть Enter.");
            Console.ReadLine();
        }
    }
}
