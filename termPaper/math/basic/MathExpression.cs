using termPaper.evaluator.core;
using termPaper.math.core.expression;
using static termPaper.math.core.constants.MathConstants;

namespace termPaper.math.basic; 

public class MathExpression : AbstractMathExpression<double> {

    public static MathExpression From(string expression, Action<MathExpression>? define = null) {
        var mathExpression = new MathExpression(expression);
        define?.Invoke(mathExpression);
        return mathExpression;
    }
    
    public static double Evaluate(string expression, Action<MathExpression>? define, params double[] arguments) =>
        From(expression, define).Evaluate(arguments);
    
    public static double Evaluate(string expression, params double[] arguments) =>
        Evaluate(expression, null, arguments);
    
    public MathExpression(string expression, IExpression<double>? parent = null) : base(expression, parent) {
        // Basic
        Define(Functions.Addition, MathOperations.Addition());
        Define(Functions.Subtraction, MathOperations.Subtraction());
        Define(Functions.Multiplication, MathOperations.Multiplication());
        Define(Functions.Division, MathOperations.Division());
        Define(Functions.Exponentiation, MathOperations.Exponentiation());
        Define(Functions.Factorial, MathOperations.Factorial());
        Define(Functions.SquareRoot, MathOperations.SquareRoot());
        Define(Functions.Negative, MathOperations.Negative());
        
        // Trigonometry
        Define(Functions.Sin, MathOperations.Sin());
        Define(Functions.Cos, MathOperations.Cos());
        Define(Functions.Tn, MathOperations.Tn());
        Define(Functions.Ctg, MathOperations.Ctg());
        
        // Other
        Define(Functions.Min, MathOperations.Min());
        Define(Functions.Max, MathOperations.Max());
        Define(Functions.Avg, MathOperations.Avg());
        Define(Functions.Abs, MathOperations.Abs());
        
        // Constants
        Define(Values.E, Math.E);
        Define(Values.Pi, Math.PI);
        Define(Values.Tau, Math.Tau);
    }

    protected override double ParseOperand(string value) => Convert.ToDouble(value);

    protected override IExpression<double> CreateExpression(string value) => new MathExpression(value, this);
}