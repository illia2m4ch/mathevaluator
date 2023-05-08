namespace termPaper.evaluator.operation; 

public delegate TResult UnaryOperation<in TOperand, out TResult>(TOperand operand);