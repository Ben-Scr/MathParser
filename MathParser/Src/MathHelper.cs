
namespace BenScr.MathParser
{
    internal static class MathHelper
    {
        public static double Max(params double[] values)
        {
            if (values.Length == 0)
                throw new ArgumentException("At least one value is required.", nameof(values));

            double max = values[0];
            for (int i = 1; i < values.Length; i++)
            {
                if (values[i] > max)
                    max = values[i];
            }

            return max;
        }

        public static double Min(params double[] values)
        {
            if (values.Length == 0)
                throw new ArgumentException("At least one value is required.", nameof(values));

            double min = values[0];
            for (int i = 1; i < values.Length; i++)
            {
                if (values[i] < min)
                    min = values[i];
            }

            return min;
        }
    }
}
