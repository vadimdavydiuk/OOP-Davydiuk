using System;
using System.Text;

namespace IndependentWork1;

public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        Rectangle rect = new Rectangle(5.0, 5.0);
        Console.WriteLine($"Розміри: {rect.Width}x{rect.Height}");
        Console.WriteLine($"Площа: {rect.CalculateArea()}");
        Console.WriteLine($"Чи квадрат: {(rect.IsSquare() ? "Так" : "Ні")}\n");

        Employee emp = new Employee("Олесь Франко", 25000.0);
        double bonus = emp.CalculateAnnualBonus(10.0);
        Console.WriteLine($"Працівник: {emp.FullName}");
        Console.WriteLine($"Річний бонус (10%): {bonus:N2} грн\n");

        Playlist playlist = new Playlist("Улюблені треки", 2);
        playlist[0] = "Океан Ельзи — Без бою";
        playlist[1] = "Hardkiss — Маяк";

        Console.WriteLine($"Плейлист: {playlist.Title}");
        Console.WriteLine($"Перший трек: {playlist[0]}");
        playlist.PrintAllTracks();
    }
}

public class Rectangle
{
    private double _width;
    private double _height;

    public double Width => _width;
    public double Height => _height;

    public Rectangle(double width, double height)
    {
        _width = width > 0 ? width : 1.0;
        _height = height > 0 ? height : 1.0;
    }

    public double CalculateArea() => _width * _height;
    public bool IsSquare() => _width == _height;
}

public class Employee
{
    private string _fullName;
    private double _monthlySalary;

    public string FullName => _fullName;
    public double MonthlySalary => _monthlySalary;

    public Employee(string fullName, double monthlySalary)
    {
        _fullName = fullName;
        _monthlySalary = monthlySalary >= 0 ? monthlySalary : 0;
    }

    public double CalculateAnnualBonus(double bonusPercent)
    {
        return (_monthlySalary * 12) * (bonusPercent / 100.0);
    }
}

public class Playlist
{
    private string _title;
    private string[] _tracks;

    public string Title => _title;

    public Playlist(string title, int capacity)
    {
        _title = title;
        _tracks = new string[capacity];
    }

    public string this[int index]
    {
        get => (index >= 0 && index < _tracks.Length) ? _tracks[index] : "Невідомий трек";
        set
        {
            if (index >= 0 && index < _tracks.Length)
                _tracks[index] = value;
        }
    }

    public void PrintAllTracks()
    {
        Console.WriteLine("Список треків:");
        for (int i = 0; i < _tracks.Length; i++)
        {
            Console.WriteLine($"  {i + 1}. {_tracks[i]}");
        }
    }
}