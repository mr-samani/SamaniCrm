using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SamaniCrm.Domain.Entities;

namespace SamaniCrm.Application.DTOs
{
    public class MenuDTO
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Is Uniquie for localization key
        /// </summary>
        public required string Code { get; set; }
        public string Title { get; set; } = string.Empty;
        [MaxLength(300)]
        public string? Icon { get; set; }
        [MaxLength(2000)]
        public string? Url { get; set; }

        public int OrderIndex { get; set; }

        public bool IsActive { get; set; } = true;

        public Guid? ParentId { get; set; }

        [MaxLength(100)]
        public MenuTargetEnum Target { get; set; } = MenuTargetEnum.Self;

        public List<MenuDTO> Children { get; set; } = [];
    }
}
