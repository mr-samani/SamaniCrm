using SamaniCrm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.FileManager.Dtos;

public class FileNodeDto
{

    public Guid? Id { get; set; }
    public Guid? ParentId { get; set; }

    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string RelativePath { get; set; } = string.Empty;


    [MaxLength(100)]
    public string? Icon { get; set; }
    public bool IsFolder { get; set; }
    public bool IsPublic { get; set; }
    public bool IsStatic { get; set; }
    public string? Extension { get; set; }
    public string? ContentType { get; set; }
    public decimal? ByteSize { get; set; }
    public string? Thumbnails { get; set; }


    public List<FileNodeDto> Children { get; set; } = new(); 

    // Implementing IAuditableEntity properties
    public DateTime CreationTime { get; set; }

}
