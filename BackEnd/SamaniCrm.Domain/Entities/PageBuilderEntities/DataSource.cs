using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities.PageBuilderEntities;

public class DataSource
{
    public Guid Id { get; set; }
    [MaxLength(100)]
    public required string Title { get; set; }
    public required DataSourceTypeEnum DataSourceType { get; set; }
    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(100)]
    public string? CacheKey { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual ICollection<DataSourceField> Fields { get; set; } = new List<DataSourceField>();
}





public enum DataSourceTypeEnum
{
    ProductCategories,
    Products,
    News,
    Articles,

}
