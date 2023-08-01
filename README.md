# CatConsult.ConfigurationParsers

A collection of parsers for converting common configuration files to `Microsoft.Extensions.Configuration` format.

## JsonConfigurationParser

This parser accepts a string or stream representing a JSON root object and recursively parses all properties.

If you pass an array or anything that `System.Text.Json` can't handle, it will throw an exception.

## YamlConfigurationParser

This parser accepts a string or stream representing a YAML root object and recursively parses all properties.

If you pass an empty string or scalar string, or if you pass multiple documents, it will throw an exception.
