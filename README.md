# Parser
A Configureable Mathemactical C# `Net 9.0` Parser

## Features
- Parsing input such as `"1+5"` -> `6`
- Using predefined variables like `pi` or `e`
- Using predefined functions such as `sin()`, `cos()`, `sqrt()`, ...

## How to use
```csharp
using BenScr.MathParser;
```

- Option 1
```csharp
string result = Calculator.Evaluate<string>("20 + 100 / 2 * 4^2");
Console.WriteLine(result); // Output: 820
```

- Option 2 (configurable)
```csharp
Evaluator evaluator = new Evaluator();

// Defines +, -, /, *, ^, ...
evaluator.DefineArithmetikOperations();

// Defines sin(), cos(), sqrt(), ...
evaluator.DefineMathematicalFunctions();

Value value = ParserRuntime.Run("20 + 100 / 2 * 4^2", evaluator);
Console.WriteLine(value); // Output: 820
```

