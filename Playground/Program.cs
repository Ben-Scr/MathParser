using BenScr.Math.Parser;

public static class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("-- Calculator --");
        Console.WriteLine("Enter your calculation:");


        while (true)
        {
            Console.Write("------------------------------------");
            string input = Console.ReadLine();
            Console.WriteLine(input + " = " + Calculator.Evaluate(input));
        }
    }
}
