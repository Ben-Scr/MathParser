
namespace BenScr.Math.Parser
{
    public static class Calculator
    {
        public static double Evaluate(string calculation)
        {
            return RunCalculation(calculation).To<double>();
        }

        public static T Evaluate<T>(string calculation)
        {
            return RunCalculation(calculation).To<T>();
        }

        private static Value RunCalculation(string calculation)
        {
            ArgumentNullException.ThrowIfNull(calculation);
            if (string.IsNullOrWhiteSpace(calculation))
                throw new ArgumentException("Calculation is empty.", nameof(calculation));

            return ParserRuntime.RunOrThrow(calculation, Evaluator.CreateCalculator());
        }
    }
}
