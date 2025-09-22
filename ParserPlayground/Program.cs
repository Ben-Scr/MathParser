using UtilityCS;
using static UtilityCS.MiniLinq;

namespace ParserPlayground
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            List<int> list = new List<int>() { 1, 2, 3, 4, 5, 921, 21, 15, -41 };
            var values = MiniLinq.Select<int,bool>(list, val => val > 0);
            foreach (var value in values)
            {
                Console.WriteLine(value);
            }
        }
    }
}
