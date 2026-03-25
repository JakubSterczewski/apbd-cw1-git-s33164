namespace C2;

public class Student : User
{
    public override int maxRentals => 2;

    public Student(string name, string email) : base(name, email)
    {
    }
}