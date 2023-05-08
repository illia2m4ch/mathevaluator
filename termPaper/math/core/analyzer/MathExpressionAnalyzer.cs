using termPaper.evaluator.analyzer;
using termPaper.evaluator.tokenizer;
using termPaper.math.core.tokenizer;
using static termPaper.math.core.constants.MathConstants;

namespace termPaper.math.core.analyzer;

public class MathExpressionAnalyzer : IExpressionAnalyzer {

    private const int LowPriority = 1;
    private const int MediumPriority = 2;
    private const int HighPriority = 3;
    private const int HighestPriority = 4;
    private const int SquareRootPriority = 5;
    private const int FunctionPriority = 6;
    
    private static readonly Dictionary<string, OperationInfo> OperatorInfos = new() {
        { Operators.Addition, new OperationInfo(LowPriority, Functions.Addition, 2) },
        { Operators.Subtraction, new OperationInfo(LowPriority, Functions.Subtraction, 2) },
        { Operators.Multiplication, new OperationInfo(MediumPriority, Functions.Multiplication, 2) },
        { Operators.Division, new OperationInfo(MediumPriority, Functions.Division, 2) },
        { Operators.Exponentiation, new OperationInfo(HighPriority, Functions.Exponentiation, 2) },
        { Operators.Factorial, new OperationInfo(HighestPriority, Functions.Factorial, 1) },
        { Operators.SquareRoot, new OperationInfo(SquareRootPriority, Functions.SquareRoot, 1) },
        { Operators.OpenScope, new OperationInfo(FunctionPriority, Functions.Scope, 1) }
    };
    
    private static readonly OperationInfo OperationNegative = new(HighestPriority, Functions.Negative, 1);
    private static readonly OperationInfo OperationImplicitMultiplication = new(FunctionPriority, Functions.Multiplication, argumentCount: 2);
    
    private readonly string _expression;
    
    // Temp
    private readonly Stack<IExpressionAnalyzer.IUnit> _stack = new();
    private readonly Stack<OperationInfo> _operations = new();
    private bool _isCompletedState; // operand | closed scope | factorial

    public MathExpressionAnalyzer(string expression) {
        _expression = expression.Trim();
    }
    
    public Stack<IExpressionAnalyzer.IUnit> Parse() {
        if (_stack.Count != 0) return _stack;
        var tokenizer = new MathExpressionTokenizer(_expression);
    
        while (tokenizer.HasNext()) {
            var token = tokenizer.Next();

            switch (token.Type) {
                case MathExpressionTokenizer.TokenNumber or MathExpressionTokenizer.TokenIdentifier or 
                    MathExpressionTokenizer.TokenAnonymousArgument: {
                    _pushOperand(token);
                    break;
                }
                case MathExpressionTokenizer.TokenOperator or MathExpressionTokenizer.TokenOpenScope: {
                    if (_isMinusSign(token.Raw)) _pushMinusSign();
                    else _pushOperator(token.Raw);
                    break;
                }
                case MathExpressionTokenizer.TokenFunction: {
                    _pushFunction(token.Raw);
                    break;
                }
                case MathExpressionTokenizer.TokenArgumentSeparator or MathExpressionTokenizer.TokenCloseScope: {
                    _flushScope(close: token.Type == MathExpressionTokenizer.TokenCloseScope);
                    break;
                }
            }

        }

        while (_operations.Count != 0) _flushTop();

        var result = new Stack<IExpressionAnalyzer.IUnit>();
        while (_stack.Count != 0) result.Push(_stack.Pop());
        if (!_isCompletedState) throw new Exception("Not completed expression");
        return result;
    }

    private void _pushOperand(IExpressionTokenizer.Token token) {
        if (_isCompletedState) _pushImplicitMultiplication();
        IOperand operand = token.Type switch {
            MathExpressionTokenizer.TokenNumber => new Operand(token.Raw),
            MathExpressionTokenizer.TokenIdentifier => new Identifier(token.Raw),
            MathExpressionTokenizer.TokenAnonymousArgument => new Argument(token.Raw),
            _ => throw new Exception("Illegal state exception")
        };
        _stack.Push(operand);
        _isCompletedState = true;
    }

    private void _pushFunction(string name) {
        if (_isCompletedState) _pushImplicitMultiplication();
        _operations.Push(new OperationInfo(FunctionPriority, name));
        _isCompletedState = false;
    }

    private void _pushOperator(string name) {
        var operation = OperatorInfos[name];
        
        if (operation.Priority > HighestPriority && _isCompletedState) _pushImplicitMultiplication();
        
        while (
            _operations.Count != 0 &&
            _operations.Peek().Priority != FunctionPriority &&
            _operations.Peek().Priority >= operation.Priority
        ) {
            _flushTop();
        }
        _operations.Push(operation);
        _isCompletedState = name == Operators.Factorial;
    }

    private void _flushScope(bool close = false) {
        while (_operations.Peek().Priority != FunctionPriority) _flushTop();
        _incrementArgumentCount();
        if (close) _flushTop();
        _isCompletedState = close;
    }

    private void _flushTop() {
        var operation = _operations.Pop();
        if (operation.Name != Functions.Scope) _stack.Push(new Operation(operation.Name, operation.ArgumentCount));
    }

    private void _incrementArgumentCount() {
        var lastFunction = _operations.Peek();
        if (!lastFunction.IsArgumentCountFixed) lastFunction.IncrementArgumentCount();
    }

    private void _pushImplicitMultiplication() {
        _operations.Push(OperationImplicitMultiplication);
        _isCompletedState = false;
    }

    private void _pushMinusSign() {
        _operations.Push(OperationNegative);
        _isCompletedState = false;
    }

    private bool _isMinusSign(string raw) {
        return raw.Equals(Operators.Negative) && !_isCompletedState;
    }

    private interface IOperand : IExpressionAnalyzer.IUnit { }
    
    public class Operand : IOperand {
        public readonly string Value;
        public Operand(string value) {
            Value = value;
        }
        public override string ToString() => Value;
    }
    
    public class Identifier : IOperand {
        public readonly string Value;
        public Identifier(string value) {
            Value = value;
        }
        public override string ToString() => Value;
    }
    
    public class Argument : IOperand {
        public readonly string Value;
        public Argument(string value) {
            Value = value;
        }
        public override string ToString() => $"${Value}";
    }
    
    public class Operation : IExpressionAnalyzer.IUnit {
        public readonly string Value;
        public readonly int ArgumentCount;
        public Operation(string value, int argumentCount) {
            Value = value;
            ArgumentCount = argumentCount;
        }
        public override string ToString() => Value + "{" + ArgumentCount + "}";
    }

    private class OperationInfo {
        public readonly int Priority;
        public readonly string Name;

        public int ArgumentCount { get; private set; }
        public readonly bool IsArgumentCountFixed;

        public OperationInfo(int priority, string name, int? argumentCount = null) {
            Priority = priority;
            Name = name;
            if (argumentCount == null) {
                IsArgumentCountFixed = false;
                ArgumentCount = 0;
            }
            else {
                IsArgumentCountFixed = true;
                ArgumentCount = argumentCount.Value;
            }
        }

        public void IncrementArgumentCount() {
            if (IsArgumentCountFixed) throw new Exception("Can't increment argument count, because it's fixed");
            ArgumentCount++;
        }

        public override bool Equals(object? obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Name == ((OperationInfo) obj).Name;
        }

        public override int GetHashCode() {
            return Name.GetHashCode();
        }
    }
}