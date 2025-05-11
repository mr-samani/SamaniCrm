using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.DTOs
{
    public class RoleDTO
    {
        public Guid Id { get; set; }
        public required string RoleName { get; set; }
        public string? DisplayName { get; set; }
    }
}
