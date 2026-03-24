namespace C2;

public abstract class User
{
    private int  id;
    private string name;
    private string email;
    
    public abstract int maxRentals { get; }
}