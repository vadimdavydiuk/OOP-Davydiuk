using System;
using System.Collections.Generic;

namespace Lab10Variant5
{
    // =========================================================================
    // ЧАСТИНА 1: ІНТЕРФЕЙС ТА ЙОГО РЕАЛІЗАЦІЇ (ПАТЕРН COMMAND)
    // =========================================================================

    // 1. Інтерфейс
    public interface ICommand
    {
        void Execute();
        void Undo();
    }

    // 2. Реалізація 1: Команда додавання фігури
    public class AddShapeCommand : ICommand
    {
        private readonly string _shapeName;
        private readonly List<string> _canvas;

        public AddShapeCommand(string shapeName, List<string> canvas)
        {
            _shapeName = shapeName;
            _canvas = canvas;
        }

        public void Execute()
        {
            _canvas.Add(_shapeName);
            Console.WriteLine($"[КОМАНДА EXECUTE]: Фігуру '{_shapeName}' додано на полотно.");
        }

        public void Undo()
        {
            if (_canvas.Remove(_shapeName))
            {
                Console.WriteLine($"[КОМАНДА UNDO]: Скасовано додавання фігури '{_shapeName}'.");
            }
        }
    }

    // 3. Реалізація 2: Команда видалення фігури
    public class RemoveShapeCommand : ICommand
    {
        private readonly string _shapeName;
        private readonly List<string> _canvas;
        private bool _wasRemoved;

        public RemoveShapeCommand(string shapeName, List<string> canvas)
        {
            _shapeName = shapeName;
            _canvas = canvas;
        }

        public void Execute()
        {
            _wasRemoved = _canvas.Remove(_shapeName);
            if (_wasRemoved)
            {
                Console.WriteLine($"[КОМАНДА EXECUTE]: Фігуру '{_shapeName}' видалено з полотна.");
            }
            else
            {
                Console.WriteLine($"[КОМАНДА EXECUTE]: Фігуру '{_shapeName}' не знайдено для видалення.");
            }
        }

        public void Undo()
        {
            if (_wasRemoved)
            {
                _canvas.Add(_shapeName);
                Console.WriteLine($"[КОМАНДА UNDO]: Відновлено видалену фігуру '{_shapeName}'.");
            }
        }
    }

    // =========================================================================
    // ЧАСТИНА 2: АБСТРАКТНИЙ КЛАС ТА ПОХІДНІ КЛАСИ (МАЛЬВАННЯ ФІГУР)
    // =========================================================================

    // 4. Абстрактний клас
    public abstract class ShapeDrawer
    {
        public string CurrentColor { get; private set; } = "Black";

        // Конкретний метод (спільна логіка)
        public void SetColor(string color)
        {
            CurrentColor = color;
            Console.WriteLine($"-> Змінено колір пензля на: {CurrentColor}");
        }

        // Абстрактний метод (шаблонний алгоритм, який реалізують похідні класи)
        public abstract void DrawShape(string shapeType);
    }

    // 5. Похідний клас 1: Малювання у Консолі
    public class ConsoleShapeDrawer : ShapeDrawer
    {
        public override void DrawShape(string shapeType)
        {
            Console.WriteLine($"[ConsoleDrawer]: Малювання фігури '{shapeType}' у консолі кольором '{CurrentColor}'...");
            if (shapeType.Equals("Square", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("  [■■■]");
                Console.WriteLine("  [■■■]");
            }
            else
            {
                Console.WriteLine($"  <Консольне представлення для {shapeType}>");
            }
        }
    }

    // 6. Похідний клас 2: Генерація SVG-коду
    public class SvgShapeDrawer : ShapeDrawer
    {
        public override void DrawShape(string shapeType)
        {
            Console.WriteLine($"[SvgDrawer]: Генерація SVG-тегу для фігури '{shapeType}'...");
            string svgTag = shapeType.ToLower() switch
            {
                "circle" => $"  <circle cx=\"50\" cy=\"50\" r=\"40\" fill=\"{CurrentColor.ToLower()}\" />",
                "rectangle" or "square" => $"  <rect width=\"100\" height=\"100\" fill=\"{CurrentColor.ToLower()}\" />",
                _ => $"  <path d=\"...\" fill=\"{CurrentColor.ToLower()}\" />"
            };
            Console.WriteLine(svgTag);
        }
    }

    // =========================================================================
    // ЧАСТИНА 3: ГОЛОВНА ПРОГРАМА (ТОЧКА ВХОДУ)
    // =========================================================================

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("==================================================");
            Console.WriteLine("  ДЕМОНСТРАЦІЯ 1: ІНТЕРФЕЙС ICommand (ПОЛІМОРФІЗМ)");
            Console.WriteLine("==================================================\n");

            List<string> canvasState = new List<string>();

            // Колекція об'єктів типу інтерфейсу ICommand
            List<ICommand> commandHistory = new List<ICommand>
            {
                new AddShapeCommand("Circle", canvasState),
                new AddShapeCommand("Square", canvasState),
                new RemoveShapeCommand("Circle", canvasState)
            };

            Console.WriteLine("--- Виконання команд (Execute) ---");
            foreach (var command in commandHistory)
            {
                command.Execute();
            }

            Console.WriteLine($"\nПоточний стан полотна: [{string.Join(", ", canvasState)}]\n");

            Console.WriteLine("--- Скасування команд у зворотному порядку (Undo) ---");
            for (int i = commandHistory.Count - 1; i >= 0; i--)
            {
                commandHistory[i].Undo();
            }

            Console.WriteLine($"\nСтан полотна після скасування: [{string.Join(", ", canvasState)}]\n");


            Console.WriteLine("==================================================");
            Console.WriteLine("  ДЕМОНСТРАЦІЯ 2: АБСТРАКТНИЙ КЛАС ShapeDrawer");
            Console.WriteLine("==================================================\n");

            // Колекція об'єктів типу абстрактного класу ShapeDrawer
            List<ShapeDrawer> drawers = new List<ShapeDrawer>
            {
                new ConsoleShapeDrawer(),
                new SvgShapeDrawer()
            };

            foreach (var drawer in drawers)
            {
                Console.WriteLine($"--- Робота з {drawer.GetType().Name} ---");
                
                // Виклик конкретного методу базового класу
                drawer.SetColor("Red");

                // Поліморфний виклик абстрактного методу
                drawer.DrawShape("Square");
                drawer.DrawShape("Circle");

                Console.WriteLine();
            }
        }
    }
}