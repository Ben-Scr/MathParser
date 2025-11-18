using BenScr.Math.Parser;

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
            double result = Calculator.Evaluate(input);
            Console.WriteLine(input + " = " + result);
        }
    }
}
