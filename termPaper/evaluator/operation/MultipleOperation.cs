namespace termPaper.evaluator.operation; 

public delegate TResult MultipleOperation<in TOperand, out TResult>(TOperand[] operands);