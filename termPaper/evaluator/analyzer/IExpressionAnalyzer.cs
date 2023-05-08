namespace termPaper.evaluator.analyzer; 

public interface IExpressionAnalyzer {
    
    Stack<IUnit> Parse();
    
    interface IUnit { }
    
}