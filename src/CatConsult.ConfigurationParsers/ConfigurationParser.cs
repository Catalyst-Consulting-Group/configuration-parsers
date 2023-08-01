using System.Globalization;

using Microsoft.Extensions.Configuration;

namespace CatConsult.ConfigurationParsers;

public abstract class ConfigurationParser
{
    private readonly SortedDictionary<string, string?> _data = new(StringComparer.OrdinalIgnoreCase);
    private readonly Stack<string> _context = new();
    private string _currentPath = string.Empty;

    protected IDictionary<string, string?> Data => _data;

    protected void SetValue(string? value)
    {
        _data.TryAdd(_currentPath, value);
    }
    
    protected void PushContext(string context)
    {
        _context.Push(context);
        _currentPath = ConfigurationPath.Combine(_context.Reverse());
    }
    
    protected void PushContext(int i)
    {
        _context.Push(i.ToString(CultureInfo.InvariantCulture));
        _currentPath = ConfigurationPath.Combine(_context.Reverse());
    }

    protected void PopContext()
    {
        _context.Pop();
        _currentPath = ConfigurationPath.Combine(_context.Reverse());
    }
}
