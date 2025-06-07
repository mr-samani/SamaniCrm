using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.FileManager.Dtos;

public class FileNodeDto
{
    public string Name { get; set; } = "";
    public string FullPath { get; set; } = "";
    public bool IsFolder { get; set; }
    public List<FileNodeDto> Children { get; set; } = new();
}
