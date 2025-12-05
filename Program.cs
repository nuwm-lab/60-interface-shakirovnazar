using System;
using System.Collections.Generic;
using System.Text;

namespace AdapterMatrixLab
{
    // =========================================================================
    // I. INTERFACES
    // =========================================================================

    /// <summary>
    /// Основний інтерфейс для роботи з матрицями.
    /// </summary>
    public interface IMatrixOperations
    {
        void FillRandom(Random random);
        void FillFromConsole();
        double FindMin();
        void Display();
    }

    /// <summary>
    /// Інтерфейс для логування.
    /// </summary>
    public interface ILoggable
    {
        void LogInfo(string message);
    }

    // =========================================================================
    // II. ABSTRACT TARGET
    // =========================================================================

    public abstract class MatrixBase : IMatrixOperations, IDisposable
    {
        public string Name { get; protected set; } = "Matrix";
        private bool _disposed = false;

        public abstract void FillRandom(Random random);
        public abstract void FillFromConsole();
        public abstract double FindMin();

        public virtual void Display()
        {
            Console.WriteLine($"\n--- {Name} ---");
        }

        // --- Pattern Disposable ---
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
                    // Тут звільняємо керовані ресурси (наприклад, списки, потоки)
                    // Для демонстрації можна залишити лог, але ТІЛЬКИ тут, не у фіналізаторі
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine($"[System] Ресурси '{Name}' звільнено коректно.");
                    Console.ResetColor();
                }
                _disposed = true;
            }
        }

        // Фіналізатор видалено, оскільки ми не використовуємо некеровані ресурси (WinAPI тощо).
        // Це відповідає сучасним стандартам C#.
    }

    // =========================================================================
    // III. CONCRETE CLASS (2D Matrix)
    // =========================================================================

    public class Matrix2D : MatrixBase, ILoggable
    {
        private const int Size = 3;
        private readonly double[,] _data;

        public Matrix2D()
        {
            Name = "Матриця 2D [3x3]";
            _data = new double[Size, Size];
        }

        public override void FillRandom(Random random)
        {
            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                    _data[i, j] = Math.Round(random.NextDouble() * 20 - 10, 2);
            
            LogInfo("Заповнено випадковими числами.");
        }

        public override void FillFromConsole()
        {
            Console.WriteLine($"\nВведіть елементи для {Name}:");
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    _data[i, j] = GetUserDouble($"Element [{i},{j}]: ");
                }
            }
            LogInfo("Заповнено вручну.");
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

        public void LogInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[LOG 2D]: {message}");
            Console.ResetColor();
        }

        private double GetUserDouble(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();
                // Додана перевірка на null (на випадок EOF або Ctrl+Z)
                if (input != null && double.TryParse(input, out double result))
                {
                    return result;
                }
                Console.WriteLine("Помилка: введіть коректне число.");
            }
        }
    }

    // =========================================================================
    // IV. ADAPTEE (3D Matrix)
    // =========================================================================

    public class Matrix3D
    {
        private const int Size = 3;
        private readonly double[,,] _cubeData;

        public Matrix3D()
        {
            _cubeData = new double[Size, Size, Size];
        }

        public void FillVolumeRandom(Random random)
        {
            for (int z = 0; z < Size; z++)
                for (int y = 0; y < Size; y++)
                    for (int x = 0; x < Size; x++)
                        _cubeData[z, y, x] = Math.Round(random.NextDouble() * 20 - 10, 2);
        }

        public void FillVolumeManual()
        {
            Console.WriteLine("Заповнення 3D матриці (пошарово)...");
            for (int z = 0; z < Size; z++)
            {
                Console.WriteLine($"--- Шар Z={z} ---");
                for (int y = 0; y < Size; y++)
                {
                    for (int x = 0; x < Size; x++)
                    {
                        bool valid = false;
                        while (!valid)
                        {
                            Console.Write($"Val [z:{z}, y:{y}, x:{x}]: ");
                            string? input = Console.ReadLine();
                            if (input != null && double.TryParse(input, out double val))
                            {
                                _cubeData[z, y, x] = val;
                                valid = true;
                            }
                            else
                            {
                                Console.WriteLine("Число некоректне.");
                            }
                        }
                    }
                }
            }
        }

        public double GetMinFromVolume()
        {
            double min = double.MaxValue;
            foreach (var val in _cubeData)
                if (val < min) min = val;
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
                        Console.Write($"{_cubeData[z, y, x],7:F2} ");
                    Console.WriteLine();
                }
            }
        }
    }

    // =========================================================================
    // V. ADAPTER
    // =========================================================================

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
            _adaptee.FillVolumeRandom(random);
            LogInfo("Заповнено RANDOM через адаптер.");
        }

        public override void FillFromConsole()
        {
            Console.WriteLine($"\n[Adapter] Викликаю ручне заповнення для 3D...");
            _adaptee.FillVolumeManual();
            LogInfo("Заповнено MANUAL через адаптер.");
        }

        public override double FindMin()
        {
            return _adaptee.GetMinFromVolume();
        }

        public override void Display()
        {
            base.Display();
            Console.WriteLine("(Відображення адаптованого 3D об'єкта)");
            _adaptee.ShowLayers();
            PrintBuffer();
        }

        public void LogInfo(string message)
        {
            _logBuffer.Add($"[LOG 3D BUFFER] {DateTime.Now:T}: {message}");
        }

        private void PrintBuffer()
        {
            if (_logBuffer.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                foreach (var log in _logBuffer) Console.WriteLine(log);
                Console.ResetColor();
            }
        }

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

            Console.WriteLine("=== Matrix Adapter Demo (Random & Input) ===\n");

            // Використовуємо типізований List для кращої безпеки типів
            List<MatrixBase> matrices = new List<MatrixBase>
            {
                new Matrix2D(),
                new Matrix3DAdapter(new Matrix3D())
            };

            Console.WriteLine("Оберіть метод заповнення матриць:");
            Console.WriteLine("1. Випадкові числа (Random)");
            Console.WriteLine("2. Введення з клавіатури (Console Input)");
            Console.Write("Ваш вибір: ");
            
            string? choice = Console.ReadLine();
            bool useRandom = (choice != "2"); 

            foreach (var matrix in matrices)
            {
                try
                {
                    // 1. Поліморфне заповнення
                    if (useRandom)
                        matrix.FillRandom(random);
                    else
                        matrix.FillFromConsole();

                    // 2. Демонстрація роботи з інтерфейсом ILoggable
                    if (matrix is ILoggable logger)
                    {
                        logger.LogInfo("Успішна операція (Logged via Interface).");
                    }

                    // 3. Відображення та пошук мінімуму
                    matrix.Display();
                    Console.WriteLine($">> MIN Element: {matrix.FindMin():F2}");
                    Console.WriteLine(new string('-', 40));
                }
                finally
                {
                    // 4. Гарантоване очищення ресурсів
                    matrix.Dispose();
                }
            }

            Console.WriteLine("\nПрограма завершена. Натисніть Enter.");
            Console.ReadLine();
        }
    }
}
