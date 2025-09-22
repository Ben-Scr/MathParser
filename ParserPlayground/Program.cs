using UtilityCS;
using static UtilityCS.MiniLinq;

namespace ParserPlayground
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            SaveManager.Save("highscore", 1000);
            SaveManager.Save("position2D", new Vector2(5,5));
            SaveManager.Save("position3D", new Vector3(5, 115));

            Console.WriteLine(SaveManager.Load<int>("highscore"));
            Console.WriteLine(SaveManager.Load<Vector2>("position2D"));
            Console.WriteLine(SaveManager.Load<Vector3>("position3D"));
            Console.WriteLine(SaveManager.MainPath);


            List<int> list = new List<int>() { 1, 2, 3, 4, 5, 921, 21, 15, -41 };
            var values = MiniLinq.Select<int,bool>(list, val => val > 0);
            foreach (var value in values)
            {
                Console.WriteLine(value);
            }
        }
    }
}
