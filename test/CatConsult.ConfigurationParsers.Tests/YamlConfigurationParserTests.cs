using System.Text;

namespace CatConsult.ConfigurationParsers.Tests;

public class YamlConfigurationParserTests
{
    [Fact]
    public void Parse_Parses_Valid_Yaml_Object_String()
    {
        var yaml = File.ReadAllText("fixtures/object.yaml");
        var data = YamlConfigurationParser.Parse(yaml);

        ValidateObject(data);
    }
    
    private static void ValidateObject(IDictionary<string, string?> data)
    {
        data.Should().Contain("Object:String", "string");
        data.Should().Contain("Object:Number", "1");
        data.Should().Contain("Object:True", "true");
        data.Should().Contain("Object:False", "false");
        data.Should().Contain("Object:Null", string.Empty);

        data.Should().Contain("Object:NestedObject:String", "string");
        data.Should().Contain("Object:NestedObject:Number", "1");
        data.Should().Contain("Object:NestedObject:True", "true");
        data.Should().Contain("Object:NestedObject:False", "false");
        data.Should().Contain("Object:NestedObject:Null", string.Empty);

        data.Should().Contain("ArrayOfObject:0:ArrayObject1:String", "string");
        data.Should().Contain("ArrayOfObject:0:ArrayObject1:Number", "1");
        data.Should().Contain("ArrayOfObject:0:ArrayObject1:True", "true");
        data.Should().Contain("ArrayOfObject:0:ArrayObject1:False", "false");
        data.Should().Contain("ArrayOfObject:0:ArrayObject1:Null", string.Empty);

        data.Should().Contain("ArrayOfObject:1:ArrayObject2:String", "string");
        data.Should().Contain("ArrayOfObject:1:ArrayObject2:Number", "1");
        data.Should().Contain("ArrayOfObject:1:ArrayObject2:True", "true");
        data.Should().Contain("ArrayOfObject:1:ArrayObject2:False", "false");
        data.Should().Contain("ArrayOfObject:1:ArrayObject2:Null", string.Empty);

        data.Should().Contain("ArrayOfString:0", "foobar");
        data.Should().Contain("ArrayOfString:1", "baz");
    }
    
    [Fact]
    public void Parse_Parses_Valid_Yaml_Object_Stream()
    {
        var yaml = File.OpenRead("fixtures/object.yaml");
        var data = YamlConfigurationParser.Parse(yaml);

        ValidateObject(data);
    }

    [Fact]
    public void Parse_Returns_Empty_On_Yaml_Array_String()
    {
        var yaml = File.ReadAllText("fixtures/array.yaml");

        var act = () => YamlConfigurationParser.Parse(yaml);

        act.Should().Throw<FormatException>()
            .WithMessage("Expected the root node to be a YAML object");
    }

    [Fact]
    public void Parse_Returns_Empty_On_Yaml_Array_Stream()
    {
        var yaml = File.OpenRead("fixtures/array.yaml");

        var act = () => YamlConfigurationParser.Parse(yaml);

        act.Should().Throw<FormatException>()
            .WithMessage("Expected the root node to be a YAML object");
    }

    [Fact]
    public void Parse_Returns_Empty_On_Invalid_Yaml_String()
    {
        const string yaml = "definitely not yaml";

        var act = () => YamlConfigurationParser.Parse(yaml);

        act.Should().Throw<FormatException>()
            .WithMessage("Expected the root node to be a YAML object");
    }


    [Fact]
    public void Parse_Returns_Empty_On_Invalid_Yaml_Stream()
    {
        var bytes = Encoding.UTF8.GetBytes("definitely not yaml");
        var yaml = new MemoryStream(bytes);

        var act = () => YamlConfigurationParser.Parse(yaml);

        act.Should().Throw<FormatException>()
            .WithMessage("Expected the root node to be a YAML object");
    }
}