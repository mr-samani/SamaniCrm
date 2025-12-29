using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Core.Shared.Enums;
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
        public string? Introduction { get; set; }
        public string? Description { get; set; }


        public PageTypeEnum Type { get; set; }
        public bool IsActive { get; set; }
        public bool IsSystem { get; set; }
        public string? CoverImage { get; set; }
        public required string Culture { get; set; }   
        [MaxLength(2000)]
        public string? MetaDescription { get; set; }
        [MaxLength(2000)]
        public string? MetaKeywords { get; set; }
        public string? Data { get; set; }
        public string? Styles { get; set; }
        public string? Scripts { get; set; }
        public string? Html { get; set; }
    }
}
