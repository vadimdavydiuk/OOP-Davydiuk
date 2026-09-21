using System;

namespace Lab4Variant5
{
    public class Vector3D : IEquatable<Vector3D>
    {
        private double _x;
        private double _y;
        private double _z;

        private const double Epsilon = 1e-9;

        public double X
        {
            get => _x;
            set
            {
                ValidateValue(value, nameof(X));
                _x = value;
            }
        }

        public double Y
        {
            get => _y;
            set
            {
                ValidateValue(value, nameof(Y));
                _y = value;
            }
        }

        public double Z
        {
            get => _z;
            set
            {
                ValidateValue(value, nameof(Z));
                _z = value;
            }
        }

        public static Vector3D Zero => new Vector3D(0.0, 0.0, 0.0);

        public Vector3D() : this(0.0, 0.0, 0.0) { }

        public Vector3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double this[int index]
        {
            get => index switch
            {
                0 => X,
                1 => Y,
                2 => Z,
                _ => throw new IndexOutOfRangeException("Індекс має бути від 0 до 2.")
            };
            set
            {
                switch (index)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    case 2: Z = value; break;
                    default:
                        throw new IndexOutOfRangeException("Індекс має бути від 0 до 2.");
                }
            }
        }

        public static Vector3D operator +(Vector3D a, Vector3D b)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            if (b is null) throw new ArgumentNullException(nameof(b));
            return new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static double operator *(Vector3D a, Vector3D b)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            if (b is null) throw new ArgumentNullException(nameof(b));
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        public static bool operator ==(Vector3D? left, Vector3D? right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(Vector3D? left, Vector3D? right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return $"Vector3D({X:F2}, {Y:F2}, {Z:F2})";
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Vector3D);
        }

        public bool Equals(Vector3D? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return Math.Abs(X - other.X) < Epsilon &&
                   Math.Abs(Y - other.Y) < Epsilon &&
                   Math.Abs(Z - other.Z) < Epsilon;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Math.Round(X, 6), Math.Round(Y, 6), Math.Round(Z, 6));
        }

        private static void ValidateValue(double value, string propertyName)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                throw new ArgumentException($"Значення {propertyName} не може бути NaN або нескінченністю.");
            }
        }
    }

    internal class Program
    {
        private static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Лабораторна робота №4 | Варіант 5: Vector3D ===\n");

            Vector3D v1 = new Vector3D(1.5, 2.0, 3.0);
            Vector3D v2 = new Vector3D(4.0, -1.0, 0.5);

            Console.WriteLine($"Вектор v1: {v1}");
            Console.WriteLine($"Вектор v2: {v2}");

            Vector3D zeroVector = Vector3D.Zero;
            Console.WriteLine($"\n[Статичний член] Нульовий вектор (Vector3D.Zero): {zeroVector}");

            Console.WriteLine("\n[Індексатор] Читання компонентів v1:");
            Console.WriteLine($"v1[0] (X): {v1[0]}, v1[1] (Y): {v1[1]}, v1[2] (Z): {v1[2]}");

            v1[0] = 5.0;
            Console.WriteLine($"v1 після зміни v1[0] = 5.0: {v1}");

            Vector3D sum = v1 + v2;
            Console.WriteLine($"\n[Оператор +] v1 + v2 = {sum}");

            double dotProduct = v1 * v2;
            Console.WriteLine($"[Оператор *] Скалярний добуток (v1 * v2) = {dotProduct}");

            Vector3D v3 = new Vector3D(5.0, 2.0, 3.0);
            Console.WriteLine($"\n[Оператор ==] v1 == v3: {v1 == v3}");
            Console.WriteLine($"[Оператор !=] v1 != v2: {v1 != v2}");
            Console.WriteLine($"[Метод Equals] v1.Equals(v3): {v1.Equals(v3)}");

            Console.WriteLine("\n[Валідація] Спроба присвоїти double.NaN у властивість X:");
            try
            {
                v1.X = double.NaN;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Помилка валідації успішно перехоплена: {ex.Message}");
            }
        }
    }
}