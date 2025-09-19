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

        public double ToDouble()
        {
            return Type == Kind.Number ? Number : Type == Kind.Bool ? (Bool ? 1 : 0) : throw new InvalidOperationException("Not a number");
        }
        public float ToFloat()
        {
            return Type switch
            {
                Value.Kind.Number => checked((float)Number), // 121.5 -> 121 (oder vorher runden)
                Value.Kind.Bool => Bool ? 1 : 0,
                Value.Kind.String => Convert.ToSingle(String, CultureInfo.InvariantCulture),
                Value.Kind.Object => (Object is IConvertible ic)
                    ? Convert.ToSingle(ic, CultureInfo.InvariantCulture)
                    : throw new InvalidCastException("Object not convertible to float"),
                _ => 0
            };
        }
        public int ToInt()
        {
            return Type switch
            {
                Value.Kind.Number => checked((int)Number), // 121.5 -> 121 (oder vorher runden)
                Value.Kind.Bool => Bool ? 1 : 0,
                Value.Kind.String => Convert.ToInt32(String, CultureInfo.InvariantCulture),
                Value.Kind.Object => (Object is IConvertible ic)
                    ? Convert.ToInt32(ic, CultureInfo.InvariantCulture)
                    : throw new InvalidCastException("Object not convertible to int"),
                _ => 0
            };
        }
        public ulong ToUlong()
        {
            return Type switch
            {
                Value.Kind.Number => checked((ulong)Number), // 121.5 -> 121 (oder vorher runden)
                Value.Kind.Bool => Bool ? 1UL : 0UL,
                Value.Kind.String => Convert.ToUInt64(String, CultureInfo.InvariantCulture),
                Value.Kind.Object => (Object is IConvertible ic)
                    ? Convert.ToUInt64(ic, CultureInfo.InvariantCulture)
                    : throw new InvalidCastException("Object not convertible to UInt64"),
                _ => 0UL
            };
        }

        public override string ToString()
        {
            switch (Type)
            {
                case Kind.Number: return Number.ToString(CultureInfo.InvariantCulture);
                case Kind.String: return String;
                case Kind.Bool: return Bool.ToString();
                case Kind.Null: return "null";
                case Kind.Object: return Object?.ToString() ?? "null";
                default: return "?";
            }
        }
    }
}
