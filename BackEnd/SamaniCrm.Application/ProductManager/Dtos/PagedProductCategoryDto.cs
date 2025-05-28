using SamaniCrm.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.ProductManagerManager.Dtos
{
    public class PagedProductCategoryDto
    {
        public Guid? Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string? Slug { get; set; }
        public int OrderIndex { get; set; }
        public bool IsActive { get; set; }

        public bool hasChild { get; set; }
        public int ChildCount { get; set; }

        public DateTime CreationTime { get; set; }


        public Guid? ParentId { get; set; }


    }


}
