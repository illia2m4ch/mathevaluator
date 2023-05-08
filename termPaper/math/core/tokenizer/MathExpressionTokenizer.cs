using termPaper.evaluator.tokenizer;
using static termPaper.math.core.constants.MathConstants;

namespace termPaper.math.core.tokenizer;

public class MathExpressionTokenizer : IExpressionTokenizer {

    public const int TokenNumber = 0;
    public const int TokenIdentifier = 1;
    public const int TokenOpenScope = 2;
    public const int TokenFunction = 3;
    public const int TokenOperator = 4;
    public const int TokenArgumentSeparator = 5;
    public const int TokenCloseScope = 6;
    public const int TokenAnonymousArgument = 7;

    private readonly string _expression;
    private int _currentIndex;

    public MathExpressionTokenizer(string expression) {
        _expression = expression.Trim();
        _currentIndex = 0;
    }

    public IExpressionTokenizer.Token Next() {
        _skipEmpty();
        var symbol = _expression[_currentIndex];
        if (char.IsDigit(symbol)) return _nextNumber();
        if (symbol == Tokens.OpenBracket) return _nextOpenScope();
        if (symbol == Tokens.CloseBracket) return _nextCloseScope();
        if (symbol == Tokens.ArgumentSeparator) return _nextArgumentSeparator();
        if (symbol == Tokens.AnonymousArgument) return _nextAnonymousArgument();
        if (char.IsLetter(symbol)) return _nextIdentifier();

        return _nextOperator();
    }

    public bool HasNext() => _currentIndex != _expression.Length;

    private void _skipEmpty() {
        while (!_isLastIndex() && _isEmptySymbol(_expression[_currentIndex])) _currentIndex++;
    }
    
    private IExpressionTokenizer.Token _nextNumber() {
        var startIndex = _currentIndex;
        
        var pointIndex = -1;

        while (true) {
            if (_isLastIndex()) {
                ++_currentIndex;
                break;
            }
            
            var next = _expression[++_currentIndex];

            if (next == Tokens.FloatPoint) {
                if (pointIndex != -1) throw new Exception("Error parsing number");
                pointIndex = _currentIndex - startIndex;
            }
            else if (!char.IsDigit(next)) break;
        }

        var raw = _expression.ToCharArray(startIndex, _currentIndex - startIndex);
        if (pointIndex != -1) raw[pointIndex] = ',';

        return new IExpressionTokenizer.Token(TokenNumber, new string(raw));
    }

    private IExpressionTokenizer.Token _nextOpenScope() {
        return new IExpressionTokenizer.Token(TokenOpenScope, _expression[_currentIndex++].ToString());
    }
    
    private IExpressionTokenizer.Token _nextCloseScope() {
        return new IExpressionTokenizer.Token(TokenCloseScope, _expression[_currentIndex++].ToString());
    }
    
    private IExpressionTokenizer.Token _nextIdentifier() {
        var startIndex = _currentIndex;
        bool isLast;
        char next;
        do {
            isLast = _isLastIndex();
            if (isLast) {
                ++_currentIndex;
                break;
            }

            next = _expression[++_currentIndex];
        } while (char.IsLetter(next) || char.IsDigit(next));

        var raw = _expression.Substring(startIndex, _currentIndex - startIndex);

        if (isLast) return _nextIdentifier(raw);

        if (_expression[_currentIndex] == Tokens.OpenBracket) {
            _currentIndex++; // skip open bracket
            return _nextFunction(raw);
        }

        return _nextIdentifier(raw);
    }
    
    private IExpressionTokenizer.Token _nextIdentifier(string raw) {
        return new IExpressionTokenizer.Token(TokenIdentifier, raw);
    }
    
    private IExpressionTokenizer.Token _nextFunction(string raw) {
        return new IExpressionTokenizer.Token(TokenFunction, raw);
    }
    
    private IExpressionTokenizer.Token _nextOperator() {
        return new IExpressionTokenizer.Token(TokenOperator, _expression[_currentIndex++].ToString());
    }

    private IExpressionTokenizer.Token _nextArgumentSeparator() {
        return new IExpressionTokenizer.Token(TokenArgumentSeparator, _expression[_currentIndex++].ToString());
    }

    private IExpressionTokenizer.Token _nextAnonymousArgument() {
        var startIndex = _currentIndex;
        
        while (true) {
            if (_isLastIndex()) {
                ++_currentIndex;
                break;
            }
            
            var next = _expression[++_currentIndex];

            if (!char.IsDigit(next)) break;
        }

        startIndex++; // remove first $ sign
        var raw = _expression.Substring(startIndex, _currentIndex - startIndex);
        
        return new IExpressionTokenizer.Token(TokenAnonymousArgument, raw);
    }

    private static bool _isEmptySymbol(char symbol) => char.IsWhiteSpace(symbol);
    
    private bool _isLastIndex() => _currentIndex == _expression.Length - 1;

}