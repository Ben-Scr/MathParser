# Parser
A Mathemactical C# Parser

## Usage
- Parsing input such as "1+5" -> 6
- Using predefined variables like pi or e
- Using predefined functions such as sin(), cos(), sqrt(), ...

## How to use
```csharp
Evaluator evaluator = Evaluator.Calculator();
string input = Console.ReadLine();
Value? value = ParserRuntime.Run(input, evaluator);

// Possible Conversions
int iValue = value.To<int>();
string  sValue = value.To<string>();
```
