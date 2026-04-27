using BenScr.Math.Parser;
using System.Text;

public static class Program
{
    private static readonly string Prefix = Environment.UserName + ">";

    public static void Main(string[] args)
    {
        Evaluator evaluator = Evaluator.CreateCalculator();

        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("Calculator");
        Console.WriteLine("----------");

        while (true)
        {
            Console.Write($"{Prefix} ");
            string input = Console.ReadLine();

            Value result = ParserRuntime.Run(input, evaluator);
            Console.WriteLine($"{Prefix} {input} = {result}");
        }
    }
}
