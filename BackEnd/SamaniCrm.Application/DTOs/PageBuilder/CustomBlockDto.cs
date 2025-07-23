using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.DTOs.PageBuilder;

public class CustomBlockDto
{

    public Guid? Id { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(5000)]
    public string? Icon { get; set; }
    [MaxLength(5000)]
    public string? Image { get; set; }

    [MaxLength(100)]
    public string? CategoryName { get; set; }

    public string? Data { get; set; }
    public bool? CanDelete { get; set; }
}
