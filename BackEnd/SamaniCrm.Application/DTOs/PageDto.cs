using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Application.DTOs
{
    public class PageDto
    {
        public Guid Id { get; set; }
        public string? Slag { get; set; }
        public PageStatusEnum Status { get; set; }
        public string? Author { get; set; }
        public DateTime Created { get; set; }
        public required string Title { get; set; }
        public string? Abstract { get; set; }
        public string? Description { get; set; }
    }
}
