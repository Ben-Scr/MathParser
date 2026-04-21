
namespace BenScr.Math.Parser
{
    public enum TokenType
    {
        /// <summary>
        /// A numeric literal
        /// </summary>
        Number,

        /// <summary>
        /// A string literal defined with double quotes
        /// </summary>
        String,

        /// <summary>
        /// An identifier such as a variable or function name
        /// </summary>
        Ident,

        /// <summary>
        /// Addition operator +
        /// </summary>
        Plus,

        /// <summary>
        /// Subtraction operator -
        /// </summary>
        Minus,

        /// <summary>
        /// Exponentiation operator ^
        /// </summary>
        Caret,

        /// <summary>
        /// Multiplication operator * or x
        /// </summary>
        Star,

        /// <summary>
        /// Division operator / or :
        /// </summary>
        Slash,

        /// <summary>
        /// Modulo operator %
        /// </summary>
        Modulo,

        /// <summary>
        /// Left Parenthesis (
        /// </summary>
        LParen,

        /// <summary>
        /// Right Parenthesis )
        /// </summary>
        RParen,

        /// <summary>
        /// Separator for function arguments ;.
        /// </summary>
        Seperator,

        /// <summary>
        /// Separator for function arguments ;.
        /// </summary>
        Separator = Seperator,

        /// <summary>
        /// Sqrt operator √
        /// </summary>
        Sqrt,

        /// <summary>
        /// Indicates whether a value uses a currency symbol like $, €
        /// </summary>
        Currency,

        /// <summary>
        /// Indicates the end of the input 
        /// </summary>
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
