class City
{
    private string name;
    private string country;
    private int population;

    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    public string Country
    {
        get { return country; }
        set { country = value; }
    }

    public int Population
    {
        get { return population; }
        set { population = value; }
    }

    public City(string name, string country, int population)
    {
        this.name = name;
        this.country = country;
        this.population = population;
    }

    public string GetInfo()
    {
        return $"Місто: {Name}, Країна: {Country}, Населення: {Population}";
    }
}

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        City city1 = new City("Київ", "Україна", 2950000);
        City city2 = new City("Львів", "Україна", 720000);
        City city3 = new City("Одеса", "Україна", 1010000);

        Console.WriteLine(city1.GetInfo());
        Console.WriteLine(city2.GetInfo());
        Console.WriteLine(city3.GetInfo());
    }
}
