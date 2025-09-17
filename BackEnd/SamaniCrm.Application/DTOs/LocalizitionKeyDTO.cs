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
    public class LocalizationKeyDTO
    {
        public Guid? Id { get; set; }
        public required string Culture { get; set; }
        public required string Key { get; set; }
        public string Value { get; set; } = string.Empty;
        public required LocalizationCategoryEnum Category { get; set; } = LocalizationCategoryEnum.App;

    }
}
