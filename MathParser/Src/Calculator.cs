
namespace BenScr.Math.Parser
{
    public static class Calculator
    {
        private static Evaluator evaluator = Evaluator.Calculator();

        public static double Evaluate(string calculation)
        {
            return ParserRuntime.Run(calculation, evaluator).To<double>();
        }
        public static T Evaluate<T>(string calculation)
        {
            return ParserRuntime.Run(calculation, evaluator).To<T>();
        }
    }
}
