namespace C2;

public abstract class User
{
    private static int maxId = 0;
    public int id { get; set; }
    public string name { get; set; }
    public string email { get; set; }
    
    public abstract int maxRentals { get; }

    protected User(string name, string email)
    {
        id = maxId++;
        this.name = name;
        this.email = email;
    }
}