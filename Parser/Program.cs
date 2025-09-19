using Parser;

public class Program
{
    public static void Main(string[] args)
    {
        var ev = new Evaluator();
        ev.Define("max", a => Math.Max(a[0], a[1]));
        ev.Define("min", a => Math.Min(a[0], a[1]));
        ev.Define("pow", a => Math.Pow(a[0], a[1]));
        ev.Define("square", a => a[0] * a[0]);
        Console.WriteLine(MiniLangRuntime.Run("1+2*3", ev));
        Console.WriteLine(MiniLangRuntime.Run("(1+2)*3", ev));
        Console.WriteLine(MiniLangRuntime.Run("max(2, square(3+1))", ev));

        while (true){
            string input = Console.ReadLine();
            Console.WriteLine(input + " = " + MiniLangRuntime.Run(input, ev));
        }
    }
}