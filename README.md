# Parser
A Mathemactical C# Parser

## Features
- Parsing input such as "1+5" -> 6
- Using predefined variables like pi or e
- Using predefined functions such as sin(), cos(), sqrt(), ...

## How to use
```csharp
using BenScr.MathParser;
```

- Option 1
```csharp
string input = Console.ReadLine();
string result = Calculator.Evaluate<string>(input);
```

- Option 2 (Complex)
```csharp
Evaluator evaluator = Evaluator.Calculator();
string input = Console.ReadLine();
Value value = ParserRuntime.Run(input, evaluator);
```
