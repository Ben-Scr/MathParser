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
        WriteHeader();

        while (true)
        {
            Console.Write($"{Prefix} ");
            string? input = Console.ReadLine();
            if (input == null)
                break;

            input = input.Trim();
            if (input.Length == 0)
                continue;

            if (TryHandleCommand(input, out bool shouldExit))
            {
                if (shouldExit)
                    return;

                continue;
            }

            Value result = ParserRuntime.Run(input, evaluator);
            Console.WriteLine($"{Prefix} {input} = {result}");
        }
    }

    private static bool TryHandleCommand(string input, out bool shouldExit)
    {
        shouldExit = false;

        switch (input.ToLowerInvariant())
        {
            case "clear":
                Console.Clear();
                WriteHeader();
                return true;
            case "quit":
            case "exit":
                shouldExit = true;
                return true;
            default:
                return false;
        }
    }

    private static void WriteHeader()
    {
        Console.WriteLine("Calculator");
        Console.WriteLine("----------");
    }
}
