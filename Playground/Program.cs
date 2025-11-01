using BenScr.Math.Parser;

public static class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("-- Calculator --");
        Console.WriteLine("Enter your calculation:");

        int resultLength = 30;

        while (true)
        {
            for (int i = 0; i < resultLength; i++)
                Console.Write("-");
            Console.WriteLine();
            string input = Console.ReadLine();
            string result = input + " = " + Calculator.Evaluate(input);
            resultLength = result.Length;
            Console.WriteLine(result);
            Console.WriteLine();
        }
    }
}
