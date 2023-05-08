namespace termPaper.evaluator.core;

public interface IExpression<TType> {
    TType Evaluate(params TType[] arguments);
    bool HasValue(string name);
    TType GetValue(string name);
    object GetFunction(string name);
    bool HasFunction(string name);
}