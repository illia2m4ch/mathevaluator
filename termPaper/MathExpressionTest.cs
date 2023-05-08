using termPaper.evaluator.operation;
using termPaper.math.basic;
using termPaper.test;

namespace termPaper; 

public class MathExpressionTest : BaseTest {

    public new static void Run() {
        ((BaseTest) new MathExpressionTest()).Run();
    }
    
    public MathExpressionTest() {
        _testAll();
    }
    
    private void _testAll() {
        _testSimple();
        _testBasicOperations();
        _testComplex();
        _testWithDefine();
        _testAnonymousArgument();
    }

    private void _testSimple() {
        _testExpectEquals(1, "1");
        _testExpectEquals(Math.E, "e");
        _testExpectEquals(5, "$0", 5);
    }

    private void _testBasicOperations() {
        _testExpectEquals(10 + 20, "10 + 20");
        _testExpectEquals(5 - 3, "5 - 3");
        _testExpectEquals(1.5 * 3, "1.5 * 3");
        _testExpectEquals(5.2 / 2, "5.2 / 2");
        _testExpectEquals(Math.Pow(3, 7), "3 ^ 7");
        _testExpectEquals(5 * 4 * 3 * 2 * 1, "5!");
        _testExpectEquals(Math.Sqrt(1234), "√1234");
    }

    private void _testComplex() {
        _testExpectEquals(
            expect: Math.Min(2, Math.Sin(Math.Pow(Math.Sqrt(Math.E), 2 * 1))),
            expression: "min(2, sin(√e ^ 2!))"
        );
        _testExpectEquals(
            expect: Math.Max(10, Math.Max(7 + 3, 3)) + 2 + Math.Min(1, 2) + 3 * 2 * 1,
            expression: "sum(max(10, 7 + 3, 3), 2, min(1, 2), 3!)"
        );
        _testExpectEquals(expect: -2 * (-3 + -(1 + 2)) * -Math.Sqrt(4) / -(3 * 2 * 1),
            expression: "-2 * (-3 + -(1+2)) * -sqrt(4) / -(3!)"
        );
        _testExpectEquals(expect: -2 * Math.Pow(-2, -(-2 - 1)),
            expression: "-2pow(-2, -(-2 - 1))"
        );
        _testExpectEquals(expect: 15.0/(7-(1+1))*3-(2+(1+1))*15.0/(7-(200+1))*3-(2+(1+1))*(15.0/(7-(1+1))*3-(2+(1+1))+15.0/(7-(1+1))*3-(2+(1+1))),
            expression: "15/(7-(1+1))*3-(2+(1+1))*15/(7-(200+1))*3-(2+(1+1))*(15/(7-(1+1))*3-(2+(1+1))+15/(7-(1+1))*3-(2+(1+1)))");
        _testExpectEquals(expect: 6.0/(2*(1+2)),
            expression: "6/2(1+2)"
        );
    }

    private void _testWithDefine() {
        var a = 3;
        var b = 4;
        var c = 5;
        
        UnaryOperation<double, double> powByTwo = o => Math.Pow(o, 2);
        BinaryOperation<double, double> hypotenuse = (o1, o2) => Math.Sqrt(Math.Pow(o1, 2) + Math.Pow(o2, 2));
        MultipleOperation<double, double> count = operands => operands.Length;

        _testExpectEquals(
            expect: powByTwo(Math.Sqrt(a) + 3 * 2 * 1) * hypotenuse(b, c) + count(new []{Math.E, Math.PI, 1}),
            expression: "powByTwo(√a + 3!) * hypotenuse(b, c) + count(e, pi, 1)",
            define: expression => {
                expression.Define("a", 3);
                expression.Define("b", 4);
                expression.Define("c", 5);
                expression.Define("powByTwo", powByTwo);
                expression.Define("hypotenuse", hypotenuse);
                expression.Define("count", count);
            }
        );
    }

    private void _testAnonymousArgument() {
        _testExpectEquals(expect: 1 + 2,
            expression: "$0 + $1", 1, 2
        );
        _testExpectEquals(expect: 5 * 5,
            expression: "$0 * $0", 5
        );
        
        _testExpectEquals(expect: Math.Sqrt(Math.Pow(3, 2) + Math.Pow(4, 2)),
            expression: "hypotenuse(3, 4)",
            define: expression => {
                expression.Define("hypotenuse", "√($0 ^ 2 + $1 ^ 2)");
            }
        );
    }

    private void _testExpectEquals(
        double expect,
        string expression,
        Action<MathExpression>? define,
        params double[] arguments
    ) {
        Test(
            name: expression,
            action: () => AssertEquals(expect, MathExpression.Evaluate(expression, define, arguments))
        );
    }
    
    private void _testExpectEquals(double expect, string expression, params double[] arguments) {
        _testExpectEquals(expect, expression, null, arguments);
    }
    
    private void _markedTestExpectEquals(double expect, string expression, Action<MathExpression>? define, params double[] arguments) {
        Test(name: "!" + expression, action: () => AssertEquals(expect, MathExpression.Evaluate(expression, define, arguments)));
    }
    
    private void _markedTestExpectEquals(double expect, string expression, params double[] arguments) {
        _markedTestExpectEquals(expect, expression, null, arguments);
    }

}