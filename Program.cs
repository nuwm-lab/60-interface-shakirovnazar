using System;
using System.Collections.Generic;
using System.Text;

namespace AdapterMatrixLab
{
    // =========================================================================
    // I. INTERFACES (Виконання вимоги про наявність інтерфейсів)
    // =========================================================================

    /// <summary>
    /// Основний інтерфейс для роботи з матрицями.
    /// </summary>
    public interface IMatrixOperations
    {
        void Fill(Random random);
        double FindMin();
        void Display();
    }

    /// <summary>
    /// Додатковий інтерфейс для демонстрації (Завдання з огляду).
    /// </summary>
    public interface ILoggable
    {
        void LogInfo(string message);
    }

    // =========================================================================
    // II. ABSTRACT TARGET
    // =========================================================================

    /// <summary>
    /// Абстрактний базовий клас. Реалізує інтерфейс та IDisposable.
    /// </summary>
    public abstract class MatrixBase : IMatrixOperations, IDisposable
    {
        public string Name { get; protected set; }
        private bool _disposed = false;

        // Реалізація контракту інтерфейсу IMatrixOperations
        public abstract void Fill(Random random);
        public abstract double FindMin();

        public virtual void Display()
        {
            Console.WriteLine($"\n--- {Name} ---");
        }

        // --- Реалізація IDisposable (Вимога про деструктори/очищення) ---
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Тут звільняємо керовані ресурси (якщо є)
                }
                // Тут звільняємо некеровані ресурси (симуляція)
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"[System] Ресурс '{Name}' звільнено (Dispose).");
                Console.ResetColor();
                _disposed = true;
            }
        }

        // Деструктор (Фіналізатор) - додано на вимогу завдання
        ~MatrixBase()
        {
            Dispose(false);
        }
    }

    // =========================================================================
    // III. CONCRETE CLASS (2D Matrix)
    // =========================================================================

    /// <summary>
    /// Звичайна двовимірна матриця.
    /// Реалізує логування прямо в консоль.
    /// </summary>
    public class Matrix2D : MatrixBase, ILoggable
    {
        private const int Size = 3;
        private readonly double[,] _data;

        public Matrix2D()
        {
            Name = "Матриця 2D [3x3]";
            _data = new double[Size, Size];
        }

        public override void Fill(Random random)
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    _data[i, j] = Math.Round(random.NextDouble() * 20 - 10, 2);
                }
            }
            LogInfo("Матрицю 2D заповнено випадковими числами.");
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
            for (int i = 0; i < Size; i++)
            {
                Console.Write("|");
                for (int j = 0; j < Size; j++)
                {
                    Console.Write($"{_data[i, j],7:F2} ");
                }
                Console.WriteLine("|");
            }
        }

        // Реалізація ILoggable: просте повідомлення
        public void LogInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[LOG 2D]: {message}");
            Console.ResetColor();
        }
    }

    // =========================================================================
    // IV. ADAPTEE (Адаптовуваний клас)
    // =========================================================================

    /// <summary>
    /// Сторонній клас 3D матриці з іншим інтерфейсом.
    /// </summary>
    public class Matrix3D
    {
        private const int Size = 3;
        private readonly double[,,] _cubeData;

        public Matrix3D()
        {
            _cubeData = new double[Size, Size, Size];
        }

        public void FillVolume(Random random)
        {
            for (int z = 0; z < Size; z++)
                for (int y = 0; y < Size; y++)
                    for (int x = 0; x < Size; x++)
                        _cubeData[z, y, x] = Math.Round(random.NextDouble() * 20 - 10, 2);
        }

        public double GetMinFromVolume()
        {
            double min = double.MaxValue;
            foreach (var val in _cubeData)
            {
                if (val < min) min = val;
            }
            return min;
        }

        public void ShowLayers()
        {
            for (int z = 0; z < Size; z++)
            {
                Console.WriteLine($"Шар Z = {z}:");
                for (int y = 0; y < Size; y++)
                {
                    Console.Write("  ");
                    for (int x = 0; x < Size; x++)
                    {
                        Console.Write($"{_cubeData[z, y, x],7:F2} ");
                    }
                    Console.WriteLine();
                }
            }
        }
    }

    // =========================================================================
    // V. ADAPTER
    // =========================================================================

    /// <summary>
    /// Адаптер, що узгоджує Matrix3D з IMatrixOperations.
    /// Реалізує логування у внутрішній буфер (симуляція).
    /// </summary>
    public class Matrix3DAdapter : MatrixBase, ILoggable
    {
        private readonly Matrix3D _adaptee;
        private readonly List<string> _logBuffer = new List<string>(); // Імітація файлу/буфера

        public Matrix3DAdapter(Matrix3D existingMatrix3D)
        {
            Name = "Адаптер для 3D Матриці";
            _adaptee = existingMatrix3D;
        }

        public override void Fill(Random random)
        {
            _adaptee.FillVolume(random);
            LogInfo("3D Об'єм заповнено через адаптер.");
        }

        public override double FindMin()
        {
            return _adaptee.GetMinFromVolume();
        }

        public override void Display()
        {
            base.Display();
            Console.WriteLine("(Відображення через адаптер...)");
            _adaptee.ShowLayers();
            
            // Виводимо накопичені логи, щоб показати, що буфер працює
            PrintBuffer();
        }

        // Реалізація ILoggable: запис у "буфер" замість консолі
        public void LogInfo(string message)
        {
            string logEntry = $"[BUFFER LOG 3D] {DateTime.Now:T}: {message}";
            _logBuffer.Add(logEntry);
        }

        private void PrintBuffer()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("--- Вміст лог-буфера адаптера ---");
            foreach (var log in _logBuffer)
            {
                Console.WriteLine(log);
            }
            Console.ResetColor();
        }

        // Перевизначення Dispose для очищення буфера (демонстрація)
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _logBuffer.Clear();
            }
            base.Dispose(disposing);
        }
    }

    // =========================================================================
    // VI. CLIENT
    // =========================================================================

    internal static class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Random random = new Random();

            Console.WriteLine("=== Лабораторна: Adapter + Interfaces + Disposable ===\n");

            // Використовуємо List<MatrixBase>, щоб показати роботу Dispose у циклі
            var matrices = new List<MatrixBase>
            {
                new Matrix2D(),
                new Matrix3DAdapter(new Matrix3D())
            };

            foreach (var matrix in matrices)
            {
                // 1. Робота через інтерфейс IMatrixOperations (успадкований в MatrixBase)
                matrix.Fill(random);
                matrix.Display();

                double min = matrix.FindMin();
                Console.WriteLine($"\n>> Min: {min:F2}");
                Console.WriteLine(new string('-', 40));

                // 2. Явний виклик Dispose (або можна було використовувати блок using при створенні)
                matrix.Dispose();
                Console.WriteLine();
            }

            Console.WriteLine("Натисніть будь-яку клавішу для завершення...");
            Console.ReadKey();
        }
    }
}
