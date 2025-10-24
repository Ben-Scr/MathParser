
using System.Diagnostics;
using UtilityCS;

namespace Parser
{
    public sealed class Evaluator
    {
        private readonly Dictionary<string, Func<Value[], Value>> funcs = new();
        private readonly Dictionary<string, Value> vars = new();

        public void Define(string name, Func<Value[], Value> f) => funcs[name] = f; 
        public void SetVar(string name, Value value) => vars[name] = value;

        private Evaluator()
        {
            DefineDefaultFunctions();
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
            SetVar("repos", new Value("https://github.com/rrainix/Parser"));
            SetVar("app", new Value(Path.GetFullPath("Parser.exe")));

            Define("run", a =>
            {
                CmdHelper.Run(a[0].ToString());
                return Value.Null;
            });
            Define("shutdown", a =>
            {
                if (a.Length > 0)
                    CmdHelper.Shutdown(a[0].To<int>());
                else
                    CmdHelper.Shutdown();

                return new Value("Shutdown intialized");
            });
            Define("define", a =>
            {
                string defineName = a[0].ToString();
                string defineValue = a[1].ToString();

                if (double.TryParse(defineValue, out double result))
                {
                    if (result % 1 == 0)
                        SetVar(defineName, new Value((int)result));
                    else
                        SetVar(defineName, new Value(result));

                    return new Value($"Succesfully defined variable {a[0].ToString()} with value {result}");
                }
                else
                {
                    Define(defineName, b => new Value(defineValue));
                    return new Value("Succesfully defined function " + a[0].ToString());
                }
            });
            Define("functions", a=> {
                string funcsInfo = string.Empty;
                foreach (string name in funcs.Keys) funcsInfo += name + ": " + funcs[name].Method.ToString() + '\n';
                return new Value(funcsInfo);
            });
            Define("quit", a => { Environment.Exit(0); return Value.Null; });
            Define("exit", a => { Environment.Exit(0); return Value.Null; });
            Define("toHex", a => new Value(TextUtils.ToHex(MiniLinq.Select<Value, int>(a, value => value.To<int>()))));
        }

        public void DefineMathematicalFunctions()
        {
            SetVar("π", new Value(Math.PI));
            SetVar("pi", new Value(Math.PI));
            SetVar("e", new Value(Math.E));

            Define("max", a => new Value(MathCS.Max(MiniLinq.Select(a, value => value.To<float>()))));
            Define("min", a => new Value(MathCS.Min(MiniLinq.Select(a, value => value.To<float>()))));
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


            Define("random", a =>
            {
                if (a.Length == 2)
                {
                    double min = a[0].To<double>(), max = a[1].To<double>();
                    bool ints = (min % 1 == 0) && (max % 1 == 0) && min >= int.MinValue && max <= int.MaxValue;

                    if (ints) return new Value(RandomHandler.Range((int)min, (int)max));
                    return new Value(min + RandomHandler.Range(min, max));
                }
                return new Value(RandomHandler.NextDouble());
            });
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
        public void DefineTimeFunctions()
        {
            Define("time", a => new Value(DateTime.Now.TimeOfDay.ToString()));
            Define("date", a => new Value(DateTime.Now.ToShortDateString()));
            Define("year", a => new Value(DateTime.Now.Year));
            Define("day", a => new Value(DateTime.Now.Day));
            Define("month", a => new Value(DateTime.Now.Month.ToString()));

            Define("ticks", a => new Value(DateTime.Now.Ticks));
            Define("second", a => new Value(DateTime.Now.Second));
            Define("hour", a => new Value(DateTime.Now.Hour));
            Define("minute", a => new Value(DateTime.Now.Minute));
        }
        public void DefineBasicFunctions()
        {
            Define("writeLine", a => { Console.WriteLine(string.Join(" ", a.Select(x => x.ToString()))); return Value.Null; });
            Define("calculate", a =>
            {
                var src = a[0].ToString();
                var inner = ParserRuntime.Run(src, this);
                return inner ?? Value.Null;
            });
            Define("fileSize", a => new Value(Filemanager.GetFileSize(a[0].ToString(), MemoryUnit.KiloByte) + " kb"));
            Define("error", a => { Logger.Error("", a[0].ToString()); return Value.Null; });
            Define("save", a => { SaveManager.Save<string>(a[0].ToString(), a[1].ToString()); return Value.Null; });
        }

        public static Evaluator Basic()
        {
            Evaluator ev = new Evaluator();
            ev.DefineBasicFunctions();
            ev.DefineTimeFunctions();
            return ev;
        }

        public Value EvalToValue(Expr e) => e switch
        {
            NumberExpr n => new Value(n.Value),
            StringExpr s => new Value(s.Value),
            VarExpr v => vars.TryGetValue(v.Name, out var vv) ? vv : throw new Exception($"Unknown variable {v.Name}"),

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
                throw new Exception($"Unknown function {name}({string.Join(", ", args.Select(a => a))})");
            }

            return f(args);
        }
    }
}
