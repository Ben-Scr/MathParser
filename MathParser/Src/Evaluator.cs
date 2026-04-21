
namespace BenScr.Math.Parser
{
    public sealed class Evaluator
    {
        private readonly Dictionary<string, Func<Value[], Value>> funcs = new();
        private readonly Dictionary<string, Value> vars = new();

        public void Define(string name, Func<Value[], Value> f)
        {
            ValidateName(name);
            ArgumentNullException.ThrowIfNull(f);
            funcs[name] = f;
        }

        public void Define(string name, int parameterCount, Func<Value[], Value> f)
        {
            if (parameterCount < 0)
                throw new ArgumentOutOfRangeException(nameof(parameterCount));

            Define(name, WrapExactArgumentCount(name, parameterCount, f));
        }

        public void DefineVariadic(string name, int minimumParameterCount, Func<Value[], Value> f)
        {
            if (minimumParameterCount < 0)
                throw new ArgumentOutOfRangeException(nameof(minimumParameterCount));

            Define(name, WrapMinimumArgumentCount(name, minimumParameterCount, f));
        }

        public void SetVariable(string name, Value value)
        {
            ValidateName(name);
            vars[name] = value;
        }

        /// <summary>
        /// Returns all defined variables
        /// </summary>
        public Dictionary<string, Value> GetVariables() => new(vars);

        /// <summary>
        /// Returns all defined functions
        /// </summary>
        public Dictionary<string, Func<Value[], Value>> GetFunctions() => new(funcs);


        public static Evaluator CreateCalculator()
        {
            Evaluator ev = new();

            ev.DefineArithmeticOperations();
            ev.DefineMathematicalFunctions();

            return ev;
        }

        public static Evaluator Calculator() => CreateCalculator();

        public void DefineDefaultFunctions()
        {
            DefineConsoleFunctions();
        }

        public void DefineConsoleFunctions()
        {
            SetVariable("app", new Value(Environment.ProcessPath ?? AppContext.BaseDirectory));
            Define("clear", a => { Console.Clear(); return new Value("Calculator"); });
            Define("quit", a => { Environment.Exit(0); return Value.Null; });
            Define("exit", a => { Environment.Exit(0); return Value.Null; });
        }

        public void DefineMathematicalFunctions()
        {
            SetVariable("ans", new Value(0d));
            SetVariable("π", new Value(System.Math.PI));
            SetVariable("pi", new Value(System.Math.PI));
            SetVariable("e", new Value(System.Math.E));

            DefineVariadic("max", 1, a => new Value(MathHelper.Max(a.Select(value => value.To<double>()).ToArray())));
            DefineVariadic("min", 1, a => new Value(MathHelper.Min(a.Select(value => value.To<double>()).ToArray())));
            Define("pow", 2, a => new Value(System.Math.Pow(a[0].To<double>(), a[1].To<double>())));
            Define("sin", 1, a => new Value(System.Math.Sin(a[0].To<double>())));
            Define("cos", 1, a => new Value(System.Math.Cos(a[0].To<double>())));
            Define("tan", 1, a => new Value(System.Math.Tan(a[0].To<double>())));
            Define("log", 1, a => new Value(System.Math.Log(a[0].To<double>())));
            Define("abs", 1, a => new Value(System.Math.Abs(a[0].To<double>())));
            Define("atan", 1, a => new Value(System.Math.Atan(a[0].To<double>())));
            Define("atan2", 2, a => new Value(System.Math.Atan2(a[0].To<double>(), a[1].To<double>())));
            Define("clamp", 3, a => new Value(System.Math.Clamp(a[0].To<double>(), a[1].To<double>(), a[2].To<double>())));
            Define("sqrt", 1, a => new Value(System.Math.Sqrt(a[0].To<double>())));
        }

        public void DefineArithmeticOperations()
        {
            Define("+", 2, a => new Value(a[0].To<double>() + a[1].To<double>()));
            Define("-", 2, a => new Value(a[0].To<double>() - a[1].To<double>()));
            Define("*", 2, a => new Value(a[0].To<double>() * a[1].To<double>()));
            Define("/", 2, a => new Value(a[0].To<double>() / a[1].To<double>()));
            Define("%", 2, a => new Value(a[0].To<double>() % a[1].To<double>()));
            Define("^", 2, a => new Value(System.Math.Pow(a[0].To<double>(), a[1].To<double>())));

            Define("√", 1, a => new Value(System.Math.Sqrt(a[0].To<double>())));
            Define("neg", 1, a => new Value(-a[0].To<double>()));
            Define("pos", 1, a => new Value(+a[0].To<double>()));
        }

        public void DefineArithmetikOperations() => DefineArithmeticOperations();

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
            BinaryExpr b when b.Op == TokenType.Modulo => Invoke("%", EvalToValue(b.Left), EvalToValue(b.Right)),
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

        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty.", nameof(name));
        }

        private static Func<Value[], Value> WrapExactArgumentCount(string name, int parameterCount, Func<Value[], Value> f)
        {
            ArgumentNullException.ThrowIfNull(f);

            return args =>
            {
                if (args.Length != parameterCount)
                    throw new ArgumentException($"Function \"{name}\" expects exactly {FormatArgumentCount(parameterCount)}, got {args.Length}.");

                return f(args);
            };
        }

        private static Func<Value[], Value> WrapMinimumArgumentCount(string name, int minimumParameterCount, Func<Value[], Value> f)
        {
            ArgumentNullException.ThrowIfNull(f);

            return args =>
            {
                if (args.Length < minimumParameterCount)
                    throw new ArgumentException($"Function \"{name}\" expects at least {FormatArgumentCount(minimumParameterCount)}, got {args.Length}.");

                return f(args);
            };
        }

        private static string FormatArgumentCount(int count)
        {
            return count == 1 ? "1 argument" : $"{count} arguments";
        }
    }
}
