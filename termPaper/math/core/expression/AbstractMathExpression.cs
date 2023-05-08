using termPaper.evaluator.core;
using termPaper.evaluator.operation;
using termPaper.math.core.analyzer;
using termPaper.math.core.constants;

namespace termPaper.math.core.expression; 

public abstract class AbstractMathExpression<TType> : IExpression<TType> {

    private readonly IExpression<TType>? _parent;

    private readonly Dictionary<string, object> _functions = new();

    private readonly Dictionary<string, TType> _values = new();

    private readonly MathExpressionAnalyzer _analyzer;

    public readonly string Expression;
    
    public AbstractMathExpression(string expression, IExpression<TType>? parent = null) {
        _parent = parent;
        Expression = expression;
        _analyzer = new MathExpressionAnalyzer(expression);
    }

    public void Define(string name, UnaryOperation<TType, TType> value) {
        _functions[name] = value;
    }
    
    public void Define(string name, BinaryOperation<TType, TType> value) {
        _functions[name] = value;
    }
    
    public void Define(string name, MultipleOperation<TType, TType> value) {
        _functions[name] = value;
    }

    public void Define(string name, string value) {
        _functions[name] = CreateExpression(value);
    }

    public void Define(string name, TType value) {
        _values[name] = value;
    }

    public TType Evaluate(params TType[] arguments) {
        var stack = _analyzer.Parse();
        var numbers = new Stack<TType>();

        while (stack.Count != 0) {
            var unit = stack.Pop();
            switch (unit) {
                case MathExpressionAnalyzer.Operand operand: {
                    numbers.Push(ParseOperand(operand.Value));
                    break;
                }
                case MathExpressionAnalyzer.Identifier identifier: {
                    numbers.Push(GetValue(identifier.Value));
                    break;
                }
                case MathExpressionAnalyzer.Argument argument: {
                    var index = Convert.ToInt32(argument.Value);
                    if (index > arguments.Length - 1) throw new Exception($"Argument index {index} out of bounds");
                    numbers.Push(arguments[index]);
                    break;
                }
                case MathExpressionAnalyzer.Operation operation: {
                    var functionName = operation.Value;
                    var function = GetFunction(functionName);
                    
                    switch (function) {
                        case UnaryOperation<TType, TType> unary: {
                            if (operation.ArgumentCount != 1) _throwIllegalArgumentException(functionName, unary);
                            var operand = numbers.Pop();
                            numbers.Push(unary.Invoke(operand));
                            break;
                        }
                        case BinaryOperation<TType, TType> binary: {
                            if (operation.ArgumentCount != 2) _throwIllegalArgumentException(functionName, binary);
                            var operand2 = numbers.Pop();
                            var operand1 = numbers.Pop();
                            numbers.Push(binary.Invoke(operand1, operand2));
                            break;
                        }
                        case MultipleOperation<TType, TType> multiple: {
                            var count = operation.ArgumentCount;
                            var args = new List<TType>();
                            while (count-- != 0) args.Add(numbers.Pop());
                            args.Reverse();
                            numbers.Push(multiple.Invoke(args.ToArray()));
                            break;
                        }
                        case IExpression<TType> expression: {
                            var count = operation.ArgumentCount;
                            var args = new List<TType>();
                            while (count-- != 0) args.Add(numbers.Pop());
                            args.Reverse();
                            numbers.Push(expression.Evaluate(args.ToArray()));
                            break;
                        }
                        default: throw new Exception();
                    }
                    break;
                }
            }
        }
        
        return numbers.Pop();
    }

    public bool HasValue(string name) {
        if (_values.ContainsKey(name)) return true;
        if (_parent == null) return false;

        return _parent.HasValue(name);
    }

    public TType GetValue(string name) {
        if (_values.TryGetValue(name, out var value)) return value;
        if (_parent == null) throw new Exception($"Unknown value {value}");

        return _parent.GetValue(name);
    }

    public bool HasFunction(string name) {
        if (_functions.ContainsKey(name)) return true;
        if (_parent == null) return false;

        return _parent.HasFunction(name);
    }

    public object GetFunction(string name) {
        if (_functions.TryGetValue(name, out var function)) return function;
        if (_parent == null) throw new Exception($"Unknown function {function}");

        return _parent.GetFunction(name);
    }

    protected static void _throwIllegalArgumentException(string name, object operation) {
        throw new Exception($"Illegal argument count {name} in function {operation}");
    }

    protected abstract TType ParseOperand(string value);

    protected abstract IExpression<TType> CreateExpression(string value);

}