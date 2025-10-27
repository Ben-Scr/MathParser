using MathParser;

namespace ParserPlayground
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("-- Calculator --");
            Evaluator evaluator = Evaluator.Calculator();

            Console.WriteLine("Enter your calculation:");
            while (true)
            {
                Console.WriteLine("--------------------------------------");
                string input = Console.ReadLine();
                Console.WriteLine(input + " = " + ParserRuntime.Run(input, evaluator));
            }
        }
    }
}
