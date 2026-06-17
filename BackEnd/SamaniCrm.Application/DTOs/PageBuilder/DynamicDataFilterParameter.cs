
namespace SamaniCrm.Application.DTOs.PageBuilder;

public class DynamicDataFilterParameter
{
    public required string Name { get; set; }
    public required string Label { get; set; }
    public List<DynamicDataFilterOptions>? Options { get; set; }


}


