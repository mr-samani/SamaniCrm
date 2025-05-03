using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Application.DTOs
{
    public class RolePermissionsDTO
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public bool Selected { get; set; } = false;
        public List<RolePermissionsDTO> Children { get; set; } = [];
        public string? DisplayName { get; set; }

        public RolePermissionsDTO GetOrAddChild(string name)
        {
            var existing = Children.FirstOrDefault(c => c.Name == name);
            if (existing != null) return existing;

            var newNode = new RolePermissionsDTO { Name = name };
            Children.Add(newNode);
            return newNode;
        }

    }

   

}
