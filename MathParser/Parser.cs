using System.Globalization;

namespace BenScr.MathParser
{
    public class Parser
    {
        private readonly List<Token> tokens;
        private int pos;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        Token Peek() => tokens[pos];
        Token Next() => tokens[pos++];
        bool Match(TokenType tt) { if (Peek().Type == tt) { pos++; return true; } return false; }
        Token Expect(TokenType tt) { var x = Next(); if (x.Type != tt) throw new Exception($"Expected \"{tt}\" at \"{x.Pos}\""); return x; }

        // Note: rbp (right binding power) directs the calculation order/priority.
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


        // Note: Nud (Null Denotation) 
        // Processes operators/expressions that start on the left
        // Example: -2
        Expr Nud(Token tok)
        {
            return tok.Type switch
            {
                TokenType.Number => new NumberExpr(double.Parse(tok.Lexeme, CultureInfo.InvariantCulture)),
                TokenType.Ident => ParseIdentOrCall(tok),
                TokenType.LParen => ParseGrouping(),
                TokenType.Plus => new UnaryExpr(TokenType.Plus, ParseExpression(70)),
                TokenType.Minus => new UnaryExpr(TokenType.Minus, ParseExpression(70)),
                TokenType.Sqrt => new UnaryExpr(TokenType.Sqrt, ParseExpression(70)),
                TokenType.String => new StringExpr(tok.Lexeme),
                _ => throw new Exception($"Unexpected Token \"{tok.Type}\" at \"{tok.Pos}\"")
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

        // Note: Lbp (Left Binding Power)
        // Defines how strong an operator binds to the left.
        // Example: 1 + 2 * 3 
        // + = 10 (rpb)
        // * = 20
        // -> 2 * 3
        int Lbp(TokenType t)
        {
            switch (t)
            {
                case TokenType.Plus:
                case TokenType.Minus:
                    return 10;
                case TokenType.Star:
                case TokenType.Slash:
                    return 20;
                case TokenType.Caret:
                    return 30;
                default:
                    return 0;
            }
        }

        // Note: Led(Left Denotation)
        // Gets called when operator is between an left and right expression
        // The rpb of the right expression defines wether further operators
        // from right can connect.
        Expr Led(Token tok, Expr left)
        {
            switch (tok.Type)
            {
                case TokenType.Plus:
                    return new BinaryExpr(TokenType.Plus, left, ParseExpression(10));
                case TokenType.Minus:
                    return new BinaryExpr(TokenType.Minus, left, ParseExpression(10));
                case TokenType.Star:
                    return new BinaryExpr(TokenType.Star, left, ParseExpression(20));
                case TokenType.Slash:
                    return new BinaryExpr(TokenType.Slash, left, ParseExpression(20));
                case TokenType.Caret:
                    return new BinaryExpr(TokenType.Caret, left, ParseExpression(29));
                default:
                    throw new Exception($"Unexpected Operator \"{tok.Type}\" at \"{tok.Pos}\"");
            }
        }
    }

    public static class ParserRuntime
    {
        public static Value? Run(string src, Evaluator ev)
        {
            try
            {
                List<Token> tokens = Lexer.BuildTokens(src);
                Parser parser = new Parser(tokens);
                Expr expr = parser.ParseExpression();
                Value result = ev.EvalToValue(expr);
                return result;
            }
            catch(Exception e)
            {
                return new Value("Error: " + e.Message);
            }
        }
    }
}
