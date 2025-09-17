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
    public class MenuDTO
    {
        public Guid? Id { get; set; }
        public string? Title { get; set; }
        [MaxLength(300)]
        public string? Icon { get; set; }
        [MaxLength(2000)]
        public string? Url { get; set; }

        public int OrderIndex { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsSystem { get; set; } = false;


        public Guid? ParentId { get; set; }

        public MenuTargetEnum Target { get; set; } = MenuTargetEnum.Self;

        public List<MenuDTO> Children { get; set; } = [];

        public List<MenuTranslationsDTO>? Translations { get; set; }
    }

    public class MenuTranslationsDTO
    {
        public Guid MenuId { get; set; }
        public required string Culture { get; set; }
        public string Title { get; set; } = string.Empty;
    }
}
