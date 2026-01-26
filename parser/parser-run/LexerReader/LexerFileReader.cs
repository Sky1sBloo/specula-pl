using System.Text.Json;

namespace SpeculaSyntaxAnalyzer.LexerReader;

public class LexerFileReader
{
    public static LexerOutput ParseFile(string fileName)
    {
        string fileContent;
        using (var sr = new StreamReader(fileName))
        {
            fileContent = sr.ReadToEnd();
        }

        try
        {
            return ParseJson(fileContent);
        }
        catch (LexerReadException)
        {
            throw new LexerReadException($"Serialize failed on file: {fileName}. Invalid format");
        }
    }

    public static LexerOutput ParseJson(string json)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        LexerOutput? output = JsonSerializer.Deserialize<LexerOutput>(json, options);

        if (output == null)
        {
            throw new LexerReadException($"Serialize failed. Invalid format");
        }
        return output;
    }
}
