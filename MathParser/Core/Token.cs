
namespace BenScr.MathParser
{
    public enum TokenType
    {
        Number,
        String,
        Ident,
        Plus,
        Minus,
        Caret,
        Star,
        Slash,
        Modulo,
        LParen,
        RParen,
        Comma,
        Sqrt,
        EOF
    }

    public readonly struct Token
    {
        public readonly TokenType Type;
        public readonly string Lexeme;
        public readonly int Pos;

        public Token(TokenType type, string lexeme, int pos)
        {
            Type = type;
            Lexeme = lexeme;
            Pos = pos;
        }

        public override string ToString() => $"{Type}:{Lexeme}";
    }
}
