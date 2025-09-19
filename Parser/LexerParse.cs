using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    public abstract record Expr;
    public record NumberExpr(double Value) : Expr;
    public record VarExpr(string Name) : Expr;
    public record UnaryExpr(TokenType Op, Expr Right) : Expr;
    public record BinaryExpr(TokenType Op, Expr Left, Expr Right) : Expr;
    public record CallExpr(string Callee, List<Expr> Args) : Expr;

    public sealed class Parser
    {
        readonly List<Token> t;
        int p;
        public Parser(List<Token> tokens) { t = tokens; }

        Token Peek() => t[p];
        Token Next() => t[p++];
        bool Match(TokenType tt) { if (Peek().Type == tt) { p++; return true; } return false; }
        Token Expect(TokenType tt) { var x = Next(); if (x.Type != tt) throw new Exception($"Erwartet {tt} bei {x.Pos}"); return x; }

        public Expr ParseExpression(int rbp = 0)
        {
            var tok = Next();
            var left = Nud(tok);
            while (rbp < Lbp(Peek().Type))
            {
                tok = Next();
                left = Led(tok, left);
            }
            return left;
        }

        Expr Nud(Token tok)
        {
            return tok.Type switch
            {
                TokenType.Number => new NumberExpr(double.Parse(tok.Lexeme, CultureInfo.InvariantCulture)),
                TokenType.Ident => ParseIdentOrCall(tok),
                TokenType.LParen => ParseGrouping(),
                TokenType.Plus => new UnaryExpr(TokenType.Plus, ParseExpression(70)),
                TokenType.Minus => new UnaryExpr(TokenType.Minus, ParseExpression(70)),
                _ => throw new Exception($"Unerwartetes Token {tok.Type} bei {tok.Pos}")
            };
        }

        Expr ParseGrouping()
        {
            var e = ParseExpression();
            Expect(TokenType.RParen);
            return e;
        }

        Expr ParseIdentOrCall(Token ident)
        {
            if (Match(TokenType.LParen))
            {
                var args = new List<Expr>();
                if (!Match(TokenType.RParen))
                {
                    while (true)
                    {
                        args.Add(ParseExpression());
                        if (Match(TokenType.RParen)) break;
                        Expect(TokenType.Comma);
                    }
                }
                return new CallExpr(ident.Lexeme, args);
            }
            return new VarExpr(ident.Lexeme);
        }

        int Lbp(TokenType t) => t switch
        {
            TokenType.Plus or TokenType.Minus => 10,
            TokenType.Star or TokenType.Slash => 20,
            _ => 0
        };

        Expr Led(Token tok, Expr left) => tok.Type switch
        {
            TokenType.Plus => new BinaryExpr(TokenType.Plus, left, ParseExpression(10)),
            TokenType.Minus => new BinaryExpr(TokenType.Minus, left, ParseExpression(10)),
            TokenType.Star => new BinaryExpr(TokenType.Star, left, ParseExpression(20)),
            TokenType.Slash => new BinaryExpr(TokenType.Slash, left, ParseExpression(20)),
            _ => throw new Exception($"Unerwarteter Operator {tok.Type} bei {tok.Pos}")
        };
    }

    public sealed class Evaluator
    {
        readonly Dictionary<string, Func<double[], double>> funcs = new();
        readonly Dictionary<string, double> vars = new();

        public void Define(string name, Func<double[], double> f) => funcs[name] = f;
        public void SetVar(string name, double value) => vars[name] = value;

        public double Eval(Expr e) => e switch
        {
            NumberExpr n => n.Value,
            VarExpr v => vars.TryGetValue(v.Name, out var val) ? val : throw new Exception($"Unbekannte Variable {v.Name}"),
            UnaryExpr u when u.Op == TokenType.Minus => -Eval(u.Right),
            UnaryExpr u when u.Op == TokenType.Plus => +Eval(u.Right),
            BinaryExpr b when b.Op == TokenType.Plus => Eval(b.Left) + Eval(b.Right),
            BinaryExpr b when b.Op == TokenType.Minus => Eval(b.Left) - Eval(b.Right),
            BinaryExpr b when b.Op == TokenType.Star => Eval(b.Left) * Eval(b.Right),
            BinaryExpr b when b.Op == TokenType.Slash => Eval(b.Left) / Eval(b.Right),
            CallExpr c => Call(c),
            _ => throw new Exception("Nicht auswertbar")
        };

        double Call(CallExpr c)
        {
            if (!funcs.TryGetValue(c.Callee, out var f)) throw new Exception($"Unbekannte Funktion {c.Callee}");
            var args = new double[c.Args.Count];
            for (int i = 0; i < args.Length; i++) args[i] = Eval(c.Args[i]);
            return f(args);
        }
    }

    public static class MiniLangRuntime
    {
        public static double Run(string src, Evaluator ev)
        {
            var tokens = Lexer.Scan(src);
            var parser = new Parser(tokens);
            var expr = parser.ParseExpression();
            return ev.Eval(expr);
        }
    }
}
