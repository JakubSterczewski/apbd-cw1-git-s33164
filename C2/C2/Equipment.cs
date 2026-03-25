namespace C2;

public abstract class Equipment
{
    public int Id {get;}
    public string Producer { get; set; }
    public string Name { get; set; }
    public bool IsAvailable { get; set; } = true;
    
    public Equipment(string producer, string name)
    {
        Id = Guid.NewGuid().GetHashCode();
        Producer = producer;
        Name = name;
    }
}