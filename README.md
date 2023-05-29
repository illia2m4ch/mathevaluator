# Math Evaluator

C# Mathematical expression parser and evaluator

## Supported Operators

|Operator|Name|Example|Result|
|:---:|:---:|:---:|:---:|
|**+**| Addition | 2 + 2 | 4 |
|**-**| Subtraction | 5 - 3 | 2 |
|**\***| Multiplication | 4 * 3 | 12 |
|**/**| Division | 10 / 5 | 2 |
|**^**| Exponentiation | 3 * 2 | 9 |
|**!**| Factorial | 4! | 24 |
|**√**| Square Root | √25 | 5 |

## Features
* Basic math operations
* Brackets
* Functions with different number of operators
* Custom functions
* Custom values (f. e. a = 5)
* Lambda expressions (f. e. $0 + $1)
* Implicit multiplication (f. e. 2pi, 3√5, ...)


## Usage
```csharp
var expression1 = MathExpression.Evaluate("3! + 3 * (1+6)"); // 27

var expression2 = MathExpression.Evaluate("min(2, sin(√e ^ 2!))"); // 0,4107812905029084

UnaryOperation<double, double> powByTwo = o => Math.Pow(o, 2);
BinaryOperation<double, double> hypotenuse = (o1, o2) => Math.Sqrt(Math.Pow(o1, 2) + Math.Pow(o2, 2));
MultipleOperation<double, double> count = operands => operands.Length;

var expression3 = MathExpression.Evaluate(
    expression: "powByTwo(√a + 3!) * hypotenuse(b, c) + count(e, pi, 1)",
    define: expression => {
        expression.Define("a", 3);
        expression.Define("b", 4);
        expression.Define("c", 5);
        expression.Define("powByTwo", powByTwo);
        expression.Define("hypotenuse", hypotenuse);
        expression.Define("count", count);
    } 
); // 385,80828333679403
```
