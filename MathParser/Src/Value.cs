
namespace BenScr.Math.Parser
{
    public readonly record struct CurrencyAmount(double Amount, string Symbol)
    {
        public override string ToString()
        {
            var formatted = Amount.ToString("N2");
            return $"{formatted}{Symbol}";
        }
    }

    // Utility Wrapper for the Object
    public readonly struct Value
    {
        public object Object { get; }

        public Value(object obj = default) => Object = obj;

        public static readonly Value Null = new Value(null);

        public T To<T>()
        {
            if (Object is CurrencyAmount currency)
            {
                if (typeof(T) == typeof(string))
                    return (T)(object)currency.ToString();

                return (T)Convert.ChangeType(currency.Amount, typeof(T));
            }

            return (T)Convert.ChangeType(Object, typeof(T));
        }

        public override string ToString()
        {
            if (Object is CurrencyAmount currency)
                return currency.ToString();

            return Object?.ToString() ?? "null";
        }
    }
}
