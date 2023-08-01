using System.Text;
using System.Text.Json;

namespace CatConsult.ConfigurationParsers.Tests;

public class JsonConfigurationParserTests
{
    [Fact]
    public void Parse_Parses_Valid_Json_Object_String()
    {
        var json = File.ReadAllText("fixtures/object.json");
        var data = JsonConfigurationParser.Parse(json);

        ValidateObject(data);
    }

    private static void ValidateObject(IDictionary<string, string?> data)
    {
        data.Should().Contain("Object:String", "string");
        data.Should().Contain("Object:Number", "1");
        data.Should().Contain("Object:True", "true");
        data.Should().Contain("Object:False", "false");
        data.Should().Contain("Object:Null", null);

        data.Should().Contain("Object:NestedObject:String", "string");
        data.Should().Contain("Object:NestedObject:Number", "1");
        data.Should().Contain("Object:NestedObject:True", "true");
        data.Should().Contain("Object:NestedObject:False", "false");
        data.Should().Contain("Object:NestedObject:Null", null);

        data.Should().Contain("ArrayOfObject:0:ArrayObject1:String", "string");
        data.Should().Contain("ArrayOfObject:0:ArrayObject1:Number", "1");
        data.Should().Contain("ArrayOfObject:0:ArrayObject1:True", "true");
        data.Should().Contain("ArrayOfObject:0:ArrayObject1:False", "false");
        data.Should().Contain("ArrayOfObject:0:ArrayObject1:Null", null);

        data.Should().Contain("ArrayOfObject:1:ArrayObject2:String", "string");
        data.Should().Contain("ArrayOfObject:1:ArrayObject2:Number", "1");
        data.Should().Contain("ArrayOfObject:1:ArrayObject2:True", "true");
        data.Should().Contain("ArrayOfObject:1:ArrayObject2:False", "false");
        data.Should().Contain("ArrayOfObject:1:ArrayObject2:Null", null);

        data.Should().Contain("ArrayOfString:0", "foobar");
        data.Should().Contain("ArrayOfString:1", "baz");
    }

    [Fact]
    public void Parse_Parses_Valid_Json_Object_Stream()
    {
        var json = File.OpenRead("fixtures/object.json");
        var data = JsonConfigurationParser.Parse(json);

        ValidateObject(data);
    }

    [Fact]
    public void Parse_Throws_On_Json_Array_String()
    {
        var json = File.ReadAllText("fixtures/array.json");

        var act = () => JsonConfigurationParser.Parse(json);

        act.Should().Throw<FormatException>()
            .WithMessage("Expected a JSON object, got: Array");
    }

    [Fact]
    public void Parse_Throws_On_Json_Array_Stream()
    {
        var json = File.OpenRead("fixtures/array.json");

        var act = () => JsonConfigurationParser.Parse(json);

        act.Should().Throw<FormatException>()
            .WithMessage("Expected a JSON object, got: Array");
    }

    [Fact]
    public void Parse_Throws_On_Invalid_Json_String()
    {
        const string json = "definitely not json";

        var act = () => JsonConfigurationParser.Parse(json);

        act.Should().Throw<JsonException>();
    }


    [Fact]
    public void Parse_Throws_On_Invalid_Json_Stream()
    {
        var bytes = Encoding.UTF8.GetBytes("definitely not json");
        var json = new MemoryStream(bytes);

        var act = () => JsonConfigurationParser.Parse(json);

        act.Should().Throw<JsonException>();
    }
}
