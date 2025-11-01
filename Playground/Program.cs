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
            string result = input + " = " + Calculator.Evaluate(input);
            Console.WriteLine(result);
            Console.WriteLine();
        }
    }
}
