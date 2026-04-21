# MathParser

A small C# math parser for expressions like `20 + 100 / 2 * 4^2`.

## Features

- Parses arithmetic expressions with operator precedence
- Supports functions like `pow`, `sin`, `cos`, `sqrt`, `clamp`
- Supports predefined variables like `pi`, `π`, `e`, and `ans`
- Supports currency expressions like `2$ + 5$`
- Accepts localized decimal separators such as `1,5` and `1.5`
- Offers a simple stateless API and a configurable evaluator API

## Installation

```bash
dotnet add package MathParserCS
```

## Simple Usage

```csharp
using BenScr.Math.Parser;

double result = Calculator.Evaluate("20 + 100 / 2 * 4^2");
Console.WriteLine(result); // 820

string currency = Calculator.Evaluate<string>("2$ + 5$");
Console.WriteLine(currency); // 7.00$ / 7,00$ depending on culture
```

`Calculator` is stateless, so each call is independent.

## Configurable Usage

```csharp
using BenScr.Math.Parser;

Evaluator evaluator = Evaluator.CreateCalculator();

if (ParserRuntime.TryRun("pow(4;2) + ans", evaluator, out Value value, out string? error))
{
    Console.WriteLine(value);
}
else
{
    Console.WriteLine(error);
}
```

Use a shared `Evaluator` when you want session-style behavior like `ans`.

## Supported Built-ins

- Variables: `ans`, `pi`, `π`, `e`
- Operators: `+`, `-`, `*`, `/`, `%`, `^`, unary `+`, unary `-`, `√`
- Functions: `max`, `min`, `pow`, `sin`, `cos`, `tan`, `log`, `abs`, `atan`, `atan2`, `clamp`, `sqrt`

Function arguments are separated with `;`, for example `pow(2;8)`.

## Error Handling

```csharp
using BenScr.Math.Parser;

Evaluator evaluator = Evaluator.CreateCalculator();
Value value = ParserRuntime.Run("1 +", evaluator);
Console.WriteLine(value); // Error: ...
```

If you prefer exceptions, use `ParserRuntime.RunOrThrow(...)`.

## Example Expressions

- `1 + 5` -> `6`
- `pow(4;2)` -> `16`
- `3 + 5 % 2` -> `4`
- `2$ + 5$` -> `7.00$` / `7,00$`
- `sqrt(81)` -> `9`

## Playground

The repository includes a small console playground in `Playground/Program.cs`.
