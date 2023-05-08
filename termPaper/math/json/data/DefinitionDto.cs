using System.Text.Json.Serialization;

namespace termPaper.math.json.data; 

public class DefinitionDto {
    [JsonPropertyName("name")]
    public string Name { get; }
    [JsonPropertyName("value")]
    public double? Value { get; }
    [JsonPropertyName("function")]
    public string? Function { get; }

    [JsonConstructor]
    public DefinitionDto(string name, double? value, string? function) {
        Name = name;
        Value = value;
        Function = function;
    }
}