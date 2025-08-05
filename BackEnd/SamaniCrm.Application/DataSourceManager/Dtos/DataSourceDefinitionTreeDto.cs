using SamaniCrm.Domain.Entities.PageBuilderEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.DataSourceManager.Dtos;

public class DataSourceDefinitionTreeDto
{
    public required string Title { get; set; }
    public required DataSourceTypeEnum DataSourceType { get; set; }
    public string? Description { get; set; }

    public string? CacheKey { get; set; }

    public List<DataSourceFieldDto> Fields { get; set; } = new List<DataSourceFieldDto>();
}
