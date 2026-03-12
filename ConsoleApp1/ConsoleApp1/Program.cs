using System.Collections;

public class InputValidator
{
    public static void Main(string[] args)
    {
        ArrayList enteredNumbers = new ArrayList();
        int enteredNumber = 0;
        while (enteredNumber >= 0)
        {
            enteredNumber = GetNumberFromUser();
            if (enteredNumber >= 0)
            {
                Console.Write("Entered value: " + enteredNumber + "\n");
                enteredNumbers.Add(enteredNumber);
            }
            else
            {
                Console.Write("Quit");
            }
        }
    }

    public static int GetNumberFromUser()
    {
        while (true)
        {
            Console.Write("Please enter a number (q to quit): ");
            string input = Console.ReadLine();

            if (input == "q")
            {
                return -1;
            }

            try
            {
                int enteredNumber = int.Parse(input);
                if (enteredNumber >= 0)
                {
                    return enteredNumber;
                }

                Console.WriteLine("Number cannot be negative");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}