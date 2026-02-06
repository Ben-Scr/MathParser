
namespace BenScr.Math.Parser
{
    public abstract record Expr;
    public record NumberExpr(double Value) : Expr;
    public record VarExpr(string Name) : Expr;
    public record UnaryExpr(TokenType Op, Expr Right) : Expr;
    public record MoneyExpr(Expr Left, string Lexeme) : Expr;
    public record BinaryExpr(TokenType Op, Expr Left, Expr Right) : Expr;
    public record CallExpr(string CallName, List<Expr> Args) : Expr;
    public record StringExpr(string Value) : Expr;
}