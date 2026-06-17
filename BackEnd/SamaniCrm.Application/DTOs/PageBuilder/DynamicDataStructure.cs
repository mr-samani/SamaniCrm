
namespace SamaniCrm.Application.DTOs.PageBuilder;

public class DynamicDataStructure
{
    public required string Name { get; set; }
    public required string DisplayName { get; set; }
    public DynamicValueType Type { get; set; }
    public object? Value { get; set; }
}


