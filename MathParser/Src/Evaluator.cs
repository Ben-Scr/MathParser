
namespace BenScr.Math.Parser
{
    public sealed class Evaluator
    {
        private readonly Dictionary<string, Func<Value[], Value>> funcs = new();
        private readonly Dictionary<string, Value> vars = new();

        public void Define(string name, Func<Value[], Value> f) => funcs[name] = f;
        public void SetVariable(string name, Value value) => vars[name] = value;

        public Evaluator()
        {
            DefineDefaultFunctions();
        }

        /// <summary>
        /// Returns all defined variables
        /// </summary>
        public Dictionary<string, Value> GetVariables() => vars;

        /// <summary>
        /// Returns all defined functions
        /// </summary>
        public Dictionary<string, Func<Value[], Value>> GetFunctions() => funcs;


        public static Evaluator Calculator()
        {
            Evaluator ev = new Evaluator();

            ev.DefineDefaultFunctions();
            ev.DefineArithmetikOperations();
            ev.DefineMathematicalFunctions();

            return ev;
        }

        public void DefineDefaultFunctions()
        {
            SetVariable("app", new Value(Path.GetFullPath("ParserPlayground.exe")));
            Define("clear", a => { Console.Clear(); return new Value("Calculator"); });
            Define("quit", a => { Environment.Exit(0); return Value.Null; });
            Define("exit", a => { Environment.Exit(0); return Value.Null; });
        }

        public void DefineMathematicalFunctions()
        {
            SetVariable("ans", new Value(0));
            SetVariable("π", new Value(System.Math.PI));
            SetVariable("pi", new Value(System.Math.PI));
            SetVariable("e", new Value(System.Math.E));

            Define("max", a => new Value(MathHelper.Max(a.Select(value => value.To<float>()).ToArray())));
            Define("min", a => new Value(MathHelper.Min(a.Select(value => value.To<float>()).ToArray())));
            Define("pow", a => new Value(System.Math.Pow(a[0].To<double>(), a[1].To<double>())));
            Define("sin", a => new Value(System.Math.Sin(a[0].To<double>())));
            Define("cos", a => new Value(System.Math.Cos(a[0].To<double>())));
            Define("tan", a => new Value(System.Math.Tan(a[0].To<double>())));
            Define("log", a => new Value(System.Math.Log(a[0].To<double>())));
            Define("abs", a => new Value(System.Math.Abs(a[0].To<double>())));
            Define("atan", a => new Value(System.Math.Atan(a[0].To<double>())));
            Define("atan2", a => new Value(System.Math.Atan2(a[0].To<double>(), a[1].To<double>())));
            Define("clamp", a => new Value(System.Math.Clamp(a[0].To<double>(), a[1].To<double>(), a[2].To<double>())));
            Define("sqrt", a => new Value(System.Math.Sqrt(a[0].To<double>())));
        }
        public void DefineArithmetikOperations()
        {
            Define("+", a => new Value(a[0].To<double>() + a[1].To<double>()));
            Define("-", a => new Value(a[0].To<double>() - a[1].To<double>()));
            Define("*", a => new Value(a[0].To<double>() * a[1].To<double>()));
            Define("/", a => new Value(a[0].To<double>() / a[1].To<double>()));
            Define("^", a => new Value(System.Math.Pow(a[0].To<double>(), a[1].To<double>())));

            Define("√", a => new Value(System.Math.Sqrt(a[0].To<double>())));
            Define("neg", a => new Value(-a[0].To<double>()));
            Define("pos", a => new Value(+a[0].To<double>()));
        }
        internal Value EvalToValue(Expr e) => e switch
        {
            NumberExpr n => new Value(n.Value),
            StringExpr s => new Value(s.Value),
            VarExpr v => vars.TryGetValue(v.Name, out var vv) ? vv : throw new Exception($"Unknown variable \"{v.Name}\""),

            MoneyExpr m =>EvalCurrency(m),

            UnaryExpr u when u.Op == TokenType.Minus => Invoke("neg", EvalToValue(u.Right)),
            UnaryExpr u when u.Op == TokenType.Plus => Invoke("pos", EvalToValue(u.Right)),
            UnaryExpr u when u.Op == TokenType.Sqrt => Invoke("√", EvalToValue(u.Right)),

            BinaryExpr b when b.Op == TokenType.Plus => Invoke("+", EvalToValue(b.Left), EvalToValue(b.Right)),
            BinaryExpr b when b.Op == TokenType.Minus => Invoke("-", EvalToValue(b.Left), EvalToValue(b.Right)),
            BinaryExpr b when b.Op == TokenType.Star => Invoke("*", EvalToValue(b.Left), EvalToValue(b.Right)),
            BinaryExpr b when b.Op == TokenType.Slash => Invoke("/", EvalToValue(b.Left), EvalToValue(b.Right)),
            BinaryExpr b when b.Op == TokenType.Caret => Invoke("^", EvalToValue(b.Left), EvalToValue(b.Right)),

            CallExpr c => Invoke(c.CallName, c.Args.Select(EvalToValue).ToArray()),
            _ => throw new Exception("Not evaluable")
        };

        private Value EvalCurrency(MoneyExpr expr)
        {
            var left = EvalToValue(expr.Left);

            if (left.Object is CurrencyAmount currency)
                return new Value(currency with { Symbol = expr.Lexeme });

            return new Value(new CurrencyAmount(left.To<double>(), expr.Lexeme));
        }

        private static string? GetCurrencySymbol(params Value[] values)
        {
            foreach (Value value in values)
            {
                if (value.Object is CurrencyAmount currency)
                    return currency.Symbol;
            }

            return null;
        }

        private Value Invoke(string name, params Value[] args)
        {
            if (!funcs.TryGetValue(name, out var f))
                throw new Exception($"Unknown function \"{name}({string.Join(", ", args.Select(a => a))})\"");

            Value result = f(args);

            string? symbol = GetCurrencySymbol(args);
            if (symbol == null)
                return result;

            if (result.Object is CurrencyAmount currency)
                return new Value(currency with { Symbol = symbol });

            if (result.Object != null && IsNumericType(result.Object.GetType()))
                return new Value(new CurrencyAmount(result.To<double>(), symbol));

            return result;
        }

        private static bool IsNumericType(Type type)
        {
            TypeCode typeCode = Type.GetTypeCode(type);
            return typeCode == TypeCode.Byte
                || typeCode == TypeCode.SByte
                || typeCode == TypeCode.UInt16
                || typeCode == TypeCode.UInt32
                || typeCode == TypeCode.UInt64
                || typeCode == TypeCode.Int16
                || typeCode == TypeCode.Int32
                || typeCode == TypeCode.Int64
                || typeCode == TypeCode.Decimal
                || typeCode == TypeCode.Double
                || typeCode == TypeCode.Single;
        }
    }
}
