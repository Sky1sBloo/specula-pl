using System.Text.Json.Serialization;
namespace SpeculaSyntaxAnalyzer.LexerReader;

public class LexerError
{
    public int CharPos { get; set; }
    public int Line { get; set; }
    public string Message { get; set; } = "Unknown";
}

public class LexerFileInfo
{
    public required string Name { get; set; }
    public required string Type { get; set; }
}

public class LexerOutput
{
    [JsonPropertyName("file")]
    public required LexerFileInfo FileInfo { get; set; }
    public List<LexerError> Errors { get; set; } = new();
    public List<Token> Tokens { get; set; } = new();
}
