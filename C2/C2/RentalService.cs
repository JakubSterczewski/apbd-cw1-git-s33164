namespace C2;

public class RentalService
{
    private List<Rental> RentalList = new List<Rental>();

    public void RentEquipment(User user, Equipment equipment)
    {
        if (!equipment.IsAvailable)
        {
            Console.WriteLine("Equipment niedostepny");
            return;
        }
        
        int active = RentalList.Count(r => !r.IsReturned() && r.User == user);
        if (user.maxRentals <= active)
        {
            Console.WriteLine("Maksymalny ilosc rentow");
            return;
        }

        var rental = new Rental(user, equipment, new DateTime(), new DateTime().Add(new TimeSpan(0,1,0)));
        equipment.IsAvailable = false;
        RentalList.Add(rental);
        Console.WriteLine("Wypozyczono");
    }

    public void ReturnEquipment(Equipment equipment)
    {
        Rental? rental = RentalList.Where(r => r.Equipment.Equals(equipment) && !r.IsReturned()).LastOrDefault();

        if (rental != null)
        {
            rental.ReturnDate = new DateOnly();
            rental.Equipment.IsAvailable = true;
            
            int lateDays = (rental.ReturnDate.Value.DayNumber - rental.DueDate.DayNumber);
            
            if (lateDays > 0)
            {
                Console.WriteLine("Po terminie");
                rental.Penalty += lateDays * 10;
            }
        }
        else
        {
            Console.WriteLine("Nie ma takiego renta");
        }
    }
}