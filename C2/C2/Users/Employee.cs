namespace C2;

public class Employee : User
{
    public override int maxRentals => 5;
    
    public Employee(string name, string email) : base(name, email)
    {
    }
}