using System.Collections;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using SpeculaSyntaxAnalyzer.LexerReader;
using SpeculaSyntaxAnalyzer.ParseTree;
using SpeculaSyntaxAnalyzer.SyntaxAnalyzer;
using SpeculaSyntaxAnalyzer.Controllers;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();
app.Run();

namespace SpeculaSyntaxAnalyzer.Controllers
{
    [ApiController]
    [Route("api")]
    public class SyntaxAnalyzerController : ControllerBase
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        [HttpPost("analyze")]
        public IActionResult Analyze([FromBody] LexerOutput input)
        {
            var errorHandler = new ErrorsHandler();
            foreach (var error in input.Errors)
            {
                errorHandler.AddError($"Lexer Error at {error.Line}:{error.CharPos}: {error.Message}");
            }

            JsonNode? rootSummary = null;

            if (input.Errors.Count == 0)
            {
                var analyzer = new SyntaxAnalyzerRoot(errorHandler);
                ParseNode? node = analyzer.ReadTokens(input.Tokens);
                if (node != null && errorHandler.ErrorList.Count == 0)
                {
                    rootSummary = ToJsonNode(node, JsonOptions);
                }
            }

            var result = new
            {
                fileInfo = input.FileInfo,
                errors = errorHandler.ErrorList,
                root = rootSummary
            };

            return Ok(result);
        }

        private static JsonNode? ToJsonNode(object? value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                return null;
            }

            switch (value)
            {
                case string s:
                    return JsonValue.Create(s);
                case bool b:
                    return JsonValue.Create(b);
                case byte or sbyte or short or ushort or int or uint or long or ulong or float or double or decimal:
                    return JsonValue.Create((ValueType)value);
                case Enum e:
                    return JsonValue.Create(e.ToString());
            }

            if (value is System.Collections.IEnumerable enumerable && value is not string)
            {
                JsonArray array = new();
                foreach (var item in enumerable)
                {
                    array.Add(ToJsonNode(item, options));
                }
                return array;
            }

            JsonObject obj = new();
            var naming = options.PropertyNamingPolicy;
            
            string typeName = value.GetType().Name;
            obj["$type"] = JsonValue.Create(typeName);
            
            foreach (PropertyInfo prop in value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!prop.CanRead) continue;
                string propName = naming?.ConvertName(prop.Name) ?? prop.Name;
                obj[propName] = ToJsonNode(prop.GetValue(value), options);
            }
            return obj;
        }
    }
}

