using termPaper;
using termPaper.math.json.parser;

const string inputPath = @"D:\projects\c#\oaip\termPaper\termPaper\termPaper\input.json";

MathExpressionJsonParser.Parse(inputPath).ForEach(expression => {
    Console.WriteLine($"{expression.Expression} = {expression.Evaluate()}");
});

MathExpressionTest.Run();