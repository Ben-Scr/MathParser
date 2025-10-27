using BenScr.MathParser;

public static class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("-- Calculator --");
        Console.WriteLine("Enter your calculation:");
        while (true)
        {
            Console.WriteLine("--------------------------------------");
            string input = Console.ReadLine();
            Console.WriteLine(input + " = " + Calculator.Evaluate(input));
        }
    }
}
