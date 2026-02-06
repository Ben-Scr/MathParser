
namespace BenScr.Math.Parser
{
    public static class Lexer
    {
        /// <summary>
        /// Floating point separator character, used when parsing numbers.
        /// </summary>
        public static char FloatingPointSeparator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
        public static HashSet<char> Currencies = new HashSet<char>() { '€', '$' };

        public static List<Token> BuildTokens(string source)
        {
            List<Token> tokens = new List<Token>();

            int i = 0;
            while (i < source.Length)
            {
                char c = source[i];
                if (char.IsWhiteSpace(c)) { i++; continue; }

                if (Currencies.Contains(c))
                {
                    tokens.Add(new Token(TokenType.Currency, c.ToString(), i));
                    i++;
                    continue;
                }

                if (char.IsDigit(c) || (c == FloatingPointSeparator && i + 1 < source.Length && char.IsDigit(source[i + 1])))
                {
                    int start = i;
                    bool dot = false;
                    while (i < source.Length)
                    {
                        char d = source[i];
                        if (d == FloatingPointSeparator)
                        {
                            if (dot) break;
                            dot = true;
                            i++;
                        }
                        else if (char.IsDigit(d)) i++;
                        else break;
                    }

                    tokens.Add(new Token(TokenType.Number, source.Substring(start, i - start), start));

                    if (i < source.Length && Currencies.Contains(source[i]))
                    {
                        tokens.Add(new Token(TokenType.Currency, source[i].ToString(), i));
                        i++;
                    }

                    continue;
                }

                if (c == '"')
                {
                    int start = ++i;
                    while (i < source.Length && source[i] != '"') i++;
                    if (i >= source.Length) throw new Exception($"Unterminated string at ({start - 1})");
                    var lex = source.Substring(start, i - start);
                    i++;
                    tokens.Add(new Token(TokenType.String, lex, start - 1));
                    continue;
                }
                if (char.IsLetter(c) || c == '_')
                {
                    int start = i;
                    i++;

                    while (i < source.Length && (char.IsLetterOrDigit(source[i]) || source[i] == '_')) i++;

                    string ident = source.Substring(start, i - start);
                    if (ident.Length > 1)
                    {
                        tokens.Add(new Token(TokenType.Ident, ident, start));
                        continue;
                    }
                }

                switch (c)
                {
                    case '+': tokens.Add(new Token(TokenType.Plus, "+", i)); i++; break;
                    case '-': tokens.Add(new Token(TokenType.Minus, "-", i)); i++; break;
                    case '*': tokens.Add(new Token(TokenType.Star, "*", i)); i++; break;
                    case '/': tokens.Add(new Token(TokenType.Slash, "/", i)); i++; break;
                    case '%': tokens.Add(new Token(TokenType.Modulo, "%", i)); i++; break;
                    case '^': tokens.Add(new Token(TokenType.Caret, "^", i)); i++; break;
                    case '(': tokens.Add(new Token(TokenType.LParen, "(", i)); i++; break;
                    case ')': tokens.Add(new Token(TokenType.RParen, ")", i)); i++; break;
                    case ';': tokens.Add(new Token(TokenType.Seperator, ";", i)); i++; break;
                    case '√': tokens.Add(new Token(TokenType.Sqrt, "√", i)); i++; break;
                    default: throw new Exception($"Unexpected character ({c}) at string index ({i})");
                }
            }
            tokens.Add(new Token(TokenType.EOF, "", source.Length));
            return tokens;
        }
    }
}