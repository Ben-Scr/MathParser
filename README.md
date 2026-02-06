# Math Parser
A Mathemactical C# `Net 9.0` Parser

## Features
- Parsing inputs such as `"1 + 5"` = `6` or `"pow(4;2)"` = `16`
- Calculating with currencies `2$ + 5$` = `7.00$`
- Localized Floating Point Seperators
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
string result = Calculator.Evaluate<string>("20 + 100 / 2 * 4^2");
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

## Example Calculations

## Example Project
An example Project using this Parser is [`AdvancedCalculator`](https://github.com/Ben-Scr/AdvancedCalculator)
