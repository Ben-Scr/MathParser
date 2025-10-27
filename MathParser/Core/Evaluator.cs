
namespace BenScr.MathParser
{
    public sealed class Evaluator
    {
        private readonly Dictionary<string, Func<Value[], Value>> funcs = new();
        private readonly Dictionary<string, Value> vars = new();

        public void Define(string name, Func<Value[], Value> f) => funcs[name] = f; 
        public void SetVar(string name, Value value) => vars[name] = value;

        public Evaluator()
        {

        }

        public static Evaluator Calculator()
        {
            Evaluator ev = new Evaluator();

            ev.DefineArithmetikOperations();
            ev.DefineMathematicalFunctions();

            return ev;
        }
        public void DefineDefaultFunctions()
        {
            SetVar("app", new Value(Path.GetFullPath("ParserPlayground.exe")));
            Define("quit", a => { Environment.Exit(0); return Value.Null; });
            Define("exit", a => { Environment.Exit(0); return Value.Null; });
        }

        public void DefineMathematicalFunctions()
        {
            SetVar("π", new Value(Math.PI));
            SetVar("pi", new Value(Math.PI));
            SetVar("e", new Value(Math.E));

            Define("max", a => new Value(MathHelper.Max(a.Select(value => value.To<float>()).ToArray())));
            Define("min", a => new Value(MathHelper.Min(a.Select(value => value.To<float>()).ToArray())));
            Define("pow", a => new Value(Math.Pow(a[0].To<double>(), a[1].To<double>())));
            Define("sin", a => new Value(Math.Sin(a[0].To<double>())));
            Define("cos", a => new Value(Math.Cos(a[0].To<double>())));
            Define("tan", a => new Value(Math.Tan(a[0].To<double>())));
            Define("log", a => new Value(Math.Log(a[0].To<double>())));
            Define("abs", a => new Value(Math.Abs(a[0].To<double>())));
            Define("atan", a => new Value(Math.Atan(a[0].To<double>())));
            Define("atan2", a => new Value(Math.Atan2(a[0].To<double>(), a[1].To<double>())));
            Define("clamp", a => new Value(Math.Clamp(a[0].To<double>(), a[1].To<double>(), a[2].To<double>())));
            Define("sqrt", a => new Value(Math.Sqrt(a[0].To<double>())));
        }
        public void DefineArithmetikOperations()
        {
            Define("+", a => new Value(a[0].To<double>() + a[1].To<double>()));
            Define("-", a => new Value(a[0].To<double>() - a[1].To<double>()));
            Define("*", a => new Value(a[0].To<double>() * a[1].To<double>()));
            Define("/", a => new Value(a[0].To<double>() / a[1].To<double>()));
            Define("^", a => new Value(Math.Pow(a[0].To<double>(), a[1].To<double>())));

            Define("√", a => new Value(Math.Sqrt(a[0].To<double>())));
            Define("neg", a => new Value(-a[0].To<double>()));
            Define("pos", a => new Value(+a[0].To<double>()));
        }
        internal Value EvalToValue(Expr e) => e switch
        {
            NumberExpr n => new Value(n.Value),
            StringExpr s => new Value(s.Value),
            VarExpr v => vars.TryGetValue(v.Name, out var vv) ? vv : throw new Exception($"Unknown variable \"{v.Name}\""),

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
        private Value Invoke(string name, params Value[] args)
        {
            if (!funcs.TryGetValue(name, out var f))
            {
                throw new Exception($"Unknown function \"{name}({string.Join(", ", args.Select(a => a))})\"");
            }

            return f(args);
        }
    }
}
