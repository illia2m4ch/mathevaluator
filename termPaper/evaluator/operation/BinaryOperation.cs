namespace termPaper.evaluator.operation; 

public delegate TResult BinaryOperation<in TOperand, out TResult>(TOperand operand1, TOperand operand2);