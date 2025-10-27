
namespace BenScr.Math.Parser
{
    public static class Calculator
    {
        private static Evaluator evaluator = Evaluator.Calculator();

        public static object Evaluate(string calculation)
        {
            return ParserRuntime.Run(calculation, evaluator).Object;
        }
        public static T Evaluate<T>(string calculation)
        {
            return ParserRuntime.Run(calculation, evaluator).To<T>();
        }
    }
}
