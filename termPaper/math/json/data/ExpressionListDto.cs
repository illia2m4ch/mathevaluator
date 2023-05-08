using System.Text.Json.Serialization;

namespace termPaper.math.json.data;

public class ExpressionListDto {
    [JsonPropertyName("define")]
    public List<DefinitionDto>? Definitions { get; }
    [JsonPropertyName("expressions")]
    public List<ExpressionDto> Expressions { get; }
    
    [JsonConstructor]
    public ExpressionListDto(List<DefinitionDto>? definitions, List<ExpressionDto> expressions) {
        Definitions = definitions;
        Expressions = expressions;
    }
}