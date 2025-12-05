using System;
using System.Text;

namespace AdapterMatrixLab
{
    // =========================================================================
    // 1. Target (Ціль)
    // Абстрактний клас, який визначає інтерфейс для клієнтського коду.
    // Клієнт (Main) вміє працювати тільки з цим типом.
    // =========================================================================
    public abstract class MatrixBase
    {
        public string Name { get; protected set; }

        // Абстрактні методи, які мають бути у всіх "сумісних" матриць
        public abstract void Fill(Random rnd);
        public abstract double FindMin();
        public virtual void Display()
        {
            Console.WriteLine($"\n--- {Name} ---");
        }
    }

    // =========================================================================
    // Конкретна реалізація Target
    // Звичайна двовимірна матриця 3x3
    // =========================================================================
    public class Matrix2D : MatrixBase
    {
        // Константи розмірності
        private const int Size = 3;
        
        // Приватне поле (інкапсуляція)
        private readonly double[,] _data;

        public Matrix2D()
        {
            Name = "Матриця 2D [3x3]";
            _data = new double[Size, Size];
        }

        public override void Fill(Random rnd)
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    // Випадкові числа від -10.0 до 10.0
                    _data[i, j] = Math.Round(rnd.NextDouble() * 20 - 10, 2);
                }
            }
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
    }

    // =========================================================================
    // 2. Adaptee (Адаптовуваний клас)
    // Тривимірна матриця 3x3x3.
    // Вона має іншу структуру даних та інші назви методів.
    // =========================================================================
    public class Matrix3D
    {
        private const int Size = 3;
        private readonly double[,,] _cubeData;

        public Matrix3D()
        {
            _cubeData = new double[Size, Size, Size];
        }

        // Метод має іншу назву, ніж у Target (FillVolume замість Fill)
        public void FillVolume(Random rnd)
        {
            for (int z = 0; z < Size; z++)
                for (int y = 0; y < Size; y++)
                    for (int x = 0; x < Size; x++)
                        _cubeData[z, y, x] = Math.Round(rnd.NextDouble() * 20 - 10, 2);
            
            Console.WriteLine("[Matrix3D System] Об'єм заповнено.");
        }

        // Специфічний метод пошуку мінімуму в 3D
        public double GetMinFromVolume()
        {
            double min = double.MaxValue;
            foreach (var val in _cubeData)
            {
                if (val < min) min = val;
            }
            return min;
        }

        // Специфічний метод виводу (пошарово)
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
    // 3. Adapter (Адаптер)
    // Дозволяє використовувати Matrix3D там, де очікується MatrixBase.
    // =========================================================================
    public class Matrix3DAdapter : MatrixBase
    {
        // Посилання на адаптовуваний об'єкт
        private readonly Matrix3D _adaptee;

        public Matrix3DAdapter(Matrix3D existingMatrix3D)
        {
            Name = "Адаптер для 3D Матриці";
            _adaptee = existingMatrix3D;
        }

        // Адаптуємо метод Fill -> FillVolume
        public override void Fill(Random rnd)
        {
            _adaptee.FillVolume(rnd);
        }

        // Адаптуємо метод FindMin -> GetMinFromVolume
        public override double FindMin()
        {
            return _adaptee.GetMinFromVolume();
        }

        // Адаптуємо Display -> ShowLayers
        public override void Display()
        {
            base.Display();
            Console.WriteLine("(Відображення через адаптер...)");
            _adaptee.ShowLayers();
        }
    }

    // =========================================================================
    // 4. Client (Головна програма)
    // =========================================================================
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Random rnd = new Random();

            Console.WriteLine("=== Лабораторна робота №6: Патерн Адаптер ===");
            Console.WriteLine("Демонстрація роботи з 2D та 3D матрицями через єдиний інтерфейс.\n");

            // Клієнтський код працює з абстракцією MatrixBase
            MatrixBase[] matrices = new MatrixBase[2];

            // 1. Створюємо стандартну 2D матрицю
            matrices[0] = new Matrix2D();

            // 2. Створюємо 3D матрицю і обгортаємо її в Адаптер
            Matrix3D hugeMatrix = new Matrix3D();
            matrices[1] = new Matrix3DAdapter(hugeMatrix);

            // Основний цикл обробки (поліморфізм + адаптер)
            foreach (var matrix in matrices)
            {
                // Виклик Fill() для 3D матриці пройде через Адаптер і викличе FillVolume()
                matrix.Fill(rnd);
                
                matrix.Display();

                // Виклик FindMin() для 3D матриці пройде через Адаптер і викличе GetMinFromVolume()
                double min = matrix.FindMin();
                Console.WriteLine($"\n>> МІНІМАЛЬНИЙ ЕЛЕМЕНТ: {min:F2}");
                Console.WriteLine(new string('-', 40));
            }

            Console.WriteLine("\nРоботу завершено.");
            Console.ReadKey();
        }
    }
}
