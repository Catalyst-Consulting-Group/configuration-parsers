using YamlDotNet.RepresentationModel;

namespace CatConsult.ConfigurationParsers;

/// <summary>
/// A configuration parser that accepts YAML input and returns a dictionary of .NET-formatted configuration values.
/// </summary>
public class YamlConfigurationParser : ConfigurationParser
{
    private YamlConfigurationParser() { }

    public static IDictionary<string, string?> Parse(string yaml)
    {
        using var reader = new StringReader(yaml);
        var yamlStream = new YamlStream();
        yamlStream.Load(reader);

        return Parse(yamlStream);
    }

    public static IDictionary<string, string?> Parse(Stream stream)
    {
        using var reader = new StreamReader(stream);
        var yamlStream = new YamlStream();
        yamlStream.Load(reader);

        return Parse(yamlStream);
    }

    private static IDictionary<string, string?> Parse(YamlStream stream)
    {
        if (stream.Documents.Count != 1)
        {
            throw new FormatException($"Expected 1 YAML document, got: {stream.Documents.Count}");
        }

        var root = stream.Documents.First().RootNode;
        if (root is not YamlMappingNode)
        {
            throw new FormatException("Expected the root node to be a YAML object");
        }

        var parser = new YamlConfigurationParser();
        parser.ParseNode(root);

        return parser.Data;
    }

    private void ParseNode(YamlNode node)
    {
        switch (node)
        {
            case YamlMappingNode mapping:
                foreach (var item in mapping)
                {
                    var key = item.Key;
                    var value = item.Value;

                    PushContext(key.ToString());
                    ParseNode(value);
                    PopContext();
                }

                break;

            case YamlSequenceNode sequence:
                var i = 0;
                foreach (var property in sequence.Children)
                {
                    PushContext(i);
                    ParseNode(property);
                    PopContext();
                    i++;
                }

                break;

            case YamlScalarNode scalar:
                SetValue(scalar.Value);
                break;
        }
    }
}
