using termPaper.evaluator.operation;

namespace termPaper.math.basic; 

public static class MathOperations {

    /**
     * Basic
     */
    public static MultipleOperation<double, double> Addition() => operands => operands.Sum();

    public static BinaryOperation<double, double> Subtraction() => (first, second) => first - second;

    public static MultipleOperation<double, double> Multiplication() => operands => 
        operands.Aggregate(1.0, (f, s) => f * s);

    public static BinaryOperation<double, double> Division() => (first, second) => first / second;

    public static BinaryOperation<double, double> Exponentiation() => Math.Pow;

    public static UnaryOperation<double, double> Factorial() => operand => {
        if (operand < 0) throw new Exception("Negative factorial");
        var temp = Convert.ToInt32(operand);
        if (temp == 0) return 1;
        var result = temp;
        while (--temp != 0) result *= temp;
        return result;
    };

    public static UnaryOperation<double, double> SquareRoot() => Math.Sqrt;

    public static UnaryOperation<double, double> Negative() => operand => -operand;

    /**
     * Trigonometry
     */
    public static UnaryOperation<double, double> Sin() => Math.Sin;
    
    public static UnaryOperation<double, double> Cos() => Math.Cos;
    
    public static UnaryOperation<double, double> Tn() => Math.Tan;
    
    public static UnaryOperation<double, double> Ctg() => operand => 1.0 / Math.Tan(operand);

    /**
     * Standard functions
     */
    public static MultipleOperation<double, double> Min() => operands => operands.Min();

    public static MultipleOperation<double, double> Max() => operands => operands.Max();

    public static MultipleOperation<double, double> Avg() => operands => operands.Average();

    public static UnaryOperation<double, double> Abs() => Math.Abs;
    
}