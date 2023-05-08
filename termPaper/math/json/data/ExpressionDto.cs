using System.Text.Json.Serialization;

namespace termPaper.math.json.data; 

public class ExpressionDto {
    [JsonPropertyName("define")]
    public List<DefinitionDto>? Definitions { get; }
    [JsonPropertyName("expression")]
    public string Expression { get; }
    
    [JsonConstructor]
    public ExpressionDto(List<DefinitionDto>? definitions, string expression) {
        Definitions = definitions;
        Expression = expression;
    }
}