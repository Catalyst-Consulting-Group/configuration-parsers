using System.Globalization;
using System.Text.Json;

using Microsoft.Extensions.Configuration;

namespace CatConsult.ConfigurationParsers;

/// <summary>
/// A configuration parser that accepts JSON input and returns a dictionary of .NET-formatted configuration values.
///
/// Heavily inspired by: https://github.com/aws/aws-dotnet-extensions-configuration/blob/master/src/Amazon.Extensions.Configuration.SystemsManager/Internal/JsonConfigurationParser.cs
/// </summary>
public class JsonConfigurationParser
{
    private readonly SortedDictionary<string, string?> _data = new(StringComparer.OrdinalIgnoreCase);
    private readonly Stack<string> _context = new();
    private string _currentPath = string.Empty;

    private JsonConfigurationParser() { }

    public static IDictionary<string, string?> Parse(string json)
    {
        using var document = JsonDocument.Parse(json);

        return Parse(document);
    }

    public static IDictionary<string, string?> Parse(Stream stream)
    {
        using var document = JsonDocument.Parse(stream);

        return Parse(document);
    }

    private static IDictionary<string, string?> Parse(JsonDocument document)
    {
        var root = document.RootElement;
        if (root.ValueKind is not JsonValueKind.Object)
        {
            throw new FormatException($"Expected a JSON object, got: {Enum.GetName(root.ValueKind)}");
        }

        var parser = new JsonConfigurationParser();
        parser.ParseElement(root);

        return parser._data;
    }

    private void ParseElement(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var property in element.EnumerateObject())
                {
                    PushContext(property.Name);
                    ParseElement(property.Value);
                    PopContext();
                }

                break;

            case JsonValueKind.Array:
                var i = 0;
                foreach (var property in element.EnumerateArray())
                {
                    PushContext(i.ToString(CultureInfo.InvariantCulture));
                    ParseElement(property);
                    PopContext();
                    i++;
                }

                break;

            case JsonValueKind.String:
            case JsonValueKind.Number:
            case JsonValueKind.True:
            case JsonValueKind.False:
                _data.TryAdd(_currentPath, element.ToString().ToLower());
                break;

            case JsonValueKind.Null:
                _data.TryAdd(_currentPath, null);
                break;
        }
    }

    private void PushContext(string context)
    {
        _context.Push(context);
        _currentPath = ConfigurationPath.Combine(_context.Reverse());
    }

    private void PopContext()
    {
        _context.Pop();
        _currentPath = ConfigurationPath.Combine(_context.Reverse());
    }
}
