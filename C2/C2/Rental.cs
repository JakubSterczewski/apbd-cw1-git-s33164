namespace C2;

public class Rental
{
    private User User {get;set;}
    private Equipment Qquipment {get;set;}
    private DateTime RentDate {get;set;}
    private DateTime DueDate {get;set;}
    private DateTime? ReturnDate {get;set;}

    public bool IsReturned()
    {
        return ReturnDate != null;
    }

    public Rental(User user, Equipment qquipment, DateTime rentDate, DateTime dueDate, DateTime? returnDate)
    {
        User = user;
        Qquipment = qquipment;
        RentDate = rentDate;
        DueDate = dueDate;
        ReturnDate = returnDate;
    }
}