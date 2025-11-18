# Math Parser
A Mathemactical C# `Net 9.0` Parser

## Features
- Parsing input such as `"1+5"` -> `6`
- Using and Setting predefined variables like `pi` or `e`
- Using and Setting predefined functions such as `sin()`, `cos()`, `sqrt()`, ...

## How to use
Namespace
```csharp
using BenScr.Math.Parser;
```

- Option 1 - Simple
```csharp
// Variant 1- Default
double result = Calculator.Evaluate("20 + 100 / 2 * 4^2");
Console.WriteLine(result); // Output: 820

// Variant 2 - Custom Type
int result = Calculator.Evaluate<int>("20 + 100 / 2 * 4^2");
Console.WriteLine(result); // Output: 820
```

- Option 2 - Configurable
```csharp
Evaluator evaluator = new Evaluator();

// Defines +, -, /, *, ^, ...
evaluator.DefineArithmetikOperations();

// Defines sin(), cos(), sqrt(), ...
evaluator.DefineMathematicalFunctions();

Value value = ParserRuntime.Run("20 + 100 / 2 * 4^2", evaluator);
Console.WriteLine(value); // Output: 820
```

## Example Project
An example Project using this Parser is [`AdvancedCalculator`](https://github.com/Ben-Scr/AdvancedCalculator)

## Support
[![Ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/benscr)
[![Linktree](https://img.shields.io/badge/Linktree-00C853?style=for-the-badge&logo=linktree&logoColor=white)](https://linktr.ee/benscr)
