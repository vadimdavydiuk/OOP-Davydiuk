using System;
using System.Linq;

namespace Lab5V5
{
    // Клас BitCollection (Масив бітів)
    public class BitCollection
    {
        private bool[] _bits;

        // Властивість для отримання кількості бітів
        public int Count => _bits.Length;

        // Конструктор за замовчуванням / заданим розміром
        public BitCollection(int size)
        {
            if (size <= 0)
                throw new ArgumentException("Розмір має бути більше нуля.", nameof(size));

            _bits = new bool[size];
        }

        // Конструктор на основі масиву bool
        public BitCollection(bool[] bits)
        {
            if (bits == null)
                throw new ArgumentNullException(nameof(bits));

            _bits = (bool[])bits.Clone();
        }

        // 1. ІНДЕКСАТОР
        public bool this[int index]
        {
            get
            {
                ValidateIndex(index);
                return _bits[index];
            }
            set
            {
                ValidateIndex(index);
                _bits[index] = value;
            }
        }

        // Встановлення всіх бітів у єдине значення
        public void SetAll(bool value)
        {
            for (int i = 0; i < _bits.Length; i++)
            {
                _bits[i] = value;
            }
        }

        private void ValidateIndex(int index)
        {
            if (index < 0 || index >= _bits.Length)
                throw new IndexOutOfRangeException($"Індекс {index} вийшов за межі колекції [0..{_bits.Length - 1}].");
        }

        // 2. ПЕРЕВАНТАЖЕННЯ ОПЕРАТОРІВ

        // Побітове І (&)
        public static BitCollection operator &(BitCollection a, BitCollection b)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Об'єкти не можуть бути null.");

            int minLength = Math.Min(a.Count, b.Count);
            BitCollection result = new BitCollection(minLength);

            for (int i = 0; i < minLength; i++)
            {
                result[i] = a[i] & b[i];
            }

            return result;
        }

        // Побітове АБО (|)
        public static BitCollection operator |(BitCollection a, BitCollection b)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Об'єкти не можуть бути null.");

            int maxLength = Math.Max(a.Count, b.Count);
            BitCollection result = new BitCollection(maxLength);

            for (int i = 0; i < maxLength; i++)
            {
                bool valA = i < a.Count ? a[i] : false;
                bool valB = i < b.Count ? b[i] : false;
                result[i] = valA | valB;
            }

            return result;
        }

        // Оператори порівняння == та !=
        public static bool operator ==(BitCollection a, BitCollection b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false;

            return a.Equals(b);
        }

        public static bool operator !=(BitCollection a, BitCollection b)
        {
            return !(a == b);
        }

        // 3. ПЕРЕВИЗНАЧЕННЯ СТАНДАРТНИХ МЕТОДІВ
        public override bool Equals(object obj)
        {
            if (obj is BitCollection other)
            {
                if (Count != other.Count) return false;
                return _bits.SequenceEqual(other._bits);
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            foreach (bool bit in _bits)
            {
                hash = hash * 31 + bit.GetHashCode();
            }
            return hash;
        }

        public override string ToString()
        {
            char[] chars = new char[_bits.Length];
            for (int i = 0; i < _bits.Length; i++)
            {
                chars[i] = _bits[i] ? '1' : '0';
            }
            return new string(chars);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // 1. Створення об'єктів
            BitCollection bits1 = new BitCollection(new bool[] { true, false, true, true, false });
            BitCollection bits2 = new BitCollection(new bool[] { true, true, false, true, false });

            Console.WriteLine("--- Початкові бітові колекції ---");
            Console.WriteLine($"Колекція 1: {bits1}");
            Console.WriteLine($"Колекція 2: {bits2}");

            // 2. Демонстрація індексатора (читання та запис)
            Console.WriteLine("\n--- Робота з індексатором ---");
            Console.WriteLine($"bits1[0] (читання): {bits1[0]}");
            Console.WriteLine($"bits1[1] (читання): {bits1[1]}");

            Console.WriteLine("Змінюємо bits1[1] на true...");
            bits1[1] = true;
            Console.WriteLine($"bits1 після зміни: {bits1}");

            // Повертаємо початковий стан для коректності наступних тестів
            bits1[1] = false;

            // 3. Демонстрація перевантажених операторів
            Console.WriteLine("\n--- Перевантажені бітові оператори ---");
            BitCollection andResult = bits1 & bits2;
            BitCollection orResult = bits1 | bits2;

            Console.WriteLine($"bits1 & bits2 (Побітове І):   {andResult}");
            Console.WriteLine($"bits1 | bits2 (Побітове АБО): {orResult}");

            // 4. Демонстрація операторів порівняння == та !=
            Console.WriteLine("\n--- Перевантажені оператори порівняння ---");
            BitCollection bits3 = new BitCollection(new bool[] { true, false, true, true, false });

            Console.WriteLine($"bits1 == bits2: {bits1 == bits2}");
            Console.WriteLine($"bits1 != bits2: {bits1 != bits2}");
            Console.WriteLine($"bits1 == bits3 (однаковий вміст): {bits1 == bits3}");

            // 5. Додаткові методи
            Console.WriteLine("\n--- Метод SetAll() ---");
            BitCollection bits4 = new BitCollection(4);
            Console.WriteLine($"Нова порожня колекція (4 біти): {bits4}");
            bits4.SetAll(true);
            Console.WriteLine($"Після SetAll(true):             {bits4}");
        }
    }
}