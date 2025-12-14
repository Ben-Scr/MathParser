using BenScr.Math.Parser;

public static class Program
{
    const string Prefix = $"ParserPlayground.exe>";

    public static void Main(string[] args)
    {
        Console.WriteLine("Calculator");
        Console.WriteLine("----------");

        while (true)
        {
            Console.Write($"{Prefix} ");              // Prompt anzeigen
            string input = Console.ReadLine();

            string result = Calculator.Evaluate<string>(input);

            Console.CursorTop = Math.Clamp(Console.CursorTop - 1, 0, int.MaxValue);
            Console.Write($"{Prefix} {input} = {result}");

            Console.CursorLeft = 0;
            Console.CursorTop = Math.Clamp(Console.CursorTop + 1, 0, int.MaxValue);
        }
    }
}
