

namespace SamaniCrm.Application.DTOs.PageBuilder;

public class DynamicDataListDto
{

    public string Id { get => this.Name; }

    public required string Name { get; set; }
    public required string DisplayName { get; set; }
    public required DynamicValueType Type { get; set; }

    public List<DynamicDataStructure>? Fields { get; set; }

    public List<DynamicDataFilterParameter>? FilterParams { get; set; }
}


