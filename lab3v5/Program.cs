using System;

namespace Lab3v5
{
    public class GraphicsContext : IDisposable
    {
        private bool _disposed = false;
        private bool _isContextCreated;
        private readonly int _contextId;

        public int ContextId => _contextId;
        public bool IsContextCreated => _isContextCreated;

        public GraphicsContext(int contextId)
        {
            _contextId = contextId;
            _isContextCreated = true;
            Console.WriteLine($"[GraphicsContext {_contextId}] Контекст створено.");
        }

        public void DrawShape(string shape)
        {
            if (_disposed || !_isContextCreated)
            {
                throw new ObjectDisposedException(nameof(GraphicsContext), $"Контекст {_contextId} знищено.");
            }
            Console.WriteLine($"[GraphicsContext {_contextId}] Малювання фігури: {shape}");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Console.WriteLine($"[GraphicsContext {_contextId}] Звільнення керованих ресурсів.");
                }

                if (_isContextCreated)
                {
                    Console.WriteLine($"[GraphicsContext {_contextId}] Знищення графічного контексту.");
                    _isContextCreated = false;
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~GraphicsContext()
        {
            Console.WriteLine($"[GraphicsContext {_contextId}] Виклик деструктора.");
            Dispose(false);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Сценарій 1: using ===");
            using (var context1 = new GraphicsContext(101))
            {
                context1.DrawShape("Коло");
            }

            Console.WriteLine("\n=== Сценарій 2: Явний Dispose() ===");
            var context2 = new GraphicsContext(102);
            context2.DrawShape("Квадрат");
            context2.Dispose();

            Console.WriteLine("\n=== Сценарій 3: GC.Collect() ===");
            CreateAndAbandonObject();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.WriteLine("\nЗавершення роботи.");
        }

        static void CreateAndAbandonObject()
        {
            var context3 = new GraphicsContext(103);
            context3.DrawShape("Трикутник");
        }
    }
}