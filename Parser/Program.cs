using Parser;

public class Program
{
    public static void Main(string[] args)
    {
        Evaluator ev = Evaluator.Default();

        int line = 0;

        while (true)
        {
            string input = Console.ReadLine();
        
            Value? value = ParserRuntime.Run(input, ev);
            if (value.HasValue)
            {
                Console.SetCursorPosition(0, line);
                Console.WriteLine(input + " = " + value);
            }
            line++;
        }
    }
}