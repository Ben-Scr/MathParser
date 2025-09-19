using System.Globalization;

namespace Parser
{
    public readonly struct Value
    {
        public enum Kind { Number, String, Bool, Null, Object }
        public Kind Type { get; }
        public double Number { get; }
        public string String { get; }
        public bool Bool { get; }
        public object Object { get; }

        private Value(Kind t, double n = 0, string s = null, bool b = false, object o = null)
            => (Type, Number, String, Bool, Object) = (t, n, s, b, o);

        public static readonly Value Null = new Value(Kind.Null);
        public static Value From(double n) => new Value(Kind.Number, n);
        public static Value From(string s) => new Value(Kind.String, s: s);
        public static Value From(bool b) => new Value(Kind.Bool, b: b);
        public static Value FromObject(object o) => new Value(Kind.Object, o: o);

        public double AsNumber() =>
            Type == Kind.Number ? Number :
            Type == Kind.Bool ? (Bool ? 1 : 0) :
            throw new InvalidOperationException("Not a number");

        public override string ToString() => Type switch
        {
            Kind.Number => Number.ToString(CultureInfo.InvariantCulture),
            Kind.String => String,
            Kind.Bool => Bool.ToString(),
            Kind.Null => "null",
            Kind.Object => Object?.ToString() ?? "null",
            _ => "?"
        };
    }
}
