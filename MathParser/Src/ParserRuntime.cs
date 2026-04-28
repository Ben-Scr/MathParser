
namespace BenScr.MathParser
{
    public static class ParserRuntime
    {
        public static Value Run(string src, Evaluator ev)
        {
            return TryRun(src, ev, out Value result, out string? error)
                ? result
                : new Value("Error: " + error);
        }

        public static Value RunOrThrow(string src, Evaluator ev)
        {
            return RunCore(src, ev);
        }

        public static bool TryRun(string src, Evaluator ev, out Value result, out string? error)
        {
            try
            {
                result = RunCore(src, ev);
                error = null;
                return true;
            }
            catch (Exception e)
            {
                result = Value.Null;
                error = e.Message;
                return false;
            }
        }

        private static Value RunCore(string src, Evaluator ev)
        {
            ArgumentNullException.ThrowIfNull(src);
            ArgumentNullException.ThrowIfNull(ev);

            if (string.IsNullOrWhiteSpace(src))
                throw new ArgumentException("Input is empty.", nameof(src));

            List<Token> tokens = Lexer.BuildTokens(src);
            Parser parser = new Parser(tokens);
            Expr expr = parser.ParseExpression();
            parser.ExpectEndOfInput();
            Value result = ev.EvalToValue(expr);
            ev.SetVariable("ans", result);
            return result;
        }
    }
}
