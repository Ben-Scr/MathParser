
namespace BenScr.Math.Parser
{
    // Utility Wrapper for the Object
    public readonly struct Value
    {
        public object Object { get; }

        public Value(object obj = default) => Object = obj;

        public static readonly Value Null = new Value(null);

        public T To<T>()
        {
            return (T)Convert.ChangeType(Object, typeof(T));
        }

        public override string ToString()
        {
            return Object?.ToString() ?? "null";
        }
    }
}
