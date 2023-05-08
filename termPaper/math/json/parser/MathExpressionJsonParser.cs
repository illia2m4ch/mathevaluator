using System.Text.Json;
using termPaper.math.basic;
using termPaper.math.json.data;

namespace termPaper.math.json.parser; 

public static class MathExpressionJsonParser {

    public static List<MathExpression> Parse(string path) {
        var raw = File.ReadAllText(path);
        var json = JsonSerializer.Deserialize<ExpressionListDto>(raw);
        return json.Expressions.ConvertAll(expressionDto => MathExpression.From(
            expression: expressionDto.Expression,
            define: expression => {
                json.Definitions?.ForEach(dto => expression.Define(dto.Name, dto.Value, dto.Function));
                expressionDto.Definitions?.ForEach(dto => expression.Define(dto.Name, dto.Value, dto.Function));
            }
        ));
    }

    private static void Define(this MathExpression expression, string name, double? value, string? function) {
        if (value != null) {
            expression.Define(name, value.Value);
            return;
        }
        if (function != null) {
            expression.Define(name, function);
            return;
        }
        throw new Exception("Definition is not specified");
    }

}