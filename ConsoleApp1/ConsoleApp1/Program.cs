using System.Collections;

public class InputValidator
{
    public static void Main(string[] args)
    {
        List<int> enteredNumbers = new List<int>();
        int enteredNumber = 0;
        while (enteredNumber >= 0)
        {
            enteredNumber = GetNumberFromUser();
            if (enteredNumber >= 0)
            {
                Console.WriteLine("Entered value: " + enteredNumber);
                enteredNumbers.Add(enteredNumber);
            }
            else
            {
                Console.WriteLine("Quit!!!");
            }
        }
        
        Console.WriteLine("Average: " + CalculateAverage(enteredNumbers));

        Console.WriteLine("Min: " + CalculateMin(enteredNumbers));
    }

    public static int CalculateMax(List<int> numbers)
    {
        int max = numbers.Max();
        return max;
    }
    
    public static int CalculateMin(List<int> numbers)
    {
        int min = numbers.Min();
        return min;
    }

    public static int CalculateAverage(List<int> values)
    {
        int sum = 0;
        foreach (var i in values)
        {
            sum += i;
        }
        return sum/values.Count;
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