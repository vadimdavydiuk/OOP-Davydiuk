using System;
using System.Collections.Generic;

namespace Lab8V5
{
    // Базовий клас Animal
    public class Animal
    {
        public string Name { get; set; }

        public Animal(string name)
        {
            Name = name;
        }

        // Віртуальний метод для видачі звуку
        public virtual string MakeSound()
        {
            return $"{Name} видає невизначений звук.";
        }
    }

    // Похідний клас Lion
    public class Lion : Animal
    {
        public double ManeSize { get; set; } // Розмір гриви у см

        public Lion(string name, double maneSize) : base(name)
        {
            ManeSize = maneSize;
        }

        public override string MakeSound()
        {
            return $"Лев {Name} (грива {ManeSize} см) ричить: ROAR!";
        }
    }

    // Похідний клас Elephant
    public class Elephant : Animal
    {
        public double TrunkLength { get; set; } // Довжина хобота у м

        public Elephant(string name, double trunkLength) : base(name)
        {
            TrunkLength = trunkLength;
        }

        public override string MakeSound()
        {
            return $"Слон {Name} (хобот {TrunkLength} м) трубить: TRUMPET!";
        }
    }

    // Похідний клас Bird
    public class Bird : Animal
    {
        public double WingSpan { get; set; } // Розмах крил у см

        public Bird(string name, double wingSpan) : base(name)
        {
            WingSpan = wingSpan;
        }

        public override string MakeSound()
        {
            return $"Птах {Name} (розмах крил {WingSpan} см) щебече: CHIRP!";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // 1. Створення колекції об'єктів базового типу List<Animal>
            List<Animal> zoo = new List<Animal>
            {
                new Lion("Сімба", 25.5),
                new Elephant("Джамбо", 2.1),
                new Bird("Кеша", 35.0),
                new Lion("Муфаса", 30.0),
                new Bird("Твіті", 18.5)
            };

            // Список для агрегації звуків
            List<string> soundsLog = new List<string>();

            // 2. Виклики поліморфного методу
            Console.WriteLine("--- Звуки тварин ---");
            foreach (Animal animal in zoo)
            {
                string sound = animal.MakeSound();
                Console.WriteLine(sound);

                // Агрегація результатів
                soundsLog.Add(sound);
            }

            // 3. Вивід агрегованих результатів
            Console.WriteLine("\n--- Підсумковий список ---");
            Console.WriteLine($"Всього тварин: {zoo.Count}");
            Console.WriteLine($"Всього записано звуків: {soundsLog.Count}\n");

            for (int i = 0; i < soundsLog.Count; i++)
            {
                Console.WriteLine($" {i + 1}. {soundsLog[i]}");
            }
        }
    }
}