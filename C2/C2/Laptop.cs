namespace C2;

public class Laptop : Equipment
{
    private int Ram { get; set; }
    private string Cpu { get; set; }

    public Laptop(string producer, string name, int ram, string cpu) : base(producer, name)
    {
        Ram = ram;
        Cpu = cpu;
    }
}