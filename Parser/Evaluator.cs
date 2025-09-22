
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
            SetVar("repos", Value.From("https://github.com/rrainix/Parser"));
            SetVar("app", Value.From(Path.GetFullPath("Parser.exe")));
            Define("run", a =>
            {
                var psi = new ProcessStartInfo
                {
                    FileName = a[0].ToString(),
                    UseShellExecute = true // wichtig für .NET 5+ bei "normalem" EXE-Start
                };
                Process.Start(psi);
                return Value.Null;
            });
            Define("shutdown", a =>
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "shutdown",
                    Arguments = $"/s /t {(a.Length > 0 ? a[0].ToInt() : "15")}",
                    UseShellExecute = true
                });
                return Value.Null;
            });
            Define("define", a =>
            {
                string defineName = a[0].ToString();
                string defineValue = a[1].ToString();

                if (double.TryParse(defineValue, out double result))
                {
                    if (result % 1 == 0)
                        SetVar(defineName, Value.From((int)result));
                    else
                        SetVar(defineName, Value.From(result));

                    return Value.From($"Succesfully defined variable {a[0].ToString()} with value {result}");
                }
                else
                {
                    Define(defineName, b => Value.From(defineValue));
                    return Value.From("Succesfully defined function " + a[0].ToString());
                }
            });
            Define("functions", a =>
            {
                string funcsInfo = string.Empty;
                foreach (string name in funcs.Keys) funcsInfo += name + ": " + funcs[name].Method.ToString() + '\n';
                return Value.From(funcsInfo);
            });
            Define("quit", a => { Environment.Exit(0); return Value.Null; });
        }

        public void DefineMathematicalFunctions()
        {
            SetVar("π", Value.From(Math.PI));
            SetVar("e", Value.From(Math.E));
            Define("max", a => Value.From(MathCS.Max(MiniLinq.As(a, value => value.ToFloat()))));
            Define("min", a => Value.From(MathCS.Min(MiniLinq.As(a, value => value.ToFloat()))));
            Define("pow", a => Value.From(Math.Pow(a[0].ToDouble(), a[1].ToDouble())));
            Define("sin", a => Value.From(Math.Sin(a[0].ToDouble())));
            Define("cos", a => Value.From(Math.Cos(a[0].ToDouble())));
            Define("tan", a => Value.From(Math.Tan(a[0].ToDouble())));
            Define("log", a => Value.From(Math.Log(a[0].ToDouble())));
            Define("abs", a => Value.From(Math.Abs(a[0].ToDouble())));
            Define("atan", a => Value.From(Math.Atan(a[0].ToDouble())));
            Define("atan2", a => Value.From(Math.Atan2(a[0].ToDouble(), a[1].ToDouble())));
            Define("clamp", a => Value.From(Math.Clamp(a[0].ToDouble(), a[1].ToDouble(), a[2].ToDouble())));
            Define("sqrt", a => Value.From(Math.Sqrt(a[0].ToDouble())));


            Define("random", a =>
            {
                if (a.Length == 2)
                {
                    double min = a[0].ToDouble(), max = a[1].ToDouble();
                    bool ints = (min % 1 == 0) && (max % 1 == 0) && min >= int.MinValue && max <= int.MaxValue;

                    if (ints) return Value.From(RandomHandler.Range((int)min, (int)max));
                    return Value.From(min + RandomHandler.Range(min, max));
                }
                return Value.From(RandomHandler.NextDouble());
            });
            Define("flip", a => Value.From(RandomHandler.FlipCoin()));
            Define("chance", a => Value.From(RandomHandler.Chance((float)a[0].ToDouble(), (float)a[1].ToDouble())));
            Define("setSeed", a => { RandomHandler.SetSeed(a[0].ToUlong()); return Value.Null; });
        }
        public void DefineArithmetikOperations()
        {
            Define("+", a => Value.From(a[0].ToDouble() + a[1].ToDouble()));
            Define("-", a => Value.From(a[0].ToDouble() - a[1].ToDouble()));
            Define("*", a => Value.From(a[0].ToDouble() * a[1].ToDouble()));
            Define("/", a => Value.From(a[0].ToDouble() / a[1].ToDouble()));
            Define("^", a => Value.From(Math.Pow(a[0].ToDouble(), a[1].ToDouble())));

            Define("√", a => Value.From(Math.Sqrt(a[0].ToDouble())));
            Define("neg", a => Value.From(-a[0].ToDouble()));
            Define("pos", a => Value.From(+a[0].ToDouble()));
        }
        public void DefineTimeFunctions()
        {
            Define("time", a => Value.From(DateTime.Now.TimeOfDay.ToString()));
            Define("date", a => Value.From(DateTime.Now.ToShortDateString()));
            Define("year", a => Value.From(DateTime.Now.Year));
            Define("day", a => Value.From(DateTime.Now.Day));
            Define("month", a => Value.From(DateTime.Now.Month.ToString()));

            Define("ticks", a => Value.From(DateTime.Now.Ticks));
            Define("second", a => Value.From(DateTime.Now.Second));
            Define("hour", a => Value.From(DateTime.Now.Hour));
            Define("minute", a => Value.From(DateTime.Now.Minute));
        }
        public void DefineBasicFunctions()
        {
            Define("writeLine", a => { Console.WriteLine(string.Join(" ", a.Select(x => x.ToString()))); return Value.Null; });
            Define("calculate", a =>
            {
                var src = a[0].Type == Value.Kind.String ? a[0].String : a[0].ToString();
                var inner = ParserRuntime.Run(src, this);
                return inner ?? Value.Null;
            });
            Define("fileSize", a => Value.From(Filemanager.GetFileSize(a[0].ToString(), MemoryUnit.KiloByte) + " kb"));
            Define("error", a => { Logger.Error("", a[0].ToString()); return Value.Null; });
            Define("save", a => { SaveManager.Save<string>(a[0].ToString(), a[1].ToString()); return Value.Null; });
            Define("toHex", a => Value.From(TextUtils.ToHex(MiniLinq.As<Value, int>(a, value => value.ToInt()))));
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
            NumberExpr n => Value.From(n.Value),
            StringExpr s => Value.From(s.Value),
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
                throw new Exception($"Unknown function {name}({string.Join(", ", args.Select(a => a.Type))})");
            }

            return f(args);
        }
    }
}
