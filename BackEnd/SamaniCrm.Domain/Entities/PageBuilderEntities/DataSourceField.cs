using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities.PageBuilderEntities;

public class DataSourceField
{
    public Guid Id { get; set; }
    [MaxLength(200)]
    public required string NameSpace { get; set; }

    [MaxLength(200)]
    public required string Title { get; set; }

    public DataFieldTypeEnum Type { get; set; }


    public virtual DataSource? DataSource { get; set; }
    public Guid DataSourceId { get; set; }
}




public enum DataFieldTypeEnum
{
    String,
    Int,
    Image,
    Date,
    DateTime,
    Time,
    ArrayString
}
