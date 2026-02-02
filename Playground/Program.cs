using BenScr.Math.Parser;

public static class Program
{
    static string Prefix = $"ParserPlayground.exe>";
    const bool DisplayNextLine = true;

    public static void Main(string[] args)
    {
        Console.WriteLine("Calculator");
        Console.WriteLine("----------");

        Prefix = Environment.UserName + ">";

        while (true)
        {
            Console.Write($"{Prefix} ");
            string input = Console.ReadLine();

            string result = Calculator.Evaluate<string>(input);
            string output = $"{Prefix} {input} = {result}";

            if (!DisplayNextLine)
            {
                Console.CursorTop = Math.Max(Console.CursorTop - 1, 0);
                Console.Write(output);

                Console.CursorLeft = 0;
                Console.CursorTop = Math.Max(Console.CursorTop + 1, 0);
            }
            else
            {
                Console.WriteLine(output);
            }
        }
    }
}
