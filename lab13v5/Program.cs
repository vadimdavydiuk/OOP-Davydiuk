using System;
using System.Collections.Generic;

namespace lab13v5
{
    // Клас для демонстрації роботи з власними класами
    public class Student
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public double Grade { get; set; }

        public Student(string name, int age, double grade)
        {
            Name = name;
            Age = age;
            Grade = grade;
        }

        public override string ToString() => $"{Name} (Вік: {Age}, Середній бал: {Grade})";
    }

    public static class FunctionalUtils
    {
        // 1. Map: Перетворює колекцію List<T> у List<TResult> за допомогою функції перетворення
        public static List<TResult> Map<T, TResult>(List<T> list, Func<T, TResult> mapper)
        {
            var result = new List<TResult>();
            foreach (var item in list)
            {
                result.Add(mapper(item));
            }
            return result;
        }

        // 2. Reduce: Згортає колекцію до одного значення за допомогою акумулятора
        public static TAccumulate Reduce<T, TAccumulate>(List<T> list, TAccumulate seed, Func<TAccumulate, T, TAccumulate> accumulator)
        {
            TAccumulate current = seed;
            foreach (var item in list)
            {
                current = accumulator(current, item);
            }
            return current;
        }

        // 3. ForEach: Виконує дію Action<T> для кожного елемента колекції
        public static void ForEach<T>(List<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
            }
        }

        // 4. All: Перевіряє, чи всі елементи колекції задовольняють умову (predicate)
        public static bool All<T>(List<T> list, Predicate<T> predicate)
        {
            foreach (var item in list)
            {
                if (!predicate(item))
                {
                    return false;
                }
            }
            return true;
        }

        // 5. FirstOrDefault: Повертає перший елемент, що задовольняє умову, або default(T)
        public static T FirstOrDefault<T>(List<T> list, Predicate<T> predicate)
        {
            foreach (var item in list)
            {
                if (predicate(item))
                {
                    return item;
                }
            }
            return default;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== ЛАБОРАТОРНА РОБОТА №13 (Варіант 5) ===");
            Console.WriteLine("Тема: Generic-методи та делегати Func/Action/Predicate\n");

            // --- ТЕСТУВАННЯ 1: Тип int ---
            Console.WriteLine("--- 1. Тестування на колекції цілих чисел (int) ---");
            List<int> numbers = new List<int> { 2, 4, 6, 8, 10 };
            
            // ForEach
            Console.Write("Елементи списку: ");
            FunctionalUtils.ForEach(numbers, n => Console.Write($"{n} "));
            Console.WriteLine();

            // Map (множимо на 3)
            var multiplied = FunctionalUtils.Map(numbers, n => n * 3);
            Console.WriteLine($"Map (помножити на 3): {string.Join(", ", multiplied)}");

            // Reduce (сума елементів)
            int sum = FunctionalUtils.Reduce(numbers, 0, (acc, n) => acc + n);
            Console.WriteLine($"Reduce (сума): {sum}");

            // All (чи всі парні?)
            bool allEven = FunctionalUtils.All(numbers, n => n % 2 == 0);
            Console.WriteLine($"All (чи всі парні?): {allEven}");

            // FirstOrDefault (перше число більше 5)
            int firstBig = FunctionalUtils.FirstOrDefault(numbers, n => n > 5);
            Console.WriteLine($"FirstOrDefault (перше > 5): {firstBig}\n");


            // --- ТЕСТУВАННЯ 2: Тип string ---
            Console.WriteLine("--- 2. Тестування на колекції рядків (string) ---");
            List<string> words = new List<string> { "C#", "Python", "JavaScript", "C++" };

            // ForEach
            Console.WriteLine("Виведення через ForEach:");
            FunctionalUtils.ForEach(words, w => Console.WriteLine($" - Мова програмування: {w}"));

            // Map (перетворення в довжину рядка)
            var lengths = FunctionalUtils.Map(words, w => w.Length);
            Console.WriteLine($"Map (довжини рядків): {string.Join(", ", lengths)}");

            // Reduce (конкатенація рядків)
            string combined = FunctionalUtils.Reduce(words, "", (acc, w) => acc + (acc == "" ? "" : ", ") + w);
            Console.WriteLine($"Reduce (об'єднані рядки): {combined}");

            // All (чи всі довші за 1 символ?)
            bool allLongerThanOne = FunctionalUtils.All(words, w => w.Length > 1);
            Console.WriteLine($"All (чи довші за 1 символ?): {allLongerThanOne}");

            // FirstOrDefault (перше слово, що починається на 'J')
            string firstJ = FunctionalUtils.FirstOrDefault(words, w => w.StartsWith("J"));
            Console.WriteLine($"FirstOrDefault (починається на 'J'): {firstJ}\n");


            // --- ТЕСТУВАННЯ 3: Власний клас (Student) ---
            Console.WriteLine("--- 3. Тестування на власному класі (Student) ---");
            List<Student> students = new List<Student>
            {
                new Student("Олександр", 20, 85.5),
                new Student("Марія", 19, 92.0),
                new Student("Іван", 21, 78.4),
                new Student("Софія", 20, 95.2)
            };

            // ForEach (вивести студентів)
            Console.WriteLine("Список студентів:");
            FunctionalUtils.ForEach(students, s => Console.WriteLine($"   * {s}"));

            // Map (витягнути лише імена)
            var names = FunctionalUtils.Map(students, s => s.Name);
            Console.WriteLine($"\nMap (тільки імена): {string.Join(", ", names)}");

            // Reduce (знайти загальну суму балів)
            double totalGrade = FunctionalUtils.Reduce(students, 0.0, (acc, s) => acc + s.Grade);
            Console.WriteLine($"Reduce (сума всіх середніх балів): {totalGrade:F2}");

            // All (чи всі повнолітні, age >= 18?)
            bool allAdults = FunctionalUtils.All(students, s => s.Age >= 18);
            Console.WriteLine($"All (чи всі студенти повнолітні?): {allAdults}");

            // FirstOrDefault (знайти першого студента з балом > 90)
            Student topStudent = FunctionalUtils.FirstOrDefault(students, s => s.Grade > 90.0);
            Console.WriteLine($"FirstOrDefault (перший з балом > 90): {topStudent?.Name} ({topStudent?.Grade})");
        }
    }
}