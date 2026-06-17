
namespace SamaniCrm.Application.DTOs.PageBuilder;

public class DynamicDataFilterOptions
{
    public required string Id { get; set; }
    public required string Title { get; set; }
    public DynamicDataInputType Type { get; set; }
    public string? Value { get; set; }

}


