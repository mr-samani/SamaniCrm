using System.ComponentModel.DataAnnotations;

namespace SamaniCrm.Domain.Entities;

public class ProductFile : BaseEntity
{
    public Guid ProductId { get; set; }

    public Guid FileId { get; set; } 
    [MaxLength(1000)]
    public string? Description { get; set; } 

    public virtual Product Product { get; set; } = default!;
    public virtual FileFolder File { get; set; } = default!;


}
