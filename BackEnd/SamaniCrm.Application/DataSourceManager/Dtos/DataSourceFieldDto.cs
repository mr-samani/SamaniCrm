using SamaniCrm.Domain.Entities.PageBuilderEntities;

namespace SamaniCrm.Application.DataSourceManager.Dtos;

public class DataSourceFieldDto
{ 
    public required string NameSpace { get; set; }
    public required string Title { get; set; }
    public DataFieldTypeEnum Type { get; set; }

}