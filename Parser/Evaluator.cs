
namespace Parser
{
    public sealed class Evaluator
    {
        private readonly Dictionary<string, Func<Value[], Value>> funcs = new();
        private readonly Dictionary<string, Value> vars = new();

        public void Define(string name, Func<Value[], Value> f) => funcs[name] = f;
        public void SetVar(string name, Value value) => vars[name] = value;

        public Evaluator()
        {
            Define("help", a => Value.From("Help at https://github.com/rrainix/Parser"));
            Define("switch", a => Value.Null);
        }

        public static Evaluator Calculator()
        {
            Evaluator ev = new Evaluator();

            // arithmetik
            ev.Define("+", a => Value.From(a[0].AsNumber() + a[1].AsNumber()));
            ev.Define("-", a => Value.From(a[0].AsNumber() - a[1].AsNumber()));
            ev.Define("*", a => Value.From(a[0].AsNumber() * a[1].AsNumber()));
            ev.Define("/", a => Value.From(a[0].AsNumber() / a[1].AsNumber()));
            ev.Define("^", a => Value.From(Math.Pow(a[0].AsNumber(), a[1].AsNumber())));

            // mathematic
            ev.Define("max", a => Value.From(Math.Max(a[0].AsNumber(), a[1].AsNumber())));
            ev.Define("min", a => Value.From(Math.Min(a[0].AsNumber(), a[1].AsNumber())));
            ev.Define("pow", a => Value.From(Math.Pow(a[0].AsNumber(), a[1].AsNumber())));
            ev.Define("sin", a => Value.From(Math.Sin(a[0].AsNumber())));
            ev.Define("cos", a => Value.From(Math.Cos(a[0].AsNumber())));
            ev.Define("abs", a => Value.From(Math.Abs(a[0].AsNumber())));
            ev.Define("atan", a => Value.From(Math.Atan(a[0].AsNumber())));
            ev.Define("atan2", a => Value.From(Math.Atan2(a[0].AsNumber(), a[1].AsNumber())));
            ev.Define("clamp", a => Value.From(Math.Clamp(a[0].AsNumber(), a[1].AsNumber(), a[2].AsNumber())));
            ev.Define("sqrt", a => Value.From(Math.Sqrt(a[0].AsNumber())));

            return ev;
        }
        public static Evaluator Default()
        {
            Evaluator ev = new Evaluator();

            ev.Define("writeLine", a => { Console.WriteLine(string.Join(" ", a.Select(x => x.ToString()))); return Value.Null; });
            ev.Define("calculate", a =>
            {
                var src = a[0].Type == Value.Kind.String ? a[0].String : a[0].ToString();
                var inner = ParserRuntime.Run(src, ev);
                return inner ?? Value.Null;
            });

            return ev;
        }

        public Value EvalToValue(Expr e) => e switch
        {
            NumberExpr n => Value.From(n.Value),
            StringExpr s => Value.From(s.Value),
            VarExpr v => vars.TryGetValue(v.Name, out var vv) ? vv : throw new Exception($"Unknown variable {v.Name}"),

            UnaryExpr u when u.Op == TokenType.Minus => Invoke("neg", EvalToValue(u.Right)),
            UnaryExpr u when u.Op == TokenType.Plus => Invoke("pos", EvalToValue(u.Right)),

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
                throw new Exception($"Unknown function {name}({string.Join(", ", args.Select(a => a.Type))})");
            }

            return f(args);
        }
    }
}
